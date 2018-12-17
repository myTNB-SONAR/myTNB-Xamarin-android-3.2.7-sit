using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
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