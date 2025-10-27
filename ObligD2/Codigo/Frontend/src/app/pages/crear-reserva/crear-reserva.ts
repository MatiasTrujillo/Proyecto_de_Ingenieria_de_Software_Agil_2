import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { CommonService } from '../../services/CommonService';
import { ReservaService, Reserva } from '../../services/reserva.service';



@Component({
  selector: 'app-crear-reserva',
  templateUrl: './crear-reserva.html',
  styleUrls: ['./crear-reserva.css']
})
export class CrearReservaComponent implements OnInit {
  medicamento: string = '';
  cantidad: number = 1;
  stock: boolean = false;
  private MEDICAMENTO_API = environment.apiUrl + '/api/Drug';
  medicamentosObj: any[] = [];
  medicamentos: string[] = [];
  errorNombre = false;
  errorStock = false;
  errorCantidad = false;

  constructor(
    private reservaService: ReservaService,
    private http: HttpClient,
    private commonService: CommonService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadMedicamentos();
  }

  loadMedicamentos(): void {
  this.http.get<any[]>(this.MEDICAMENTO_API).subscribe({
    next: (res) => {
      this.medicamentosObj = res || [];
      this.medicamentos = (res || []).map(item => item.name || 'Sin nombre');
    }
  });
  }



  goReserve(): void {
    const payload = {
      nombreMedicamento: this.medicamento.trim(),
      cantidadMedicamento: this.cantidad,
      fechaExpiracion: (() => { const d = new Date(); d.setDate(d.getDate() + 1); return d.toISOString(); })(),
      usuario: 'usuario1',
      requierePrescripcion: false
    };

    if (this.medicamento.trim().length === 0) {
      this.errorNombre = true;
      return;
    }
    if (!Number.isFinite(this.cantidad)) {
      this.errorCantidad = true;
      return;
    }

    this.reservaService.checkStock(this.medicamento, this.cantidad).subscribe({
      next: (res) => {
        this.stock = res;

        if(!this.stock){
          this.errorStock = true;
          return;
        }

        this.errorNombre = false;
        this.errorStock = false;
        this.errorCantidad = false;

        this.reservaService.createReserva(payload).subscribe({
          next: () => {
            this.commonService.updateToastData(
              'Reserva creada',
              'success',
              'La reserva se ha creado correctamente.'
            );
            this.medicamento = '';
            this.cantidad = 1;
            this.router.navigate(['/']);
          },
          error: (err) => {
            console.error('Error creando reserva', err);
            this.commonService.updateToastData(
              'Error',
              'danger',
              'No fue posible crear la reserva.'
            );
          }
        });
      }
    });
  }
}