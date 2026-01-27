import { Component, OnInit } from '@angular/core';
import { PlatesWatchlist } from '../../models/plate-watchlist';
import { Catalog } from '../../services/catalog';
import { DecimalPipe, DatePipe } from '@angular/common';

@Component({
  selector: 'app-plate-watchlist',
  imports: [DecimalPipe, DatePipe],
  templateUrl: './plate-watchlist.html',
  styleUrl: './plate-watchlist.css',
})
export class PlateWatchlist implements OnInit {
  watchlist: PlatesWatchlist[] = [];
  error: string | null = null;
  loading = false;

  constructor(private catalogService: Catalog) { }
  
  ngOnInit(): void {
    this.loadWatchlist();
  }

  loadWatchlist(): void {
    this.loading = true;
    this.catalogService.getWatchlist().subscribe({
      next: (data) => {
        this.watchlist = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load watchlist.';
        console.error(err);
        this.loading = false;
      }
    });
  }

}
