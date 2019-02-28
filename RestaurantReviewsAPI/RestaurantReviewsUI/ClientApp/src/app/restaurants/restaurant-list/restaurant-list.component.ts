import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Restaurant } from '../../shared/models/restaurant';
import { RestaurantService } from '../../shared/services/restaurant.service';

@Component({
  selector: 'app-restaurants',
  templateUrl: './restaurant-list.component.html'
})
export class RestaurantListComponent {
  public restaurants: Restaurant[];

  constructor(private restaurantService: RestaurantService) {
  }

  ngOnInit() {
    this.restaurantService.getRestaurants().subscribe(restaurants => this.restaurants = restaurants);
  }
}
