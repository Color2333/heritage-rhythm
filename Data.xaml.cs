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


namespace heritage_rhythm
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class Data : Page
    {
        private ObservableCollection<Product> products;
        private SqlConnection connection;

        public Data()
        {
            InitializeComponent();
            DNO dno = new DNO();
            connection = dno.Connection();
            LoadProductsFromDatabase();
        }

        private void LoadProductsFromDatabase()
        {
            products = new ObservableCollection<Product>();

            // 连接字符串，替换为你自己的连接字符串

            try
            {
                using (connection)
                {
                    connection.Open();
                    string query = "SELECT * FROM products";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Product product = new Product
                        {
                            ProductId = reader["product_id"].ToString(),
                            Name = reader["name"].ToString(),
                            Details = Convert.IsDBNull(reader["details"]) ? string.Empty : reader["details"].ToString(),
                            StoreId = reader["store_id"].ToString(),
                            Category = reader["category"].ToString(),
                            Price = Convert.IsDBNull(reader["price"]) ? 0m : Convert.ToDecimal(reader["price"]),
                            StockQuantity = Convert.IsDBNull(reader["stock_quantity"]) ? 0 : Convert.ToInt32(reader["stock_quantity"]),
                            Status = Convert.IsDBNull(reader["status"]) ? string.Empty : reader["status"].ToString(),
                            CreationTime = Convert.IsDBNull(reader["creation_time"]) ? DateTime.MinValue : Convert.ToDateTime(reader["creation_time"]),
                            UpdateTime = Convert.IsDBNull(reader["update_time"]) ? DateTime.MinValue : Convert.ToDateTime(reader["update_time"])
                        };
                        products.Add(product);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载商品信息时出现错误：" + ex.Message);
            }

            productsDataGrid.ItemsSource = products;
        }


        public class Product
        {
            public string ProductId { get; set; }
            public string Name { get; set; }
            public string Details { get; set; }
            public string StoreId { get; set; }
            public string Category { get; set; }
            public decimal Price { get; set; }
            public int StockQuantity { get; set; }
            public string Status { get; set; }
            public DateTime CreationTime { get; set; }
            public DateTime UpdateTime { get; set; }
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取用户输入的搜索关键字
            string keyword = textBoxSearch.Text;

            products = new ObservableCollection<Product>();

            try
            {
                using (connection)
                {
                    connection.Open();
                    // 在查询中使用搜索关键字
                    string query = "SELECT * FROM products WHERE name LIKE @Keyword OR details LIKE @Keyword OR category LIKE @Keyword OR status LIKE @Keyword";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Product product = new Product
                        {
                            ProductId = reader["product_id"].ToString(),
                            Name = reader["name"].ToString(),
                            Details = Convert.IsDBNull(reader["details"]) ? string.Empty : reader["details"].ToString(),
                            StoreId = reader["store_id"].ToString(),
                            Category = reader["category"].ToString(),
                            Price = Convert.IsDBNull(reader["price"]) ? 0m : Convert.ToDecimal(reader["price"]),
                            StockQuantity = Convert.IsDBNull(reader["stock_quantity"]) ? 0 : Convert.ToInt32(reader["stock_quantity"]),
                            Status = Convert.IsDBNull(reader["status"]) ? string.Empty : reader["status"].ToString(),
                            CreationTime = Convert.IsDBNull(reader["creation_time"]) ? DateTime.MinValue : Convert.ToDateTime(reader["creation_time"]),
                            UpdateTime = Convert.IsDBNull(reader["update_time"]) ? DateTime.MinValue : Convert.ToDateTime(reader["update_time"])
                        };
                        products.Add(product);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("搜索商品信息时出现错误：" + ex.Message);
            }

            productsDataGrid.ItemsSource = products;
        }


    }
}
