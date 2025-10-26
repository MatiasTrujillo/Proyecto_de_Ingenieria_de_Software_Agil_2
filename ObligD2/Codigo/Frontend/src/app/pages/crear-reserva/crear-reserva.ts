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
  stock: boolean = false;
  private RESERVA_API = environment.apiUrl + '/api/Reserva';
  private MEDICAMENTO_API = environment.apiUrl + '/api/Drug';
  medicamentosObj: any[] = [];
  medicamentos: string[] = [];
  errorNombre = false;
  errorStock = false;

  constructor(
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
      NombreMedicamento: this.medicamento.trim(),
      CantidadMedicamento: this.cantidad
    };

    if (this.medicamento.trim().length === 0) {
      this.errorNombre = true;
      return;
    }

    this.http.get<boolean>(this.RESERVA_API,  { params: { nombre: this.medicamento, cantidad: this.cantidad } }).subscribe({
      next: (res) => {
        this.stock = res;

        if(!this.stock){
          this.errorStock = true;
          return;
        }

        this.errorNombre = false;
        this.errorStock = false;

        this.http.post<any>(this.RESERVA_API, payload).subscribe({
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