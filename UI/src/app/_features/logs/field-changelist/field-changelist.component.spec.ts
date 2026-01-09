import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FieldChangelistComponent } from './field-changelist.component';

describe('FieldChangelistComponent', () => {
  let component: FieldChangelistComponent;
  let fixture: ComponentFixture<FieldChangelistComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [FieldChangelistComponent]
    });
    fixture = TestBed.createComponent(FieldChangelistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
