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
            httpContext.Request.Headers["Authorization"] = token;
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
            var reserva = new Reserva
            {
                NombreMedicamento = "Aspirina",
                CantidadMedicamento = 2,
            };

            _reservaManagerMock.Setup(service => service.CrearReserva(It.IsAny<Reserva>()))
                .Verifiable();

            var result = _reservaController.CrearReserva(reserva) as CreatedResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);
        }
    }
}