import { Component } from '@angular/core';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.scss'],
})
export class StatisticsComponent {
  paymentOptions: any[] = [
    { name: 'Current yr', value: 1 },
    { name: 'Past yr', value: 2 },
    { name: 'Past 5 yr', value: 3 },
  ];
}
