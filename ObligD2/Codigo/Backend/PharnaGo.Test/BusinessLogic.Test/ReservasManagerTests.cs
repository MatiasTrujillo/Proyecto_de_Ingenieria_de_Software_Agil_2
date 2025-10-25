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
            var reserva = new Reserva 
            { 
                NombreMedicamento = "Aspirina", 
                CantidadMedicamento = 2,
                FechaExpiracion = DateTime.Now.AddDays(7),
                FechaReserva = DateTime.Now,
                Usuario = "TestUser"
            };

            _reservaRepository.Setup(r => r.InsertOne(It.IsAny<Reserva>()));
            _reservaRepository.Setup(r => r.Save());

            _reservaManager.Create(reserva);

            _reservaRepository.Verify(repo => repo.InsertOne(
                It.Is<Reserva>(a =>
                    a.NombreMedicamento == reserva.NombreMedicamento &&
                    a.CantidadMedicamento == reserva.CantidadMedicamento)), Times.Once);
            _reservaRepository.Verify(repo => repo.Save(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidResourceException))]
        public void CrearReserva_WhenNombreMedicamentoVacio_ShouldThrowException()
        {
            var reserva = new Reserva 
            { 
                NombreMedicamento = "", 
                CantidadMedicamento = 2,
                FechaExpiracion = DateTime.Now.AddDays(7),
                Usuario = "TestUser"
            };

            _reservaManager.Create(reserva);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidResourceException))]
        public void CrearReserva_WhenCantidadMenorOIgualACero_ShouldThrowException()
        {
            var reserva = new Reserva 
            { 
                NombreMedicamento = "Aspirina", 
                CantidadMedicamento = 0,
                FechaExpiracion = DateTime.Now.AddDays(7),
                Usuario = "TestUser"
            };

            _reservaManager.Create(reserva);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidResourceException))]
        public void CrearReserva_WhenFechaExpiracionPasada_ShouldThrowException()
        {
            var reserva = new Reserva 
            { 
                NombreMedicamento = "Aspirina", 
                CantidadMedicamento = 2,
                FechaExpiracion = DateTime.Now.AddDays(-1),
                Usuario = "TestUser"
            };

            _reservaManager.Create(reserva);
        }

        [TestMethod]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void CrearReserva_WhenReservaEsNull_ShouldThrowException()
        {
            _reservaManager.Create(null);
        }

        [TestMethod]
        public void GetAll_WhenReservasExisten_ShouldRetornarLista()
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

            _reservaRepository.Setup(r => r.GetAllByExpression(It.IsAny<Expression<Func<Reserva, bool>>>()))
                .Returns(reservas);

            var result = _reservaManager.GetAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Aspirina", result.First().NombreMedicamento);
            _reservaRepository.Verify(r => r.GetAllByExpression(It.IsAny<Expression<Func<Reserva, bool>>>()), Times.Once);
        }

        [TestMethod]
        public void GetAll_WhenNoHayReservas_ShouldRetornarListaVacia()
        {
            _reservaRepository.Setup(r => r.GetAllByExpression(It.IsAny<Expression<Func<Reserva, bool>>>()))
                .Returns(new List<Reserva>());

            var result = _reservaManager.GetAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            _reservaRepository.Verify(r => r.GetAllByExpression(It.IsAny<Expression<Func<Reserva, bool>>>()), Times.Once);
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

            _reservaRepository.Setup(r => r.GetOneByExpression(It.IsAny<Expression<Func<Reserva, bool>>>()))
                .Returns(reserva);

            var result = _reservaManager.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Aspirina", result.NombreMedicamento);
            _reservaRepository.Verify(r => r.GetOneByExpression(It.IsAny<Expression<Func<Reserva, bool>>>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void GetById_WhenReservaNoExiste_ShouldThrowException()
        {
            _reservaRepository.Setup(r => r.GetOneByExpression(It.IsAny<Expression<Func<Reserva, bool>>>()))
                .Returns((Reserva)null);

            _reservaManager.GetById(999);
        }

        [TestMethod]
        public void Delete_WhenReservaExiste_ShouldEliminarReserva()
        {
            var reserva = new Reserva 
            { 
                Id = 1,
                NombreMedicamento = "Aspirina", 
                CantidadMedicamento = 2,
                FechaExpiracion = DateTime.Now.AddDays(7),
                FechaReserva = DateTime.Now,
                Usuario = "TestUser",
                Confirmada = false
            };

            _reservaRepository.Setup(r => r.GetOneByExpression(It.IsAny<Expression<Func<Reserva, bool>>>()))
                .Returns(reserva);
            _reservaRepository.Setup(r => r.DeleteOne(It.IsAny<Reserva>()));
            _reservaRepository.Setup(r => r.Save());

            _reservaManager.Delete(1);

            _reservaRepository.Verify(r => r.GetOneByExpression(It.IsAny<Expression<Func<Reserva, bool>>>()), Times.Once);
            _reservaRepository.Verify(r => r.DeleteOne(It.Is<Reserva>(res => res.Id == 1)), Times.Once);
            _reservaRepository.Verify(r => r.Save(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidResourceException))]
        public void Delete_WhenReservaEstaConfirmada_ShouldThrowException()
        {
            var reserva = new Reserva 
            { 
                Id = 1,
                NombreMedicamento = "Aspirina", 
                CantidadMedicamento = 2,
                FechaExpiracion = DateTime.Now.AddDays(7),
                FechaReserva = DateTime.Now,
                Usuario = "TestUser",
                Confirmada = true
            };

            _reservaRepository.Setup(r => r.GetOneByExpression(It.IsAny<Expression<Func<Reserva, bool>>>()))
                .Returns(reserva);

            _reservaManager.Delete(1);
        }

        [TestMethod]
        public void Delete_WhenReservaNOEstaConfirmada_ShouldEliminarReserva()
        {
            var reserva = new Reserva 
            { 
                Id = 1,
                NombreMedicamento = "Aspirina", 
                CantidadMedicamento = 2,
                FechaExpiracion = DateTime.Now.AddDays(7),
                FechaReserva = DateTime.Now,
                Usuario = "TestUser",
                Confirmada = false
            };

            _reservaRepository.Setup(r => r.GetOneByExpression(It.IsAny<Expression<Func<Reserva, bool>>>()))
                .Returns(reserva);
            _reservaRepository.Setup(r => r.DeleteOne(It.IsAny<Reserva>()));
            _reservaRepository.Setup(r => r.Save());

            _reservaManager.Delete(1);

            _reservaRepository.Verify(r => r.DeleteOne(It.Is<Reserva>(res => res.Id == 1 && res.Confirmada == false)), Times.Once);
            _reservaRepository.Verify(r => r.Save(), Times.Once);
        }

        [TestMethod]
        public void PuedeCancelarse_WhenReservaNOConfirmada_ShouldReturnTrue()
        {
            var reserva = new Reserva 
            { 
                Id = 1,
                NombreMedicamento = "Aspirina", 
                CantidadMedicamento = 2,
                FechaExpiracion = DateTime.Now.AddDays(7),
                FechaReserva = DateTime.Now,
                Usuario = "TestUser",
                Confirmada = false
            };

            var result = reserva.PuedeCancelarse();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PuedeCancelarse_WhenReservaConfirmada_ShouldReturnFalse()
        {
            var reserva = new Reserva 
            { 
                Id = 1,
                NombreMedicamento = "Aspirina", 
                CantidadMedicamento = 2,
                FechaExpiracion = DateTime.Now.AddDays(7),
                FechaReserva = DateTime.Now,
                Usuario = "TestUser",
                Confirmada = true
            };

            var result = reserva.PuedeCancelarse();

            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void Delete_WhenReservaNoExiste_ShouldThrowException()
        {
            _reservaRepository.Setup(r => r.GetOneByExpression(It.IsAny<Expression<Func<Reserva, bool>>>()))
                .Returns((Reserva)null);

            _reservaManager.Delete(999);
        }
    }
}