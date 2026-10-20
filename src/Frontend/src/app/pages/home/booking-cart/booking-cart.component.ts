import { Component, OnInit } from '@angular/core';
import { StorageManager } from '../../../utils/storage-manager';
import { CommonService } from 'src/app/services/CommonService';
import { Drug } from 'src/app/interfaces/drug';
import { DrugService } from 'src/app/services/drug.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-booking-cart',
  templateUrl: './booking-cart.component.html',
  styleUrls: ['./booking-cart.component.css'],
})
export class BookingCartComponent implements OnInit {
  bookingCart: any[] = [];
  storageManager: StorageManager;
  total = 0;

  constructor(
    private drugService: DrugService,
    private router: Router,
    private commonService: CommonService,
  ) {
    this.storageManager = new StorageManager();
  }

  ngOnInit(): void {
    this.bookingCart = JSON.parse(this.storageManager.getData('booking'));
    if (!this.bookingCart) {
      this.bookingCart = [];
      this.storageManager.saveData('booking', JSON.stringify(this.bookingCart));
    }
    this.updateTotal();
  }

  createBooking() {
    this.bookingCart = JSON.parse(this.storageManager.getData('booking')) || [];
    if (this.bookingCart.length === 0) {
      return;
    }
    const pharmacyId = this.bookingCart[0].pharmacy.id.toString();

    this.drugService.getDrugsFilter(pharmacyId, '').subscribe((drugs) => {
      for (const item of this.bookingCart) {
        const drug = drugs.find((d: Drug) => d.id === item.id);
        if (item.quantity > (drug?.stock ?? 0)) {
          this.commonService.updateToastData('Insuficient stock.', 'danger', 'Error');
          return;
        }
      }

      this.storageManager.saveData('booking', JSON.stringify(this.bookingCart));
      this.router.navigate(['/home/booking-cart/checkout']);
    });
  }

  delete(index: number) {
    this.bookingCart.splice(index, 1);
    this.storageManager.saveData('booking', JSON.stringify(this.bookingCart));
    this.updateTotal();
    this.updateHeader(this.bookingCart.length);
  }

  updateTotal() {
    this.total = 0;
    this.bookingCart = JSON.parse(this.storageManager.getData('booking'));
    for (const item of this.bookingCart) {
      this.total += item.price * item.quantity;
    }
    this.storageManager.saveData('totalbooking', JSON.stringify(this.total));
  }

  updateHeader(value: number) {
    this.commonService.updateHeaderData(value);
  }

  addPrescription(index: number) {
    const fileInput = document.createElement('input');
    fileInput.type = 'file';
    fileInput.accept = 'image/*';
    fileInput.style.display = 'none';
    document.body.appendChild(fileInput);

    fileInput.onchange = (event: any) => {
      const file = event.target.files[0];
      if (file) {
        const reader = new FileReader();
        reader.onload = (e) => {
          const base64 = e.target?.result as string;
          this.bookingCart[index].prescriptionImage = base64;
          localStorage.setItem('booking', JSON.stringify(this.bookingCart));
        };
        reader.readAsDataURL(file);
      }
      document.body.removeChild(fileInput);
    };
    fileInput.click();
  }
}
