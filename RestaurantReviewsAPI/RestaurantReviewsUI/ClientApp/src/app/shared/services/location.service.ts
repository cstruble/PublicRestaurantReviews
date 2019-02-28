import { Injectable, Inject } from '@angular/core';
import { RestaurantLocation } from '../models/restaurant-location';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { of } from 'rxjs/observable/of';

@Injectable()
export class LocationService {
  private locations: RestaurantLocation[];
  obsLocs$ = new BehaviorSubject<RestaurantLocation[]>(this.locations);

  constructor(private http: HttpClient) {
    this.http.get<RestaurantLocation[]>('http://localhost:8080/api/Locations').subscribe(result => {
      this.locations = result;
      this.obsLocs$.next(this.locations);
    }, error => console.log(error));
  }

  getLocations(): Observable<RestaurantLocation[]> {
    return this.obsLocs$;
  }

  getLocation(id: number): Observable<RestaurantLocation> {
    return of(this.locations.find(location => location.Id === id));
  }
}
