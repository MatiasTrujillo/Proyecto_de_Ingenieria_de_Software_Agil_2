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
using System.Xml.Linq;

namespace PharmaGo.Test.DataAccess.Test
{
    [TestClass]
    public class ReservaRepositoryTests
    {
        private PharmacyGoDbContext context;
        private ReservaRepository _reservaRepository;
        private List<Reserva> reservasSaved;

        [TestCleanup]
        public void CleanUp()
        {
            context.Database.EnsureDeleted();
        }

        private void CreateDataBase(string name)
        {
            reservasSaved = CreateDummyReservas();
            var options = new DbContextOptionsBuilder<PharmacyGoDbContext>().UseInMemoryDatabase(databaseName: name).Options;
            context = new PharmacyGoDbContext(options);
            reservasSaved.ForEach(d => context.Set<Reserva>().Add(d));
            context.Set<Reserva>();
            context.SaveChanges();
            _reservaRepository = new ReservaRepository(context);
        }

        private List<Reserva> CreateDummyReservas()
        {
            var reservaList = new List<Reserva>();
            for (int i = 1; i < 11; i++)
            {
                reservaList.Add(new Reserva()
                {
                    NombreMedicamento = "Aspirina",
                    CantidadMedicamento = 2
                });
            }
            return reservaList;
        }

        [TestMethod]
        public void InsertOne_WhenReservaValida_ShouldAgregarseEnDB()
        {
            CreateDataBase("crearReservaTestDb");
            var reserva = new Reserva { NombreMedicamento = "Aspirina", CantidadMedicamento = 2 };
            _reservaRepository.InsertOne(reserva);
            _reservaRepository.Save();
            var recuperarReserva = _reservaRepository.GetOneByExpression(d => d.Id == reserva.Id);
            Assert.AreEqual(recuperarReserva.NombreMedicamento, reserva.NombreMedicamento);
        }
    }
}