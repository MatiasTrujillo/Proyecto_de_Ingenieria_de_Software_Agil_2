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
            return Created(string.Empty, reservaCreated);
        }

        [HttpGet]
        public ActionResult<bool> CheckStock([FromQuery] string nombre, [FromQuery] int cantidad)
        {
            bool hayStock = _reservaManager.CheckStock(nombre, cantidad);
            return Ok(hayStock);
        }
}
}
