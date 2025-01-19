import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent  implements OnInit{
  private accountService = inject(AccountService)
  @Output() cancelRequest = new EventEmitter();
  model: any={};

  register() {
      this.accountService.register(this.model).subscribe({
        next:(response: any) => {
          console.log(response);
  
        },
        error: (error: any) => console.log(error)      
      })
  }
  ngOnInit(): void {
  }

  cancel() {
    console.log("cancel called");
    this.cancelRequest.emit(false);
  }
}
