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

        public IEnumerable<Reserva> GetAll()
        {
            return _reservaRepository.GetAllByExpression(r => true);
        }

        public Reserva GetById(int id)
        {
            var reserva = _reservaRepository.GetOneByExpression(r => r.Id == id);
            if (reserva == null)
            {
                throw new ResourceNotFoundException($"Reserva with id {id} not found.");
            }
            return reserva;
        }

        public void Delete(int id)
        {
            var reserva = GetById(id);
            _reservaRepository.DeleteOne(reserva);
            _reservaRepository.Save();
        }
    }
}
