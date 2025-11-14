using Microsoft.AspNetCore.Mvc;
using PharmaGo.Domain.Entities;
using PharmaGo.Domain.SearchCriterias;
using PharmaGo.IBusinessLogic;
using PharmaGo.WebApi.Enums;
using PharmaGo.WebApi.Filters;
using PharmaGo.WebApi.Instrumentation;
using System.Diagnostics;
using PharmaGo.WebApi.Models.In;
using PharmaGo.WebApi.Models.Out;
using System.Linq;
using Microsoft.AspNetCore.Http;

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
            var sw = Stopwatch.StartNew();
            string endpoint = "/api/reserva";
            string method = "POST";
            try
            {
                // Testing helper: if client sends NombreMedicamento == "FORCE500" we force a server exception
                if (reservaModel != null && string.Equals(reservaModel.NombreMedicamento, "FORCE500", System.StringComparison.OrdinalIgnoreCase))
                {
                    throw new System.Exception("Forced 500 for testing");
                }
                Reserva reservaCreated = _reservaManager.Create(reservaModel.ToEntity());
                ReservaModelResponse response = new ReservaModelResponse(reservaCreated);
                sw.Stop();
                double seconds = sw.Elapsed.TotalSeconds;
                // prometheus-net metrics (local)
                AppMetrics.ReservationDuration.WithLabels(endpoint, method).Observe(seconds);
                AppMetrics.ReservationRequestsTotal.WithLabels(endpoint, method, "201").Inc();
                // OpenTelemetry metrics (exported via OTLP collector)
                OpenTelemetryMetrics.ReservationDuration.Record(seconds, new System.Collections.Generic.KeyValuePair<string, object>("endpoint", endpoint), new System.Collections.Generic.KeyValuePair<string, object>("method", method));
                OpenTelemetryMetrics.ReservationRequests.Add(1, new System.Collections.Generic.KeyValuePair<string, object>("endpoint", endpoint), new System.Collections.Generic.KeyValuePair<string, object>("method", method), new System.Collections.Generic.KeyValuePair<string, object>("status_code", "201"));
                return Created(string.Empty, response);
            }
            catch (System.Exception ex)
            {
                sw.Stop();
                double seconds = sw.Elapsed.TotalSeconds;
                // prometheus-net
                AppMetrics.ReservationDuration.WithLabels(endpoint, method).Observe(seconds);
                AppMetrics.ReservationErrorsTotal.WithLabels(endpoint, method, "500", "exception").Inc();
                // Also count the request with status_code=500 so Prometheus can filter by status
                AppMetrics.ReservationRequestsTotal.WithLabels(endpoint, method, "500").Inc();
                // OpenTelemetry
                OpenTelemetryMetrics.ReservationDuration.Record(seconds, new System.Collections.Generic.KeyValuePair<string, object>("endpoint", endpoint), new System.Collections.Generic.KeyValuePair<string, object>("method", method));
                OpenTelemetryMetrics.ReservationErrors.Add(1, new System.Collections.Generic.KeyValuePair<string, object>("endpoint", endpoint), new System.Collections.Generic.KeyValuePair<string, object>("method", method), new System.Collections.Generic.KeyValuePair<string, object>("status_code", "500"));
                // Also record the request counter with status_code=500 for OTLP/collector
                OpenTelemetryMetrics.ReservationRequests.Add(1, new System.Collections.Generic.KeyValuePair<string, object>("endpoint", endpoint), new System.Collections.Generic.KeyValuePair<string, object>("method", method), new System.Collections.Generic.KeyValuePair<string, object>("status_code", "500"));
                throw; // let the global ExceptionFilter handle response
            }
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

        [HttpPatch("{id}/confirmar")]
        [AuthorizationFilter(new string[] { nameof(RoleType.Employee) })]
        public IActionResult ConfirmarReserva([FromRoute] int id, [FromBody] ConfirmarReservaModel model)
        {
            string empleado = HttpContext.Session.GetString("userName");
            Reserva reserva = _reservaManager.ConfirmarReserva(id, empleado, model.TienePrescripcion);
            ReservaModelResponse response = new ReservaModelResponse(reserva);
            return Ok(response);
        }

        [HttpGet("check-stock")]
        public ActionResult<bool> CheckStock([FromQuery] string nombre, [FromQuery] int cantidad)
        {
            bool hayStock = _reservaManager.CheckStock(nombre, cantidad);
            return Ok(hayStock);
        }
    }
}
