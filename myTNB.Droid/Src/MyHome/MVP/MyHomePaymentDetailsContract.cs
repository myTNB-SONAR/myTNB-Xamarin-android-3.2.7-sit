using System;
using Android.OS;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.MyTNBService.Model;
using System.Collections.Generic;
using myTNB_Android.Src.MyHome.Model;

namespace myTNB_Android.Src.MyHome.MVP
{
	public class MyHomePaymentDetailsContract
	{
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Set up Views
            /// </summary>
            void SetUpViews();

            /// <summary>
            /// Updates Account Info UI
            /// </summary>
            void UpdateAccountInfoUI();

            /// <summary>
            /// Updates My Charges UI
            /// </summary>
            void UpdateMyChargesUI();

            /// <summary>
            /// Updates Payments Method UI
            /// </summary>
            void UpdatePaymentsMethodUI();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Initializes variables
            /// </summary>
            void OnInitialize(Bundle bundle);

            /// <summary>
            /// Gets the MyHomePaymentDetailsModel data model
            /// </summary>
            /// <returns></returns>
            MyHomePaymentDetailsModel GetMyHomePaymentDetailsModel();

            /// <summary>
            /// Gets the AccountCharge data model
            /// </summary>
            /// <returns></returns>
            AccountChargeModel GetAccountChargeModel();
        }
    }
}

