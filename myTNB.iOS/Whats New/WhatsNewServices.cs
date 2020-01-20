using System;
using System.Diagnostics;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;

namespace myTNB
{
    public static class WhatsNewServices
    {
        public static bool FilterExpiredWhatsNew()
        {
            bool isExpired = false;
            WhatsNewEntity whatsNewEntity = new WhatsNewEntity();
            var list = whatsNewEntity.GetAllItems();
            if (list != null && list.Count > 0)
            {
                foreach (var whatsNew in list)
                {
                    if (WhatsNewHasExpired(whatsNew))
                    {
                        isExpired = true;
                        whatsNewEntity.DeleteItem(whatsNew.ID);
                    }
                }
            }
            return isExpired;
        }

        public static bool WhatsNewHasExpired(WhatsNewModel whatsNew)
        {
            bool res = true;
            if (whatsNew != null && whatsNew.ID.IsValid())
            {
                if (whatsNew.EndDate.IsValid())
                {
                    var rewardEndDate = DateHelper.GetDateWithoutSeparator(whatsNew.EndDate);
                    if (rewardEndDate != default(DateTime))
                    {
                        DateTime now = DateTime.Now.Date;
                        if (now < rewardEndDate)
                        {
                            res = false;
                        }
                    }
                }
            }
            return res;
        }

        public static string GetPublishedDate(string publishedDate)
        {
            string strPublishedDate = string.Empty;
            try
            {
                DateTime? pDate = DateHelper.GetDateWithoutSeparator(publishedDate);
                DateTime? pDateValue = pDate.Value.ToLocalTime();
                strPublishedDate = pDateValue.Value.ToString(WhatsNewConstants.Format_Date, DateHelper.DateCultureInfo);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Parse Error: " + e.Message);
            }
            return strPublishedDate;
        }
    }
}
