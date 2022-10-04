import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { User } from '../../../../../SharedData/user';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  public isLoginActive : boolean;
  public password : string;
  public username : string;

  constructor(private router : Router, private authenticationSerice : AuthenticationService) { 
    this.isLoginActive = true;
  }

  ngOnInit(): void {
  }

  public changeForm(){
    this.router.navigate(['/register']);
  }

  public confirmLogin(){
    // Need to hash this.
    let user = new User(this.username, this.password);
    this.authenticationSerice.tryAuthenticateUser(user)
    .then((success)=>{
      if (success){
        this.router.navigate(['/home']);
      }
      else{
        this.username = '';
        this.password = '';
      }
    })
    .catch((error)=>{
      // maybe log error?
      this.username = '';
      this.password = '';
    });
  }
}
