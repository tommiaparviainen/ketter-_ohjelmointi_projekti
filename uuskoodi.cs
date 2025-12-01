using System;
using System.Collections.Generic;
//System.Data for command object
using System.Data;
using System.Data.Common;
//Import namespace OleDb for databases (outside class)
using System.Data.OleDb;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hellopello;
using MySql.Data;
using MySql.Data.MySqlClient;
using static System.Net.Mime.MediaTypeNames;


namespace hellopello
{
    public enum UserRole { INTERNAL, EXTERNAL, ADMIN }
    public enum StakeholderRole { VIEVER, EDITOR, APPROVER }

    public enum Visibility { INTERNAL, EXTERNAL }

    public class User
    {

        private int userId;
        private string userName;
        private string email;

        private UserRole Role;

        public string Username
        {
            get { return userName; }
        }
        public string Email
        {
            get { return email; }
        }

        public UserRole role
        {
            get { return Role; }
        }

        public User(int id, string name, string mail, UserRole role)
        {
            userId = id;
            userName = name;
            email = mail;
            Role = role;
        }
        public override string ToString()
        {
            return userId + ": " + userName + ", " + email + ", Role: " + Role;
        }

        public void AddUser() //käyttäjä tietokantaan
        {

            string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;";
            using (MySqlConnection connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();


                // SQL INSERT-lause
                string checkQuery = "SELECT COUNT(*) FROM users WHERE email = @email";
                using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@email", email);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count == 0)
                    {
                        string insertQuery = "INSERT INTO users (userName, email, Role) VALUES (@userName, @email, @Role)";
                        using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@userName", userName);
                            insertCmd.Parameters.AddWithValue("@email", email);
                            insertCmd.Parameters.AddWithValue("@Role", Role);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Email already exists!");
                    }
                }
            }
        }
        public void editUser(string newName, string newEmail) //käyttäjän tietojen muokkaus
        {
            userName = newName;
            email = newEmail;

            string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;";
            using (MySqlConnection connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();
                string query = "UPDATE users SET UserName=@UserName, Email=@Email WHERE UserId=@UserId";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                }
            }

        }


        public void deleteUser() //käyttäjän poistaminen
        {

            userName = null;
            email = null;

            string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;";
            using (MySqlConnection connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();
                string query = "DELETE FROM Users WHERE UserId=@UserId";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                }
            }


        }
    }

    public class ContractBlock // sopimuslohko
    {
        private int blockId;
        private string title;
        private string content;
        private int CreatedBy;
        private DateTime CreatedAt;

        public ContractBlock(int block, string tit, string cont)
        {
            blockId = block;
            title = tit;
            content = cont;
        }

        public ContractBlock CopyBlock(int newId, string newTitle, string newContent) // skopiointi
        {
            return new ContractBlock(newId, newTitle, newContent);
        }


        public void EditBlock(string newTitle, string newContent) //muokkaus
        {
            title = newTitle;
            content = newContent;
        }
        public void SaveBlock(int createdBy) //yksittäinen sopimuslohko tietokantaan (???)
        {
            string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;";
            using (MySqlConnection connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();
                string query = @"INSERT INTO contractblocks (Title, Content, CreatedBy, CreatedAt) 
                         VALUES (@Title, @Content, @CreatedBy, @CreatedAt)";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Content", content);
                    cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy); // creator.userId
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    cmd.ExecuteNonQuery();

                }
            }
        }

        public void UpdateBlock() //blockin päivitys/muokkaus tietokantaan
        {
            string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;";
            using (MySqlConnection connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();

                string query = @"UPDATE contractblocks
                         SET Title=@Title, Content=@Content 
                         WHERE BlockId=@BlockId";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Content", content);
                    cmd.Parameters.AddWithValue("@BlockId", blockId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeleteBlock() //blockin poisto tietokannasta
        {
            string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;";
            using (MySqlConnection connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();

                string query = "DELETE FROM contractblockss WHERE BlockId=@BlockId";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@BlockId", blockId);
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }

    public class Contract
    {
        private int contractId;
        private string title;
        private string clientName;
        private string status;
        private string content;
        private int createdBy;
        private DateTime createdAt;

        // Käytetään listaa taulukon sijaan
        public List<ContractBlock> contractBlocks = new List<ContractBlock>();

        public Contract(string tl, string name, string st, int creator, int ConID)
        {
            title = tl;
            clientName = name;
            status = st;
            createdBy = creator;
            createdAt = DateTime.Now;
            contractId = ConID;
        }

        public void addBlock(ContractBlock block, int order)
        {
            contractBlocks.Add(block);
        }

        public void SaveContract()
        {
            string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;";
            using (MySqlConnection connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();

                // 1. Tallennetaan contract
                string insertContract = @"INSERT INTO contracts (Title, ClientName, Status, CreatedBy, CreatedAt)
                                      VALUES (@Title, @ClientName, @Status, @CreatedBy, @CreatedAt)";
                using (MySqlCommand cmd = new MySqlCommand(insertContract, connection))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@ClientName", clientName);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                    cmd.Parameters.AddWithValue("@CreatedAt", createdAt);

                    cmd.ExecuteNonQuery();
                }

                // Haetaan luodun contractin ID erikseen
                string idQuery = "SELECT LAST_INSERT_ID()";
                using (MySqlCommand idCmd = new MySqlCommand(idQuery, connection))
                {
                    contractId = Convert.ToInt32(idCmd.ExecuteScalar());
                }

                // 2. Tallennetaan kaikki blockit tähän contractiin
                foreach (var block in contractBlocks)
                {
                    if (block == null) continue; // ohitetaan null-arvot

                    string blockQuery = @"INSERT INTO contracts (Title, ClientName, Status, CreatedBy, CreatedAt)
    VALUES (@Title, @ClientName, @Status, @CreatedBy, @CreatedAt);
";

                    using (MySqlCommand cmd = new MySqlCommand(insertContract, connection))
                    {
                        cmd.Parameters.AddWithValue("@Title", title);                 // esim. "Vuokrasopimus"
                        cmd.Parameters.AddWithValue("@ClientName", clientName ?? (object)DBNull.Value); // salli NULL
                        cmd.Parameters.AddWithValue("@Status", status);               // esim. "Aktiivinen"
                        cmd.Parameters.AddWithValue("@CreatedBy", createdBy);         // olemassa oleva users.UserId
                        cmd.Parameters.AddWithValue("@CreatedAt", createdAt);         // DateTime.Now

                        cmd.ExecuteNonQuery();
                    }

                }
            }
        }
    }
    public class DataService
    {
        private string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;";

        public List<User> GetAllCustomers()
        {
            List<User> users = new List<User>();
            using (MySqlConnection connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();
                string query = "SELECT UserId, UserName, Email, Role FROM users";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32("UserId");
                        string name = reader.GetString("UserName");
                        string email = reader.GetString("Email");

                        // Haetaan rooli tietokannasta
                        string roleStr = reader.IsDBNull(reader.GetOrdinal("Role"))
                            ? "EXTERNAL"
                            : reader.GetString("Role");

                        UserRole role;
                        // TryParse varmistaa ettei tule virhettä, case-insensitive
                        if (!Enum.TryParse<UserRole>(roleStr, true, out role))
                        {
                            role = UserRole.EXTERNAL; // oletus jos arvo ei kelpaa
                        }

                        users.Add(new User(id, name, email, role));
                    }
                }
            }
            return users;
        }

        public User GetUserByName(string userName)
        {
            using (MySqlConnection connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();
                string query = "SELECT UserId, UserName, Email, Role FROM users WHERE UserName=@UserName";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.GetInt32("UserId");
                            string name = reader.GetString("UserName");
                            string email = reader.GetString("Email");

                            string roleStr = reader.IsDBNull(reader.GetOrdinal("Role"))
                                ? "EXTERNAL"
                                : reader.GetString("Role");

                            UserRole role;
                            if (!Enum.TryParse<UserRole>(roleStr, true, out role))
                            {
                                role = UserRole.EXTERNAL;
                            }

                            return new User(id, name, email, role);
                        }
                    }
                }
            }
            return null;

        }
    }

    class MyApplication
    {
        DataService myDataService;

        public MyApplication()
        {
            myDataService = new DataService();
        }

        public string GetAllCustomers()
        {
            var customers = myDataService.GetAllCustomers();
            return string.Join("\n", customers.Select(c => c.ToString()));
        }

        public User GetCustomerDataByName(string userName)
        {
            return myDataService.GetUserByName(userName);
        }
    }

    class UI
    {
        MyApplication myApp = new MyApplication();

        public void ShowMenu()
        {
            Console.WriteLine("In this app you can (select with number):");
            Console.WriteLine("1. Show all customers");
            Console.WriteLine("2. Show data of one customer only");
            Console.WriteLine("exit (to finish)");
        }

        private void ShowListEnumerated(string[] stringList)
        {
            for (int i = 0; i < stringList.Length; i++)
                Console.WriteLine((i + 1) + ": " + stringList[i]);
        }

        private void ShowOneCustomer()
        {
            bool goOn = true;
            while (goOn)
            {
                Console.Clear();
                string[] customers = myApp.GetAllCustomers().Split('\n');

                if (customers.Length == 0)
                {
                    Console.WriteLine("No customers available in the database");
                    break;
                }

                ShowListEnumerated(customers);
                Console.WriteLine("Enter customer number:");

                try
                {
                    int custNr = Convert.ToInt32(Console.ReadLine());
                    if (custNr < 1 || custNr > customers.Length)
                        throw new Exception();

                    string selectedName = customers[custNr - 1].Split(':')[1].Trim().Split(' ')[0];
                    User cust = myApp.GetCustomerDataByName(selectedName);

                    if (cust != null)
                    {
                        Console.WriteLine($"ID: {cust.role} | Name: {cust.Username} | Email: {cust.Email}");
                    }
                    else
                    {
                        Console.WriteLine("Customer not found.");
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid input. Try again.");
                }

                Console.WriteLine("Want to see another customer data (Y/N)?");
                if (Console.ReadLine().ToUpper() != "Y")
                    goOn = false;
            }
        }

        public void Run()
        {
            ShowMenu();
            string command = Console.ReadLine();

            while (true)
            {
                switch (command)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine(myApp.GetAllCustomers());
                        break;
                    case "2":
                        Console.Clear();
                        ShowOneCustomer();
                        break;
                    case "exit":
                        Console.WriteLine("Press any key to close the program");
                        Console.ReadLine();
                        return;
                    default:
                        Console.WriteLine("Invalid input: You can only select from given options");
                        break;
                }
                ShowMenu();
                command = Console.ReadLine();
            }
        }



    }



        internal class Program
        {
            static void Main(string[] args)
            {
            Console.WriteLine("=== TESTAUSOHJELMA: KK-sopimusjärjestelmä ===");

            // 1. Luodaan käyttäjä
            User u1 = new User(1, "Matti", "matti@testi.fi", UserRole.INTERNAL);
            Console.WriteLine("Luotu käyttäjä: " + u1);

            // Tallennetaan käyttäjä tietokantaan
            u1.AddUser();

            // Muokataan käyttäjää
            u1.editUser("Matti Meikäläinen", "matti.meikalainen@testi.fi");
            Console.WriteLine("Muokattu käyttäjä: " + u1);

            // 2. Luodaan sopimuslohko
            ContractBlock block1 = new ContractBlock(1, "Ehdot", "Auto palautettava tankki täynnä.");
            block1.SaveBlock(u1.role.GetHashCode()); // tallennus tietokantaan
            Console.WriteLine("Tallennettu sopimuslohko: Ehdot");

            // Muokataan lohkoa
            block1.EditBlock("Ehdot (päivitetty)", "Auto palautettava tankki täynnä tai lisämaksu 80 €.");
            block1.UpdateBlock();
            Console.WriteLine("Päivitetty sopimuslohko.");

            // 3. Luodaan sopimus
            Contract contract = new Contract("Vuokrasopimus", "Asiakas Oy", "Aktiivinen", 1, 1);
            contract.addBlock(block1, 1);
            contract.SaveContract();
            Console.WriteLine("Sopimus tallennettu tietokantaan.");

            // 4. Haetaan asiakkaita DataServicen kautta
            DataService ds = new DataService();
            List<User> allUsers = ds.GetAllCustomers();
            Console.WriteLine("Tietokannasta haetut käyttäjät:");
            foreach (var usr in allUsers)
            {
                Console.WriteLine(usr);
            }

            // Haetaan yksi käyttäjä nimellä
            User foundUser = ds.GetUserByName("Matti");
            if (foundUser != null)
            {
                Console.WriteLine("Haettu käyttäjä: " + foundUser);
            }

            // 5. Käynnistetään UI
            Console.WriteLine("\n=== Käynnistetään käyttöliittymä ===");
            UI ui = new UI();
            ui.Run();

            // Lopuksi poistetaan käyttäjä
            u1.deleteUser();
            Console.WriteLine("Käyttäjä poistettu tietokannasta.");
        

    }





}

    
}

