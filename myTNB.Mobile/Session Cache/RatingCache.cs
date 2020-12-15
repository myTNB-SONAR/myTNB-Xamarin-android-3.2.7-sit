using System;
using myTNB.Mobile.Extensions;

namespace myTNB.Mobile.SessionCache
{
    public sealed class RatingCache
    {
        private static readonly Lazy<RatingCache> lazy
            = new Lazy<RatingCache>(() => new RatingCache());

        public static RatingCache Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private RatingCache() { }

        private int Rating = 0;
        private string RatingSuccessToast = string.Empty;

        public void SetRating(int rating)
        {
            if (rating > 0)
            {
                Rating = rating;
            }
        }

        public void SetRatingToast(string message)
        {
            if (message.IsValid())
            {
                RatingSuccessToast = message;
            }
            else
            {
                RatingSuccessToast = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "rateSuccessMessage");
            }
        }

        public string GetRating()
        {
            if (Rating > 0)
            {
                return string.Format("{0}{1}"
                    , LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "youRated")
                    , Rating.ToString());
            }
            return string.Empty;
        }

        public string GetRatingToastMessage()
        {
            if (RatingSuccessToast.IsValid())
            {
                return RatingSuccessToast;
            }
            return string.Empty;
        }

        public void Clear()
        {
            Rating = 0;
            RatingSuccessToast = string.Empty;
        }
    }
}