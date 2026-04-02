import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminAssign } from './admin-assign';

describe('AdminAssign', () => {
  let component: AdminAssign;
  let fixture: ComponentFixture<AdminAssign>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminAssign]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminAssign);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
