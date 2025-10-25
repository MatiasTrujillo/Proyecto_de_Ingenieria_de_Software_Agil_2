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

        public ReservaManager(IRepository<Reserva> reservaRepository)
        {
            _reservaRepository = reservaRepository;
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
    }
}
