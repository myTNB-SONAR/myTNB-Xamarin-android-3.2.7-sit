using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class SaveEnergyBudgetRequest : BaseRequest
    {
        public string AccountNo;
        public int BudgetAmount;

        public SaveEnergyBudgetRequest(string AccNo, int budgetAmount)
        {
            this.BudgetAmount = budgetAmount;
            this.AccountNo = AccNo;
        }
    }
}
