import { Injectable } from '@angular/core';
import { Router, CanActivate, RouterStateSnapshot, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from './shared/services/user.service';
import { Observable } from 'rxjs';

@Injectable()
export class AuthGuard implements CanActivate {
  private state: RouterStateSnapshot;
  constructor(private userService: UserService, private router: Router, private activeRoute: ActivatedRoute) {
    this.state = router.routerState.snapshot;
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {

    if (!this.userService.isLoggedIn()) {
      this.router.navigate(['/account/login'], { queryParams: { returnUrl: this.createURL(route) } });
      return false;
    }

    return true;
  }

  createURL(route: ActivatedRouteSnapshot): string {
    var url = this.state.url;
    if (route && route.url && route.url.length > 0) {
      url = "";
      for (var i = 0; i < route.url.length; i++) {
        url += "/" + route.url[i];
      }
    }
    return url;
  }
}
