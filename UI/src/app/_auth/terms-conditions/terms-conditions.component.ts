import { Component } from '@angular/core';

@Component({
  selector: 'app-terms-conditions',
  templateUrl: './terms-conditions.component.html',
  styleUrls: ['./terms-conditions.component.scss'],
})
export class TermsConditionsComponent {
  companyName: string = 'VVMS';
  contactName: string = 'Raju';
  appName: string = 'Work Integrated Monitoring and Evaluation';
  contactEmail: string = 'Raju@gmail.com';
}
