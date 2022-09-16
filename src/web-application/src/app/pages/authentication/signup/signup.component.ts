import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { AuthService } from 'src/app/core/services/auth.service';
import { UserRegister } from 'src/app/shared/models/authenticate';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent implements OnInit {
private userRegister!: UserRegister;
  public msgError!: string;
  public loading: Subject<boolean> = this.authService.isLoading;

  public formSignup: FormGroup = this.formBuilder.group({
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
    passwordConfirm: ['', [Validators.required]]
  });

  constructor(private formBuilder: FormBuilder, private authService: AuthService) { }

  ngOnInit(): void {
  }

  public submitForm(){
    this.userRegister = this.formSignup.value;
    if(this.formSignup.valid){
      this.authService.signup(this.userRegister).subscribe({
        next: (res) => {
          alert(`USUÁRIO CADASTRADO: ${this.userRegister.email}`);
        },
        error: (e) => (this.msgError = e.errors.Mensagens[0]),
      });
    }
  }
}
