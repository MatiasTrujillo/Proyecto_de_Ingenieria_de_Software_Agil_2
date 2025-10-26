import { Component, OnInit } from '@angular/core';
import { ReservaService, Reserva } from '../../services/reserva.service';
import { CommonService } from '../../services/CommonService';
import { Router } from '@angular/router';

@Component({
  selector: 'app-gestionar-reservas',
  templateUrl: './gestionar-reservas.html',
  styleUrls: ['./gestionar-reservas.css']
})
export class GestionarReservasComponent implements OnInit {
  reservas: Reserva[] = [];
  loading: boolean = true;
  error: string = '';

  constructor(
    private reservaService: ReservaService,
    private commonService: CommonService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadReservas();
  }

  loadReservas(): void {
    this.loading = true;
    this.error = '';
    
    this.reservaService.getAllReservas().subscribe({
      next: (data) => {
        this.reservas = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading reservas:', err);
        this.error = 'Error al cargar las reservas';
        this.loading = false;
        this.commonService.updateToastData('Error', 'danger', 'No se pudieron cargar las reservas');
      }
    });
  }

  cancelarReserva(id: number, nombreMedicamento: string): void {
    if (!confirm(`¿Está seguro que desea cancelar la reserva de ${nombreMedicamento}?`)) {
      return;
    }

    this.reservaService.deleteReserva(id).subscribe({
      next: () => {
        this.commonService.updateToastData('Éxito', 'success', 'Reserva cancelada exitosamente');
        this.loadReservas(); // Recargar la lista
      },
      error: (err) => {
        console.error('Error canceling reserva:', err);
        this.commonService.updateToastData('Error', 'danger', 'No se pudo cancelar la reserva');
      }
    });
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('es-ES', { 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  isExpired(fechaExpiracion: string): boolean {
    return new Date(fechaExpiracion) < new Date();
  }

  getReservasActivas(): number {
    return this.reservas.filter(r => !this.isExpired(r.fechaExpiracion)).length;
  }

  goToHome(): void {
    this.router.navigate(['/home']);
  }

  goToCrearReserva(): void {
    this.router.navigate(['/crear-reserva']);
  }
}
