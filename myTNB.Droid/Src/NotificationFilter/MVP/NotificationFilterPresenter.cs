using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.NotificationFilter.Models;
using myTNB.Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB.Android.Src.NotificationFilter.MVP
{
    public class NotificationFilterPresenter : NotificationFilterContract.IUserActionsListener
    {

        private NotificationFilterContract.IView mView;

        public NotificationFilterPresenter(NotificationFilterContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnSelectFilterItem(NotificationFilterData notificationFilterData, int position)
        {
            try
            {
                NotificationFilterEntity.UnSelectAll();
                int row = NotificationFilterEntity.SelectItem(notificationFilterData.Id);
                if (row > 0)
                {
                    this.mView.ShowSelectedFilterItem(notificationFilterData, position);
                }
                else
                {
                    // TODO : SHOW ERROR CANNOT SELECT
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void Start()
        {
            try
            {
                //
                List<NotificationFilterEntity> entityList = NotificationFilterEntity.List();
                List<NotificationFilterData> filterList = new List<NotificationFilterData>();
                foreach (NotificationFilterEntity entity in entityList)
                {
                    filterList.Add(NotificationFilterData.Get(entity));
                }
                this.mView.ShowNotificationList(filterList);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}