using System.Diagnostics.Metrics;

namespace PharmaGo.WebApi.Instrumentation
{
    public static class OpenTelemetryMetrics
    {
        private static readonly Meter meter = new Meter("PharmaGo.CustomMetrics", "1.0.0");

        public static readonly Counter<long> ReservationRequests = meter.CreateCounter<long>("reservation_requests_total");
        public static readonly Counter<long> ReservationErrors = meter.CreateCounter<long>("reservation_errors_total");
        public static readonly Histogram<double> ReservationDuration = meter.CreateHistogram<double>("reservation_request_duration_seconds");
    }
}
