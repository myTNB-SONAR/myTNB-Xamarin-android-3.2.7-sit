namespace myTNB.Mobile.AWS.Models.DBR
{
    public class PostInstallationDetailsRequest
    {
        public InstallationDetailsModel InstallationDetails { set; get; }
    }

    public class InstallationDetailsModel
    {
        public string ContractAccount { set; get; }
    }
}