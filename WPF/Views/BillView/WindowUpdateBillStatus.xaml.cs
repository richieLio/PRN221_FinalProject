using BusinessObject.Object;
using DataAccess.Model.BillModel;
using DataAccess.Repository.CombineRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF.Views.BillView
{
    /// <summary>
    /// Interaction logic for WindowUpdateBillStatus.xaml
    /// </summary>
    public partial class WindowUpdateBillStatus : Window
    {
        private readonly ICombineRepository _repository;
        private readonly Bill _bill;
        public WindowUpdateBillStatus(ICombineRepository repository, Bill bill)
        {
            InitializeComponent();
            _repository = repository;
            _bill = bill;
            InitializeBillData();
        }

        private void InitializeBillData()
        {
            if (_bill.IsPaid)
            {
                StatusComboBox.SelectedIndex = 0; 
            }
            else
            {
                StatusComboBox.SelectedIndex = 1; 
            }

            // Set the payment date
            PaymentDatePicker.SelectedDate = _bill.PaymentDate;
        }

        private async void UpdateStatusButton_Click(object sender, RoutedEventArgs e)
        {
            string statusText = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            bool isPaid = statusText == "Paid";
            DateTime? paymentDate = PaymentDatePicker.SelectedDate;

            // If "Unpaid" is selected, set paymentDate to null
            if (statusText == "Unpaid")
            {
                paymentDate = null;
            }

            var billUpdateStatusReqModel = new BillUpdateStatusReqModel
            {
                BillId = _bill.Id,
                Status = isPaid,
                PaymentDay = paymentDate
            };

            var result = await _repository.UpdateBillStatus(App.LoggedInUserId, billUpdateStatusReqModel);

            MessageBox.Show(result.Message);
            Close();
        }

    }
}
