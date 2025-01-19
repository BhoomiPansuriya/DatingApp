import { Component, inject, OnInit } from '@angular/core';
import { RegisterComponent } from "../register/register.component";
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RegisterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit{
  registerMode = true;
  http = inject(HttpClient); 
  user: any;
  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  ngOnInit(): void {
    this.getUsers();
  }

  getUsers() {
    this.http.get("https://localhost:5145/api/users").subscribe({
      next: response => this.user = response,
      error:error => console.log(error),
      complete:() => console.log("Request is complete")
    })
  }
  cancelRegisterMode(event:boolean) {
    console.log("cancel request mode"+event);
    this.registerMode = !event;
  }
}
