using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Common;
using myTNB_Android.Src.Common.Model;

namespace myTNB_Android.Src.Enquiry.GSL.MVP
{
    public class GSLRebateStepOneContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Setting Up Layout
            /// </summary>
            void SetUpViews();

            void SetSelectedCountry(Country country);

            void UpdateSelectedRebateType(Item item);

            void ShowEmptyError(GSLLayoutType layoutType);

            void ClearErrors(GSLLayoutType layoutType);

            void UpdateButtonState(bool isEnabled);

            bool IsMobileNumEmpty();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Initialization in the Presenter
            /// </summary>
            void OnInitialize();

            void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data);

            List<Item> GetRebateTypeList();

            Item GetDefaultSelectedRebateType();

            void SetTenantFullName(string name);

            void SetTenantEmailAddress(string email);

            void SetTenantMobileNumber(string number);

            bool CheckRequiredFields();

            GSLRebateModel GetGSLRebateModel();
        }
    }
}
