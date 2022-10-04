import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PoolDiaryComponent } from './pool-diary.component';

describe('PoolDiaryComponent', () => {
  let component: PoolDiaryComponent;
  let fixture: ComponentFixture<PoolDiaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PoolDiaryComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PoolDiaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
