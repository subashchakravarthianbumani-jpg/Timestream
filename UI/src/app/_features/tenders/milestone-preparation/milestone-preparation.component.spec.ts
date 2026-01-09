import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MilestonePreparationComponent } from './milestone-preparation.component';

describe('MilestonePreparationComponent', () => {
  let component: MilestonePreparationComponent;
  let fixture: ComponentFixture<MilestonePreparationComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MilestonePreparationComponent]
    });
    fixture = TestBed.createComponent(MilestonePreparationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
