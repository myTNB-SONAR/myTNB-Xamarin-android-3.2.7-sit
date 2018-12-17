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

namespace myTNB_Android.Src.Base.MVP
{
    public interface IBaseView<T>
    {
        /// <summary>
        /// Allows to pass the presenter into the view
        /// </summary>
        /// <param name="userActionListener">the generic object that represents the user action listener</param>
        void SetPresenter(T userActionListener);

        /// <summary>
        /// Determines if the view is active and visible
        /// </summary>
        /// <returns>boolean representation of active and visible</returns>
        Boolean IsActive();
    }
}