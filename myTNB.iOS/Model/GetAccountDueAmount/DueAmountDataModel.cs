using System.Collections.Generic;
using myTNB.DataManager;
using SQLite;

namespace myTNB.Model
{
    public class DueAmountDataModel
    {
        string _billDueDate = string.Empty;
        string _accNum = string.Empty;
        string _accNickName = string.Empty;

        [PrimaryKey]
        public string accNum
        {
            set
            {
                _accNum = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _accNum;
            }
        }

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

        #region Account fields
        public double IncrementREDueDateByDays
        {
            get;
            set;
        }

        public string accNickName
        {
            set
            {
                _accNickName = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _accNickName;
            }
        }

        public bool IsReAccount { set; get; }
        public bool IsNormalAccount { set; get; }
        public bool IsSSMR { set; get; }
        public bool IsOwnedAccount { set; get; }
        public bool IsPayEnabled { set; get; }
        public bool ShowEppToolTip { set; get; } //Created by Syahmi ICS 05052020

        public List<ItemisedBilling> ItemizedBillings { set; get; }
        public double OpenChargesTotal { set; get; }
        public bool IsItemisedBilling
        {
            get
            {
                return OpenChargesTotal > 0;
            }
        }

        public string WhyThisAmountLink { set; get; }
        public string WhyThisAmountTitle { set; get; }
        public string WhyThisAmountMessage { set; get; }
        public string WhyThisAmountPriButtonText { set; get; }
        public string WhyThisAmountSecButtonText { set; get; }

        #endregion

        /// <summary>
        /// Updates the values.
        /// </summary>
        /// <param name="model">Model.</param>
        public void UpdateValues(DueAmountDataModel model)
        {
            if (model != null)
            {
                // update all except nickname and account num (key)
                amountDue = model.amountDue;
                billDueDate = model.billDueDate;
                IncrementREDueDateByDays = model.IncrementREDueDateByDays;
            }
        }

        /// <summary>
        /// Updates the SSMR Flag
        /// </summary>
        /// <param name="model"></param>
        public void UpdateIsSSMRValue(SMRAccountStatusModel model)
        {
            if (model != null)
            {
                var flag = string.Compare(model.IsTaggedSMR.ToLower(), "true") == 0;
                IsSSMR = flag;

            }
        }
    }
}