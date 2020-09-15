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

namespace myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.MVP
{
    [Activity(Label = "Search Application Status", Theme = "@style/Theme.RegisterForm")]
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

        TypeModel selectedType = new TypeModel();
        const string PAGE_ID = "ApplicationStatus";

        private string targetApplicationType = "";
        private string targetApplicationTypeId = "";
        private string targetSearchBy = "";
        private string targetNumber = "";
        private bool isEdiging = false;
        List<TypeModel> mTypeList = new List<TypeModel>();
        List<SearchByModel> mSearchByList = new List<SearchByModel>();

        SearchApplicationStatusPresenter mPresenter;

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
            GetApplicationStatus();

        }
        private async void GetApplicationStatus()
        {
            GetApplicationStatusResponse applicationStatusResponse = await ApplicationStatusManager.Instance.GetApplicationStatus("ASR", "ApplicationNo", "362");


            Intent applicationStatusDetailIntent = new Intent(this, typeof(ApplicationStatusDetailActivity));
            applicationStatusDetailIntent.PutExtra("applicationStatusResponse", JsonConvert.SerializeObject(applicationStatusResponse.Content));
            StartActivity(applicationStatusDetailIntent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            txtSearchApplicationTitle.Text = Utility.GetLocalizedLabel("ApplicationStatusSearch", "searchForTitle");
            txtSearchBy.Text = Utility.GetLocalizedLabel("ApplicationStatusSearch", "searchBy");
            txtApplicationType.Text = Utility.GetLocalizedLabel("ApplicationStatusSearch", "applicationType");
            btnSearchApplication.Text = Utility.GetLocalizedLabel("ApplicationStatusSearch", "searchStatus");

            mPresenter = new SearchApplicationStatusPresenter(this);

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutApplicationType, txtInputLayoutSearchBy, txtInputLayoutServiceRequestNum);
            TextViewUtils.SetMuseoSans300Typeface(txtApplicationType, txtSearchBy, txtServiceRequestNum);
            TextViewUtils.SetMuseoSans500Typeface(btnSearchApplication, txtSearchApplicationTitle);

            //  TODO: ApplicationStatus Multilingual
            SetToolBarTitle(Utility.GetLocalizedLabel("ApplicationStatusSearch", "title"));
            // txtInputLayoutFromDate.Hint = GetLabelCommonByLanguage("email");
            // txtInputLayoutToDate.Hint = GetLabelCommonByLanguage("password");

            txtApplicationType.AddTextChangedListener(new InputFilterFormField(txtApplicationType, txtInputLayoutApplicationType));
            txtSearchBy.AddTextChangedListener(new InputFilterFormField(txtSearchBy, txtInputLayoutSearchBy));
            txtServiceRequestNum.AddTextChangedListener(new InputFilterFormField(txtServiceRequestNum, txtInputLayoutServiceRequestNum));

            Bundle extras = Intent.Extras;

            DisableButton();

            txtApplicationType.EnableClick();
            txtApplicationType.SetOnTouchListener(this);

            txtSearchBy.EnableClick();
            txtSearchBy.SetOnTouchListener(this);
            txtServiceRequestNum.AfterTextChanged += TxtServiceRequestNum_AfterTextChanged;
            txtInputLayoutSearchBy.Visibility = ViewStates.Gone;
            txtInputLayoutServiceRequestNum.Visibility = ViewStates.Gone;

            //  TODO: ApplicationStatus Mock

            if (extras != null)
            {
                if (extras.ContainsKey("searchApplicationType"))
                {
                    List<SearhApplicationTypeModel> searhApplicationTypeModels = new List<SearhApplicationTypeModel>();
                    searhApplicationTypeModels = DeSerialze<List<SearhApplicationTypeModel>>(extras.GetString("searchApplicationType"));

                    foreach (var searchTypeItem in searhApplicationTypeModels)
                    {
                        mTypeList.Add(new TypeModel(searchTypeItem)
                        {
                            SearchApplicationTypeId = searchTypeItem.SearchApplicationTypeId,
                            SearchApplicationTypeDesc = searchTypeItem.SearchApplicationTypeDesc,
                            SearchTypes = searchTypeItem.SearchTypes,
                            isChecked = false
                        });
                    }

                }
            }
            //mTypeList = JsonConvert.DeserializeObject<List<TypeModel>>("[{\"Title\":\"Change of Tenancy\",\"Code\":\"\",\"SearchBy\":[\"AN\",\"EAN\",\"SNN\",\"SRN\"]},{\"Title\":\"Change Tariff\",\"Code\":\"\",\"SearchBy\":[\"AN\",\"EAN\",\"SNN\",\"SRN\"]},{\"Title\":\"Project\",\"Code\":\"\",\"SearchBy\":[\"AN\"]},{\"Title\":\"Renewable Energy\",\"Code\":\"\",\"SearchBy\":[\"AN\",\"EAN\",\"SNN\",\"SRN\"]},{\"Title\":\"Self Meter Reading\",\"Code\":\"\",\"SearchBy\":[\"AN\",\"EAN\",\"SNN\",\"SRN\"]},{\"Title\":\"Start Electricity\",\"Code\":\"\",\"SearchBy\":[\"AN\",\"EAN\",\"SNN\",\"SRN\"]},{\"Title\":\"Stop Electricity\",\"Code\":\"\",\"SearchBy\":[\"AN\",\"EAN\",\"SNN\",\"SRN\"]},{\"Title\":\"Upgrade\\/Downgrade Electricity\",\"Code\":\"\",\"SearchBy\":[\"AN\",\"EAN\",\"SNN\",\"SRN\"]}]");
            //mSearchByList = JsonConvert.DeserializeObject<List<SearchByModel>>("[{\"Title\":\"Application Number\",\"Code\":\"AN\"},{\"Title\":\"Electricity Account Number\",\"Code\":\"EAN\"},{\"Title\":\"Service Notification Number\",\"Code\":\"SNN\"},{\"Title\":\"Service Request Number\",\"Code\":\"SRN\"}]");
        }

        private void TxtServiceRequestNum_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            try
            {
                //if(selectedType.SearchTypes.Where(x=>x.SearchTypeId == ) )
                if (isEdiging) return;
                isEdiging = true;

                string format = "RE-###-###-####-TS";
                bool isPreffixChange = false;
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
                if (preffix.Length > inputString.Length)
                {
                    inputString = preffix;
                    location = preffix.Length;
                }
                if (lastIndex < format.Length)
                {
                    suffix = format.Substring(lastIndex + 1, format.Length - (lastIndex + 1));
                    stringlength = ((format.Length - preffix.Length) - suffix.Length);
                    if (format.Length - 1 == location)
                    {
                        location = stringlength + preffix.Length;
                        if (suffix != string.Empty)
                        {
                            inputString = inputString != string.Empty ? inputString.Substring(0, preffix.Length + stringlength) : inputString;
                        }
                    }
                }
                location = (suffix != string.Empty && location > 0 && inputString.Length > 0 && inputString[location - 1] == '-') ? location - 1 : location;
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
                sb.Append(inputString.Replace("-", ""));
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
               
                if (txtServiceRequestNum.Text != string.Empty)
                {
                    EnableButton();
                }
                else
                {
                    DisableButton();
                }
                isEdiging = false;
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
                if (requestCode == Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        targetSearchBy = string.Empty;
                        Bundle extra = data.Extras;
                        List<TypeModel> resultTypeList = new List<TypeModel>();

                        if (extra.ContainsKey(Constants.APPLICATION_STATUS_TYPE_LIST_KEY))
                        {
                            txtSearchBy.Text = "";
                            resultTypeList = JsonConvert.DeserializeObject<List<TypeModel>>(extra.GetString(Constants.APPLICATION_STATUS_TYPE_LIST_KEY));
                            selectedType = resultTypeList.Find(x => x.isChecked);
                            //  TODO: ApplicationStatus dummp

                            targetApplicationType = selectedType.SearchApplicationTypeDesc;
                            targetApplicationTypeId = selectedType.SearchApplicationTypeId;
                            txtApplicationType.Text = targetApplicationType;

                            if (selectedType.SearchTypes.Count <= 1)
                            {
                                txtInputLayoutSearchBy.Visibility = ViewStates.Gone;
                                txtInputLayoutServiceRequestNum.Visibility = ViewStates.Gone;
                                txtInputLayoutServiceRequestNum.Visibility = ViewStates.Visible;
                                // txtServiceRequestNum.Text = " ";
                                txtInputLayoutServiceRequestNum.Hint = selectedType.SearchTypes[0].SearchTypeDesc;

                            }
                            else
                            {
                                txtInputLayoutServiceRequestNum.Visibility = ViewStates.Gone;
                                txtInputLayoutSearchBy.Visibility = ViewStates.Visible;
                            }




                            //if (extra.ContainsKey(Constants.APPLICATION_STATUS_SEARCH_BY_LIST_KEY))
                            //{
                            //    List<SearchByModel> resultSearchByList = new List<SearchByModel>();
                            //    resultSearchByList = JsonConvert.DeserializeObject<List<SearchByModel>>(extra.GetString(Constants.APPLICATION_STATUS_SEARCH_BY_LIST_KEY));
                            //    SearchByModel selectedType2 = resultSearchByList.Find(x => x.isChecked);
                            //    targetSearchBy = selectedType2.Code;
                            //    txtSearchBy.Text = selectedType2.Title;
                            //    txtInputLayoutServiceRequestNum.Visibility = ViewStates.Visible;
                            //    txtInputLayoutServiceRequestNum.Hint = selectedType.Title;
                            //}


                        }

                    }
                }
                else if (requestCode == Constants.APPLICATION_STATUS_FILTER_SEARCH_BY_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        Bundle extra = data.Extras;
                        List<SearchByModel> resultSearchByList = new List<SearchByModel>();

                        if (extra.ContainsKey(Constants.APPLICATION_STATUS_SEARCH_BY_LIST_KEY))
                        {
                            resultSearchByList = JsonConvert.DeserializeObject<List<SearchByModel>>(extra.GetString(Constants.APPLICATION_STATUS_SEARCH_BY_LIST_KEY));
                            SearchByModel selectedType = resultSearchByList.Find(x => x.isChecked);
                            targetSearchBy = selectedType.SearchTypeId;
                            txtSearchBy.Text = selectedType.SearchTypeDesc;
                            txtInputLayoutServiceRequestNum.Visibility = ViewStates.Gone;
                            txtInputLayoutServiceRequestNum.Visibility = ViewStates.Visible;
                            txtServiceRequestNum.Text = " ";
                            txtInputLayoutServiceRequestNum.Hint = selectedType.SearchTypeDesc;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
                            }
                            if (!string.IsNullOrEmpty(targetApplicationType))
                            {
                                for (int i = 0; i < listShowing.Count; i++)
                                {
                                    if (listShowing[i].SearchApplicationTypeDesc == targetApplicationType)
                                    {
                                        listShowing[i].isChecked = true;
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
            }
            return false;
        }
    }
}
