import { Component, OnInit } from '@angular/core';
import { ReservaService, Reserva } from '../../../services/reserva.service';
import { CommonService } from '../../../services/CommonService';

@Component({
  selector: 'app-confirmar-reservas',
  templateUrl: './confirmar-reservas.component.html',
  styleUrls: ['./confirmar-reservas.component.css']
})
export class ConfirmarReservasComponent implements OnInit {

  reservas: Reserva[] = [];
  loading = false;

  constructor(
    private reservaService: ReservaService,
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
    this.loadReservas();
  }

  loadReservas(): void {
    this.loading = true;
    this.reservaService.getAllReservas().subscribe(
      (data: Reserva[]) => {
        // Mostrar solo las reservas no confirmadas
        this.reservas = data.filter(r => !r.confirmada);
        this.loading = false;
      },
      error => {
        console.error('Error al cargar reservas', error);
        this.commonService.updateToastData('Error al cargar las reservas', 'danger', 'Error');
        this.loading = false;
      }
    );
  }

  confirmarReserva(reserva: Reserva, tienePrescripcion: boolean): void {
    if (reserva.requierePrescripcion && !tienePrescripcion) {
      this.commonService.updateToastData('No se puede confirmar. El medicamento requiere prescripción', 'danger', 'Error');
      return;
    }

    this.reservaService.confirmarReserva(reserva.id, { tienePrescripcion }).subscribe(
      (updated: Reserva) => {
        this.commonService.updateToastData('Reserva confirmada exitosamente', 'success', 'Éxito');
        this.loadReservas(); // Recargar la lista
      },
      error => {
        console.error('Error al confirmar reserva', error);
        this.commonService.updateToastData('Error al confirmar la reserva', 'danger', 'Error');
      }
    );
  }

  confirmarConPrescripcion(reserva: Reserva): void {
    this.confirmarReserva(reserva, true);
  }

  confirmarSinPrescripcion(reserva: Reserva): void {
    this.confirmarReserva(reserva, false);
  }
}
