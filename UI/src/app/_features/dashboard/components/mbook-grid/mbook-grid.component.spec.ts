import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MbookGridComponent } from './mbook-grid.component';

describe('MbookGridComponent', () => {
  let component: MbookGridComponent;
  let fixture: ComponentFixture<MbookGridComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MbookGridComponent]
    });
    fixture = TestBed.createComponent(MbookGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
