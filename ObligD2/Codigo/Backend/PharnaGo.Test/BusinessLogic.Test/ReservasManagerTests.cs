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
        private Mock<IRepository<Drug>> _drugRepository;


        [TestInitialize]
        public void InitTest()
        {
            _reservaRepository = new Mock<IRepository<Reserva>>();
            _drugRepository = new Mock<IRepository<Drug>>();
            _reservaManager = new ReservaManager(_reservaRepository.Object, _drugRepository.Object);

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

            _reservaManager.Create(reserva);

            _reservaRepository.Verify(repo => repo.InsertOne(
                It.Is<Reserva>(a =>
                    a.NombreMedicamento == reserva.NombreMedicamento &&
                    a.CantidadMedicamento == reserva.CantidadMedicamento)), Times.Once);
        }

        [TestMethod]
        public void CheckStock_WhenDrugExistsAndHasStock_ShouldReturnTrue()
        {
            string nombre = "Aspirina";
            int cantidad = 5;
            var drug = new Drug { Name = nombre, Stock = 10 };

            _drugRepository.Setup(repo => repo.GetOneByExpression(
                It.Is<Expression<Func<Drug, bool>>>(expr => expr.Compile()(drug))
            )).Returns(drug);

            bool result = _reservaManager.CheckStock(nombre, cantidad);

            Assert.IsTrue(result);
        }
    }
}