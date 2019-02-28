import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LoginFormComponent } from './login-form/login-form.component';
import { AccountRoutingModule } from './account.routing';

@NgModule({
  imports: [
    CommonModule, FormsModule, AccountRoutingModule
  ],
  declarations: [LoginFormComponent]
})
export class AccountModule { }
