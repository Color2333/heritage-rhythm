using System;
using System.Collections.Generic;
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

namespace heritage_rhythm.UserControls
{
    /// <summary>
    /// Product.xaml 的交互逻辑
    /// </summary>
    public partial class Product : UserControl
    {
        public Product()
        {
            InitializeComponent();
            this.MouseUp += Product_MouseUp;
        }
        public string ProductId { get; set; }
        public ImageSource ProductImageSource
        {
            get { return (ImageSource)GetValue(ProductImageSourceProperty); }
            set { SetValue(ProductImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ProductImageSourceProperty = DependencyProperty.Register("ProductImageSource", typeof(ImageSource), typeof(Product));

        public ImageSource SellerAvatarSource
        {
            get { return (ImageSource)GetValue(SellerAvatarSourceProperty); }
            set { SetValue(SellerAvatarSourceProperty, value); }
        }

        public static readonly DependencyProperty SellerAvatarSourceProperty = DependencyProperty.Register("SellerAvatarSource", typeof(ImageSource), typeof(Product));

        public string SellerName
        {
            get { return (string)GetValue(SellerNameProperty); }
            set { SetValue(SellerNameProperty, value); }
        }

        public static readonly DependencyProperty SellerNameProperty = DependencyProperty.Register("SellerName", typeof(string), typeof(Product));

        public string ProductName
        {
            get { return (string)GetValue(ProductNameProperty); }
            set { SetValue(ProductNameProperty, value); }
        }

        public static readonly DependencyProperty ProductNameProperty = DependencyProperty.Register("ProductName", typeof(string), typeof(Product));

        public string ProductDescription
        {
            get { return (string)GetValue(ProductDescriptionProperty); }
            set { SetValue(ProductDescriptionProperty, value); }
        }

        public static readonly DependencyProperty ProductDescriptionProperty = DependencyProperty.Register("ProductDescription", typeof(string), typeof(Product));

        public decimal Price
        {
            get { return (decimal)GetValue(PriceProperty); }
            set { SetValue(PriceProperty, value); }
        }

        public static readonly DependencyProperty PriceProperty = DependencyProperty.Register("Price", typeof(decimal), typeof(Product));
        public string ProductTitle
        {
            get { return (string)GetValue(ProductTitleProperty); }
            set { SetValue(ProductTitleProperty, value); }
        }

        public static readonly DependencyProperty ProductTitleProperty = DependencyProperty.Register("ProductTitle", typeof(string), typeof(Product));
        private void Product_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // 获取 MainWindow 的引用
            var mainWindow = Application.Current.MainWindow as MainWindow;

            // 导航到商品详情页
            if (mainWindow != null)
            {
                mainWindow.mainFrame.Navigate(new PurchaseDetails(ProductId));
            }
        }
    }
    public class ProductClickedEventArgs : EventArgs
    {
        public string ProductId { get; private set; }

        public ProductClickedEventArgs(string productId)
        {
            ProductId = productId;
        }
    }
}
