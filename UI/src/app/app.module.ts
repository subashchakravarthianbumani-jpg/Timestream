import { NgModule, isDevMode } from '@angular/core';
import {
  CommonModule,
  HashLocationStrategy,
  LocationStrategy,
} from '@angular/common';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { AppLayoutModule } from './layout/app.layout.module';
import { NotfoundComponent } from './demo/components/notfound/notfound.component';
import { ProductService } from './demo/service/product.service';
import { CountryService } from './demo/service/country.service';
import { CustomerService } from './demo/service/customer.service';
import { EventService } from './demo/service/event.service';
import { IconService } from './demo/service/icon.service';
import { NodeService } from './demo/service/node.service';
import { PhotoService } from './demo/service/photo.service';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptor } from './_helpers/jwt.interceptor';
import { ErrorInterceptor } from './_helpers/error.interceptor';
import { fakeBackendProvider } from './_helpers/fake-backend';
import { NgxPermissionsModule } from 'ngx-permissions';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { SpinnerComponent } from './shared/spinner/spinner.component';
import { LoaderInterceptor } from './_helpers/loader.interceptor';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { MessageService } from 'primeng/api';
import { AuthStateKey, authReducer } from './state/reducers/auth.reducers';
import { AuthEffects } from './state/effects/auth.effects';
import { AuthFacade } from './state/facades/auth.facades';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { environment } from 'src/environments/environment';
import { ToastModule } from 'primeng/toast';
import { MessagesModule } from 'primeng/messages';
import { RoundPipe } from './shared/pipes/InitCaps';
import { PipeModuleModule } from './shared/pipe-module/pipe-module.module';
import { CookieService } from 'ngx-cookie-service';
import { DatePipe } from '@angular/common';



//import { SelectModule } from 'primeng/select';


@NgModule({
  declarations: [AppComponent, NotfoundComponent, SpinnerComponent],
  imports: [
    CommonModule,
    PipeModuleModule,
    AppRoutingModule,
    HttpClientModule,
    //SelectModule,
    AppLayoutModule,
    MessagesModule,
    ToastModule,
    ProgressSpinnerModule,
    NgxPermissionsModule.forRoot(),
    StoreModule.forRoot({}),
    StoreModule.forFeature(AuthStateKey, authReducer),
    EffectsModule.forFeature([AuthEffects]),
    EffectsModule.forRoot([AuthEffects]),
    !environment.production ? StoreDevtoolsModule.instrument() : [],
  ],
  exports: [],
  providers: [
    { provide: LocationStrategy, useClass: HashLocationStrategy },
    CountryService,
    CustomerService,
    CookieService,
    MessageService,
    EventService,
    IconService,
    NodeService,
    PhotoService,
    ProductService,
    AuthFacade,
    DatePipe,
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptor, multi: true },

    // provider used to create fake backend
    // fakeBackendProvider,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
