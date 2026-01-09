import { TitleCasePipe } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ApprovalFlowModel } from 'src/app/_models/configuration/approval.flow';
import { Guid } from 'guid-typescript';
import { ApprovalFlowConfigFacade } from './state/approvalflow.facades';
import { ErrorStatus, FailedStatus } from 'src/app/_models/ResponseStatus';
import { TCModel } from 'src/app/_models/user/usermodel';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { TwoColConfigFacade } from '../two-column-config/state/facades/two.col.facades';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { privileges } from 'src/app/shared/commonFunctions';

@UntilDestroy()
@Component({
  selector: 'app-approval-flow',
  templateUrl: './approval-flow.component.html',
  styleUrls: ['./approval-flow.component.scss'],
  providers: [ConfirmationService, MessageService],
})
export class ApprovalFlowComponent {
  title: string = 'Approval Flow';
  approvalFlowForm!: FormGroup;

  department!: string;
  roles!: TCModel[];
  departments!: TCModel[];
  selectedRoles!: string[];
  rolesForApproval!: TCModel[];
  approvalFlows!: ApprovalFlowModel[];

  privleges = privileges;
  get f() {
    return this.approvalFlowForm.controls;
  }
  get t() {
    return this.f['roles'] as FormArray;
  }
  get milestoneList() {
    return this.t.controls as FormGroup[];
  }

  constructor(
    private approvalFlowFacade: ApprovalFlowConfigFacade,
    private twoColConfigFacade: TwoColConfigFacade,
    private messageService: MessageService,
    private formBuilder: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.twoColConfigFacade.getDepartments();
    this.approvalFlowForm = this.formBuilder.group({
      roles: new FormArray([]),
    });

    this.twoColConfigFacade.selectDepartments$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.departments = x;
        }
      });
    this.approvalFlowFacade.selectRoles$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.roles = x;
          this.selectedRoles = this.roles
            .filter((x) => x.selected)
            .flatMap((x) => x.value);
          this.generateApprovalRoles();
          this.generateDefaultRows(this.approvalFlows);
        }
      });
    this.approvalFlowFacade.selectApprovalFlow$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.approvalFlows = x;
          this.generateApprovalRoles();
          this.generateDefaultRows(this.approvalFlows);
        }
      });
    this.approvalFlowFacade.selectSaveStatus$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x && (x.Status == FailedStatus || x.Status == ErrorStatus)) {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            life: 80000,
            detail: x.message,
          });
        } else if (x) {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: x?.message,
          });
        }
        this.approvalFlowFacade.approvalFlowGet(this.department);
      });
    this.approvalFlowFacade.selectAddRoleStatus$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x && (x.Status == FailedStatus || x.Status == ErrorStatus)) {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            life: 80000,
            detail: x.message,
          });
          this.approvalFlowFacade.approvalFlowGet(this.department);
        } else if (x) {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: x?.message,
          });
          this.approvalFlowFacade.approvalFlowGet(this.department);
        }
      });
  }
  changeDepartments() {
    this.approvalFlowFacade.getApprovalFlowRoles(this.department);
    this.approvalFlowFacade.approvalFlowGet(this.department);
  }
  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(
      this.milestoneList,
      event.previousIndex,
      event.currentIndex
    );
    let index = 0;
    this.t.controls.map((x) => {
      index++;
      (x as FormGroup).controls['orderNumber'].setValue(index);
    });
  }

  generateApprovalRoles() {
    this.rolesForApproval = [];
    this.rolesForApproval = this.roles?.filter((x) =>
      this.approvalFlows?.some((y) => y.roleId == x.value)
    );
    this.rolesForApproval.unshift({ text: 'Is NA', value: '-1' });
    this.rolesForApproval.push({ text: 'Is Final', value: '-2' });
  }

  roleChanged(e: Event) {}

  addRoles(e: Event) {
    this.approvalFlowFacade.addRoleforApprovalFlow({
      roleIds: this.selectedRoles,
      departmentId: this.department,
    });
  }

  generateDefaultRows(templateMilestones: ApprovalFlowModel[]) {
    this.t.clear();
    if (templateMilestones) {
      templateMilestones.forEach((x) => {
        this.t.push(
          this.formBuilder.group({
            id: [x.id, Validators.required],
            roleName: [x.roleName, [Validators.required]],
            roleId: [x.roleId, [Validators.required]],
            approvalFlowId: [x.approvalFlowId, [Validators.required]],
            orderNumber: [x.orderNumber, [Validators.required]],
            returnFlowId: [x.returnFlowId, [Validators.required]],
          })
        );
      });
    }
  }

  submit() {
    const roles = this.approvalFlowForm.controls['roles'].value;
    this.approvalFlowFacade.saveApprovalFlow(roles);
  }

  resetForm() {
    this.approvalFlowForm.reset();
  }

  ngOnDestroy() {
    this.selectedRoles = [];
    this.approvalFlowFacade.reset();
  }
}
