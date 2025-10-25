using Microsoft.AspNetCore.Mvc;
using PharmaGo.Domain.Entities;
using PharmaGo.Domain.SearchCriterias;
using PharmaGo.IBusinessLogic;
using PharmaGo.WebApi.Enums;
using PharmaGo.WebApi.Filters;
using PharmaGo.WebApi.Models.In;
using PharmaGo.WebApi.Models.Out;

namespace PharmaGo.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionFilter))]
    public class ReservaController : Controller
    {
        private readonly IReservaManager _reservaManager;

        public ReservaController(IReservaManager manager)
        {
            _reservaManager = manager;
        }

        [HttpPost]
        public IActionResult Create([FromBody] ReservaModel reservaModel)
        {
            Reserva reservaCreated = _reservaManager.Create(reservaModel.ToEntity());
            ReservaModelResponse response = new ReservaModelResponse(reservaCreated);
            return Created(string.Empty, response);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Reserva> reservas = _reservaManager.GetAll();
            IEnumerable<ReservaModelResponse> response = reservas.Select(r => new ReservaModelResponse(r));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            Reserva reserva = _reservaManager.GetById(id);
            ReservaModelResponse response = new ReservaModelResponse(reserva);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _reservaManager.Delete(id);
            return Ok(new { message = "Reserva cancelada exitosamente" });
        }
    }
}
