import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { NgxPermissionsService } from 'ngx-permissions';
import { Message, MessageService } from 'primeng/api';
import { first } from 'rxjs';
import { ResponseModel, SuccessStatus } from 'src/app/_models/ResponseStatus';
import { AccountService } from 'src/app/_services/account.service';
import { LayoutService } from 'src/app/layout/service/app.layout.service';
import { AuthFacade } from 'src/app/state/facades/auth.facades';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.scss'],
  providers: [MessageService],
  styles: [
    `
      :host ::ng-deep .pi-eye,
      :host ::ng-deep .pi-eye-slash {
        transform: scale(1.6);
        margin-right: 1rem;
        color: var(--primary-color) !important;
      }
    `,
  ],
})
export class LoginComponent implements OnInit, AfterViewInit {
  msgs: Message[] = [];
  valCheck: string[] = ['remember'];
  ErrorMessage!: string;
  isError: boolean = false;
  rememberMe: boolean = false;
  password!: string;
  username!: string;

  constructor(
    public layoutService: LayoutService,
    private permissionsService: NgxPermissionsService,
    private accountService: AccountService,
    private cookieService: CookieService,
    private route: ActivatedRoute,
    private router: Router,
    private authFacade: AuthFacade
  ) {}
  ngOnInit(): void {}

  @ViewChild('videoPlayer') videoPlayer!: ElementRef;

  ngAfterViewInit() {
    const video = this.videoPlayer.nativeElement;
    video.muted = true;
    video.play().catch((error: any) => {
      console.log('Video play was interrupted:', error);
    });
  }

  clearErrormessage(value: any) {
    this.isError = false;
    this.ErrorMessage = '';
  }

  onSubmit() {
    //this.authFacade.authenticate(this.username, this.password);
    this.accountService
      .login(this.username, this.password)
      .pipe(first())
      .subscribe((data: ResponseModel) => {
        if (data.status === SuccessStatus) {
          const privillage: string = this.cookieService.get('privillage');
          this.permissionsService.loadPermissions(privillage.split(','));
          const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
          this.router.navigateByUrl(returnUrl);
        } else {
          this.ErrorMessage = data.message;
          this.isError = true;
          // this.service.add({
          //   key: 'tst',
          //   severity: 'error',
          //   summary: 'Error Message',
          //   detail: data.message,
          // });
        }
      });
  }
}
