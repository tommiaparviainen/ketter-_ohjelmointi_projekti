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
    public enum UserRole {INTERNAL, EXTERNAL, ADMIN }
    public enum Status { DRAFT, PENDING_APPROVAL, APPROVED, SENT_TO_CLIENT}

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
                            insertCmd.Parameters.AddWithValue("@Role", Role.ToString()); // tärkeä muutos!

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

    public class ContractBlock
    {
        public int blockId { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public int CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string Category { get; private set; } = "General";
        public bool IsPublic { get; private set; } = true;

        public ContractBlock(int BlockId, string title, string content)
        {
            blockId = BlockId;
            Title = title;
            Content = content;
        }

        public ContractBlock CopyBlock(int newId, string newTitle, string newContent)
        {
            return new ContractBlock(newId, newTitle, newContent);
        }

        public void EditBlock(string newTitle, string newContent)
        {
            Title = newTitle;
            Content = newContent;
        }

        public void SaveBlock(int createdBy)
        {
            string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;";
            using (MySqlConnection connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();
                string query = @"INSERT INTO contractblocks (Title, Content, Category, IsPublic, CreatedBy, CreatedAt)
                             VALUES (@Title, @Content, @Category, @IsPublic, @CreatedBy, @CreatedAt)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Title", Title);
                    cmd.Parameters.AddWithValue("@Content", Content);
                    cmd.Parameters.AddWithValue("@Category", Category);
                    cmd.Parameters.AddWithValue("@IsPublic", IsPublic);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    cmd.ExecuteNonQuery();
                    blockId = (int)cmd.LastInsertedId;
                }
            }
        }

        public void UpdateBlock()
        {
            string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;";
            using (MySqlConnection connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();
                string query = @"UPDATE contractblocks
                             SET Title=@Title, Content=@Content, Category=@Category, IsPublic=@IsPublic
                             WHERE BlockId=@BlockId";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Title", Title);
                    cmd.Parameters.AddWithValue("@Content", Content);
                    cmd.Parameters.AddWithValue("@Category", Category);
                    cmd.Parameters.AddWithValue("@IsPublic", IsPublic);
                    cmd.Parameters.AddWithValue("@BlockId", blockId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteBlock()
        {
            string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;";
            using (MySqlConnection connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();
                string query = "DELETE FROM contractblocks WHERE BlockId=@BlockId";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@BlockId", blockId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SetCategory(string category, bool isPublic)
        {
            Category = category;
            IsPublic = isPublic;
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
                string query = @"INSERT INTO comments (ContractId, AuthorId, Visibility, Content, CreatedAt)
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

    public class Approval
    {
        public int ApprovalId { get; set; }
        public int ContractId { get; private set; }
        public int ApproverId { get; private set; }
        public int ReviewerId { get; private set; }
        public string Decision { get; private set; }
        public string Note { get; private set; }
        public DateTime? DecidedAt { get; private set; }

        public Approval(int contractId, int approverId, int reviewerId, string note)
        {
            ContractId = contractId;
            ApproverId = approverId;
            ReviewerId = reviewerId;
            Decision = "PENDING";
            Note = note;
            DecidedAt = null;
        }

        public void SaveApproval()
        {
            string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;";
            using (var connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();
                string query = @"INSERT INTO approvals (ContractId, ApproverId, ReviewerId, Decision, Note, DecidedAt)
                             VALUES (@ContractId, @ApproverId, @ReviewerId, @Decision, @Note, @DecidedAt)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ContractId", ContractId);
                    cmd.Parameters.AddWithValue("@ApproverId", ApproverId);
                    cmd.Parameters.AddWithValue("@ReviewerId", ReviewerId == 0 ? (object)DBNull.Value : ReviewerId);
                    cmd.Parameters.AddWithValue("@Decision", Decision);
                    cmd.Parameters.AddWithValue("@Note", string.IsNullOrEmpty(Note) ? (object)DBNull.Value : Note);
                    cmd.Parameters.AddWithValue("@DecidedAt", DBNull.Value);

                    cmd.ExecuteNonQuery();


                    ApprovalId = (int)cmd.LastInsertedId;
                }
            }
            Console.WriteLine("Approval request created for contract " + ContractId);
        }


        public void SetStatus(string newDecision, string note = "")
        {
            Decision = newDecision;
            Note = note;

            string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;";
            using (var connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();
                string query = @"UPDATE approvals 
                         SET Decision=@Decision, Note=@Note, DecidedAt=@DecidedAt 
                         WHERE ApprovalId=@ApprovalId";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Decision", newDecision);
                    cmd.Parameters.AddWithValue("@Note", string.IsNullOrEmpty(note) ? (object)DBNull.Value : note);
                    cmd.Parameters.AddWithValue("@DecidedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ApprovalId", ApprovalId);

                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Approval " + ApprovalId + " updated to " + Decision);
        }
    }


    public class Contract
    {
        public int contractId;
        private string title;
        private string clientName;
        private Status status;
        private string content;
        private int createdBy;
        private DateTime createdAt;

        // Käytetään listaa taulukon sijaan
        public List<ContractBlock> contractBlocks = new List<ContractBlock>();

        public Contract(string tl, string name, Status st, int creator, int ConID)
        {
            title = tl;
            clientName = name;
            status = st ;
            createdBy = creator;
            createdAt = DateTime.Now;
            contractId = ConID;
        }

        public void addBlock(ContractBlock block, int order)
        {
            contractBlocks.Add(block);
        }

        public void SaveContract(int userId)
        {
            string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;";
            using (MySqlConnection connection = new MySqlConnection(MySqlCon))
            {
                connection.Open();
                string insertContract = @"INSERT INTO contracts (Title, ClientName, Status, CreatedBy, CreatedAt)
                          VALUES (@Title, @ClientName, @Status, @CreatedBy, @CreatedAt)";

                using (MySqlCommand cmd = new MySqlCommand(insertContract, connection))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@ClientName", clientName);
                    cmd.Parameters.AddWithValue("@Status", status.ToString());
                    cmd.Parameters.AddWithValue("@CreatedBy", userId);   // sama nimi kuin SQL:ssä
                    cmd.Parameters.AddWithValue("@CreatedAt", createdAt);

                    cmd.ExecuteNonQuery();
                    contractId = (int)cmd.LastInsertedId;


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

                    using (MySqlCommand cmd = new MySqlCommand(blockQuery, connection))
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

                            string rawContent = reader.GetString("Content");
                            string content = rawContent.Replace("\r", "")
                                                       .Replace("\n", " ")
                                                       .Replace("\t", " ");

                            blocks.Add(new ContractBlock(id, title, content));
                        }
                    }
                }
                return blocks;
            }

            public List<ContractBlock> GetBlocksForUser(UserRole role)
            {
                var blocks = new List<ContractBlock>();
                using (var connection = new MySqlConnection(MySqlCon))
                {
                    connection.Open();
                    string query = role == UserRole.EXTERNAL
                        ? "SELECT BlockId, Title, Content FROM contractblocks WHERE IsPublic = 1"
                        : "SELECT BlockId, Title, Content FROM contractblocks";
                    using (var cmd = new MySqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
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
                            string statusStr = reader.GetString("Status");
                            int createdBy = reader.GetInt32("CreatedBy");

                            // Parsitaan string -> enum
                            Status status;
                            if (!Enum.TryParse<Status>(statusStr, true, out status))
                            {
                                status = Status.DRAFT; // fallback jos DB:ssä outo arvo
                            }

                            contracts.Add(new Contract(title, client, status, createdBy, id));
                        }
                    }
                }
                return contracts;
            }


            public void LinkBlockToContract(int contractId, int blockId, int sortOrder)
            {
                using (var connection = new MySqlConnection(MySqlCon))
                {
                    connection.Open();
                    string q = @"INSERT INTO contract_contractblock (ContractId, BlockId, SortOrder)
                     VALUES (@ContractId, @BlockId, @SortOrder)";
                    using (var cmd = new MySqlCommand(q, connection))
                    {
                        cmd.Parameters.AddWithValue("@ContractId", contractId);
                        cmd.Parameters.AddWithValue("@BlockId", blockId);
                        cmd.Parameters.AddWithValue("@SortOrder", sortOrder);
                        cmd.ExecuteNonQuery();
                    }
                }
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
            private int reviewerId;

            public object ApprovalId { get; private set; }
            private User currentUser;
            private void ShowMenu(User currentUser)
            {
                Console.WriteLine("Select an option:");

                if (currentUser.role == UserRole.ADMIN)
                {
                    Console.WriteLine("1. Show all customers");
                    Console.WriteLine("3. Create user");
                    Console.WriteLine("4. Delete user");
                    Console.WriteLine("5. Update user");
                }

                if (currentUser.role == UserRole.INTERNAL)
                {
                    Console.WriteLine("6. Create contract block");
                    Console.WriteLine("7. Copy contract block");
                    Console.WriteLine("8. Update contract block");
                    Console.WriteLine("9. Delete contract block");
                    Console.WriteLine("10. Create contract");
                    Console.WriteLine("11. Add block to contract");
                    Console.WriteLine("12. Save contract");
                    Console.WriteLine("13. Add comment to contract");
                    Console.WriteLine("17. Request approval for contract");
                    Console.WriteLine("18. Approve/Reject approval request");
                }

                if (currentUser.role == UserRole.EXTERNAL)
                {
                    Console.WriteLine("16. Show contract by ID");
                    Console.WriteLine("13. Add comment to contract");
                }

                Console.WriteLine("logout (to sign out)");
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

                UserRole role;
                switch (roleStr.Trim().ToUpper())
                {
                    case "INTERNAL":
                    case "INT":
                        role = UserRole.INTERNAL;
                        break;
                    case "EXTERNAL":
                    case "EXT":
                        role = UserRole.EXTERNAL;
                        break;
                    case "ADMIN":
                    case "ADM":
                        role = UserRole.ADMIN;
                        break;
                    default:
                        Console.WriteLine("Invalid role entered. Defaulting to EXTERNAL.");
                        role = UserRole.EXTERNAL;
                        break;
                }

                User newUser = new User(0, username, email, role);
                newUser.AddUser();
                Console.WriteLine($"User created with role: {role}");
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
                block.SaveBlock(currentUser.userId);

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

                Console.WriteLine("Enter status (DRAFT / PENDING_APPROVAL / APPROVED / SENT_TO_CLIENT):");
                string statusStr = Console.ReadLine();

                // Parsitaan käyttäjän syöte enumiksi
                Status status;
                if (!Enum.TryParse<Status>(statusStr, true, out status))
                {
                    Console.WriteLine("Invalid status entered. Defaulting to DRAFT.");
                    status = Status.DRAFT;
                }

                // Käyttäjä-ID (tässä kovakoodattu 1, mutta login-flow'ssa haetaan oikea)
                int currentUserId = 1;

                // ContractId annetaan 0, koska SaveContract luo sen AUTO_INCREMENT:llä
                Contract contract = new Contract(title, client, status, currentUserId, 0);

                // Tallennetaan sopimus
                contract.SaveContract(currentUser.userId);

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

                Contract contract = new Contract("title", "client", Status.DRAFT, 1, contractId);
                ContractBlock block = new ContractBlock(blockId, "title", "content");
                contract.addBlock(block, 0);
                Console.WriteLine("Block added to contract.");
            }

            private void SaveContract()
            {
                Console.WriteLine("Enter contract id:");
                int contractId = Convert.ToInt32(Console.ReadLine());
                Contract contract = new Contract("title", "client", Status.DRAFT, currentUser.userId, contractId);
                Console.WriteLine("Contract saved.");
            }

            private void AddCommentToContract()
            {
                Console.WriteLine("Enter contract id:");
                int contractId = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter comment:");
                string comment = Console.ReadLine();

                User user = new User(1, "TestUser", "test@mail.com", UserRole.INTERNAL);

                Contract contract = new Contract("title", "client", Status.DRAFT, user.userId, contractId);

                Comment c = new Comment();
                c.addComment(contract, user, comment, "Public");

                Console.WriteLine("Comment added with ID: " + c.CommentId);
            }

         

            private void DeleteContract()
            {
                Console.WriteLine("Enter contract id:");
                int contractId = Convert.ToInt32(Console.ReadLine());
                // tähän pitäisi toteuttaa DB-poisto
                Console.WriteLine($"Contract {contractId} deleted.");
            }
            private void ShowContractById()
            {
                Console.WriteLine("Enter contract id:");
                int contractId = Convert.ToInt32(Console.ReadLine());

                string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;";
                using (MySqlConnection connection = new MySqlConnection(MySqlCon))
                {
                    connection.Open();

                    // --- 1. Hae sopimuksen tiedot ---
                    string contractQuery = @"SELECT ContractId, Title, ClientName, Status, CreatedBy, CreatedAt 
                                 FROM contracts WHERE ContractId=@ContractId";
                    using (MySqlCommand cmd = new MySqlCommand(contractQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ContractId", contractId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine("Contract:");
                                Console.WriteLine($"ID: {reader.GetInt32(0)}");
                                Console.WriteLine($"Title: {reader.GetString(1)}");
                                Console.WriteLine($"Client: {(reader.IsDBNull(2) ? "(no client)" : reader.GetString(2))}");
                                Console.WriteLine($"Status: {reader.GetString(3)}");
                                Console.WriteLine($"CreatedBy: {reader.GetInt32(4)}");
                                Console.WriteLine($"CreatedAt: {reader.GetDateTime(5)}");
                            }
                            else
                            {
                                Console.WriteLine("Contract not found.");
                                return;
                            }
                        }
                    }

                    // --- 2. Hae sopimukseen liittyvät blockit ---
                    string blocksQuery = @"SELECT b.BlockId, b.Title, b.Content, b.Category, b.IsPublic
                            FROM contract_blocks cb
                            JOIN contractblocks b ON cb.BlockId = b.BlockId
                            WHERE cb.ContractId=@ContractId;";
                    using (MySqlCommand cmd = new MySqlCommand(blocksQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ContractId", contractId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\n Blocks: ");
                            while (reader.Read())
                            {
                                Console.WriteLine($"Block {reader.GetInt32(0)}: {reader.GetString(1)} ({reader.GetString(3)}, Public={reader.GetBoolean(4)})");
                                Console.WriteLine($"   Content: {reader.GetString(2)}");
                            }
                        }
                    }


                    // --- 3. Hae sopimukseen liittyvät kommentit ---
                    string commentsQuery = @"SELECT CommentId, AuthorId, Visibility, Content, CreatedAt 
                                 FROM comments WHERE ContractId=@ContractId";
                    using (MySqlCommand cmd = new MySqlCommand(commentsQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ContractId", contractId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\nComments:");
                            while (reader.Read())
                            {
                                Console.WriteLine($"Comment {reader.GetInt32(0)} by User {reader.GetInt32(1)} ({reader.GetString(2)}):");
                                Console.WriteLine($"   {reader.GetString(3)}");
                                Console.WriteLine($"   At {reader.GetDateTime(4)}");
                            }
                        }
                    }
                }
            }
            public void ShowAllBlocks()
            {
                string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;Charset=utf8mb4;";
                using (MySqlConnection connection = new MySqlConnection(MySqlCon))
                {
                    connection.Open();
                    string query = "SELECT BlockId, Title, Content, Category, IsPublic, CreatedBy, CreatedAt FROM contractblocks";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("All contract blocks:");
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string title = reader.GetString(1);
                            string content = reader.GetString(2).Replace("\r", "").Replace("\n", " ");
                            string category = reader.GetString(3);
                            bool isPublic = reader.GetBoolean(4);
                            int createdBy = reader.GetInt32(5);
                            DateTime createdAt = reader.GetDateTime(6);

                            Console.WriteLine($"Block {id}: {title} ({category}, Public={isPublic}) by {createdBy} at {createdAt}");
                            Console.WriteLine($"   Content: {content}");
                        }
                    }
                }
            }



            //-- approvals--
            // -- approvals --
            private void RequestApproval()
            {
                Console.WriteLine("Enter contract id:");
                int contractId = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Enter approver user id:");
                int approverId = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Enter reviewer user id (or 0 if none):");
                int reviewerId = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Enter note:");
                string note = Console.ReadLine();

                Approval approval = new Approval(contractId, approverId, reviewerId, note);
                approval.SaveApproval();

                Console.WriteLine("Approval created with ID: " + approval.ApprovalId);
            }

            private void ApproveOrReject()
            {
                Console.WriteLine("Enter approval id:");
                int approvalId = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Enter new status (APPROVED/REJECTED):");
                string status = Console.ReadLine();

                Console.WriteLine("Enter note:");
                string note = Console.ReadLine();

                // Dummy approval, mutta asetetaan ID
                Approval approval = new Approval(0, 0, 0, "");
                approval.ApprovalId = approvalId;
                approval.SetStatus(status, note);
            }

            public void SetStatus(string decision, string note)
            {
                string MySqlCon = "Server=127.0.0.1;Port=3306;Database=uusiyritys;Uid=root;Pwd=;Charset=utf8mb4;";
                using (var connection = new MySqlConnection(MySqlCon))
                {
                    connection.Open();

                    // 1. Päivitä approvals-taulu
                    string updateApproval = @"UPDATE approvals 
                                  SET Decision=@Decision, Note=@Note, DecidedAt=NOW() 
                                  WHERE ApprovalId=@ApprovalId";
                    using (var cmd = new MySqlCommand(updateApproval, connection))
                    {
                        cmd.Parameters.AddWithValue("@Decision", decision.ToUpper());
                        cmd.Parameters.AddWithValue("@Note", note);
                        cmd.Parameters.AddWithValue("@ApprovalId", this.ApprovalId);
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Hae ContractId
                    int contractId;
                    using (var cmd = new MySqlCommand("SELECT ContractId FROM approvals WHERE ApprovalId=@ApprovalId", connection))
                    {
                        cmd.Parameters.AddWithValue("@ApprovalId", this.ApprovalId);
                        contractId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // 3. Päivitä contracts-taulun status
                    string updateContract = @"UPDATE contracts SET Status=@Status WHERE ContractId=@ContractId";
                    using (var cmd = new MySqlCommand(updateContract, connection))
                    {
                        cmd.Parameters.AddWithValue("@Status", decision.ToUpper()); // esim. APPROVED tai REJECTED
                        cmd.Parameters.AddWithValue("@ContractId", contractId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }


            // --- Run ---
            public void Run()
            {
                bool loggedIn = false;
                currentUser = null;

                while (true)
                {
                    bool hasUsers = myApp.GetAllCustomers().Any();

                    if (!loggedIn)
                    {
                        Console.Clear();
                        Console.WriteLine("Welcome! You must login before using contracts.");
                        if (!hasUsers)
                        {
                            Console.WriteLine("1. Create first admin user");
                        }
                        else
                        {
                            Console.WriteLine("1. Login");
                        }
                        Console.WriteLine("exit (to finish)");

                        string command = Console.ReadLine();

                        if (!hasUsers && command == "1")
                        {
                            // Luo ensimmäinen admin
                            Console.WriteLine("Enter username:");
                            string username = Console.ReadLine();
                            Console.WriteLine("Enter email:");
                            string email = Console.ReadLine();
                            User admin = new User(0, username, email, UserRole.ADMIN);
                            admin.AddUser();
                            Console.WriteLine("First admin user created. Please login.");
                        }
                        else if (hasUsers && command == "1")
                        {
                            Console.WriteLine("Enter username:");
                            string username = Console.ReadLine();
                            currentUser = myApp.GetCustomerDataByName(username);
                            if (currentUser != null)
                            {
                                loggedIn = true;
                                Console.WriteLine($"Login successful! Welcome {currentUser.Username} (Role: {currentUser.role})");
                            }
                            else
                            {
                                Console.WriteLine("Login failed. User not found.");
                            }
                        }
                        else if (command == "exit")
                        {
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input");
                        }
                    }

                    else
                    {
                        ShowMenu(currentUser); // välitetään kirjautunut käyttäjä

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
                            case "15": ShowAllBlocks(); break;
                            case "16": ShowContractById(); break;
                            case "17": RequestApproval(); break;
                            case "18": ApproveOrReject(); break;
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

}