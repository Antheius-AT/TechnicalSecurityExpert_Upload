import { Injectable } from '@angular/core';
import { User } from '../../../../SharedData/user';
import { HttpClient } from '@angular/common/http';
import { AuthToken } from '../../../../SharedData/authToken'
import { RegistrationToken } from '../../../../SharedData/registrationToken'

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  private isAuthenticated : boolean;

  constructor(private httpClient : HttpClient) { 
    this.isAuthenticated = false;
  }

  /**
   * Tries to authenticate a user with the database server.
   * @param user The user that is trying to authenticate.
   */
  tryAuthenticateUser(user : User) : Promise<boolean> {
    return this.httpClient.post<AuthToken>('http://localhost:41005/authenticate/', user).toPromise()
    .then((token) =>{
      this.isAuthenticated = token.isAuthenticated;
      return token.isAuthenticated;
    })
    .catch(() =>{
      // Think about logging error.
      return false;
    });
  }

  /**
   * Tries to register a new user.
   * @param user The user to register.
   */
  tryRegisterUser(user : User) : Promise<boolean>{
    return this.httpClient.post<RegistrationToken>('http://localhost:41005/registerUser/', user).toPromise()
    .then((token) =>{
      return token.registrationSuccessful;
    })
    .catch(() =>{
      // Think abuot logging error.
      return false;
    });
  }

  /**
   * Gets a value indicating whether the user is currently authenticated.
   */
  IsUserAuthenticated() : boolean{
    return this.isAuthenticated;
  }
}
