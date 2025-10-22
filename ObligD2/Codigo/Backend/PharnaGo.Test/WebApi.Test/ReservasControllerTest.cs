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
        private ReservasController _reservasController;
        private Mock<IReservasManager> _reservasManagerMock;

        [TestInitialize]
        public void SetUp()
        {

            _reservasManagerMock = new Mock<IReservasManager>(MockBehavior.Strict);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = token;
            _reservasController = new ReservasController(_reservasManagerMock.Object)
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
            _reservasManagerMock.VerifyAll();
        }

        [TestMethod]
        public void CrearReserva_WhenTodosLosParametrosSonValidos_ShouldCrearReserva()
        {
            var reserva = new Reserva
            {
                NombreMedicamento = "Aspirina",
                CantidadMedicamento = 2,
            };

            _reservasManagerMock.Setup(service => service.CrearReserva(It.IsAny<Reserva>()))
                .Verifiable();

            var result = _reservasController.CrearReserva(reserva) as CreatedResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);
        }
    }
}