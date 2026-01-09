import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApprovalFlowComponent } from './approval-flow.component';

describe('ApprovalFlowComponent', () => {
  let component: ApprovalFlowComponent;
  let fixture: ComponentFixture<ApprovalFlowComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ApprovalFlowComponent]
    });
    fixture = TestBed.createComponent(ApprovalFlowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
