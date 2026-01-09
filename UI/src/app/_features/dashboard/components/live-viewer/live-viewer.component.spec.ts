import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LiveViewerComponent } from './live-viewer.component';

describe('LiveViewerComponent', () => {
  let component: LiveViewerComponent;
  let fixture: ComponentFixture<LiveViewerComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [LiveViewerComponent]
    });
    fixture = TestBed.createComponent(LiveViewerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
