using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.LogUserAccess.Models;
using System.Collections.Generic;

namespace myTNB_Android.Src.LogUserAccess.MVP
{
    public class LogUserAccessContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show logUserAccessData list
            /// </summary>
            /// <param name="logUserAccess">List<paramref name="logUserAccessData"/></param>
            void ShowLogList(List<LogUserAccessNewData> LogUserAccessData);
         
            void emptyThisWeekList();
           
            void emptyLastWeekList();
          
            void emptyLastMonthList();
         
            void ShowLogListThisWeek(List<LogUserAccessNewData> thisWeek);
         
            void ShowLogListLastWeek(List<LogUserAccessNewData> lastWeek);
         
            void HideShowProgressDialog();
         
            void FirstLoadData(int v);
         
            void ShowProgressDialog();
        }

        public interface IUserActionsListener : IBasePresenter
        {

            void SortLogListDataByDate(List<LogUserAccessNewData> loglist);
        }
    }
}