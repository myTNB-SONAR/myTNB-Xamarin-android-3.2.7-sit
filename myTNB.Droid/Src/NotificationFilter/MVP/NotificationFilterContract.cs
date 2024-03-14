using myTNB.Android.Src.Base.MVP;
using myTNB.Android.Src.NotificationFilter.Models;
using System.Collections.Generic;

namespace myTNB.Android.Src.NotificationFilter.MVP
{
    public class NotificationFilterContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show notification list
            /// </summary>
            /// <param name="notificationFilters">List<paramref name="NotificationFilterData"/></param>
            void ShowNotificationList(List<NotificationFilterData> notificationFilters);

            /// <summary>
            /// Show filtered notification data
            /// </summary>
            /// <param name="notificationFilterData">NotificationFilterData</param>
            /// <param name="position">integer</param>
            void ShowSelectedFilterItem(NotificationFilterData notificationFilterData, int position);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to navigate to filter notifications
            /// </summary>
            /// <param name="notificationFilterData">NotificationFilterData</param>
            /// <param name="position">integer</param>
            void OnSelectFilterItem(NotificationFilterData notificationFilterData, int position);
        }
    }
}