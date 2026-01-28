import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PlateWatchlist } from './plate-watchlist';
import { Catalog } from '../../services/catalog';
import { PlatesWatchlist } from '../../models/plate-watchlist';
import { PlateStatus } from '../../models/plate';
import { of, throwError } from 'rxjs';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('PlateWatchlist', () => {
  let component: PlateWatchlist;
  let fixture: ComponentFixture<PlateWatchlist>;
  let mockCatalogService: any;

  const mockWatchlist: PlatesWatchlist[] = [
  {
    id: 'w1',
    plateId: 'p1',
    priceAlert: 4500,
    createdDate: new Date().toISOString(),
    plate: {
      id: 'p1',
      registration: 'AB12 CDE',
      purchasePrice: 4000,
      salePrice: 4200,
      status: PlateStatus.ForSale,
    },
  },
  {
    id: 'w2',
    plateId: 'p2',
    createdDate: new Date().toISOString(),
    plate: {
      id: 'p2',
      registration: 'XY99 ZZZ',
      purchasePrice: 4500,
      salePrice: 5000,
      status: PlateStatus.Sold,
    },
  },
];

  beforeEach(async () => {
    mockCatalogService = {
      getWatchlist: vi.fn().mockReturnValue(of(mockWatchlist)),
      removeFromWatchlist: vi.fn().mockReturnValue(of(null)),
    };

    await TestBed.configureTestingModule({
      imports: [PlateWatchlist],
      providers: [{ provide: Catalog, useValue: mockCatalogService }],
    }).compileComponents();

    fixture = TestBed.createComponent(PlateWatchlist);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should load watchlist on init', async () => {
      fixture.detectChanges();
      await new Promise((r) => setTimeout(r, 0));

      expect(mockCatalogService.getWatchlist).toHaveBeenCalled();
      expect(component.watchlist.length).toBe(2);
      expect(component.loading).toBe(false);
    });
  });

  describe('loadWatchlist', () => {
    it('should populate watchlist items', async () => {
      component.loadWatchlist();
      await new Promise((r) => setTimeout(r, 0));

      expect(component.watchlist).toEqual(mockWatchlist);
      expect(component.loading).toBe(false);
    });

    it('should handle load errors gracefully', async () => {
      mockCatalogService.getWatchlist.mockReturnValue(
        throwError(() => new Error('API failure')),
      );

      component.loadWatchlist();
      await new Promise((r) => setTimeout(r, 0));

      expect(component.error).toBe('Failed to load watchlist.');
      expect(component.loading).toBe(false);
    });
  });

  describe('removeFromWatchlist', () => {
    it('should optimistically remove item from list', async () => {
      component.watchlist = [...mockWatchlist];

      component.removeFromWatchlist('p1');
      await new Promise((r) => setTimeout(r, 0));

      expect(component.watchlist.length).toBe(1);
      expect(component.watchlist.find(w => w.plateId === 'p1')).toBeUndefined();
      expect(mockCatalogService.removeFromWatchlist).toHaveBeenCalledWith('p1');
    });

    it('should rollback if removal fails', async () => {
      mockCatalogService.removeFromWatchlist.mockReturnValue(
        throwError(() => new Error('Delete failed')),
      );

      component.watchlist = [...mockWatchlist];
      component.removeFromWatchlist('p1');
      await new Promise((r) => setTimeout(r, 0));

      expect(component.watchlist.length).toBe(2);
      expect(component.error).toBe('Failed to remove plate from watchlist.');
    });
  });

  describe('getStatusText', () => {
    it('should return correct status text', () => {
      expect(component.getStatusText(PlateStatus.ForSale)).toBe('For Sale');
      expect(component.getStatusText(PlateStatus.Reserved)).toBe('Reserved');
      expect(component.getStatusText(PlateStatus.Sold)).toBe('Sold');
    });

    it('should return Unknown for undefined status', () => {
      expect(component.getStatusText(undefined)).toBe('Unknown');
    });
  });
});
