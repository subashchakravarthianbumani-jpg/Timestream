import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GnrteWorkIdComponent } from './gnrte-work-id.component';

describe('GnrteWorkIdComponent', () => {
  let component: GnrteWorkIdComponent;
  let fixture: ComponentFixture<GnrteWorkIdComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [GnrteWorkIdComponent]
    });
    fixture = TestBed.createComponent(GnrteWorkIdComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
