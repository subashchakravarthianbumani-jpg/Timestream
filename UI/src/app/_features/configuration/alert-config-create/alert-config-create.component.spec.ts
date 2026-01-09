import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AlertConfigCreateComponent } from './alert-config-create.component';

describe('AlertConfigCreateComponent', () => {
  let component: AlertConfigCreateComponent;
  let fixture: ComponentFixture<AlertConfigCreateComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AlertConfigCreateComponent]
    });
    fixture = TestBed.createComponent(AlertConfigCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
