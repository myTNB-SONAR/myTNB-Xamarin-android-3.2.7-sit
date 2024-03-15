using myTNB.AndroidApp.Src.AppLaunch.Models;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.SelectFeedbackType.MVP
{
    public class SelectFeedbackTypePresenter : SelectFeedbackTypeContract.IUserActionsListener
    {
        private SelectFeedbackTypeContract.IView mView;

        public SelectFeedbackTypePresenter(SelectFeedbackTypeContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnSelect(FeedbackType feedbackType)
        {
            try
            {
                FeedbackTypeEntity.RemoveActive();
                FeedbackTypeEntity.SetSelected(feedbackType.FeedbackTypeId);

                this.mView.ShowSelectedSuccess(feedbackType);
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
                var list = new List<FeedbackType>();
                var entityList = FeedbackTypeEntity.GetActiveList();
                foreach (FeedbackTypeEntity entity in entityList)
                {
                    list.Add(new FeedbackType()
                    {
                        FeedbackTypeId = entity.Id,
                        FeedbackTypeName = entity.Name,
                        IsSelected = entity.IsSelected
                    });
                }
                this.mView.ShowList(list);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}