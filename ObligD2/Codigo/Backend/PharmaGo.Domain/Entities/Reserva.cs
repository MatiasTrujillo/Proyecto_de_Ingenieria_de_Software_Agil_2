using PharmaGo.Exceptions;

namespace PharmaGo.Domain.Entities
{
    public class Reserva
    {
        public int Id { get; set; }
        public string NombreMedicamento { get; set; }
        public int CantidadMedicamento { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public DateTime FechaReserva { get; set; }
        public string Usuario { get; set; }
        public bool Confirmada { get; set; } = false;
        public bool RequierePrescripcion { get; set; } = false;
        public bool TienePrescripcion { get; set; } = false;
        public string EmpleadoConfirmacion { get; set; }
        public DateTime? FechaConfirmacion { get; set; }


        public void ValidOrFail()
        {
            if (string.IsNullOrEmpty(NombreMedicamento) || CantidadMedicamento <= 0)
            {
                throw new InvalidResourceException("La reserva no está correctamente creada.");
            }
            
            if (FechaExpiracion <= DateTime.Now)
            {
                throw new InvalidResourceException("La fecha de expiración debe ser futura.");
            }
        }

        public bool PuedeCancelarse()
        {
            return !Confirmada;
        }

        public void Confirmar(string empleado, bool tienePrescripcion)
        {
            if (Confirmada)
            {
                throw new InvalidResourceException("La reserva ya está confirmada.");
            }

            if (RequierePrescripcion && !tienePrescripcion)
            {
                throw new InvalidResourceException("No se puede confirmar la reserva sin prescripción cuando el medicamento la requiere.");
            }

            Confirmada = true;
            TienePrescripcion = tienePrescripcion;
            EmpleadoConfirmacion = empleado;
            FechaConfirmacion = DateTime.Now;
        }

        public bool PuedeConfirmarse()
        {
            return !Confirmada;
        }
    }
}