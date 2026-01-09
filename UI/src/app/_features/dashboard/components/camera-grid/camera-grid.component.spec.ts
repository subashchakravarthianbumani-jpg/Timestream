import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CameraGridComponent } from './camera-grid.component';

describe('CameraGridComponent', () => {
  let component: CameraGridComponent;
  let fixture: ComponentFixture<CameraGridComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CameraGridComponent]
    });
    fixture = TestBed.createComponent(CameraGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
