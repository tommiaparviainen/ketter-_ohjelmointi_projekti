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
            string mysqlconn = "server=127.0.0.1;user=root;database=peruna;password;";
            MySqlConnection connection = new MySqlConnection(mysqlconn);

            try
            {
                connection.Open();
                Console.WriteLine("Connection successful!");

                while (true)
                {
                    Console.WriteLine("Enter buyer name (or 'exit' to stop): ");
                    string buyerName = Console.ReadLine();

                    if (buyerName.ToLower() == "exit")
                        break;
                    string insertQuery = "INSERT INTO buyers (buyer_name) VALUES (@buyer_name)";
                    MySqlCommand cmd = new MySqlCommand(insertQuery, connection);
                    insertCmd.Parameters.AddWithValue("@buyer_name", buyerName);
                    insertCmd.ExecuteNonQuery();
                    Console.WriteLine("Buyer added successfully!");
                }

                //Query to get buyer data
                string query = "SELECT buyer_id, buyer_name FROM buyers";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                //Display the data
                Console.WriteLine("buyer Data:");
                Console.WriteLine("-------------------------------------------");

                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write(reader.GetName(i) + ": " + reader[i] + " | ");
                    }
                    Console.WriteLine();
                }
                reader.Close();
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
