using System;
using System.Collections.Generic;
using myTNB.Model;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class UserAccountsEntity : CustomerAccountRecordModel
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        public void CreateTable()
        {
            SQLiteHelper._db.CreateTable<UserAccountsEntity>();
            List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("UserAccountsEntity");
        }
        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void InsertItem(UserAccountsEntity item)
        {
            try
            {
                int newRecord = SQLiteHelper._db.InsertOrReplace(item);
                Console.WriteLine("Insert Record: {0}", newRecord);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }
        /// <summary>
        /// Inserts the list of items.
        /// </summary>
        /// <param name="itemList">Item list.</param>
        public void InsertListOfItems(CustomerAccountRecordListModel itemList)
        {
            if (itemList != null && itemList.d != null && itemList.d.Count > 0)
            {
                foreach (CustomerAccountRecordModel obj in itemList.d)
                {
                    UserAccountsEntity item = new UserAccountsEntity();
                    item.__type = obj.__type;
                    item.accNum = obj.accNum;
                    item.userAccountID = obj.userAccountID;
                    item.accDesc = obj.accDesc;
                    item.icNum = obj.icNum;
                    item.amCurrentChg = obj.amCurrentChg;
                    item.isRegistered = obj.isRegistered;
                    item.isPaid = obj.isPaid;
                    item.isError = obj.isError;
                    item.message = obj.message;
                    item.isOwned = obj.isOwned;
                    item.isLocal = obj.isLocal;
                    item.accountTypeId = obj.accountTypeId;
                    item.accountStAddress = obj.accountStAddress;
                    item.accountNickName = obj.accountNickName;
                    item.accountCategoryId = obj.accountCategoryId;
                    item.ownerName = obj.ownerName;
                    item.smartMeterCode = obj.smartMeterCode;
                    InsertItem(item);
                }
            }
        }
        /// <summary>
        /// Gets all items.
        /// </summary>
        /// <returns>The all items.</returns>
        List<UserAccountsEntity> GetAllItems()
        {
            List<UserAccountsEntity> itemList = new List<UserAccountsEntity>();
            try
            {
                itemList = SQLiteHelper._db.Query<UserAccountsEntity>("select * from UserAccountsEntity order by accountCategoryId desc, accountNickName collate nocase asc");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }
        /// <summary>
        /// Deletes the table.
        /// </summary>
        public void DeleteTable()
        {
            try
            {
                SQLiteHelper._db.DeleteAll<UserAccountsEntity>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }
        /// <summary>
        /// Gets the customer account record list.
        /// </summary>
        /// <returns>The customer account record list.</returns>
        public CustomerAccountRecordListModel GetCustomerAccountRecordList()
        {
            List<UserAccountsEntity> userAccountsEntityList = GetAllItems();
            CustomerAccountRecordListModel customerAccountList = new CustomerAccountRecordListModel();
            customerAccountList.d = new List<CustomerAccountRecordModel>();
            if (userAccountsEntityList != null && userAccountsEntityList.Count > 0)
            {
                foreach (UserAccountsEntity obj in userAccountsEntityList)
                {
                    CustomerAccountRecordModel item = new CustomerAccountRecordModel();
                    item.__type = obj.__type;
                    item.accNum = obj.accNum;
                    item.userAccountID = obj.userAccountID;
                    item.accDesc = obj.accDesc;
                    item.icNum = obj.icNum;
                    item.amCurrentChg = obj.amCurrentChg;
                    item.isRegistered = obj.isRegistered;
                    item.isPaid = obj.isPaid;
                    item.isError = obj.isError;
                    item.message = obj.message;
                    item.isOwned = obj.isOwned;
                    item.isLocal = obj.isLocal;
                    item.accountTypeId = obj.accountTypeId;
                    item.accountStAddress = obj.accountStAddress;
                    item.accountNickName = obj.accountNickName;
                    item.accountCategoryId = obj.accountCategoryId;
                    item.ownerName = obj.ownerName;
                    item.smartMeterCode = obj.smartMeterCode;
                    customerAccountList.d.Add(item);
                }
            }
            return customerAccountList;
        }
    }
}