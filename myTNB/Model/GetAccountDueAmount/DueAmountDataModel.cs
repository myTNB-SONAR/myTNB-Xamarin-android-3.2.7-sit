using myTNB.DataManager;

namespace myTNB.Model
{
    public class DueAmountDataModel
    {
        string _billDueDate = string.Empty;

        public double amountDue { set; get; }
        public string billDueDate
        {
            set
            {
                _billDueDate = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _billDueDate;
            }
        }
    }
}