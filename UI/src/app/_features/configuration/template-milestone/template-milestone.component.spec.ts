import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TemplateMilestoneComponent } from './template-milestone.component';

describe('TemplateMilestoneComponent', () => {
  let component: TemplateMilestoneComponent;
  let fixture: ComponentFixture<TemplateMilestoneComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [TemplateMilestoneComponent]
    });
    fixture = TestBed.createComponent(TemplateMilestoneComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
