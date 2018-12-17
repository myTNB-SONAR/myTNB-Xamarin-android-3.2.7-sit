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

namespace myTNB_Android.Src.AddCard.MVP
{
    public class AddCardContract
    {
        
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Validate credit card details entered by user
            /// </summary>
            void ValidateCardDetails();

            /// <summary>
            /// Show error message for add card
            /// </summary>
            void ShowErrorMessage(string title, string message);

            /// <summary>
            /// Start scanner activity to scan card
            /// </summary>
            void OnScanCard();

        }

        public interface IUserActionsListener : IBasePresenter
        {

        }
    }
}