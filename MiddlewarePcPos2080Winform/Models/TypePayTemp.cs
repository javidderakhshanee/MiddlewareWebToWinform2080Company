namespace MiddlewarePcPos2080WinForms.Models;

public sealed class TypePayTemp
{
    public string? BankAccNo { get;  set; }
    public bool isConnectPOS { get;  set; }

    public string IP { get;  set; }
    public string? TerminalNo { get;  set; }
    public int Port { get;  set; }
    public int TimeOut { get;  set; }
    public Providers ProviderId { get;  set; }
}
