using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace heritage_rhythm
{
    internal class DNO
    {
        private string connectionstr = "Data Source=rm-bp18s2069235f0xl1vo.sqlserver.rds.aliyuncs.com, 3433;Initial Catalog=HeritageRhythm;Persist Security Info=True;User ID=thinkbook;password=Zyc123456;";
        public SqlConnection Connection()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionstr);
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating SqlConnection: " + ex.Message);
                return null;
            }
        }
    }
}
