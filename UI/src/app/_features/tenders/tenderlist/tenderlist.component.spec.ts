import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TenderlistComponent } from './tenderlist.component';

describe('TenderlistComponent', () => {
  let component: TenderlistComponent;
  let fixture: ComponentFixture<TenderlistComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [TenderlistComponent]
    });
    fixture = TestBed.createComponent(TenderlistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
