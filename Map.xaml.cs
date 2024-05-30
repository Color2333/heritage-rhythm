using System;
using System.Windows.Controls;
using System.Collections.Generic;
using Microsoft.Maps.MapControl.WPF;
using System.Data.SqlClient; // 确保已引用System.Data命名空间

namespace heritage_rhythm
{
    public partial class Map : Page
    {
        private SqlConnection connection;

        public Map()
        {
            InitializeComponent();
            DNO dno = new DNO();
            connection = dno.Connection(); // 使用你提供的方法获取数据库连接
            LoadMapData("上海");  // 你可以根据实际情况传递区域参数
        }

        /*private void LoadMapData(string region)
        {
            var heritageItems = GetHeritageItems(region);
            mapItems.ItemsSource = heritageItems;  // 确保你的XAML中有MapItemsControl绑定到此数据源
        }

        private List<HeritageItem> GetHeritageItems(string region)
        {
            var items = new List<HeritageItem>();
            string sql = @"
            SELECT name, imagePath, latitude, longitude 
            FROM products 
            WHERE province = @province AND category = '非遗'";

            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@province", region);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new HeritageItem
                        {
                            Name = reader["name"] as string,
                            ImagePath = reader["imagePath"] as string,
                            Location = new Location(Convert.ToDouble(reader["latitude"]), Convert.ToDouble(reader["longitude"]))
                        };
                        items.Add(item);
                    }
                }
                connection.Close();
            }
            return items;
        }
    }
        */
        private void LoadMapData(string region)
        {
            var heritageItems = GetHeritageItems(region);
            mapItems.ItemsSource = heritageItems;  // 确保你的XAML中有MapItemsControl绑定到此数据源
        }

        private List<HeritageItem> GetHeritageItems(string region)
        {
            // 创建假数据
            return new List<HeritageItem>
            {
                new HeritageItem
                {
                    Name = "非遗项目1",
                    ImagePath = "Images/p1.jpg",
                    Location = new Location(39.9042, 116.4074) // 北京
                },
                new HeritageItem
                {
                    Name = "非遗项目2",
                    ImagePath = "Images/p2.jpg",
                    Location = new Location(31.2304, 121.4737) // 上海
                },
                new HeritageItem
                {
                    Name = "非遗项目3",
                    ImagePath = "Images/p3.jpg",
                    Location = new Location(23.1291, 113.2644) // 广州
                }
            };
        }
        // 商品信息类，如果还未定义
        public class HeritageItem
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public Location Location { get; set; }
    }
}
