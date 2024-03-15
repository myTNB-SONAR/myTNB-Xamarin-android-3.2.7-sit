using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.Database.Model;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.EBPopupScreen.MVP
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