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
        private readonly SolidColorBrush[] seriesColors = { Brushes.Blue, Brushes.Orange, Brushes.Green };


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
                await UpdateChartData();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving houses: {ex.Message}");
            }
        }


        private async Task UpdateChartData()
        {
            if (_houses == null || !_houses.Any())
                return;

            // Lấy giá trị start date và end date từ date picker
            DateTime startDate = DateTime.Now.AddYears(-1);
            DateTime endDate = DateTime.Now;

            // Thực hiện truy vấn cơ sở dữ liệu để lấy dữ liệu từng tháng của từng nhà
            var monthlyData = await _repository.GetMonthlyRevenueByHouse(startDate, endDate);

            // Cập nhật giao diện người dùng với dữ liệu mới
            await UpdateChart(monthlyData);
        }


        private async Task UpdateChart(Dictionary<House, List<(DateTime PaymentDate, decimal Revenue)>> monthlyData)
        {
            // Xóa các series hiện có trên biểu đồ
            ReportChart.Series.Clear();
            HousePanel.Children.Clear();
            ColorPanel.Children.Clear();

            // Tạo một series mới cho mỗi nhà và thêm dữ liệu từ monthlyData
            for (int i = 0; i < monthlyData.Count; i++)
            {
                var houseData = monthlyData.ElementAt(i);
                var house = houseData.Key;
                var monthlyRevenue = houseData.Value;

                var series = new LineSeries
                {
                    Title = house.Name,
                    Values = new ChartValues<decimal>(),
                    PointGeometrySize = 5,
                    StrokeThickness = 5,
                    Stroke = seriesColors[i],
                    LabelPoint = (point) =>
                    {
                        var index = (int)point.X;
                        if (index >= 0 && index < monthlyRevenue.Count)
                        {
                            var monthName = monthlyRevenue[index].PaymentDate.ToString("MMMM");
                            return $"{monthName}";
                        }
                        return "";
                    }
                };

                // Thêm series vào biểu đồ
                ReportChart.Series.Add(series);

                // Thêm dữ liệu vào Values của series
                foreach (var dataPoint in monthlyRevenue)
                {
                    series.Values.Add(dataPoint.Revenue);
                }

                // Tạo Border với cùng màu của LineSeries
                var colorBorder = new Border
                {
                    Width = 12,
                    Height = 12,
                    Background = seriesColors[i],
                    CornerRadius = new CornerRadius(3),
                    Margin = new Thickness(0, 3, 0, 0)
                };

                // Thêm Border và TextBlock vào ColorPanel và HousePanel
                ColorPanel.Children.Add(colorBorder);
                HousePanel.Children.Add(new TextBlock { Text = house.Name });
            }
        }
    }
}
