import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { UserDto } from '../../../../Shared/user.dto'

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private loggedIn : boolean = false;
  constructor(private httpClient : HttpClient) { 
  }


  login(user : UserDto) {
    return this.httpClient.post('http://localhost:41005/user/authenticate', user);
  }
}
