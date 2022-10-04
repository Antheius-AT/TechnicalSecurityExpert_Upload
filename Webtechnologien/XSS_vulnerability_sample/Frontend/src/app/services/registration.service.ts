import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { first } from 'rxjs/operators';
import { UserDto } from '../../../../Shared/user.dto';
import { AlertService } from './alert.service';

@Injectable({
  providedIn: 'root'
})
export class RegistrationService {

  constructor(private httpClient : HttpClient,
                      private alertService : AlertService,
                      private router : Router) {
    
  }

  public register(user: UserDto) {
    return this.httpClient.post('http://localhost:41005/user/add', user);
  }
}
