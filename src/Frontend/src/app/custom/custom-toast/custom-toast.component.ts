import { Component } from '@angular/core';
import { CommonService } from '../../services/CommonService';

@Component({
  selector: 'app-custom-toast',
  templateUrl: './custom-toast.component.html',
  styleUrls: ['./custom-toast.component.css'],
})
export class CustomToastComponent {
  visible = false;
  message = '';
  color = 'danger';
  title = '';

  constructor(private commonService: CommonService) {
    this.commonService.onToastDataUpdate.subscribe((data: any) => {
      if (Object.keys(data).length !== 0 && data.message !== '') {
        this.visible = true;
        this.message = data.message;
        this.color = data.type;
        this.title = data.title;
      }
    });
  }

  onCloseToast() {
    this.visible = false;
    this.commonService.updateToastData('', '', '');
  }
}
