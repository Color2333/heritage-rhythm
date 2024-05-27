using heritage_rhythm.UserControls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace heritage_rhythm
{
    /// <summary>
    /// PurchaseDetails.xaml 的交互逻辑
    /// </summary>
    public partial class edit : Page
    {
        private string merchantId;
        private string productId;
        private SqlConnection connection;

        public edit(string productId,string merchantId)
        {
            InitializeComponent();
            this.merchantId = merchantId;
            this.productId = productId;
            DNO dno = new DNO();
            connection = dno.Connection();
            LoadProductDetails();
        }

        public class SelectedProduct
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Price { get; set; }
            public string Category { get; set; }
            public string StockQuantity { get; set; }
            public string Details { get; set; }
            public int Quantity { get; set; }
            public string StoreName { get; set; }
            public BitmapImage[] Images { get; set; }
        }





        private void ButtonDecrease_Click(object sender, RoutedEventArgs e)
        {
            int quantity = int.Parse(TextBox4.Text);
            if (quantity > 1) // Assuming the quantity cannot go below 1
            {
                quantity--;
                TextBox4.Text = quantity.ToString();
            }
        }

        private void ButtonIncrease_Click(object sender, RoutedEventArgs e)
        {
            int quantity = int.Parse(TextBox4.Text);
            quantity++;
            TextBox4.Text = quantity.ToString();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void UploadImagesToDatabase(List<string> filePaths, string productId, SqlConnection connection, SqlTransaction transaction)
        {
            foreach (string filePath in filePaths)
            {
                byte[] imageData = File.ReadAllBytes(filePath);
                string insertImageQuery = "INSERT INTO product_images (product_id, image_data) VALUES (@productId, @imageData)";
                using (SqlCommand cmd = new SqlCommand(insertImageQuery, connection, transaction))
                {
                    cmd.Parameters.Add("@productId", SqlDbType.Int).Value = productId;
                    cmd.Parameters.Add("@imageData", SqlDbType.VarBinary).Value = imageData;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private List<string> filePaths = new List<string>();
        private void imgbtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择图片";
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                List<BitmapImage> images = new List<BitmapImage>();
                filePaths.Clear(); // 清空之前的路径
                foreach (string file in openFileDialog.FileNames)
                {
                    BitmapImage image = new BitmapImage(new Uri(file));
                    images.Add(image);
                    filePaths.Add(file);  // 保存文件路径
                }

                // 将图片列表绑定到ItemsControl以显示
                imagesControl.ItemsSource = images;

                // 假设您已经有了一个product_id// 需要替换为实际的产品ID
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string storeId = merchantId; 

            // 定义SQL更新命令
            string updateProductQuery = @"
        UPDATE products SET
            name = @name, 
            details = @details, 
            store_id = @storeId, 
            category = @category, 
            price = @price, 
            stock_quantity = @stockQuantity, 
            update_time = GETDATE()
        WHERE product_id = @productId;
    ";

            // 尝试解析价格和库存数量
            decimal price;
            int stockQuantity;
            if (!decimal.TryParse(TextBox2.Text, out price))
            {
                MessageBox.Show("请输入有效的价格");
                return;
            }
            if (!int.TryParse(TextBox4.Text, out stockQuantity))
            {
                MessageBox.Show("请输入有效的库存数量");
                return;
            }

            // 开始数据库操作
            try
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();  // 开始一个事务

                SqlCommand updateProductCommand = new SqlCommand(updateProductQuery, connection, transaction);
                updateProductCommand.Parameters.AddWithValue("@name", TextBox1.Text.Trim());
                updateProductCommand.Parameters.AddWithValue("@details", TextBox3.Text.Trim());
                updateProductCommand.Parameters.AddWithValue("@storeId", storeId);
                updateProductCommand.Parameters.AddWithValue("@category", ComboBox1.Text.Trim());
                updateProductCommand.Parameters.AddWithValue("@price", price);
                updateProductCommand.Parameters.AddWithValue("@stockQuantity", stockQuantity);
                updateProductCommand.Parameters.AddWithValue("@productId", productId);

                int rowsAffected = updateProductCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    UploadImagesToDatabase(filePaths, productId, connection, transaction); // 使用同一个事务处理图片上传
                    transaction.Commit(); // 如果一切顺利，提交事务
                    MessageBox.Show("商品信息更新成功！");
                }
                else
                {
                    MessageBox.Show("商品信息更新失败！");
                }
            }
            catch (Exception ex)
            {
                // 如果捕获到异常，确保事务能够回滚
                MessageBox.Show("更新商品时出现错误：" + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }



        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }
        private ObservableCollection<BitmapImage> imageCollection = new ObservableCollection<BitmapImage>();
        private void LoadProductDetails()
        {
            try
            {
                connection.Open();
                string query = @"
                SELECT p.product_id, p.Name, p.Price, p.stock_quantity, p.Category, p.Details, m.StoreName,
                    pi.image_data
                FROM products p
                JOIN merchants m ON p.store_id = m.MerchantId
                LEFT JOIN product_images pi ON p.product_id = pi.product_id
                WHERE p.product_id = @ProductId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductId", productId);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    // 假设已经定义了一个名为 selectedProduct 的 SelectedProduct 类的实例
                    TextBox1.Text = reader["Name"].ToString();
                    TextBox2.Text = reader["Price"].ToString();
                    TextBox3.Text = reader["Details"].ToString();
                    ComboBox1.SelectedValue = reader["Category"].ToString();
                    TextBox4.Text = reader["stock_quantity"].ToString();
                    textBlockStoreName.Text = reader["StoreName"].ToString();

                    byte[] imageData = reader["image_data"] as byte[];
                    if (imageData != null && imageData.Length > 0)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        using (MemoryStream stream = new MemoryStream(imageData))
                        {
                            stream.Position = 0;
                            bitmap.BeginInit();
                            bitmap.StreamSource = stream;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                        }
                        imageCollection.Add(bitmap);
                        // 假设你有一个用于显示图片的 Image 控件名为 imageControl
                        imagesControl.ItemsSource = imageCollection;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载商品信息时出现错误：" + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
    }
    

}
