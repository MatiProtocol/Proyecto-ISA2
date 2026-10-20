import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { cilShortText, cilPencil } from '@coreui/icons';
import { PharmacyRequest } from 'src/app/interfaces/pharmacy';
import { CommonService } from 'src/app/services/CommonService';
import { PharmacyService } from 'src/app/services/pharmacy.service';

@Component({
  selector: 'app-create-pharmacy',
  templateUrl: './create-pharmacy.component.html',
  styleUrls: ['./create-pharmacy.component.css'],
})
export class CreatePharmacyComponent {
  form: FormGroup | any;

  icons = { cilShortText, cilPencil };

  constructor(
    private fb: FormBuilder,
    private commonService: CommonService,
    private pharmacyService: PharmacyService,
  ) {
    this.form = fb.group({
      name: new FormControl(),
      address: new FormControl(),
    });
  }

  get pharmacy_name(): AbstractControl {
    return this.form.controls.name;
  }

  get pharmacy_address(): AbstractControl {
    return this.form.controls.address;
  }

  createPharmacy(): void {
    const pharmacyName = this.pharmacy_name.value ? this.pharmacy_name.value : '';
    const pharmacyAddress = this.pharmacy_address.value ? this.pharmacy_address.value : '';

    const pharmacyRequest = new PharmacyRequest(pharmacyName, pharmacyAddress);
    this.pharmacyService.createPharmacy(pharmacyRequest).subscribe((pharmacy) => {
      if (pharmacy) {
        this.form.reset();
        this.commonService.updateToastData(
          'Success creating pharmacy',
          'success',
          'Pharmacy created.',
        );
      }
    });
  }
}
