using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace heritage_rhythm.UserControls
{
    public partial class ProductCard : UserControl
    {
        public ProductCard()
        {
            InitializeComponent();
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ProductCard));


        public string merchant
        {
            get { return (string)GetValue(UpPriceProperty); }
            set { SetValue(UpPriceProperty, value); }
        }

        public static readonly DependencyProperty UpPriceProperty = DependencyProperty.Register("merchant", typeof(string), typeof(ProductCard));



        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(ProductCard));


        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(ProductCard));

    }
}