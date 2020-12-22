using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.LogUserAccess.Models;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.LogUserAccess.MVP
{
    public class LogUserAccessPresenter : LogUserAccessContract.IUserActionsListener
    {

        private LogUserAccessContract.IView mView;

        public LogUserAccessPresenter(LogUserAccessContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnSelectFilterItem(LogUserAccessData notificationFilterData, int position)
        {
            try
            {
                /*NotificationFilterEntity.UnSelectAll();
                int row = NotificationFilterEntity.SelectItem(notificationFilterData.Id);
                if (row > 0)
                {
                    this.mView.ShowSelectedFilterItem(notificationFilterData, position);
                }
                else
                {
                    // TODO : SHOW ERROR CANNOT SELECT
                }*/
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
                List<LogUserAccessEntity> entityList = LogUserAccessEntity.List();
                List<LogUserAccessData> filterList = new List<LogUserAccessData>();
                foreach (LogUserAccessEntity entity in entityList)
                {
                    filterList.Add(LogUserAccessData.Get(entity));
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