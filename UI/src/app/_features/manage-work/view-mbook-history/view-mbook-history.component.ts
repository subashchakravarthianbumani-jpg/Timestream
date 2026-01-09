import { Component, Input } from '@angular/core';
import { MBookApprovalHistoryViewModel } from 'src/app/_models/mbook/mbook';
import { GeneralService } from 'src/app/_services/general.service';
import {
  dateconvertion,
  dateconvertionwithOnlyDate,
} from 'src/app/shared/commonFunctions';

@Component({
  selector: 'app-view-mbook-history',
  templateUrl: './view-mbook-history.component.html',
  styleUrls: ['./view-mbook-history.component.scss'],
})
export class ViewMbookHistoryComponent {
  @Input() history!: MBookApprovalHistoryViewModel[] | null | undefined;

  constructor(
    private generalService: GeneralService,
  )
  {}
  dc(val: any) {
    return dateconvertion(val);
  }
  downloadMilestoneFiles(fileid: string, name: string) {
    this.generalService.downloads(fileid, name ?? 'File.png');
  }
}
