using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySql.Data;

namespace sqlperunat
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string mysqlconn = "server=127.0.0.1;user=root;database=perunat;uid=root;password;";
            MySqlConnection connection = new MySqlConnection(mysqlconn);

            try
            {
                connection.Open();
                Console.WriteLine("Connection successful!");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            finally
            {
                connection.Close();
            }
        }
    }
}
