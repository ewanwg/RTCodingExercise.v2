import { Component, OnInit } from '@angular/core';
import { PlatesWatchlist } from '../../models/plate-watchlist';
import { Catalog } from '../../services/catalog';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { PlateStatus } from '../../models/plate';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-plate-watchlist',
  imports: [CommonModule, DecimalPipe, DatePipe],
  templateUrl: './plate-watchlist.html',
  styleUrl: './plate-watchlist.css',
})
export class PlateWatchlist implements OnInit {
  watchlist: PlatesWatchlist[] = [];
  error: string | null = null;
  loading = false;

  PlateStatus = PlateStatus;

  constructor(private catalogService: Catalog) {}

  ngOnInit(): void {
    this.loadWatchlist();
  }

  loadWatchlist(): void {
    this.loading = true;

    this.catalogService
      .getWatchlist()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (items) => {
          this.watchlist = items;
        },
        error: (err) => {
          this.error = 'Failed to load watchlist.';
          console.error('Error loading watchlist:', err);
          setTimeout(() => (this.error = null), 5000);
        },
      });
  }

  removeFromWatchlist(plateId: string): void {
    const previous = [...this.watchlist];
    this.watchlist = this.watchlist.filter((w) => w.plateId !== plateId);

    this.catalogService.removeFromWatchlist(plateId).subscribe({
      error: (err) => {
        // Rollback on failure
        this.watchlist = previous;

        this.error = 'Failed to remove plate from watchlist.';
        console.error(err);
        setTimeout(() => (this.error = null), 5000);
      },
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
