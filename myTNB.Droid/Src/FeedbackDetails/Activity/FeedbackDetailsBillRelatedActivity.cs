using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Castle.Core.Internal;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FeedbackDetails.Adapter;
using myTNB_Android.Src.FeedbackDetails.MVP;
using myTNB_Android.Src.FeedbackFullScreenImage.Activity;
using myTNB_Android.Src.FeedbackGeneralEnquiryStepOne.Adapter;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime;
using static myTNB_Android.Src.Base.Models.SubmittedFeedbackDetails;

namespace myTNB_Android.Src.FeedbackDetails.Activity
{
    [Activity(Label = "@string/bill_related_activity_title"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.BillRelated")]
    public class FeedbackDetailsBillRelatedActivity : BaseToolbarAppCompatActivity, FeedbackDetailsContract.BillRelated.IView
    {

        [BindView(Resource.Id.txtInputLayoutFeedbackId)]
        TextInputLayout txtInputLayoutFeedbackId;

        [BindView(Resource.Id.txtInputLayoutDateTime)]
        TextInputLayout txtInputLayoutDateTime;

        [BindView(Resource.Id.txtInputLayoutAccountNo)]
        TextInputLayout txtInputLayoutAccountNo;

        [BindView(Resource.Id.txtInputLayoutFeedback)]
        TextInputLayout txtInputLayoutFeedback;

        [BindView(Resource.Id.txtInputLayoutStatus)]
        TextInputLayout txtInputLayoutStatus;

        [BindView(Resource.Id.txtFeedbackId)]
        EditText txtFeedbackId;

        [BindView(Resource.Id.txtFeedbackStatus)]
        EditText txtFeedbackStatus;

        [BindView(Resource.Id.txtFeedbackDateTime)]
        EditText txtFeedbackDateTime;

        [BindView(Resource.Id.txtAccountNo)]
        EditText txtAccountNo;

        [BindView(Resource.Id.txtFeedback)]
        EditText txtFeedback;

        [BindView(Resource.Id.txtRelatedScreenshotTitle)]
        TextView txtRelatedScreenshotTitle;

        [BindView(Resource.Id.recyclerView)]
        RecyclerView recyclerView;


        [BindView(Resource.Id.txtStatus)]
        TextView txtStatus;

        [BindView(Resource.Id.txtFeedback_status_new)]
        TextView txtFeedback_status_new;

        [BindView(Resource.Id.LinearLayout_topBanner)]
        LinearLayout LinearLayout_topBanner;

        [BindView(Resource.Id.FrameLayoutEnquiryDetails)]
        FrameLayout FrameLayoutEnquiryDetails;

        [BindView(Resource.Id.txtforMyhouse)]
        TextView txtforMyhouse;

        [BindView(Resource.Id.txtEnquiryDetails)]
        TextView txtEnquiryDetails;

        [BindView(Resource.Id.viewSpacer)]
        View viewSpacer;

        [BindView(Resource.Id.TextInputLayoutname)]
        TextInputLayout TextInputLayoutname;

        [BindView(Resource.Id.TextInputLayoutEmailAddress)]
        TextInputLayout TextInputLayoutEmailAddress;

        [BindView(Resource.Id.TextInputLayoutMobileNumber)]
        TextInputLayout TextInputLayoutMobileNumber;


        [BindView(Resource.Id.EditTextName)]
        EditText EditTextName;


        [BindView(Resource.Id.EditTextEmailAddress)]
        EditText EditTextEmailAddress;


        [BindView(Resource.Id.EditTextMobileNumber)]
        EditText EditTextMobileNumber;


        [BindView(Resource.Id.FrameLayoutContactDetails)]
        FrameLayout FrameLayoutContactDetails;



        [BindView(Resource.Id.TextView_contactDetails)]
        TextView TextView_contactDetails;

        [BindView(Resource.Id.TextInputLayoutRelationOwner)]
        TextInputLayout TextInputLayoutRelationOwner;

        [BindView(Resource.Id.EditTextRElationOwner)]
        EditText EditTextRElationOwner;


        [BindView(Resource.Id.TextInputLayoutNewIC)]
        TextInputLayout TextInputLayoutNewIC;

        [BindView(Resource.Id.EditTextNewIC)]
        EditText EditTextNewIC;


        [BindView(Resource.Id.TextInputLayoutNewAccName)]
        TextInputLayout TextInputLayoutNewAccName;

        [BindView(Resource.Id.EditTextNewAccName)]
        EditText EditTextNewAccName;



        [BindView(Resource.Id.TextInputLayoutNewMobileNumber)]
        TextInputLayout TextInputLayoutNewMobileNumber;

        [BindView(Resource.Id.EditTextNewMobileNumber)]
        EditText EditTextNewMobileNumber;




        [BindView(Resource.Id.TextInputLayoutNewEmailAddress)]
        TextInputLayout TextInputLayoutNewEmailAddress;

        [BindView(Resource.Id.EditTextNewEmailAddress)]
        EditText EditTextNewEmailAddress;


        [BindView(Resource.Id.TextInputLayoutNewMailingAddress)]
        TextInputLayout TextInputLayoutNewMailingAddress;

        [BindView(Resource.Id.EditTextNewMailingAddress)]
        EditText EditTextNewMailingAddress;


        [BindView(Resource.Id.TextInputLayoutNewPremiseAddress)]
        TextInputLayout TextInputLayoutNewPremiseAddress;

        [BindView(Resource.Id.EditTextNewPremiseAddress)]
        EditText EditTextNewPremiseAddress;


        [BindView(Resource.Id.barLine)]
        View barLine;


        [BindView(Resource.Id.TextInputLayoutServiceRequestNumber)]
        TextInputLayout TextInputLayoutServiceRequestNumber;


        [BindView(Resource.Id.EditTextServiceRequestNumber)]
        EditText EditTextServiceRequestNumber;


        private bool isNewScreen= false;


        FeedbackImageRecyclerAdapterNew adapter2;

        FeedbackImageRecyclerAdapter adapter;

        GridLayoutManager layoutManager;

        LinearLayoutManager layoutManager2;

        SubmittedFeedbackDetails submittedFeedback;


        FeedbackDetailsBillRelatedPresenter mPresenter;
        FeedbackDetailsContract.BillRelated.IUserActionsListener userActionsListener;

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.FeedbackDetailsBillRelatedView;
        }

        public void SetPresenter(FeedbackDetailsContract.BillRelated.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowImages(List<AttachedImage> list)
        {

            if (isNewScreen)
            {
                adapter2.AddAll(list);
                if (list.Count <= 0)
                {
                    txtRelatedScreenshotTitle.Visibility = ViewStates.Gone;
                }
                else
                {
                    txtRelatedScreenshotTitle.Visibility = ViewStates.Visible;
                }
            }
            else
            {
                adapter.AddAll(list);
                if (list.Count <= 0)
                {
                    txtRelatedScreenshotTitle.Visibility = ViewStates.Gone;
                }
                else
                {
                    txtRelatedScreenshotTitle.Visibility = ViewStates.Visible;
                }

            }

        
        }


        public void ShowInputData(string feedbackId, string feedbackStatus, string feedbackCode, string dateTime, string accountNoName, string feedback, List<FeedbackUpdate> feedbackUpdateList , string name, string email, string mobile , int? relationShip ,string relationshipDesc)
        {
            try
            {
                
                isNewScreen = true;

                if (isNewScreen)
                {

                    adapter2 = new FeedbackImageRecyclerAdapterNew(true);
                    layoutManager2 = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                    recyclerView.SetLayoutManager(layoutManager2);
                    recyclerView.SetAdapter(adapter2);
                    adapter2.SelectClickEvent += Adapter_SelectClickEvent;
                }
                else
                {

                    adapter = new FeedbackImageRecyclerAdapter(true);
                    layoutManager = new GridLayoutManager(this, Constants.GRID_IMAGE_COUNT);
                    recyclerView.SetLayoutManager(layoutManager);
                    recyclerView.SetAdapter(adapter);
                    adapter.SelectClickEvent += Adapter_SelectClickEvent;

                }

                if (isNewScreen)
                {
                    barLine.Visibility = ViewStates.Visible;
                    TextInputLayoutServiceRequestNumber.Visibility = ViewStates.Visible;
                    txtInputLayoutFeedbackId.Visibility = ViewStates.Gone;
                    txtInputLayoutStatus.Visibility = ViewStates.Gone;
                    txtInputLayoutDateTime.Visibility = ViewStates.Gone;
                    txtInputLayoutAccountNo.Visibility = ViewStates.Gone;
                    TextInputLayoutname.Visibility = ViewStates.Gone;

                }
                else
                {
                    FrameLayoutEnquiryDetails.Visibility = ViewStates.Gone;
                    LinearLayout_topBanner.Visibility = ViewStates.Gone;
                    viewSpacer.Visibility = ViewStates.Gone;
                    TextInputLayoutname.Visibility = ViewStates.Gone;
                    barLine.Visibility = ViewStates.Gone;
                    TextInputLayoutServiceRequestNumber.Visibility = ViewStates.Gone;

                }


                EditTextServiceRequestNumber.Text = feedbackId;
                txtFeedbackId.Text = feedbackId;  //not use
           

             

                txtFeedback_status_new.Text = feedbackStatus;  //set status new one



                if ((!name.IsNullOrEmpty() || !email.IsNullOrEmpty() || !mobile.IsNullOrEmpty())&& isNewScreen)
                {
                    FrameLayoutContactDetails.Visibility = ViewStates.Visible;
                }

                if (!name.IsNullOrEmpty() && isNewScreen)
                {
                    TextInputLayoutname.Visibility = ViewStates.Visible;
                    EditTextName.Text = name;
                    TextInputLayoutname.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "name").ToUpper();
                }

                if (!email.IsNullOrEmpty() && isNewScreen)
                {
                    TextInputLayoutEmailAddress.Visibility = ViewStates.Visible;
                    EditTextEmailAddress.Text = email;
                    TextInputLayoutEmailAddress.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "emailHint").ToUpper();
                }
                if (!mobile.IsNullOrEmpty() && isNewScreen)
                {
                    TextInputLayoutMobileNumber.Visibility = ViewStates.Visible;
                    EditTextMobileNumber.Text = mobile;
                    TextInputLayoutMobileNumber.Hint= Utility.GetLocalizedLabel("SubmitEnquiry", "mobileHint").ToUpper();
                }



                //if there was no feedback its mean it is an update , if not it is enquiry
                if (feedback.IsNullOrEmpty() && !feedbackUpdateList.IsNullOrEmpty())
                {
                    txtEnquiryDetails.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "reqUpdate");   // translate
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "updatePersonalDetTitle"));
                }
                else
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "submittedEnquiryTitle"));
                }

                // relationship hide or not

                if (relationShip != null && isNewScreen)
                {
                    //switch (relationShip)
                    //{
                    //    case 0:
                    //        {
                    //            TextInputLayoutRelationOwner.Visibility = ViewStates.Gone;

                    //        }
                    //        break;
                    //    case 1:
                    //        {
                    //            TextInputLayoutRelationOwner.Visibility = ViewStates.Visible;
                    //            EditTextRElationOwner.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "childTitle"); //translate
                    //            TextInputLayoutRelationOwner.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "relationshipTitle").ToUpper();
                    //        }
                    //        break;
                    //    case 2:
                    //        {
                    //            TextInputLayoutRelationOwner.Visibility = ViewStates.Visible;
                    //            EditTextRElationOwner.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "tenantTitle");
                    //            TextInputLayoutRelationOwner.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "relationshipTitle").ToUpper();
                    //        }
                    //        break;
                    //    case 3:
                    //        {
                    //            TextInputLayoutRelationOwner.Visibility = ViewStates.Visible;
                    //            EditTextRElationOwner.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "guardianTitle");
                    //            TextInputLayoutRelationOwner.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "relationshipTitle").ToUpper();
                    //        }
                    //        break;
                    //    case 4:
                    //        {
                    //            TextInputLayoutRelationOwner.Visibility = ViewStates.Visible;
                    //            EditTextRElationOwner.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "parentTitle");
                    //            TextInputLayoutRelationOwner.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "relationshipTitle").ToUpper();
                    //        }
                    //        break;
                    //    case 5:
                    //        {
                    //            TextInputLayoutRelationOwner.Visibility = ViewStates.Visible;
                    //            EditTextRElationOwner.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "spouseTitle");
                    //            TextInputLayoutRelationOwner.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "relationshipTitle").ToUpper();
                    //        }
                    //        break;
                    //    case 6:
                    //        {
                    //            TextInputLayoutRelationOwner.Visibility = ViewStates.Visible;
                    //            EditTextRElationOwner.Text = relationshipDesc;
                    //            TextInputLayoutRelationOwner.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "relationshipTitle").ToUpper();
                    //        }
                    //        break;

                    //}

                    if (relationShip == 0)
                    {
                        TextInputLayoutRelationOwner.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        TextInputLayoutRelationOwner.Visibility = ViewStates.Visible;
                        EditTextRElationOwner.Text = relationshipDesc;
                        TextInputLayoutRelationOwner.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "relationshipTitle").ToUpper();
                    }
            
                }
                else
                {
                    TextInputLayoutRelationOwner.Visibility = ViewStates.Gone;
                }


                


                foreach (FeedbackUpdate item in feedbackUpdateList)
                {

                    if (item.FeedbackUpdInfoType == 1 && isNewScreen)
                    {

                        string icNo = item.FeedbackUpdInfoValue;
                        if (!string.IsNullOrEmpty(icNo) && icNo.Length > 4)
                        {
                            string lastDigit = icNo.Substring(icNo.Length - 4);
                            int loopNumber = icNo.Length - 4;
                            string asterik = "";
                            for (int i = 0; i < loopNumber; i++)
                            {
                                asterik = asterik + "*";
                            }
                            icNo = asterik + lastDigit;
                        }
                        string maskedICNo = icNo;

                        TextInputLayoutNewIC.Visibility = ViewStates.Visible;
                        EditTextNewIC.Text = item.FeedbackUpdInfoValue.Length>4 ? maskedICNo : item.FeedbackUpdInfoValue;
                        TextInputLayoutNewIC.Hint = item.FeedbackUpdInfoTypeDesc.ToUpper();




                    }

                    if(item.FeedbackUpdInfoType == 2 && isNewScreen)
                    {
                        TextInputLayoutNewAccName.Visibility = ViewStates.Visible;
                        EditTextNewAccName.Text = item.FeedbackUpdInfoValue;
                        TextInputLayoutNewAccName.Hint = item.FeedbackUpdInfoTypeDesc.ToUpper();//Utility.GetLocalizedLabel("SubmitEnquiry", "newAccName").ToUpper();

                    }
                    if (item.FeedbackUpdInfoType == 3 && isNewScreen)
                    {
                        TextInputLayoutNewMobileNumber.Visibility = ViewStates.Visible;
                        EditTextNewMobileNumber.Text = item.FeedbackUpdInfoValue;
                        TextInputLayoutNewMobileNumber.Hint = item.FeedbackUpdInfoTypeDesc.ToUpper();//Utility.GetLocalizedLabel("SubmitEnquiry", "newMobileNumber").ToUpper();

                    }
                    if (item.FeedbackUpdInfoType == 4 && isNewScreen)
                    {
                        TextInputLayoutNewEmailAddress.Visibility = ViewStates.Visible;
                        EditTextNewEmailAddress.Text = item.FeedbackUpdInfoValue;
                        TextInputLayoutNewEmailAddress.Hint = item.FeedbackUpdInfoTypeDesc.ToUpper();//Utility.GetLocalizedLabel("SubmitEnquiry", "newEmailAdd").ToUpper();

                    }
                    if (item.FeedbackUpdInfoType == 5 && isNewScreen)
                    {
                        TextInputLayoutNewMailingAddress.Visibility = ViewStates.Visible;
                        EditTextNewMailingAddress.Text = item.FeedbackUpdInfoValue;
                        TextInputLayoutNewMailingAddress.Hint = item.FeedbackUpdInfoTypeDesc.ToUpper();//Utility.GetLocalizedLabel("SubmitEnquiry", "newMailing").ToUpper();

                    }
                    if (item.FeedbackUpdInfoType == 6 && isNewScreen)
                    {
                        TextInputLayoutNewPremiseAddress.Visibility = ViewStates.Visible;
                        EditTextNewPremiseAddress.Text = item.FeedbackUpdInfoValue;
                        TextInputLayoutNewPremiseAddress.Hint = item.FeedbackUpdInfoTypeDesc.ToUpper();//Utility.GetLocalizedLabel("SubmitEnquiry", "newPremisesAdd").ToUpper();

                    }


                }



                if (feedbackCode.Equals("CL01"))
                {
                    txtFeedbackStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.createdColor)));
                    txtFeedback_status_new.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.createdColorSubmit)));
                }
                else if (feedbackCode.Equals("CL02"))
                {
                    txtFeedbackStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.inProgressColor)));
                    txtFeedback_status_new.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.inProgressColor)));

                }
                else if (feedbackCode.Equals("CL03"))
                {
                    txtFeedbackStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.completedColor)));
                    txtFeedback_status_new.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.completedColor)));
                }
                else if (feedbackCode.Equals("CL04"))
                {
                    txtFeedbackStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.completedColor)));
                    txtFeedback_status_new.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.completedColor)));
                }
                else if (feedbackCode.Equals("CL06"))
                {
                    txtFeedbackStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.cancelledColor)));
                    txtFeedback_status_new.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.cancelledColor)));
                }

                txtFeedbackDateTime.Text = dateTime;
                txtAccountNo.Text = accountNoName;

                if (feedback.IsNullOrEmpty())
                {
                    txtInputLayoutFeedback.Visibility = ViewStates.Gone;
                }
                else
                {
                    txtFeedback.Text = feedback;
                }

           
                txtforMyhouse.Text = accountNoName;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if(Intent.HasExtra("TITLE") && !string.IsNullOrEmpty(Intent.GetStringExtra("TITLE")))
                {
                    SetToolBarTitle(Intent.GetStringExtra("TITLE"));
                }



                if (Intent.HasExtra("NEWSCREEN") && !string.IsNullOrEmpty(Intent.GetStringExtra("NEWSCREEN")))
                {

                     isNewScreen = bool.Parse(Intent.GetStringExtra("NEWSCREEN"));

                }

                if (UserEntity.IsCurrentlyActive())
                {
                    isNewScreen = true;
                }
                else
                {
                    isNewScreen = false;
                }


                
                // to delete
                if (isNewScreen)
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "submittedEnquiryTitle"));  // translate
                }

                // Create your application here
                string selectedFeedback = UserSessions.GetSelectedFeedback(PreferenceManager.GetDefaultSharedPreferences(this));
                submittedFeedback = JsonConvert.DeserializeObject<SubmittedFeedbackDetails>(selectedFeedback);

                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutAccountNo, txtInputLayoutFeedback, txtInputLayoutFeedbackId, txtInputLayoutDateTime, txtInputLayoutStatus);
                TextViewUtils.SetMuseoSans300Typeface(TextInputLayoutname, TextInputLayoutMobileNumber, TextInputLayoutEmailAddress, TextInputLayoutServiceRequestNumber);
                TextViewUtils.SetMuseoSans300Typeface(EditTextName, EditTextEmailAddress, EditTextMobileNumber);
                TextViewUtils.SetMuseoSans300Typeface(txtFeedbackId, txtFeedbackDateTime, txtAccountNo, txtFeedback, txtRelatedScreenshotTitle,  txtforMyhouse);
                TextViewUtils.SetMuseoSans500Typeface(txtStatus, txtFeedback_status_new, txtEnquiryDetails, TextView_contactDetails);
                TextViewUtils.SetMuseoSans300Typeface(EditTextServiceRequestNumber, EditTextRElationOwner, EditTextNewIC, EditTextNewAccName, EditTextNewMobileNumber, EditTextNewEmailAddress, EditTextNewMailingAddress, EditTextNewPremiseAddress);
                TextViewUtils.SetMuseoSans300Typeface(TextInputLayoutServiceRequestNumber,TextInputLayoutRelationOwner, TextInputLayoutNewIC, TextInputLayoutNewAccName, TextInputLayoutNewMobileNumber, TextInputLayoutNewEmailAddress, TextInputLayoutNewMailingAddress, TextInputLayoutNewPremiseAddress);
                
                //TRANSLATION
                txtEnquiryDetails.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "enquiryDetailsTitle");

                TextInputLayoutNewIC.Hint= Utility.GetLocalizedLabel("SubmitEnquiry", "newIC");

                TextInputLayoutNewAccName.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "newAccName");

                TextInputLayoutNewMobileNumber.Hint= Utility.GetLocalizedLabel("SubmitEnquiry", "newMobileNumber");

                TextInputLayoutNewEmailAddress.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "newEmailAdd");

                TextInputLayoutNewMailingAddress.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "newEmailAdd");

                TextView_contactDetails.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "contactDetailsTitle");

                TextInputLayoutname.Hint = Utility.GetLocalizedLabel("Common", "name");

                TextInputLayoutEmailAddress.Hint = Utility.GetLocalizedLabel("Common", "emailAddress");

                TextInputLayoutMobileNumber.Hint = Utility.GetLocalizedLabel("Common", "mobileNo");

                TextInputLayoutServiceRequestNumber.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "serviceNoTitle").ToUpper();



                string feedbackIdTitle = Utility.GetLocalizedLabel("FeedbackDetails", "feedbackID");
                txtInputLayoutFeedbackId.Hint = feedbackIdTitle;

                string feedbackStatusTitle = Utility.GetLocalizedLabel("FeedbackDetails", "feedbackStatus");
                txtInputLayoutStatus.Hint = feedbackStatusTitle;

                string feedbackDateTimeTitle = Utility.GetLocalizedLabel("FeedbackDetails", "dateTimeTitle");
                txtInputLayoutDateTime.Hint = feedbackDateTimeTitle;
                txtInputLayoutAccountNo.Hint = Utility.GetLocalizedLabel("Common", "accountNo");
                txtInputLayoutFeedback.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "messageHint");
                txtRelatedScreenshotTitle.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "supportingDocTitle");
          


                txtFeedbackId.AddTextChangedListener(new InputFilterFormField(txtFeedbackId, txtInputLayoutFeedbackId));
                txtFeedbackStatus.AddTextChangedListener(new InputFilterFormField(txtFeedbackStatus, txtInputLayoutStatus));
                txtFeedbackDateTime.AddTextChangedListener(new InputFilterFormField(txtFeedbackDateTime, txtInputLayoutDateTime));
                txtAccountNo.AddTextChangedListener(new InputFilterFormField(txtAccountNo, txtInputLayoutAccountNo));
                txtFeedback.AddTextChangedListener(new InputFilterFormField(txtFeedback, txtInputLayoutFeedback));


               
                //requested update lay
                TextInputLayoutRelationOwner.Visibility = ViewStates.Gone;
                TextInputLayoutNewIC.Visibility = ViewStates.Gone;
                TextInputLayoutNewAccName.Visibility = ViewStates.Gone;
                TextInputLayoutNewMobileNumber.Visibility = ViewStates.Gone;
                TextInputLayoutNewEmailAddress.Visibility = ViewStates.Gone;
                TextInputLayoutNewMailingAddress.Visibility = ViewStates.Gone;
                TextInputLayoutNewPremiseAddress.Visibility = ViewStates.Gone;

                ///contaact name lay
                TextInputLayoutMobileNumber.Visibility = ViewStates.Gone;
                TextInputLayoutEmailAddress.Visibility = ViewStates.Gone;
                FrameLayoutContactDetails.Visibility = ViewStates.Gone;



         

                mPresenter = new FeedbackDetailsBillRelatedPresenter(this, submittedFeedback,isNewScreen);
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Feedback Details");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void Adapter_SelectClickEvent(object sender, int e)
        {
            if (!this.GetIsClicked())
            {
                AttachedImage selectedImage;
                this.SetIsClicked(true);

                if (isNewScreen)
                {
                    selectedImage= adapter2.GetItemObject(e);
                }
                else
                {
                    selectedImage = adapter.GetItemObject(e);
                }

               

                var fullImageIntent = new Intent(this, typeof(FeedbackDetailsFullScreenImageActivity));
                fullImageIntent.PutExtra(Constants.SELECTED_FEEDBACK_DETAIL_IMAGE, JsonConvert.SerializeObject(selectedImage));
                StartActivity(fullImageIntent);
            }
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
    }
}