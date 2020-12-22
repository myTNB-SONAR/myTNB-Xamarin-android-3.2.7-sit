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
            void ShowNotificationList(List<LogUserAccessData> LogUserAccessData);

            /// <summary>
            /// Show logUserAccessData data
            /// </summary>
            /// <param name="logUserAccessData">logUserAccessData</param>
            /// <param name="position">integer</param>
            void ShowSelectedFilterItem(LogUserAccessData logUserAccessData, int position);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to navigate to filter logUserAccessData
            /// </summary>
            /// <param name="logUserAccessData">logUserAccessData</param>
            /// <param name="position">integer</param>
            void OnSelectFilterItem(LogUserAccessData logUserAccessData, int position);
        }
    }
}