import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TwoColumnConfigComponent } from './two-column-config.component';

describe('TwoColumnConfigComponent', () => {
  let component: TwoColumnConfigComponent;
  let fixture: ComponentFixture<TwoColumnConfigComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [TwoColumnConfigComponent]
    });
    fixture = TestBed.createComponent(TwoColumnConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
