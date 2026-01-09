import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TenderStatsComponent } from './tender-stats.component';

describe('TenderStatsComponent', () => {
  let component: TenderStatsComponent;
  let fixture: ComponentFixture<TenderStatsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [TenderStatsComponent]
    });
    fixture = TestBed.createComponent(TenderStatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
