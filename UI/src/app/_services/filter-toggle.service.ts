import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class FilterToggleService {

  private toggleSource = new Subject<void>();
  toggle$ = this.toggleSource.asObservable();

  openFilterPanel() {
    this.toggleSource.next();
  }
}
