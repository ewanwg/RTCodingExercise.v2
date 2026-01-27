import { Component, OnInit } from '@angular/core';
import { PlatesWatchlist } from '../../models/plate-watchlist';
import { Catalog } from '../../services/catalog';
import { DecimalPipe, DatePipe } from '@angular/common';
import { PlateStatus } from '../../models/plate';

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

  PlateStatus = PlateStatus;

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

  removeFromWatchlist(plateId: string): void {
    this.catalogService.removeFromWatchlist(plateId).subscribe({
      next: () => {
        this.watchlist = this.watchlist.filter(w => w.plateId !== plateId);
      },
      error: (err) => {
        this.error = 'Failed to remove plate from watchlist.';
        console.error(err);
      }
    });
  }

  getStatusText(status?: PlateStatus): string {
    switch (status) {
      case PlateStatus.ForSale:
        return 'For Sale';
      case PlateStatus.Reserved:
        return 'Reserved';
      case PlateStatus.Sold:
        return 'Sold';
      default:
        return 'Unknown';
    }
  }
}
