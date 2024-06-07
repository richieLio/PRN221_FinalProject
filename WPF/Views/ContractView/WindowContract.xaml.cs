using DataAccess.Model.BillModel;
using DataAccess.Model.ContractModel;
using DataAccess.Repository;
using DataAccess.Repository.CombineRepository;
using DataAccess.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using WPF.Views.BillView;
using WPF.Views.ContractView;

namespace WPF.ContractView
{
    /// <summary>
    /// Interaction logic for WindowContract.xaml
    /// </summary>
    public partial class WindowContract : UserControl
    {
        private readonly CloudStorage _cloudStorage;
        private readonly ICombineRepository _repository;


        public WindowContract(CloudStorage cloudStorage, ICombineRepository repository)
        {
            InitializeComponent();
            _cloudStorage = cloudStorage;
            _repository = repository;
            LoadContracts(App.LoggedInUserId);
        }

        /*  private async void Button_Click(object sender, RoutedEventArgs e)
          {
              OpenFileDialog openFileDialog = new OpenFileDialog();
              if (openFileDialog.ShowDialog() == true)
              {

              var form = new ContractReqModel
              {
                  Id = Guid.NewGuid(),
                  ImagesUrl = ConvertToIFormFile(openFileDialog.FileName),
              };

              string filePath = $"Contract/{form.Id}/images/{form.ImagesUrl.FileName}";
             var result =  await _cloudStorage.UploadFile(form.ImagesUrl, filePath);
                  if(result != null)
                  {
                      MessageBox.Show($"File uploaded succesfully");
                  }
              }
          private IFormFile ConvertToIFormFile(string filePath)
        {
            var fileStream = new FileStream(filePath, FileMode.Open);
            return new FormFile(fileStream, 0, fileStream.Length, null, System.IO.Path.GetFileName(filePath))
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream"
            };
        }
          }*/
        private async void LoadContracts(Guid userId)
        {
            var result = await _repository.GetContractList(userId);
            if (result.IsSuccess)
            {
                lvContracts.ItemsSource = result.Data as List<ContractInfoResModel>;
            }
            else
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
      
       

       
        private async void ViewContractDetails_Click(object sender, RoutedEventArgs e)
        {
       
            if (lvContracts.SelectedItem is ContractInfoResModel selectedContract)
            {
                 var detailsWindow = new WindowContractDetails(_repository, selectedContract, _cloudStorage);
                detailsWindow.ShowDialog();
            }
        }   
        private void EditContract_Click(object sender, RoutedEventArgs e)
        {
            // Implement edit bill logic here
            MessageBox.Show("Edit contract.");
        }

     
        private void DeleteContract_Click(object sender, RoutedEventArgs e)
        {
            // Implement delete bill logic here
            MessageBox.Show("Delete contract.");
        }
    }
}
