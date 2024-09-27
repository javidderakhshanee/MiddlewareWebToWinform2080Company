
using System.Diagnostics.CodeAnalysis;

namespace MiddlewarePcPos2080WinForms.Models
{
    public sealed class PosConfig
    {
        public PosConfig()
        {
            Ip = string.Empty;
            Port = 0;
            Provider = $"{Providers.Pasargad}";
            Timeout = 30;
            TerminalId = string.Empty;
            BankAccount = string.Empty;
        }

        [SetsRequiredMembers]
        public PosConfig(string ip, int port, string provider, int timeout, string terminalId, string bankAccount)
        {
            Ip = ip;
            Port = port;
            Provider = provider;
            Timeout = timeout;
            TerminalId = terminalId;
            BankAccount = bankAccount;
        }
        public required string Ip { get; set; }
        public required int Port { get; set; }
        public required string Provider { get; set; }
        public Providers ProvidersEnum
        {
            get
            {
                if (Enum.TryParse<Providers>(Provider, true, out var result))
                    return result;

                return Providers.Pasargad;
            }
        }
        public required int Timeout { get; set; }
        public required string TerminalId { get; set; }
        public required string BankAccount { get; set; }

        internal TypePayTemp ToPosModel()
        {
            return new TypePayTemp
            {
                BankAccNo = BankAccount,
                IP = Ip,
                Port = Port,
                isConnectPOS = true,
                TerminalNo = TerminalId,
                TimeOut = Timeout,
                ProviderId = ProvidersEnum
            };
        }
    }
}
