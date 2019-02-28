import { Restaurant } from './restaurant';
import { User } from './user';

export class Review {
  Id: number;
  Restaurant: Restaurant;
  User: User;
  Stars: number;
  Description: string;

  constructor(values: Object = {}) {
    (<any>Object).assign(this, values);
  }
}
