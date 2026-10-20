import { Component } from '@angular/core';
import { BookingService } from '../../../services/booking.service';
import { CryptoService } from 'src/app/services/crypto.service';
import { CommonService } from '../../../services/CommonService';
import { BookingResponse } from 'src/app/interfaces/booking';
import { cilCart, cilPlus, cilCompass, cilMagnifyingGlass, cilThumbUp } from '@coreui/icons';
import { IconSetService } from '@coreui/icons-angular';

@Component({
  selector: 'app-pick-up',
  templateUrl: './pick-up.component.html',
  styleUrls: ['./pick-up.component.css'],
})
export class PickUpComponent {
  bookingId = 1;
  privateKey = '';
  booking: BookingResponse | null = null;

  constructor(
    private bookingService: BookingService,
    private cryptoService: CryptoService,
    private commonService: CommonService,
    public iconSet: IconSetService,
  ) {
    iconSet.icons = { cilCart, cilPlus, cilCompass, cilMagnifyingGlass, cilThumbUp };
  }

  searchBooking(): void {
    if (this.bookingId <= 0) {
      this.commonService.updateToastData(
        'The booking id must be a positive integer.',
        'danger',
        'Error',
      );
      return;
    }

    this.bookingService.getBooking(this.bookingId).subscribe((booking) => {
      this.booking = booking;
    });
  }

  pickUpBooking(): void {
    if (this.booking && this.booking.status != 'Confirmed') {
      this.commonService.updateToastData(
        'Only confirmed reservations can be withdrawn.',
        'danger',
        'Error',
      );
      return;
    }

    this.bookingService.postChallenge(this.bookingId).subscribe((challengeResponse) => {
      const challenge = challengeResponse.challenge;
      try {
        const signature = this.cryptoService.signChallenge(challenge, this.privateKey);
        this.bookingService
          .confirmChallenge(this.bookingId, challenge, signature)
          .subscribe((response) => {
            if (!response) {
              this.privateKey = '';
              return;
            }
            this.bookingService.pickUpBooking(this.bookingId).subscribe(() => {
              this.commonService.updateToastData(
                'Booking picked up successfully',
                'success',
                'Success',
              );
              this.booking = null;
            });
          });
      } catch (error) {
        this.commonService.updateToastData(
          'The provided private key is not valid for this reservation.',
          'danger',
          'Error',
        );
        this.privateKey = '';
        return;
      }
    });
  }
}
