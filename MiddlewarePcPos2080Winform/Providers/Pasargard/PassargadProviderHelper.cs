using MiddlewarePcPos2080WinForms.Models;
using PcPosClassLibrary;
using System.Text.Json;

namespace MiddlewarePcPos2080WinForms.Providers.Pasargard;

public sealed class PassargadProviderHelper
{
    private static PassargadProviderHelper instance = new PassargadProviderHelper();
    public static PassargadProviderHelper Instance => instance;
    private PCPOS? pos = null;
    private PassargadProviderHelper()
    {
    }

    public void Prepare(TypePayTemp gateway)
    {
        pos = new PCPOS(gateway.Port, gateway.IP);
        pos.DataRecieved += new DataRecievedEventHandler(pos_DataReceived);
        pos.SetLanReceiveTimeout(gateway.TimeOut);
    }

    public bool SendToPos(long Amount)
    {
        DoTransactionSync(pos, Amount);

        return true;
    }


    private RecievedData DoTransactionSync(PCPOS pos, long Amount)
    {
        var output = pos.SyncSale(Amount);

        pos.Close();

        return output;
    }

    private void pos_DataReceived(object sender, DataRecievedArgs e)
    {
        SettingHelper.Log = $"Concrete--->{JsonSerializer.Serialize(e.recievedData)}";
        if (e.recievedData != null)
        {
            if (e.recievedData.HasError == true)
                SetError(e.recievedData.ErrorCode, JsonSerializer.Serialize(e.recievedData));
            else
            {
                var outRefNo = e.recievedData.ReferenceNumber;
                SettingHelper.Log = outRefNo;
                if (!string.IsNullOrWhiteSpace(outRefNo) && e.recievedData.ErrorCode == 0)
                    SetSuccussed(e);
                else
                    SetError(e.recievedData.ErrorCode, JsonSerializer.Serialize(e.recievedData));
            }
        }

        if (pos.isLan)
            pos.Close();

        PosPayment.isWaitReaderPOSPC = false;
    }

    private static void SetError(int errorCode, string data)
    {
        PosPayment.SetCurrentResponse(new CallBackPosViewModel
        {
            IsSuccussed = false,
            ResponseCode = errorCode,
            Message = $"Transaction_Failed---->{data}"
        });
    }

    private void SetSuccussed(DataRecievedArgs e)
    {
        var data = new CallBackPosViewModel
        {
            IsSuccussed = true,
            CardNumberMask = $"{e.recievedData.CardNumber}************",
            Date = e.recievedData.Date,
            AccountNo = "0",
            TransactionSerialNo = long.Parse(e.recievedData.SequenceNumber).ToString(),
            TerminalNo = long.Parse(e.recievedData.TerminalCode).ToString(),
            TraceNumber = long.Parse(e.recievedData.SequenceNumber).ToString(),
            ReferenceNumber = e.recievedData.ReferenceNumber,
            Time = DateTime.Now.ToString("HH:mm:ss"),
            ResponseCode = 0,
            Message = "Transaction_Succussed"
        };

        long.TryParse(e.recievedData.Amount, out var amount);
        data.Amount = amount;

        PosPayment.SetCurrentResponse(data);
    }
}
