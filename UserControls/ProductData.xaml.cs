using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace heritage_rhythm.UserControls
{
    /// <summary>
    /// ProductData.xaml 的交互逻辑
    /// </summary>
    public partial class ProductData : UserControl
    {
        public ObservableCollection<Product> Products { get; set; }
        private SqlConnection connection;
        public ProductData()
        {
            InitializeComponent();
            Products = new ObservableCollection<Product>();
            productsDataGrid.ItemsSource = Products;
            DNO dno = new DNO();
            connection = dno.Connection();
            LoadProductsFromDatabase();
        }

        public class Product
        {
            public string ProductId { get; set; }
            public string ProductName { get; set; }
            public string StoreName { get; set; } // If you want to display the merchant's name along with the product
            public string ProductCategory { get; set; }
            public decimal Price { get; set; }
            public int StockQuantity { get; set; }
            public string ProductDetails { get; set; }
        }

        private void LoadProductsFromDatabase()
        {
            try
            {   
                string merchantId=Properties.Settings.Default.MerchantId;
                connection.Open();
                string query = @"SELECT p.product_id, p.name, m.storename, p.category, p.price, p.stock_quantity, p.details
                             FROM products p
                             INNER JOIN merchants m ON p.store_id = m.merchantid
                             WHERE m.merchantid = @MerchantId"; // Assuming you want to filter by merchant

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MerchantId", merchantId);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Products.Add(new Product
                    {
                        ProductId = reader["product_id"].ToString(),
                        ProductName = reader["name"].ToString(),
                        StoreName = reader["storename"].ToString(),
                        ProductCategory = reader["category"].ToString(),
                        Price = Convert.ToDecimal(reader["price"]),
                        StockQuantity = Convert.ToInt32(reader["stock_quantity"]),
                        ProductDetails = reader["details"].ToString(),
                    });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            Button detailsButton = sender as Button;
            Product product = detailsButton.DataContext as Product;

            if (product != null)
            {
                PurchaseDetails detailsPage = new PurchaseDetails(product.ProductId);

                // 寻找Frame
                Frame frame = FindParent<Frame>(this);
                if (frame != null)
                {
                    frame.Navigate(detailsPage);
                }
            }
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            if (parent == null) return null;

            T parentT = parent as T;
            if (parentT != null)
            {
                return parentT;
            }
            else
            {
                return FindParent<T>(parent);
            }
        }


        private void editButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Button editButton = sender as Button;
            Product product = editButton.DataContext as Product;

            if (product != null)
            {
                edit editPage = new edit(product.ProductId,Properties.Settings.Default.MerchantId);

                // 寻找Frame
                Frame frame = FindParent<Frame>(this);
                if (frame != null)
                {
                    frame.Navigate(editPage);
                }
            }
        }
    }
}
