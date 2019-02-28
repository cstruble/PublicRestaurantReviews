import { Component, OnInit, Input } from '@angular/core';
import { Router, ActivatedRoute, ParamMap, CanActivate } from '@angular/router';
import { Location } from '@angular/common';
import { switchMap } from 'rxjs/operators';

import { UserService } from '../../shared/services/user.service';
import { RestaurantService } from '../../shared/services/restaurant.service';
import { Restaurant } from '../../shared/models/restaurant';
import { RestaurantLocation } from '../../shared/models/restaurant-location';
import { LocationService } from '../../shared/services/location.service';

@Component({
  selector: 'app-restaurant-detail',
  templateUrl: './restaurant-detail.component.html',
  styleUrls: ['./restaurant-detail.component.css']
})

export class RestaurantDetailComponent implements OnInit, CanActivate {
  private restaurant: Restaurant;
  public locations: RestaurantLocation[];
  selectedLocation: RestaurantLocation;
  private dataSaved = false;
  isUserLoggedIn = false;
  isUserAdmin = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,
    private locationService: LocationService,
    private restaurantService: RestaurantService,
    private userService: UserService
  )
  {
    this.userService.isLoggedIn().subscribe(b => this.isUserLoggedIn = b);
    this.userService.isAdmin().subscribe(b => this.isUserAdmin = b);
  }

  canActivate(): boolean {
    if (!this.userService.isLoggedIn()) {
      this.router.navigate(['/account/login']);
      return false;
    }
  }

  ngOnInit() {
    let id = +this.route.snapshot.paramMap.get('id');
    this.restaurantService.getRestaurant(id).subscribe(restaurant => {
      if (restaurant) {
        this.restaurant = restaurant;
      } else {
        this.restaurant = new Restaurant();
      }
      this.locationService.getLocations().subscribe(locations => this.UpdateLocations(locations));
    });
  }

  UpdateLocations(locations: RestaurantLocation[]): void {
    this.locations = locations;

    if (locations && locations.length > 0 && this.restaurant && this.restaurant.Location)
      this.selectedLocation = this.locations.find(l => l.Id === this.restaurant.Location.Id);
  }

  onChangeLocation(newLocation: RestaurantLocation) {
    this.restaurant.Location = newLocation;
    this.selectedLocation = newLocation;
  }

  public onFormSubmit({ value, valid }: { value: Restaurant, valid: boolean }) {
    this.dataSaved = false;
    this.restaurant.Name = value.Name;
    this.restaurant.Location = this.selectedLocation;
    this.restaurantService.addEditRestaurant(this.restaurant).subscribe(success => {
      if (success)
        this.dataSaved = true;
      else
        this.dataSaved = false;
    });
  }

}
