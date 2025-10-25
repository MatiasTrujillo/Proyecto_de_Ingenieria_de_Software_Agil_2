using PharmaGo.Domain.Entities;

namespace PharmaGo.WebApi.Models.Out
{
    public class ReservaModelResponse
    {
        public int Id { get; set; }
        public string NombreMedicamento { get; set; }
        public int CantidadMedicamento { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public DateTime FechaReserva { get; set; }
        public string Usuario { get; set; }

        public ReservaModelResponse(Reserva reserva)
        {
            Id = reserva.Id;
            NombreMedicamento = reserva.NombreMedicamento;
            CantidadMedicamento = reserva.CantidadMedicamento;
            FechaExpiracion = reserva.FechaExpiracion;
            FechaReserva = reserva.FechaReserva;
            Usuario = reserva.Usuario;
        }
    }
}
