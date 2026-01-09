import { Component, Input } from '@angular/core';
import { MBookMasterViewModel } from 'src/app/_models/mbook/mbook';
import { dateconvertionwithOnlyDate } from 'src/app/shared/commonFunctions';

@Component({
  selector: 'app-view-mbook-rejection',
  templateUrl: './view-mbook-rejection.component.html',
  styleUrls: ['./view-mbook-rejection.component.scss'],
})
export class ViewMbookRejectionComponent {
  @Input() rejectedMbooks!: MBookMasterViewModel[] | null | undefined;

  dc(val: any) {
    return dateconvertionwithOnlyDate(val);
  }
}
