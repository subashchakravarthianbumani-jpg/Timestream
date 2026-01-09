import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from 'src/app/_services/account.service';
import { LayoutService } from 'src/app/layout/service/app.layout.service';
import { AuthFacade } from 'src/app/state/facades/auth.facades';
import { ForgetPasswordFacade } from './state/forget-password.facades';
import { ErrorStatus, FailedStatus } from 'src/app/_models/ResponseStatus';
import { MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { ForgetPasswordService } from 'src/app/_services/forget-password.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

@UntilDestroy()
@Component({
  selector: 'app-forget-password',
  templateUrl: './forget-password.component.html',
  styleUrls: ['./forget-password-style.scss'],
})
export class ForgetPasswordComponent implements OnInit, OnDestroy {
  isError: boolean = false;
  ErrorMessage: string = '';
  mobileNumber: string = '';
  otp: string = '';
  showOTPField: boolean = false;
  token: string = '';
  disableMobileField: boolean = false;

  buttonLabel: string = 'Send OTP';

  showPassword: boolean = false;
  password: string = '';
  confirmPassword: string = '';

  showCounter: boolean = false;
  showResendOtp: boolean = false;
  countDown: Subscription = {} as Subscription;
  counter = 60;
  tick = 1000;

  disableButton: boolean = false;

  actionCode: string = 'SendOTP';

  constructor(
    public layoutService: LayoutService,
    private accountService: AccountService,
    private route: ActivatedRoute,
    private router: Router,
    private messageService: MessageService,
    private forgetPasswordFacade: ForgetPasswordFacade,
    private forgetPasswordService: ForgetPasswordService
  ) {}

  ngOnDestroy() {
    //this.countDown.unsubscribe();
  }

  ngOnInit(): void {
    this.forgetPasswordFacade.getSentOtpStatus$
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
          this.showOTPField = true;
          this.buttonLabel = 'Verify OTP';
          this.actionCode = 'VerifyOTP';

          this.showCounter = true;
          this.showResendOtp = false;
          this.counter = 60;
          this.countDown = this.forgetPasswordService
            .getCounter(this.tick)
            //.pipe(untilDestroyed(this))
            .subscribe(() => {
              if (this.counter == 0) {
                this.showCounter = false;
                this.showResendOtp = true;
                this.countDown.unsubscribe();
              }
              this.counter--;
            });

          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: x?.message,
          });
        }
      });

    this.forgetPasswordFacade.getOtpVerificationStatus$
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
          this.showOTPField = false;
          this.showPassword = true;
          this.disableMobileField = true;
          this.buttonLabel = 'Save Password';
          this.actionCode = 'SavePassword';

          this.showCounter = false;
          this.showResendOtp = false;
          this.countDown.unsubscribe();

          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: x?.message,
          });
        }
      });

    this.forgetPasswordFacade.saveNewPasswordStatus$
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
          this.disableButton = true;

          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: x?.message,
          });

          setTimeout(() => {
            this.navigateToLogin();
          }, 3000);
        }
      });

    this.forgetPasswordFacade.getToken$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        this.token = x;
      });
  }

  clearErrormessage(value: any) {
    this.isError = false;
    this.ErrorMessage = '';
  }

  keyPress(event: any) {
    const pattern = /[0-9\+\-\ ]/;

    let inputChar = String.fromCharCode(event.charCode);
    if (event.keyCode != 8 && !pattern.test(inputChar)) {
      event.preventDefault();
    }
  }

  resendOTP() {
    if (this.mobileNumber.length == 10) {
      this.forgetPasswordFacade.sendOtp(this.mobileNumber);
    } else {
      this.isError = true;
      this.ErrorMessage = 'Please enter valid mobile number.';
    }
  }

  navigateToLogin() {
    this.router.navigate(['/auth/login/']);
  }

  onSubmit() {
    if (this.actionCode == 'SendOTP') {
      if (this.mobileNumber.length == 10) {
        this.forgetPasswordFacade.sendOtp(this.mobileNumber);
      } else {
        this.isError = true;
        this.ErrorMessage = 'Please enter valid mobile number.';
      }
    } else if (this.actionCode == 'VerifyOTP') {
      if (this.otp == '') {
        this.isError = true;
        this.ErrorMessage = 'Please enter OTP.';
      } else if (this.otp.length != 6) {
        this.isError = true;
        this.ErrorMessage = 'Please enter valid OTP.';
      } else if (this.otp.length == 6) {
        this.forgetPasswordFacade.validateOtp(this.mobileNumber, this.otp);
      }
    } else if (this.actionCode == 'SavePassword') {
      if (this.password == '') {
        this.isError = true;
        this.ErrorMessage = 'Please enter password';
      } else if (this.confirmPassword == '') {
        this.isError = true;
        this.ErrorMessage = 'Please enter confirm password';
      } else if (this.password != this.confirmPassword) {
        this.isError = true;
        this.ErrorMessage = 'Password is not matched';
      } else {
        this.forgetPasswordFacade.saveNewPassword(
          this.token,
          this.otp,
          this.password
        );
      }
    }
  }
}
