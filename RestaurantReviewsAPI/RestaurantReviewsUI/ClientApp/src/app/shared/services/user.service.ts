import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs/observable/of';
import { User } from '../models/user';

@Injectable()
export class UserService {

  private loggedIn = false;
  private loggedIn$ = new BehaviorSubject<boolean>(this.loggedIn);
  private roles = [];
  private roles$ = new BehaviorSubject<string[]>(this.roles);
  private hasAdmin = false;
  private hasAdmin$ = new BehaviorSubject<boolean>(this.hasAdmin);
  private username;
  private users = [];
  private users$ = new BehaviorSubject<User[]>(this.users);

  constructor(private http: HttpClient) {
    this.loggedIn = !!localStorage.getItem('auth_token');
    this.http.get<User[]>('http://localhost:8080/api/Account/Users').subscribe(result => {
      this.users = result;
      this.users$.next(this.users);
    }, error => console.log(error));
  }

  login(userName: string, password: string) {
    this.username = userName;
    var headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded', 'No-Auth': 'True' });
    var userData = "username=" + userName + "&password=" +password + "&grant_type=password";

    return this.http
      .post('http://localhost:8080/Token', userData, { headers: headers })
      .pipe(map(res => {
        localStorage.setItem('auth_token', res['access_token']);
        this.loggedIn = true;
        this.loggedIn$.next(this.loggedIn);
        this.roles = res['roles'];
        if (this.roles && this.roles.indexOf("Admin") > -1) {
          this.hasAdmin = true;
        }
        else {
          this.hasAdmin = false;
        }
        this.hasAdmin$.next(this.hasAdmin);
        this.roles$.next(this.roles);
        return true;
      }), catchError(this.handleError));
  }

  getUsername(): string {
    return this.username;
  }

  protected handleError(error: any) {
    var applicationError = error.headers.get('Application-Error');

    // either applicationError in header or model error in body
    if (applicationError) {
      return Observable.throw(applicationError);
    }

    var modelStateErrors: string = '';
    var serverError = error;

    if (!serverError.type) {
      for (var key in serverError) {
        if (serverError[key])
          modelStateErrors += serverError[key] + '\n';
      }
    }

    modelStateErrors = modelStateErrors = '' ? null : modelStateErrors;
    return Observable.throw(modelStateErrors || 'Server error');
  }

  isLoggedIn(): Observable<boolean> {
    return this.loggedIn$;
  }

  isAdmin(): Observable<boolean> {
    return this.hasAdmin$;
  }

  getUsers(): Observable<User[]> {
    return this.users$;
  }

}
