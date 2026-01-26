import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlateWatchlist } from './plate-watchlist';

describe('PlateWatchlist', () => {
  let component: PlateWatchlist;
  let fixture: ComponentFixture<PlateWatchlist>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlateWatchlist]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlateWatchlist);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
