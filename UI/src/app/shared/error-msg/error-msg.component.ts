import { Component, Input, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-error-msg',
  templateUrl: './error-msg.component.html',
  styleUrls: ['./error-msg.component.scss'],
})
export class ErrorMsgComponent implements OnInit {
  ngOnInit(): void {
    this.control?.valueChanges.subscribe((value) => {
      this.updateErrorMsg(this.control);
    });
  }
  errors: string[] = [];

  control: FormControl = new FormControl();
  @Input('control') set _control(value: any) {
    this.control = value as FormControl;
    this.updateErrorMsg(value);
  }

  updateErrorMsg(ctrl: FormControl) {
    this.errors = [];
    if (ctrl && ctrl.invalid && (ctrl.dirty || ctrl.touched) && ctrl.errors) {
      const keys = Object.keys(ctrl.errors);
      keys.forEach((key: string) => {
        switch (key) {
          case 'required':
            {
              if (!this.errors.includes('Required'))
                this.errors = [...this.errors, 'Required'];
            }
            break;
          case 'minlength':
            {
              if (!this.errors.includes('Minimum')) {
                this.errors = [
                  ...this.errors,
                  'Minimum of ' +
                    (ctrl?.errors?.['minlength']
                      ? ctrl?.errors?.['minlength'].requiredLength
                      : 0) +
                    ' characters required',
                ];
              }
            }
            break;
          case 'maxlength':
            {
              if (!this.errors.includes('Maximum')) {
                this.errors = [
                  ...this.errors,
                  'Maximum of ' +
                    (ctrl?.errors?.['maxlength']
                      ? ctrl?.errors?.['maxlength'].requiredLength
                      : 0) +
                    ' characters allowed',
                ];
              }
            }
            break;
          case 'email':
            {
              if (!this.errors.includes('Email')) {
                this.errors = [...this.errors, 'Email is Invalid'];
              }
            }
            break;
          case 'min':
            {
              if (!this.errors.includes('Minimum')) {
                this.errors = [
                  ...this.errors,
                  'Minimum of ' +
                    (ctrl?.errors?.['min'] ? ctrl?.errors?.['min'].min : 0) +
                    ' is required',
                ];
              }
            }
            break;
          case 'max':
            {
              if (!this.errors.includes('Maximum')) {
                this.errors = [
                  ...this.errors,
                  'Maximum of ' +
                    (ctrl?.errors?.['max'] ? ctrl?.errors?.['max'].max : 0) +
                    ' is required',
                ];
              }
            }
            break;
        }
      });
    }
  }
}
