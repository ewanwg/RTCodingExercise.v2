import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import { ReplaySubject } from 'rxjs';

export interface PlateEvent {
  type: 'PlateSold' | 'PlateReserved' | 'PlateUnreserved' | 'NotificationReceived' | string;
  payload: any;
}

@Injectable({
  providedIn: 'root',
})
export class NotificationsService {
  private hubConnection: HubConnection | null = null;
  private eventsSubject = new ReplaySubject<PlateEvent>(10);
  public events$ = this.eventsSubject.asObservable();

  start(): Promise<void> {
    const url = `${environment.catalogApiUrl.replace(/\/$/, '')}/hubs/plates`;

    if (this.hubConnection && this.hubConnection.state === HubConnectionState.Connected) {
      return Promise.resolve();
    }

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(url)
      .withAutomaticReconnect()
      .build();

    // Map server method names to local events and log for debugging
    this.hubConnection.on('NotificationReceived', (payload: any) => {
      console.debug('SignalR received NotificationReceived', payload);
      this.eventsSubject.next({ type: 'NotificationReceived', payload });
    });

    // Keep these in case other parts of server use them
    this.hubConnection.on('PlateSold', (payload: any) => {
      console.debug('SignalR received PlateSold', payload);
      this.eventsSubject.next({ type: 'PlateSold', payload });
    });
    this.hubConnection.on('PlateReserved', (payload: any) => {
      console.debug('SignalR received PlateReserved', payload);
      this.eventsSubject.next({ type: 'PlateReserved', payload });
    });
    this.hubConnection.on('PlateUnreserved', (payload: any) => {
      console.debug('SignalR received PlateUnreserved', payload);
      this.eventsSubject.next({ type: 'PlateUnreserved', payload });
    });

    this.hubConnection.onreconnected((connectionId) => {
      console.info('SignalR reconnected:', connectionId);
    });

    this.hubConnection.onclose((error) => {
      console.warn('SignalR connection closed', error);
    });

    return this.hubConnection
      .start()
      .then(() => {
        console.info('SignalR connected to', url);
      })
      .catch((err) => {
        console.error('SignalR start failed:', err);
        throw err;
      });
  }

  stop(): Promise<void> {
    if (!this.hubConnection) return Promise.resolve();
    return this.hubConnection.stop().then(() => {
      this.hubConnection = null;
    });
  }

  // Join a per-plate group (server hub has JoinPlateGroup)
  joinPlateGroup(plateId: string): Promise<void> {
    if (!this.hubConnection) return Promise.reject('SignalR not started');
    try {
      return this.hubConnection.invoke('JoinPlateGroup', plateId);
    } catch (err) {
      return Promise.reject(err);
    }
  }

  leavePlateGroup(plateId: string): Promise<void> {
    if (!this.hubConnection) return Promise.reject('SignalR not started');
    try {
      return this.hubConnection.invoke('LeavePlateGroup', plateId);
    } catch (err) {
      return Promise.reject(err);
    }
  }
}