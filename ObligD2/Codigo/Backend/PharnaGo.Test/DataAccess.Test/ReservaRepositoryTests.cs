using Microsoft.EntityFrameworkCore;
using PharmaGo.DataAccess;
using PharmaGo.DataAccess.Repositories;
using PharmaGo.Domain.Entities;
using PharmaGo.Domain.Enums;
using PharmaGo.Domain.SearchCriterias;
using PharmaGo.WebApi.Models.Out;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaGo.Test.DataAccess.Test
{
    [TestClass]
    public class ReservaRepositoryTests
    {
        private PharmacyGoDbContext context;
        private ReservaRepository _reservaRepository;

        [TestCleanup]
        public void CleanUp()
        {
            context.Database.EnsureDeleted();
        }

        [TestMethod]
        public void InsertOne_WhenReservaValida_ShouldAgregarseEnDB()
        {
            var reserva = new Reserva { NombreMedicamento = "Aspirina", CantidadMedicamento = 2 };

            _reservaRepository.Add(reserva);

            _reservaRepository.InsertOne(reserva);
            _reservaRepository.Save();
            var recuperarReserva = _reservaRepository.GetOneByExpression(d => d.Id == reserva.Id);
            Assert.AreEqual(recuperarReserva.NombreMedicamento, reserva.NombreMedicamento);
        }
    }
}