using System;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.Model;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class UserEntity : UserAuthenticationModel
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        public void CreateTable()
        {
            try
            {
                SQLiteHelper._db.CreateTable<UserEntity>();
                List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("UserEntity");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Create Table : {0}", e.Message);
            }
        }
        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void InsertItem(UserEntity item)
        {
            try
            {
                if (item != null)
                {
                    int newRecord = SQLiteHelper._db.InsertOrReplace(item);
#if DEBUG
                    Debug.WriteLine("Insert Record: {0}", newRecord);
#endif
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }
        /// <summary>
        /// Inserts the list of items.
        /// </summary>
        /// <param name="data">Data.</param>
        public void InsertListOfItems(UserAuthenticationModel data)
        {
            if (data != null && data != null)
            {
                UserEntity item = new UserEntity();
                item.userID = data.userID;
                item.displayName = data.displayName;
                item.userName = data.userName;
                item.email = data.email;
                item.dateCreated = data.dateCreated;
                item.lastLoginDate = data.lastLoginDate;
                item.isError = data.isError;
                item.message = data.message;
                item.identificationNo = data.identificationNo;
                item.mobileNo = data.mobileNo;
                InsertItem(item);
            }
        }
        /// <summary>
        /// Gets all items.
        /// </summary>
        /// <returns>The all items.</returns>
        public List<UserEntity> GetAllItems()
        {
            List<UserEntity> itemList = new List<UserEntity>();
            try
            {
                itemList = SQLiteHelper._db.Query<UserEntity>("select * from UserEntity");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Get All Items : {0}", e.Message);
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
                SQLiteHelper._db.DeleteAll<UserEntity>();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }
        /// <summary>
        /// Reset this instance.
        /// </summary>
        public void Reset()
        {
            DeleteTable();
            CreateTable();
        }
    }
}