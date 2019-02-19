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
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.SelectFeedbackState.MVP
{
    public class SelectFeedbackStatePresenter : SelectFeedbackStateContract.IUserActionsListener
    {
        private SelectFeedbackStateContract.IView mView;

        public SelectFeedbackStatePresenter(SelectFeedbackStateContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnSelect(FeedbackState feedbackState)
        {
            try {
            FeedbackStateEntity.RemoveActive();
            FeedbackStateEntity.SetSelected(feedbackState.StateId);

            this.mView.ShowSelectedSuccess(feedbackState);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void Start()
        {
            try {
            var list = new List<FeedbackState>();
            var entityList = FeedbackStateEntity.GetActiveList();
            foreach (FeedbackStateEntity entity in entityList)
            {
                list.Add(new FeedbackState()
                {
                    StateId = entity.Id,
                    StateName = entity.Name,
                    IsSelected = entity.IsSelected
                });
            }
            List<FeedbackState> SortedList = list.OrderBy(o => o.StateName).ToList();
            this.mView.ShowList(SortedList);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}