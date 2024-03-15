using System.Collections.Generic;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Base.MVP;

namespace myTNB.AndroidApp.Src.Enquiry.GSL.MVP.GSLSubmittedDetails
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
