using myTNB_Android.Src.Base.MVP;
using System.Threading.Tasks;

namespace myTNB_Android.Src.DigitalBill.MVP
{
    public class DigitalBillContract
    {

        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show terms and condition service sucess
            /// </summary>
            /// <param name="success"></param>
            void ShowDigitalBill(bool success);

            void HideProgressBar();

            void OnSavedTimeStamp(string savedTimeStamp);

            void ShowDigitalBillTimestamp(bool success);
        }


        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Call sitecore service to get terms and conditions 
            /// On success it will save data into local DB
            /// </summary>
            /// <returns></returns>
            Task GetDigitalBillData();

            void GetSavedDigitalBillTimeStamp();

            Task OnGetDigitalBillTimeStamp();
        }

    }
}