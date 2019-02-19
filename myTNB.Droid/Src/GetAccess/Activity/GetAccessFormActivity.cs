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
using myTNB_Android.Src.Base.Activity;
using Android.Content.PM;
using CheeseBind;
using myTNB_Android.Src.Utils;
using Android.Support.Design.Widget;
using myTNB_Android.Src.GetAccess.MVP;
using myTNB_Android.Src.GetAccessSuccess.Activity;
using Newtonsoft.Json;
using myTNB_Android.Src.myTNBMenu.Models;

namespace myTNB_Android.Src.GetAccess.Activity
{
    [Activity(Label = "@string/get_access_activity_title"
     , ScreenOrientation = ScreenOrientation.Portrait
     , Theme = "@style/Theme.GetAccess")]
    public class GetAccessFormActivity : BaseToolbarAppCompatActivity , GetAccessFormContract.IView
    {
        [BindView(Resource.Id.txtInputLayoutICNo)]
        TextInputLayout txtInputLayoutICNo;

        [BindView(Resource.Id.txtInputLayoutMaidenName)]
        TextInputLayout txtInputLayoutMaidenName;

        [BindView(Resource.Id.txtIcNo)]
        EditText txtIcNo;

        [BindView(Resource.Id.txtMaidenName)]
        EditText txtMaidenName;

        [BindView(Resource.Id.btnGetAccess)]
        Button btnGetAccess;

        AccountData selectedAccount;

        private GetAccessFormPresenter mPresenter;
        private GetAccessFormContract.IUserActionsListener userActionsListener;

        public void ClearErrors()
        {
            txtInputLayoutICNo.Error = null;
            txtInputLayoutMaidenName.Error = null;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.GetAccessFormView;
        }

        public void SetPresenter(GetAccessFormContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowEmptyICNo()
        {
            txtInputLayoutICNo.Error = GetString(Resource.String.get_access_form_empty_ic_no_error);
        }

        public void ShowEmptyMaidenName()
        {
            txtInputLayoutMaidenName.Error = GetString(Resource.String.get_access_form_empty_maiden_name_error);
        }

        public void ShowSuccess()
        {
            // SHOW SUCCESS VIEW
            Intent access_form_success = new Intent(this , typeof(GetAccessSuccessActivity));
            access_form_success.PutExtra(Constants.SELECTED_ACCOUNT , JsonConvert.SerializeObject(selectedAccount));
            StartActivity(access_form_success);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.mPresenter = new GetAccessFormPresenter(this);

            TextViewUtils.SetMuseoSans300Typeface(txtIcNo , txtMaidenName);
            TextViewUtils.SetMuseoSans500Typeface(btnGetAccess);

            selectedAccount = JsonConvert.DeserializeObject<AccountData>(Intent.Extras.GetString(Constants.SELECTED_ACCOUNT));
        }
        [OnClick(Resource.Id.btnGetAccess)]
        void OnBtnGetAccess(object sender, EventArgs eventArgs)
        {
            string icno = txtIcNo.Text;
            string maiden_name = txtMaidenName.Text;
            this.userActionsListener.OnGetAccess(icno , maiden_name);
        }
    }
}