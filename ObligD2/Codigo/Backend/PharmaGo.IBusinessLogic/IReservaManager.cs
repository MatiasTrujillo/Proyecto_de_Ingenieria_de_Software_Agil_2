using ExportationModel.ExportDomain;
using PharmaGo.Domain.Entities;
using PharmaGo.Domain.SearchCriterias;

namespace PharmaGo.IBusinessLogic
{
    public interface IReservaManager
    {
        Reserva Create(Reserva reserva);
        IEnumerable<Reserva> GetAll();
        Reserva GetById(int id);
        void Delete(int id);
    }
}
