import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewMbookHistoryComponent } from './view-mbook-history.component';

describe('ViewMbookHistoryComponent', () => {
  let component: ViewMbookHistoryComponent;
  let fixture: ComponentFixture<ViewMbookHistoryComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ViewMbookHistoryComponent]
    });
    fixture = TestBed.createComponent(ViewMbookHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
