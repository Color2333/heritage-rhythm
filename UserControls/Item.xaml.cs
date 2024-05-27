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
    /// Item.xaml 的交互逻辑
    /// </summary>
    public partial class Item : UserControl
    {
        public Item()
        {
            InitializeComponent();
        }
        public bool IsSelected
        {
            get { return ItemCheckBox.IsChecked ?? false; }
            set { ItemCheckBox.IsChecked = value; }
        }
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(Item));


        public string ProductId
        {
            get { return (string)GetValue(ProductIdProperty); }
            set { SetValue(ProductIdProperty, value); }
        }

        public static readonly DependencyProperty ProductIdProperty =
            DependencyProperty.Register("ProductId", typeof(string), typeof(Item), new PropertyMetadata(""));

        // Product Name
        public string ProductName
        {
            get { return (string)GetValue(ProductNameProperty); }
            set { SetValue(ProductNameProperty, value); }
        }

        public static readonly DependencyProperty ProductNameProperty =
            DependencyProperty.Register("ProductName", typeof(string), typeof(Item), new PropertyMetadata(""));

        // Price
        public string Price
        {
            get { return (string)GetValue(PriceProperty); }
            set { SetValue(PriceProperty, value); }
        }

        public static readonly DependencyProperty PriceProperty =
            DependencyProperty.Register("Price", typeof(string), typeof(Item), new PropertyMetadata(""));
        // 店铺名称
        public string StoreName
        {
            get { return (string)GetValue(StoreNameProperty); }
            set { SetValue(StoreNameProperty, value); }
        }

        public static readonly DependencyProperty StoreNameProperty =
            DependencyProperty.Register("StoreName", typeof(string), typeof(Item), new PropertyMetadata(""));

        // 选定的数量
        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(int), typeof(Item), new PropertyMetadata(0));
        public string Category
        {
            get { return (string)GetValue(CategoryProperty); }
            set { SetValue(CategoryProperty, value); }
        }

        public static readonly DependencyProperty CategoryProperty =
            DependencyProperty.Register("Category", typeof(string), typeof(Item), new PropertyMetadata(""));
        private void ButtonDecrease_Click(object sender, RoutedEventArgs e)
        {
            int quantity = int.Parse(textBlockQuantity.Text);
            if (quantity > 1) // Assuming the quantity cannot go below 1
            {
                quantity--;
                textBlockQuantity.Text = quantity.ToString();
            }
        }

        private void ButtonIncrease_Click(object sender, RoutedEventArgs e)
        {
            int quantity = int.Parse(textBlockQuantity.Text);
            quantity++;
            textBlockQuantity.Text = quantity.ToString();
        }

        private void Item_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;

            // 导航到商品详情页
            if (mainWindow != null)
            {
                mainWindow.mainFrame.Navigate(new PurchaseDetails(ProductId));
            }
        }
    }
}
