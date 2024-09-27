namespace MiddlewarePcPos2080WinForms.Models
{
    public sealed class ConfigModel
    {
        public required string RequestPath { get; set; }
        public required string ResponsePath { get; set; }
        public required string LogPath { get; set; }
        public required int TimePullSecond { get; set; }
        public int TimePullMillisecond => TimePullSecond * 1000;
        public required bool IsSavePath { get; set; }
        public required string AfterRetryFailedStrategy { get; set; }
        public required ProvidersReturnCodesFilePath ProvidersReturnCodesFilePath { get; set; }
        public EnumAfterRetryFailedStrategy AfterRetryFailedStrategyEnum
        {
            get
            {
                if (Enum.TryParse<EnumAfterRetryFailedStrategy>(AfterRetryFailedStrategy, true, out var result))
                    return result;

                return EnumAfterRetryFailedStrategy.Hold;

            }
        }
        public required int WaitTimeToContinueRetrySecond { get; set; }
        public required int Retry { get; set; }

    }
}
