using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Import namespace OleDb for databases (outside class)
using System.Data.OleDb;
//System.Data for command object
using System.Data;


namespace perunanryostajat
{
    class DataService
    {
        private OleDbConnection myConnection;

        public DataService()
        {
            //In class method(s), create and open connection
            //This can be done either once (e.g. Page_Load) for each
            //page request, or separately every time db connection is required
            String connstr;
            //set the path here acording to the location of database folder
            String projectPath = @"C:\Users\kkyyr\Downloads\";
            connstr = "Provider = Microsoft.ACE.OLEDB.12.0;" + @"Data Source = " +
            projectPath + @"C:\Users\kkyyr\Downloads\peruna.sql";
            /*- Määrittää tiedostopolun tietokantaan
            - Luo OleDbConnection-olion ja avaa yhteyden
            - Käyttää Microsoft Access -provideria (Microsoft.ACE.OLEDB.12.0)
            */
            //OleDbConnection requires namespace System.Data.OleDb
            myConnection = new OleDbConnection();
            myConnection.ConnectionString = connstr;
            myConnection.Open();

            /*

             * 
             * Luokka DataService:
            - Luo ja avaa yhteyden Access-tietokantaan (CustomerOrders2019.accdb)
            - Tarjoaa kolme metodia tietojen hakemiseen:
            - GetData: hakee kaikki rivit tietyistä kentistä
            - GetDataWhereString: hakee rivit, joissa kenttä vastaa tiettyä arvoa
            - GetDataWhereBetween: hakee rivit, joissa kentän arvo on tietyllä välillä
            */
        }

        private OleDbDataReader GetData(string[] fields, string table)
        {
            /*- Rakentaa SQL-kyselyn tyyliin:
                SELECT Field1, Field2 FROM TableName
                - Palauttaa OleDbDataReader-olion, jolla voi lukea tulokset
                */
            OleDbCommand myCommand = new OleDbCommand();

            myCommand.Connection = myConnection;
            //SQL query string
            myCommand.CommandText = "SELECT ";

            foreach (string s in fields)
                myCommand.CommandText += s + ", ";

            myCommand.CommandText = myCommand.CommandText.Remove(myCommand.CommandText.LastIndexOf(","));
            myCommand.CommandText += " FROM " + table;
            //CommandType requires namespace System.Data
            myCommand.CommandType = CommandType.Text;

            //Execute the SQL request command and
            //store the output in myReader object
            OleDbDataReader myReader;
            myReader = myCommand.ExecuteReader();

            return myReader;
        }

        private OleDbDataReader GetDataWhereString(string[] fields, string table, string keyField, string keyValue)
        {/*- Rakentaa SQL-kyselyn tyyliin:
        SELECT Field1, Field2 FROM TableName WHERE KeyField = 'KeyValue'
        - Soveltuu tekstimuotoisiin ehtoihin (esim. asiakasnimi = "Matti")
        */
            OleDbCommand myCommand = new OleDbCommand();

            myCommand.Connection = myConnection;
            //SQL query string
            myCommand.CommandText = "SELECT ";

            foreach (string s in fields)
                myCommand.CommandText += s + ", ";

            myCommand.CommandText = myCommand.CommandText.Remove(myCommand.CommandText.LastIndexOf(","));
            myCommand.CommandText += " FROM " + table;

            myCommand.CommandText += " WHERE " + keyField + "='" + keyValue + "';";
            //CommandType requires namespace System.Data
            myCommand.CommandType = CommandType.Text;


            //CommandType requires namespace System.Data
            myCommand.CommandType = CommandType.Text;

            //Execute the SQL request command and
            //store the output in myReader object
            OleDbDataReader myReader;
            myReader = myCommand.ExecuteReader();

            return myReader;
        }

        private OleDbDataReader GetDataWhereBetween(string[] fields, string table, string keyField, double minValue, double maxValue)
        {
            /*- Rakentaa SQL-kyselyn tyyliin:
        SELECT Field1, Field2 FROM TableName WHERE KeyField BETWEEN minValue AND maxValue
        - Soveltuu numeerisiin ehtoihin (esim. tilauksen summa välillä 100–500)
        */
            OleDbCommand myCommand = new OleDbCommand();

            myCommand.Connection = myConnection;
            //SQL query string
            myCommand.CommandText = "SELECT ";

            foreach (string s in fields)
                myCommand.CommandText += s + ", ";

            myCommand.CommandText = myCommand.CommandText.Remove(myCommand.CommandText.LastIndexOf(","));
            myCommand.CommandText += " FROM " + table;

            myCommand.CommandText += " WHERE " + keyField + " BETWEEN " + minValue + " AND " + maxValue + ";";
            //CommandType requires namespace System.Data
            myCommand.CommandType = CommandType.Text;


            //CommandType requires namespace System.Data
            myCommand.CommandType = CommandType.Text;

            //Execute the SQL request command and
            //store the output in myReader object
            OleDbDataReader myReader;
            myReader = myCommand.ExecuteReader();

            return myReader;
        }
    }
        //--------------------------------------------------------------------------------------------------------------------------------------------------

        //kaikki enumit tässä samassa

        enum UserRole
        {
            INTERNAL,
            EXTERNAL,
            ADMIN
        }

        enum ContractStatus
        {
            DRAFT,
            PENDING_APPROVAL,
            APPROVED,
            SENT_TO_CLIENT
        }

        enum StakeholderRole
        {
            VIEWER,
            EDITOR,
            APPROVER
        }

        enum Visibility
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
        private bool IsInternal { get; set; }
        private UserRole Role { get; set; }

        public void CreateUser(int id, string name, string mail)
        {
            userID = id;
            username = name;
            email = mail;
            
        }

        public void editUser( string newName, string newEmail)
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

        public void editContract( string newTitle)
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

         public void RemoveBlock(int categoryId)
        {
           
        }


    }

    public class ContractStakeHolder
    {
        private int stakeHolderID{ get; set; }
        private Contract contract { get; set; }
        private User user { get; set; }
        //private StakeHolderRole role;

        public void assignStakeHolder(int id, Contract c, User u)
        {
            stakeHolderID = id;
            contract = c;
            user = u;
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
        private bool visibility { get; set; }
        public void addComment(int id, Contract c, User u, string cnt, DateTime date)
        {

           commentID = id;
           Contract contract = c;
           User user = u;
           content = cnt;
           createdAt = date;
          
           
        }

        

        public void editComment(string newContent)
        {
            content = newContent;
            
        }

    }

    internal class Program
    {
        static void Main(string[] args)
        {
            //User user = new User();
            //user.CreateUser(1, "Kaisa", "kaisa@example.com");

            //// 2. Luo sopimus
            //Contract contract = new Contract();
            //contract.createContract(100, "Työsopimus");

            //// 3. Luo lohkokategoria ja lisää sopimukseen
            //BlockCategory category = new BlockCategory();
            //category.createCategory(10, "Yleiset ehdot", "Sisältää yleiset sopimusehdot");
            //contract.addBlock(category);

            //// 4. Luo sopimuslohko
            ////ContractBlock block = new ContractBlock();
            ////block.CreateBlock(200, "Palkkaus", "Palkka on 3000€/kk");

            //// 5. Sijoita lohko sopimukseen
            //ContractBlockAssigment assignment = new ContractBlockAssigment();
            //assignment.assignBlock(500, contract, //block, 1, "Muokattava versio: Palkka 3200€/kk");

            //// 6. Lisää sidoshenkilö
            //ContractStakeHolder stakeholder = new ContractStakeHolder();
            //stakeholder.assignStakeHolder(600, contract, user);

            //// 7. Hyväksyntä
            //Approval approval = new Approval();
            //approval.approve(700, contract, user, DateTime.Now);

            //// 8. Kommentti
            //Comment comment = new Comment();
            //comment.addComment(800, contract, user, "Voidaanko palkka neuvotella?", DateTime.Now);

            //// Tulostuksia testin vuoksi
            //Console.WriteLine("Testiohjelma suoritettu onnistuneesti.");

        }
    }
}
