import { TitleCasePipe } from '@angular/common';
import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ActionModel, Actions, Column } from 'src/app/_models/datatableModel';
import { TenderFilterModel } from 'src/app/_models/filterRequest';
import { TenderFacade } from '../state/tender.facades';

@Component({
  selector: 'app-mnge-work-id',
  templateUrl: './mnge-work-id.component.html',
  styleUrls: ['./mnge-work-id.component.scss'],
})
export class MngeWorkIdComponent {}
