import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ReviewsRoutingModule } from './reviews-routing.module';
import { ReviewListComponent } from './review-list/review-list.component';
import { ReviewAddEditComponent } from './review-add-edit/review-add-edit.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReviewsRoutingModule
  ],
  declarations: [ReviewListComponent, ReviewAddEditComponent]
})
export class ReviewsModule { }
