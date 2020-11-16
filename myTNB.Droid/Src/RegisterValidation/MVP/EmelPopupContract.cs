using System;
using System.Collections.Generic;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.MultipleAccountPayment.Models;
using Refit;

namespace myTNB_Android.Src.RegisterValidation.MVP
{
    public class EmelPopupContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {

        }

        public interface IUserActionsListener : IBasePresenter
        {

        }
    }
}
