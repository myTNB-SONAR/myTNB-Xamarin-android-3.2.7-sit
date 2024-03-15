using System;
using Android.App;
using System.Reflection.Emit;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.FloatingButtonMarketing.MVP;
using Android.Content.PM;
using Android.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.Bills.NewBillRedesign.MVP;
using Android.OS;
using myTNB.AndroidApp.Src.Utils;
using myTNB.AndroidApp.Src.Database.Model;
using System.Collections.Generic;
using Android.Preferences;
using Android.Views;
using static myTNB.SitecoreCMS.Constants.Sitecore;
using Android.Text;
using Android.Text.Style;
using myTNB.SitecoreCMS.Model;
using Newtonsoft.Json;
using myTNB.AndroidApp.Src.SitecoreCMS.Model;
using Android.Support.Design.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using myTNB.AndroidApp.Src.myTNBMenu.MVP;
using Facebook.Shimmer;
using myTNB.AndroidApp.Src.Utils.PDFView;
using myTNB.AndroidApp.Src.Utils.ZoomImageView;
using System.IO;
using myTNB.AndroidApp.Src.ManageBillDelivery.MVP;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Activity;
using Android.Content;
using myTNB.Mobile.AWS.Models;
using myTNB.Mobile.AWS.Models.DBR;
using Java.Security.Acl;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.Mobile;
using myTNB.AndroidApp.Src.DeviceCache;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.EnergyBudget.Activity;
using myTNB.AndroidApp.Src.myTNBMenu.Activity;
using myTNB.AndroidApp.Src.SSMR.SMRApplication.MVP;
using myTNB.AndroidApp.Src.ViewBill.Activity;
using Android.Runtime;

namespace myTNB.AndroidApp.Src.FloatingButtonMarketing.Activity
{
    [Activity(Label = "", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class FloatingButtonMarketingActivity : BaseToolbarAppCompatActivity, FloatingButtonMarketingContract.IView
    {
        [BindView(Resource.Id.FBMarketingContainer)]
        LinearLayout FBMarketingContainer;

        [BindView(Resource.Id.txtDescription)]
        TextView txtDescription;

        [BindView(Resource.Id.contentFullImageDetailLayout)]
        LinearLayout contentFullImageDetailLayout;

        [BindView(Resource.Id.contentFullScreenShimmerLayout)]
        LinearLayout contentFullScreenShimmerLayout;

        [BindView(Resource.Id.shimmerFullScreenLayout)]
        ShimmerFrameLayout shimmerFullScreenLayout;

        [BindView(Resource.Id.imgFullView)]
        ZoomImageView imgFullView;

        [BindView(Resource.Id.contentFullPDFDetailLayout)]
        LinearLayout contentFullPDFDetailLayout;

        [BindView(Resource.Id.pdfFullView)]
        PDFView pdfFullView;

        [BindView(Resource.Id.btnIntentContent)]
        Button btnIntentContent;


        private const string PAGE_ID = "FloatingButtonMarketing";
        private string ItemID = "";
        private string Title = "";
        private bool isFloatingButtonSiteCoreDone = false;
        private Snackbar mNoInternetSnackbar;
        private string savedFBContentTimeStamp = "0000000";
        internal static readonly int SELECT_SM_POPUP_REQUEST_CODE = 8810;

        private FloatingButtonMarketingContract.IUserActionsListener userActionsListener;
        public FloatingButtonMarketingPresenter mPresenter;
        private FloatingButtonMarketingModel LocalItem = new FloatingButtonMarketingModel();
        //private PostBREligibilityIndicatorsResponse _billRenderingTenantResponse;
        private string _accountNumber = string.Empty;
        private AccountData mSelectedAccountData;
        private string selectedAccountNumber;
        private bool _isOwner { get; set; }
        GetBillRenderingResponse billRenderingResponse;

        protected override void OnCreate(Bundle savedInstanceState)
        {
           
            base.OnCreate(savedInstanceState);
           
            Bundle extras = Intent.Extras;
            mPresenter = new FloatingButtonMarketingPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));
            SetTheme(TextViewUtils.IsLargeFonts ? Resource.Style.Theme_DashboardLarge : Resource.Style.Theme_Dashboard);
            try
            {
                SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                TextViewUtils.SetMuseoSans500Typeface(txtDescription,btnIntentContent);
                TextViewUtils.SetTextSize16(txtDescription,btnIntentContent);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            if (extras != null)
            {
                if (extras.ContainsKey("accountNumber"))
                {
                    _accountNumber = extras.GetString("accountNumber");
                    List<CustomerBillingAccount> allAccountList = CustomerBillingAccount.List();
                    int accountIndex = allAccountList.FindIndex(x => x.AccNum == _accountNumber);
                    if (accountIndex > -1)
                    {
                        mSelectedAccountData = AccountData.Copy(allAccountList[accountIndex], true);
                        selectedAccountNumber = mSelectedAccountData.AccountNum;

                        CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(selectedAccountNumber);
                        _isOwner = account.isOwned && DBRUtility.Instance.IsCAEligible(selectedAccountNumber);
                    }
                }
               
                //if (extras.ContainsKey("billRenderingTenantResponse"))
                //{
                //    _billRenderingTenantResponse = JsonConvert.DeserializeObject<PostBREligibilityIndicatorsResponse>(extras.GetString("billRenderingTenantResponse"));
                //}
            }

            GetBillRenderingAsync(mSelectedAccountData);
            SetupContentDetails();
            
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        //public override string GetPageId()
        //{
        //    return PAGE_ID;
        //}

        public override int ResourceId()
        {
            return Resource.Layout.FloatingButtonMarketingView;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public string GetLocalItemID()
        {
            return LocalItem.ID;
        }


        private void SetupContentDetails()
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {

                        if (FloatingButtonMarketingUtils.GetContent() != null)
                        {
                            this.userActionsListener.GetFBMarketingContent(FloatingButtonMarketingUtils.GetContent().ID);
                        }
                        else
                        {
                            if (!isFloatingButtonSiteCoreDone)
                            {
                                this.userActionsListener.GetSavedFBContentTimeStamp();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private async void GetBillRenderingAsync(AccountData selectedAccount)
        {
            try
            {
                if (DBRUtility.Instance.IsAccountEligible && DBRUtility.Instance.IsCAEligible(selectedAccount.AccountNum))
                {
                    AccountData dbrAccount = selectedAccount;
                    if (!AccessTokenCache.Instance.HasTokenSaved(this))
                    {
                        string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
                        AccessTokenCache.Instance.SaveAccessToken(this, accessToken);
                    }
                    billRenderingResponse = await DBRManager.Instance.GetBillRendering(dbrAccount.AccountNum, AccessTokenCache.Instance.GetAccessToken(this), selectedAccount.IsOwner);
                    ////Nullity Check
                    //if (billrenderingresponse != null
                    //   && billrenderingresponse.StatusDetail != null
                    //   && billrenderingresponse.StatusDetail.IsSuccess)
                    //{
                    //    intent.PutExtra("billrenderingresponse", JsonConvert.SerializeObject(billrenderingresponse));
                    //}
                }
                //StartActivity(intent);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            HideProgressDialog();
        }

        //public void SetToolBarContentTitle(FloatingButtonMarketingModel item)
        //{
        //    SetToolBarTitle(item.Title);
        //}

        //private async void OnSetupFloatingButtonMarketing()
        //{
        //    //floatingbutton
        //    try
        //    {
        //        try
        //        {
        //            if (FloatingButtonMarketingUtils.GetContent() != null)
        //            {
        //                //SetFBMarketingContent(FloatingButtonMarketingUtils.GetContent());
        //                this.userActionsListener.GetFBMarketingContent(FloatingButtonMarketingUtils.GetContent().ID);
        //            }
        //            else
        //            {
        //                if (!isFloatingButtonSiteCoreDone)
        //                {
        //                    this.userActionsListener.GetSavedFBContentTimeStamp();
        //                }
        //            }
        //        }
        //        catch (Exception ne)
        //        {
        //            Utility.LoggingNonFatalError(ne);
        //        }
        //    }
        //    catch (Exception ne)
        //    {
        //        Utility.LoggingNonFatalError(ne);
        //    }

        //}

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

        public void OnSavedFBContentTimeStampRecieved(string timestamp)
        {
            try
            {
                if (timestamp != null)
                {
                    savedFBContentTimeStamp = timestamp;
                }
                this.userActionsListener.OnGetFBContentTimeStamp();
            }
            catch (Exception e)
            {
                this.userActionsListener.OnGetFBContentCache();
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnFBContentTimeStampRecieved(string timestamp)
        {
            try
            {
                if (timestamp != null)
                {
                    if (timestamp.Equals(savedFBContentTimeStamp))
                    {
                        this.userActionsListener.OnGetFBContentCache();
                    }
                    else
                    {
                        this.userActionsListener.OnGetFBContentItem();
                    }
                }
                else
                {
                    this.userActionsListener.OnGetFBContentCache();
                }
            }
            catch (Exception e)
            {
                this.userActionsListener.OnGetFBContentCache();
                Utility.LoggingNonFatalError(e);
            }
        }

       

        public void SetFBMarketingDetail(FloatingButtonMarketingModel item)
        {
            try
            {
                if (item != null)
                {
                    LocalItem = item;
                    SetToolBarTitle(item.Title);
                    btnIntentContent.Text = item.ButtonTitle;
                    txtDescription.TextFormatted = GetFormattedTextNoExtraSpacing(item.Description);

                    if (item.Description != null)
                    {

                        txtDescription = LinkRedirectionUtils
                            .Create(this, item.Title)
                            .SetTextView(txtDescription)
                            .SetMessage(item.Description)
                            .SetAction(HideNoInternetSnackbar)
                            .Build()
                            .GetProcessedTextView();
                    }

                    if (item.Description != null && (item.Description.Contains("<img")))
                    {
                        if (!string.IsNullOrEmpty(item.Description_Images))
                        {
                            try
                            {
                                List<FBMarketingDetailImageDBModel> dbList = JsonConvert.DeserializeObject<List<FBMarketingDetailImageDBModel>>(item.Description_Images);

                                if (dbList.Count > 0)
                                {
                                    _ = this.userActionsListener.ProcessContentImage(dbList);
                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
                                Utility.LoggingNonFatalError(ex);
                            }
                        }

                        List<FBMarketingDetailImageModel> containedImage = this.userActionsListener.ExtractImage(item.Description);
                        if (containedImage.Count > 0)
                        {
                            try
                            {
                                SpannableString contentDetailString = new SpannableString(txtDescription.TextFormatted);
                                var imageSpans = contentDetailString.GetSpans(0, contentDetailString.Length(), Java.Lang.Class.FromType(typeof(ImageSpan)));
                                if (imageSpans != null && imageSpans.Length > 0)
                                {
                                    List<Bitmap> mShimmerBitmapList = new List<Bitmap>();
                                    Drawable mShimmerDrawable = ContextCompat.GetDrawable(this, Resource.Drawable.shimmer_rectangle);
                                    string urlHeightWidthRegex = "(<img\\b|(?!^)\\G)[^>]*?\\b(src|width|height)=([\"']?)([^\"]*)\\3";
                                    System.Text.RegularExpressions.MatchCollection matcheImgSrc = System.Text.RegularExpressions.Regex.Matches(item.Description, urlHeightWidthRegex, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
                                    bool foundWidth = false;
                                    bool foundHeight = false;
                                    float textImgWidth = 0;
                                    float textImgHeight = 0;
                                    for (int index = 0; index < matcheImgSrc.Count; index++)
                                    {
                                        if (matcheImgSrc[index].Groups[2].Value == "width")
                                        {
                                            if (foundHeight)
                                            {
                                                textImgWidth = float.Parse(matcheImgSrc[index].Groups[4].Value);
                                                float deviceWidth = this.Resources.DisplayMetrics.WidthPixels - DPUtils.ConvertDPToPx(36f);
                                                float calImgRatio = deviceWidth / textImgWidth;
                                                float deviceHeight = textImgHeight * calImgRatio;

                                                mShimmerBitmapList.Add(BitmapUtils.CreateBitmapFromDrawable(mShimmerDrawable, (int)deviceWidth, (int)deviceHeight));

                                                foundHeight = false;
                                            }
                                            else
                                            {
                                                foundWidth = true;
                                                textImgWidth = float.Parse(matcheImgSrc[index].Groups[4].Value);
                                            }
                                        }
                                        else if (matcheImgSrc[index].Groups[2].Value == "height")
                                        {
                                            if (foundWidth)
                                            {
                                                textImgHeight = float.Parse(matcheImgSrc[index].Groups[4].Value);
                                                float deviceWidth = this.Resources.DisplayMetrics.WidthPixels - DPUtils.ConvertDPToPx(36f);
                                                float calImgRatio = deviceWidth / textImgWidth;
                                                float deviceHeight = textImgHeight * calImgRatio;

                                                mShimmerBitmapList.Add(BitmapUtils.CreateBitmapFromDrawable(mShimmerDrawable, (int)deviceWidth, (int)deviceHeight));

                                                foundWidth = false;
                                            }
                                            else
                                            {
                                                foundHeight = true;
                                                textImgHeight = float.Parse(matcheImgSrc[index].Groups[4].Value);
                                            }
                                        }
                                    }

                                    for (int j = 0; j < imageSpans.Length; j++)
                                    {
                                        ImageSpan imageSpan = new ImageSpan(this, mShimmerBitmapList[j], SpanAlign.Baseline);
                                        ImageSpan ImageItem = imageSpans[j] as ImageSpan;
                                        int startIndex = contentDetailString.GetSpanStart(imageSpans[j]);
                                        int endIndex = contentDetailString.GetSpanEnd(imageSpans[j]);
                                        contentDetailString.RemoveSpan(imageSpans[j]);
                                        contentDetailString.SetSpan(imageSpan, startIndex, endIndex, SpanTypes.ExclusiveExclusive);
                                    }

                                    txtDescription.TextFormatted = contentDetailString;
                                }
                            }
                            catch (Exception ex)
                            {
                                Utility.LoggingNonFatalError(ex);
                            }

                            _ = this.userActionsListener.FetchContentImage(containedImage);
                        }
                    }
                }
                else
                {
                    this.Finish();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetContentDetailImage(List<FBMarketingDetailImageModel> containedImage)
        {
            try
            {
               
                SpannableString contentString = new SpannableString(txtDescription.TextFormatted);

                var imageSpans = contentString.GetSpans(0, contentString.Length(), Java.Lang.Class.FromType(typeof(ImageSpan)));

                if (imageSpans != null && imageSpans.Length > 0)
                {
                    for (int j = 0; j < imageSpans.Length; j++)
                    {
                        if (containedImage[j].ExtractedImageBitmap != null)
                        {
                            float currentImgWidth = this.Resources.DisplayMetrics.WidthPixels - DPUtils.ConvertDPToPx(36f);
                            float calImgRatio = currentImgWidth / containedImage[j].ExtractedImageBitmap.Width;
                            int currentImgHeight = (int)(containedImage[j].ExtractedImageBitmap.Height * calImgRatio);
                            ImageSpan imageSpan = new ImageSpan(this, Bitmap.CreateScaledBitmap(containedImage[j].ExtractedImageBitmap, (int)currentImgWidth, currentImgHeight, false), SpanAlign.Baseline);
                            ImageSpan ImageItem = imageSpans[j] as ImageSpan;
                            int startIndex = contentString.GetSpanStart(imageSpans[j]);
                            int endIndex = contentString.GetSpanEnd(imageSpans[j]);
                            contentString.RemoveSpan(imageSpans[j]);
                            contentString.SetSpan(imageSpan, startIndex, endIndex, SpanTypes.ExclusiveExclusive);
                        }
                    }
                   
                    txtDescription.TextFormatted = contentString;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnUpdateFullScreenPdf(string path)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        //fullScreenFirstLoaded = true;
                        StopFullScreenShimmer();
                        contentFullPDFDetailLayout.Visibility = ViewStates.Visible;

                        Java.IO.File file = new Java.IO.File(path);

                        pdfFullView
                            .FromFile(file)
                            .Show();
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnUpdateFullScreenImage(Bitmap fullBitmap)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        //fullScreenFirstLoaded = true;
                        StopFullScreenShimmer();
                        contentFullImageDetailLayout.Visibility = ViewStates.Visible;

                        imgFullView
                            .FromBitmap(fullBitmap)
                            .Show();
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public string GenerateTmpFilePath()
        {
            string path = "";
            try
            {
                string rootPath = this.FilesDir.AbsolutePath;

                if (Utils.FileUtils.IsExternalStorageReadable() && Utils.FileUtils.IsExternalStorageWritable())
                {
                    rootPath = this.GetExternalFilesDir(null).AbsolutePath;
                }

                var directory = System.IO.Path.Combine(rootPath, "pdf");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string filename = "tmpFloatingBtnMarketing.pdf";
                path = System.IO.Path.Combine(directory, filename);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return path;
        }

        public void UpdateContentDetail(FloatingButtonMarketingModel item)
        {
            try
            {
                if (item != null)
                {
                    LocalItem = item;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetupFullScreenShimmer()
        {
            try
            {
                contentFullScreenShimmerLayout.Visibility = ViewStates.Visible;

                if (shimmerFullScreenLayout.IsShimmerStarted)
                {
                    shimmerFullScreenLayout.StopShimmer();
                }
                var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                if (shimmerBuilder != null)
                {
                    shimmerFullScreenLayout.SetShimmer(shimmerBuilder?.Build());
                }
                shimmerFullScreenLayout.StartShimmer();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void StopFullScreenShimmer()
        {
            try
            {
                //FBMarketingContainer.Visibility = ViewStates.Visible;
                contentFullScreenShimmerLayout.Visibility = ViewStates.Gone;
                if (shimmerFullScreenLayout.IsShimmerStarted)
                {
                    shimmerFullScreenLayout.StopShimmer();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideNoInternetSnackbar()
        {
            try
            {
                if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
                {
                    mNoInternetSnackbar.Dismiss();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnIntentContent)]
        void OnSelectContent(object sender, EventArgs eventArgs)
        {
            FloatingButtonEntity wtManager = new FloatingButtonEntity();
            List<FloatingButtonEntity> contentList = wtManager.GetAllItems();

            if (contentList[0].Title == Module.TNG.ToString())
            {
                if (!string.IsNullOrEmpty(contentList[0].Description))
                {

                    if (!UserSessions.HasPayBillShown(PreferenceManager.GetDefaultSharedPreferences(this)))
                    {
                        UserSessions.DoPayBillShown(PreferenceManager.GetDefaultSharedPreferences(this));
                    }
                    Intent payment_activity = new Intent(this, typeof(SelectAccountsActivity));
                    payment_activity.PutExtra("FromFloatingButtonMarketing", true);
                    StartActivity(payment_activity);
                    Intent result = new Intent();
                    //result.PutExtra("ContentTNG", "ContentTNG");
                    //SetResult(Result.Ok, result);
                    //Finish();


                }

            }
            else if (contentList[0].Title == Module.DBR.ToString())
            {
                if (!string.IsNullOrEmpty(contentList[0].Description))
                {
                    try
                    {
                        Intent intent = new Intent(this, typeof(ManageBillDeliveryActivity));
                        intent.PutExtra("accountNumber", selectedAccountNumber);
                        intent.PutExtra("billRenderingResponse", JsonConvert.SerializeObject(billRenderingResponse));
                        //intent.PutExtra("billRenderingTenantResponse", JsonConvert.SerializeObject(_billRenderingTenantResponse));
                        intent.PutExtra("FromFloatingButtonMarketing", true);
                        StartActivity(intent);
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }

            }
        }

        public void SetPresenter(FloatingButtonMarketingContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }
    }
}

