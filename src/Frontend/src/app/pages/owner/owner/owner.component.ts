import { Component } from '@angular/core';
import { cilUser, cilLockLocked, cilArrowThickRight } from '@coreui/icons';

@Component({
  selector: 'app-owner',
  templateUrl: './owner.component.html',
  styleUrls: ['./owner.component.css'],
})
export class OwnerComponent {
  icons = { cilUser, cilLockLocked, cilArrowThickRight };
}
