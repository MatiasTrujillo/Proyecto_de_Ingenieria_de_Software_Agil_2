using ExportationModel.ExportDomain;
using PharmaGo.Domain.Entities;
using PharmaGo.Domain.SearchCriterias;
using PharmaGo.Exceptions;
using PharmaGo.IBusinessLogic;
using PharmaGo.IDataAccess;

namespace PharmaGo.BusinessLogic
{
    public class ReservaManager : IReservaManager
    {
        private readonly IRepository<Reserva> _reservaRepository;
        private readonly IRepository<Drug> _drugRepository;

        public ReservaManager(IRepository<Reserva> reservaRepository, IRepository<Drug> drugRepository)
        {
            _reservaRepository = reservaRepository;
            _drugRepository = drugRepository;
        }

        public Reserva Create(Reserva reserva)
        {
            if (reserva == null)
            {
                throw new ResourceNotFoundException("Please create a reservation before inserting it.");
            }
            reserva.ValidOrFail();

            _reservaRepository.InsertOne(reserva);
            _reservaRepository.Save();
            return reserva;
        }

        public bool CheckStock(string nombre, int cantidad)
        {
            var drug = _drugRepository.GetOneByExpression(d => d.Name == nombre);
            if (drug == null)
            {
                return false;
            }
            return drug.Stock >= cantidad;
        }
    }
}
