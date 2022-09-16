import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { catchError, map, Observable, Subject, throwError } from 'rxjs';
import { UserLogin, UserRegister } from 'src/app/shared/models/authenticate';
import { UserLoginView } from 'src/app/shared/models/authenticate-responses';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private url: string = "https://localhost:5001/";
  public isLoading = new Subject<boolean>();

  constructor(private httpCliente: HttpClient, private route: Router) { }

  public sign(login: UserLogin): Observable<any>{
    this.isLoading.next(true);
    return this.httpCliente.post<UserLoginView>(`${this.url}api/account/login`, login).pipe(
      map((data) => {
        localStorage.removeItem('access-token');
        localStorage.setItem('access-token', data.accessToken);
        this.isLoading.next(false);
        return this.route.navigate(['']);
      }),
      catchError((err) => {
        this.isLoading.next(false);
        if(err.error.status == 400) return throwError(() => err.error.errors.Mensagens);
        return throwError(() => "Ops... ocorreu um erro, tente novamente mais tarde :(");
      })
    );
  }

  public signup(userRegister: UserRegister): Observable<any>{
    this.isLoading.next(true);
    return this.httpCliente.post<UserLoginView>(`${this.url}api/account/signup`, userRegister).pipe(
      map((data) => {
        this.isLoading.next(false);
        return this.route.navigate(['auth/login']);
      }),
      catchError((err) => {
        this.isLoading.next(false);
        if(err.error.status == 400) return throwError(() => err.error);
        return throwError(() => "Ops... ocorreu um erro, tente novamente mais tarde :(");
      })
    );
  }

  public logout(){
    localStorage.removeItem('access-token');
    return this.route.navigate(['auth/login']);
  }

  public isAuthenticated(): boolean {
    const token = localStorage.getItem('access-token');
    if(token == null) return false;
    const jwtHelper = new JwtHelperService();

    return token != null && !jwtHelper.isTokenExpired(token);
  }
}
