using Android.App;
using Android.Content;
using Android.OS;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Base.Activity;
using Android.Views;
using Android.Util;
using System;
using CheeseBind;
using Android.Widget;
using myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.SearchApplicationStatusSelection.MVP;
using Android.Runtime;
using Google.Android.Material.TextField;
using AndroidX.Core.Content;
using myTNB.Mobile;
using System.Linq;
using myTNB;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP;
using System.Text;
using Android.Text;
using myTNB_Android.Src.Database.Model;
using Android.Graphics.Drawables;
using myTNB.Mobile.API.Models.ApplicationStatus.GetApplicationsByCA;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.Adapter;
using myTNB.Mobile.SessionCache;
using Android.Content.PM;

namespace myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.MVP
{
    [Activity(Label = "Search Application Status", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.RegisterForm")]
    public class SearchApplicationStatusActivity : BaseActivityCustom, SearchApplicationStatusContract.IView, View.IOnTouchListener
    {
        [BindView(Resource.Id.txtSearchApplicationTitle)]
        TextView txtSearchApplicationTitle;

        [BindView(Resource.Id.txtInputLayoutApplicationType)]
        TextInputLayout txtInputLayoutApplicationType;

        [BindView(Resource.Id.txtApplicationType)]
        EditText txtApplicationType;

        [BindView(Resource.Id.txtInputLayoutSearchBy)]
        TextInputLayout txtInputLayoutSearchBy;

        [BindView(Resource.Id.txtSearchBy)]
        EditText txtSearchBy;

        [BindView(Resource.Id.txtInputLayoutServiceRequestNum)]
        TextInputLayout txtInputLayoutServiceRequestNum;

        [BindView(Resource.Id.txtServiceRequestNum)]
        EditText txtServiceRequestNum;

        [BindView(Resource.Id.btnSearchApplication)]
        Button btnSearchApplication;

        [BindView(Resource.Id.whyAccountsNotHere)]
        TextView txtWhyAccountsNotHere;

        [BindView(Resource.Id.whyAccountsNotHereLayOut)]
        LinearLayout whyAccountsNotHereLayOut;

        [BindView(Resource.Id.searchApplicatioStatuListResult)]
        LinearLayout searchApplicatioStatuListResult;

        [BindView(Resource.Id.searchApplicationStatusListRecyclerView)]
        RecyclerView searchApplicationStatusListRecyclerView;

        [BindView(Resource.Id.txtSearchApplicationStatusListResult)]
        TextView txtSearchApplicationStatusListResult;

        TypeModel selectedType = new TypeModel();
        const string PAGE_ID = "ApplicationStatus";

        private string targetApplicationType = string.Empty;
        private string targetApplicationTypeId = string.Empty;
        private string targetSearchBy = string.Empty;
        private string targetNumber = string.Empty;
        private bool isSearchByCA = false;
        private bool isEdiging = false;
        List<TypeModel> mTypeList = new List<TypeModel>();
        SearchByModel searchByModel = new SearchByModel();
        SearchApplicationStatusPresenter mPresenter;
        private bool isTextChange = false;
        int selectedTypeIndex = 0;
        RecyclerView.LayoutManager layoutManager;
        SearchApplicationAdapter searchApplicationAdapter;
        GetApplicationsByCAResponse applicationsByCAResponse;
        ApplicationDetailDisplay applicationDetailDisplay;
        int searchApplicationPosition;
        string ErrorMessage = string.Empty;
        public override int ResourceId()
        {
            return Resource.Layout.SearchApplicationStatusLayout;
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override View OnCreateView(string name, Context context, IAttributeSet attrs)
        {
            return base.OnCreateView(name, context, attrs);
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

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                ApplicationStatusSearchDetailCache.Instance.Clear();
                FirebaseAnalyticsUtils.SetScreenName(this, "Search Application Status");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void DisableButton()
        {
            btnSearchApplication.Enabled = false;
            btnSearchApplication.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        public void EnableButton()
        {
            btnSearchApplication.Enabled = true;
            btnSearchApplication.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
        }

        [OnClick(Resource.Id.btnSearchApplication)]
        internal void OnConfirmClickAsync(object sender, EventArgs e)
        {
            searchApplicatioStatuListResult.Visibility = ViewStates.Gone;
            if (ErrorMessage != null && ErrorMessage != string.Empty)
            {
                ShowError();
            }
            else
            {
                if (isSearchByCA)
                {
                    GetSearchByCA();
                }
                else
                {
                    GetApplicationStatus();
                }
            }
        }

        private void GetSearchByCA()
        {
            RunOnUiThread(async () =>
            {
                ShowProgressDialog();

                applicationsByCAResponse = await ApplicationStatusManager.Instance.GetApplicationByCA(txtServiceRequestNum.Text);

                if (applicationsByCAResponse != null && applicationsByCAResponse.Content != null
                    && applicationsByCAResponse.StatusDetail != null && applicationsByCAResponse.StatusDetail.IsSuccess)
                {
                    List<GetApplicationsByCAModel> innerList = new List<GetApplicationsByCAModel>();
                    innerList = applicationsByCAResponse.Content;
                    searchApplicationAdapter = new SearchApplicationAdapter(this, innerList);
                    layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                    searchApplicationStatusListRecyclerView.SetLayoutManager(layoutManager);
                    searchApplicationStatusListRecyclerView.SetAdapter(searchApplicationAdapter);
                    searchApplicationAdapter.ItemClick += OnItemClick;
                    searchApplicationAdapter.NotifyDataSetChanged();
                    searchApplicatioStatuListResult.Visibility = ViewStates.Visible;
                }
                else
                {
                    searchApplicatioStatuListResult.Visibility = ViewStates.Gone;
                    ShowApplicaitonPopupMessage(this, applicationsByCAResponse.StatusDetail);
                }
                HideProgressDialog();
            });
        }

        void OnItemClick(object sender, int position)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    searchApplicationPosition = position;
                    GetApplicationStatus();
                    this.SetIsClicked(false);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private async void GetApplicationStatus()
        {
            ShowProgressDialog();
            if (isSearchByCA)
            {
                if (applicationsByCAResponse != null && applicationsByCAResponse.Content.Count > 0)
                {
                    GetApplicationsByCAModel applicationSearch = applicationsByCAResponse.Content[searchApplicationPosition];
                    applicationDetailDisplay = await ApplicationStatusManager.Instance.GetApplicationStatus(
                         applicationSearch.ApplicationType
                       , applicationSearch.SearchType
                       , applicationSearch.BackendReferenceNo
                       , applicationSearch.ApplicationTypeDisplay
                       , applicationSearch.SearchTypeDisplay
                       , UserEntity.GetActive() != null);
                }
            }
            else
            {
                applicationDetailDisplay = await ApplicationStatusManager.Instance.GetApplicationStatus(
                      targetApplicationTypeId
                    , targetSearchBy
                    , txtServiceRequestNum.Text
                    , txtApplicationType.Text
                    , txtSearchBy.Text
                    , UserEntity.GetActive() != null);
            }

            HideProgressDialog();
            if (!applicationDetailDisplay.StatusDetail.IsSuccess)
            {
                ShowApplicaitonPopupMessage(this, applicationDetailDisplay.StatusDetail);
            }
            else
            {
                Intent applicationStatusDetailIntent = new Intent(this, typeof(ApplicationStatusDetailActivity));
                applicationStatusDetailIntent.PutExtra("applicationStatusResponse", JsonConvert.SerializeObject(applicationDetailDisplay.Content));
                StartActivityForResult(applicationStatusDetailIntent, Constants.APPLICATION_STATUS_SEARCH_DETAILS_REQUEST_CODE);
            }
        }

        public async void ShowApplicaitonPopupMessage(Android.App.Activity context, StatusDetail statusDetail)
        {
            MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(statusDetail.Title)
                .SetMessage(statusDetail.Message)
                .SetCTALabel(statusDetail.PrimaryCTATitle)
                .Build();
            errorPopup.Show();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            txtSearchApplicationTitle.Text = Utility.GetLocalizedLabel("ApplicationStatusSearch", "searchForTitle");
            txtInputLayoutApplicationType.Hint = Utility.GetLocalizedLabel("ApplicationStatusSearch", "applicationType");
            txtInputLayoutApplicationType.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
            txtInputLayoutSearchBy.Hint = Utility.GetLocalizedLabel("ApplicationStatusSearch", "searchBy");
            txtInputLayoutSearchBy.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
            btnSearchApplication.Text = Utility.GetLocalizedLabel("ApplicationStatusSearch", "searchStatus");
            txtSearchApplicationStatusListResult.Text = Utility.GetLocalizedLabel("ApplicationStatusSearch", "searchResultTitle");
            txtInputLayoutServiceRequestNum.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
            mPresenter = new SearchApplicationStatusPresenter(this);

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutApplicationType, txtInputLayoutSearchBy, txtInputLayoutServiceRequestNum);
            TextViewUtils.SetMuseoSans300Typeface(txtApplicationType, txtSearchBy, txtServiceRequestNum);
            TextViewUtils.SetMuseoSans500Typeface(btnSearchApplication, txtSearchApplicationTitle);

            txtSearchApplicationTitle.TextSize = TextViewUtils.GetFontSize(16f);
            txtApplicationType.TextSize = TextViewUtils.GetFontSize(16f);
            txtSearchBy.TextSize = TextViewUtils.GetFontSize(16f);
            txtServiceRequestNum.TextSize = TextViewUtils.GetFontSize(16f);
            txtWhyAccountsNotHere.TextSize = TextViewUtils.GetFontSize(12f);
            txtSearchApplicationStatusListResult.TextSize = TextViewUtils.GetFontSize(16f);
            btnSearchApplication.TextSize = TextViewUtils.GetFontSize(16f);
            TextViewUtils.SetMuseoSans500Typeface(txtWhyAccountsNotHere);
            txtWhyAccountsNotHere.Text = Utility.GetLocalizedLabel("ApplicationStatusSearch", "whereToGetThisNumber");
            txtWhyAccountsNotHere.Click += OnWhereAreTheseNoClick; ;
            txtWhyAccountsNotHere.Visibility = ViewStates.Gone;
            whyAccountsNotHereLayOut.Visibility = ViewStates.Gone;

            SetToolBarTitle(Utility.GetLocalizedLabel("ApplicationStatusSearch", "title"));

            txtApplicationType.AddTextChangedListener(new InputFilterFormField(txtApplicationType, txtInputLayoutApplicationType));
            txtSearchBy.AddTextChangedListener(new InputFilterFormField(txtSearchBy, txtInputLayoutSearchBy));
            txtServiceRequestNum.AddTextChangedListener(new InputFilterFormField(txtServiceRequestNum, txtInputLayoutServiceRequestNum));

            // txtSearchBy.drawable drawableLeft = "@drawable/ic_field_search"
            Drawable d = ContextCompat.GetDrawable(this, Resource.Drawable.ic_field_search);
            d.SetBounds(0, 0, d.IntrinsicWidth, d.IntrinsicHeight);

            Drawable dropdown = ContextCompat.GetDrawable(this, Resource.Drawable.ic_action_dropdown1);
            dropdown.SetBounds(0, 0, dropdown.IntrinsicWidth, dropdown.IntrinsicHeight);

            // Drawable img = (Drawable)Resource.Drawable.ic_field_search;
            txtSearchBy.SetCompoundDrawablesWithIntrinsicBounds(d, null, dropdown, null);

            Drawable accountNo = ContextCompat.GetDrawable(this, Resource.Drawable.ic_field_account_no);
            accountNo.SetBounds(0, 0, accountNo.IntrinsicWidth, accountNo.IntrinsicHeight);

            txtServiceRequestNum.SetCompoundDrawablesWithIntrinsicBounds(accountNo, null, null, null);

            Bundle extras = Intent.Extras;

            DisableButton();

            txtApplicationType.EnableClick();
            txtApplicationType.SetOnTouchListener(this);

            txtSearchBy.EnableClick();
            txtSearchBy.SetOnTouchListener(this);
            txtServiceRequestNum.AfterTextChanged += TxtServiceRequestNum_AfterTextChanged;
            txtServiceRequestNum.SetOnTouchListener(this);
            txtInputLayoutSearchBy.Visibility = ViewStates.Gone;
            txtInputLayoutServiceRequestNum.Visibility = ViewStates.Gone;

            if (extras != null && extras.ContainsKey("searchApplicationType"))
            {
                List<SearchApplicationTypeModel> searhApplicationTypeModels = new List<SearchApplicationTypeModel>();
                searhApplicationTypeModels = DeSerialze<List<SearchApplicationTypeModel>>(extras.GetString("searchApplicationType"));
                if (searhApplicationTypeModels != null)
                {
                    foreach (var searchTypeItem in searhApplicationTypeModels)
                    {
                        mTypeList.Add(new TypeModel(searchTypeItem)
                        {
                            SearchApplicationTypeId = searchTypeItem.SearchApplicationTypeId,
                            SearchApplicationTypeDesc = searchTypeItem.SearchApplicationTypeDesc,
                            ApplicationTypeDisplay = searchTypeItem.SearchApplicationTypeDescDisplay,
                            SearchTypes = searchTypeItem.SearchTypes,
                            isChecked = false
                        });
                    }
                }
            }

            txtInputLayoutServiceRequestNum.DefaultHintTextColor = ContextCompat.GetColorStateList(this, Resource.Color.silverchalice);
            txtInputLayoutServiceRequestNum.SetHelperTextColor(ContextCompat.GetColorStateList(this, Resource.Color.new_grey));
        }

        private void OnWhereAreTheseNoClick(object sender, EventArgs e)
        {
            try
            {
                if (searchByModel != null && selectedType != null && selectedType.SearchTypes != null)
                {
                    var searchType = selectedType.SearchTypes.Count == 1 ? selectedType.SearchTypes[0] : searchByModel;
                    if (searchType != null && searchType.Type == ApplicationStatusSearchType.CA)
                    {
                        MyTNBAppToolTipBuilder whereAreTheseNumbers = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                            .SetHeaderImage(Resource.Drawable.img_register_acct_no)
                            .SetTitle(Utility.GetLocalizedLabel("ApplicationStatusSearch", "whereToGetThisNumberTitleCA"))
                            .SetMessage(Utility.GetLocalizedLabel("ApplicationStatusSearch", "whereToGetThisNumberMessageCA"))
                            .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                            .Build();
                        whereAreTheseNumbers.Show();
                    }
                    else
                    {
                        MyTNBAppToolTipBuilder whereAreTheseNumbers = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                            .SetTitle(Utility.GetLocalizedLabel("ApplicationStatusSearch", "whereToGetThisNumberTitle"))
                            .SetMessage(string.Format(Utility.GetLocalizedLabel("ApplicationStatusSearch", "whereToGetThisNumberMessage"), searchType.SearchTypeDescDisplay))
                            .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                            .Build();
                        whereAreTheseNumbers.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void TxtServiceRequestNum_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            CheckError();
            EnableButton();
        }
        private void ShowError()
        {
            try
            {
               if (txtServiceRequestNum.Text != string.Empty && ErrorMessage != null && ErrorMessage != string.Empty)
                {
                    txtInputLayoutServiceRequestNum.HelperText = ErrorMessage;
                    txtInputLayoutServiceRequestNum.SetHelperTextTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }
        private void CheckError()
        {
            try
            {
                ErrorMessage = string.Empty;
                if (isTextChange && searchByModel != null && selectedType != null && selectedType.SearchTypes != null)
                {
                    var searchType = selectedType.SearchTypes.Count == 1 ? selectedType.SearchTypes[0].Type : searchByModel.Type;
                    if (searchType == ApplicationStatusSearchType.ServiceRequestNo)
                    {
                        txtServiceRequestNum.SetFilters(new IInputFilter[] { });
                        
                        if (txtServiceRequestNum.Text.Count() == 10)
                        {
                            EnableButton();
                            txtServiceRequestNum.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(10) });
                        }
                        else if (txtServiceRequestNum.Text.Count() != 10 && txtServiceRequestNum.Text != string.Empty)
                        {
                            ErrorMessage = string.Format(Utility.GetLocalizedLabel("Error", "invalidReferenceNumber"), selectedType.SearchTypes[selectedTypeIndex].SearchTypeDescDisplay);
                            DisableButton();
                        }
                    }

                    if (searchType == ApplicationStatusSearchType.CA)
                    {
                        txtServiceRequestNum.SetFilters(new IInputFilter[] { });
                        if (txtServiceRequestNum.Text.Count() == 12)
                        {
                            txtServiceRequestNum.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(12) });
                            EnableButton();
                        }
                        else if (txtServiceRequestNum.Text.Count() != 12)
                        {
                            ErrorMessage = string.Format(Utility.GetLocalizedLabel("Error", "invalidReferenceNumber"), selectedType.SearchTypes[selectedTypeIndex].SearchTypeDescDisplay);
                            DisableButton();
                        }
                    }

                    if (searchType == ApplicationStatusSearchType.ApplicationNo)
                    {
                        if (isEdiging) return;
                        isEdiging = true;

                        string format = selectedType.SearchApplicationNoInputMask;

                        txtServiceRequestNum.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(format.Length) });
                        string inputString = txtServiceRequestNum.Text.ToString();
                        int firstIndex = format.IndexOf("#");
                        int lastIndex = format.LastIndexOf("#");
                        string preffix = string.Empty;
                        string suffix = string.Empty;
                        int stringlength = 0;
                        int location = txtServiceRequestNum.SelectionStart;

                        if (!isEdiging && location > 0 && inputString[location - 1] == '-')
                        {
                            return;
                        }

                        preffix = format.Substring(0, firstIndex);
                        if (preffix.Length >= inputString.Length)
                        {
                            inputString = preffix;
                            location = preffix.Length;
                            txtServiceRequestNum.SetText(inputString, TextView.BufferType.Editable);
                        }
                        else
                        {
                            location = (location > 0 && inputString.Length > 0 && inputString[location - 1] == '-') ? location - 1 : location;
                        }
                        if (lastIndex < format.Length)
                        {
                            suffix = format.Substring(lastIndex + 1, format.Length - (lastIndex + 1));
                            stringlength = ((format.Length - preffix.Length) - suffix.Length);
                            if (format.Length - 1 == location)
                            {
                                if (suffix != string.Empty)
                                {
                                    location = stringlength + preffix.Length;
                                    inputString = inputString != string.Empty ? inputString.Substring(0, preffix.Length + stringlength) : inputString;
                                }
                            }
                        }

                        if (inputString.Length > format.Length)
                        {
                            inputString = inputString.Substring(0, stringlength + preffix.Length) + suffix;
                            location = stringlength + preffix.Length;
                        }
                        else if (suffix != string.Empty && inputString.Length < format.Length)
                        {
                            inputString = inputString.Replace(suffix, string.Empty);
                        }
                        else
                        {
                            inputString = txtServiceRequestNum.Text.ToString();
                        }
                        if (inputString.Length >= preffix.Length)
                        {
                            inputString = inputString.Remove(0, preffix.Length);
                        }
                        var txtFirstIndex = inputString.IndexOf("-");
                        var txtPreffix = txtFirstIndex > 0 ? inputString.Substring(0, txtFirstIndex) : ".";
                        int preIndexChar = txtServiceRequestNum.Text.ToString().Count(f => f == '-');
                        // removing old dashes
                        StringBuilder sb = new StringBuilder();
                        sb.Append(inputString.Replace("-", string.Empty));
                        if (!preffix.Contains(txtPreffix))
                        {
                            sb.Insert(0, preffix);
                        }
                        if (sb.Length > preffix.Length + 3)
                        {
                            sb.Insert(preffix.Length + 3, "-");

                        }
                        if (sb.Length > preffix.Length + 7)
                        {
                            sb.Insert(preffix.Length + 7, "-");

                        }
                        if (suffix != string.Empty && inputString.Contains(suffix))
                        {
                            sb = new StringBuilder();
                            sb.Append(preffix);
                            sb.Append(inputString);
                            txtServiceRequestNum.SetText(sb.ToString(), TextView.BufferType.Editable);
                        }
                        else
                        {
                            if (suffix != string.Empty && sb.Length == format.Length - suffix.Length)
                            {
                                sb.Append(suffix);
                            }
                            txtServiceRequestNum.SetText(sb.ToString(), TextView.BufferType.Editable);
                        }
                        int postIndexChar = sb.ToString().Count(f => f == '-');

                        if (preIndexChar == 1)
                        {
                            if (postIndexChar == 2 || postIndexChar == 3)
                            {
                                location += 1;
                            }
                        }
                        if (preIndexChar == 2)
                        {
                            if (postIndexChar == 3)
                            {
                                location += 1;
                            }
                        }
                        if (location == 1)
                        {

                            txtServiceRequestNum.SetSelection(preffix.Length);
                        }
                        else
                        {

                            txtServiceRequestNum.SetSelection(location);
                        }

                        if (txtServiceRequestNum.Text.Count() == format.Count())
                        {
                            EnableButton();
                        }
                        else
                        {
                                ErrorMessage = string.Format(Utility.GetLocalizedLabel("Error", "invalidReferenceNumber"), selectedType.SearchTypes[selectedTypeIndex].SearchTypeDescDisplay);
                        }
                        isEdiging = false;
                    }

                    if (searchType == ApplicationStatusSearchType.ServiceNotificationNo)
                    {
                        txtServiceRequestNum.SetFilters(new IInputFilter[] { });
                       
                        if (txtServiceRequestNum.Text.Count() == 12)
                        {
                            EnableButton();
                            txtServiceRequestNum.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(10) });
                        }
                        else if (txtServiceRequestNum.Text.Count() != 10 && txtServiceRequestNum.Text != string.Empty)
                        {
                            ErrorMessage = string.Format(Utility.GetLocalizedLabel("Error", "invalidReferenceNumber"), selectedType.SearchTypes[selectedTypeIndex].SearchTypeDescDisplay);
                            DisableButton();
                        }
                    }
                    if (searchType == ApplicationStatusSearchType.ApplicationNo)
                    {
                        txtInputLayoutServiceRequestNum.HelperText = selectedType.ApplicationNoHint;
                    }
                    else if (searchType == ApplicationStatusSearchType.ServiceNotificationNo)
                    {
                        txtInputLayoutServiceRequestNum.HelperText = Utility.GetLocalizedLabel("Hint", "serviceNotificationNo");
                    }
                    else if (searchType == ApplicationStatusSearchType.ServiceRequestNo)
                    {
                        txtInputLayoutServiceRequestNum.HelperText = Utility.GetLocalizedLabel("Hint", "serviceRequestNumber");
                    }
                    else if (searchType == ApplicationStatusSearchType.CA)
                    {
                        isSearchByCA = true;
                        txtInputLayoutServiceRequestNum.HelperText = Utility.GetLocalizedLabel("Hint", "electricityAccountNumber");
                    }
                    txtInputLayoutServiceRequestNum.SetHelperTextTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);

                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }
  
        public bool IsValid(string key)
        {
            return !string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                txtInputLayoutServiceRequestNum.SetHelperTextColor(ContextCompat.GetColorStateList(this, Resource.Color.new_grey));

                if (requestCode == Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        isTextChange = false;
                        targetSearchBy = string.Empty;
                        Bundle extra = data.Extras;
                        List<TypeModel> resultTypeList = new List<TypeModel>();

                        if (extra.ContainsKey(Constants.APPLICATION_STATUS_TYPE_LIST_KEY))
                        {
                            resultTypeList = JsonConvert.DeserializeObject<List<TypeModel>>(extra.GetString(Constants.APPLICATION_STATUS_TYPE_LIST_KEY));
                            selectedType = resultTypeList.Find(x => x.isChecked);
                            targetApplicationType = selectedType.SearchApplicationTypeDescDisplay;
                            targetApplicationTypeId = selectedType.SearchApplicationTypeId;
                            txtApplicationType.Text = targetApplicationType;

                            if (selectedType.SearchTypes.Count <= 1)
                            {
                                txtInputLayoutSearchBy.Visibility = ViewStates.Gone;
                                txtInputLayoutServiceRequestNum.Visibility = ViewStates.Visible;
                                txtInputLayoutServiceRequestNum.ClearFocus();
                                txtServiceRequestNum.Text = null;
                                txtServiceRequestNum.ClearFocus();
                                txtInputLayoutServiceRequestNum.Hint = selectedType.SearchTypes[0].SearchTypeDescDisplay;
                                txtInputLayoutServiceRequestNum.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
                                txtWhyAccountsNotHere.Visibility = ViewStates.Visible;
                                whyAccountsNotHereLayOut.Visibility = ViewStates.Visible;
                                if (selectedType.SearchTypes[0].Type == ApplicationStatusSearchType.ApplicationNo)
                                {
                                    txtInputLayoutServiceRequestNum.HelperText = selectedType.ApplicationNoHint;
                                    txtInputLayoutServiceRequestNum.SetHelperTextTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
                                    txtSearchBy.Text = selectedType.SearchTypes[0].SearchTypeDescDisplay;
                                    selectedTypeIndex = 0;
                                    targetSearchBy = selectedType.SearchTypes[0].SearchTypeId;
                                }
                                if (selectedType.SearchTypes[0].Type == ApplicationStatusSearchType.CA)
                                {
                                    isSearchByCA = true;
                                    txtInputLayoutServiceRequestNum.HelperText = Utility.GetLocalizedLabel("Hint", "electricityAccountNumber");
                                    txtInputLayoutServiceRequestNum.SetHelperTextTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
                                }

                            }
                            else
                            {
                                txtSearchBy.Text = string.Empty;
                                txtInputLayoutServiceRequestNum.Visibility = ViewStates.Gone;
                                txtInputLayoutSearchBy.Visibility = ViewStates.Visible;
                                txtWhyAccountsNotHere.Visibility = ViewStates.Gone;
                                whyAccountsNotHereLayOut.Visibility = ViewStates.Gone;
                            }
                        }
                    }
                }
                else if (requestCode == Constants.APPLICATION_STATUS_FILTER_SEARCH_BY_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        isTextChange = false;
                        Bundle extra = data.Extras;
                        List<SearchByModel> resultSearchByList = new List<SearchByModel>();

                        if (extra.ContainsKey(Constants.APPLICATION_STATUS_SEARCH_BY_LIST_KEY))
                        {
                            resultSearchByList = JsonConvert.DeserializeObject<List<SearchByModel>>(extra.GetString(Constants.APPLICATION_STATUS_SEARCH_BY_LIST_KEY));
                            searchByModel = resultSearchByList.Find(x => x.isChecked);
                            targetSearchBy = searchByModel.SearchTypeId;
                            txtSearchBy.Text = searchByModel.SearchTypeDescDisplay;
                            selectedTypeIndex = resultSearchByList.IndexOf(searchByModel);
                            txtInputLayoutServiceRequestNum.Visibility = ViewStates.Visible;

                            txtServiceRequestNum.Text = null;
                            txtServiceRequestNum.ClearFocus();
                            txtServiceRequestNum.SetText(string.Empty, TextView.BufferType.Editable);

                            txtInputLayoutServiceRequestNum.Hint = searchByModel.SearchTypeDescDisplay;
                            txtInputLayoutServiceRequestNum.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
                            if (searchByModel.Type == ApplicationStatusSearchType.ApplicationNo)
                            {
                                txtInputLayoutServiceRequestNum.HelperText = selectedType.ApplicationNoHint;
                            }
                            else if (searchByModel.Type == ApplicationStatusSearchType.ServiceNotificationNo)
                            {
                                txtInputLayoutServiceRequestNum.HelperText = Utility.GetLocalizedLabel("Hint", "serviceNotificationNo");
                            }
                            else if (searchByModel.Type == ApplicationStatusSearchType.ServiceRequestNo)
                            {
                                txtInputLayoutServiceRequestNum.HelperText = Utility.GetLocalizedLabel("Hint", "serviceRequestNumber");
                            }
                            else if (searchByModel.Type == ApplicationStatusSearchType.CA)
                            {
                                isSearchByCA = true;
                                txtInputLayoutServiceRequestNum.HelperText = Utility.GetLocalizedLabel("Hint", "electricityAccountNumber");
                            }
                            txtInputLayoutServiceRequestNum.SetHelperTextTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
                        }
                    }
                }

                Drawable accountNo = ContextCompat.GetDrawable(this, Resource.Drawable.ic_field_account_no);
                accountNo.SetBounds(0, 0, accountNo.IntrinsicWidth, accountNo.IntrinsicHeight);
                txtServiceRequestNum.SetCompoundDrawablesWithIntrinsicBounds(accountNo, null, null, null);

                if (requestCode == Constants.APPLICATION_STATUS_SEARCH_DETAILS_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        Finish();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            txtInputLayoutServiceRequestNum.SetHelperTextColor(ContextCompat.GetColorStateList(this, Resource.Color.new_grey));
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (v is EditText)
            {
                txtInputLayoutServiceRequestNum.SetHelperTextColor(ContextCompat.GetColorStateList(this, Resource.Color.new_grey));
                EditText eTxtView = v as EditText;
                if (eTxtView.Id == Resource.Id.txtApplicationType)
                {
                    if (e.Action == MotionEventActions.Up)
                    {
                        if (!this.GetIsClicked())
                        {
                            this.SetIsClicked(true);
                            Intent filterIntent = new Intent(this, typeof(SearchApplicationStatusSelectionActivity));
                            var listShowing = mTypeList;
                            for (int i = 0; i < listShowing.Count; i++)
                            {
                                listShowing[i].isChecked = false;
                                listShowing[i].ApplicationTypeDisplay = listShowing[i].SearchApplicationTypeDescDisplay;
                            }
                            if (!string.IsNullOrEmpty(targetApplicationType))
                            {
                                for (int i = 0; i < listShowing.Count; i++)
                                {
                                    if (listShowing[i].SearchApplicationTypeDescDisplay == targetApplicationType)
                                    {
                                        listShowing[i].isChecked = true;
                                        listShowing[i].ApplicationTypeDisplay = listShowing[i].SearchApplicationTypeDescDisplay;
                                        break;
                                    }
                                }
                            }
                            filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_REQUEST_KEY, Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE);
                            filterIntent.PutExtra(Constants.APPLICATION_STATUS_TYPE_LIST_KEY, JsonConvert.SerializeObject(listShowing));
                            StartActivityForResult(filterIntent, Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE);
                        }
                        return true;
                    }
                }
                else if (eTxtView.Id == Resource.Id.txtSearchBy)
                {
                    if (e.Action == MotionEventActions.Up)
                    {
                        if (!this.GetIsClicked())
                        {
                            this.SetIsClicked(true);
                            Intent filterIntent = new Intent(this, typeof(SearchApplicationStatusSelectionActivity));
                            List<SearchByModel> mList = new List<SearchByModel>();
                            if (!string.IsNullOrEmpty(targetApplicationTypeId))
                            {
                                for (int i = 0; i < mTypeList.Count; i++)
                                {
                                    if (mTypeList[i].SearchApplicationTypeId == targetApplicationTypeId)
                                    {
                                        for (int j = 0; j < mTypeList[i].SearchTypes.Count; j++)
                                        {
                                            var foundSearchBy = mTypeList[i].SearchTypes[j];
                                            mList.Add(new SearchByModel(foundSearchBy)
                                            {
                                                SearchTypeId = foundSearchBy.SearchTypeId,
                                                SearchTypeDesc = foundSearchBy.SearchTypeDesc,
                                                SearchTypeDisplay = foundSearchBy.SearchTypeDescDisplay,
                                                isChecked = false
                                            });
                                        }
                                        break;
                                    }
                                }
                            }

                            for (int i = 0; i < mList.Count; i++)
                            {
                                mList[i].isChecked = false;
                            }

                            if (!string.IsNullOrEmpty(targetSearchBy) && mList != null && mList.Count > 0)
                            {
                                for (int i = 0; i < mList.Count; i++)
                                {
                                    if (mList[i].SearchTypeId == targetSearchBy)
                                    {
                                        mList[i].isChecked = true;
                                        break;
                                    }
                                }
                            }
                            filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_REQUEST_KEY, Constants.APPLICATION_STATUS_FILTER_SEARCH_BY_REQUEST_CODE);
                            filterIntent.PutExtra(Constants.APPLICATION_STATUS_SEARCH_BY_LIST_KEY, JsonConvert.SerializeObject(mList));
                            StartActivityForResult(filterIntent, Constants.APPLICATION_STATUS_FILTER_SEARCH_BY_REQUEST_CODE);
                        }
                        return true;
                    }
                }
                else if (eTxtView.Id == Resource.Id.txtServiceRequestNum)
                {
                    isTextChange = true;
                    if (searchByModel != null && selectedType != null && selectedType.SearchTypes != null)
                    {
                        var searchType = selectedType.SearchTypes.Count == 1 ? selectedType.SearchTypes[0].Type : searchByModel.Type;
                        txtInputLayoutServiceRequestNum.Hint = selectedType.SearchTypes.Count == 1 ? selectedType.SearchTypes[0].SearchTypeDescDisplay.ToUpper() : searchByModel.SearchTypeDescDisplay.ToUpper();
                        txtInputLayoutServiceRequestNum.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
                        txtInputLayoutServiceRequestNum.DefaultHintTextColor = ContextCompat.GetColorStateList(this, Resource.Color.silverchalice);
                        txtInputLayoutServiceRequestNum.SetHelperTextColor(ContextCompat.GetColorStateList(this, Resource.Color.new_grey));


                        if (searchType == ApplicationStatusSearchType.ApplicationNo)
                        {
                            txtInputLayoutServiceRequestNum.HelperText = selectedType.ApplicationNoHint;
                            string format = selectedType.SearchApplicationNoInputMask;
                            string inputString = txtServiceRequestNum.Text.ToString();
                            int firstIndex = format.IndexOf("#");

                            string preffix = firstIndex > -1 ? format.Substring(0, firstIndex) : string.Empty;
                            if (preffix.Length >= inputString.Length)
                            {
                                inputString = preffix;
                                txtServiceRequestNum.SetText(inputString, TextView.BufferType.Editable);
                                txtServiceRequestNum.SetSelection(preffix.Length);
                            }
                        }
                        if (searchType == ApplicationStatusSearchType.ServiceNotificationNo)
                        {
                            txtInputLayoutServiceRequestNum.HelperText = Utility.GetLocalizedLabel("Hint", "serviceNotificationNo");
                        }
                        else if (searchType == ApplicationStatusSearchType.ServiceRequestNo)
                        {
                            txtInputLayoutServiceRequestNum.HelperText = Utility.GetLocalizedLabel("Hint", "serviceRequestNumber");
                        }
                        else if (searchType == ApplicationStatusSearchType.CA)
                        {
                            isSearchByCA = true;
                            txtInputLayoutServiceRequestNum.HelperText = Utility.GetLocalizedLabel("Hint", "electricityAccountNumber");
                        }
                        txtInputLayoutServiceRequestNum.SetHelperTextTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);}
                }
            }
            CheckError();
            txtInputLayoutServiceRequestNum.SetHelperTextColor(ContextCompat.GetColorStateList(this, Resource.Color.new_grey));
            txtInputLayoutServiceRequestNum.SetHelperTextTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
            return false;
        }
    }
}