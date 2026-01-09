import { Store } from '@ngrx/store';
import * as AuthActions from '../actions/auth.actions';
import * as AuthSelectors from '../selectors/auth.selectors';
import { Injectable } from '@angular/core';

@Injectable()
export class AuthFacade {
  constructor(private store: Store) {}

  selectuserDetails$ = this.store.select(AuthSelectors.selectuserDetails);

  authenticate(username: string, password: string) {
    this.store.dispatch(AuthActions.Authenticate({ username, password }));
  }
}
