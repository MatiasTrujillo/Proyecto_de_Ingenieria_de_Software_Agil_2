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
            
            if (!reserva.PuedeCancelarse())
            {
                throw new InvalidResourceException("No se puede cancelar una reserva confirmada.");
            }
            
            _reservaRepository.DeleteOne(reserva);
            _reservaRepository.Save();
        }

        public Reserva ConfirmarReserva(int id, string empleado, bool tienePrescripcion)
        {
            var reserva = GetById(id);
            
            reserva.Confirmar(empleado, tienePrescripcion);
            
            _reservaRepository.UpdateOne(reserva);
            _reservaRepository.Save();
            
            return reserva;
        }
    }
}
