import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewWorkIdComponent } from './view-work-id.component';

describe('ViewWorkIdComponent', () => {
  let component: ViewWorkIdComponent;
  let fixture: ComponentFixture<ViewWorkIdComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ViewWorkIdComponent]
    });
    fixture = TestBed.createComponent(ViewWorkIdComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
