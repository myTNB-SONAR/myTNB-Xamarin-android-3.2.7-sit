using System.Collections.Generic;
using Android.Graphics;
using myTNB.SitecoreCMS.Model;

namespace myTNB_Android.Src.RewardDetail.MVP
{
	public class RewardDetailContract
    {
		public interface IRewardDetailView
        {
            void SetRewardDetail(RewardsModel item);
            void SetRewardImage(Bitmap imgSrc);
		}

		public interface IRewardDetailPresenter
        {
            void FetchRewardImage(RewardsModel item);

            string BitmapToBase64(Bitmap bitmap);

            Bitmap Base64ToBitmap(string base64String);

            void GetActiveReward(string itemID);

            void UpdateRewardSave(string itemID, bool flag);

            void UpdateRewardUsed(string itemID, bool flag);

        }
	}
}
