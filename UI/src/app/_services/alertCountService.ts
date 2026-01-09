// src/app/_services/alert-count.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AlertCountService {
  private alertCountSubject = new BehaviorSubject<number>(0);
  alertCount$ = this.alertCountSubject.asObservable();

  setCount(count: number) {
    this.alertCountSubject.next(count);
  }

  getCount(): number {
    return this.alertCountSubject.value;
  }
}
