using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace heritage_rhythm.UserControls
{
    public partial class Card : UserControl
    {
        public Card()
        {
            InitializeComponent();
        }
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Day", typeof(string), typeof(Card));


        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(Card));

    }
}