using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.ManageCards.Models;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Models;
using Refit;

namespace myTNB.AndroidApp.Src.RegisterValidation.MVP
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
