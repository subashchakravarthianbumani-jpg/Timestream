import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportMilestoneComponent } from './report-milestone.component';

describe('ReportMilestoneComponent', () => {
  let component: ReportMilestoneComponent;
  let fixture: ComponentFixture<ReportMilestoneComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ReportMilestoneComponent]
    });
    fixture = TestBed.createComponent(ReportMilestoneComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
