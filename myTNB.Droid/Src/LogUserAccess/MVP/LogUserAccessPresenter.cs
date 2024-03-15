using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.LogUserAccess.Models;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.LogUserAccess.MVP
{
    public class LogUserAccessPresenter : LogUserAccessContract.IUserActionsListener
    {

        private LogUserAccessContract.IView mView;
        private static DateTime ThisweekStartDate;
        private static DateTime ThisweekEndDate;


        public LogUserAccessPresenter(LogUserAccessContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public static bool DateInsideOneWeek(DateTime date2)
        {
            DateTime referenceDate;
            referenceDate = DateTime.Now;

            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;

            var d1 = referenceDate.Date.AddDays(-1 * (int)cal.GetDayOfWeek(referenceDate));
            var d2 = date2.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date2));

            return d1 == d2;
        }

        public static bool DateInsideLastWeek(DateTime date2)
        {
            DateTime referenceDate;
            referenceDate = DateTime.Now;

            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;

            var d1 = referenceDate.Date.AddDays(-1 * (int)cal.GetDayOfMonth(referenceDate));
            var d2 = date2.Date.AddDays(-1 * (int)cal.GetDayOfMonth(date2));

            return d1 == d2;
        }

        public static bool DateInsideLastMonth(DateTime date2)
        {
            return true;
        }

        public void SortLogListDataByDate(List<LogUserAccessNewData> loglist)
        {
            try
            {
                this.mView.ShowProgressDialog();

                List<LogUserAccessNewData> thisWeek = new List<LogUserAccessNewData>();
                List<LogUserAccessNewData> lastWeek = new List<LogUserAccessNewData>();
                List<LogUserAccessNewData> lastMonth = new List<LogUserAccessNewData>();
                List<LogUserAccessNewData> newloglist = new List<LogUserAccessNewData>();

                /*if (loglist.Count > 15)
                {
                    newloglist = loglist.GetRange(0, 15);
                    this.mView.FirstLoadData(15);
                }
                else
                {
                    newloglist = loglist;
                }*/

                foreach (LogUserAccessNewData entity in loglist)
                {
                    if (DateInsideOneWeek(entity.CreatedDate.Date))
                    {
                        thisWeek.Add(entity);
                    }
                    else if (DateInsideLastWeek(entity.CreatedDate.Date))
                    {
                        lastWeek.Add(entity);
                    }
                    else if (DateInsideLastMonth(entity.CreatedDate.Date))
                    {
                        lastMonth.Add(entity);
                    }
                }

                if (thisWeek.Count > 0)
                {
                    this.mView.ShowLogListThisWeek(thisWeek);
                }
                else
                {
                    this.mView.emptyThisWeekList();
                }

                if (lastWeek.Count > 0)
                {
                    this.mView.ShowLogListLastWeek(lastWeek);
                }
                else
                {
                    this.mView.emptyLastWeekList();
                }

                if (lastMonth.Count > 0)
                {
                    this.mView.ShowLogList(lastMonth);
                }
                else
                {
                    this.mView.emptyLastMonthList();
                }

                this.mView.HideShowProgressDialog();

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void Start()
        {
            //throw new NotImplementedException();
        }
    }
}