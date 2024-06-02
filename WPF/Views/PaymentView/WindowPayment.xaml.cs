using MomoPayment;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using System.Windows;

namespace WPF.Views.PaymentView
{
    /// <summary>
    /// Interaction logic for WindowPayment.xaml
    /// </summary>
    public partial class WindowPayment : System.Windows.Controls.UserControl
    {
        string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
        string partnerCode = "MOMO5RGX20191128"; // thay bang key cua minh
        string accessKey = "M8brj9K6E22vXoDB";   // thay bang key cua minh
        string serectkey = "nqQiVSgDMy809JoPF6OzP5OdBUB550Y4"; // thay bang key cua minh
        string order_id;
            
        public WindowPayment()
        {
            InitializeComponent();
        }



        /*     private void btnThemgiohang_Click(object sender, RoutedEventArgs e)
             {
                 int totalAmount = 0;
                 if (ckbquan.IsChecked == true) totalAmount += (int)numquan.Value * 1000;

                 txtamount.Text = totalAmount.ToString();
             }*/
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
            }
            else if (radTwoYears.IsChecked == true)
            {
                totalAmount = 5500000; 
            }
            else if (radThreeYears.IsChecked == true)
            {
                totalAmount = 8000000; 
            }

            txtamount.Text = totalAmount.ToString();
        }



        private void btnThanhtoan_Click(object sender, RoutedEventArgs e)
        {
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = "MOMO";
            string accessKey = "F8BBA842ECF85";
            string secretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
            string orderInfo = "thanh toan dich vu tai rms";
            string redirectUrl = "https://momo.vn/return";
            string ipnUrl = "https://momo.vn/notify";
            string requestType = "captureWallet";

            string orderId = Guid.NewGuid().ToString();
            string amount = txtamount.Text;
            string requestId = Guid.NewGuid().ToString();
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

            }

         
        }

        private void btnKiemTraGiaoDich_Click(object sender, RoutedEventArgs e)
        {
            string orderId = "order_id"; // Thay bằng mã orderId thật
            string requestId = "request_id"; // Thay bằng mã requestId thật
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/query";
            string accessKey = "F8BBA842ECF85";
            string secretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";

            string rawHash = $"accessKey={accessKey}&orderId={orderId}&requestId={requestId}&partnerCode=MOMO";
            MomoSecurity crypto = new MomoSecurity();
            string signature = crypto.signSHA256(rawHash, secretKey);

            JObject message = new JObject
            {
                { "partnerCode", "MOMO" },
                { "orderId", orderId },
                { "requestId", requestId },
                { "signature", signature }
            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());
            JObject jmessage = JObject.Parse(responseFromMomo);
            string kq = jmessage.GetValue("resultCode").ToString();

            if (kq == "0")
                System.Windows.MessageBox.Show("Giao dịch thành công!");
            else
                System.Windows.MessageBox.Show("Giao dịch thất bại");
        }
    }

}

