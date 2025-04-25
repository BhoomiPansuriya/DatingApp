import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { of } from 'rxjs/internal/observable/of';
import { tap } from 'rxjs';
import { Photo } from '../_models/photos';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userparams';
import { setPaginatedResponse, setPaginationHeaders } from './paginationHelper';


@Injectable({
  providedIn: 'root'
})
export class MembersService {

  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  members = signal<Member[]>([]);
  paginatedResult = signal<PaginatedResult<Member[]>|null>(null);
  memberCache = new Map();

  getMembers(userParams: UserParams) {
    const response  = this.memberCache.get(Object.values(userParams).join('-'));
    if(response) return setPaginatedResponse(response, this.paginatedResult);
    let params = setPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);
    return this.http.get<Member[]>(this.baseUrl + 'users', {observe:'response', params}).subscribe({
      next: response => {
       setPaginatedResponse(response, this.paginatedResult);
       this.memberCache.set(Object.values(userParams).join('-'),response);
      }
    });

  }

  getMember(username: string) {
    const member: Member = [...this.memberCache.values()]
    .reduce((arr,ele)=> arr.concat(ele.body), [])
    .find((m:Member)=> m.userName === username);
    if(member) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users' , member).pipe(
      tap(()=> {
        this.members.update(members => members.map(m=>m.userName === member.userName ? member :m))
      })
    )
  }
  setMainPhoto(photo: Photo) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/'+ photo.id, {}).pipe(
      tap(()=>{
        this.members.update(members => members.map(m=>{
          if(m.photos.includes(photo)) {
            m.photoUrl = photo.url
          }
          return m;
        }))
      })
    )
  }

  deletePhoto(photo: Photo) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/'+photo.id).pipe(
      tap(()=>{
        this.members.update(members => members.map(m=> {
          if(m.photos.includes(photo)) {
            m.photos =m.photos.filter(x=>x.id !== photo.id)
          }
          return m
        }))
      })
    )
  }
}
