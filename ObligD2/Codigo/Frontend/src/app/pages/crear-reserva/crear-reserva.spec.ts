import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CrearReservaComponent } from './crear-reserva';

describe('CrearReserva', () => {
  let component: CrearReservaComponent;
  let fixture: ComponentFixture<CrearReservaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CrearReservaComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CrearReservaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create component instance', () => {
    expect(component).to.exist;
  });

  /*it('should create', () => {
    expect(component).toBeTruthy();
  });*/

  it('deberia crear reserva al clickear en boton', () => {
    // Arrange
    const mockReserva = [
      { id: 1, medicamento: 'Aspirina', cantidad: 10 }
    ];
    reservarService.reservar.and.returnValue(of(mockReserva));
    component.medicamento = 'Aspirina';

    // Act
    component.reservar();

    // Assert
    expect(reservarService.reservar).toHaveBeenCalledWith('Aspirina');
    expect(component.reservas).toEqual(mockReserva);
  });
});
