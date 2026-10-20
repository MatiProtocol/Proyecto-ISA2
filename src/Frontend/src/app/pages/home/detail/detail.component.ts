import { Component, OnInit } from '@angular/core';
import { cilCart, cilPlus, cilCompass } from '@coreui/icons';
import { IconSetService } from '@coreui/icons-angular';
import { ActivatedRoute } from '@angular/router';
import { Drug } from 'src/app/interfaces/drug';
import { DrugService } from '../../../services/drug.service';
import { StorageManager } from '../../../utils/storage-manager';
import { Router } from '@angular/router';
import { CommonService } from '../../../services/CommonService';

@Component({
  selector: 'app-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.css'],
})
export class DetailComponent implements OnInit {
  drug: Drug | undefined;
  quantity = 1;
  cart: any[] = [];
  booking: any[] = [];

  constructor(
    private route: ActivatedRoute,
    public iconSet: IconSetService,
    private drugService: DrugService,
    private storageManager: StorageManager,
    private router: Router,
    private commonService: CommonService,
  ) {
    iconSet.icons = { cilCart, cilPlus, cilCompass };
  }

  ngOnInit(): void {
    this.getDrug();
    this.storageManager.saveData('total', JSON.stringify(0));
  }

  getDrug(): void {
    const id = parseInt(this.route.snapshot.paramMap.get('id')!, 10);
    this.drugService.getDrug(id).subscribe((drug) => (this.drug = drug));
  }

  addToCart(drug: Drug) {
    if (drug) {
      if (this.quantity > drug.stock || this.quantity <= 0) {
        this.commonService.updateToastData(
          'The quantity must be not more than ' + drug.stock,
          'danger',
          'Error',
        );
        return;
      }
      this.cart = JSON.parse(this.storageManager.getData('cart'));
      if (!this.cart) {
        this.cart = [];
        this.storageManager.saveData('cart', JSON.stringify(this.cart));
      }

      let exist = false;
      for (const item of this.cart) {
        if (item.code === drug.code && item.id === drug.id) {
          item.quantity += this.quantity;
          exist = true;
          break;
        }
      }
      if (!exist) {
        drug.quantity = this.quantity;
        this.cart.push(drug);
      }
      this.storageManager.saveData('cart', JSON.stringify(this.cart));
    }
    this.updateHeader(this.cart.length);
    this.router.navigate(['/home/cart']);
  }

  addToBooking(drug: Drug) {
    if (drug) {
      if (this.quantity <= 0) {
        this.commonService.updateToastData('Product count cant be 0 or less.', 'danger', 'Error');
        return;
      }
      this.booking = JSON.parse(this.storageManager.getData('booking') || '[]');
      let currentPharmacyId = null;
      if (this.booking.length > 0) {
        currentPharmacyId = this.booking[0].pharmacy.id;
      }
      if (currentPharmacyId !== null && currentPharmacyId !== drug.pharmacy.id) {
        this.commonService.updateToastData(
          'All medications in the reservation must belong to the same pharmacy.',
          'danger',
          'Error',
        );
        return;
      }

      let exist = false;
      for (const item of this.booking) {
        if (item.name === drug.name && item.id === drug.id) {
          item.quantity += this.quantity;
          exist = true;
          break;
        }
      }
      if (!exist) {
        drug.stock = this.quantity;
        this.booking.push(this.convertDrug(drug));
      }
      this.storageManager.saveData('booking', JSON.stringify(this.booking));
      this.router.navigate(['/home/cart']);
    }
  }

  convertDrug(drug: Drug) {
    return {
      id: drug.id,
      name: drug.name,
      code: drug.code,
      price: drug.price,
      quantity: drug.stock,
      prescription: drug.prescription,
      pharmacy: {
        id: drug.pharmacy.id,
        name: drug.pharmacy.name,
      },
    };
  }

  updateHeader(value: number) {
    this.commonService.updateHeaderData(value);
  }
}
