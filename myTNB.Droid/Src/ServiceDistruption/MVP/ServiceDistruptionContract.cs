using myTNB.Android.Src.Base.MVP;
using myTNB.Android.Src.Database.Model;
using System.Threading.Tasks;

namespace myTNB.Android.Src.ServiceDistruption.MVP
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