import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';
import {JwtHelperService} from '@auth0/angular-jwt';
import { connectableObservableDescriptor } from 'rxjs/internal/observable/ConnectableObservable';
import { environment } from 'src/environments/environment';

@Injectable({ //inject things into our service
  providedIn: 'root'
})
export class AuthService {
  //we want the HttpClient from Angular/common/http
  constructor(private http: HttpClient) { }
  baseUrl = environment.apiUrl + 'auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;

  login(model: any)
  {
    //pass request and model
    //the response returns a token "token" : value
    return this.http.post(this.baseUrl + 'login', model)
      .pipe(
        map((response: any) => {
          const user = response;
          if(user){
            localStorage.setItem('token', user.token);  //set token in local storage
            this.decodedToken = this.jwtHelper.decodeToken(user.token);
            console.log(this.decodedToken);
          }
        })
      )
  }

  register(model: any){
    return this.http.post(this.baseUrl + 'register', model);
  }

  loggedIn(){
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token); //return true if token is NOT expired (or its a valid token)
  }

}
