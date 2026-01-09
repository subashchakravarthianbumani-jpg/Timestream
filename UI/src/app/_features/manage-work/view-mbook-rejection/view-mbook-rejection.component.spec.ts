import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewMbookRejectionComponent } from './view-mbook-rejection.component';

describe('ViewMbookRejectionComponent', () => {
  let component: ViewMbookRejectionComponent;
  let fixture: ComponentFixture<ViewMbookRejectionComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ViewMbookRejectionComponent]
    });
    fixture = TestBed.createComponent(ViewMbookRejectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
