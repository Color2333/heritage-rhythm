using heritage_rhythm.UserControls;
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

namespace heritage_rhythm
{
    /// <summary>
    /// settings.xaml 的交互逻辑
    /// </summary>
    public partial class settings : Page
    {
        public settings()
        {
            InitializeComponent();
            DynamicContentArea.Content = new PersonalInfoControl();
        }
        private void DetailCard_CardClicked(object sender, RoutedEventArgs e)
        {
            // 更新DynamicContentArea的内容为新的Address控件
            var addressControl = new Address();
            addressControl.RequestReload += AddressControl_RequestReload; // 订阅事件
            DynamicContentArea.Content = addressControl;
        }
        private void PDetailCard_CardClicked(object sender, RoutedEventArgs e)
        {
            // 更新DynamicContentArea的内容为新的Address控件
            DynamicContentArea.Content = new PersonalInfoControl();
        }
        private void AddressControl_RequestReload(object sender, EventArgs e)
        {
            var newAddressControl = new Address();
            newAddressControl.RequestReload += AddressControl_RequestReload; // 重新订阅事件
            DynamicContentArea.Content = newAddressControl;
        }


    }
}
