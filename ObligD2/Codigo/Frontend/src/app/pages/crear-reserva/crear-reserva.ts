import { Component } from '@angular/core';

@Component({
  selector: 'app-crear-reserva',
  templateUrl: './crear-reserva.html',
  styleUrls: ['./crear-reserva.css']
})
export class CrearReservaComponent {

}

/*
// ...existing code...
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-crear-reserva',
  templateUrl: './crear-reserva.html'
})
export class CrearReservaComponent implements OnInit {
  form: FormGroup;
  medicamentos: string[] = [
    'Paracetamol',
    'Ibuprofeno',
    'Amoxicilina',
    'Aspirina'
  ];

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      medicamento: ['', Validators.required],
      cantidad: [null, [Validators.required, Validators.min(1)]]
    });
  }

  goReserve(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const reserva = {
      medicamento: this.form.value.medicamento,
      cantidad: this.form.value.cantidad
    };

    // TODO: llamar servicio para crear reserva. Por ahora:
    console.log('Reservando:', reserva);
    // si tienes un toast o router, puedes mostrar mensaje o navegar aquí
  }
}
// ...existing code...
*/
