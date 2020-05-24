import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';

@Injectable({ //inject things into our service
  providedIn: 'root'
})
export class AuthService {
  //we want the HttpClient from Angular/common/http
  constructor(private http: HttpClient) { }
  baseUrl = 'http://localhost:5000/api/auth/';

  login(model: any)
  {
    //pass request and model
    //the response returns a token "token" : value
    return this.http.post(this.baseUrl + 'login', model)
      .pipe(
        map((response: any) => {
          const user = response;
          if(user){
            localStorage.setItem('token', user.token);
          }
        })
      )
  }

  register(model: any){
    return this.http.post(this.baseUrl + 'register', model);
  }

}
