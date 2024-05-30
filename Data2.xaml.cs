using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data;
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
using System.Data.Common;

namespace heritage_rhythm
{
    /// <summary>
    /// Data2.xaml 的交互逻辑
    /// </summary>
    public partial class Data2 : Page
    {
        private SqlConnection connection;
        public Data2()
        {

            InitializeComponent();
            DNO dno = new DNO();
            connection = dno.Connection();
            LoadOrders();
        }
        public class Order
        {
            public int OrderId { get; set; }
            public int UserId { get; set; }
            public string ProductName { get; set; }
            public string UserNick { get; set; }
            public int Quantity { get; set; }
            public DateTime OrderTime { get; set; }
        }

        private void LoadOrders()
        {
            var orders = new ObservableCollection<Order>();
            try
            {
                connection.Open();
                string query = @"SELECT 
                            o.OrderId, 
                            u.usernick AS UserNick, 
                            p.name AS ProductName, 
                            o.PurchaseQuantity, 
                            o.OrderTime
                        FROM 
                            orders o
                            INNER JOIN users u ON o.UserId = u.userid
                            INNER JOIN products p ON o.ProductId = p.product_id;";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderId = Convert.ToInt32(reader["OrderId"]),
                        UserNick = reader["UserNick"].ToString(),
                        ProductName = reader["ProductName"].ToString(),
                        Quantity = Convert.ToInt32(reader["PurchaseQuantity"]),
                        OrderTime = Convert.ToDateTime(reader["OrderTime"])
                    });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载订单信息时出现错误：" + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            ordersDataGrid.ItemsSource = orders; // 绑定到 DataGrid
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取用户输入的搜索关键字
            string keyword = textBoxSearch.Text;

            var orders = new ObservableCollection<Order>();

            try
            {
                connection.Open();
                // 在查询中使用搜索关键字
                string query = @"SELECT 
                            o.OrderId, 
                            u.usernick AS UserNick, 
                            p.name AS ProductName, 
                            o.PurchaseQuantity, 
                            o.OrderTime
                        FROM 
                            orders o
                            INNER JOIN users u ON o.UserId = u.userid
                            INNER JOIN products p ON o.ProductId = p.product_id
                        WHERE 
                            u.usernick LIKE @Keyword OR p.name LIKE @Keyword";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderId = Convert.ToInt32(reader["OrderId"]),
                        UserNick = reader["UserNick"].ToString(),
                        ProductName = reader["ProductName"].ToString(),
                        Quantity = Convert.ToInt32(reader["PurchaseQuantity"]),
                        OrderTime = Convert.ToDateTime(reader["OrderTime"])
                    });
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("搜索订单信息时出现错误：" + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            ordersDataGrid.ItemsSource = orders; // 绑定到 DataGrid
        }




    }
}
