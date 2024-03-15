using System;

namespace myTNB.AndroidApp.Src.Base.MVP
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