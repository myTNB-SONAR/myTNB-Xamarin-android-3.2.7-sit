using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.Database.Model;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.ServiceDistruption.MVP
{
    public class ServiceDistruptionContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
        }

        public interface IUserActionsListener : IBasePresenter
        {
        }
    }
}