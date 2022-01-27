using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Database.Model;
using System.Threading.Tasks;

namespace myTNB_Android.Src.ServiceDistruption.MVP
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