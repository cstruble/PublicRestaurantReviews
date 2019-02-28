import { Component } from '@angular/core';
import { UserService } from '../shared/services/user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  public isUserLoggedIn: boolean;
  public isUserAdmin: boolean;

  constructor(private userService: UserService) {
    this.userService.isLoggedIn().subscribe(b => this.isUserLoggedIn = b);
    this.userService.isAdmin().subscribe(b => this.isUserAdmin = b);
  }
}
