import { Component, OnInit, Input } from '@angular/core';
import { Pharmacy } from '../../interfaces/pharmacy';
import { CommonService } from '../../services/CommonService';
import { PharmacyService } from '../../services/pharmacy.service';
import { DrugService } from '../../services/drug.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-custom-header',
  templateUrl: './custom-header.component.html',
  styleUrls: ['./custom-header.component.css'],
})
export class CustomHeaderComponent implements OnInit {
  pharmacys: Pharmacy[] = [];
  badge = 0;
  searchValue = '';
  pharmacyId = '0';

  @Input() icons = true;
  @Input() select = true;
  @Input() search = true;

  constructor(
    private commonService: CommonService,
    private pharmacyService: PharmacyService,
    private drugService: DrugService,
    private router: Router,
  ) {
    this.commonService.onHeaderDataUpdate.subscribe((data: any) => {
      this.badge = data;
    });
  }

  ngOnInit(): void {
    this.getPharmacys();
  }

  getPharmacys(): void {
    this.pharmacyService.getPharmacys().subscribe((pharmacys) => (this.pharmacys = pharmacys));
  }

  onChangeSelect(id: any): void {
    this.pharmacyId = id;
    this.getDrugsFilter(this.pharmacyId, this.searchValue);
  }

  onSearch() {
    this.getDrugsFilter(this.pharmacyId, this.searchValue);
  }

  navToLogin() {
    this.router.navigate(['/login']);
  }

  getDrugsFilter(pharmacyId: string, pharmacyName: string): void {
    this.drugService.getDrugsFilter(pharmacyId, pharmacyName).subscribe((drugs) => {
      this.commonService.updateSearchData(drugs);
    });
  }

  clearSearch() {
    this.searchValue = '';
    this.getDrugsFilter(this.pharmacyId, this.searchValue);
  }
}
