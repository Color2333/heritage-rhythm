using heritage_rhythm.UserControls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static heritage_rhythm.PurchaseDetails;

namespace heritage_rhythm
{
    /// <summary>
    /// ShopingCar.xaml 的交互逻辑
    /// </summary>
    public partial class ShopingCar : Page
    {
        private SqlConnection connection;
        public ShopingCar()
        {
            DNO dno = new DNO();
            connection = dno.Connection();
            InitializeComponent();
            LoadCartItems();
        }
        // 这个方法假设你已经有了一个代表用户购物车的数据结构
        private void LoadCartItems()
        {
            // 假设您已经建立了数据库连接
                try
                {
                connection.Open();
                string query = @"
    SELECT 
        p.product_id, 
        p.name, 
        m.storename, 
        p.category, 
        p.price, 
        p.stock_quantity, 
        p.details,
        (SELECT TOP 1 image_data FROM product_images pi WHERE pi.product_id = p.product_id ORDER BY pi.image_id) AS image_data
    FROM products p
    INNER JOIN merchants m ON p.store_id = m.merchantid
    INNER JOIN shopping_cart_items sci ON p.product_id = sci.productid
    INNER JOIN shopping_cart sc ON sci.cartid = sc.cartid
    WHERE sc.userid = @UserId";


                using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", "10000001"); // 用实际的用户ID替换

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            var itemControl = new Item
                            {
                                ProductId=reader["product_id"].ToString(),
                                ProductName = reader["name"].ToString(),
                                Price = reader["price"].ToString(),
                                StoreName = reader["storename"].ToString(),
                                Category = reader["category"].ToString(),
                                Count = Convert.ToInt32(reader["stock_quantity"]),
                                IsSelected = false
                                };

                            byte[] imageData = (byte[])reader["image_data"];
                            if (imageData != null)
                            {
                                using (var ms = new MemoryStream(imageData))
                                {
                                    BitmapImage bitmap = new BitmapImage();
                                    bitmap.BeginInit();
                                    bitmap.CacheOption = BitmapCacheOption.OnLoad; // This helps to load the image synchronously
                                    bitmap.StreamSource = ms;
                                    bitmap.EndInit();
                                    bitmap.Freeze(); // This makes the image usable across threads
                                    itemControl.Source = bitmap;                 // Now you can assign the bitmap to an Image control or use it wherever needed
                                }
                            }



                            // 将Item控件添加到购物车的StackPanel中
                            CartItemsStackPanel.Children.Add(itemControl);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 处理任何异常，比如显示错误消息
                    MessageBox.Show("无法加载购物车信息: " + ex.Message);
                }
            
        }
        private List<SelectedProduct> GetSelectedProducts()
        {
            List<SelectedProduct> selectedProducts = new List<SelectedProduct>();
            foreach (Item item in CartItemsStackPanel.Children)
            {
                if (item.IsSelected)
                {
                    SelectedProduct product = new SelectedProduct
                    {
                        Id = item.ProductId, // Assuming there's a ProductId property in Item
                        Name = item.ProductName,
                        Price = item.Price,
                        Category = item.Category,
                        Quantity = item.Count, // Assuming there's a StockQuantity property in Item
                        StoreName = item.StoreName,
                        Images = new BitmapImage[] { item.Source as BitmapImage } // Assuming item.Source is already a BitmapImage
                    };

                    selectedProducts.Add(product);
                }
            }
            return selectedProducts;
        }
        private void GoToConfirmationPageButton_Click(object sender, RoutedEventArgs e)
        {
            List<SelectedProduct> selectedProducts = GetSelectedProducts();
            var confirmationPage = new ConfirmationPaymentPage();
            confirmationPage.DataContext = selectedProducts;  // 传递选中的商品
            NavigationService.Navigate(confirmationPage);
        }



    }
}
