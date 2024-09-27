using MiddlewarePcPos2080WinForms.Models;
using MiddlewarePcPos2080WinForms.Providers.Pasargard;
using System.Data;


namespace MiddlewarePcPos2080WinForms.Providers;

public static class PosPayment
{
    public static bool isWaitReaderPOSPC = false;
    private static CallBackPosViewModel _response = new CallBackPosViewModel();
    private static List<TypePayTemp> _gateways = new List<TypePayTemp>();
    public static void Load()
    {
        _gateways.Add(new TypePayTemp
        {
            isConnectPOS = true,
            ProviderId = Models.Providers.Pasargad
        });
    }

    internal static List<TypePayTemp> LoadAllPcPoc()
    {
        return LoadPcPoc(false);
    }
    internal static bool IsSinglePos()
    {
        return LoadPcPoc(false).Count == 1;
    }

    internal static List<TypePayTemp> LoadPcPoc(bool isConnectPOS = true)
    {
        if (isConnectPOS)
            return _gateways.Where(xx => xx.isConnectPOS).ToList();

        return _gateways;
    }
    internal static bool SendAmount(TypePayTemp gateway, long amount)
    {
        _response.AccountNo = gateway.BankAccNo ?? string.Empty;
        isWaitReaderPOSPC = true;
        switch (gateway.ProviderId)
        {
            case Models.Providers.Pasargad:
                PassargadProviderHelper.Instance.Prepare(gateway);
                return PassargadProviderHelper.Instance.SendToPos(amount);
        }

        return false;
    }

    internal static void SetCurrentResponse(CallBackPosViewModel response)
    {
        _response = response;
    }
    internal static CallBackPosViewModel GetCurrentResponse()
    {
        return _response;
    }
    internal static void ResetCurrentResponse()
    {
        _response = new CallBackPosViewModel();
    }
}
