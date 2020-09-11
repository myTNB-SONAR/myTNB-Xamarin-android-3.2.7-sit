﻿using Android.App;
using Android.Content;
using Android.OS;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Base.Activity;
using Android.Views;
using Android.Util;
using System;
using CheeseBind;
using Android.Widget;


using System.Globalization;
using myTNB_Android.Src.Base.Fragments;
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
using System.Text.RegularExpressions;

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

        const string PAGE_ID = "ApplicationStatus";

        private string targetApplicationType = "";
        private string targetApplicationTypeId = "";
        private string targetSearchBy = "";
        private string targetNumber = "";

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

            txtServiceRequestNum.TextChanged += TxtServiceRequestNum_TextChanged;
           

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

        

        private void TxtServiceRequestNum_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                txtServiceRequestNum.Text = FormatApplicationNumber("ASR-###-###-####", e.Text.ToString());
                //if (c._selectedSearchType == "ApplicationNo")
                //{
                //    //CustomUITextField textField = sender as CustomUITextField;
                //    string input = e.Text.ToString();
                //    string format = c._selectedFormat;
                //    int firstIndex = format.IndexOf("#");
                //    int lastIndex = format.LastIndexOf("#");
                //    string preffix = string.Empty;
                //    string suffix = string.Empty;
                //    int location = e.Text. (int)range.Location;

                //    if (firstIndex > 0)
                //    {
                //        preffix = format.Substring(0, firstIndex);
                //        if (IsValid(preffix) && location < firstIndex)
                //        {
                //            //return false;
                //        }
                //    }

                //    if (lastIndex < format.Length)
                //    {
                //        suffix = format.Substring(lastIndex + 1, format.Length - (lastIndex + 1));
                //    }
                //    Log.Debug("preffix: " , preffix);
                //    Log.Debug("suffix: " , suffix);
                //    if (input.Length == format.Length && IsValid(suffix))
                //    {
                //        input = input.Replace(suffix, string.Empty);
                //        if (range.Location >= input.Length)
                //        {
                //            //return false;
                //        }
                //    }
                //    e.
                //    string replacedString = (input).Replace(range, replacementString).ToString();
                //    //if (replacementString == "")
                //    //{
                //    //    string currentChar = input[location].ToString();
                //    //    bool isLetter;
                //    //    if (currentChar == "-" && location > 0)
                //    //    {
                //    //        int preLocation = location - 1;
                //    //        isLetter = Regex.Matches(format[preLocation].ToString(), @"[a-zA-Z]").Count > 0;
                //    //        textField.Text = FormatApplicationNumber(format
                //    //            , isLetter ? input : replacedString);
                //    //        //return false;
                //    //    }
                //    //    else if (location < format.Length && Regex.Matches(format[location].ToString(), @"[a-zA-Z]").Count > 0)
                //    //    {
                //    //        //return false;
                //    //    }
                //    //}
                //    txtServiceRequestNum.Text = FormatApplicationNumber(format
                //        , replacedString);
                //    //DispatchQueue.MainQueue.DispatchAsync(() =>
                //    //{
                //        int cursorLocation = location;
                //        if (location < textField.Text.Length && textField.Text[location] == '-')
                //        {
                //            cursorLocation += 1;
                //        }

                //        UITextPosition newPosition = textField.GetPosition(textField.BeginningOfDocument
                //            , cursorLocation + replacementString.Length);
                //        if (newPosition != null)
                //        {
                //            textField.SelectedTextRange = textField.GetTextRange(newPosition, newPosition);
                //        }
                //});
                //return false;
                // }

                if (txtServiceRequestNum.Text != string.Empty)
                {
                    EnableButton();
                }
                else
                {
                    DisableButton();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }
        private string FormatApplicationNumber(string format
            , string replacedString)
        {

            Log.Debug("format: " , format);
            Log.Debug("replacedString: " , replacedString);

            string formattedInput = replacedString.Replace("-", "");
            int lastIndex = format.LastIndexOf("#");
            string suffix = string.Empty;
            if (lastIndex < format.Length)
            {
                suffix = format.Substring(lastIndex + 1, format.Length - (lastIndex + 1));
            }

            Log.Debug("suffix: " , suffix);
            List<int> dashIndices = new List<int>();
            for (int i = 0; i < format.Length; i++)
            {
                if (i < format.Length && format[i] == '-')
                {
                    dashIndices.Add(i);
                }
            }
            Log.Debug("removedDash: " , formattedInput);
            for (int j = 0; j < dashIndices.Count(); j++)
            {
                int index = dashIndices[j];
                if (index < formattedInput.Length)
                {
                    formattedInput = formattedInput.Insert(index, "-");
                    Log.Debug("test: " , formattedInput);
                }
                else if (index == formattedInput.Length && index == format.IndexOf("-"))
                {
                    formattedInput = formattedInput.Insert(index, "-");
                    Log.Debug("test: " , formattedInput);
                }
                else
                {
                    break;
                }
            }

            if (IsValid(suffix))
            {
                if (dashIndices != null && dashIndices.Count > 0)
                {
                    if (dashIndices[dashIndices.Count - 1] < formattedInput.Length)
                    {
                        formattedInput = formattedInput.Remove(dashIndices[dashIndices.Count - 1]) + suffix;
                    }
                    else if (dashIndices[dashIndices.Count - 1] == formattedInput.Length)
                    {
                        formattedInput += suffix;
                    }
                    else
                    {
                        formattedInput = formattedInput.Replace(suffix, string.Empty);
                    }
                }
                else
                {
                    formattedInput = formattedInput.Replace(suffix, string.Empty);
                }
            }
            else if (lastIndex + 1 < formattedInput.Length)
            {
                formattedInput = formattedInput.Remove(lastIndex + 1);
            }
            Log.Debug("Applied Dash: " ,formattedInput);
            return formattedInput;
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
                            TypeModel selectedType = resultTypeList.Find(x => x.isChecked);
                            //  TODO: ApplicationStatus dummp

                            targetApplicationType = selectedType.SearchApplicationTypeDesc;
                            targetApplicationTypeId = selectedType.SearchApplicationTypeId;
                            txtApplicationType.Text = targetApplicationType;

                            if (selectedType.SearchTypes.Count <= 1)
                            {
                                txtInputLayoutSearchBy.Visibility = ViewStates.Gone;
                                txtInputLayoutServiceRequestNum.Visibility = ViewStates.Gone;
                                txtInputLayoutServiceRequestNum.Visibility = ViewStates.Visible;
                                txtServiceRequestNum.Text = string.Empty;
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
                            txtServiceRequestNum.Text = string.Empty;
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

