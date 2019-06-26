using System.Collections.Generic;
using myTNB.DataManager;
using myTNB.SQLite.SQLiteDataManager;
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

        public List<ItemisedBilling> ItemizedBillings { set; get; }
        public double OpenChargesTotal { set; get; }
        public bool IsItemisedBilling
        {
            get
            {
                return OpenChargesTotal > 0;
            }
        }

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

        /// <summary>
        /// Converts to an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        public DueEntity ToEntity()
        {
            var entity = new DueEntity();
            entity.accNickName = accNickName;
            entity.accNum = accNum;
            entity.amountDue = amountDue;
            entity.billDueDate = billDueDate;
            entity.IncrementREDueDateByDays = IncrementREDueDateByDays;
            entity.IsReAccount = IsReAccount;

            return entity;
        }

        /// <summary>
        /// Updates from an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public void UpdateFromEntity(DueEntity entity)
        {
            accNickName = entity.accNickName;
            accNum = entity.accNum;
            amountDue = entity.amountDue;
            billDueDate = entity.billDueDate;
            IncrementREDueDateByDays = entity.IncrementREDueDateByDays;
            IsReAccount = entity.IsReAccount;
        }

    }
}
