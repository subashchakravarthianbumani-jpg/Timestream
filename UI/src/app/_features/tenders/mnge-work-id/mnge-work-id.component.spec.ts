import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MngeWorkIdComponent } from './mnge-work-id.component';

describe('MngeWorkIdComponent', () => {
  let component: MngeWorkIdComponent;
  let fixture: ComponentFixture<MngeWorkIdComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MngeWorkIdComponent]
    });
    fixture = TestBed.createComponent(MngeWorkIdComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
