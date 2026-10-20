import { Component, OnInit } from '@angular/core';
import { BookingEmployeeRequest, BookingResponse } from 'src/app/interfaces/booking';
import { cilCheckAlt, cilX, cilSearch, cilCalendar, cilDelete } from '@coreui/icons';
import { BookingService } from 'src/app/services/booking.service';
import { DrugService } from 'src/app/services/drug.service';
import { Drug } from 'src/app/interfaces/drug';

@Component({
  selector: 'app-booking-requests',
  templateUrl: './booking-requests.component.html',
  styleUrls: ['./booking-requests.component.css'],
})
export class BookingRequestsComponent implements OnInit {
  drugs: Drug[] = [];
  bookings: BookingResponse[] = [];
  icons = { cilCheckAlt, cilX, cilSearch, cilCalendar, cilDelete };
  pharmacyId = '';
  employeeName = '';

  constructor(private bookingService: BookingService, private drugService: DrugService) {}

  ngOnInit(): void {
    this.drugService.getDrugsByUser().subscribe((d: any) => (this.drugs = d));
    this.pharmacyId = this.drugs.length > 0 ? this.drugs[0].pharmacy.id.toString() : '';
    this.bookingService
      .getBookingsByPharmacy(this.pharmacyId)
      .subscribe((d: BookingResponse[]) => (this.bookings = d));
    const loginRaw = localStorage.getItem('login');
    let parsedLogin: any = null;
    if (loginRaw) {
      try {
        parsedLogin = JSON.parse(loginRaw);
      } catch {
        try {
          parsedLogin = JSON.parse(loginRaw.replace(/^"|"$/g, ''));
        } catch {
          parsedLogin = null;
        }
      }
    }
    this.employeeName = parsedLogin?.userName ?? '';
  }

  approveBooking(bookingId: number): void {
    const bookin = this.bookings.find((b) => b.id === bookingId);
    if (bookin == null) {
      return;
    }
    if (bookin.status !== 'Pending') {
      return;
    }
    const bookingEmployeeRequest: BookingEmployeeRequest = {
      pharmacyId: bookin?.details[0].pharmacyId ?? 0,
      approverName: this.employeeName,
      bookingId,
      cancel: false,
    };

    this.bookingService.approveBooking(bookingEmployeeRequest).subscribe((resp) => {
      const booking = this.bookings.find((b) => b.id === bookingEmployeeRequest.bookingId);
      if (!booking) {
        return;
      }
      if (resp && resp.success) {
        booking.status = 'Confirmed';
        booking.confirmationDate = new Date().toISOString();
      } else {
        booking.status = 'Pending';
      }
    });
  }
  cancelBooking(bookingId: number): void {
    const bookin = this.bookings.find((b) => b.id === bookingId);
    if (bookin == null) {
      return;
    }

    const bookingEmployeeRequest: BookingEmployeeRequest = {
      pharmacyId: bookin?.details[0].pharmacyId ?? 0,
      approverName: this.employeeName,
      bookingId,
      cancel: true,
    };

    this.bookingService.cancelBooking(bookingEmployeeRequest).subscribe((resp) => {
      const booking = this.bookings.find((b) => b.id === bookingEmployeeRequest.bookingId);
      if (!booking) {
        return;
      }
      if (resp && resp.success) {
        booking.status = 'Canceled';
        booking.closureDate = new Date().toISOString();
      } else {
        booking.status = 'Pending';
      }
    });
  }
}
