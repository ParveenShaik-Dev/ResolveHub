import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminCreateAgent } from './admin-create-agent';

describe('AdminCreateAgent', () => {
  let component: AdminCreateAgent;
  let fixture: ComponentFixture<AdminCreateAgent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCreateAgent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminCreateAgent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
