import { Component, inject, input, OnInit, output, ViewChild } from '@angular/core';
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';
import { TimeagoModule } from 'ngx-timeago';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  imports: [TimeagoModule, FormsModule],
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css'
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?: NgForm;
  @ViewChild('scrollMe') scrollContainer?: any;
  messageService = inject(MessageService);
  username = input.required<string>();
  messages = input.required<Message[]>();
  messageContent = '';
  loading = false;
  updateMessage = output<Message>();
  ngOnInit(): void {
  }

  sendMessage() {
    this.loading = true;
    this.messageService.sendMessage(this.username(), this.messageContent).subscribe({
      next: (message) => {
        this.updateMessage.emit(message);
        this.messageForm?.reset();
      },
      error: () => {
        this.loading = false;
      }
    });
  }
  
  private scrollToBottom() {
    if (this.scrollContainer) {
      this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
    }
  }
}
