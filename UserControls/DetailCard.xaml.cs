using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// DetailCard.xaml 的交互逻辑
    /// </summary>
    public partial class DetailCard : UserControl
    {
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(string), typeof(DetailCard), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(DetailCard), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(string), typeof(DetailCard), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.Register("Number", typeof(int), typeof(DetailCard), new PropertyMetadata(default(int)));

        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string Time
        {
            get { return (string)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public int Number
        {
            get { return (int)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        // Event declaration
        public event RoutedEventHandler CardClicked;

        public DetailCard()
        {
            InitializeComponent();
            this.MouseUp += OnMouseUpHandler;
            this.MouseLeftButtonUp += OnMouseLeftButtonUpHandler;
        }

        private void OnMouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            // Invoke the CardClicked event on mouse up
            CardClicked?.Invoke(this, new RoutedEventArgs());
        }
        private void OnMouseLeftButtonUpHandler(object sender, MouseButtonEventArgs e)
        {
            CardClicked?.Invoke(this, new RoutedEventArgs());
        }


    }

}
