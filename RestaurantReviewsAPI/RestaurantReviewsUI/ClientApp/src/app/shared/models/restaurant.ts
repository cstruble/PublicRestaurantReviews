import { RestaurantLocation } from './restaurant-location';

export class Restaurant {
  Id: number;
  Name: string;
  Location: RestaurantLocation;

  constructor(values: Object = {}) {
    (<any>Object).assign(this, values);
  }

}
