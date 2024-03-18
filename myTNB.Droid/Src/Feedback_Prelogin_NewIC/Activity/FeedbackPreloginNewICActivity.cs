using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Net;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using myTNB.AndroidApp.Src.Barcode.Activity;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.Enquiry.GSL.Activity;
using myTNB.AndroidApp.Src.Feedback_Login_BillRelated.Activity;
using myTNB.AndroidApp.Src.Feedback_Prelogin_NewIC.MVP;
using myTNB.AndroidApp.Src.FeedbackAboutBillEnquiryStepOne.Activity;
using myTNB.AndroidApp.Src.FeedbackGeneralEnquiryStepOne.Activity;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.OverVoltageClaim.Activity;
using myTNB.AndroidApp.Src.SiteCore;
using myTNB.AndroidApp.Src.UpdatePersonalDetailStepOne.Activity;
using myTNB.AndroidApp.Src.Utils;
using myTNB.Mobile;
using Newtonsoft.Json;
using Org.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static myTNB.LanguageManager;

namespace myTNB.AndroidApp.Src.Feedback_Prelogin_NewIC.Activity
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

        [BindView(Resource.Id.gslRebateConstraint)]
        LinearLayout gslRebateConstraint;

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

        [BindView(Resource.Id.txtAboutBillEnquiry)]
        TextView txtAboutBillEnquiry;

        [BindView(Resource.Id.txtAboutBillEnquiry_subContent)]
        TextView txtAboutBillEnquiry_subContent;

        [BindView(Resource.Id.txtUpdatePersonal)]
        TextView txtUpdatePersonal;

        [BindView(Resource.Id.txtUpdatePersonalContent)]
        TextView txtUpdatePersonalContent;

        [BindView(Resource.Id.txtGSLRebateTitle)]
        TextView txtGSLRebateTitle;

        [BindView(Resource.Id.txtGSLRebateSubTitle)]
        TextView txtGSLRebateSubTitle;

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
        TriggerOVISServicesResponseModel TriggerOVISServicesResponse;
        List<CustomerBillingAccount> accountList;
        List<string> contactAccountNumbers;
        OVISRequest listData;

        MyTNBAppToolTipBuilder leaveDialog;

        public enum EnquiryTypeEnum
        {
            General,
            UpdatePersonalDetails,
            AboutMyBill,
            GSLRebate,
            OvervoltageClaim,
            None
        }

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
                TextViewUtils.SetMuseoSans300Typeface(txtUpdatePersonalContent, txtGeneralEnquiry_subContent, txtAccountNo, txtGSLRebateSubTitle, txtAboutBillEnquiry_subContent, txtOverVoltageClaimContent);
                TextViewUtils.SetMuseoSans500Typeface(infoLabeltxtWhereIsMyAcc, howCanWeHelpYou, txtGeneralEnquiry, txtUpdatePersonal, txtAboutBillEnquiry, txtGSLRebateTitle, txtOverVoltageClaim, InfoLabel);
                TextViewUtils.SetTextSize12(txtUpdatePersonalContent, txtGeneralEnquiry_subContent, infoLabeltxtWhereIsMyAcc, txtAboutBillEnquiry_subContent, txtGSLRebateSubTitle, txtOverVoltageClaimContent, InfoLabel);
                TextViewUtils.SetTextSize14(txtGeneralEnquiry, txtUpdatePersonal, txtAboutBillEnquiry, txtGSLRebateTitle, txtOverVoltageClaim);
                TextViewUtils.SetTextSize16(txtAccountNo, howCanWeHelpYou);

                //set translation of string 
                txtInputLayoutAccountNo.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "accNumberHint");
                infoLabeltxtWhereIsMyAcc.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "accNumberInfo");
                howCanWeHelpYou.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "infoHowcan");
                txtGeneralEnquiry.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "generalEnquiryTitle");
                txtAboutBillEnquiry.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "aboutMyBillTitle");
                txtAboutBillEnquiry_subContent.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "aboutMyBillDescription");
                txtGeneralEnquiry_subContent.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "generalEnquiryDescription");
                txtUpdatePersonal.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "updatePersonalDetTitle");
                txtOverVoltageClaim.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimTitle");
                txtUpdatePersonalContent.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "personalDetailsDescription");
                txtGSLRebateTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_TITLE);
                txtGSLRebateSubTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_DESC);
                txtOverVoltageClaimContent.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimDescription");

                gslRebateConstraint.Visibility = LanguageManager.Instance.GetConfigToggleValue(ConfigPropertyEnum.IsGSLRebateEnabled) ? ViewStates.Visible : ViewStates.Gone;

                if (!UserEntity.IsCurrentlyActive())
                {
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


                if (isUpdatePersonalDetail == false)
                {
                    updatePersonalInfoConstraint.Visibility = ViewStates.Visible;
                }
                else
                {
                    updatePersonalInfoConstraint.Visibility = ViewStates.Gone;
                }
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
                    CancellationTokenSourceWrapper.isOvervoltageClaimPilotNonPilotTimeout = true;
                    TriggerOVISServicesResponse = await ServiceApiImpl.Instance.TriggerOVISServices(new TriggerOVISServicesRequestModel(usin.sspuid, "POST", "/claim/verifyCADetailsExt", listData));//verifyCADetailsExt //verifyCADetailsUnderMaintenanceExt
                    CancellationTokenSourceWrapper.isOvervoltageClaimPilotNonPilotTimeout = false;
                    if (TriggerOVISServicesResponse != null)
                    {
                        if (TriggerOVISServicesResponse.d != null)
                        {
                            var jsondata = JsonConvert.SerializeObject(TriggerOVISServicesResponse.d);
                            JSONObject containerObject = new JSONObject(jsondata);
                            if (containerObject.Has("IsOvisUnderMaintenance"))
                            {
                                IsPilot = TriggerOVISServicesResponse.d.OvervoltageClaimEnabled;
                                OvisUnderMaintenance = TriggerOVISServicesResponse.d.IsOvisUnderMaintenance;

                                if (IsPilot)
                                {
                                    //InLocal Storage store true.
                                    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                                    ISharedPreferencesEditor editor = prefs.Edit();
                                    editor.PutBoolean("StoreUnderMaintanance", true);
                                    editor.Apply();
                                    if (OvisUnderMaintenance)
                                    {
                                        // Ovis UnderMaintenance flow
                                        overvoltageclaimConstraint.Visibility = ViewStates.Visible;
                                        txtOverVoltageClaim.SetTextColor(Android.Graphics.Color.ParseColor("#C8C8C8"));
                                        txtOverVoltageClaimContent.SetTextColor(Android.Graphics.Color.ParseColor("#C8C8C8"));
                                        updatePersoanlInfoIcon2.Visibility = ViewStates.Visible;
                                        updatePersoanlInfoIcon1.Visibility = ViewStates.Invisible;
                                        overvoltageclaimConstraint.Clickable = true;
                                        accountLayout4.Visibility = ViewStates.Visible;

                                        var infoValue = Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageMaintenanceLabel");
                                        InfoLabel.Text = infoValue;
                                    }
                                    else
                                    {
                                        // Pilot Flow
                                        overvoltageClaimVisible = true;
                                        IsWhiteListedArea = true;
                                        overvoltageclaimConstraint.Visibility = ViewStates.Visible;
                                        //accountLayout4.Visibility = ViewStates.Visible;
                                        overvoltageclaimConstraint.Clickable = true;
                                        if (!string.IsNullOrEmpty(txtAccountNo.Text))
                                        {
                                            string isValid = null;
                                            if (TriggerOVISServicesResponse.d.OvervoltageClaimSupported.ContainsKey(txtAccountNo.Text))
                                            {
                                                for (int i = 0; i < TriggerOVISServicesResponse.d.OvervoltageClaimSupported.Count(); i++)
                                                {
                                                    if (TriggerOVISServicesResponse.d.OvervoltageClaimSupported.ElementAt(i).Key == txtAccountNo.Text)
                                                    {
                                                        isValid = TriggerOVISServicesResponse.d.OvervoltageClaimSupported.ElementAt(i).Value;
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
                                                txtOverVoltageClaim.SetTextColor(Android.Graphics.Color.ParseColor("#1c79ca"));
                                                txtOverVoltageClaimContent.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
                                                accountLayout4.Visibility = ViewStates.Invisible;
                                            }
                                            else if (isValid == "false" || isValid == "INVALID")
                                            {
                                                IsWhiteListedArea = false;
                                                txtOverVoltageClaim.SetTextColor(Android.Graphics.Color.ParseColor("#C8C8C8"));
                                                txtOverVoltageClaimContent.SetTextColor(Android.Graphics.Color.ParseColor("#C8C8C8"));
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
                                else
                                {
                                    // Non Pilot flow
                                    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                                    ISharedPreferencesEditor editor = prefs.Edit();
                                    editor.PutBoolean("StoreUnderMaintanance", false);
                                    editor.Apply();
                                    overvoltageClaimVisible = false;
                                }
                            }
                            else
                            {
                                ServerDown();
                            }
                        }
                        else
                        {
                            if (TriggerOVISServicesResponse.d == null)
                            {
                                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                                bool mBool = prefs.GetBoolean("StoreUnderMaintanance", false);
                                if (mBool)
                                {
                                    //server down
                                    ServerDown();
                                }
                                else
                                {
                                    overvoltageClaimVisible = false;
                                }
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
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                bool mBool = prefs.GetBoolean("StoreUnderMaintanance", false);
                if (mBool)
                {
                    //server down
                    ServerDown();
                }
                else
                {
                    overvoltageClaimVisible = false;
                }
                HideProgressDialog();

            }
        }

        public void ServerDown()
        {
            IsServerDown = true;
            overvoltageclaimConstraint.Visibility = ViewStates.Visible;
            txtOverVoltageClaim.SetTextColor(Android.Graphics.Color.ParseColor("#C8C8C8"));
            txtOverVoltageClaimContent.SetTextColor(Android.Graphics.Color.ParseColor("#C8C8C8"));
            updatePersoanlInfoIcon2.Visibility = ViewStates.Visible;
            updatePersoanlInfoIcon1.Visibility = ViewStates.Invisible;
            overvoltageclaimConstraint.Clickable = true;
            accountLayout4.Visibility = ViewStates.Visible;
            var infoValue = Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimtemproryUnavailable");
            InfoLabel.Text = infoValue;
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





        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                if (IsServerDown)
                {
                    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                    bool mBool = prefs.GetBoolean("StoreUnderMaintanance", false);
                    if (mBool)
                    {
                        //server down
                        ServerDown();
                    }
                    else
                    {
                        overvoltageClaimVisible = false;
                    }
                }
                else
                {
                    if (OvisUnderMaintenance == true)
                    {
                        overvoltageclaimConstraint.Visibility = ViewStates.Visible;
                        txtOverVoltageClaim.SetTextColor(Android.Graphics.Color.ParseColor("#C8C8C8"));
                        txtOverVoltageClaimContent.SetTextColor(Android.Graphics.Color.ParseColor("#C8C8C8"));
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
                            txtOverVoltageClaim.SetTextColor(Android.Graphics.Color.ParseColor("#C8C8C8"));
                            txtOverVoltageClaimContent.SetTextColor(Android.Graphics.Color.ParseColor("#C8C8C8"));
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
                            txtOverVoltageClaim.SetTextColor(Android.Graphics.Color.ParseColor("#1c79ca"));
                            txtOverVoltageClaimContent.SetTextColor(Android.Graphics.Color.ParseColor("#49494a"));
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
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
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
                if (string.IsNullOrEmpty(AccNoDesc))
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
            txtInputLayoutAccountNo.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                : Resource.Style.TextInputLayoutBottomErrorHint);

            if (txtInputLayoutAccountNo.Error != Utility.GetLocalizedLabel("SubmitEnquiry", "validElectricityAccountNoError"))
            {
                txtInputLayoutAccountNo.Error = Utility.GetLocalizedLabel("SubmitEnquiry", "validElectricityAccountNoError");
            }

        }

        public void ShowEnterOrSelectAccNumber()
        {
            txtInputLayoutAccountNo.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                : Resource.Style.TextInputLayoutBottomErrorHint);

            if (txtInputLayoutAccountNo.Error != Utility.GetLocalizedLabel("SubmitEnquiry", "plsEnterAcc"))
            {
                txtInputLayoutAccountNo.Error = Utility.GetLocalizedLabel("SubmitEnquiry", "plsEnterAcc");
            }

            txtInputLayoutAccountNo.RequestFocus();
        }


        public void RemoveNumberErrorMessage()
        {
            txtInputLayoutAccountNo.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                : Resource.Style.TextInputLayoutBottomErrorHint);

            txtInputLayoutAccountNo.Error = "";

        }

        public void ShowGeneralEnquiry()
        {
            Intent generalEnquiry = new Intent(this, typeof(FeedbackGeneralEnquiryStepOneActivity));
            generalEnquiry.PutExtra(Constants.ACCOUNT_NUMBER, txtAccountNo.Text.ToString().Trim());
            StartActivity(generalEnquiry);
        }

        public void ShowOvervoltageClaim()
        {
            Intent overvoltageClaim = new Intent(this, typeof(OvervoltageClaim));
            overvoltageClaim.PutExtra(Constants.ACCOUNT_NUMBER, txtAccountNo.Text.ToString().Trim());
            StartActivity(overvoltageClaim);
        }

        public void ShowAboutBillEnquiry()
        {
            Intent generalEnquiry = new Intent(this, typeof(FeedbackAboutBillEnquiryStepOneActivity));
            generalEnquiry.PutExtra(Constants.ACCOUNT_NUMBER, txtAccountNo.Text.ToString().Trim());
            StartActivity(generalEnquiry);
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
                            if (TriggerOVISServicesResponse.d.OvervoltageClaimSupported.ContainsKey(txtAccountNo.Text))
                            {
                                for (int i = 0; i < TriggerOVISServicesResponse.d.OvervoltageClaimSupported.Count(); i++)
                                {
                                    if (TriggerOVISServicesResponse.d.OvervoltageClaimSupported.ElementAt(i).Key == txtAccountNo.Text)
                                    {
                                        isValid = TriggerOVISServicesResponse.d.OvervoltageClaimSupported.ElementAt(i).Value;
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
                        this.userActionsListener.ValidateAccountAsync(txtAccountNo.Text.ToString().Trim(), EnquiryTypeEnum.General);
                    }
                    else
                    {
                        this.SetIsClicked(false);
                    }
                }
            }
        }

        [OnClick(Resource.Id.aboutBillEnquiryConstraint)]
        void OnAboutBillEnquiryConstraint(object sender, EventArgs eventArgs)
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
                        this.userActionsListener.ValidateAccountAsync(txtAccountNo.Text.ToString().Trim(), EnquiryTypeEnum.AboutMyBill);
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
                        this.userActionsListener.ValidateAccountAsync(txtAccountNo.Text.ToString().Trim(), EnquiryTypeEnum.UpdatePersonalDetails);
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
                //Overvoltage clickable with blue color
                if (IsWhiteListedArea)
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
                            this.userActionsListener.ValidateAccountAsync(txtAccountNo.Text.ToString().Trim(), EnquiryTypeEnum.OvervoltageClaim);
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

        [OnClick(Resource.Id.gslRebateConstraint)]
        void GSLRebateConstraintOnClick(object sender, EventArgs eventArgs)
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
                    bool isAllowed = this.userActionsListener.CheckRequiredFields(accno);

                    if (isAllowed)
                    {
                        this.SetIsClicked(true);
                        this.userActionsListener.ValidateAccountAsync(txtAccountNo.Text.ToString().Trim(), EnquiryTypeEnum.GSLRebate);
                    }
                    else
                    {
                        this.SetIsClicked(false);
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

        public void ShowGSLRebate(bool isOwner)
        {
            Intent gslRebateActivity = new Intent(this, typeof(GSLRebateStepOneActivity));
            gslRebateActivity.PutExtra(Constants.ACCOUNT_NUMBER, txtAccountNo.Text.ToString().Trim());
            gslRebateActivity.PutExtra(Constants.IS_OWNER, isOwner);
            StartActivity(gslRebateActivity);
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

        public void ShowWhereIsMyAcc()
        {
            if (BillRedesignUtility.Instance.IsAccountEligible)
            {
                MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                   .SetHeaderImage(Resource.Drawable.img_register_acct_noV2)
                   .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "accNumberInfo"))
                   .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "accNumberDetailsV2"))
                   .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                   .SetCTAaction(() => { this.SetIsClicked(false); })
                   .Build();
                whereisMyacc.Show();
            }
            else
            {
                string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC);
                if (!string.IsNullOrWhiteSpace(base64Image))
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
                {
                    this.SetIsClicked(false);
                }
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
                        if (string.IsNullOrWhiteSpace(base64Image))
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
                        if (string.IsNullOrWhiteSpace(base64Image))
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
                        if (string.IsNullOrWhiteSpace(base64Image))
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
                        if (string.IsNullOrWhiteSpace(base64Image))
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

        //public override void OnBackPressed()
        //{
        //    RestartHomeMenu();
        //}

        //public void GetStarted()
        //{
        //    Finish();
        //    StartActivity(new Intent(this, typeof(NotificationSettingsActivity)));
        //    FeedbackMenuFragment fragment = new FeedbackMenuFragment();

        //    if (((DashboardHomeActivity) != null)
        //    {
        //        DashboardHomeActivity.SetCurrentFragment(fragment);
        //        ((DashboardHomeActivity)Activity).HideAccountName();
        //        ((DashboardHomeActivity)Activity).SetToolbarTitle(Resource.String.feedback_menu_activity_title);
        //    }
        //    FragmentManager.BeginTransaction()
        //                   .Replace(Resource.Id.content_layout, fragment)
        //             .CommitAllowingStateLoss();
        //}

        //public void RestartHomeMenu()
        //{
        //    try
        //    {
        //        DashboardHomeActivity.ShowHomeDashBoard();
        //    }
        //    catch (System.Exception e)
        //    {
        //        Utility.LoggingNonFatalError(e);
        //    }
        //}
    }

}
