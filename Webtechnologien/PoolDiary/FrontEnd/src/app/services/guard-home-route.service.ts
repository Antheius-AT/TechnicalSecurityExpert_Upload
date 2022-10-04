import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { AuthenticationService } from './authentication.service';

@Injectable({
  providedIn: 'root',
})
export class GuardHomeRouteService implements CanActivate {
  constructor(private authenticationService : AuthenticationService, private router : Router) {}

  /**
   * Determines whether a user is able to navigate to the requested URL.
   * @param route The route in question.
   * @param state The router state snapshot.
   */
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {

    if (!this.authenticationService.IsUserAuthenticated()) {
      alert('You are not allowed to view this page. Redirecting you to Login..');
      this.router.navigate(['login']);

      return false;
    }

    return true;
  }
}
