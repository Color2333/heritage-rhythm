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
    public partial class MainWindow : Window
    {
        private MainPage mainPage;
        private Chat chatPage = null;

        public MainWindow()
        {
            InitializeComponent();
            mainPage = new MainPage();
            this.LocationChanged += MainWindow_LocationChanged;
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


        private void MainPageButton_Click(object sender, RoutedEventArgs e)
        {
            // 切换到 MainPage
            mainFrame.Navigate(mainPage);

            // 更新 MenuButton 的激活状态
            UpdateMenuButtonActiveState(sender as MenuButton);
        }

        private void ShopingCarButton_Click(object sender, RoutedEventArgs e)
        {
            // 切换到 ShoppingCarPage
            mainFrame.Source = new Uri("ShopingCar.xaml", UriKind.Relative);

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
            if (chatPage == null)
            {
                chatPage = new Chat(); // 第一次点击时创建实例
            }

            mainFrame.Navigate(chatPage); // 使用实例进行导航

            // 更新 MenuButton 的激活状态
            UpdateMenuButtonActiveState(sender as MenuButton);
        }

        private void CollectionButton_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Source = new Uri("Collection.xaml", UriKind.Relative);

            // 更新 MenuButton 的激活状态
            UpdateMenuButtonActiveState(sender as MenuButton);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // 创建新窗口的实例
            Login secondWindow = new Login();
            Application.Current.MainWindow = secondWindow;
            // 显示新窗口
            secondWindow.Show();

            // 关闭当前窗口
            this.Close();
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
        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Source = new Uri("Myorder.xaml", UriKind.Relative);

            // 更新 MenuButton 的激活状态
            UpdateMenuButtonActiveState(sender as MenuButton);
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Source = new Uri("settings.xaml", UriKind.Relative);

            // 更新 MenuButton 的激活状态
            UpdateMenuButtonActiveState(sender as MenuButton);
        }

        /*private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();

            }
        }
        */
        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();

            }
        }
        private void MinimizeButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private bool isAeroSnapMaximized = false; // 标记是否是通过拖动操作最大化
        private Rect restoreBounds; // 存储最大化前的窗口位置和大小
        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized && !isAeroSnapMaximized)
            {
                // 如果窗口是最大化的且不是由拖动操作引起的，则不处理
                return;
            }

            var screenTopEdge = SystemParameters.WorkArea.Top;
            var windowTopEdge = this.Top;

            if (this.WindowState == WindowState.Normal)
            {
                // 记录窗口最大化前的位置和大小
                restoreBounds = new Rect(this.Left, this.Top, this.Width, this.Height);

                // 当窗口顶部接近屏幕顶部时
                if (windowTopEdge <= screenTopEdge + 10) // 触发阈值
                {
                    this.WindowState = WindowState.Maximized;
                    isAeroSnapMaximized = true; // 标记为拖动操作最大化
                }
            }
            else if (this.WindowState == WindowState.Maximized && isAeroSnapMaximized)
            {
                // 当窗口是最大化状态，且是通过拖动操作最大化的
                if (windowTopEdge > screenTopEdge + 10)
                {
                    // 还原窗口大小和位置
                    this.Left = restoreBounds.Left;
                    this.Top = restoreBounds.Top;
                    this.Width = restoreBounds.Width;
                    this.Height = restoreBounds.Height;

                    this.WindowState = WindowState.Normal;
                    isAeroSnapMaximized = false;
                }
            }
        }
        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized && !isAeroSnapMaximized)
            {
                // 如果窗口被最大化且不是通过拖动操作，重置相关变量
                isAeroSnapMaximized = false;
            }
        }
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

        private void MenuButton_Click_1(object sender, RoutedEventArgs e)
        {
            mainFrame.Source = new Uri("Map.xaml", UriKind.Relative);

            // 更新 MenuButton 的激活状态
            UpdateMenuButtonActiveState(sender as MenuButton);
        }
    }
}
