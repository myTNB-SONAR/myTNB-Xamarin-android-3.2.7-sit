using Refit;

namespace myTNB.AndroidApp.Src.AddAccount.Models
{
    public class AddAccountResquest
    {

        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        [AliasAs("userID")]
        public string userID { get; set; }

        [AliasAs("email")]
        public string email { get; set; }

        [AliasAs("tnbBillAccountNum")]
        public string tnbBillAccountNum { get; set; }

        [AliasAs("tnbAccountHolderICNum")]
        public string tnbAccountHolderICNum { get; set; }

        [AliasAs("tnbAccountContractNum")]
        public string tnbAccountContractNum { get; set; }

        [AliasAs("type")]
        public string type { get; set; }

        [AliasAs("description")]
        public string description { get; set; }

        [AliasAs("isOwner")]
        public bool isOwner { get; set; }

        [AliasAs("suppliedMotherName")]
        public string suppliedMotherName { get; set; }

        public AddAccountResquest(string apiKeyID, string userID, string email, string tnbBillAcctNo, string tnbAcctHolderIC, string tnbAcctContractNo, string type, string des, bool isOwner, string suppliedMotherName)
        {
            this.apiKeyID = apiKeyID;
            this.userID = userID;
            this.email = email;
            this.tnbBillAccountNum = tnbBillAccountNum;
            this.tnbAccountHolderICNum = tnbAccountHolderICNum;
            this.tnbAccountContractNum = tnbAccountContractNum;
            this.type = type;
            this.description = des;
            this.isOwner = isOwner;
            this.suppliedMotherName = suppliedMotherName;
        }
    }
}