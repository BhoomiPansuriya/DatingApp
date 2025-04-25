import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { NgIf } from '@angular/common';
import { TextInputComponent } from "../_forms/text-input/text-input.component";
import { DatePickerComponent } from '../_forms/date-picker/date-picker.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, TextInputComponent, DatePickerComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent  implements OnInit{
  private accountService = inject(AccountService);
  private fb = inject(FormBuilder);
  private router = inject(Router);
  @Output() cancelRequest = new EventEmitter();
  model: any={};
  validationError: string[] = [];
  regiserForm : FormGroup = new FormGroup({});
  register() {
    const dob = this.getDateOnly(this.regiserForm.get('dateOfBirth')?.value);
    this.regiserForm.patchValue({dateOfBirth: dob});
      this.accountService.register(this.regiserForm.value).subscribe({
        next: _ => {
          this.router.navigateByUrl('/members');
        },
        error: (error: any) => this.validationError = error      
      })
  }
  ngOnInit(): void {
    this.initializeFrom();  
  }

  initializeFrom() {
    this.regiserForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs:['', Validators.required],
      dateOfBirth:['', Validators.required],
      city:['', Validators.required],
      country:['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    });
    this.regiserForm.controls['password'].valueChanges.subscribe({
      next: () =>this.regiserForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null: {isMatching: true}
    }
  }

  cancel() {
    console.log("cancel called");
    this.cancelRequest.emit(false);
  }
  private getDateOnly(dob: string|undefined) {
    if(!dob) return;
    return new Date(dob).toISOString().slice(0,10);
  }
}
