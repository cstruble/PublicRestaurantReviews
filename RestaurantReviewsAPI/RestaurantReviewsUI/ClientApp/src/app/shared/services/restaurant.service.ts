import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { of } from 'rxjs/observable/of';
import { map } from 'rxjs/operators';
import { RestaurantLocation } from '../models/restaurant-location';
import { Restaurant } from '../models/restaurant';

@Injectable()
export class RestaurantService {
  private restaurants: Restaurant[];
  private restaurantsByLocation: Restaurant[];
  obsRests$ = new BehaviorSubject<Restaurant[]>(this.restaurants);
  restsByLoc$ = new BehaviorSubject<Restaurant[]>(this.restaurantsByLocation);

  constructor(private http: HttpClient) {
    this.initializeRestaurants();
  }

  private initializeRestaurants() {
    this.http.get<Restaurant[]>('http://localhost:8080/api/Restaurants').subscribe(result => {
      this.restaurants = result;
      this.obsRests$.next(this.restaurants);
    }, error => console.log(error));
  }

  getRestaurants(): Observable<Restaurant[]> {
      return this.obsRests$;
  }

  getRestaurant(id: number): Observable<Restaurant> {
    return this.getRestaurants().map(rest => this.filterRestaurants(id));
  }

  private filterRestaurants(id: number) {
    if (this.restaurants) {
      return this.restaurants.find(restaurant => restaurant.Id == id);
    }
    return null;
  }

  setLocationRestaurants(location: RestaurantLocation) {
    if (location && location.Id)
      this.http.get<Restaurant[]>('http://localhost:8080/api/locations/' + location.Id + '/restaurants').subscribe(restaurants => this.updateRestaurantsByLocation(restaurants));
    else
      this.restsByLoc$.next(null);
  }

  getRestaurantsByLocation(): Observable<Restaurant[]> {
    return this.restsByLoc$;
  }

  updateRestaurantsByLocation(restsByLocation: Restaurant[]) {
    this.restaurantsByLocation = restsByLocation;
    this.restsByLoc$.next(this.restaurantsByLocation);
  }

  addEditRestaurant(restaurant: Restaurant) {
    if (restaurant && restaurant.Location && restaurant.Name) {
      var token = 'bearer ' + localStorage.getItem('auth_token');
      var headers = new HttpHeaders({ 'Authorization': token });
      return this.http.post<Restaurant>('http://localhost:8080/api/restaurants', restaurant, { headers: headers }).pipe(map(r => {
        this.restaurants.filter(function (others) {
          return others.Id !== restaurant.Id;
        });
        this.restaurants.push(r);
        this.obsRests$.next(this.restaurants);
        return true;
      }));
    }
    return of(false);
  }
}
