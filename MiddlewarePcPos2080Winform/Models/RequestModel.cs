using System.Diagnostics.CodeAnalysis;

namespace MiddlewarePcPos2080WinForms.Models
{
    public sealed class RequestModel
    {
        [SetsRequiredMembers]
        public RequestModel()
        {
            InvoiceId = string.Empty;
            Price = 0;
            Unit = "Rial";
            PosConfig = new PosConfig(string.Empty, 0, $"{Providers.Pasargad}", 30, string.Empty, string.Empty);
        }
        public required string InvoiceId { get; set; }
        public required decimal Price { get; set; }
        public decimal AmountByUnit => Price * (UnitEnum == UnitPrice.Toman ? 10 : 1);
        public required string Unit { get; set; }
        public UnitPrice UnitEnum
        {
            get
            {
                if (Enum.TryParse<UnitPrice>(Unit, true, out var result))
                    return result;

                return UnitPrice.Toman;
            }
        }
        public required PosConfig PosConfig { get; set; }
    }
}
