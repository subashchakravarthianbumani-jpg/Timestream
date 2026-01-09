import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MbookStatsComponent } from './mbook-stats.component';

describe('MbookStatsComponent', () => {
  let component: MbookStatsComponent;
  let fixture: ComponentFixture<MbookStatsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MbookStatsComponent]
    });
    fixture = TestBed.createComponent(MbookStatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
