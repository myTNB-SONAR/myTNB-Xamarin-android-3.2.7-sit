using myTNB.Android.Src.Base.MVP;
using myTNB.Android.Src.Database.Model;
using System.Threading.Tasks;

namespace myTNB.Android.Src.EBPopupScreen.MVP
{
    public class EBPopupScreenContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
        }

        public interface IUserActionsListener : IBasePresenter
        {
        }
    }
}