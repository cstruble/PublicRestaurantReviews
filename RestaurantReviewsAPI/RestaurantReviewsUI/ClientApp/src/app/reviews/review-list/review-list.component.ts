import { Component, Inject } from '@angular/core';
import { LocationService } from '../../shared/services/location.service';
import { RestaurantService } from '../../shared/services/restaurant.service';
import { ReviewService } from '../../shared/services/review.service';
import { RestaurantLocation } from '../../shared/models/restaurant-location';
import { Restaurant } from '../../shared/models/restaurant';
import { Review } from '../../shared/models/review';
import { UserService } from '../../shared/services/user.service';
import { Observable, BehaviorSubject } from 'rxjs';
import { User } from '../../shared/models/user';

@Component({
  selector: 'app-reviews',
  templateUrl: './review-list.component.html'
})
export class ReviewListComponent {
  public locations: RestaurantLocation[];
  selectedLocation: RestaurantLocation;
  public restaurants: Restaurant[];
  public users: User[];
  selectedRestaurant: Restaurant;
  public reviews: Review[];
  public username: string;
  selectedUser: User;

  constructor(private locationService: LocationService, private restaurantService: RestaurantService, private reviewService: ReviewService, private userService: UserService) {
    this.username = this.userService.getUsername();
  }

  ngOnInit() {
    this.locationService.getLocations().subscribe(locations => this.UpdateLocations(locations));
    this.userService.getUsers().subscribe(users => this.users = users);
    this.restaurants = [];
  }

  UpdateLocations(locations: RestaurantLocation[]): void {
    this.locations = locations;

    if (locations && locations.length > 0) {
      this.selectedLocation = this.locations[0];
      this.restaurantService.setLocationRestaurants(this.selectedLocation);
      this.restaurantService.getRestaurantsByLocation().subscribe(restaurants => this.restaurants = restaurants);
    }
  }

  onChangeLocation(newLocation: RestaurantLocation) {
    if (newLocation) {
      this.selectedUser = null;
      this.selectedLocation = newLocation;
      this.restaurantService.setLocationRestaurants(newLocation);
      this.restaurantService.getRestaurantsByLocation().subscribe(restaurants => this.restaurants = restaurants);
    }
  }

  onChangeRestaurant(newRestaurant: Restaurant) {
    if (newRestaurant) {
      this.selectedRestaurant = newRestaurant;
      this.selectedUser = null;
      this.reviewService.setRestaurantReviews(newRestaurant);
      this.reviewService.getRestaurantReviews().subscribe(reviews => this.reviews = reviews);
    }
  }

  onChangeUser(newUser: User) {
    if (newUser) {
      this.selectedUser = newUser;
      this.selectedLocation = null;
      this.selectedRestaurant = null;
      this.reviewService.setReviewsByUser(this.selectedUser);
      this.reviewService.getRestaurantReviews().subscribe(reviews => this.reviews = reviews);
    }
  }

  delete(review: Review) {
    this.reviewService.deleteReview(review).subscribe(rev => {
      if (this.selectedRestaurant) {
        this.reviewService.setRestaurantReviews(this.selectedRestaurant);
        this.reviewService.getRestaurantReviews().subscribe(reviews => this.reviews = reviews);
      } else if (this.selectedUser) {
        this.reviewService.setReviewsByUser(this.selectedUser);
        this.reviewService.getRestaurantReviews().subscribe(reviews => this.reviews = reviews);
      }
    });
  }
}
