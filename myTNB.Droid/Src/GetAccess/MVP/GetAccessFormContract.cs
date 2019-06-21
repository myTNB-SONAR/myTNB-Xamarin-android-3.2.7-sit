using myTNB_Android.Src.Base.MVP;

namespace myTNB_Android.Src.GetAccess.MVP
{
    public class GetAccessFormContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            void ShowEmptyICNo();

            void ShowEmptyMaidenName();

            void ClearErrors();

            void ShowSuccess();

        }

        public interface IUserActionsListener : IBasePresenter
        {
            void OnGetAccess(string icno, string maiden_name);
        }
    }
}