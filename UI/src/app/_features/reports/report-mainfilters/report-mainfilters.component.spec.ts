import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportMainfiltersComponent } from './report-mainfilters.component';

describe('ReportMainfiltersComponent', () => {
  let component: ReportMainfiltersComponent;
  let fixture: ComponentFixture<ReportMainfiltersComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ReportMainfiltersComponent]
    });
    fixture = TestBed.createComponent(ReportMainfiltersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
