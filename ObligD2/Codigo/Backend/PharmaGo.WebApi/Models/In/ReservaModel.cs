using PharmaGo.Domain.Entities;

namespace PharmaGo.WebApi.Models.In
{
    public class ReservaModel
    {
        public string NombreMedicamento { get; set; }
        public int CantidadMedicamento { get; set; }
        public string FechaExpiracion { get; set; }
        public string Usuario { get; set; }
        public bool RequierePrescripcion { get; set; }

        public Reserva ToEntity()
        {
            if (string.IsNullOrWhiteSpace(this.FechaExpiracion) ||
                !DateTimeOffset.TryParse(this.FechaExpiracion, out var dto))
            {
                throw new ArgumentException("FechaExpiracion inválida o mal formateada");
            }
            var fechaExpUtc = dto.UtcDateTime; // DateTime en UTC

            return new Reserva()
            {
                NombreMedicamento = this.NombreMedicamento,
                CantidadMedicamento = this.CantidadMedicamento,
                FechaExpiracion = fechaExpUtc,
                FechaReserva = DateTime.UtcNow,
                Usuario = this.Usuario ?? "Anónimo",
                RequierePrescripcion = this.RequierePrescripcion,
                EmpleadoConfirmacion = "Empleado0",
                FechaConfirmacion = DateTime.UtcNow
            };
        }
    }
}

