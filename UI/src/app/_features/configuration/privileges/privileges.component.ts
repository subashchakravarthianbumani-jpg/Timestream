import { Component } from '@angular/core';
import { RoleConfigFacade } from '../role/state/role.facades';
import { ConfirmationService, MessageService } from 'primeng/api';
import { AccountPrivilegeByGroupModel } from 'src/app/_models/configuration/privilege';
import { IRoleModel } from 'src/app/_models/configuration/role';
import { ErrorStatus, FailedStatus } from 'src/app/_models/ResponseStatus';
import { Router } from '@angular/router';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

@UntilDestroy()
@Component({
  selector: 'app-privileges',
  templateUrl: './privileges.component.html',
  styleUrls: ['./privileges.component.scss'],
  providers: [ConfirmationService, MessageService],
})
export class PrivilegesComponent {
  title: string = 'Privileges';
  roles!: IRoleModel[];
  selectedRoles!: string;
  privileges!: AccountPrivilegeByGroupModel[];
  constructor(
    private roleConfigFacade: RoleConfigFacade,
    private messageService: MessageService,
    private router: Router
  ) {}
  ngOnInit() {
    this.roleConfigFacade.getRoles(true);
    this.roleConfigFacade.selectRoles$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.roles = x;
          if (x.length > 0) {
            this.roleConfigFacade.getPrivileges(x[0].id);
            this.selectedRoles = x[0].id;
          }
        }
      });
    this.roleConfigFacade.selectPrivileges$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.privileges = x;
        }
      });
    this.roleConfigFacade.selectSavePrivilegesStatus$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x && (x.Status == FailedStatus || x.Status == ErrorStatus)) {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            life: 80000,
            detail: x.message,
          });
          this.roleConfigFacade.getPrivileges(this.selectedRoles);
        } else if (x) {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: x?.message,
          });
        }
      });
  }
  changeRole(val: any) {
    this.roleConfigFacade.getPrivileges(val.value);
  }
  changeEvent(val: any, id: string) {
    this.roleConfigFacade.updatesPrivilegeaveStatus();
    if (val.checked) {
      this.roleConfigFacade.savePrivilege({
        privilegeId: id,
        roleId: this.selectedRoles,
        isSelected: true,
      });
    } else {
      this.roleConfigFacade.savePrivilege({
        privilegeId: id,
        roleId: this.selectedRoles,
        isSelected: false,
      });
    }
  }
  back() {
    this.router.navigateByUrl('configuration/role');
  }
  ngOnDestroy() {
    this.roleConfigFacade.reset();
  }
}
