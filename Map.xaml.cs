using heritage_rhythm.UserControls;
using Microsoft.Maps.MapControl.WPF;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace heritage_rhythm
{
    /// <summary>
    /// Map.xaml 的交互逻辑
    /// </summary>
    public partial class Map : Page
    {
        public Map()
        {
            InitializeComponent();
            LoadMapData("China");
            OverlayNonChinaRegions();
        }
        private void LoadMapData(string region)
        {
            var heritageItems = new List<HeritageItem>();

            switch (region)
            {
                case "China":
                    heritageItems.Add(new HeritageItem { Name = "非遗项目1", Location = new Location(39.9042, 116.4074) }); // 北京
                    heritageItems.Add(new HeritageItem { Name = "非遗项目2", Location = new Location(31.2304, 121.4737) }); // 上海
                    heritageItems.Add(new HeritageItem { Name = "非遗项目3", Location = new Location(23.1291, 113.2644) }); // 广州
                    myMap.Center = new Location(35.86166, 104.195397);
                    myMap.ZoomLevel = 5;
                    break;

                case "Beijing":
                    heritageItems.Add(new HeritageItem { Name = "北京非遗项目1", Location = new Location(39.9042, 116.4074) });
                    heritageItems.Add(new HeritageItem { Name = "北京非遗项目2", Location = new Location(39.9139, 116.3917) });
                    myMap.Center = new Location(39.9042, 116.4074);
                    myMap.ZoomLevel = 10;
                    break;

                case "Shanghai":
                    heritageItems.Add(new HeritageItem { Name = "上海非遗项目1", Location = new Location(31.2304, 121.4737) });
                    heritageItems.Add(new HeritageItem { Name = "上海非遗项目2", Location = new Location(31.2159, 121.4894) });
                    myMap.Center = new Location(31.2304, 121.4737);
                    myMap.ZoomLevel = 10;
                    break;

                case "Guangdong":
                    heritageItems.Add(new HeritageItem { Name = "广东非遗项目1", Location = new Location(23.1291, 113.2644) }); // 广州
                    heritageItems.Add(new HeritageItem { Name = "广东非遗项目2", Location = new Location(22.5431, 114.0579) }); // 深圳
                    myMap.Center = new Location(23.1291, 113.2644);
                    myMap.ZoomLevel = 8;
                    break;

                    // Add more cases for other provinces
            }

            mapItems.ItemsSource = heritageItems;
        }

        private void ProvinceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*if (provinceComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedProvince = selectedItem.Content.ToString();
                LoadMapData(selectedProvince);
            }*/
        }

        private void Pushpin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Pushpin pushpin && pushpin.DataContext is HeritageItem item)
            {
                //infoTextBlock.Text = $"名称: {item.Name}\n位置: {item.Location.Latitude}, {item.Location.Longitude}";
            }
        }

        private void OverlayNonChinaRegions()
        {
            // Define the boundary coordinates of China
            var chinaBoundary = new List<Location>
            {
                new Location(53.5606, 73.6754),  // Upper left corner
                new Location(53.5606, 135.0834), // Upper right corner
                new Location(3.8370, 135.0834),  // Lower right corner
                new Location(3.8370, 73.6754)    // Lower left corner
            };

            // Define the boundary of the entire map
            var mapBoundary = new List<Location>
            {
                new Location(85, -180), // Upper left corner
                new Location(85, 180),  // Upper right corner
                new Location(-85, 180), // Lower right corner
                new Location(-85, -180) // Lower left corner
            };

            // Create a polygon to cover the non-China regions
            var nonChinaRegion = new MapPolygon
            {
                Locations = new LocationCollection
                {
                    mapBoundary[0],
                    mapBoundary[1],
                    chinaBoundary[1],
                    chinaBoundary[0],
                    chinaBoundary[3],
                    chinaBoundary[2],
                    mapBoundary[2],
                    mapBoundary[3]
                },
                Fill = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)),
                StrokeThickness = 0
            };

            overlayLayer.Children.Add(nonChinaRegion);
        }
    }

    public class HeritageItem
    {
        public string Name { get; set; }
        public Location Location { get; set; }
        public List<Card> Items { get; set; } = new List<Card>();
    }

}
