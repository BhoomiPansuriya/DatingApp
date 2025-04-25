import { Component, inject, OnInit } from '@angular/core';
import { LikesService } from '../_services/likes.service';
import { Member } from '../_models/member';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { FormsModule } from '@angular/forms';
import { MemberCardComponent } from "../members/member-card/member-card.component";
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-list',
  standalone: true,
  imports: [ButtonsModule, FormsModule, MemberCardComponent, PaginationModule],
  templateUrl: './list.component.html',
  styleUrl: './list.component.css'
})
export class ListComponent implements OnInit {
  likesService = inject(LikesService);
  members: Member[] = [];
  predicate = 'liked';
  pageNumber = 1;
  pageSize = 5;

  ngOnInit() {
    this.loadLikes();
  }

  loadLikes() {
    this.likesService.getLikes(this.predicate, this.pageNumber, this.pageSize);
  }

  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadLikes();
    }
  }


  getTitle() {
    switch (this.predicate) {
      case 'liked':
        return 'People you liked';
      case 'likedBy':
        return 'People who liked you';
      default:
        return 'Likes';
    }
  }

  ngOnDestroy() {
    this.likesService.paginatedResult.set(null); // Clear the paginated result when the component is destroyed

  }
}
