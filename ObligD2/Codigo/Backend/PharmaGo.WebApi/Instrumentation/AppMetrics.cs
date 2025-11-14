using System.Diagnostics;
using Prometheus;

namespace PharmaGo.WebApi.Instrumentation
{
    // Métricas globales para las reservas (prometheus-net)
    public static class AppMetrics
    {
        // Counter: total de requests (etiquetas: endpoint, method, status_code)
        public static readonly Counter ReservationRequestsTotal = Metrics.CreateCounter(
            "reservation_requests_total",
            "Total reservation requests",
            new CounterConfiguration
            {
                LabelNames = new[] { "endpoint", "method", "status_code" }
            }
        );

        // Counter: errores en reservas (etiquetas: endpoint, method, status_code, error_type)
        public static readonly Counter ReservationErrorsTotal = Metrics.CreateCounter(
            "reservation_errors_total",
            "Reservation errors (4xx/5xx)",
            new CounterConfiguration
            {
                LabelNames = new[] { "endpoint", "method", "status_code", "error_type" }
            }
        );

        // Histogram: duración de la petición en segundos (etiquetas: endpoint, method)
        // Buckets de ejemplo: desde 10ms hasta ~0.5s (ajustar según latencia real)
        public static readonly Histogram ReservationDuration = Metrics.CreateHistogram(
            "reservation_request_duration_seconds",
            "Reservation request duration seconds",
            new HistogramConfiguration
            {
                Buckets = Histogram.LinearBuckets(start: 0.01, width: 0.05, count: 10),
                LabelNames = new[] { "endpoint", "method" }
            }
        );
    }
}
