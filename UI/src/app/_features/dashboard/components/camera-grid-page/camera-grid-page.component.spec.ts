import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CameraGridPageComponent } from './camera-grid-page.component';

describe('CameraGridPageComponent', () => {
  let component: CameraGridPageComponent;
  let fixture: ComponentFixture<CameraGridPageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CameraGridPageComponent]
    });
    fixture = TestBed.createComponent(CameraGridPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
