import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { CommonService } from './CommonService';
import { StorageManager } from '../utils/storage-manager';

export interface Reserva {
  id: number;
  nombreMedicamento: string;
  cantidadMedicamento: number;
  fechaExpiracion: string;
  fechaReserva: string;
  usuario: string;
}

export interface ReservaRequest {
  nombreMedicamento: string;
  cantidadMedicamento: number;
  fechaExpiracion: string;
  usuario: string;
}

@Injectable({ providedIn: 'root' })
export class ReservaService {

  private url = environment.apiUrl + '/api/Reserva';

  constructor(
    private http: HttpClient,
    private commonService: CommonService,
    private storageManager: StorageManager
  ) { }

  getHttpHeaders(): HttpHeaders {
    let login = JSON.parse(this.storageManager.getLogin());
    let token = login ? login.token : "";
    
    return new HttpHeaders()
      .set('Content-Type', 'application/json')
      .set('Authorization', token);
  }

  getAllReservas(): Observable<Reserva[]> {
    return this.http.get<Reserva[]>(this.url, { headers: this.getHttpHeaders() })
      .pipe(
        tap(_ => console.log('fetched reservas')),
        catchError(this.handleError<Reserva[]>('getAllReservas', []))
      );
  }

  getReservaById(id: number): Observable<Reserva> {
    const url = `${this.url}/${id}`;
    return this.http.get<Reserva>(url, { headers: this.getHttpHeaders() })
      .pipe(
        tap(_ => console.log(`fetched reserva id=${id}`)),
        catchError(this.handleError<Reserva>(`getReservaById id=${id}`))
      );
  }

  createReserva(reserva: ReservaRequest): Observable<Reserva> {
    return this.http.post<Reserva>(this.url, reserva, { headers: this.getHttpHeaders() })
      .pipe(
        tap((newReserva: Reserva) => console.log(`added reserva w/ id=${newReserva.id}`)),
        catchError(this.handleError<Reserva>('createReserva'))
      );
  }

  deleteReserva(id: number): Observable<any> {
    const url = `${this.url}/${id}`;
    return this.http.delete<any>(url, { headers: this.getHttpHeaders() })
      .pipe(
        tap(_ => console.log(`deleted reserva id=${id}`)),
        catchError(this.handleError<any>('deleteReserva'))
      );
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(error);
      console.log(`${operation} failed: ${error.message}`);
      return of(result as T);
    };
  }
}
