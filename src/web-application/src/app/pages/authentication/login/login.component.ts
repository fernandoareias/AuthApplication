import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { AuthService } from 'src/app/core/services/auth.service';
import { UserLogin } from 'src/app/shared/models/authenticate';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  private login!: UserLogin;
  public msgError!: string;
  public loading: Subject<boolean> = this.authService.isLoading;

  public formAuth: FormGroup = this.formBuilder.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  constructor(private formBuilder: FormBuilder, private authService: AuthService) { }

  ngOnInit(): void {
  }

  public submitForm(){
    this.login = this.formAuth.value;
    if(this.formAuth.valid){
      this.authService.sign(this.login).subscribe({
        next: (res) => res,
        error: (e) => (this.msgError = e[0]),
      });
    }
  }
}
