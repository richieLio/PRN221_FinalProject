using DataAccess.Model.UserModel;
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

namespace WPF.Views.UserView
{
    /// <summary>
    /// Interaction logic for WindowChangePassword.xaml
    /// </summary>
    public partial class WindowChangePassword : Window
    {
        private readonly ICombineRepository _repository;
        public WindowChangePassword(ICombineRepository repository)
        {
            InitializeComponent();
            _repository = repository;
        }

        private async void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {

            if (txtPassword.Password != txtConfirmPass.Password)
            {
                MessageBox.Show("New password does not match", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            var changePasswordModel =  new ChangePasswordReqModel
            {
                OldPassword = txtOldPassword.Password,
                NewPassword = txtPassword.Password,

            };
            var result = await _repository.ChangePassword(App.LoggedInUserId, changePasswordModel);
            if (result.IsSuccess)
            {
                MessageBox.Show($"{result.Message}");
                Close();
            }else
            {
                MessageBox.Show($"{result.Message}");

            }
        }
    }
}
