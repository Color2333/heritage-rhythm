using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using heritage_rhythm.UserControls;

namespace heritage_rhythm
{
    /// <summary>
    /// AdminMainPage.xaml 的交互逻辑
    /// </summary>
    public partial class AdminMainPage : Page
    {
        public AdminMainPage()
        {
            InitializeComponent();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string merchantId = Properties.Settings.Default.MerchantId;
            // 假设您已经有了merchantId，可能是从某个控件或者属性获取
            if (this.NavigationService != null)
            {
                this.NavigationService.Navigate(new release(merchantId));
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var brushConverter = new BrushConverter();
            Pbtn.BorderBrush = (Brush)brushConverter.ConvertFromString("#784FF2");  // 设置为紫色
            Obtn.BorderBrush = Brushes.Transparent;  // 透明色或者null，根据你的需要选择
            DataBorder.Child = new ProductData();
        }

        private void ButtonBase2_OnClick(object sender, RoutedEventArgs e)
        {
            var brushConverter = new BrushConverter();
            Obtn.BorderBrush = (Brush)brushConverter.ConvertFromString("#784FF2");  // 设置为紫色
            Pbtn.BorderBrush = Brushes.Transparent;  // 透明色或者null，根据你的需要选择
            DataBorder.Child = new OrderData();  // 切换到订单数据视图
        }
    }
}

