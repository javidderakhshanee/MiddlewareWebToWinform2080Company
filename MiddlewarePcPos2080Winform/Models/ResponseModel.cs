namespace MiddlewarePcPos2080WinForms.Models
{
    public sealed class ResponseModel
    {
        public string InvoiceId { get; set; }=string.Empty;
        public long TransactionDateTime { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
        public bool IsSuccess => PosResponse.Code == 0;
        public RequestModel TransactionRequest { get; set; } = new();
        public PspConcreteResponse PosResponse { get; set; } = new();
    }
}
