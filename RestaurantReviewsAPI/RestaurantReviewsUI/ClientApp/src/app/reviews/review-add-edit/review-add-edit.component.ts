import { Component, OnInit } from '@angular/core';
import { Review } from '../../shared/models/review';
import { Router, ActivatedRoute, ParamMap, CanActivate } from '@angular/router';
import { UserService } from '../../shared/services/user.service';
import { Restaurant } from '../../shared/models/restaurant';
import { RestaurantService } from '../../shared/services/restaurant.service';
import { ReviewService } from '../../shared/services/review.service';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-review-add-edit',
  templateUrl: './review-add-edit.component.html',
  styleUrls: ['./review-add-edit.component.css']
})
export class ReviewAddEditComponent implements OnInit {
  private review: Review;
  private restaurantId: number;
  private reviewId: number = -999;
  private stars: number[];
  private dataSaved = false;
  private review$ = new BehaviorSubject<Review>(this.review);

  constructor(private route: ActivatedRoute,
    private userService: UserService,
    private router: Router,
    private restaurantService: RestaurantService,
    private reviewService: ReviewService) { }

  canActivate(): boolean {
    if (!this.userService.isLoggedIn()) {
      this.router.navigate(['/account/login']);
      return false;
    }
  }

  ngOnInit() {
    this.restaurantId = +this.route.snapshot.paramMap.get('restaurantId');
    if (this.route.snapshot.paramMap.keys.indexOf('reviewId') >= 0) {
      this.reviewId = +this.route.snapshot.paramMap.get('reviewId');
    }
    if (this.reviewId >= 0) {
      this.reviewService.getReview(this.reviewId).subscribe(rev => {
        this.review = rev;
        this.review$.next(this.review);
      });
    } else {
      this.review = new Review({
        Restaurant: new Restaurant({
          Id: this.restaurantId
        }),
      });
      this.restaurantService.getRestaurant(this.restaurantId).subscribe(restaurant => this.review.Restaurant = restaurant);
      if (!this.restaurantId) {
        this.router.navigate(['/reviews']);
      }
    }
    this.stars = [0, 1, 2, 3, 4, 5];
  }

  public onFormSubmit({ value, valid }: { value: Review, valid: boolean }) {
    this.dataSaved = false;
    this.review.Stars = value.Stars;
    this.review.Description = value.Description;
    this.reviewService.addEditReview(this.review).subscribe(success => {
      if (success)
        this.dataSaved = true;
      else
        this.dataSaved = false;
    });
  }
}
