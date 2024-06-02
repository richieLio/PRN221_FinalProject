using MomoPayment;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using System.Windows;
using DataAccess.Repository;
using System.Net;
using System.Text;
using BusinessObject.Object;
using System.Net.Http;
using static Google.Apis.Requests.BatchRequest;
using MailKit.Search;
using DataAccess.Model;

namespace WPF.Views.PaymentView
{
    /// <summary>
    /// Interaction logic for WindowPayment.xaml
    /// </summary>
    public partial class WindowPayment : System.Windows.Controls.UserControl
    {
        private readonly PaymentViewModel _viewModel;


        private readonly ICombineRepository _repository;
        private int _selectedYears = 1; // Default to 1 year
        string _endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
        string _partnerCode = "MOMO"; // thay bang key cua minh
        string _accessKey = "F8BBA842ECF85";   // thay bang key cua minh
        string _serectkey = "K951B6PE1waDMi640xX08PD3vg6EkVlz"; // thay bang key cua minh
        string _orderIdToCheck = "";
        string _requestIdToCheck = "";
        string _redirectUrl = "";

        public WindowPayment(ICombineRepository repository)
        {
            InitializeComponent();
            _repository = repository;
            _viewModel = new PaymentViewModel();
            DataContext = _viewModel;
            LoadLicence();
        }
        public async void LoadLicence()
        {
            var licence = await _repository.GetLicenceByUserId(App.LoggedInUserId);
            var remainingDays = (licence.ExpiryDate - DateTime.Now)?.Days ?? 0;
            if (licence.IsLicence == true)
            {
                _viewModel.ContractMessage = $"Hợp đồng của bạn còn đến ngày {licence.ExpiryDate}";
            }
        }




        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateTotalAmount();
        }

        private void UpdateTotalAmount()
        {
            int totalAmount = 0;

            if (radOneYear.IsChecked == true)
            {
                totalAmount = 3000000;
                _selectedYears = 1;

            }
            else if (radTwoYears.IsChecked == true)
            {
                totalAmount = 5500000;
                _selectedYears = 2;

            }
            else if (radThreeYears.IsChecked == true)
            {
                totalAmount = 8000000;
                _selectedYears = 3;

            }

            txtamount.Text = totalAmount.ToString();
        }



        private void btnThanhtoan_Click(object sender, RoutedEventArgs e)
        {
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = _partnerCode;
            string accessKey = _accessKey;
            string secretKey = _serectkey;
            string orderInfo = "Thanh toan dich vu tai RMS";
            string redirectUrl = "https://momo.vn/return";
            string ipnUrl = "https://momo.vn/notify";
            string requestType = "captureWallet";

            string orderId = Guid.NewGuid().ToString();
            _orderIdToCheck = orderId;
            string amount = txtamount.Text;
            string requestId = Guid.NewGuid().ToString();
            _requestIdToCheck = requestId;
            string extraData = "";

            string rawHash = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";
            MomoSecurity crypto = new MomoSecurity();
            string signature = crypto.signSHA256(rawHash, secretKey);

            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerName", "Test" },
                { "storeId", "MomoTestStore" },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "redirectUrl", redirectUrl },
                { "ipnUrl", ipnUrl },
                { "lang", "vi" },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }
            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());
            JObject jmessage = JObject.Parse(responseFromMomo);
            var result = System.Windows.MessageBox.Show("Ấn OK để tới trang thanh toán", "Thông báo", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Uri uri = new Uri(jmessage.GetValue("payUrl").ToString());
                Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
                _redirectUrl = uri.AbsoluteUri;
                TransactionCheck();

            }
        }
        private async Task WaitForSuccess()
        {
            string signatureData = $"accessKey={_accessKey}&orderId={_orderIdToCheck}&partnerCode={_partnerCode}&requestId={_requestIdToCheck}";
            string signatureCheck = CalculateHmacSHA256Signature(signatureData, _serectkey);

            JObject request = new JObject
                                     {
        { "partnerCode", _partnerCode },
        { "requestId", _requestIdToCheck },
        { "orderId", _orderIdToCheck },
        { "signature", signatureCheck },
        { "lang", "en" }
                     };

            string responseFromMomo = SendRequestToMomo("https://test-payment.momo.vn/v2/gateway/api/query", request.ToString());
            JObject jsonResponse = JObject.Parse(responseFromMomo);
            string resultCode = jsonResponse.GetValue("resultCode").ToString();


            // Khi resultCode là "0" (thành công), tiến hành xử lý tiếp
            MessageBox.Show("Giao dịch thành công!");
        }



        private async void TransactionCheck()
        {
            string orderId = _orderIdToCheck; // Thay bằng mã orderId thật
            string requestId = _requestIdToCheck; // Thay bằng mã requestId thật
            string accessKey = _accessKey;
            string secretKey = _serectkey;
            string partnerCode = _partnerCode;

            string signatureData = $"accessKey={accessKey}&orderId={orderId}&partnerCode={partnerCode}&requestId={requestId}";
            string signature = CalculateHmacSHA256Signature(signatureData, secretKey);

            JObject request = new JObject
    {
        { "partnerCode", partnerCode },
        { "requestId", requestId },
        { "orderId", orderId },
        { "signature", signature },
        { "lang", "en" }
    };


            string response = SendRequestToMomo("https://test-payment.momo.vn/v2/gateway/api/query", request.ToString());
            JObject jmessage = JObject.Parse(response);
            string resultCode = jmessage.GetValue("resultCode").ToString();
            string status = jmessage.GetValue("message").ToString();




            while (status == "Transaction is initiated, waiting for user confirmation.")
            {
                response = SendRequestToMomo("https://test-payment.momo.vn/v2/gateway/api/query", request.ToString());
                jmessage = JObject.Parse(response);
                resultCode = jmessage.GetValue("resultCode").ToString();
                status = jmessage.GetValue("message").ToString();
            }

            if (resultCode == "0")
            {
                MessageBox.Show("Giao dịch thành công!");

                var transactionHistory = new TransactionHistory
                {
                    Id = Guid.NewGuid(),
                    UserId = App.LoggedInUserId,
                    Amount = decimal.Parse(txtamount.Text),
                    CreatedAt = DateTime.Now,
                    IpnUrl = "https://momo.vn/notify",
                    OrderId = orderId,
                    PartnerCode = partnerCode,
                    OrderInfo = "Thanh toan dich vu tai RMS",
                    Signature = signature,
                    RedirectUrl = _redirectUrl,
                    RequestId = requestId,
                    Response = response,
                };
                _repository.InsertTransaction(transactionHistory);


                var existingLicence = await _repository.GetLicenceByUserId(App.LoggedInUserId);
                if (existingLicence != null)
                {
                    if (existingLicence.IsLicence == true)
                    {
                        // Trường hợp user vẫn còn hợp đồng thì gia hạn
                        var remainingDays = (existingLicence.ExpiryDate - DateTime.Now)?.Days ?? 0;
                        var newExpiryDate = DateTime.Now.AddDays(remainingDays).AddYears(_selectedYears);
                        existingLicence.ExpiryDate = newExpiryDate;
                        _repository.UpdateLicence(existingLicence);
                    }
                }
                else
                {
                    // Trường hợp user chưa có hợp đồng thì tạo hợp đồng mới
                    var newLicence = new Licence
                    {
                        Id = Guid.NewGuid(),
                        IsLicence = true,
                        IssueDate = DateTime.Now,
                        ExpiryDate = DateTime.Now.AddYears(_selectedYears),
                        UserId = App.LoggedInUserId,
                    };
                    _repository.InsertLicence(newLicence);
                }

            }
            else
            {
                MessageBox.Show("Giao dịch thất bại hoặc thời gian giao dịch đã hết hiệu lực");
            }
        }



        private string CalculateHmacSHA256Signature(string data, string key)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        // Hàm gửi request đến Momo và nhận response
        private string SendRequestToMomo(string url, string requestData)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    return client.UploadString(url, "POST", requestData);
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi gửi request
                return ex.Message;
            }
        }
    }

}

