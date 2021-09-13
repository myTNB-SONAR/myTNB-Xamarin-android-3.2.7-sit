using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Database.Model;
using System.Threading.Tasks;

namespace myTNB_Android.Src.EBPopupScreen.MVP
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