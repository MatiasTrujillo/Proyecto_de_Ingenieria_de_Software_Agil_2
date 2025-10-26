using ExportationModel.ExportDomain;
using PharmaGo.Domain.Entities;
using PharmaGo.Domain.SearchCriterias;

namespace PharmaGo.IBusinessLogic
{
    public interface IReservaManager
    {
        Reserva Create(Reserva reserva);
        bool CheckStock(string nombre, int cantidad);
        IEnumerable<Reserva> GetAll();
        Reserva GetById(int id);
        void Delete(int id);
        Reserva ConfirmarReserva(int id, string empleado, bool tienePrescripcion);
    }
}
