import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { RestaurantsRoutingModule } from './restaurants-routing.module';
import { RestaurantDetailComponent } from './restaurant-detail/restaurant-detail.component';
import { RestaurantListComponent } from './restaurant-list/restaurant-list.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    RestaurantsRoutingModule
  ],
  declarations: [RestaurantDetailComponent, RestaurantListComponent]
})
export class RestaurantsModule { }
