import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl;


constructor(private http: HttpClient) { }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + 'users/getusers');
  }

  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/getuser/' + id);
  }

  updateUser(id: number, user: User)
  {
    return this.http.put(this.baseUrl + 'users/userupdate/' + id, user);
  }
}
