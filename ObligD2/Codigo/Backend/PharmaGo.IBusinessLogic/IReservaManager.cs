using ExportationModel.ExportDomain;
using PharmaGo.Domain.Entities;
using PharmaGo.Domain.SearchCriterias;

namespace PharmaGo.IBusinessLogic
{
    public interface IReservaManager
    {
        Reserva Create(Reserva reserva);
        bool CheckStock(string nombre, int cantidad);
    }
}
