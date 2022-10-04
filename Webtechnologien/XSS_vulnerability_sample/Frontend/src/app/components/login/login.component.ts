
import { HttpClient } from '@angular/common/http';
import { Component, Injectable, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { first } from 'rxjs/operators';
import { UserDto } from '../../../../../Shared/user.dto';
import { AuthenticationService } from '../../services/authentication.service';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
@Injectable()
export class LoginComponent implements OnInit {

  public username: string | undefined;
  public password: string | undefined;
  public isSubmitted: boolean = false;
  public isLoading: boolean = false;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authenticationService: AuthenticationService
    ) {
    }

  ngOnInit(): void {
    
  }


  public onSubmit(): void {
    this.isLoading = true;
    let user = new UserDto(this.username || '', this.password || '');
    this.authenticationService
      .login(user)
      .toPromise()
      .then((response) => {
        this.router.navigate(['/home']);
      })
      .catch((error) => {
        alert('Invalid user data. Please register.');
      });

    this.isLoading = false;
    
  }

  public switchToRegistration() {
    this.router.navigate(['/register']);
  }
}
