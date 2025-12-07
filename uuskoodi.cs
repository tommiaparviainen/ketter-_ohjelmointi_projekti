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

    public class User
    {

        public int userId;
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
                        string insertQuery = "INSERT INTO users (UserName, Email, Role) VALUES (@userName, @email, @Role)";
                        using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@userName", userName);
                            insertCmd.Parameters.AddWithValue("@email", email);
                            insertCmd.Parameters.AddWithValue("@Role", Role); // tai int jos tallennat numerona
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
        public int blockId;
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
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy); // creator.userId
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    cmd.ExecuteNonQuery();

                    // Hae AUTO_INCREMENT ID
                    this.blockId = (int)cmd.LastInsertedId;


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
    public class Comment
    {
        private Contract contract;
        private User user;
        public int CommentId { get; private set; }   // AUTO_INCREMENT ID
        public int ContractId { get; private set; }
        public int AuthorId { get; private set; }
        public string Visibility { get; set; }       // esim. "Public" / "Private"
        private string content;
        private DateTime createdAt;

        // Lisää kommentti ja tallenna tietokantaan
        public void addComment(Contract c, User u, string cnt, string visibility)
        {
            contract = c;
            user = u;
            content = cnt;
            createdAt = DateTime.Now;
            Visibility = visibility;

            string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;";
            using (MySqlConnection connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();
                string query = @"INSERT INTO comment (ContractId, AuthorId, Visibility, Content, CreatedAt)
                             VALUES (@ContractId, @AuthorId, @Visibility, @Content, @CreatedAt)";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ContractId", c.contractId);
                    cmd.Parameters.AddWithValue("@AuthorId", u.userId);
                    cmd.Parameters.AddWithValue("@Visibility", visibility);
                    cmd.Parameters.AddWithValue("@Content", cnt);
                    cmd.Parameters.AddWithValue("@CreatedAt", createdAt);

                    cmd.ExecuteNonQuery();

                    // Hae AUTO_INCREMENT CommentId
                    CommentId = (int)cmd.LastInsertedId;
                }
            }

            Console.WriteLine("Comment created with ID: " + CommentId);
        }

        // Muokkaa kommentin sisältöä ja päivitä tietokantaan
        public void editComment(string newContent)
        {
            content = newContent;

            string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;";
            using (MySqlConnection connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();
                string query = @"UPDATE comment SET Content = @Content WHERE CommentId = @CommentId";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Content", newContent);
                    cmd.Parameters.AddWithValue("@CommentId", CommentId);
                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Comment " + CommentId + " updated.");
        }
    }
    public class Contract
        {
            public int contractId;
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

        // --- Users ---
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
                        string roleStr = reader.GetString("Role");
                        if (!Enum.TryParse<UserRole>(roleStr, true, out UserRole role))
                            role = UserRole.EXTERNAL;
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
                            string roleStr = reader.GetString("Role");
                            if (!Enum.TryParse<UserRole>(roleStr, true, out UserRole role))
                                role = UserRole.EXTERNAL;
                            return new User(id, name, email, role);
                        }
                    }
                }
            }
            return null;
        }

        // --- Contract Blocks ---
        public List<ContractBlock> GetAllBlocks()
        {
            List<ContractBlock> blocks = new List<ContractBlock>();
            using (MySqlConnection connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();
                string query = "SELECT BlockId, Title, Content FROM contractblocks";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32("BlockId");
                        string title = reader.GetString("Title");
                        string content = reader.GetString("Content");
                        blocks.Add(new ContractBlock(id, title, content));
                    }
                }
            }
            return blocks;
        }

        // --- Contracts ---
        public List<Contract> GetAllContracts()
        {
            List<Contract> contracts = new List<Contract>();
            using (MySqlConnection connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();
                string query = "SELECT ContractId, Title, ClientName, Status, CreatedBy FROM contracts";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32("ContractId");
                        string title = reader.GetString("Title");
                        string client = reader.GetString("ClientName");
                        string status = reader.GetString("Status");
                        int createdBy = reader.GetInt32("CreatedBy");
                        contracts.Add(new Contract(title, client, status, createdBy, id));
                    }
                }
            }
            return contracts;
        }

    }

        class MyApplication
        {
         private DataService myDataService;

        public MyApplication()
        {
            myDataService = new DataService();
        }

        // --- Users ---
        public string GetAllCustomers()
        {
            var customers = myDataService.GetAllCustomers();
            return string.Join("\n", customers.Select(c => c.ToString()));
        }

        public User GetCustomerDataByName(string userName)
        {
            return myDataService.GetUserByName(userName);
        }

        // --- Contract Blocks ---
        public string GetAllBlocks()
        {
            var blocks = myDataService.GetAllBlocks();
            return string.Join("\n", blocks.Select(b => $"BlockId {b.ToString()}"));
        }

        // --- Contracts ---
        public string GetAllContracts()
        {
            var contracts = myDataService.GetAllContracts();
            return string.Join("\n", contracts.Select(c => $"ContractId {c.ToString()}"));
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
            Console.WriteLine("3. Create user");
            Console.WriteLine("4. Delete user");
            Console.WriteLine("5. Update user");
            Console.WriteLine("6. Create contract block");
            Console.WriteLine("7. Copy contract block");
            Console.WriteLine("8. Update contract block");
            Console.WriteLine("9. Delete contract block");
            Console.WriteLine("10. Create contract");
            Console.WriteLine("11. Add block to contract");
            Console.WriteLine("12. Save contract");
            Console.WriteLine("13. Add comment to contract");
            Console.WriteLine("14. Delete contract");
            Console.WriteLine("exit (to finish)");
        }

        // --- Käyttäjät ---
        private void CreateUser()
        {
            Console.WriteLine("Enter username:");
            string username = Console.ReadLine();
            Console.WriteLine("Enter email:");
            string email = Console.ReadLine();
            Console.WriteLine("Enter role (INTERNAL/EXTERNAL/ADMIN):");
            string roleStr = Console.ReadLine();

            if (!Enum.TryParse<UserRole>(roleStr, true, out UserRole role))
                role = UserRole.EXTERNAL;

            User newUser = new User(0, username, email, role);
            newUser.AddUser();
            Console.WriteLine("User created.");
        }

        private void DeleteUser()
        {
            Console.WriteLine("Enter username to delete:");
            string username = Console.ReadLine();
            User u = myApp.GetCustomerDataByName(username);
            if (u != null)
            {
                u.deleteUser();
                Console.WriteLine("User deleted.");
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }

        private void UpdateUser()
        {
            Console.WriteLine("Enter username to update:");
            string username = Console.ReadLine();
            User u = myApp.GetCustomerDataByName(username);
            if (u != null)
            {
                Console.WriteLine("Enter new name:");
                string newName = Console.ReadLine();
                Console.WriteLine("Enter new email:");
                string newEmail = Console.ReadLine();
                u.editUser(newName, newEmail);
                Console.WriteLine("User updated.");
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }

        // --- Contract Block ---
        private void CreateContractBlock()
        {
            Console.WriteLine("Enter block title:");
            string title = Console.ReadLine();
            Console.WriteLine("Enter block content:");
            string content = Console.ReadLine();

            // BlockId = 0, koska AUTO_INCREMENT luo sen
            ContractBlock block = new ContractBlock(0, title, content);

            // CreatedBy = 1 tässä esimerkissä, mutta käytä kirjautuneen käyttäjän ID:tä
            block.SaveBlock(1);

            // Tulosta luodun lohkon ID
            Console.WriteLine("Contract block created with ID: " + block.blockId);
        }

        private void CopyContractBlock()
        {
            Console.WriteLine("Enter original block id:");
            int id = Convert.ToInt32(Console.ReadLine());

            // Tässä pitäisi hakea DB:stä oikea title/content, mutta esimerkissä kovakoodattu
            ContractBlock original = new ContractBlock(id, "origTitle", "origContent");

            // Luo kopio
            ContractBlock copy = original.CopyBlock(0, "Copy of " + original.ToString(), "Copied content");

            // Tallenna kopio
            copy.SaveBlock(1);

            // Tulosta uuden kopion ID
            Console.WriteLine("Block copied with ID: " + copy.blockId);
        }


        private void UpdateContractBlock()
        {
            Console.WriteLine("Enter block id:");
            int id = Convert.ToInt32(Console.ReadLine());
            ContractBlock block = new ContractBlock(id, "oldTitle", "oldContent"); // pitäisi hakea DB:stä
            Console.WriteLine("Enter new title:");
            string newTitle = Console.ReadLine();
            Console.WriteLine("Enter new content:");
            string newContent = Console.ReadLine();
            block.EditBlock(newTitle, newContent);
            block.UpdateBlock();
            Console.WriteLine("Block updated.");
        }

        private void DeleteContractBlock()
        {
            Console.WriteLine("Enter block id:");
            int id = Convert.ToInt32(Console.ReadLine());
            ContractBlock block = new ContractBlock(id, "", "");
            block.DeleteBlock();
            Console.WriteLine("Block deleted.");
        }

        // --- Contract ---
        private void CreateContract()
        {
            Console.WriteLine("Enter contract title:");
            string title = Console.ReadLine();
            Console.WriteLine("Enter client name:");
            string client = Console.ReadLine();
            Console.WriteLine("Enter status:");
            string status = Console.ReadLine();

            // Käyttäjä-ID (tässä kovakoodattu 1, mutta login-flow'ssa haetaan oikea)
            int currentUserId = 1;

            // ContractId annetaan 0, koska SaveContract luo sen AUTO_INCREMENT:llä
            Contract contract = new Contract(title, client, status, currentUserId, 0);

            // Tallennetaan sopimus
            contract.SaveContract();

            // Hae luodun rivin ID tietokannasta
            int newContractId = contract.contractId; // tämä pitää asettaa SaveContract-metodissa LastInsertedId:stä

            Console.WriteLine("Contract created with ID: " + newContractId);
        }


        private void AddBlockToContract()
        {
            Console.WriteLine("Enter contract id:");
            int contractId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter block id:");
            int blockId = Convert.ToInt32(Console.ReadLine());

            Contract contract = new Contract("title", "client", "status", 1, contractId);
            ContractBlock block = new ContractBlock(blockId, "title", "content");
            contract.addBlock(block, 0);
            Console.WriteLine("Block added to contract.");
        }

        private void SaveContract()
        {
            Console.WriteLine("Enter contract id:");
            int contractId = Convert.ToInt32(Console.ReadLine());
            Contract contract = new Contract("title", "client", "status", 1, contractId);
            contract.SaveContract();
            Console.WriteLine("Contract saved.");
        }

        private void AddCommentToContract()
        {
            Console.WriteLine("Enter contract id:");
            int contractId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter comment:");
            string comment = Console.ReadLine();

            // Esimerkkikäyttäjä, käytä kirjautuneen käyttäjän ID:tä oikeasti
            User user = new User(1, "TestUser", "test@mail.com", UserRole.INTERNAL);

            // Luo Contract-olio, jossa on oikea ContractId
            Contract contract = new Contract("title", "client", "status", user.userId, contractId);

            Comment c = new Comment();
            c.addComment(contract, user, comment, "Public"); // visibility esim. "Public"

            Console.WriteLine("Comment added with ID: " + c.CommentId);
        }


        private void DeleteContract()
        {
            Console.WriteLine("Enter contract id:");
            int contractId = Convert.ToInt32(Console.ReadLine());
            // tähän pitäisi toteuttaa DB-poisto
            Console.WriteLine($"Contract {contractId} deleted.");
        }

        // --- Run ---
        public void Run()
        {
            bool loggedIn = false;
            User currentUser = null;

            while (true)
            {
                if (!loggedIn)
                {
                    Console.Clear();
                    Console.WriteLine("Welcome! You must create or login before using contracts.");
                    Console.WriteLine("1. Show all customers");
                    Console.WriteLine("2. Create user");
                    Console.WriteLine("3. Login");
                    Console.WriteLine("exit (to finish)");

                    string command = Console.ReadLine();

                    switch (command)
                    {
                        case "1":
                            Console.WriteLine(myApp.GetAllCustomers());
                            break;
                        case "2":
                            CreateUser();
                            break;
                        case "3":
                            Console.WriteLine("Enter username:");
                            string username = Console.ReadLine();
                            currentUser = myApp.GetCustomerDataByName(username);
                            if (currentUser != null)
                            {
                                loggedIn = true;
                                Console.WriteLine($"Login successful! Welcome {currentUser.Username}");
                            }
                            else
                            {
                                Console.WriteLine("Login failed. User not found.");
                            }
                            break;
                        case "exit":
                            return;
                        default:
                            Console.WriteLine("Invalid input");
                            break;
                    }
                }
                else
                {
                    ShowMenu(); // tämä on se laajempi menu, jossa blockit ja contractit
                    string command = Console.ReadLine();

                    switch (command)
                    {
                        case "1": Console.WriteLine(myApp.GetAllCustomers()); break;
                        case "3": CreateUser(); break;
                        case "4": DeleteUser(); break;
                        case "5": UpdateUser(); break;
                        case "6": CreateContractBlock(); break;
                        case "7": CopyContractBlock(); break;
                        case "8": UpdateContractBlock(); break;
                        case "9": DeleteContractBlock(); break;
                        case "10": CreateContract(); break;
                        case "11": AddBlockToContract(); break;
                        case "12": SaveContract(); break;
                        case "13": AddCommentToContract(); break;
                        case "14": DeleteContract(); break;
                        case "15": Console.WriteLine(myApp.GetAllBlocks()); break;
                        case "16": Console.WriteLine(myApp.GetAllContracts()); break;
                        case "logout":
                            loggedIn = false;
                            currentUser = null;
                            Console.WriteLine("You have logged out.");
                            break;
                        case "exit":
                            return;
                        default:
                            Console.WriteLine("Invalid input");
                            break;
                    }
                }
            }
        }

    }

    //class UI
    //{
    //    MyApplication myApp = new MyApplication();

    //    public void ShowMenu()
    //    {
    //        Console.WriteLine("In this app you can (select with number):");
    //        Console.WriteLine("1. Show all customers");
    //        Console.WriteLine("2. Show data of one customer only");
    //        Console.WriteLine("exit (to finish)");
    //    }

    //    private void ShowListEnumerated(string[] stringList)
    //    {
    //        for (int i = 0; i < stringList.Length; i++)
    //            Console.WriteLine((i + 1) + ": " + stringList[i]);
    //    }

    //    private void ShowOneCustomer()
    //    {
    //        bool goOn = true;
    //        while (goOn)
    //        {
    //            Console.Clear();
    //            string[] customers = myApp.GetAllCustomers().Split('\n');

    //            if (customers.Length == 0)
    //            {
    //                Console.WriteLine("No customers available in the database");
    //                break;
    //            }

    //            ShowListEnumerated(customers);
    //            Console.WriteLine("Enter customer number:");

    //            try
    //            {
    //                int custNr = Convert.ToInt32(Console.ReadLine());
    //                if (custNr < 1 || custNr > customers.Length)
    //                    throw new Exception();

    //                string selectedName = customers[custNr - 1].Split(':')[1].Trim().Split(' ')[0];
    //                User cust = myApp.GetCustomerDataByName(selectedName);

    //                if (cust != null)
    //                {
    //                    Console.WriteLine($"ID: {cust.role} | Name: {cust.Username} | Email: {cust.Email}");
    //                }
    //                else
    //                {
    //                    Console.WriteLine("Customer not found.");
    //                }
    //            }
    //            catch
    //            {
    //                Console.WriteLine("Invalid input. Try again.");
    //            }

    //            Console.WriteLine("Want to see another customer data (Y/N)?");
    //            if (Console.ReadLine().ToUpper() != "Y")
    //                goOn = false;
    //        }
    //    }

    //    public void Run()
    //    {
    //        ShowMenu();
    //        string command = Console.ReadLine();

    //        while (true)
    //        {
    //            switch (command)
    //            {
    //                case "1":
    //                    Console.Clear();
    //                    Console.WriteLine(myApp.GetAllCustomers());
    //                    break;
    //                case "2":
    //                    Console.Clear();
    //                    ShowOneCustomer();
    //                    break;
    //                case "exit":
    //                    Console.WriteLine("Press any key to close the program");
    //                    Console.ReadLine();
    //                    return;
    //                default:
    //                    Console.WriteLine("Invalid input: You can only select from given options");
    //                    break;
    //            }
    //            ShowMenu();
    //            command = Console.ReadLine();
    //        }
    //    }







    internal class Program
        {
            static void Main(string[] args)
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8; // jos haluat ääkköset oikein

                UI ui = new UI();
                ui.Run();


           }





        }
    

    
}

