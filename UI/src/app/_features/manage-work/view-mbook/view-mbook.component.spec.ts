import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewMbookComponent } from './view-mbook.component';

describe('ViewMbookComponent', () => {
  let component: ViewMbookComponent;
  let fixture: ComponentFixture<ViewMbookComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ViewMbookComponent]
    });
    fixture = TestBed.createComponent(ViewMbookComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
