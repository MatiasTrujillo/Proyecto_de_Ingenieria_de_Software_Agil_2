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
    }
}