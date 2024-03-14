using System.Collections.Generic;
using myTNB.Android.Src.Base.Models;
using myTNB.Android.Src.Base.MVP;

namespace myTNB.Android.Src.Enquiry.GSL.MVP.GSLSubmittedDetails
{
    public class GSLRebateSubmittedDetailsContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Setting Up Layout
            /// </summary>
            void SetUpViews();

            void RenderAttachments(List<AttachedImage> attachedImages);

            void ShowProgressDialog();

            void HideProgressDialog();

            void RenderUIFromModel(GSLRebateModel model);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Initialization in the Presenter
            /// </summary>
            void OnInitialize(SubmittedFeedbackDetails details);

            void PrepareDataAsync();
        }
    }
}
