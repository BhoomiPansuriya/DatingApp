import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { map } from 'rxjs';
import { User } from '../_models/User';
import { environment } from '../../environments/environment';
import { LikesService } from './likes.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  private likeService = inject(LikesService);
  baseUrl = environment.apiUrl;
  currentUser = signal<User | null>(null);
  constructor() { }
  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map(user => {
        if (user) {
          this.setCurrentuser(user);  
        }
      })
    );
  }
  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }

  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
        if (user) {
          this.setCurrentuser(user);
        }
        return user;
      })
    );
  }
  setCurrentuser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
    this.likeService.getLikeIds(); // Fetch like IDs for the current user
  }
}
