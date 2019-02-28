import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ReviewListComponent } from './review-list/review-list.component';
import { ReviewAddEditComponent } from './review-add-edit/review-add-edit.component';
import { AuthGuard } from '../auth.guard';

const routes: Routes = [
  { path: 'reviews', component: ReviewListComponent },
  { path: 'review-add-edit/:restaurantId', component: ReviewAddEditComponent, canActivate: [AuthGuard] },
  { path: 'review-add-edit/:reviewId/:restaurantId', component: ReviewAddEditComponent, canActivate: [AuthGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [AuthGuard]
})
export class ReviewsRoutingModule { }
