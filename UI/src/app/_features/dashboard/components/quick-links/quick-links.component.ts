import { Component } from '@angular/core';
import { SuccessStatus } from 'src/app/_models/ResponseStatus';
import { QuickLinkService } from 'src/app/_services/quicklinks.service';
import {
  imgFileTypes,
  pdfFileTypes,
  wordFileTypes,
  xlFileTypes,
} from 'src/app/shared/commonFunctions';

@Component({
  selector: 'app-quick-links',
  templateUrl: './quick-links.component.html',
  styleUrls: ['./quick-links.component.scss'],
})
export class QuickLinksComponent {
  links!: any[];
  constructor(private quickLinkService: QuickLinkService) {}
  ngOnInit() {
    this.quickLinkService.getQuickLinks(true).subscribe(
      (data) => {
        if (data && data.status == SuccessStatus) {
          this.links = data.data;
        }
      },
      (error) => {}
    );
  }
  getFiletype(type: string) {
    if (wordFileTypes.includes(type)) {
      return 'pi-file-word';
    } else if (xlFileTypes.includes(type)) {
      return 'pi-file-excel';
    } else if (pdfFileTypes.includes(type)) {
      return 'pi-file-pdf';
    } else if (imgFileTypes.includes(type)) {
      return 'pi-image';
    } else {
      return 'pi-file';
    }
  }
}
