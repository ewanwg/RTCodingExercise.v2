import { Plate } from './plate';

export interface PlatesWatchlist {
  id: string;
  plateId: string;
  priceAlert?: number;
  createdDate: string;
  plate?: Plate;
}
