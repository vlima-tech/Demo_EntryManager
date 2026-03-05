
namespace EntryManager.Shared.Interop
{
    public static class KnowHeaders
    {
        public const string TraceId = "x-trace-id";
        public const string TraceParent = "traceparent";
        public const string Metadata = "baggage";
        
        public const string CorrelationId = "x-correlation-id";
        public const string IdempotencyKey = "x-idempotency-key";
    }
}