using MiddlewarePcPos2080WinForms.Helpers;

namespace MiddlewarePcPos2080WinForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            try
            {
                Application.Run(new fMain());
            }
            catch (Exception ex)
            {
                LoggerHelper.SaveLog($"Global Exception Handling: {ex.Message}");
            }
            finally
            {
                Application.Restart();
            }
        }
    }
}