using heritage_rhythm.UserControls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    public partial class release : Page
    {
        private string merchantId;
        private SqlConnection connection;

        public release(string merchantid)
        {
            InitializeComponent();
            this.merchantId = merchantid;
            DNO dno = new DNO();
            connection = dno.Connection();
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

        private void UploadImagesToDatabase(List<string> filePaths, int productId, SqlConnection connection, SqlTransaction transaction)
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
            string storeId =merchantId;

            string insertProductQuery = "INSERT INTO products (name, details, store_id, category, price, stock_quantity, status, creation_time, update_time) VALUES (@name, @details, @storeId, @category, @price, @stockQuantity, 'True', GETDATE(), GETDATE()); SELECT SCOPE_IDENTITY();";
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

            // 然后使用这些变量设置参数

            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();

            SqlCommand insertProductCommand = new SqlCommand(insertProductQuery, connection, transaction);
            insertProductCommand.Parameters.AddWithValue("@name", TextBox1.Text.Trim());
            insertProductCommand.Parameters.AddWithValue("@details", TextBox3.Text.Trim());
            insertProductCommand.Parameters.AddWithValue("@storeId", storeId);
            insertProductCommand.Parameters.AddWithValue("@category", ComboBox1.Text.Trim());
            insertProductCommand.Parameters.AddWithValue("@price", price);
            insertProductCommand.Parameters.AddWithValue("@stockQuantity", stockQuantity);
            try
            {
                // 执行插入产品并获取插入后的最新 product_id
                int productId = Convert.ToInt32(insertProductCommand.ExecuteScalar());
                if (productId > 0)
                {
                    UploadImagesToDatabase(filePaths, productId, connection, transaction);  // 确保这个方法也使用同一个事务
                    transaction.Commit();  // 提交事务
                    MessageBox.Show("发布成功");
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback(); // 回滚事务
                MessageBox.Show("发布商品时出现错误：" + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }
    }
    

}
