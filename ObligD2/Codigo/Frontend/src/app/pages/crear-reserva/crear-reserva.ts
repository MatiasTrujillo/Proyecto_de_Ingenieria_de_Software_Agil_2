import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { CommonService } from '../../services/CommonService';


@Component({
  selector: 'app-crear-reserva',
  templateUrl: './crear-reserva.html',
  styleUrls: ['./crear-reserva.css']
})
export class CrearReservaComponent implements OnInit {
  medicamento: string = '';
  cantidad: number = 1;
  submitted: boolean = false;
  private RESERVA_API = environment.apiUrl + '/api/Reserva';

  constructor(
    private http: HttpClient,
    private commonService: CommonService,
    private router: Router
  ) {}

  ngOnInit(): void {}

  validate(): { valid: boolean; message?: string } {
    if (!this.medicamento || this.medicamento.trim().length === 0) {
      return { valid: false, message: 'Debe ingresar el nombre del medicamento.' };
    }
    if (!Number.isFinite(this.cantidad) || this.cantidad < 1) {
      return { valid: false, message: 'La cantidad debe ser mayor o igual a 1.' };
    }
    return { valid: true };
  }

  goReserve(): void {
    this.submitted = true;
    const v = this.validate();
    if (!v.valid) {
      this.commonService.updateToastData('Error', 'danger', v.message || 'Datos inválidos.');
      return;
    }

    const payload = {
      NombreMedicamento: this.medicamento.trim(),
      CantidadMedicamento: this.cantidad
    };

    this.http.post<any>(this.RESERVA_API, payload).subscribe({
      next: () => {
        this.commonService.updateToastData(
          'Reserva creada',
          'success',
          'La reserva se ha creado correctamente.'
        );
        this.medicamento = '';
        this.cantidad = 1;
        this.submitted = false;
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
}
