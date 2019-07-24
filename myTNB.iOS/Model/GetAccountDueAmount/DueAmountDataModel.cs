﻿using System.Collections.Generic;
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

        public bool IsReAccount
        {
            get;
            set;
        }

        public bool IsNormalAccount
        {
            get;
            set;
        }

        public bool IsSSMR
        {
            get;
            set;
        }

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
            // update all except nickname and account num (key)
            amountDue = model.amountDue;
            billDueDate = model.billDueDate;
            IncrementREDueDateByDays = model.IncrementREDueDateByDays;
        }
    }
}
