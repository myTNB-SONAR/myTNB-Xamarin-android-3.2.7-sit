using System;
using System.Collections.Generic;
using myTNB.Android.Src.Base.MVP;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.ManageCards.Models;
using myTNB.Android.Src.MultipleAccountPayment.Models;
using Refit;

namespace myTNB.Android.Src.RegisterValidation.MVP
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
