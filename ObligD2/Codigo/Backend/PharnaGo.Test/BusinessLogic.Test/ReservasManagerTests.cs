using ExportationModel.ExportDomain;
using Moq;
using PharmaGo.BusinessLogic;
using PharmaGo.Domain.Entities;
using PharmaGo.Domain.SearchCriterias;
using PharmaGo.Exceptions;
using PharmaGo.IDataAccess;
using PharmaGo.WebApi.Models.In;
using PharmaGo.WebApi.Models.Out;
using System.Linq.Expressions;

namespace PharmaGo.Test.BusinessLogic.Test
{
    [TestClass]
    public class ReservasManagerTests
    {
        private Mock<IRepository<Reserva>> _reservaRepository;
        private ReservaManager _reservaManager;


        [TestInitialize]
        public void InitTest()
        {
            _reservaRepository = new Mock<IRepository<Reserva>>();
            _reservaManager = new ReservaManager(_reservaRepository.Object);

        }

        [TestCleanup]
        public void Cleanup()
        {
            _reservaRepository.VerifyAll();
        }

        [TestMethod]
        public void CrearReserva_WhenReservaValida_ShouldLlamarAlRepositorio()
        {
            var reserva = new Reserva { NombreMedicamento = "Aspirina", CantidadMedicamento = 2 };

            _reservaManager.CrearReserva(reserva);

            _reservaRepository.Verify(repo => repo.InsertOne(
                It.Is<Reserva>(a =>
                    a.NombreMedicamento == reserva.NombreMedicamento &&
                    a.CantidadMedicamento == reserva.CantidadMedicamento)), Times.Once);
        }
    }
}