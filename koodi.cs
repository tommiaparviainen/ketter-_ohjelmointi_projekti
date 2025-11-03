using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace perunanryostajat
{
    public class User
    {
        private int userID;
        private string username;
        private string email;

        public void createUser(int id, string name, string mail)
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
            userID = 0;
            username = null;
            email = null;

        }
    }

    public class BlockCategory
    {
        public int categoryId;
        private string name;
        private string description;

        public void createCategory(int ID, string nm, string desc)
        {
            categoryId = ID;
            name = nm;
            description = desc;
        }

        public void editCategory(int ID, string nm, string desc)
        {
            categoryId = ID;
            name = nm;
            description = desc;
        }

    }







   

    public class ContractBlock
    {
        private int blockID { get; set; }
        private string title { get; set; }
            private string content { get; set; }

            public void createBlock(int block, string tit, string cont)
        {

            blockID = block;
            title = tit;
            content = cont;

        }

        public void copyBlock(int newBlockID, string newTitle, string newContent)
        {
            blockID = newBlockID;
            title = newTitle;
            content = newContent;
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
        private int assignmentID;
        private Contract contract;
        private ContractBlock block;
        private int orderNumber;
        private string editableCopy;

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
        private int contractId;
        private string title;
        public List<BlockCategory> Blocks { get; private set; } = new List<BlockCategory>();

        

        public void createContract(int Id, string tl)
        {
            contractId = Id;
            title = tl;
        }

        public void editContract(int Id, string tl)
        {
            contractId = Id;
            title = tl;
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
        private int stakeHolderID;
        private Contract contract;
        private User user; 
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
        private int approvalID;
        private Contract contract;
        private User user;
        private DateTime approvedAt;
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
        private int commentID;
        private Contract contract;
        private User user;
        private string content;
        private DateTime createdAt;
        private bool visibility;
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
            User user = new User();
            user.createUser(1, "Kaisa", "kaisa@example.com");

            // 2. Luo sopimus
            Contract contract = new Contract();
            contract.createContract(100, "Työsopimus");

            // 3. Luo lohkokategoria ja lisää sopimukseen
            BlockCategory category = new BlockCategory();
            category.createCategory(10, "Yleiset ehdot", "Sisältää yleiset sopimusehdot");
            contract.addBlock(category);

            // 4. Luo sopimuslohko
            ContractBlock block = new ContractBlock();
            block.createBlock(200, "Palkkaus", "Palkka on 3000€/kk");

            // 5. Sijoita lohko sopimukseen
            ContractBlockAssigment assignment = new ContractBlockAssigment();
            assignment.assignBlock(500, contract, block, 1, "Muokattava versio: Palkka 3200€/kk");

            // 6. Lisää sidoshenkilö
            ContractStakeHolder stakeholder = new ContractStakeHolder();
            stakeholder.assignStakeHolder(600, contract, user);

            // 7. Hyväksyntä
            Approval approval = new Approval();
            approval.approve(700, contract, user, DateTime.Now);

            // 8. Kommentti
            Comment comment = new Comment();
            comment.addComment(800, contract, user, "Voidaanko palkka neuvotella?", DateTime.Now);

            // Tulostuksia testin vuoksi
            Console.WriteLine("Testiohjelma suoritettu onnistuneesti.");

        }
    }
}
