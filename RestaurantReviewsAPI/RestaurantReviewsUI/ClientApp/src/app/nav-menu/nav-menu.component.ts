import { Component } from '@angular/core';
import { UserService } from '../shared/services/user.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;
  isUserLoggedIn = false;
  isUserAdmin = false;

  constructor(private userService: UserService) {
    this.userService.isLoggedIn().subscribe(b => this.isUserLoggedIn = b);
    this.userService.isAdmin().subscribe(b => this.isUserAdmin = b);
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
