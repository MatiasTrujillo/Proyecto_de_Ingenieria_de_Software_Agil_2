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
            var reserva = new Reserva 
            { 
                Id = 1,
                NombreMedicamento = "Aspirina", 
                CantidadMedicamento = 2,
                FechaExpiracion = DateTime.Now.AddDays(7),
                FechaReserva = DateTime.Now,
                Usuario = "TestUser"
            };
            var reservaModel = new ReservaModel 
            { 
                NombreMedicamento = "Aspirina", 
                CantidadMedicamento = 2,
                FechaExpiracion = DateTime.Now.AddDays(7).ToString(),
                Usuario = "TestUser"
            };

            _reservaManagerMock.Setup(x => x.Create(It.IsAny<Reserva>())).Returns(reserva);
            var result = _reservaController.Create(reservaModel);
            var objectResult = result as ObjectResult;
            var statusCode = objectResult.StatusCode;

            _reservaManagerMock.VerifyAll();
            Assert.AreEqual(201, statusCode);
        }

        [TestMethod]
        public void Available_WhenHayStock_ShouldReturnTrue()
        {
            string nombre = "Aspirina";
            int cantidad = 2;

            _reservaManagerMock.Setup(x => x.CheckStock(nombre, cantidad)).Returns(true);

            var result = _reservaController.CheckStock(nombre, cantidad);
            var okResult = result.Result as OkObjectResult;
            var hayStock = (bool)okResult.Value;

            _reservaManagerMock.VerifyAll();
            Assert.IsTrue(hayStock);
        }
      
        public void GetAll_WhenReservasExisten_ShouldRetornarListaDeReservas()
        {
            var reservas = new List<Reserva>
            {
                new Reserva 
                { 
                    Id = 1,
                    NombreMedicamento = "Aspirina", 
                    CantidadMedicamento = 2,
                    FechaExpiracion = DateTime.Now.AddDays(7),
                    FechaReserva = DateTime.Now,
                    Usuario = "Usuario1"
                },
                new Reserva 
                { 
                    Id = 2,
                    NombreMedicamento = "Ibuprofeno", 
                    CantidadMedicamento = 3,
                    FechaExpiracion = DateTime.Now.AddDays(10),
                    FechaReserva = DateTime.Now,
                    Usuario = "Usuario2"
                }
            };

            _reservaManagerMock.Setup(x => x.GetAll()).Returns(reservas);
            
            var result = _reservaController.GetAll();
            var okResult = result as OkObjectResult;
            var reservasResponse = okResult.Value as IEnumerable<ReservaModelResponse>;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsNotNull(reservasResponse);
            Assert.AreEqual(2, reservasResponse.Count());
            Assert.AreEqual("Aspirina", reservasResponse.First().NombreMedicamento);
            Assert.AreEqual("Ibuprofeno", reservasResponse.Last().NombreMedicamento);
            _reservaManagerMock.VerifyAll();
        }

        [TestMethod]
        public void GetAll_WhenNoHayReservas_ShouldRetornarListaVacia()
        {
            _reservaManagerMock.Setup(x => x.GetAll()).Returns(new List<Reserva>());
            
            var result = _reservaController.GetAll();
            var okResult = result as OkObjectResult;
            var reservasResponse = okResult.Value as IEnumerable<ReservaModelResponse>;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsNotNull(reservasResponse);
            Assert.AreEqual(0, reservasResponse.Count());
            _reservaManagerMock.VerifyAll();
        }

        [TestMethod]
        public void GetById_WhenReservaExiste_ShouldRetornarReserva()
        {
            var reserva = new Reserva 
            { 
                Id = 1,
                NombreMedicamento = "Aspirina", 
                CantidadMedicamento = 2,
                FechaExpiracion = DateTime.Now.AddDays(7),
                FechaReserva = DateTime.Now,
                Usuario = "TestUser"
            };

            _reservaManagerMock.Setup(x => x.GetById(1)).Returns(reserva);
            
            var result = _reservaController.GetById(1);
            var okResult = result as OkObjectResult;
            var reservaResponse = okResult.Value as ReservaModelResponse;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsNotNull(reservaResponse);
            Assert.AreEqual(1, reservaResponse.Id);
            Assert.AreEqual("Aspirina", reservaResponse.NombreMedicamento);
            Assert.AreEqual(2, reservaResponse.CantidadMedicamento);
            _reservaManagerMock.VerifyAll();
        }

        [TestMethod]
        public void Delete_WhenReservaExiste_ShouldRetornar200()
        {
            _reservaManagerMock.Setup(x => x.Delete(1));
            
            var result = _reservaController.Delete(1);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _reservaManagerMock.VerifyAll();
        }

        [TestMethod]
        public void Delete_WhenSeElimina_ShouldRetornarMensajeDeExito()
        {
            _reservaManagerMock.Setup(x => x.Delete(1));
            
            var result = _reservaController.Delete(1);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            _reservaManagerMock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidResourceException))]
        public void Delete_WhenReservaEstaConfirmada_ShouldThrowException()
        {
            _reservaManagerMock.Setup(x => x.Delete(1))
                .Throws(new InvalidResourceException("No se puede cancelar una reserva confirmada."));
            
            _reservaController.Delete(1);
        }

        [TestMethod]
        public void Delete_WhenReservaNOEstaConfirmada_ShouldEliminarReserva()
        {
            _reservaManagerMock.Setup(x => x.Delete(1));
            
            var result = _reservaController.Delete(1);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsNotNull(okResult.Value);
            
            _reservaManagerMock.Verify(x => x.Delete(1), Times.Once);
        }

        [TestMethod]
        public void Create_WhenReservaEsValida_ShouldRetornarReservaModelResponse()
        {
            var reserva = new Reserva 
            { 
                Id = 1,
                NombreMedicamento = "Aspirina", 
                CantidadMedicamento = 2,
                FechaExpiracion = DateTime.Now.AddDays(7),
                FechaReserva = DateTime.Now,
                Usuario = "TestUser"
            };
            var reservaModel = new ReservaModel 
            { 
                NombreMedicamento = "Aspirina", 
                CantidadMedicamento = 2,
                FechaExpiracion = DateTime.Now.AddDays(7).ToString(),
                Usuario = "TestUser"
            };

            _reservaManagerMock.Setup(x => x.Create(It.IsAny<Reserva>())).Returns(reserva);
            
            var result = _reservaController.Create(reservaModel);
            var createdResult = result as ObjectResult;
            var reservaResponse = createdResult.Value as ReservaModelResponse;

            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.IsNotNull(reservaResponse);
            Assert.AreEqual("Aspirina", reservaResponse.NombreMedicamento);
            Assert.AreEqual(2, reservaResponse.CantidadMedicamento);
            _reservaManagerMock.VerifyAll();
        }

        // Tests para ConfirmarReserva

        [TestMethod]
        public void ConfirmarReserva_WhenRequierePrescripcionYTienePrescripcion_ShouldConfirmar()
        {
            var reserva = new Reserva 
            { 
                Id = 1,
                NombreMedicamento = "Antibiótico", 
                CantidadMedicamento = 1,
                FechaExpiracion = DateTime.Now.AddDays(7),
                FechaReserva = DateTime.Now,
                Usuario = "TestUser",
                RequierePrescripcion = true,
                Confirmada = true,
                TienePrescripcion = true,
                EmpleadoConfirmacion = "EmpleadoTest",
                FechaConfirmacion = DateTime.Now
            };

            var model = new ConfirmarReservaModel { TienePrescripcion = true };

            var sessionMock = new Mock<ISession>();
            sessionMock.Setup(s => s.TryGetValue("userName", out It.Ref<byte[]>.IsAny))
                .Returns(true);
            _reservaController.HttpContext.Session = sessionMock.Object;

            _reservaManagerMock.Setup(x => x.ConfirmarReserva(1, It.IsAny<string>(), true))
                .Returns(reserva);

            var result = _reservaController.ConfirmarReserva(1, model);
            var okResult = result as OkObjectResult;
            var reservaResponse = okResult.Value as ReservaModelResponse;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsNotNull(reservaResponse);
            Assert.IsTrue(reservaResponse.Confirmada);
            Assert.IsTrue(reservaResponse.TienePrescripcion);
        }

        [TestMethod]
        public void ConfirmarReserva_WhenNoRequierePrescripcion_ShouldConfirmarSinPrescripcion()
        {
            var reserva = new Reserva 
            { 
                Id = 1,
                NombreMedicamento = "Paracetamol", 
                CantidadMedicamento = 2,
                FechaExpiracion = DateTime.Now.AddDays(7),
                FechaReserva = DateTime.Now,
                Usuario = "TestUser",
                RequierePrescripcion = false,
                Confirmada = true,
                TienePrescripcion = false,
                EmpleadoConfirmacion = "EmpleadoTest",
                FechaConfirmacion = DateTime.Now
            };

            var model = new ConfirmarReservaModel { TienePrescripcion = false };

            var sessionMock = new Mock<ISession>();
            sessionMock.Setup(s => s.TryGetValue("userName", out It.Ref<byte[]>.IsAny))
                .Returns(true);
            _reservaController.HttpContext.Session = sessionMock.Object;

            _reservaManagerMock.Setup(x => x.ConfirmarReserva(1, It.IsAny<string>(), false))
                .Returns(reserva);

            var result = _reservaController.ConfirmarReserva(1, model);
            var okResult = result as OkObjectResult;
            var reservaResponse = okResult.Value as ReservaModelResponse;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsNotNull(reservaResponse);
            Assert.IsTrue(reservaResponse.Confirmada);
            Assert.IsFalse(reservaResponse.TienePrescripcion);
        }

        [TestMethod]
        public void ConfirmarReserva_WhenReservaNoExiste_ShouldReturn404()
        {
            var model = new ConfirmarReservaModel { TienePrescripcion = false };

            var sessionMock = new Mock<ISession>();
            sessionMock.Setup(s => s.TryGetValue("userName", out It.Ref<byte[]>.IsAny))
                .Returns(true);
            _reservaController.HttpContext.Session = sessionMock.Object;

            _reservaManagerMock.Setup(x => x.ConfirmarReserva(999, It.IsAny<string>(), false))
                .Throws(new ResourceNotFoundException("Reserva not found"));

            Assert.ThrowsException<ResourceNotFoundException>(() => _reservaController.ConfirmarReserva(999, model));
        }
    }
}