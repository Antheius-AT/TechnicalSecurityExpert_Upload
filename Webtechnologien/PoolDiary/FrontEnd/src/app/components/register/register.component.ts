import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { User } from '../../../../../SharedData/user';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  public isRegistrationActive : boolean;
  public password : string;
  public passwordConfirmation : string;
  public username : string;

  constructor(private router : Router, private authenticationService : AuthenticationService) { 
    this.isRegistrationActive = true;
  }

  /**
   * Makes a post request to confirm registration of a new user.
   */
  public confirmRegistration(){
    // Hash password first.
        let user = new User(this.username, this.password);
        this.authenticationService.tryRegisterUser(user)
        .then((result)=>{
          if (result){
            this.router.navigate(['/login']);
          }
          else{
            this.username = '';
            this.password = '';
            this.passwordConfirmation = '';
          }
        })
        .catch((error) =>{
          // think about logging error.
          this.username = '';
          this.password = '';
       });
  }

  /**
   * Checks whether password and password confirmation match.
   */
  public doPasswordsMatch() : boolean{

    if (!this.username || !this.password || !/\S/.test(this.username) || !/\S/.test(this.password)){
      return false;
    }
    else{
      return this.password.length > 6 && this.password === this.passwordConfirmation;
    }
}

  /**
   * Changes form to login.
   */
  public changeForm(){
    this.router.navigate(['/login']);
  }

  ngOnInit(): void {
  }

}
