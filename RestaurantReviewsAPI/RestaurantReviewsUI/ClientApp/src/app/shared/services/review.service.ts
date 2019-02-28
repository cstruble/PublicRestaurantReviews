import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Restaurant } from '../models/restaurant';
import { Review } from '../models/review';
import { Observable, BehaviorSubject } from 'rxjs';
import { of } from 'rxjs/observable/of';
import { map } from 'rxjs/operators';
import { User } from '../models/user';

@Injectable()
export class ReviewService {
  private reviews: Review[];
  reviews$ = new BehaviorSubject<Review[]>(this.reviews);
  private review: Review;
  review$ = new BehaviorSubject<Review>(this.review);

  constructor(private http: HttpClient) { }

  setRestaurantReviews(restaurant: Restaurant) {
    if (restaurant && restaurant.Id)
      this.http.get<Review[]>('http://localhost:8080/api/restaurants/' + restaurant.Id + '/reviews').subscribe(reviews => this.updateRestaurantReviews(reviews));
  }

  getRestaurantReviews(): Observable<Review[]> {
    return this.reviews$;
  }

  setReviewsByUser(user: User) {
    if (user) {
      this.http.get<Review[]>('http://localhost:8080/api/reviews/reviewsByUser/' + user.Id).subscribe(reviews => this.updateRestaurantReviews(reviews));
    }
  }

  updateRestaurantReviews(restReviews: Review[]) {
    this.reviews = restReviews;
    this.reviews$.next(this.reviews);
  }

  addEditReview(review: Review) {
    if (review && review.Restaurant && review.Stars && review.Description) {
      console.log("review = " + JSON.stringify(review));
      var token = 'bearer ' + localStorage.getItem('auth_token');
      var headers = new HttpHeaders({ 'Authorization': token });
      return this.http.post<Review>('http://localhost:8080/api/reviews', review, { headers: headers }).pipe(map(r => {
        this.reviews.push(r);
        this.reviews$.next(this.reviews);
        return true;
      }));
    }
    return of(false);
  }

  getReview(id: number): Observable<Review> {
    var token = 'bearer ' + localStorage.getItem('auth_token');
    var headers = new HttpHeaders({ 'Authorization': token });
    this.http.get<Review>('http://localhost:8080/api/reviews/' + id, { headers: headers }).subscribe(result => {
      this.review = result;
      this.review$.next(this.review);
    }, error => console.log(error));
    return this.review$;
  }

  deleteReview(review: Review) {
    var token = 'bearer ' + localStorage.getItem('auth_token');
    var headers = new HttpHeaders({ 'Authorization': token });
    return this.http.delete<Review>('http://localhost:8080/api/reviews/' + review.Id, { headers: headers }).pipe(map(rev => {
      if (this.reviews$ && this.reviews) {
        this.reviews.filter(r => r.Id !== rev.Id);
        this.reviews$.next(this.reviews);
      }
    }));
  }
}
