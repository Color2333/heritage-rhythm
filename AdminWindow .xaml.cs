using heritage_rhythm.UserControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
namespace heritage_rhythm
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AdminWindow : Window
    {
        private AdminDashBoard DashBoard  = null;
        public AdminWindow()
        {
            InitializeComponent();
            SwitchToShopingCar += (sender, e) =>
            {
                mainFrame.Source = new Uri("AdminPage.xaml", UriKind.Relative);
            };
            SwitchToMainPage += (sender, e) =>
            {
                mainFrame.Source = new Uri("AdminMainPage.xaml", UriKind.Relative);
            };
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private bool IsMaximize = false;
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (IsMaximize)
                {
                    this.WindowState = WindowState.Normal;
                    this.Width = 1250;
                    this.Height = 830;

                    IsMaximize = false;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;

                    IsMaximize = true;
                }
            }
        }
        public event EventHandler SwitchToShopingCar;

        public void OnSwitchToShopingCar()
        {
            SwitchToShopingCar?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler SwitchToMainPage;

        public void OnSwitchToMainPage()
        {
            SwitchToMainPage?.Invoke(this, EventArgs.Empty);
        }


        private void MainPageButton_Click(object sender, RoutedEventArgs e)
        {
            // 切换到 MainPage
            mainFrame.Source = new Uri("AdminPage1.xaml", UriKind.Relative);

            // 更新 MenuButton 的激活状态
            UpdateMenuButtonActiveState(sender as MenuButton);
        }

        private void ShopingCarButton_Click(object sender, RoutedEventArgs e)
        {
            // 切换到 ShoppingCarPage
            mainFrame.Source = new Uri("Adminpage.xaml", UriKind.Relative);

            // 更新 MenuButton 的激活状态
            UpdateMenuButtonActiveState(sender as MenuButton);
        }

        private void UpdateMenuButtonActiveState(MenuButton activeButton)
        {
            // 将所有 MenuButton 的 IsActive 设置为 false
            foreach (var button in FindVisualChildren<MenuButton>(this))
            {
                button.IsActive = false;
            }

            // 将被点击的 MenuButton 的 IsActive 设置为 true
            if (activeButton != null)
            {
                activeButton.IsActive = true;
            }
        }

        // 辅助方法，用于查找所有 MenuButton 控件
        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (DashBoard == null)
            {
                DashBoard = new AdminDashBoard(); // 第一次点击时创建实例
            }

            mainFrame.Navigate(DashBoard); // 使用实例进行导航

            // 更新 MenuButton 的激活状态
            UpdateMenuButtonActiveState(sender as MenuButton);
        }

        private void CollectionButton_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Source = new Uri("StoreChat.xaml", UriKind.Relative);

            // 更新 MenuButton 的激活状态
            UpdateMenuButtonActiveState(sender as MenuButton);
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // 创建新窗口的实例
            Login secondWindow = new Login();

            // 显示新窗口
            secondWindow.Show();

            // 关闭当前窗口
            this.Close();
        }

        private void DataButton_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Source = new Uri("Data.xaml", UriKind.Relative);

            // 更新 MenuButton 的激活状态
            UpdateMenuButtonActiveState(sender as MenuButton);
        }
        private void Data1Button1_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Source = new Uri("Data1.xaml", UriKind.Relative);

            // 更新 MenuButton 的激活状态
            UpdateMenuButtonActiveState(sender as MenuButton);
        }
        private void Data2Button1_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Source = new Uri("Data2.xaml", UriKind.Relative);

            // 更新 MenuButton 的激活状态
            UpdateMenuButtonActiveState(sender as MenuButton);
        }
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取当前程序的完整路径
            string appPath = Process.GetCurrentProcess().MainModule.FileName;

            // 启动一个新的应用程序实例
            Process.Start(appPath);

            // 关闭当前应用程序实例
            Application.Current.Shutdown();
        }
        // 在MainWindow.xaml.cs中
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
