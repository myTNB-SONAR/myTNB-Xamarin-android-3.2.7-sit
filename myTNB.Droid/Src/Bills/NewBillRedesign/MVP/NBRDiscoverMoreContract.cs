using System;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Bills.NewBillRedesign.Model;

namespace myTNB_Android.Src.Bills.NewBillRedesign.MVP
{
    public class NBRDiscoverMoreContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Setting Up Layout
            /// </summary>
            void SetUpViews();

            /// <summary>
            /// Render content in the view
            /// </summary>
            /// <param name="model"></param>
            void RenderContent(NBRDiscoverMoreModel model);

            /// <summary>
            /// Updates the View for different states (Loading and Filled)
            /// </summary>
            /// <param name="isLoading"></param>
            void UpdateView(bool isLoading);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Initialization in the Presenter
            /// </summary>
            void OnInitialize();

            /// <summary>
            /// Method for triggering actions when screen has started
            /// </summary>
            void OnStart();
        }
    }
}

