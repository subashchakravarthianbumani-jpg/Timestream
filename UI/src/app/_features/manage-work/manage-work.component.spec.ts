import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageWorkComponent } from './manage-work.component';

describe('ManageWorkComponent', () => {
  let component: ManageWorkComponent;
  let fixture: ComponentFixture<ManageWorkComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ManageWorkComponent]
    });
    fixture = TestBed.createComponent(ManageWorkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
