using Android.Text;
using myTNB_Android.Src.Utils;
using System;

namespace myTNB_Android.Src.GetAccess.MVP
{
    public class GetAccessFormPresenter : GetAccessFormContract.IUserActionsListener
    {
        private GetAccessFormContract.IView mView;

        public GetAccessFormPresenter(GetAccessFormContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnGetAccess(string icno, string maiden_name)
        {
            try
            {
                if (TextUtils.IsEmpty(icno))
                {
                    this.mView.ShowEmptyICNo();
                    return;
                }

                if (TextUtils.IsEmpty(maiden_name))
                {
                    this.mView.ShowEmptyMaidenName();
                    return;
                }
                this.mView.ShowSuccess();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void Start()
        {
            // NO IMPL
        }
    }
}