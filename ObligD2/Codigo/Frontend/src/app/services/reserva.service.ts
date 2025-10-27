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
  confirmada?: boolean;
  requierePrescripcion?: boolean;
  tienePrescripcion?: boolean;
  empleadoConfirmacion?: string;
  fechaConfirmacion?: string;
}

export interface ReservaRequest {
  nombreMedicamento: string;
  cantidadMedicamento: number;
  fechaExpiracion: string;
  usuario: string;
  requierePrescripcion?: boolean;
}

export interface ConfirmarReservaRequest {
  tienePrescripcion: boolean;
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

  checkStock(nombreMedicamento: string, cantidadMedicamento: number): Observable<boolean> {
    const url = `${this.url}/check-stock`;
    const params = { nombre: nombreMedicamento, cantidad: cantidadMedicamento.toString() };
    return this.http.get<boolean>(url, { headers: this.getHttpHeaders(), params })
      .pipe(
        map(response => response),
        catchError(this.handleError<boolean>('checkStock', false))
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

  confirmarReserva(id: number, request: ConfirmarReservaRequest): Observable<Reserva> {
    const url = `${this.url}/${id}/confirmar`;
    return this.http.patch<Reserva>(url, request, { headers: this.getHttpHeaders() })
      .pipe(
        tap((reserva: Reserva) => console.log(`confirmed reserva id=${id}`)),
        catchError(this.handleError<Reserva>('confirmarReserva'))
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
