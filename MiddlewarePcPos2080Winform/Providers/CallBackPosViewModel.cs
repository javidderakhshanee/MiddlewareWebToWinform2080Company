using MiddlewarePcPos2080WinForms.Models;

namespace MiddlewarePcPos2080WinForms.Providers;

public sealed class CallBackPosViewModel
{
    public TypePayTemp SelectedPos { get; set; } = new();
    public long Amount { get; set; } = 0;
    public int Index { get; set; } = 0;

    public int ResponseCode { get; set; } = -1;
    public bool IsSuccussed { get; set; } = false;
    public bool NotToSend { get; set; } = false;
    public string TerminalNo { get; set; } = string.Empty;
    public string CardNumberMask { get; set; } = string.Empty;
    public string AccountNo { get; set; } = string.Empty;
    public string TransactionSerialNo { get; set; } = string.Empty;
    public string TraceNumber { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
}
