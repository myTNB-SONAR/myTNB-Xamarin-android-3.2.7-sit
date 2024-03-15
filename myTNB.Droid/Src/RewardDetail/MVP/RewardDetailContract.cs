using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.NewAppTutorial.MVP;

namespace myTNB.AndroidApp.Src.RewardDetail.MVP
{
	public class RewardDetailContract
    {
		public interface IRewardDetailView
        {
            void SetRewardDetail(RewardsModel item);
            void SetRewardImage(Bitmap imgSrc);

            void OnCountDownReward();

            void ShowProgressDialog();

            void HideProgressDialog();

            void ShowRetryPopup();

        }

		public interface IRewardDetailPresenter
        {
            void FetchRewardImage(RewardsModel item);

            string BitmapToBase64(Bitmap bitmap);

            Bitmap Base64ToBitmap(string base64String);

            void GetActiveReward(string itemID);

            void UpdateRewardSave(string itemID, bool flag);

            Task UpdateRewardUsed(string itemID);

            List<string> ExtractUrls(string text);

            List<NewAppModel> OnGeneraNewAppTutorialList();

            Bitmap ToGrayscale(Bitmap srcImage);

        }
	}
}
