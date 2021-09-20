using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Net;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using Castle.Core.Internal;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Barcode.Activity;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.Common.Activity;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Feedback_Login_BillRelated.Activity;
using myTNB_Android.Src.Feedback_Prelogin_NewIC.Model;
using myTNB_Android.Src.Feedback_Prelogin_NewIC.MVP;
using myTNB_Android.Src.FeedbackFail.Activity;
using myTNB_Android.Src.FeedbackGeneralEnquiryStepOne.Activity;
using myTNB_Android.Src.FeedbackSuccess.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.OverVoltageClaim.Activity;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.UpdatePersonalDetailStepOne.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web;



namespace myTNB_Android.Src.Feedback_Prelogin_NewIC.Activity
{
    [Activity(Label = "Submit New Enquiry"
  , ScreenOrientation = ScreenOrientation.Portrait
          , WindowSoftInputMode = SoftInput.AdjustResize
  , Theme = "@style/Theme.FaultyStreetLamps")]


    public class FeedbackPreloginNewICActivity : BaseToolbarAppCompatActivity, FeedbackPreloginNewICContract.IView, View.IOnTouchListener
    {


        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.generalEnquiryConstraint)]
        ConstraintLayout generalEnquiryConstraint;

        [BindView(Resource.Id.updatePersonalInfoConstraint)]
        ConstraintLayout updatePersonalInfoConstraint;

        [BindView(Resource.Id.overvoltageclaimConstraint)]
        ConstraintLayout overvoltageclaimConstraint;


        [BindView(Resource.Id.txtAccountNo)]
        EditText txtAccountNo;

        [BindView(Resource.Id.txtInputLayoutAccountNo)]
        TextInputLayout txtInputLayoutAccountNo;

        [BindView(Resource.Id.infoLabeltxtWhereIsMyAcc)]
        TextView infoLabeltxtWhereIsMyAcc;

        [BindView(Resource.Id.infoLabelWhereIsMyAcc)]
        LinearLayout infoLabelWhereIsMyAcc;


        [BindView(Resource.Id.howCanWeHelpYou)]
        TextView howCanWeHelpYou;

        [BindView(Resource.Id.txtGeneralEnquiry)]
        TextView txtGeneralEnquiry;

        [BindView(Resource.Id.txtGeneralEnquiry_subContent)]
        TextView txtGeneralEnquiry_subContent;

        [BindView(Resource.Id.txtUpdatePersonal)]
        TextView txtUpdatePersonal;

        [BindView(Resource.Id.txtUpdatePersonalContent)]
        TextView txtUpdatePersonalContent;

        [BindView(Resource.Id.txtOverVoltageClaim)]
        TextView txtOverVoltageClaim;

        [BindView(Resource.Id.txtOverVoltageClaimContent)]
        TextView txtOverVoltageClaimContent;

        [BindView(Resource.Id.updatePersoanlInfoIcon1)]
        ImageView updatePersoanlInfoIcon1;

        [BindView(Resource.Id.updatePersoanlInfoIcon2)]
        ImageView updatePersoanlInfoIcon2;

        [BindView(Resource.Id.infoLabel)]
        TextView InfoLabel;


        [BindView(Resource.Id.scanNewEnquiry)]
        ImageButton scanNewEnquiry;


        [BindView(Resource.Id.accountLayout4)]
        FrameLayout accountLayout4;


        String GeneralEnquiry1of2_app_bar = "@string/bill_related_activity_title";

        FrameLayout rootview;

        private bool isAccChoosed = false;


        FeedbackPreloginNewICContract.IUserActionsListener userActionsListener;
        FeedbackPreloginNewICPresenter mPresenter;
        AccountData selectedAccount;


        private bool isClicked = false;
        string AccNoDesc;

        private ISharedPreferences mSharedPref;
        public bool overvoltageClaimVisible = false;
        public bool overvoltageClaimEnabled = false;
        public bool IsWhiteListedArea = false;
        public bool IsPilot = false;
        public bool OvisUnderMaintenance = false;
        public bool IsServerDown = false;
        CAVerifyResponseModel CANumberVerifyResponce;
        List<CustomerBillingAccount> accountList;
        List<string> contactAccountNumbers;
        OVISRequest listData;

        MyTNBAppToolTipBuilder leaveDialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {               
                //Contract Account List
                accountList = CustomerBillingAccount.List();
                contactAccountNumbers = new List<string>();
                foreach (var CANumber in accountList)
                {
                    var no = CANumber.AccNum;
                    contactAccountNumbers.Add(no);
                }
                listData = new OVISRequest();
                listData.contactAccountNumbers = contactAccountNumbers;
                //Verify CA number
                CANumberVerification(listData);

                //init shared preferences 
                mSharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
                //1 set presenter
                mPresenter = new FeedbackPreloginNewICPresenter(this, mSharedPref);



                // Intent intent = Intent;
                SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "submitEnquiryTitle"));
                //2 set font type , 300 normal 500 button
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutAccountNo);
                TextViewUtils.SetMuseoSans300Typeface(txtUpdatePersonalContent, txtOverVoltageClaimContent, txtGeneralEnquiry_subContent, txtAccountNo);
                TextViewUtils.SetMuseoSans500Typeface(infoLabeltxtWhereIsMyAcc, howCanWeHelpYou, txtGeneralEnquiry, txtUpdatePersonal, txtOverVoltageClaim);


                txtAccountNo.TextSize = TextViewUtils.GetFontSize(16f);
                infoLabeltxtWhereIsMyAcc.TextSize = TextViewUtils.GetFontSize(12f);
                howCanWeHelpYou.TextSize = TextViewUtils.GetFontSize(16f);
                txtGeneralEnquiry.TextSize = TextViewUtils.GetFontSize(14f);
                txtGeneralEnquiry_subContent.TextSize = TextViewUtils.GetFontSize(12f);
                txtUpdatePersonal.TextSize = TextViewUtils.GetFontSize(14f);
                txtOverVoltageClaim.TextSize = TextViewUtils.GetFontSize(14f);
                txtUpdatePersonalContent.TextSize = TextViewUtils.GetFontSize(12f);
                txtOverVoltageClaimContent.TextSize = TextViewUtils.GetFontSize(12f);
                //set translation of string 


                txtInputLayoutAccountNo.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "accNumberHint");
                infoLabeltxtWhereIsMyAcc.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "accNumberInfo");
                howCanWeHelpYou.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "infoHowcan");
                txtGeneralEnquiry.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "generalEnquiryTitle");
                txtGeneralEnquiry_subContent.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "generalEnquiryDescription");
                txtUpdatePersonal.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "updatePersonalDetTitle");
                txtOverVoltageClaim.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimTitle");
                txtUpdatePersonalContent.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "personalDetailsDescription");
                txtOverVoltageClaimContent.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimDescription");

                if (!UserEntity.IsCurrentlyActive())
                {

                    // image inject lay
                    //  ImageButton imgView = FindViewById<ImageButton>(Resource.Id.scanNewEnquiry);
                    //   FrameLayout.LayoutParams imgViewParams = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.WrapContent, FrameLayout.LayoutParams.WrapContent, GravityFlags.Right);
                    // imgViewParams.SetMargins(0, 8, 60, 0);
                    //  scanNewEnquiry.LayoutParameters = imgViewParams;

                    scanNewEnquiry.Visibility = ViewStates.Gone;

                    // bottom drawable hide
                    Drawable scanIcon = ContextCompat.GetDrawable(this, Resource.Drawable.scan);
                    Drawable accnoIcon = ContextCompat.GetDrawable(this, Resource.Drawable.ic_field_account_no);
                    txtAccountNo.SetCompoundDrawablesWithIntrinsicBounds(accnoIcon, null, scanIcon, null);



                }



                txtAccountNo.SetOnTouchListener(this);  //set listener on dropdown arrow at TextLayout
                txtAccountNo.TextChanged += TextChange;  //adding listener on text change
                txtAccountNo.FocusChange += TxtAccountNo_FocusChange;

                //Keyboard done button click
                txtAccountNo.EditorAction += delegate (object sender, TextView.EditorActionEventArgs e)
                {
                    if (e.ActionId == Android.Views.InputMethods.ImeAction.Done)
                    {
                        txtAccountNo.ClearFocus();
                        // Hide keyboard
                        var inputManager = (InputMethodManager)GetSystemService(InputMethodService);
                        inputManager.HideSoftInputFromWindow(txtAccountNo.WindowToken, HideSoftInputFlags.None);
                    }
                };
                txtAccountNo.AddTextChangedListener(new InputFilterFormField(txtAccountNo, txtInputLayoutAccountNo));  //adding listener on text change

                infoLabeltxtWhereIsMyAcc.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "accNumberInfo");  // inject translation to text

                onGetTooltipImageContent();

                var sharedpref_data = UserSessions.GetFeedbackUpdateDetailDisabled(mSharedPref);

                bool isUpdatePersonalDetail = bool.Parse(sharedpref_data);  //get from shared pref
                              

                if (isUpdatePersonalDetail == true)
                {
                    updatePersonalInfoConstraint.Visibility = ViewStates.Gone;
                }

                //210031353308




            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private View.IOnFocusChangeListener txtAccountChangedListner()
        {
            return null;
        }

        /// <summary>
        /// Check internet connection
        /// </summary>
        [Obsolete]
        public bool IsNetworkAvailable()
        {
            ConnectivityManager connectivity = (ConnectivityManager)(Application.Context.ApplicationContext).GetSystemService(Android.Content.Context.ConnectivityService);
            if (connectivity != null)
            {
                NetworkInfo[] info = connectivity.GetAllNetworkInfo();
                if (info != null)
                    for (int i = 0; i < info.Length; i++)
                        if (info[i].GetState() == NetworkInfo.State.Connected)
                        {
                            return true;                            
                        }
            }
            IsServerDown = true;
            return false;
        }

        private async void CANumberVerification(OVISRequest listData)
        {
            try
            {
                ShowProgressDialog();
                if (IsNetworkAvailable())
                {
                    var data = new MyTNBService.Request.BaseRequest();
                    var usin = data.usrInf;
                    AccNoDesc = "";
                    //verifyCADetailsExt Endpoint
                    CANumberVerifyResponce = await ServiceApiImpl.Instance.CAVerify(new CAVerifyRequestModel(usin.sspuid, "POST", "/claim/verifyCADetailsExt", listData));

                    if (CANumberVerifyResponce != null)
                    {
                        if (CANumberVerifyResponce.d != null)
                        {
                            IsPilot = CANumberVerifyResponce.d.OvervoltageClaimEnabled;
                            OvisUnderMaintenance = CANumberVerifyResponce.d.IsOvisUnderMaintenance;

                            if (OvisUnderMaintenance)
                            {
                                overvoltageclaimConstraint.Visibility = ViewStates.Visible;
                                txtOverVoltageClaim.SetTextColor(Color.ParseColor("#C8C8C8"));
                                txtOverVoltageClaimContent.SetTextColor(Color.ParseColor("#C8C8C8"));
                                updatePersoanlInfoIcon2.Visibility = ViewStates.Visible;
                                updatePersoanlInfoIcon1.Visibility = ViewStates.Invisible;
                                overvoltageclaimConstraint.Clickable = true;
                                accountLayout4.Visibility = ViewStates.Visible;

                                var infoValue = Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageMaintenanceLabel");
                                InfoLabel.Text = infoValue;
                            }
                            else
                            {
                                //Non Pilot
                                if (!CANumberVerifyResponce.d.OvervoltageClaimEnabled)
                                {
                                    overvoltageClaimVisible = false;
                                }

                                //Pilot
                                else if (CANumberVerifyResponce.d.OvervoltageClaimEnabled)
                                {
                                    overvoltageClaimVisible = true;
                                    IsWhiteListedArea = true;
                                    overvoltageclaimConstraint.Visibility = ViewStates.Visible;
                                    //accountLayout4.Visibility = ViewStates.Visible;
                                    overvoltageclaimConstraint.Clickable = true;
                                    if (!string.IsNullOrEmpty(txtAccountNo.Text))
                                    {
                                        string isValid = null;
                                        if (CANumberVerifyResponce.d.OvervoltageClaimSupported.ContainsKey(txtAccountNo.Text))
                                        {
                                            for (int i = 0; i < CANumberVerifyResponce.d.OvervoltageClaimSupported.Count(); i++)
                                            {
                                                if (CANumberVerifyResponce.d.OvervoltageClaimSupported.ElementAt(i).Key == txtAccountNo.Text)
                                                {
                                                    isValid = CANumberVerifyResponce.d.OvervoltageClaimSupported.ElementAt(i).Value;
                                                    //return;
                                                }
                                            }
                                        }
                                        if (isValid == "true")
                                        {
                                            IsWhiteListedArea = true;
                                            updatePersoanlInfoIcon1.SetBackgroundResource(Resource.Drawable.overvoltageclaimicon);
                                            updatePersoanlInfoIcon2.Visibility = ViewStates.Invisible;
                                            updatePersoanlInfoIcon1.Visibility = ViewStates.Visible;
                                            txtOverVoltageClaim.SetTextColor(Color.ParseColor("#1c79ca"));
                                            txtOverVoltageClaimContent.SetTextColor(Color.ParseColor("#49494a"));
                                            accountLayout4.Visibility = ViewStates.Invisible;
                                        }
                                        else if (isValid == "false" || isValid == "INVALID")
                                        {
                                            IsWhiteListedArea = false;
                                            txtOverVoltageClaim.SetTextColor(Color.ParseColor("#C8C8C8"));
                                            txtOverVoltageClaimContent.SetTextColor(Color.ParseColor("#C8C8C8"));
                                            updatePersoanlInfoIcon2.Visibility = ViewStates.Visible;
                                            updatePersoanlInfoIcon1.Visibility = ViewStates.Invisible;
                                            overvoltageclaimConstraint.Clickable = true;
                                            accountLayout4.Visibility = ViewStates.Visible;
                                            int index = accountList.FindIndex(s => s.AccNum.Equals(txtAccountNo.Text));
                                            if (index != -1)
                                            {
                                                AccNoDesc = "";
                                                AccNoDesc = accountList[index].AccDesc;
                                            }
                                            string infoValue;
                                            if (string.IsNullOrEmpty(AccNoDesc))
                                            {
                                                infoValue = Utility.GetLocalizedLabel("SubmitEnquiry", "currentlyNotEnabledForMelakaTitle") + txtAccountNo.Text;
                                            }
                                            else
                                            {
                                                infoValue = Utility.GetLocalizedLabel("SubmitEnquiry", "currentlyNotEnabledForMelakaTitle") + "\"" + AccNoDesc + " - " + txtAccountNo.Text + "\"";
                                            }
                                            InfoLabel.Text = infoValue;
                                        }
                                    }

                                }

                            }

                        }
                        else
                        {
                            if (CANumberVerifyResponce.d == null)
                            {
                                IsServerDown = true;
                                overvoltageclaimConstraint.Visibility = ViewStates.Visible;
                                txtOverVoltageClaim.SetTextColor(Color.ParseColor("#C8C8C8"));
                                txtOverVoltageClaimContent.SetTextColor(Color.ParseColor("#C8C8C8"));
                                updatePersoanlInfoIcon2.Visibility = ViewStates.Visible;
                                updatePersoanlInfoIcon1.Visibility = ViewStates.Invisible;
                                overvoltageclaimConstraint.Clickable = true;
                                accountLayout4.Visibility = ViewStates.Visible;
                                var infoValue = Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimtemproryUnavailable");
                                InfoLabel.Text = infoValue;
                                HideProgressDialog();
                            }
                            else
                            {
                                overvoltageClaimVisible = false;
                            }
                        }
                    }
                    else
                    {
                        overvoltageClaimVisible = false;
                        //server down
                    }
                }              
                HideProgressDialog();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Internal Server Error"))
                {
                    IsServerDown = true;
                    overvoltageclaimConstraint.Visibility = ViewStates.Visible;
                    txtOverVoltageClaim.SetTextColor(Color.ParseColor("#C8C8C8"));
                    txtOverVoltageClaimContent.SetTextColor(Color.ParseColor("#C8C8C8"));
                    updatePersoanlInfoIcon2.Visibility = ViewStates.Visible;
                    updatePersoanlInfoIcon1.Visibility = ViewStates.Invisible;
                    overvoltageclaimConstraint.Clickable = true;
                    accountLayout4.Visibility = ViewStates.Visible;
                    var infoValue = Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimtemproryUnavailable");
                    InfoLabel.Text = infoValue;
                    HideProgressDialog();
                }

            }
        }

        private void TxtAccountNo_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            try
            {

                if (!e.HasFocus)
                {
                    string accno = txtAccountNo.Text.ToString().Trim();
                    this.userActionsListener.CheckRequiredFields(accno);
                }



            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public override int ResourceId()
        {   //todo change
            return Resource.Layout.FeedbackPreloginNewICView;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public void makeSetClick(bool setClick)
        {
            this.SetIsClicked(setClick);
        }

        public void SetPresenter(FeedbackPreloginNewICContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            const int DRAWABLE_LEFT = 0;
            const int DRAWABLE_TOP = 1;
            const int DRAWABLE_RIGHT = 2;
            const int DRAWABLE_BOTTOM = 3;
            if (v is EditText)
            {
                EditText eTxtView = v as EditText;
                if (eTxtView.Id == Resource.Id.txtAccountNo)
                {

                    if (UserEntity.IsCurrentlyActive())
                    {
                        //to ensure only works if user is login
                        if (e.Action == MotionEventActions.Up)
                        {
                            if (e.RawX >= (txtAccountNo.Right - txtAccountNo.GetCompoundDrawables()[DRAWABLE_RIGHT].Bounds.Width()))
                            {
                                //this function listen to click on the dropdown drawable right
                                //check if from prelogin of after login disable if user from prelogin

                                this.userActionsListener.OnSelectAccount();


                                return true;
                            }
                        }

                    }
                    else
                    {
                        // cater if user is from outside
                        if (e.Action == MotionEventActions.Up)
                        {
                            if (e.RawX >= (txtAccountNo.Right - txtAccountNo.GetCompoundDrawables()[DRAWABLE_RIGHT].Bounds.Width()))
                            {
                                //this function listen to click on the dropdown drawable right

                                this.userActionsListener.showScan();

                                return true;
                            }
                        }
                    }



                }


            }
            return false;
        }



        private void TextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                string accno = txtAccountNo.Text.ToString().Trim();

                if (!string.IsNullOrEmpty(accno))
                {
                    txtInputLayoutAccountNo.Error = null;
                }
                //  this.userActionsListener.CheckRequiredFields(accno);

            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }


        }


        public void toggleEnableClick()
        {
            isAccChoosed = true;
            contactAccountNumbers.Add(txtAccountNo.Text);
            listData.contactAccountNumbers = contactAccountNumbers;
            CANumberVerification(listData);
        }

        public void toggleDisableClick()
        {
            isAccChoosed = false;
        }



        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        [Obsolete]
        protected override void OnResume()
        {
            base.OnResume();
            try
            {
               
                if (IsServerDown)
                {
                    IsServerDown = true;
                    overvoltageclaimConstraint.Visibility = ViewStates.Visible;
                    txtOverVoltageClaim.SetTextColor(Color.ParseColor("#C8C8C8"));
                    txtOverVoltageClaimContent.SetTextColor(Color.ParseColor("#C8C8C8"));
                    updatePersoanlInfoIcon2.Visibility = ViewStates.Visible;
                    updatePersoanlInfoIcon1.Visibility = ViewStates.Invisible;
                    overvoltageclaimConstraint.Clickable = true;
                    accountLayout4.Visibility = ViewStates.Visible;
                    var infoValue = Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimtemproryUnavailable");
                    InfoLabel.Text = infoValue;
                }
                else
                {
                    if (OvisUnderMaintenance == true)
                    {
                        overvoltageclaimConstraint.Visibility = ViewStates.Visible;
                        txtOverVoltageClaim.SetTextColor(Color.ParseColor("#C8C8C8"));
                        txtOverVoltageClaimContent.SetTextColor(Color.ParseColor("#C8C8C8"));
                        updatePersoanlInfoIcon2.Visibility = ViewStates.Visible;
                        updatePersoanlInfoIcon1.Visibility = ViewStates.Invisible;
                        overvoltageclaimConstraint.Clickable = true;
                        accountLayout4.Visibility = ViewStates.Visible;

                        var infoValue = Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageMaintenanceLabel");
                        InfoLabel.Text = infoValue;
                    }
                    //non pilot
                    else if (overvoltageClaimVisible == false)
                    {
                        overvoltageclaimConstraint.Visibility = ViewStates.Gone;
                        accountLayout4.Visibility = ViewStates.Gone;
                    }
                    //Pilot flow
                    else if (overvoltageClaimVisible == true)
                    {
                        overvoltageclaimConstraint.Visibility = ViewStates.Visible;

                        overvoltageclaimConstraint.Clickable = true;
                        if (IsWhiteListedArea == false)
                        {
                            txtOverVoltageClaim.SetTextColor(Color.ParseColor("#C8C8C8"));
                            txtOverVoltageClaimContent.SetTextColor(Color.ParseColor("#C8C8C8"));
                            updatePersoanlInfoIcon2.Visibility = ViewStates.Visible;
                            updatePersoanlInfoIcon1.Visibility = ViewStates.Invisible;
                            overvoltageclaimConstraint.Clickable = true;
                            accountLayout4.Visibility = ViewStates.Visible;
                            int index = accountList.FindIndex(s => s.AccNum.Equals(txtAccountNo.Text));
                            if (index != -1)
                            {
                                AccNoDesc = "";
                                AccNoDesc = accountList[index].AccDesc;
                            }

                            string infoValue;
                            if (string.IsNullOrEmpty(AccNoDesc))
                            {
                                infoValue = Utility.GetLocalizedLabel("SubmitEnquiry", "currentlyNotEnabledForMelakaTitle") + txtAccountNo.Text;
                            }
                            else
                            {
                                infoValue = Utility.GetLocalizedLabel("SubmitEnquiry", "currentlyNotEnabledForMelakaTitle") + "\"" + AccNoDesc + " - " + txtAccountNo.Text + "\"";
                            }
                            InfoLabel.Text = infoValue;
                        }
                        else if (IsWhiteListedArea == true)
                        {
                            updatePersoanlInfoIcon1.SetBackgroundResource(Resource.Drawable.overvoltageclaimicon);
                            updatePersoanlInfoIcon2.Visibility = ViewStates.Invisible;
                            updatePersoanlInfoIcon1.Visibility = ViewStates.Visible;
                            txtOverVoltageClaim.SetTextColor(Color.ParseColor("#1c79ca"));
                            txtOverVoltageClaimContent.SetTextColor(Color.ParseColor("#49494a"));
                            accountLayout4.Visibility = ViewStates.Invisible;
                            //updatePersoanlInfoIcon1.SetAlpha(Convert.ToInt32(1));
                        }


                    }
                }
                             
                FirebaseAnalyticsUtils.SetScreenName(this, "Submit New Enquiry");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        [OnClick(Resource.Id.accountLayout4)]
        void OnDialogClick(object sender, EventArgs eventArgs)
        {
            string title;
            if (OvisUnderMaintenance == true)
            {
                leaveDialog = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                 .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageMaintenanceTitle"))
                 .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageMaintenanceDescription"))
                 .SetCTALabel(Utility.GetLocalizedLabel("SubmitEnquiry", "Gotit"))
                 .SetCTAaction(() => { leaveDialog.DismissDialog(); })
                 .Build();
                leaveDialog.Show();
            }
            else if (IsServerDown)
            {
                leaveDialog = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                 .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimtemproryUnavailableTitle"))
                 .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimtemproryUnavailableDescription"))
                 .SetCTALabel(Utility.GetLocalizedLabel("SubmitEnquiry", "Gotit"))
                 .SetCTAaction(() => { leaveDialog.DismissDialog(); })
                 .Build();
                leaveDialog.Show();
            }
            else
            {
                if(string.IsNullOrEmpty(AccNoDesc))
                {
                    title = Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimIsCurrentlyNotEnabledForAccountTitle") + txtAccountNo.Text;
                }
                else
                {
                    title = Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimIsCurrentlyNotEnabledForAccountTitle") + "\"" + AccNoDesc + " - " + txtAccountNo.Text + "\"";
                }
                
                leaveDialog = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                 .SetTitle(title)
                 .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimIsCurrentlyNotEnabledForAccountDescription"))
                 .SetCTALabel(Utility.GetLocalizedLabel("SubmitEnquiry", "Gotit"))
                 .SetCTAaction(() => { leaveDialog.DismissDialog(); })
                 .Build();
                leaveDialog.Show();
            }

        }



        [OnClick(Resource.Id.scanNewEnquiry)]
        void OnScanClick(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {


                this.SetIsClicked(true);
                Intent barcodeIntent = new Intent(this, typeof(BarcodeActivity));
                //barcodeIntent.PutExtra(Constants.PAGE_TITLE, Utility.GetLocalizedLabel("SubmitEnquiry", "submitEnquiryTitle"));
                StartActivityForResult(barcodeIntent, Constants.BARCODE_REQUEST_CODE);
            }
        }

        public void onScan()
        {
            if (!this.GetIsClicked())
            {

                this.SetIsClicked(true);
                Intent barcodeIntent = new Intent(this, typeof(BarcodeActivity));
                //barcodeIntent.PutExtra(Constants.PAGE_TITLE, Utility.GetLocalizedLabel("SubmitEnquiry", "submitEnquiryTitle"));
                StartActivityForResult(barcodeIntent, Constants.BARCODE_REQUEST_CODE);
            }
        }

        //[OnClick(Resource.Id.txtAccountNo)]
        //void OnSelectAccountLayout1(object sender, EventArgs eventArgs)
        //{
        //    if (!this.GetIsClicked())+
        //    {
        //        this.SetIsClicked(true);
        //        this.userActionsListener.OnSelectAccount();
        //    }
        //}


        public void ShowInvalidAccountNumberError()
        {
            txtInputLayoutAccountNo.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);

            if (txtInputLayoutAccountNo.Error != Utility.GetLocalizedLabel("SubmitEnquiry", "validElectricityAccountNoError"))
            {
                txtInputLayoutAccountNo.Error = Utility.GetLocalizedLabel("SubmitEnquiry", "validElectricityAccountNoError");
            }

        }

        public void ShowEnterOrSelectAccNumber()
        {
            txtInputLayoutAccountNo.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);

            if (txtInputLayoutAccountNo.Error != Utility.GetLocalizedLabel("SubmitEnquiry", "plsEnterAcc"))
            {
                txtInputLayoutAccountNo.Error = Utility.GetLocalizedLabel("SubmitEnquiry", "plsEnterAcc");
            }

            txtInputLayoutAccountNo.RequestFocus();
        }


        public void RemoveNumberErrorMessage()
        {
            txtInputLayoutAccountNo.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);


            txtInputLayoutAccountNo.Error = "";

        }

        public void ShowGeneralEnquiry()
        {
            try
            {
                if (IsOverVoltageClick)
                {
                    Intent overvoltageClaim = new Intent(this, typeof(OvervoltageClaim));
                    overvoltageClaim.PutExtra(Constants.ACCOUNT_NUMBER, txtAccountNo.Text.ToString().Trim());
                    StartActivity(overvoltageClaim);
                    IsOverVoltageClick = false;
                }
                else
                {
                    Intent generalEnquiry = new Intent(this, typeof(FeedbackGeneralEnquiryStepOneActivity));
                    generalEnquiry.PutExtra(Constants.ACCOUNT_NUMBER, txtAccountNo.Text.ToString().Trim());
                    StartActivity(generalEnquiry);
                }


            }
            catch (Exception ex)
            {

            }


            //Intent generalEnquiry = new Intent(this, typeof(FeedbackGeneralEnquiryStepOneActivity));
            //generalEnquiry.PutExtra(Constants.ACCOUNT_NUMBER, txtAccountNo.Text.ToString().Trim());
            //StartActivityForResult(generalEnquiry, Constants.REQUEST_FEEDBACK_SUCCESS_VIEW);
        }


        public void ShowSelectAccount()
        {
            Intent supplyAccount = new Intent(this, typeof(FeedbackSelectAccountActivity));
            if (selectedAccount != null)
            {
                supplyAccount.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            }
            StartActivityForResult(supplyAccount, Constants.SELECT_ACCOUNT_REQUEST_CODE);
        }

        //handle push page with waiting return data
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {

            try
            {

                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == Constants.BARCODE_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {

                        string barcodeResultText = data.GetStringExtra(Constants.BARCODE_RESULT);
                        txtAccountNo.Text = barcodeResultText;

                    }
                }
                else if (requestCode == Constants.SELECT_ACCOUNT_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        Bundle extras = data.Extras;

                        this.selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                        //selectedCustomerBillingAccount = CustomerBillingAccount.FindByAccNum(selectedAccount.AccountNum);

                        //injecting string into the accno
                        txtAccountNo.Text = selectedAccount.AccountNum;
                        if (!IsPilot)
                        {
                            overvoltageClaimVisible = false;
                        }
                        else if (IsPilot)
                        {
                            string isValid = null;
                            if (CANumberVerifyResponce.d.OvervoltageClaimSupported.ContainsKey(txtAccountNo.Text))
                            {
                                for (int i = 0; i < CANumberVerifyResponce.d.OvervoltageClaimSupported.Count(); i++)
                                {
                                    if (CANumberVerifyResponce.d.OvervoltageClaimSupported.ElementAt(i).Key == txtAccountNo.Text)
                                    {
                                        isValid = CANumberVerifyResponce.d.OvervoltageClaimSupported.ElementAt(i).Value;
                                        //return;
                                    }
                                }
                            }
                            overvoltageClaimVisible = true;
                            if (isValid == "true")
                            {
                                IsWhiteListedArea = true;
                            }
                            else if (isValid == "false" || isValid == "INVALID")
                            {
                                IsWhiteListedArea = false;
                            }
                        }

                        //if (txtAccountNo.Text == "210000750008")//3
                        //{
                        //    overvoltageClaimVisible = true;
                        //    IsWhiteListedArea = true;
                        //}
                        //else if (txtAccountNo.Text == "220004092309")
                        //{
                        //    overvoltageClaimVisible = true;
                        //    IsWhiteListedArea = false;
                        //}
                        //else if (txtAccountNo.Text == "220004421505")//1
                        //{
                        //    overvoltageClaimVisible = false;
                        //}
                    }
                }


            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.generalEnquiryConstraint)]
        void OnGeneralEnquiryConstraint(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                if (DownTimeEntity.IsBCRMDown())
                {
                    OnBCRMDownTimeErrorMessage();
                    this.SetIsClicked(false);
                }
                else
                {
                    string accno = txtAccountNo.Text.ToString().Trim();
                    bool isAllowedToPass = this.userActionsListener.CheckRequiredFields(accno);

                    if (isAllowedToPass)
                    {
                        this.SetIsClicked(true);
                        this.userActionsListener.ValidateAccountAsync(txtAccountNo.Text.ToString().Trim(), false);
                    }
                    else
                    {
                        this.SetIsClicked(false);
                    }
                }
            }
        }

        [OnClick(Resource.Id.updatePersonalInfoConstraint)]
        void OnupdatePersonalInfoConstraint(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                if (DownTimeEntity.IsBCRMDown())
                {
                    OnBCRMDownTimeErrorMessage();
                    this.SetIsClicked(false);
                }
                else
                {
                    string accno = txtAccountNo.Text.ToString().Trim();
                    bool isAllowed = this.userActionsListener.CheckRequiredFields(accno);

                    if (isAllowed)
                    {
                        this.SetIsClicked(true);
                        this.userActionsListener.ValidateAccountAsync(txtAccountNo.Text.ToString().Trim(), true);
                    }
                    else
                    {
                        this.SetIsClicked(false);
                    }

                }
            }
        }

        [OnClick(Resource.Id.overvoltageclaimConstraint)]
        void OnovervoltageclaimConstraint(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                IsOverVoltageClick = true;
                //Overvoltage clickable with blue color
                if(IsWhiteListedArea)
                {
                    if (DownTimeEntity.IsBCRMDown())
                    {
                        OnBCRMDownTimeErrorMessage();
                        this.SetIsClicked(false);
                    }
                    else
                    {
                        string accno = txtAccountNo.Text.ToString().Trim();
                        bool isAllowed = this.userActionsListener.CheckRequiredFields(accno);

                        if (isAllowed)
                        {
                            this.SetIsClicked(true);
                            this.userActionsListener.ValidateAccountAsync(txtAccountNo.Text.ToString().Trim(), false);
                        }
                        else
                        {
                            this.SetIsClicked(false);
                        }

                    }
                }
                //Overvoltage clickable with gray color
                else
                {
                    string title;
                    if (OvisUnderMaintenance == true)
                    {
                        leaveDialog = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                         .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageMaintenanceTitle"))
                         .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageMaintenanceDescription"))
                         .SetCTALabel(Utility.GetLocalizedLabel("SubmitEnquiry", "Gotit"))
                         .SetCTAaction(() => { leaveDialog.DismissDialog(); })
                         .Build();
                        leaveDialog.Show();
                    }
                    else if (IsServerDown)
                    {
                        leaveDialog = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                         .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimtemproryUnavailableTitle"))
                         .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimtemproryUnavailableDescription"))
                         .SetCTALabel(Utility.GetLocalizedLabel("SubmitEnquiry", "Gotit"))
                         .SetCTAaction(() => { leaveDialog.DismissDialog(); })
                         .Build();
                        leaveDialog.Show();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(AccNoDesc))
                        {
                            title = Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimIsCurrentlyNotEnabledForAccountTitle") + txtAccountNo.Text;
                        }
                        else
                        {
                            title = Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimIsCurrentlyNotEnabledForAccountTitle") + AccNoDesc + " - " + txtAccountNo.Text;
                        }

                        leaveDialog = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                         .SetTitle(title)
                         .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimIsCurrentlyNotEnabledForAccountDescription"))
                         .SetCTALabel(Utility.GetLocalizedLabel("SubmitEnquiry", "Gotit"))
                         .SetCTAaction(() => { leaveDialog.DismissDialog(); })
                         .Build();
                        leaveDialog.Show();
                    }
                }
            }
        }


        Snackbar mErrorMessageSnackBar;
        public void OnBCRMDownTimeErrorMessage(string message = null)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }


            if (string.IsNullOrEmpty(message))
            {
                DownTimeEntity BCRMDownTime = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                if (!string.IsNullOrEmpty(BCRMDownTime.DowntimeTextMessage))
                {
                    message = BCRMDownTime.DowntimeTextMessage;
                }
                else
                {
                    message = Utility.GetLocalizedErrorLabel("defaultErrorMessage");
                }
            }

            mErrorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
            this.SetIsClicked(false);
        }


        public void showUpdatePersonalDetail()
        {

            var updatePersoanlInfo = new Intent(this, typeof(UpdatePersonalDetailStepOneActivity));

            updatePersoanlInfo.PutExtra(Constants.ACCOUNT_NUMBER, txtAccountNo.Text.ToString().Trim());
            updatePersoanlInfo.PutExtra(Constants.PAGE_TITLE, Utility.GetLocalizedLabel("SubmitEnquiry", "updatePersonalDetTitle"));
            updatePersoanlInfo.PutExtra(Constants.PAGE_STEP_TITLE, Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle1of3"));

            StartActivity(updatePersoanlInfo);

        }




        [OnClick(Resource.Id.infoLabelWhereIsMyAcc)]
        void OninfoLabelWhereIsMyAcc(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {


                this.SetIsClicked(true);
                this.userActionsListener.onShowWhereIsMyAcc();
            }
        }


        public async void ShowWhereIsMyAcc()

        {
            //List<WhereMyAccToolTipResponse> modelList = MyTNBAppToolTipData.GetWhereMyAccTipData();

            //if (modelList != null && modelList.Count > 0)
            //{
            //    if (!this.GetIsClicked())
            //    {
            //        this.SetIsClicked(true);
            //        MyTNBAppToolTipBuilder Tooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
            //           .SetHeaderImageBitmap(modelList[0].ImageBitmap)
            //           .SetTitle(modelList[0].PopUpTitle)
            //           .SetMessage(modelList[0].PopUpBody)
            //           .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
            //           .SetCTAaction(() => { this.SetIsClicked(false); })
            //           .Build();
            //           Tooltip.Show();
            //    }
            //}
            //else
            //{  //incase sitecoreCMSEntity return null 

            //    //please delete 

            //    var url =Utility.GetLocalizedLabel("AddAccount", "imageTest");
            //    Bitmap imageCache = ImageUtils.GetImageBitmapFromUrl(url);
            //    // .SetHeaderImage(Resource.Drawable.img_register_acct_no)
            //    MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
            //     .SetHeaderImageBitmap(imageCache)
            //    .SetHeaderImage(Resource.Drawable.img_register_acct_no)
            //    .SetTitle(Utility.GetLocalizedLabel("AddAccount", "whereIsMyAccountTitle"))
            //    .SetMessage(Utility.GetLocalizedLabel("AddAccount", "whereIsMyAccountDetails"))
            //    .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
            //    .SetCTAaction(() => { this.SetIsClicked(false); })
            //    .Build();
            //     whereisMyacc.Show();
            //}

            //   var url = Utility.GetLocalizedLabel("SubmitEnquiry", "imageWhereAcc");



            ///   Bitmap imageCache = ImageUtils.GetImageBitmapFromUrl(SiteCoreConfig.SITECORE_URL + url);
            ///   




            string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC);

            if (!base64Image.IsNullOrEmpty())
            {
                var imageCache = Base64ToBitmap(base64Image);

                MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                 .SetHeaderImageBitmap(imageCache)
                .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "accNumberInfo"))
                .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "accNumberDetails"))
                .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                .SetCTAaction(() => { this.SetIsClicked(false); })
                .Build();
                whereisMyacc.Show();


            }
            else
            {   //if sql lite data is somehow corrupted
                // Bitmap imageCache = ImageUtils.GetImageBitmapFromUrl(SiteCoreConfig.SITECORE_URL + Utility.GetLocalizedLabel("SubmitEnquiry", "imageWhereAcc"));
                // MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                // .SetHeaderImageBitmap(imageCache)
                // .SetTitle(Utility.GetLocalizedLabel("AddAccount", "whereIsMyAccountTitle"))
                // .SetMessage(Utility.GetLocalizedLabel("AddAccount", "whereIsMyAccountDetails"))
                // .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                // .SetCTAaction(() => { this.SetIsClicked(false); })
                // .Build();
                // whereisMyacc.Show();

                this.SetIsClicked(false);

            }








        }



        private Task onGetTooltipImageContent()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    //check if image is exist in sql lite
                    var imageWhereAccUrl = SiteCoreConfig.SITECORE_URL + Utility.GetLocalizedLabel("SubmitEnquiry", "imageWhereAcc");

                    if (TooltipImageDirectEntity.isNeedUpdate(imageWhereAccUrl, TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC))
                    {
                        TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC);

                        var image = ImageUtils.GetImageBitmapFromUrl(imageWhereAccUrl);
                        var base64 = BitmapToBase64(image);

                        TooltipImageDirectEntity newImage_WHERE_MY_ACC = new TooltipImageDirectEntity();
                        newImage_WHERE_MY_ACC.ImageBase64 = base64;
                        newImage_WHERE_MY_ACC.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC.ToString();
                        newImage_WHERE_MY_ACC.Url = imageWhereAccUrl;

                        TooltipImageDirectEntity.InsertItem(newImage_WHERE_MY_ACC);

                    }
                    else
                    {
                        // recheck local is the base64 exist or not is not need update
                        string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC);
                        if (base64Image.IsNullOrEmpty())
                        {
                            TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC);

                            var image = ImageUtils.GetImageBitmapFromUrl(imageWhereAccUrl);
                            var base64 = BitmapToBase64(image);

                            TooltipImageDirectEntity newImage_WHERE_MY_ACC = new TooltipImageDirectEntity();
                            newImage_WHERE_MY_ACC.ImageBase64 = base64;
                            newImage_WHERE_MY_ACC.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC.ToString();
                            newImage_WHERE_MY_ACC.Url = imageWhereAccUrl;

                            TooltipImageDirectEntity.InsertItem(newImage_WHERE_MY_ACC);

                        }

                    }

                    //check if image is exist in sql lite
                    var imageIC = SiteCoreConfig.SITECORE_URL + Utility.GetLocalizedLabel("SubmitEnquiry", "imageCopyIC");

                    if (TooltipImageDirectEntity.isNeedUpdate(imageIC, TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE))
                    {
                        TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE);

                        var image = ImageUtils.GetImageBitmapFromUrl(imageIC);
                        var base64 = BitmapToBase64(image);

                        TooltipImageDirectEntity newImage_WHERE_MY_ACC = new TooltipImageDirectEntity();
                        newImage_WHERE_MY_ACC.ImageBase64 = base64;
                        newImage_WHERE_MY_ACC.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE.ToString();
                        newImage_WHERE_MY_ACC.Url = imageIC;

                        TooltipImageDirectEntity.InsertItem(newImage_WHERE_MY_ACC);

                    }
                    else
                    {
                        // recheck local is the base64 exist or not is not need update
                        string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE);
                        if (base64Image.IsNullOrEmpty())
                        {

                            TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE);

                            var image = ImageUtils.GetImageBitmapFromUrl(imageIC);
                            var base64 = BitmapToBase64(image);

                            TooltipImageDirectEntity newImage_IC_SAMPLE = new TooltipImageDirectEntity();
                            newImage_IC_SAMPLE.ImageBase64 = base64;
                            newImage_IC_SAMPLE.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE.ToString();
                            newImage_IC_SAMPLE.Url = imageIC;

                            TooltipImageDirectEntity.InsertItem(newImage_IC_SAMPLE);

                        }
                    }
                    //check if image is exist in sql lite   imagePermises
                    var imageConsent = SiteCoreConfig.SITECORE_URL + Utility.GetLocalizedLabel("SubmitEnquiry", "imageConsent");

                    if (TooltipImageDirectEntity.isNeedUpdate(imageConsent, TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT))
                    {
                        TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT);

                        var image_consent = ImageUtils.GetImageBitmapFromUrl(imageConsent);
                        var base64 = BitmapToBase64(image_consent);

                        TooltipImageDirectEntity newImage_PROOF_OF_CONSENT = new TooltipImageDirectEntity();
                        newImage_PROOF_OF_CONSENT.ImageBase64 = base64;
                        newImage_PROOF_OF_CONSENT.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT.ToString();
                        newImage_PROOF_OF_CONSENT.Url = imageConsent;

                        TooltipImageDirectEntity.InsertItem(newImage_PROOF_OF_CONSENT);

                    }
                    else
                    {
                        // recheck local is the base64 exist or not is not need update
                        string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT);
                        if (base64Image.IsNullOrEmpty())
                        {
                            TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT);

                            var image_consent = ImageUtils.GetImageBitmapFromUrl(imageConsent);
                            var base64 = BitmapToBase64(image_consent);

                            TooltipImageDirectEntity newImage_PROOF_OF_CONSENT = new TooltipImageDirectEntity();
                            newImage_PROOF_OF_CONSENT.ImageBase64 = base64;
                            newImage_PROOF_OF_CONSENT.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT.ToString();
                            newImage_PROOF_OF_CONSENT.Url = imageConsent;

                            TooltipImageDirectEntity.InsertItem(newImage_PROOF_OF_CONSENT);
                        }

                    }
                    //check if image is exist in sql lite   
                    var imagePermises = SiteCoreConfig.SITECORE_URL + Utility.GetLocalizedLabel("SubmitEnquiry", "imagePermises");

                    if (TooltipImageDirectEntity.isNeedUpdate(imagePermises, TooltipImageDirectEntity.IMAGE_CATEGORY.PERMISE_IMAGE))
                    {
                        TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.PERMISE_IMAGE);

                        var image_Permises = ImageUtils.GetImageBitmapFromUrl(imagePermises);
                        var base64 = BitmapToBase64(image_Permises);

                        TooltipImageDirectEntity newImage_PERMISE_IMAGE = new TooltipImageDirectEntity();
                        newImage_PERMISE_IMAGE.ImageBase64 = base64;
                        newImage_PERMISE_IMAGE.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.PERMISE_IMAGE.ToString();
                        newImage_PERMISE_IMAGE.Url = imagePermises;

                        TooltipImageDirectEntity.InsertItem(newImage_PERMISE_IMAGE);

                    }
                    else
                    {
                        // recheck local is the base64 exist or not is not need update
                        string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.PERMISE_IMAGE);
                        if (base64Image.IsNullOrEmpty())
                        {
                            TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.PERMISE_IMAGE);

                            var image_Permises = ImageUtils.GetImageBitmapFromUrl(imagePermises);
                            var base64 = BitmapToBase64(image_Permises);

                            TooltipImageDirectEntity newImage_PERMISE_IMAGE = new TooltipImageDirectEntity();
                            newImage_PERMISE_IMAGE.ImageBase64 = base64;
                            newImage_PERMISE_IMAGE.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.PERMISE_IMAGE.ToString();
                            newImage_PERMISE_IMAGE.Url = imagePermises;

                            TooltipImageDirectEntity.InsertItem(newImage_PERMISE_IMAGE);
                        }

                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            });
        }

        public string BitmapToBase64(Bitmap bitmap)
        {
            string B64Output = "";
            try
            {
                MemoryStream byteArrayOutputStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                byte[] byteArray = byteArrayOutputStream.ToArray();
                B64Output = Android.Util.Base64.EncodeToString(byteArray, Base64Flags.Default);
            }
            catch (Exception e)
            {
                B64Output = "";
                Utility.LoggingNonFatalError(e);
            }

            return B64Output;
        }

        public static Bitmap Base64ToBitmap(string base64String)
        {
            Bitmap convertedBitmap = null;
            try
            {
                byte[] imageAsBytes = Android.Util.Base64.Decode(base64String, Base64Flags.Default);
                convertedBitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
            }
            catch (Exception e)
            {
                convertedBitmap = null;
                Utility.LoggingNonFatalError(e);
            }

            return convertedBitmap;
        }

        Snackbar newErrorMessageSnackBar;

        public bool IsOverVoltageClick { get; private set; }

        public void OnSubmitError(string message = null)
        {
            if (newErrorMessageSnackBar != null && newErrorMessageSnackBar.IsShown)
            {
                newErrorMessageSnackBar.Dismiss();
            }


            if (string.IsNullOrEmpty(message))
            {
                message = Utility.GetLocalizedErrorLabel("defaultErrorMessage");
            }

            newErrorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { newErrorMessageSnackBar.Dismiss(); }
            );
            View v = newErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            newErrorMessageSnackBar.Show();
            this.SetIsClicked(false);
        }


    }

}
