import { Component } from '@angular/core';

@Component({
  selector: 'app-privacy-policy',
  templateUrl: './privacy-policy.component.html',
  styleUrls: ['./privacy-policy.component.scss'],
})
export class PrivacyPolicyComponent {
  companyName: string = 'VVMS';
  contactName: string = 'Raju';
  appName: string = 'Work Integrated Monitoring and Evaluation';
  contactEmail: string = 'Raju@gmail.com';
}
