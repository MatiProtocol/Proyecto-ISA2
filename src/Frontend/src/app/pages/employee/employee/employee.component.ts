import { Component } from '@angular/core';
import { cilUser, cilLockLocked, cilArrowThickRight } from '@coreui/icons';

@Component({
  selector: 'app-employee',
  templateUrl: './employee.component.html',
  styleUrls: ['./employee.component.css'],
})
export class EmployeeComponent {
  icons = { cilUser, cilLockLocked, cilArrowThickRight };
}
