import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { AccountService } from '../_services/account.service';
import { MessageService } from 'primeng/api';
import {
  ErrorStatus,
  FailedStatus,
  ResponseModel,
} from '../_models/ResponseStatus';
import { Router } from '@angular/router';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private accountService: AccountService,
    private messageService: MessageService,
    private router: Router
  ) {}

  //   intercept(
  //     request: HttpRequest<any>,
  //     next: HttpHandler
  //   ): Observable<HttpEvent<any>> {
  //     debugger;
  //     return next.handle(request).pipe(
  //       catchError((err) => {
  //         if ([401, 403].includes(err.status) && this.accountService.userValue) {
  //           // auto logout if 401 or 403 response returned from api
  //           this.accountService.logout();
  //         }

  //         const error = err.error?.message || err.statusText;
  //         console.error(err);
  //         return throwError(() => error);
  //       })
  //     );
  //   }

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      map((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
          var body: ResponseModel = event.body;
          if (body.status === ErrorStatus) {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              life: 80000,
              detail: 'Unexpeted Error! Please try again',
            });
          }
        }
        return event;
      }),
      catchError((err) => {
        const error = err.error?.message || err.statusText;
        if ([401, 403].includes(err.status)) {
          // auto logout if 401 or 403 response returned from api
          this.accountService.logout();
          this.router.navigate(['/auth/login']);
        } else {
          console.error();
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: error,
          });
        }
        return throwError(() => error);
      })
    );
  }
}
