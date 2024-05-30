using heritage_rhythm.UserControls;
using System;
using System.Collections.Generic;
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
using System.IO;

namespace heritage_rhythm
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {
        string userId = Properties.Settings.Default.UserId;
        private SqlConnection connection;
        public MainPage()
        {
            InitializeComponent();
            DNO dno = new DNO();
            connection = dno.Connection();
            LoadProductsFromDatabase();
        }
        private void textSearch_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            txtSearch.Focus();
        }

        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text) && txtSearch.Text.Length > 0)
                textSearch.Visibility = Visibility.Collapsed;
            else
                textSearch.Visibility = Visibility.Visible;
        }
        private void txtSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                // 用户按下了回车键，执行搜索
                string searchKeyword = txtSearch.Text.Trim();
                SearchProduct(searchKeyword);
            }
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // 用户点击了搜索按钮，执行搜索
            string searchKeyword = txtSearch.Text.Trim();
            SearchProduct(searchKeyword);
        }
        private BitmapImage GetImageFromByte(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;

            using (MemoryStream stream = new MemoryStream(imageData))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
        }

        private void SearchProduct(string searchKeyword = "")
        {
            try
            {
                wrapPanel.Children.Clear();

                // 连接数据库
                connection.Open();

                // 构建查询语句，根据搜索关键词进行筛选
                string query = @"SELECT p.product_id, 
                       p.name, 
                       m.storename, 
                       p.category, 
                       p.price, 
                       p.stock_quantity, 
                       p.details,
                       (SELECT TOP 1 image_data FROM product_images pi WHERE p.product_id = pi.product_id) AS image_data,
                       CASE WHEN EXISTS (SELECT 1 FROM product_favorites WHERE userid = @UserId AND productid = p.product_id) THEN 1 ELSE 0 END AS IsFavorite
                FROM products p
                INNER JOIN merchants m ON p.store_id = m.merchantid
                WHERE p.name LIKE '%' + @SearchKeyword + '%'";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@SearchKeyword", searchKeyword);
                SqlDataReader reader = command.ExecuteReader();

                // 遍历查询结果
                while (reader.Read())
                {
                    string productId = reader["product_id"].ToString();
                    string productName = reader["name"].ToString();
                    string storeName = reader["storename"].ToString(); // 获取商店名称
                    string productCategory = reader["category"].ToString();
                    decimal price = Convert.ToDecimal(reader["price"]);
                    int stockQuantity = Convert.ToInt32(reader["stock_quantity"]);
                    string productDetails = reader["details"].ToString();
                    bool isFavorite = Convert.ToBoolean(reader["IsFavorite"]); // 获取是否已收藏的信息

                    // 创建 Product 控件
                    Product product = new Product();
                    product.Margin = new Thickness(10);
                    product.ProductId = productId; // 设置 product_id
                    product.ProductTitle = productName;
                    product.SellerName = storeName;
                    product.Price = price;

                    // 获取图片数据
                    byte[] imageData = reader["image_data"] as byte[];
                    if (imageData != null && imageData.Length > 0)
                    {
                        using (MemoryStream stream = new MemoryStream(imageData))
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.StreamSource = stream;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            product.ProductImageSource = bitmap;
                        }
                    }
                    // 添加到 WrapPanel 中
                    wrapPanel.Children.Add(product);
                }

                // 关闭数据阅读器
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载商品信息时出现错误：" + ex.Message);
            }
            finally
            {
                // 确保关闭连接
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void LoadProductsFromDatabase()
        {
            try
            {
                // 连接数据库
                connection.Open();

                // 查询 products 表
                string query = @"SELECT p.product_id, 
                               p.name, 
                               m.storename, 
                               p.category, 
                               p.price, 
                               p.stock_quantity, 
                               p.details,
                               (SELECT TOP 1 image_data FROM product_images pi WHERE p.product_id = pi.product_id) AS image_data,
                               CASE WHEN EXISTS (SELECT 1 FROM product_favorites WHERE userid = @UserId AND productid = p.product_id) THEN 1 ELSE 0 END AS IsFavorite
                        FROM products p
                        INNER JOIN merchants m ON p.store_id = m.merchantid";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);
                SqlDataReader reader = command.ExecuteReader();

                // 遍历查询结果
                while (reader.Read())
                {
                    // 从查询结果中获取商品信息
                    string productId = reader["product_id"].ToString();
                    string productName = reader["name"].ToString();
                    string storeName = reader["storename"].ToString(); // 获取商店名称
                    string productCategory = reader["category"].ToString();
                    decimal price = Convert.ToDecimal(reader["price"]);
                    int stockQuantity = Convert.ToInt32(reader["stock_quantity"]);
                    string productDetails = reader["details"].ToString();
                    bool isFavorite = Convert.ToBoolean(reader["IsFavorite"]); // 获取是否已收藏的信息

                    // 创建 Product 控件
                    Product product = new Product();
                    product.Margin = new Thickness(10);
                    product.ProductId = productId; // 设置 product_id
                    product.ProductTitle = productName;
                    product.SellerName = storeName;
                    product.Price = price;

                    // 获取图片数据
                    byte[] imageData = reader["image_data"] as byte[];
                    if (imageData != null && imageData.Length > 0)
                    {
                        using (MemoryStream stream = new MemoryStream(imageData))
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.StreamSource = stream;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            product.ProductImageSource = bitmap;
                        }
                    }
                    // 添加到 WrapPanel 中
                    wrapPanel.Children.Add(product);
                }

                // 关闭数据阅读器
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载商品信息时出现错误：" + ex.Message);
            }
            finally
            {
                // 确保关闭连接
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        public class RecommendProduct
        {
            public string ProductId { get; set; }
            public string ProductName { get; set; }
            public string StoreName { get; set; }
            public string ProductCategory { get; set; }
            public decimal Price { get; set; }
            public int StockQuantity { get; set; }
            public string ProductDetails { get; set; }
            public bool IsFavorite { get; set; }
            public BitmapImage ProductImage { get; set; } // 图像数据
        }

        private void GenerateRecommendedProducts()
        {
            try
            {
                wrapPanel.Children.Clear();
                connection.Open();
                List<string> userCategories = GetUserPurchaseCategories();
                if (userCategories.Count == 0)
                {
                    MessageBox.Show("No categories found for user purchases.");
                    return;
                }

                List<RecommendProduct> recommendedProducts = GetRecommendedProductsByCategories(userCategories);
                if (recommendedProducts.Count == 0)
                {
                    MessageBox.Show("No recommended products found.");
                    return;
                }

                foreach (RecommendProduct product in recommendedProducts)
                {
                    Product productControl = new Product();
                    productControl.Margin = new Thickness(10);
                    productControl.ProductId = product.ProductId;
                    productControl.ProductTitle = product.ProductName;
                    productControl.SellerName = product.StoreName;
                    productControl.Price = product.Price;
                    productControl.ProductImageSource = product.ProductImage;

                    wrapPanel.Children.Add(productControl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("生成推荐商品时出现错误：" + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }



        private List<string> GetUserPurchaseCategories()
        {
            List<string> userCategories = new List<string>();
            string query = @"SELECT DISTINCT p.category FROM Orders o INNER JOIN products p ON o.productid = p.product_id WHERE o.userid = @UserId";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                userCategories.Add(reader["category"].ToString());
            }
            reader.Close();

            return userCategories;
        }

        private List<RecommendProduct> GetRecommendedProductsByCategories(List<string> userCategories)
        {
            List<RecommendProduct> recommendedProducts = new List<RecommendProduct>();
            foreach (string category in userCategories)
            {
                string query = @"SELECT TOP 3 p.product_id, p.name, m.storename, p.category, p.price, p.stock_quantity, p.details,
                         (SELECT TOP 1 image_data FROM product_images pi WHERE pi.product_id = p.product_id) AS image_data,
                         (CASE WHEN EXISTS (SELECT 1 FROM product_favorites WHERE userid = @UserId AND productid = p.product_id) THEN 1 ELSE 0 END) AS IsFavorite
                         FROM products p
                         INNER JOIN merchants m ON p.store_id = m.merchantid
                         WHERE p.category = @Category ORDER BY NEWID()";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Category", category);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    RecommendProduct product = new RecommendProduct
                    {
                        ProductId = reader["product_id"].ToString(),
                        ProductName = reader["name"].ToString(),
                        StoreName = reader["storename"].ToString(),
                        ProductCategory = reader["category"].ToString(),
                        Price = Convert.ToDecimal(reader["price"]),
                        StockQuantity = Convert.ToInt32(reader["stock_quantity"]),
                        ProductDetails = reader["details"].ToString(),
                        IsFavorite = Convert.ToBoolean(reader["IsFavorite"]),
                        ProductImage = GetImageFromByte(reader["image_data"] as byte[])
                    };
                    recommendedProducts.Add(product);
                }
                reader.Close();
            }

            return recommendedProducts;
        }



        private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            wrapPanel.Children.Clear();
            GenerateRecommendedProducts();
            likelabel.Style = (Style)FindResource("activeTextButton");
            popularlabel.Style = (Style)FindResource("textButton");
        }

        private void Likelabel_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            wrapPanel.Children.Clear();
            LoadProductsFromDatabase();
            likelabel.Style = (Style)FindResource("textButton");
            popularlabel.Style = (Style)FindResource("activeTextButton");
        }
    }

}
