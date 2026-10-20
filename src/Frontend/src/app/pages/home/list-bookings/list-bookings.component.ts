import { Component } from '@angular/core';
import { BookingService } from '../../../services/booking.service';
import { CommonService } from 'src/app/services/CommonService';
import { BookingResponse } from 'src/app/interfaces/booking';

@Component({
  selector: 'app-list-bookings',
  templateUrl: './list-bookings.component.html',
  styleUrls: ['./list-bookings.component.css'],
})
export class ListBookingsComponent {
  email = '';
  bookings: BookingResponse[] = [];

  private bookingStatusOrder: { [key: string]: number } = {
    'Pending Validation': 1,
    Confirmed: 2,
    Pending: 3,
    Canceled: 4,
    Expired: 5,
    'Picked Up': 6,
  };

  constructor(private bookingService: BookingService, private commonService: CommonService) {}

  getBookingsByEmail(email: string): void {
    this.bookingService.getBookings(email).subscribe((response) => {
      this.bookings = response;
      if (!response) {
        this.commonService.updateToastData(
          'The email address entered is invalid.',
          'danger',
          'Error',
        );
      } else {
        this.sortBookingsByStatus();
        if (response.length == 0) {
          this.commonService.updateToastData(
            'There are no reservations associated with that email address.',
            'info',
            'Info',
          );
        }
      }
    });
  }

  private sortBookingsByStatus(): void {
    this.bookings.sort((a, b) => {
      const statusOrderA = this.bookingStatusOrder[a.status];
      const statusOrderB = this.bookingStatusOrder[b.status];
      return statusOrderA - statusOrderB;
    });
  }
}
