import { Component, OnInit } from '@angular/core';
import { cilThumbUp, cilCart, cilPlus, cilCompass } from '@coreui/icons';
import { IconSetService } from '@coreui/icons-angular';
import { BookingService } from '../../../services/booking.service';
import { CryptoService } from '../../../services/crypto.service';
import { StorageManager } from '../../../utils/storage-manager';
import { BookingRequest, BookingRequestDetail } from 'src/app/interfaces/booking';
import { Drug } from 'src/app/interfaces/drug';
import { Router } from '@angular/router';
import { CommonService } from 'src/app/services/CommonService';

@Component({
  selector: 'app-booking-cho',
  templateUrl: './booking-cho.component.html',
  styleUrls: ['./booking-cho.component.css'],
})
export class BookingChoComponent implements OnInit {
  total = 0;
  email = '';
  bookingCart: Drug[] = [];
  hasPrescriptionAll = true;

  constructor(
    public iconSet: IconSetService,
    private storageManager: StorageManager,
    private bookingService: BookingService,
    private cryptoService: CryptoService,
    private commonService: CommonService,
    private router: Router,
  ) {
    iconSet.icons = { cilThumbUp, cilCart, cilPlus, cilCompass };
  }

  ngOnInit(): void {
    this.updateCart();
    const _total = JSON.parse(this.storageManager.getData('totalbooking'));
    this.total = 0;
    if (_total) {
      this.total = _total;
    }
  }

  finishBooking(): void {
    const bookingCart = JSON.parse(this.storageManager.getData('booking'));
    const details: BookingRequestDetail[] = [];
    for (const item of bookingCart) {
      if (this.hasPrescriptionAll && item.prescription && !item.prescriptionImage) {
        this.hasPrescriptionAll = false;
      }
      const detail = new BookingRequestDetail(
        item.code,
        item.quantity,
        item.pharmacy.id,
        item.prescriptionImage,
      );
      details.push(detail);
    }
    const now = new Date().toISOString();
    this.cryptoService.generateKeyPair().then((keyPair) => {
      const bookingRequest = new BookingRequest(
        this.email,
        now,
        keyPair.publicKeyBase64,
        this.hasPrescriptionAll,
        details,
      );
      this.bookingService.addBooking(bookingRequest).subscribe((booking) => {
        this.storageManager.removeData('booking');
        this.storageManager.removeData('totalbooking');
        this.commonService.updateToastData(
          'Id: ' + booking.id + '. Private Key: ' + keyPair.privateKeyBase64,
          'success',
          'Booking created successfully.',
        );
      });
      this.router.navigate(['/home']);
    });
  }

  updateCart(): void {
    this.bookingCart = JSON.parse(this.storageManager.getData('booking'));
    if (!this.bookingCart) {
      this.bookingCart = [];
      this.storageManager.saveData('booking', JSON.stringify(this.bookingCart));
    }
  }
}
