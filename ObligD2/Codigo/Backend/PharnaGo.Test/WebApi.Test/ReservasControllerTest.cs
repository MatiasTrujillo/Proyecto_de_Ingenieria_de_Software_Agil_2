using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PharmaGo.Domain.Entities;
using PharmaGo.Domain.SearchCriterias;
using PharmaGo.Exceptions;
using PharmaGo.IBusinessLogic;
using PharmaGo.WebApi.Controllers;
using PharmaGo.WebApi.Models.In;
using PharmaGo.WebApi.Models.Out;



namespace PharmaGo.Test.WebApi.Test
{
    [TestClass]
    public class ReservasControllerTests
    {
        private ReservaController _reservaController;
        private Mock<IReservaManager> _reservaManagerMock;

        [TestInitialize]
        public void SetUp()
        {

            _reservaManagerMock = new Mock<IReservaManager>(MockBehavior.Strict);
            var httpContext = new DefaultHttpContext();
            _reservaController = new ReservaController(_reservaManagerMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext
                }
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            _reservaManagerMock.VerifyAll();
        }

        [TestMethod]
        public void CrearReserva_WhenTodosLosParametrosSonValidos_ShouldCrearReserva()
        {
            var reserva = new Reserva { NombreMedicamento = "Aspirina", CantidadMedicamento = 2 };
            var reservaModel = new ReservaModel { NombreMedicamento = "Aspirina", CantidadMedicamento = 2 };


            _reservaManagerMock.Setup(x => x.Create(It.IsAny<Reserva>())).Returns(reserva);
            var result = _reservaController.Create(reservaModel);
            var objectResult = result as ObjectResult;
            var statusCode = objectResult.StatusCode;

            _reservaManagerMock.VerifyAll();
            Assert.AreEqual(201, statusCode);
        }
    }
}