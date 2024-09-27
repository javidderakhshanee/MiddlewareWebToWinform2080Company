using MiddlewarePcPos2080WinForms.Helpers;
using MiddlewarePcPos2080WinForms.Providers;
using System.Text.Json;

namespace MiddlewarePcPos2080WinForms.Models;

public sealed class SettingHelper
{
    public static ConfigModel _configs = null;
    private static string PasargadReturnCodeFileName => "PasargadReturnCodes.json";
    public static ProviderCodes GetResponseCodes(Providers provider)
    {
        if (provider == Providers.Pasargad)
        {
            var path = _configs.ProvidersReturnCodesFilePath.Pasargad;
            if (string.IsNullOrWhiteSpace(path))
                path = Application.StartupPath;

            path = Path.Combine(path, PasargadReturnCodeFileName);

            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json))
                    return new();

                return JsonSerializer.Deserialize<ProviderCodes>(json);
            }
        }

        return new();
    }

    private static string _log = string.Empty;
    public static string Log
    {
        get { return _log; }
        set
        {
            _log = value;
            LoggerHelper.SaveLog(_log);
        }
    }
}
