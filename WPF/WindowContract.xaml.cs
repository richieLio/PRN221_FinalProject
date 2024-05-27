using Microsoft.AspNetCore.SignalR.Client;
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

namespace WPF
{
    /// <summary>
    /// Interaction logic for WindowContract.xaml
    /// </summary>
    public partial class WindowContract : UserControl
    {
        HubConnection connection;
        public WindowContract()
        {
            InitializeComponent();
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7259/notihub")
                .WithAutomaticReconnect()
                .Build();

            
            openConnect();
        }
        private async void openConnect()
        {
            connection.On<string>("ReceiveNotification", ( message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{message}";
                    messages.Items.Add(newMessage);
                });
            });
            try
            {
                await connection.StartAsync();
                messages.Items.Add("Connection stated");
               
            }
            catch (Exception ex)
            {
                messages.Items.Add(ex.Message);

            }
        }
        private async void openConnection_Click(object sender, RoutedEventArgs e)
        {
            openConnect();
        }

        
    }
}
