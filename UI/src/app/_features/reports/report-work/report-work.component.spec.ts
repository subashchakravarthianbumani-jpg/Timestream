import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportWorkComponent } from './report-work.component';

describe('ReportWorkComponent', () => {
  let component: ReportWorkComponent;
  let fixture: ComponentFixture<ReportWorkComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ReportWorkComponent]
    });
    fixture = TestBed.createComponent(ReportWorkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
