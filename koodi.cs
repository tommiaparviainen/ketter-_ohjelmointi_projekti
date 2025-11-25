using System;
using System.Collections.Generic;
//System.Data for command object
using System.Data;
//Import namespace OleDb for databases (outside class)
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySql.Data;


namespace perunanryostajat
{
    //class DataService
    //{
    //    private OleDbConnection myConnection;

    //    public DataService()
    //    {
    //        //In class method(s), create and open connection
    //        //This can be done either once (e.g. Page_Load) for each
    //        //page request, or separately every time db connection is required
    //        String connstr;
    //        //set the path here acording to the location of database folder
    //        String projectPath = @"C:\Users\kkyyr\Downloads\";
    //        connstr = "Provider = Microsoft.ACE.OLEDB.12.0;" + @"Data Source = " +
    //        projectPath + @"C:\Users\kkyyr\Downloads\peruna.sql";
    //        /*- Määrittää tiedostopolun tietokantaan
    //        - Luo OleDbConnection-olion ja avaa yhteyden
    //        - Käyttää Microsoft Access -provideria (Microsoft.ACE.OLEDB.12.0)
    //        */
    //        //OleDbConnection requires namespace System.Data.OleDb
    //        myConnection = new OleDbConnection();
    //        myConnection.ConnectionString = connstr;
    //        myConnection.Open();

    //        /*

    //         * 
    //         * Luokka DataService:
    //        - Luo ja avaa yhteyden Access-tietokantaan (CustomerOrders2019.accdb)
    //        - Tarjoaa kolme metodia tietojen hakemiseen:
    //        - GetData: hakee kaikki rivit tietyistä kentistä
    //        - GetDataWhereString: hakee rivit, joissa kenttä vastaa tiettyä arvoa
    //        - GetDataWhereBetween: hakee rivit, joissa kentän arvo on tietyllä välillä
    //        */
    //    }

    //    private OleDbDataReader GetData(string[] fields, string table)
    //    {
    //        /*- Rakentaa SQL-kyselyn tyyliin:
    //            SELECT Field1, Field2 FROM TableName
    //            - Palauttaa OleDbDataReader-olion, jolla voi lukea tulokset
    //            */
    //        OleDbCommand myCommand = new OleDbCommand();

    //        myCommand.Connection = myConnection;
    //        //SQL query string
    //        myCommand.CommandText = "SELECT ";

    //        foreach (string s in fields)
    //            myCommand.CommandText += s + ", ";

    //        myCommand.CommandText = myCommand.CommandText.Remove(myCommand.CommandText.LastIndexOf(","));
    //        myCommand.CommandText += " FROM " + table;
    //        //CommandType requires namespace System.Data
    //        myCommand.CommandType = CommandType.Text;

    //        //Execute the SQL request command and
    //        //store the output in myReader object
    //        OleDbDataReader myReader;
    //        myReader = myCommand.ExecuteReader();

    //        return myReader;
    //    }

    //    private OleDbDataReader GetDataWhereString(string[] fields, string table, string keyField, string keyValue)
    //    {/*- Rakentaa SQL-kyselyn tyyliin:
    //    SELECT Field1, Field2 FROM TableName WHERE KeyField = 'KeyValue'
    //    - Soveltuu tekstimuotoisiin ehtoihin (esim. asiakasnimi = "Matti")
    //    */
    //        OleDbCommand myCommand = new OleDbCommand();

    //        myCommand.Connection = myConnection;
    //        //SQL query string
    //        myCommand.CommandText = "SELECT ";

    //        foreach (string s in fields)
    //            myCommand.CommandText += s + ", ";

    //        myCommand.CommandText = myCommand.CommandText.Remove(myCommand.CommandText.LastIndexOf(","));
    //        myCommand.CommandText += " FROM " + table;

    //        myCommand.CommandText += " WHERE " + keyField + "='" + keyValue + "';";
    //        //CommandType requires namespace System.Data
    //        myCommand.CommandType = CommandType.Text;


    //        //CommandType requires namespace System.Data
    //        myCommand.CommandType = CommandType.Text;

    //        //Execute the SQL request command and
    //        //store the output in myReader object
    //        OleDbDataReader myReader;
    //        myReader = myCommand.ExecuteReader();

    //        return myReader;
    //    }

    //    private OleDbDataReader GetDataWhereBetween(string[] fields, string table, string keyField, double minValue, double maxValue)
    //    {
    //        /*- Rakentaa SQL-kyselyn tyyliin:
    //    SELECT Field1, Field2 FROM TableName WHERE KeyField BETWEEN minValue AND maxValue
    //    - Soveltuu numeerisiin ehtoihin (esim. tilauksen summa välillä 100–500)
    //    */
    //        OleDbCommand myCommand = new OleDbCommand();

    //        myCommand.Connection = myConnection;
    //        //SQL query string
    //        myCommand.CommandText = "SELECT ";

    //        foreach (string s in fields)
    //            myCommand.CommandText += s + ", ";

    //        myCommand.CommandText = myCommand.CommandText.Remove(myCommand.CommandText.LastIndexOf(","));
    //        myCommand.CommandText += " FROM " + table;

    //        myCommand.CommandText += " WHERE " + keyField + " BETWEEN " + minValue + " AND " + maxValue + ";";
    //        //CommandType requires namespace System.Data
    //        myCommand.CommandType = CommandType.Text;


    //        //CommandType requires namespace System.Data
    //        myCommand.CommandType = CommandType.Text;

    //        //Execute the SQL request command and
    //        //store the output in myReader object
    //        OleDbDataReader myReader;
    //        myReader = myCommand.ExecuteReader();

    //        return myReader;
    //    }
    //}
    //--------------------------------------------------------------------------------------------------------------------------------------------------

    //kaikki enumit tässä samassa

    public enum UserRole
    {
        INTERNAL,
        EXTERNAL,
        ADMIN

    }




    public void GrantAccess(UserRole role)
    {
        switch (role)
        {
            case UserRole.INTERNAL:
                Console.WriteLine("Sisäiselle käyttäjälle annetaan täydet oikeudet.");
                break;
            case UserRole.EXTERNAL:
                Console.WriteLine("Ulkoinen käyttäjä saa rajatut oikeudet.");
                break;
        }
    }





    public enum ContractStatus
    {
        DRAFT,
        PENDING_APPROVAL,
        APPROVED,
        SENT_TO_CLIENT
    }

    public enum StakeholderRole
    {
        VIEWER,
        EDITOR,
        APPROVER
    }

    public enum Visibility
    {
        INTERNAL,
        EXTERNAL
    }

    //kaikki enumit tässä samassa 

    public class User
    {

        private int userID { get; set; }
        private string username { get; set; }
        private string email { get; set; }

        private UserRole Role { get; set; }

        public void CreateUser(int id, string name, string mail)
        {
            userID = id;
            username = name;
            email = mail;

        }

        public void editUser(string newName, string newEmail)
        {
            username = newName;
            email = newEmail;
        }

        public void deleteUser()
        {

            username = null;
            email = null;

        }


    }



    public class BlockCategory
    {
        public int categoryId { get; set; }
        private string name { get; set; }
        private string description { get; set; }

        public void createCategory(int ID, string nm, string desc)
        {
            categoryId = ID;
            name = nm;
            description = desc;
        }

        public void editCategory(int ID, string nm, string desc)
        {

            name = nm;
            description = desc;
        }

    }


    public class ContractBlock
    {
        private int blockID { get; set; }
        private string title { get; set; }
        private string content { get; set; }

        public ContractBlock(int block, string tit, string cont)
        {
            blockID = block;
            title = tit;
            content = cont;
        }

        public ContractBlock CopyBlock(int newId, string newTitle, string newContent)
        {
            return new ContractBlock(newId, newTitle, newContent);
        }


        public void EditBlock(string newTitle, string newContent)
        {
            title = newTitle;
            content = newContent;
        }
    }

    //+editBlock()



    public class ContractBlockAssigment
    {
        private int assignmentID { get; set; }
        private Contract contract { get; set; }
        private ContractBlock block { get; set; }
        private int orderNumber { get; set; }
        private string editableCopy { get; set; }

        public void assignBlock(int id, Contract c, ContractBlock b, int order, string copy)
        {
            assignmentID = id;
            contract = c;
            block = b;
            orderNumber = order;
            editableCopy = copy;


        }

        public void editCopy(string newContent)
        {
            editableCopy = newContent;
        }

    }




    public class Contract
    {
        private int contractId { get; set; }
        private string title { get; set; }
        public List<BlockCategory> Blocks { get; private set; } = new List<BlockCategory>();



        public void createContract(int Id, string tl)
        {
            contractId = Id;
            title = tl;
        }

        public void editContract(string newTitle)
        {

            title = newTitle;
        }

        public void addBlock(BlockCategory block)

        {
            Blocks.Add(block);
        }

        public void removeBlock(int categoryId)
        {
            BlockCategory blockToRemove = null;

            foreach (var block in Blocks)
            {
                if (block.categoryId == categoryId)
                {
                    blockToRemove = block;
                    break;
                }

            }
            if (blockToRemove != null)
            {
                Blocks.Remove(blockToRemove);
            }
        }



    }

    public class ContractStakeHolder
    {
        private int stakeHolderID { get; set; }
        private Contract contract { get; set; }
        private User user { get; set; }
        public StakeholderRole Role { get; private set; }

        //private StakeHolderRole role;

        public void assignStakeHolder(int id, Contract c, User u, StakeholderRole role)
        {
            stakeHolderID = id;
            contract = c;
            user = u;
            Role = role;
        }


    }

    public class Approval
    {
        private int approvalID { get; set; }
        private Contract contract { get; set; }
        private User user { get; set; }
        private DateTime approvedAt { get; set; }
        public void approve(int id, Contract c, User u, DateTime date)
        {
            approvalID = id;
            contract = c;
            user = u;
            approvedAt = date;
        }
    }

    public class Comment
    {
        private int commentID { get; set; }
        private Contract contract { get; set; }
        private User user { get; set; }
        private string content { get; set; }
        private DateTime createdAt { get; set; }
        public Visibility visibility { get; private set; }


        public void addComment(int id, Contract c, User u, string cnt, DateTime date, Visibility v)
        {

            commentID = id;
            Contract contract = c;
            User user = u;
            content = cnt;
            createdAt = date;
            Visibility = v;

        }



        public void editComment(string newContent)
        {
            content = newContent;

        }




        #endregion
        #region Repository Example
        public static class BuyerRepository
        {
            public static void InsertBuyer(MySqlConnection conn, string name)
            {
                string insertQuery = "INSERT INTO buyers (buyer_name) VALUES (@buyer_name)";
                using var cmd = new MySqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@buyer_name", name);
                cmd.ExecuteNonQuery();
            }

            public static void ListBuyers(MySqlConnection conn)
            {
                string query = "SELECT buyer_id, buyer_name FROM buyers";
                using var cmd = new MySqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();
                Console.WriteLine("Buyer Data:");
                Console.WriteLine("-------------------------------------------");
                while (reader.Read())
                {
                    Console.WriteLine($"Id: {reader.GetInt32(0)} | Name: {reader.GetString(1)}");
                }
            }
        }

        #endregion

        public static class ContractRepository
        {
            public static void InsertContract(MySqlConnection conn, string title, string clientName, int createdBy)
            {
                string insertQuery = @"INSERT INTO Contracts (Title, ClientName, Status, CreatedBy)
                               VALUES (@title, @client, 'DRAFT', @createdBy)";
                using var cmd = new MySqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@client", clientName);
                cmd.Parameters.AddWithValue("@createdBy", createdBy);
                cmd.ExecuteNonQuery();
            }

            public static void ListContracts(MySqlConnection conn)
            {
                string query = "SELECT ContractId, Title, ClientName, Status FROM Contracts";
                using var cmd = new MySqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine("Contracts:");
                Console.WriteLine("-------------------------------------------");
                while (reader.Read())
                {
                    Console.WriteLine($"Id: {reader.GetInt32(0)} | Title: {reader.GetString(1)} | Client: {reader.GetString(2)} | Status: {reader.GetString(3)}");
                }
            }
        }

        public static class BlockRepository
        {
            public static void InsertBlock(MySqlConnection conn, string title, string content, int createdBy)
            {
                string insertQuery = @"INSERT INTO ContractBlocks (Title, Content, CreatedBy)
                               VALUES (@title, @content, @createdBy)";
                using var cmd = new MySqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@content", content);
                cmd.Parameters.AddWithValue("@createdBy", createdBy);
                cmd.ExecuteNonQuery();
            }

            public static void ListBlocks(MySqlConnection conn)
            {
                string query = "SELECT BlockId, Title, Content FROM ContractBlocks";
                using var cmd = new MySqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine("Contract Blocks:");
                Console.WriteLine("-------------------------------------------");
                while (reader.Read())
                {
                    Console.WriteLine($"Id: {reader.GetInt32(0)} | Title: {reader.GetString(1)} | Content: {reader.GetString(2)}");
                }
            }
        }


        internal class Program
    {
        static void Main(string[] args)
        {

            string MySqlCon = "server=127.0.0.1;user=root;database=uusiyritys;port=3306;password=;";
            MySqlConnection connection = new MySqlConnection(MySqlCon);

            try
            {
                connection.Open();
                Console.WriteLine("Connection successful!");
                    Console.WriteLine("\n== Käyttäjätesti ==");
                    string insertUser = @"INSERT INTO Users(Name, Email, Role, IsActive) 
                                  VALUES ('Test User','test@example.com','INTERNAL',1)";
                    using (var cmd = new MySqlCommand(insertUser, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    Console.WriteLine("Käyttäjä lisätty.");

                    // 2) Lisää sopimuslohko
                    Console.WriteLine("\n== Lohkotesti ==");
                    BlockRepository.InsertBlock(connection, "Delivery Terms", "Delivery within 14 days", 1);
                    BlockRepository.ListBlocks(connection);

                    // 3) Lisää sopimus
                    Console.WriteLine("\n== Sopimustesti ==");
                    ContractRepository.InsertContract(connection, "Service Agreement", "Client Oy", 1);
                    ContractRepository.ListContracts(connection);

                    // 4) Lisää kommentti
                    Console.WriteLine("\n== Kommenttitesti ==");
                    string insertComment = @"INSERT INTO Comments(ContractId, AuthorId, Visibility, Content) 
                                     VALUES (1,1,'INTERNAL','Tarkista toimitusehdot')";
                    using (var cmd = new MySqlCommand(insertComment, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    Console.WriteLine("Kommentti lisätty.");

                    // 5) Hyväksyntä
                    Console.WriteLine("\n== Hyväksyntätesti ==");
                    string insertApproval = @"INSERT INTO Approvals(ContractId, ReviewerId, Decision, Note, DecidedAt) 
                                      VALUES (1,1,'APPROVED','Hyväksytty testissä',NOW())";
                    using (var cmd = new MySqlCommand(insertApproval, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    Console.WriteLine("Sopimus hyväksytty.");

                    Console.WriteLine("\n== Testiohjelma valmis ==");

                    //while (true)
                    //{
                    //    Console.WriteLine("Enter buyer name (or 'exit' to stop): ");
                    //    string buyerName = Console.ReadLine();

                    //    if (buyerName.ToLower() == "exit")
                    //        break;
                    //    string insertQuery = "INSERT INTO buyers (buyer_name) VALUES (@buyer_name)";
                    //    MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection);
                    //    insertCmd.Parameters.AddWithValue("@buyer_name", buyerName);
                    //    insertCmd.ExecuteNonQuery();
                    //    Console.WriteLine("Buyer added successfully!");
                    //}

                    ////Query to get buyer data
                    //string query = "SELECT buyer_id, buyer_name FROM buyers";
                    //MySqlCommand cmd = new MySqlCommand(query, connection);
                    //MySqlDataReader reader = cmd.ExecuteReader();

                    ////Display the data
                    //Console.WriteLine("buyer Data:");
                    //Console.WriteLine("-------------------------------------------");

                    //while (reader.Read())
                    //{
                    //    for (int i = 0; i < reader.FieldCount; i++)
                    //    {
                    //        Console.Write(reader.GetName(i) + ": " + reader[i] + " | ");
                    //    }
                    //    Console.WriteLine();
                    //}
                    //reader.Close();

                }
                catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            finally
            {
                connection.Close();
                Console.WriteLine("Connection closed.");

            }





            }
    }
}
