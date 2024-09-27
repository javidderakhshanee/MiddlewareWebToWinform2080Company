namespace MiddlewarePcPos2080WinForms.Models
{
    public sealed class PspConcreteResponse
    {
        public int Code { get; set; } = -1;
        public string Message { get; set; } = string.Empty;
        public string customerCardNumber { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string TraceNumber { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
    }
}
