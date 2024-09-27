using MiddlewarePcPos2080WinForms.Helpers;
using MiddlewarePcPos2080WinForms.Models;
using MiddlewarePcPos2080WinForms.Providers;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace MiddlewarePcPos2080WinForms
{
    public partial class fMain : Form
    {
        private RequestModel CurrentRequest;
        private string CurrentRequestFilePath;
        private int _currentTransactionIndexOnWaitPay = 0;
        private TypePayTemp _currentTransactionPos = null;
        private List<CallBackPosViewModel> _transactions = new List<CallBackPosViewModel>();
        private int counter = 0;
        private int errorCounter = 0;
        private int retryWaiting = 0;
        private object deleteRequestFile = new object();
        public fMain()
        {
            InitializeComponent();
            ReadConfig();
        }

        private void ReadConfig()
        {
            var configPath = Path.Combine(Application.StartupPath, "configs.json");
            if (!File.Exists(configPath))
            {
                MessageBox.Show("فایل کانفیگ یافت نشد", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SettingHelper._configs = JsonSerializer.Deserialize<ConfigModel>(File.ReadAllText(configPath));
            if (SettingHelper._configs is null)
            {
                MessageBox.Show("کانفیگ معتبر نیست!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!string.IsNullOrWhiteSpace(SettingHelper._configs.RequestPath) && !Directory.Exists(SettingHelper._configs.RequestPath))
                try
                {
                    Directory.CreateDirectory(SettingHelper._configs.RequestPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            if (!string.IsNullOrWhiteSpace(SettingHelper._configs.ResponsePath) && !Directory.Exists(SettingHelper._configs.ResponsePath))
                try
                {
                    Directory.CreateDirectory(SettingHelper._configs.ResponsePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            if (!string.IsNullOrWhiteSpace(SettingHelper._configs.LogPath) && !Directory.Exists(SettingHelper._configs.LogPath))
                try
                {
                    Directory.CreateDirectory(SettingHelper._configs.LogPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            timerPullRequests.Interval = SettingHelper._configs?.TimePullMillisecond ?? 0;
            timerPullRequests.Enabled = true;
            timerPullRequests.Start();
        }

        private void StopWaiting()
        {
            lbState.Visible = false;
            timerWaitingPos.Enabled = false;
            timerWaitingPos.Stop();

            timerPullRequests.Enabled = true;
            timerPullRequests.Start();
        }
        private void StartWaiting(string msg = "")
        {
            timerWaitingPos.Interval = 1000;
            timerWaitingPos.Start();
            timerWaitingPos.Enabled = true;

            timerPullRequests.Enabled = false;
            timerPullRequests.Stop();

            ShowLoading(msg);
        }

        private void ShowLoading(string msg = "")
        {
            lbState.BackColor = Color.LemonChiffon;
            lbState.Text = string.IsNullOrWhiteSpace(msg) ? "Wait for pay card..." : msg;
            lbState.Visible = true;
        }

        private void ShowLog(string msg = "", bool saveLog = true, bool isError = false)
        {
            if (isError)
                StopWaiting();

            if (saveLog)
                SettingHelper.Log = msg;

            listBox1.Items.Add($"[{DateTime.Now}] {msg}");
            if (!isError)
                return;

            lbState.BackColor = Color.MistyRose;
            lbState.Text = !string.IsNullOrEmpty(msg) ? msg : "POCPC_Failed";
            lbState.Visible = true;

        }

        private void fMain_Shown(object sender, EventArgs e)
        {
            PosPayment.Load();
        }

        private void ResetCurrentSelectedTransaction()
        {
            PosPayment.ResetCurrentResponse();
            _currentTransactionIndexOnWaitPay = 0;
            _currentTransactionPos = null;
            CurrentRequestFilePath = string.Empty;
            CurrentRequest = new();
        }
        private void ShowSuccussed(Panel pnlTransaction = null)
        {
            lbState.BackColor = Color.PaleGreen;
            lbState.Text = "Succussed!";

        }
        private void CallBackPayPos(RequestModel request, CallBackPosViewModel response)
        {
            response.Index = _currentTransactionIndexOnWaitPay;

            GenerateResponse(new PspConcreteResponse
            {
                Code = response.ResponseCode,
                Message = response.Message,
                customerCardNumber = response.CardNumberMask,
                ReferenceNumber = response.ReferenceNumber,
                TraceNumber = response.TraceNumber,
                TransactionId = response.TransactionSerialNo
            },
            request);

            ShowSuccussed();
        }


        private void timerWaitingPos_Tick(object sender, EventArgs e)
        {
            lbState.Visible = true;

            if (!PosPayment.isWaitReaderPOSPC)
            {
                timerPullRequests.Enabled = false;
                timerPullRequests.Stop();
                var resp = PosPayment.GetCurrentResponse();
                var responseCodes = SettingHelper.GetResponseCodes(_currentTransactionPos.ProviderId);
                if (!resp.IsSuccussed)
                    resp.Message = $"Transaction_Failed---->{responseCodes?.Codes?.FirstOrDefault(x => x.Code == resp.ResponseCode)?.Message ?? resp.Message}";

                if (resp.IsSuccussed)
                {
                    ShowLog("IsSuccussed");
                    CallBackPayPos(CurrentRequest, resp);
                    DeleteRequestFile(CurrentRequestFilePath);
                    CurrentRequestFilePath = string.Empty;
                    CurrentRequest = new();
                    errorCounter = retryWaiting = 0;
                    StopWaiting();
                    return;
                }

                errorCounter++;
                if (resp.ResponseCode == 128)
                {
                    errorCounter = SettingHelper._configs.Retry + 1;
                    retryWaiting = SettingHelper._configs.WaitTimeToContinueRetrySecond + 1;
                    FailedTransaction(CurrentRequest, CurrentRequestFilePath, "Canceled by User", resp.ResponseCode);
                    ShowLog(resp.Message);
                    ResetCurrentSelectedTransaction();
                    return;
                }

                GenerateResponse(new PspConcreteResponse
                {
                    ReferenceNumber = resp.ReferenceNumber,
                    Code = resp.ResponseCode,
                    customerCardNumber = resp.CardNumberMask,
                    Message = resp.Message,
                    TraceNumber = resp.TraceNumber,
                    TransactionId = resp.TransactionSerialNo
                }, CurrentRequest);

                ShowLog(resp.Message);
                ResetCurrentSelectedTransaction();
                StopWaiting();
            }


            if (string.IsNullOrWhiteSpace(SettingHelper.Log))
                return;

            ShowLog(SettingHelper.Log);

            SettingHelper.Log = string.Empty;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Clipboard.SetText($"{listBox1.SelectedItem}");
        }

        private async void timerPullRequests_Tick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SettingHelper._configs.RequestPath))
                return;

            ShowLoading("Waiting Pull Request...");
            timerPullRequests.Stop();
            timerPullRequests.Enabled = false;
            var requestsFile = Directory.GetFiles(SettingHelper._configs.RequestPath, "*.json").ToList();
            if (!requestsFile.Any())
            {
                timerPullRequests.Enabled = true;
                timerPullRequests.Start();
                return;
            }

            foreach (var requestFile in requestsFile)
            {
                var contentJson = File.ReadAllText(requestFile);
                var request = JsonSerializer.Deserialize<RequestModel>(contentJson);
                if (request != null)
                    await ProcessQueueRequest(request, requestFile);
                else
                    ShowLog($"Request Is Invalid!---> {contentJson}");
            }

            timerPullRequests.Enabled = true;
            timerPullRequests.Start();
        }

        private void DeleteRequestFile(string? requestFile)
        {
            if (string.IsNullOrWhiteSpace(requestFile))
                return;

            if (!File.Exists(requestFile))
                return;

            lock (deleteRequestFile)
            {
                try
                {
                    File.Delete(requestFile);
                    ShowLog("Deleted Request File");
                }
                catch (Exception ex)
                {
                    ShowLog($"[ERR DeleteRequestFile]->{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }

        private async Task ProcessQueueRequest(RequestModel request, string requestFile)
        {
            if (request is null)
                return;

            if (errorCounter > 0 && retryWaiting < SettingHelper._configs.WaitTimeToContinueRetrySecond)
            {
                retryWaiting++;
                ShowLoading($"Waiting retry connect to device...   {retryWaiting}");
                return;
            }
            else if (errorCounter == SettingHelper._configs.Retry)
            {
                errorCounter = 0;
                FailedTransaction(request, requestFile, "POS Device Not Found!",-1);
                return;
            }

            retryWaiting = 0;

            ShowLoading("Check IP connectivity...");
            if (!await PingDevice(request.PosConfig.Ip))
            {
                ShowLog($"{request.PosConfig.Ip} is not connected!", isError: true);
                errorCounter++;
                return;
            }

            StartWaiting();

            CurrentRequest = request;
            CurrentRequestFilePath = requestFile;
            _currentTransactionIndexOnWaitPay = 0;
            _currentTransactionPos = request.PosConfig.ToPosModel();
            ShowLog($"Send Amount to POS: {request.AmountByUnit}");
            if (!PosPayment.SendAmount(_currentTransactionPos, (long)request.AmountByUnit))
            {
                ShowLog("POCPC_Failed");
                errorCounter++;
                return;
            }

        }

        private void FailedTransaction(RequestModel req, string requestFile,
            string msg,int responseCode)
        {
            var response = new PspConcreteResponse
            {
                Code = responseCode,
                Message = msg
            };

            if (SettingHelper._configs.AfterRetryFailedStrategyEnum == EnumAfterRetryFailedStrategy.Remove)
            {
                DeleteRequestFile(requestFile);
                GenerateResponse(response, req);
                StopWaiting();
                return;
            }

            GenerateResponse(response, req);
            HoldRequestFile(requestFile);
            StopWaiting();
        }

        private void GenerateResponse(PspConcreteResponse pspConcreteResponse, RequestModel request)
        {
            if (string.IsNullOrWhiteSpace(SettingHelper._configs.ResponsePath))
                return;


            var response = new ResponseModel();

            response.PosResponse = pspConcreteResponse;
            response.TransactionRequest = request;
            response.InvoiceId = request.InvoiceId;

            var json = JsonSerializer.Serialize(response);

            try
            {
                File.WriteAllText(Path.Combine(SettingHelper._configs.ResponsePath, $"{request.InvoiceId}.json"), json);
                ShowLog("Response Generated");
            }
            catch (Exception ex)
            {
                ShowLog(ex.Message);
            }
        }

        private void HoldRequestFile(string requestFile)
        {
            if (string.IsNullOrWhiteSpace(requestFile))
                return;

            if (!File.Exists(requestFile))
                return;

            lock (deleteRequestFile)
            {
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(requestFile);
                    var newFile = Path.Combine(Path.GetDirectoryName(requestFile), $"{fileName}.failed");
                    File.Move(requestFile, newFile);
                    ShowLog("Request File was Hold");
                }
                catch (Exception ex)
                {
                    LoggerHelper.SaveLog($"[ERR HoldRequestFile]->{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }

        private async Task<bool> PingDevice(string ip)
        {
            try
            {
                var reply = await new Ping().SendPingAsync(ip);

                return reply.Status == IPStatus.Success;
            }
            catch { }

            return false;
        }
    }
}
