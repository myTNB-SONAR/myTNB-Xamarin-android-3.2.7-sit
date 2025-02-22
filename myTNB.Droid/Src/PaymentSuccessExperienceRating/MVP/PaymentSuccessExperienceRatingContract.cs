﻿using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.PaymentSuccessExperienceRating.Response;

namespace myTNB_Android.Src.PaymentSuccessExperienceRating.MVP
{
    public class PaymentSuccessExperienceRatingContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            void OnShowRating(int value);

            void ShowProgressDialog();

            void HideProgressDialog();

            void ShowSubmitRatingSuccess(SubmitExperienceRatingResponse response);

            void ShowSubmitRatingError(string error);

            void ShowErrorMessage(string error);

            void OnValidateSubmitRating();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            void OnRating(int value);

            void SubmitRating(string rating, string message, string ratingFor);
        }
    }
}