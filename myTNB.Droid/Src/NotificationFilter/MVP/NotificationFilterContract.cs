using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.NotificationFilter.Models;

namespace myTNB_Android.Src.NotificationFilter.MVP
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
            void ShowSelectedFilterItem(NotificationFilterData notificationFilterData , int position);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to navigate to filter notifications
            /// </summary>
            /// <param name="notificationFilterData">NotificationFilterData</param>
            /// <param name="position">integer</param>
            void OnSelectFilterItem(NotificationFilterData notificationFilterData , int position);
        }
    }
}