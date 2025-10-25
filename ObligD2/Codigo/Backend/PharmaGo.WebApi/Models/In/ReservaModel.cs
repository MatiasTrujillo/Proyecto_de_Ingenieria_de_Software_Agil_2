using PharmaGo.Domain.Entities;

namespace PharmaGo.WebApi.Models.In
{
    public class ReservaModel
    {
        public string NombreMedicamento { get; set; }
        public int CantidadMedicamento { get; set; }

        public Reserva ToEntity()
        {
            return new Reserva()
            {
                NombreMedicamento = this.NombreMedicamento,
                CantidadMedicamento = this.CantidadMedicamento
            };
        }
    }
}

