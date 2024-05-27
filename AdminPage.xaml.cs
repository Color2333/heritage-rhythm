using LiveCharts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.ComponentModel;

namespace heritage_rhythm
{
    /// <summary>
    /// AdminPage.xaml 的交互逻辑
    /// </summary>
    public partial class AdminPage : Page
    {
        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> Formatter { get; set; }
        private SqlConnection connection;

        public AdminPage()
        {
            InitializeComponent();
            DNO dno = new DNO();
            connection = dno.Connection();
            DataContext = this;
            Formatter = value => value.ToString("N0");
            LoadSalesData1();
            LoadChartData1();
        }

        private string merchantId;
        private void LoadSalesData()
        {
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Daily Sales",
                    Values = new ChartValues<ObservablePoint>(),
                    Fill = Brushes.Transparent,
                    StrokeThickness = 3,
                    PointGeometrySize = 0,
                    Stroke = new LinearGradientBrush
                    {
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop(Colors.White, 0.1),
                            new GradientStop(Color.FromRgb(40, 137, 252), 0.5),
                            new GradientStop(Colors.White, 0.9)
                        }
                    }
                }
            };
            merchantId = Properties.Settings.Default.MerchantId;
            using (connection)
            {
                string query = @"
                SELECT CAST(OrderTime AS DATE) AS OrderDate, COUNT(*) AS TotalOrders
                FROM Orders
                WHERE merchantid = @MerchantId AND OrderTime >= DATEADD(day, -30, GETDATE())
                GROUP BY CAST(OrderTime AS DATE)
                ORDER BY OrderDate;";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MerchantId", merchantId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    DateTime orderDate = reader.GetDateTime(0);
                    int totalOrders = reader.GetInt32(1);
                    SeriesCollection[0].Values.Add(new ObservablePoint(orderDate.ToOADate(), totalOrders));
                }
            }
        }
        public class ChartData
        {
            public int Sales { get; set; }
            public int Favorites { get; set; }
            public int CartItems { get; set; }
            public string ProductName { get; set; }
        }
        public ChartValues<int> SalesValues { get; set; } = new ChartValues<int>();
        public ChartValues<int> FavoritesValues { get; set; } = new ChartValues<int>();
        public ChartValues<int> CartItemsValues { get; set; } = new ChartValues<int>();
        public List<string> ProductNames { get; set; } = new List<string>();

        private void LoadChartData()
        {
            try
            {
                connection.Open();

                string sql = @"
                SELECT TOP 4 
                    p.Name,
                    COUNT(f.productid) AS FavoritesCount,
                    COUNT(c.productid) AS CartCount
                FROM products p
                LEFT JOIN product_favorites f ON p.product_id = f.productid
                LEFT JOIN shopping_cart_items c ON p.product_id = c.productid
                GROUP BY p.Name
                ORDER BY COUNT(f.productid) DESC, COUNT(c.productid) DESC";

                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataReader reader = command.ExecuteReader();

                var salesValues = new ChartValues<int>();
                var favoritesValues = new ChartValues<int>();
                var cartItemsValues = new ChartValues<int>();
                var productNames = new List<string>();

                while (reader.Read())
                {
                    productNames.Add(reader["Name"].ToString());
                    favoritesValues.Add(Convert.ToInt32(reader["FavoritesCount"]));
                    cartItemsValues.Add(Convert.ToInt32(reader["CartCount"]));
                }

                MainChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "收藏数",
                    Values = favoritesValues
                },
                new ColumnSeries
                {
                    Title = "购物车数",
                    Values = cartItemsValues
                }
            };

                XAxis.Labels = productNames;
            }
            catch (Exception ex)
            {
                MessageBox.Show("无法加载数据: " + ex.Message);
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
        }

       
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadSalesData1()
        {
            // 生成假数据
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Daily Sales",
                    Values = new ChartValues<ObservablePoint>(),
                    Fill = Brushes.Transparent,
                    StrokeThickness = 3,
                    PointGeometrySize = 0,
                    Stroke = new LinearGradientBrush
                    {
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop(Colors.White, 0.1),
                            new GradientStop(Color.FromRgb(40, 137, 252), 0.5),
                            new GradientStop(Colors.White, 0.9)
                        }
                    }
                }
            };

            Random rnd = new Random();
            for (int i = 0; i < 30; i++)
            {
                double sales = rnd.Next(100, 500);
                DateTime orderDate = DateTime.Today.AddDays(-i);
                SeriesCollection[0].Values.Add(new ObservablePoint(orderDate.ToOADate(), sales));
            }

            // 通知界面更新
            OnPropertyChanged("SeriesCollection");
        }
        private void LoadChartData1()
        {
            var favoritesValues = new ChartValues<int> { 10, 20, 30, 40 };
            var cartItemsValues = new ChartValues<int> { 5, 15, 25, 35 };
            var productNames = new List<string> { "Product A", "Product B", "Product C", "Product D" };

            MainChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "收藏数",
                    Values = favoritesValues
                },
                new ColumnSeries
                {
                    Title = "购物车数",
                    Values = cartItemsValues
                }
            };

            XAxis.Labels = productNames;
        }

        private void MainChart_OnLoaded(object sender, RoutedEventArgs e)
        {

        }
    }


}

