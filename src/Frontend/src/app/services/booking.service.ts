import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { CommonService } from './CommonService';
import { StorageManager } from '../utils/storage-manager';
import { BookingRequest, BookingResponse } from '../interfaces/booking';
import { ChallengeRequest, ChallengeResponse } from '../interfaces/challenge';
import { BookingEmployeeRequest, BookingEmployeeResponse } from '../interfaces/booking';

@Injectable({ providedIn: 'root' })
export class BookingService {
  private url = environment.apiUrl + '/api/booking';

  constructor(
    private http: HttpClient,
    private commonService: CommonService,
    private storageManager: StorageManager,
  ) {}

  getHttpHeaders(): HttpHeaders {
    const login = JSON.parse(this.storageManager.getLogin());
    const token = login ? login.token : '';

    return new HttpHeaders().set('Content-Type', 'application/json').set('Authorization', token);
  }

  addBooking(booking: BookingRequest): Observable<BookingResponse> {
    return this.http
      .post<BookingResponse>(this.url, booking, {
        headers: this.getHttpHeaders(),
      })
      .pipe(tap(), catchError(this.handleError<BookingResponse>('Add Booking')));
  }

  getBookingsByPharmacy(pharmacyId?: string): Observable<BookingResponse[]> {
    let url = this.url + '/by-pharmacy';
    if (pharmacyId) {
      url += `?pharmacyId=${pharmacyId}`;
    }
    return this.http
      .get<BookingResponse[]>(url, {
        headers: this.getHttpHeaders(),
      })
      .pipe(catchError(this.handleError<BookingResponse[]>('Get Bookings')));
  }

  approveBooking(
    bookingEmployeeRequest: BookingEmployeeRequest,
  ): Observable<BookingEmployeeResponse> {
    return this.http
      .post<BookingEmployeeResponse>(
        `${this.url}/${bookingEmployeeRequest.pharmacyId}/approve`,
        bookingEmployeeRequest,
        {
          headers: this.getHttpHeaders(),
        },
      )
      .pipe(catchError(this.handleError<BookingEmployeeResponse>('Approve Booking')));
  }

  cancelBooking(
    bookingEmployeeRequest: BookingEmployeeRequest,
  ): Observable<BookingEmployeeResponse> {
    return this.http
      .post<BookingEmployeeResponse>(
        `${this.url}/${bookingEmployeeRequest.pharmacyId}/cancel`,
        bookingEmployeeRequest,
        {
          headers: this.getHttpHeaders(),
        },
      )
      .pipe(catchError(this.handleError<BookingEmployeeResponse>('Cancel Booking')));
  }

  getBookings(email?: string): Observable<BookingResponse[]> {
    let params = new HttpParams();
    if (email !== undefined && email !== null) {
      params = params.set('email', email);
    }
    return this.http
      .get<BookingResponse[]>(this.url, { params, headers: this.getHttpHeaders() })
      .pipe(tap(), catchError(this.handleError<BookingResponse[]>('Get Bookings by Email')));
  }

  getBooking(id: number): Observable<BookingResponse> {
    return this.http
      .get<BookingResponse>(`${this.url}/${id}`, {
        headers: this.getHttpHeaders(),
      })
      .pipe(tap(), catchError(this.handleError<BookingResponse>('Get Booking')));
  }

  pickUpBooking(id: number): Observable<BookingResponse> {
    return this.http
      .put<BookingResponse>(
        `${this.url}/${id}/pick-up`,
        {},
        {
          headers: this.getHttpHeaders(),
        },
      )
      .pipe(tap(), catchError(this.handleError<BookingResponse>('Pick Up Booking')));
  }

  postChallenge(id: number) {
    return this.http
      .post<ChallengeResponse>(`${this.url}/${id}/challenge`, {})
      .pipe(
        tap(),
        catchError(this.handleError<ChallengeResponse>('Challenge for pick up booking')),
      );
  }

  confirmChallenge(id: number, challenge: string, signature: string) {
    return this.http
      .post<ChallengeResponse>(
        `${this.url}/${id}/challenge-confirm`,
        new ChallengeRequest(challenge, signature),
        {
          headers: this.getHttpHeaders(),
        },
      )
      .pipe(
        tap(),
        catchError(this.handleError<ChallengeResponse>('Confirm challenge for pick up booking')),
      );
  }

  /**
   * Handle Http operation that failed.
   * Let the app continue.
   *
   * @param operation - name of the operation that failed
   * @param result - optional value to return as the observable result
   */
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      // TODO: send the error to remote logging infrastructure
      //console.error(error); // log to console instead

      // TODO: better job of transforming error for user consumption
      this.log(`${operation} failed: ${error.error.message}`);

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }

  /** Log a DrugService error with the MessageService */
  private log(message: string) {
    this.commonService.updateToastData(message, 'danger', 'Error');
  }
}
