import { TestBed } from '@angular/core/testing';

import { GuardHomeRouteService } from './guard-home-route.service';

describe('GuardHomeRouteService', () => {
  let service: GuardHomeRouteService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GuardHomeRouteService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
