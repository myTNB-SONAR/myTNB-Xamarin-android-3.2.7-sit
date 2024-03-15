using myTNB.AndroidApp.Src.Base.MVP;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.DigitalBill.MVP
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
        }


        public interface IUserActionsListener : IBasePresenter
        {
            
        }

    }
}