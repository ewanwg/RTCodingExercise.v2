import { Routes } from '@angular/router';
import { PlateListComponent } from './components/plate-list/plate-list';
import { PlateWatchlist } from './components/plate-watchlist/plate-watchlist';

export const routes: Routes = [
  { path: '', component: PlateListComponent },
  { path: 'watchlist', component: PlateWatchlist },
];
