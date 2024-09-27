
using MiddlewarePcPos2080WinForms.Models;

namespace MiddlewarePcPos2080WinForms.Helpers
{
    public static class LoggerHelper
    {
        static ReaderWriterLock locker = new ReaderWriterLock();

        private static string GetPath(string fileName) => Path.Combine(SettingHelper._configs.LogPath, $"{fileName}.json");
        public static void SaveLog(string log)
        {
            var today = $"{DateTime.Now:yyyy-MM-dd}";

            try
            {
                var filePath = GetPath(today);
                locker.AcquireWriterLock(int.MaxValue);
                using var f = new FileStream(filePath, FileMode.Append,FileAccess.Write);
                using var w = new StreamWriter(f);
                w.WriteLine($"[{DateTime.Now}] {log}");
                w.Close();
                f.Close();
            }
            finally
            {
                locker.ReleaseWriterLock();
            }
        }
    }
}
