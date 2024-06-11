using BusinessObject.Object;
using DataAccess.Repository.CombineRepository;
using LiveCharts.Wpf.Charts.Base;
using LiveCharts.Wpf;
using LiveCharts;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualBasic.ApplicationServices;
using DataAccess.Model.HouseModel;
using System.Globalization;

namespace WPF.Views.ReportView
{
    public partial class WindowReport : UserControl
    {
        private readonly ICombineRepository _repository;
        private IEnumerable<House> _houses;

        public Func<double, string> Formatter { get; set; }

        public WindowReport(ICombineRepository repository)
        {
            InitializeComponent();
            _repository = repository;
            Formatter = value => value.ToString("N0", CultureInfo.InvariantCulture); // Định dạng giá trị lớn thành định dạng ngắn gọn
            DataContext = this; // Đặt DataContext để Binding hoạt động
            LoadHouses(App.LoggedInUserId);
        }

        private async void LoadHouses(Guid userId)
        {
            try
            {
                _houses = await _repository.GetHouses(userId);
                UpdateUI();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving houses: {ex.Message}");
            }
        }

        private async Task UpdateUI()
        {
            ColorPanel.Children.Clear();
            HousePanel.Children.Clear();
            ReportChart.Series.Clear();

            foreach (var house in _houses)
            {
                var colorBorder = new Border
                {
                    Width = 12,
                    Height = 12,
                    Background = GetColorFromChart("Blue"),
                    CornerRadius = new CornerRadius(3),
                    Margin = new Thickness(0, 3, 0, 0)
                };
                var textBlock = new TextBlock
                {
                    Text = house.Name,
                };

                ColorPanel.Children.Add(colorBorder);
                HousePanel.Children.Add(textBlock);
            }
        }

        private async void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            await UpdateChartData();
        }

        private async Task UpdateChartData()
        {
            if (_houses == null || !_houses.Any())
                return;

            // Lấy giá trị start date và end date từ date picker
            DateTime startDate = StartDatePicker.SelectedDate ?? DateTime.MinValue;
            DateTime endDate = EndDatePicker.SelectedDate ?? DateTime.MaxValue;

            // Thực hiện truy vấn cơ sở dữ liệu để lấy dữ liệu từng tháng của từng nhà
            var monthlyData = await _repository.GetMonthlyRevenueByHouse(startDate, endDate);

            // Cập nhật giao diện người dùng với dữ liệu mới
            await UpdateChart(monthlyData);
        }

        private async Task UpdateChart(Dictionary<House, List<decimal>> monthlyData)
        {
            // Xóa các series hiện có trên biểu đồ
            ReportChart.Series.Clear();

            // Tạo một series mới cho mỗi nhà và thêm dữ liệu từ monthlyData
            foreach (var houseData in monthlyData)
            {
                var house = houseData.Key;
                var monthlyRevenue = houseData.Value;

                var series = new LineSeries
                {
                    Title = house.Name,
                    Values = new ChartValues<decimal>(),
                    LineSmoothness = 0
                };

                // Thêm series vào biểu đồ
                ReportChart.Series.Add(series);

                // Thêm dữ liệu vào Values của series
                foreach (var revenue in monthlyRevenue)
                {
                    series.Values.Add(revenue);
                }
            }
        }

        private SolidColorBrush GetColorFromChart(string colorName)
        {
            if (colorName == "Blue")
                return Brushes.Blue;
            else if (colorName == "Red")
                return Brushes.Red;
            else if (colorName == "Green")
                return Brushes.Green;

            return Brushes.Black;
        }
    }
}
