using BusinessObject.Object;
using DataAccess.Helper;
using DataAccess.Model.ContractModel;
using DataAccess.Model.CustomerModel;
using DataAccess.Repository.CombineRepository;
using DataAccess.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Win32;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WPF.Views.ContractView
{
    /// <summary>
    /// Interaction logic for WindowContractDetails.xaml
    /// </summary>
    public partial class WindowContractDetails : Window
    {
        private readonly CloudStorage _cloudStorage;

        public event EventHandler ContractUpdated;
        private readonly ICombineRepository _repository;
        private readonly ContractInfoResModel _contract;

        public WindowContractDetails(ICombineRepository repository, ContractInfoResModel contract, CloudStorage cloudStorage)
        {
            InitializeComponent();
            _repository = repository;
            _contract = contract;
            DataContext = contract;
            Loaded += Window_Loaded;
            _cloudStorage = cloudStorage;
        }

        private async void btnUpdateContract_Click(object sender, RoutedEventArgs e)
        {
            var contract = new ContractUpdateModel
            {
                Id = _contract.Id,
                FileUrl = _contract.FileUrl,
                EndDate = EndDatePicker.SelectedDate ?? DateTime.MinValue,
            };
            var validationResults = ValidationHelper.ValidateModel(contract);
            if (validationResults.Count > 0)
            {
                // Handle validation errors
                foreach (var validationResult in validationResults)
                {
                    MessageBox.Show(validationResult.ErrorMessage, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                }
                return;
            }
            await _repository.UpdateContract(contract);
            MessageBox.Show($"Contract updated successfully");
            ContractUpdated?.Invoke(this, EventArgs.Empty);
            Close();
        }

        private IFormFile ConvertToIFormFile(string filePath)
        {
            var fileStream = new FileStream(filePath, FileMode.Open);
            return new FormFile(fileStream, 0, fileStream.Length, null, Path.GetFileName(filePath))
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream"
            };
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void DownloadFile_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_contract.FileUrl))
            {
                string fileName = Path.GetFileName(_contract.FileUrl);

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = fileName;
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = $"Contract/{_contract.Id}/{fileName}";

                    byte[] fileBytes = await _cloudStorage.DownloadFileFromFirebase(filePath, fileName);

                    File.WriteAllBytes(saveFileDialog.FileName, fileBytes);

                    MessageBox.Show($"File downloaded successfully");
                }
            }
            else
            {
                MessageBox.Show("No file available to download");
            }
        }

        private async void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {

                var form = new ContractReqModel
                {
                    Id = _contract.Id,
                    ImagesUrl = ConvertToIFormFile(openFileDialog.FileName),
                };

                string filePath = $"Contract/{_contract.Id}/{form.ImagesUrl.FileName}";
                var result = await _cloudStorage.UploadFile(form.ImagesUrl, filePath);
               
                if (result != null)
                {
                    var contractupdate = new ContractUpdateModel
                    {
                        Id = _contract.Id,
                        FileUrl = form.ImagesUrl.FileName,
                        EndDate = _contract.EndDate
                    };
                    var validationResults = ValidationHelper.ValidateModel(contractupdate);
                    if (validationResults.Count > 0)
                    {
                        // Handle validation errors
                        foreach (var validationResult in validationResults)
                        {
                            MessageBox.Show(validationResult.ErrorMessage, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                        }
                        return;
                    }

                    await _repository.UpdateContract(contractupdate);
                    MessageBox.Show($"File uploaded succesfully");

                    // Update UI
                    txtNoFile.Visibility = Visibility.Collapsed;
                    txtFileLink.Visibility = Visibility.Visible;
                    btnUpload.Visibility = Visibility.Collapsed;
                    btnDelete.Visibility = Visibility.Visible;
                    fileName.Text = form.ImagesUrl.FileName;

                }
            }
            
        }
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_contract.FileUrl))
            {
                if (MessageBox.Show("Are you sure you want to delete this file?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    string filePath = $"Contract/{_contract.Id}/{_contract.FileUrl}";

                    await _cloudStorage.DeleteFile(filePath, _contract.FileUrl); 

                    MessageBox.Show("File deleted successfully");

                    var contract = new ContractUpdateModel
                    {
                        Id = _contract.Id,
                        FileUrl = null,
                        EndDate = _contract.EndDate

                    };
                    await _repository.UpdateContract(contract);

                    txtNoFile.Visibility = Visibility.Visible;
                    btnUpload.Visibility = Visibility.Visible;
                    txtFileLink.Visibility = Visibility.Collapsed;
                    btnDelete.Visibility = Visibility.Collapsed;
                    fileName.Text = string.Empty;

                    _contract.FileUrl = null;
                }
            }
            else
            {
                MessageBox.Show("No file available to delete");
            }
        }




        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_contract.FileUrl))
            {
                txtNoFile.Visibility = Visibility.Visible;
                btnUpload.Visibility = Visibility.Visible;
                txtFileLink.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtNoFile.Visibility = Visibility.Collapsed;
                txtFileLink.Visibility = Visibility.Visible;
                btnUpload.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Visible;
                fileName.Text = _contract.FileUrl;
            }
        }
    }
}
