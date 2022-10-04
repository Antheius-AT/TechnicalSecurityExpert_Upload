import { TestBed } from '@angular/core/testing';

import { FetchEntriesService } from './fetch-entries.service';

describe('FetchEntriesService', () => {
  let service: FetchEntriesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FetchEntriesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
