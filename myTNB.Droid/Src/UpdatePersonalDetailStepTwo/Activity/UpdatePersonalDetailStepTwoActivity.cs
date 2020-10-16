using System;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Preferences;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Castle.Core.Internal;
using CheeseBind;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FeedbackGeneralEnquiryStepOne.Adapter;
using myTNB_Android.Src.FeedbackGeneralEnquiryStepTwo.Activity;
using myTNB_Android.Src.UpdatePersonalDetailStepTwo.Adapter;
using myTNB_Android.Src.UpdatePersonalDetailStepTwo.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.UpdatePersonalDetailStepTwo.Activity
{
    [Activity( ScreenOrientation = ScreenOrientation.Portrait
      , WindowSoftInputMode = SoftInput.AdjustPan
, Theme = "@style/Theme.FaultyStreetLamps")]
    public class UpdatePersonalDetailStepTwoActivity : BaseToolbarAppCompatActivity, UpdatePersonalDetailStepTwoContract.IView
    {
        UpdatePersonalDetailStepTwoContract.IUserActionsListener userActionsListener;

        UpdatePersonalDetailStepTwoPresenter mPresenter;



        [BindView(Resource.Id.txtstep1of2)]
        TextView txtstep1of2;

        [BindView(Resource.Id.uploadSupportingDoc)]
        TextView uploadSupportingDoc;

        [BindView(Resource.Id.TextViewtitle_ownerIC)]
        TextView TextViewtitle_ownerIC;


        [BindView(Resource.Id.TextViewtitle_yourIC)]
        TextView TextViewtitle_yourIC;

        [BindView(Resource.Id.FrameLayout_copyOfIC)]
        FrameLayout FrameLayout_copyOfIC;


        [BindView(Resource.Id.TextView_exampleofIC)]
        TextView TextView_exampleofIC;

        [BindView(Resource.Id.FrameLayout_proofofconsent)]
        FrameLayout FrameLayout_proofofconsent;


        [BindView(Resource.Id.TextView_proofOfConsent)]
        TextView TextView_proofOfConsent;

        [BindView(Resource.Id.txtRelatedScreenshotTitle2)]
        TextView txtRelatedScreenshotTitle2;

        [BindView(Resource.Id.txtRelatedScreenshotTitle3)]
        TextView txtRelatedScreenshotTitle3;


        [BindView(Resource.Id.btnNext)]
        Button btnNext;

        public enum ADAPTER_TYPE
        {
            OWNER_IC,
            OWN_IC,
            SUPPORTING_DOC,
            PERMISES
        }




        [BindView(Resource.Id.recyclerView_ownerIC)]
        RecyclerView recyclerView_ownerIC;
        [BindView(Resource.Id.recyclerView_your_ic)]
        RecyclerView recyclerView_your_ic;
        [BindView(Resource.Id.recyclerView2)]
        RecyclerView recyclerView2;
        [BindView(Resource.Id.recyclerView3)]
        RecyclerView recyclerView3;

        [BindView(Resource.Id.TextView_ownerIC)]
        TextView TextView_ownerIC;

        [BindView(Resource.Id.TextView_yourIC_image)]
        TextView TextView_yourIC_image;

        [BindView(Resource.Id.TextView_proofOfConsent_image)]
        TextView TextView_proofOfConsent_image;


        [BindView(Resource.Id.TextView_proofOfConsent_image3)]
        TextView TextView_proofOfConsent_image3;


        [BindView(Resource.Id.TextView_agreement)]
        TextView TextView_agreement;

        [BindView(Resource.Id.FrameLayout_agreement)]
        FrameLayout FrameLayout_agreement;
        






        private bool isOwner =false ;
        private string ownerRelationship;
        private string icNumber;
        private string accOwnerName;
        private string mobileNumber;
        private string emailAddress;
        private string mailingAddress;
        private string premiseAddress;
        private string caNumber;
        private AlertDialog _ChooseDialog;

        FeedbackGeneralEnquiryStepOneImageRecyclerAdapter adapter;

        your_ic_adaptercs ic_adapter;

        supportingDocAdapter SupportingDocAdapter;

        permiseAdapter permiseAdapter;

        LinearLayoutManager layoutManager;

        LinearLayoutManager layoutManager2;

        LinearLayoutManager layoutManager3;

        LinearLayoutManager layoutManager4;

        private ISharedPreferences mSharedPref;

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                this.mSharedPref = PreferenceManager.GetDefaultSharedPreferences(this);  // init shared preference 
                //1 set presenter
                mPresenter = new UpdatePersonalDetailStepTwoPresenter(this);

                // get data from prev page
                Android.OS.Bundle extras = Intent.Extras;
                if (extras != null)
                {

                    if (extras.ContainsKey(Constants.ACCOUNT_NUMBER))
                    {
                        caNumber = (extras.GetString(Constants.ACCOUNT_NUMBER));

                    }



                    if (extras.ContainsKey(Constants.SELECT_REGISTERED_OWNER))
                    {
                        isOwner = bool.Parse(extras.GetString(Constants.SELECT_REGISTERED_OWNER));

                    }

                    if (!isOwner)
                    {
                        if (extras.ContainsKey(Constants.OWNER_RELATIONSHIP))
                        {
                            ownerRelationship = extras.GetString(Constants.OWNER_RELATIONSHIP);
                        }

                    }
                    if (extras.ContainsKey(Constants.ACCOUNT_IC_NUMBER))
                    {
                        icNumber = extras.GetString(Constants.ACCOUNT_IC_NUMBER);
                    }
                    if (extras.ContainsKey(Constants.ACCOUNT_OWNER_NAME))
                    {
                        accOwnerName = extras.GetString(Constants.ACCOUNT_OWNER_NAME);
                    }

                    if (extras.ContainsKey(Constants.ACCOUNT_MOBILE_NUMBER))
                    {
                        mobileNumber = extras.GetString(Constants.ACCOUNT_MOBILE_NUMBER);
                    }

                    if (extras.ContainsKey(Constants.ACCOUNT_EMAIL_ADDRESS))
                    {
                        emailAddress = extras.GetString(Constants.ACCOUNT_EMAIL_ADDRESS);
                    }

                    if (extras.ContainsKey(Constants.ACCOUNT_MAILING_ADDRESS))
                    {
                        mailingAddress = extras.GetString(Constants.ACCOUNT_MAILING_ADDRESS);
                    }
                    if (extras.ContainsKey(Constants.ACCOUNT_PREMISE_ADDRESS))
                    {
                        premiseAddress = extras.GetString(Constants.ACCOUNT_PREMISE_ADDRESS);
                    }

                }

               


                // Intent intent = Intent;
                SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "updatePersonalDetTitle"));
                
                txtstep1of2.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle2of3");

                //2 set font type , 300 normal 500 button
                TextViewUtils.SetMuseoSans300Typeface(TextViewtitle_ownerIC, TextViewtitle_yourIC, txtRelatedScreenshotTitle2, txtRelatedScreenshotTitle3);
                TextViewUtils.SetMuseoSans300Typeface(txtstep1of2, TextView_proofOfConsent_image3, TextView_yourIC_image, TextView_ownerIC, TextView_proofOfConsent_image);
                TextViewUtils.SetMuseoSans500Typeface(uploadSupportingDoc , TextView_exampleofIC, TextView_proofOfConsent, TextView_agreement);


               //owner adapter

                adapter = new FeedbackGeneralEnquiryStepOneImageRecyclerAdapter(true);
                adapter.Insert(new Base.Models.AttachedImage()
                {
                    ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                });
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                
                recyclerView_ownerIC.SetLayoutManager(layoutManager);
                recyclerView_ownerIC.SetAdapter(adapter);

                adapter.AddClickEvent += delegate { Adapter_AddClickEvent(ADAPTER_TYPE.OWNER_IC);};
                adapter.RemoveClickEvent += Adapter_RemoveClickEvent;

                //own ic adapter

                layoutManager2 = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);

                ic_adapter = new your_ic_adaptercs(true);
                ic_adapter.Insert(new Base.Models.AttachedImage()
                {
                    ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                });
                recyclerView_your_ic.SetLayoutManager(layoutManager2);
                recyclerView_your_ic.SetAdapter(ic_adapter);

                ic_adapter.AddClickEvent += delegate { Adapter_AddClickEvent(ADAPTER_TYPE.OWN_IC); };
                ic_adapter.RemoveClickEvent += IC_Adapter_RemoveClickEvent;

                //consent adapter

                layoutManager3 = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);

                SupportingDocAdapter = new supportingDocAdapter(true);
                SupportingDocAdapter.Insert(new Base.Models.AttachedImage()
                {
                    ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                });
                recyclerView2.SetLayoutManager(layoutManager3);
                recyclerView2.SetAdapter(SupportingDocAdapter);

                SupportingDocAdapter.AddClickEvent += delegate { Adapter_AddClickEvent(ADAPTER_TYPE.SUPPORTING_DOC); };
                SupportingDocAdapter.RemoveClickEvent += SUPPORTING_DOC_Adapter_RemoveClickEvent;

                //premise adapter

                layoutManager4 = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                permiseAdapter = new permiseAdapter(true);
                permiseAdapter.Insert(new Base.Models.AttachedImage()
                {
                    ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                });
                recyclerView3.SetLayoutManager(layoutManager4);
                recyclerView3.SetAdapter(permiseAdapter);

                permiseAdapter.AddClickEvent += delegate { Adapter_AddClickEvent(ADAPTER_TYPE.PERMISES); };
                permiseAdapter.RemoveClickEvent += PERMISES_Adapter_RemoveClickEvent;




                


             

                //set translation of string 
                //txtTermsConditionsGeneralEnquiry.TextFormatted = GetFormattedText(GetLabelByLanguage("tnc"));
                //StripUnderlinesFromLinks(txtTermsConditionsGeneralEnquiry);
                // txtInputLayoutAccountNo.Hint= GetLabelCommonByLanguage("email"); //sample of injecting hint using common lang

                //TRANSLATION

                TextView_ownerIC.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "attachDescription");
                TextView_yourIC_image.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "attachDescription");
                TextView_proofOfConsent_image.Text= Utility.GetLocalizedLabel("SubmitEnquiry", "attachDescription");
                TextView_proofOfConsent_image3.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "attachDescription");


                uploadSupportingDoc.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "uploadDocTitle");
                TextViewtitle_ownerIC.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "ownerIcOwner");
                TextViewtitle_yourIC.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "icTitleinfo");
                TextView_exampleofIC.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "icInfo");
                txtRelatedScreenshotTitle2.Text= Utility.GetLocalizedLabel("SubmitEnquiry", "consentTitle");
                txtRelatedScreenshotTitle3.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "copyOfPermiseProof");
                TextView_proofOfConsent.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "consentInfo");
                TextView_agreement.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "permisesTooltipTitle");
                btnNext.Text= Utility.GetLocalizedLabel("Common", "next");



                if (isOwner)
                {
                    
                    FrameLayout_proofofconsent.Visibility = ViewStates.Gone;
                    TextView_proofOfConsent_image.Visibility = ViewStates.Gone;
                    TextView_yourIC_image.Visibility = ViewStates.Gone;



                    recyclerView2.Visibility = ViewStates.Gone;
                    recyclerView_your_ic.Visibility = ViewStates.Gone;

                    TextViewtitle_yourIC.Visibility = ViewStates.Gone;
                    txtRelatedScreenshotTitle2.Visibility = ViewStates.Gone;

                }

                if (!premiseAddress.IsNullOrEmpty()) {

                    txtRelatedScreenshotTitle3.Visibility = ViewStates.Visible;
                    recyclerView3.Visibility = ViewStates.Visible;
                    txtRelatedScreenshotTitle3.Visibility = ViewStates.Visible;
                    TextView_proofOfConsent_image3.Visibility = ViewStates.Visible;
                    FrameLayout_agreement.Visibility = ViewStates.Visible;

                }
                else
                {
                    txtRelatedScreenshotTitle3.Visibility = ViewStates.Gone;
                    recyclerView3.Visibility = ViewStates.Gone;
                    txtRelatedScreenshotTitle3.Visibility = ViewStates.Gone;
                    TextView_proofOfConsent_image3.Visibility = ViewStates.Gone;
                    FrameLayout_agreement.Visibility = ViewStates.Gone;
                }

                OnCheckingAttachment();  // disable button if there was no attachment



            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public void OnCheckingAttachment()
        {
            if (isOwner)
            {  //if owner is choosed
                if (adapter?.GetAllImages().Count ==0)
                {
                    DisableSubmitButton();

                    if (!premiseAddress.IsNullOrEmpty() && permiseAdapter.GetAllImages().Count == 0)
                    {
                        DisableSubmitButton();
                    }

                }
                else
                {

                    if (!premiseAddress.IsNullOrEmpty() && permiseAdapter.GetAllImages().Count >1) {

                        EnableSubmitButton();
                    }
                    else if(!premiseAddress.IsNullOrEmpty() && permiseAdapter.GetAllImages().Count == 0 )
                    {
                        DisableSubmitButton();
                    }
                    else
                    {
                        EnableSubmitButton();
                    }

                        
                }
            }
            else
            {  
                //if tenant is choosed , mandatory 3 image
                if(adapter?.GetAllImages().Count==0 || ic_adapter?.GetAllImages().Count ==0 || SupportingDocAdapter?.GetAllImages().Count == 0)
                {
                    DisableSubmitButton();

                    if (!premiseAddress.IsNullOrEmpty() && permiseAdapter.GetAllImages().Count == 0)
                    {
                        DisableSubmitButton();
                    }
                }
                else
                {

                    if (!premiseAddress.IsNullOrEmpty() && permiseAdapter.GetAllImages().Count > 1)
                    {

                        EnableSubmitButton();
                    }
                    else if (!premiseAddress.IsNullOrEmpty() && permiseAdapter.GetAllImages().Count == 0)
                    {
                        DisableSubmitButton();
                    }
                    else
                    {
                        EnableSubmitButton();
                    }
                }
            }
        }


        public void UpdateAdapter(string pFilePath, string pFileName, string tFullname="")

        {
            ADAPTER_TYPE type;
            type = (ADAPTER_TYPE)Enum.Parse(typeof(ADAPTER_TYPE), UserSessions.GetAdapterType(this.mSharedPref));

            if (type.Equals(ADAPTER_TYPE.OWNER_IC))
            {
                adapter.Update(adapter.ItemCount - 1, new AttachedImage()
                {
                    ViewType = Constants.VIEW_TYPE_REAL_RECORD,
                    Name = pFileName,
                    Path = pFilePath,
                    FileName = tFullname

                });
                if (adapter.ItemCount < 1)
                {
                    adapter.Add(new AttachedImage()
                    {
                        ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                    });
                }
            }

            if (type.Equals(ADAPTER_TYPE.OWN_IC))
            {
                ic_adapter.Update(ic_adapter.ItemCount - 1, new AttachedImage()
                {
                    ViewType = Constants.VIEW_TYPE_REAL_RECORD,
                    Name = pFileName,
                    Path = pFilePath,
                    FileName = tFullname

                });
                if (ic_adapter.ItemCount < 1)
                {
                    ic_adapter.Add(new AttachedImage()
                    {
                        ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                    });
                }
            }

            if (type.Equals(ADAPTER_TYPE.SUPPORTING_DOC))
            {
                SupportingDocAdapter.Update(SupportingDocAdapter.ItemCount - 1, new AttachedImage()
                {
                    ViewType = Constants.VIEW_TYPE_REAL_RECORD,
                    Name = pFileName,
                    Path = pFilePath,
                    FileName = tFullname

                });
                if (SupportingDocAdapter.ItemCount < 1)
                {
                    SupportingDocAdapter.Add(new AttachedImage()
                    {
                        ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                    });
                }
            }
            if (type.Equals(ADAPTER_TYPE.PERMISES))
            {
                permiseAdapter.Update(permiseAdapter.ItemCount - 1, new AttachedImage()
                {
                    ViewType = Constants.VIEW_TYPE_REAL_RECORD,
                    Name = pFileName,
                    Path = pFilePath,
                    FileName = tFullname

                });
                if (permiseAdapter.ItemCount < 1)
                {
                    permiseAdapter.Add(new AttachedImage()
                    {
                        ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                    });
                }
            }

            OnCheckingAttachment();   // check attachment // this control the button 
        }


        public void EnableSubmitButton()
        {
            try
            {
           
                btnNext.Enabled = true;
                btnNext.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }



        [Preserve]
        private void Adapter_RemoveClickEvent(object sender, int e)
        {
            UserSessions.SaveAdapterType(mSharedPref, ADAPTER_TYPE.OWNER_IC.ToString());   // set shared pref adapter type
            try
            {
                adapter.Remove(e);
                if (adapter.GetAllImages().Count == 1 && adapter.ItemCount == 1)
                {
                    adapter.Add(new AttachedImage()
                    {
                        ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                    });
                }
                else
                {

                    if (adapter.ItemCount == 0)
                    {
                        adapter.Add(new AttachedImage()
                        {
                            ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                        });

                        TextView_ownerIC.Visibility = ViewStates.Visible;
                    }
                }

                OnCheckingAttachment();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [Preserve]
        private void IC_Adapter_RemoveClickEvent(object sender, int e)
        {
            UserSessions.SaveAdapterType(mSharedPref, ADAPTER_TYPE.OWN_IC.ToString());  // set shared pref adapter type 

            try
            {
                ic_adapter.Remove(e);
                if (ic_adapter.GetAllImages().Count == 1 && ic_adapter.ItemCount == 1)
                {
                    ic_adapter.Add(new AttachedImage()
                    {
                        ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                    });
                }
                else
                {

                    if (ic_adapter.ItemCount == 0)
                    {
                        ic_adapter.Add(new AttachedImage()
                        {
                            ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                        });
                        TextView_yourIC_image.Visibility = ViewStates.Visible;
                    }
                }
                OnCheckingAttachment();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [Preserve]
        private void SUPPORTING_DOC_Adapter_RemoveClickEvent(object sender, int e)
        {
            UserSessions.SaveAdapterType(mSharedPref, ADAPTER_TYPE.SUPPORTING_DOC.ToString());  // set shared pref adapter type 

            try
            {
                SupportingDocAdapter.Remove(e);
                if (SupportingDocAdapter.GetAllImages().Count == 1 && SupportingDocAdapter.ItemCount == 1)
                {
                    SupportingDocAdapter.Add(new AttachedImage()
                    {
                        ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                    });
                }
                else
                {

                    if (SupportingDocAdapter.ItemCount == 0)
                    {
                        SupportingDocAdapter.Add(new AttachedImage()
                        {
                            ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                        });

                        TextView_proofOfConsent_image.Visibility = ViewStates.Visible;
                    }
                }
                OnCheckingAttachment();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }


        
      [Preserve]
        private void PERMISES_Adapter_RemoveClickEvent(object sender, int e)
        {
            UserSessions.SaveAdapterType(mSharedPref, ADAPTER_TYPE.PERMISES.ToString());  // set shared pref adapter type 

            try
            {
                permiseAdapter.Remove(e);
                if (permiseAdapter.GetAllImages().Count == 1 && permiseAdapter.ItemCount == 1)
                {
                    permiseAdapter.Add(new AttachedImage()
                    {
                        ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                    });
                }
                else
                {

                    if (permiseAdapter.ItemCount == 0)
                    {
                        permiseAdapter.Add(new AttachedImage()
                        {
                            ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                        });

                        TextView_proofOfConsent_image3.Visibility = ViewStates.Visible;
                    }
                }
                OnCheckingAttachment();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }



        [Preserve]
        private void Adapter_AddClickEvent(ADAPTER_TYPE type)
        {
           
            try
            {

                if (type.Equals(ADAPTER_TYPE.OWN_IC)){
                    UserSessions.SaveAdapterType(mSharedPref, ADAPTER_TYPE.OWN_IC.ToString());
                   
                }

                if (type.Equals(ADAPTER_TYPE.OWNER_IC))
                {
                    UserSessions.SaveAdapterType(mSharedPref, ADAPTER_TYPE.OWNER_IC.ToString());
                    
                }

                if (type.Equals(ADAPTER_TYPE.SUPPORTING_DOC))
                {
                    UserSessions.SaveAdapterType(mSharedPref, ADAPTER_TYPE.SUPPORTING_DOC.ToString());
                    
                }
                if (type.Equals(ADAPTER_TYPE.PERMISES))
                {
                    UserSessions.SaveAdapterType(mSharedPref, ADAPTER_TYPE.PERMISES.ToString());

                }




                string[] items = { Utility.GetLocalizedLabel("FeedbackForm", "takePhoto")  ,
                               Utility.GetLocalizedLabel("FeedbackForm", "chooseFromLibrary") ,
                               Utility.GetLocalizedLabel("SubmitEnquiry", "choosePdf") ,
                               Utility.GetLocalizedCommonLabel("cancel")};

                AlertDialog.Builder builder = new AlertDialog.Builder(this)
                    .SetTitle(Utility.GetLocalizedLabel("FeedbackForm", "selectOptions"));
                builder.SetItems(items, (lsender, args) =>
                {



                    if (items[args.Which].Equals(Utility.GetLocalizedLabel("FeedbackForm", "takePhoto")))
                    {
                        this.userActionsListener.OnAttachPhotoCamera();
                    }
                    else if (items[args.Which].Equals(Utility.GetLocalizedLabel("FeedbackForm", "chooseFromLibrary")))
                    {
                        this.userActionsListener.OnAttachPhotoGallery();
                    }
                    else if (items[args.Which].Equals(Utility.GetLocalizedLabel("SubmitEnquiry", "choosePdf")))
                    {
                        this.userActionsListener.OnAttachPDF();
                    }
                    else if (items[args.Which].Equals(Utility.GetLocalizedCommonLabel("cancel")))
                    {
                        _ChooseDialog.Dismiss();
                    }
                }
                );
                _ChooseDialog = builder.Show();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }



        [OnClick(Resource.Id.btnNext)]
        void OnSubmit(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);

                    //todo if owner must have 1 attachment
                    //todo if non owner must have 3 attachment
                    var Intent = new Intent(this, typeof(FeedbackGeneralEnquiryStepTwoActivity));


                    Intent.PutExtra(Constants.ACCOUNT_NUMBER,caNumber.Trim());

                    if (isOwner)
                    { Intent.PutExtra(Constants.IMAGE_OWNER, JsonConvert.SerializeObject(adapter?.GetAllImages())); 
                    }else if (!isOwner)
                    {
                        Intent.PutExtra(Constants.IMAGE_OWNER, JsonConvert.SerializeObject(adapter?.GetAllImages()));
                        Intent.PutExtra(Constants.IMAGE_OWN, JsonConvert.SerializeObject(ic_adapter?.GetAllImages()));
                        Intent.PutExtra(Constants.IMAGE_SUPPORTING_DOC, JsonConvert.SerializeObject(SupportingDocAdapter?.GetAllImages()));
                    }

                    if (!premiseAddress.IsNullOrEmpty())
                    {

                        Intent.PutExtra(Constants.IMAGE_PERMISES, JsonConvert.SerializeObject(permiseAdapter?.GetAllImages()));
                        
                    }


                    if (isOwner ==true)
                    {
                        Intent.PutExtra(Constants.SELECT_REGISTERED_OWNER, true.ToString() );
                    }
                    else
                    {
                        Intent.PutExtra(Constants.SELECT_REGISTERED_OWNER, false.ToString());
                    }

                    if (ownerRelationship != null)
                    {
                        Intent.PutExtra(Constants.OWNER_RELATIONSHIP, ownerRelationship);
                    }

                    if (icNumber!=null)
                    {
                        Intent.PutExtra(Constants.ACCOUNT_IC_NUMBER, icNumber);
                    }

                    if (accOwnerName != null)
                    {
                        Intent.PutExtra(Constants.ACCOUNT_OWNER_NAME, accOwnerName);
                    }
                    if (mobileNumber != null)
                    {
                        Intent.PutExtra(Constants.ACCOUNT_MOBILE_NUMBER, mobileNumber);
                    }
                    if (emailAddress != null)
                    {
                        Intent.PutExtra(Constants.ACCOUNT_EMAIL_ADDRESS, emailAddress);
                    }
                    if (mailingAddress != null)
                    {
                        Intent.PutExtra(Constants.ACCOUNT_MAILING_ADDRESS, mailingAddress);
                    }
                    if (premiseAddress != null)
                    {
                        Intent.PutExtra(Constants.ACCOUNT_PREMISE_ADDRESS, premiseAddress);
                    }

                    Intent.PutExtra(Constants.PAGE_TITLE, Utility.GetLocalizedLabel("SubmitEnquiry", "updatePersonalDetTitle"));  // need translation    
                    Intent.PutExtra(Constants.PAGE_STEP_TITLE, Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle3of3"));// need translation        

                    StartActivity(Intent);




                }
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }


        public override void Ready()
        {
            FileUtils.CreateDirectory(this, FileUtils.TEMP_IMAGE_FOLDER);



        }


        public void ShowCamera()
        {
            if (!this.GetIsClicked())
            {
                Permission cameraPermission = ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera);
                if (cameraPermission == (int)Permission.Granted)
                {
                    this.SetIsClicked(true);
                    var intent = new Intent(MediaStore.ActionImageCapture);
                    Java.IO.File file = new Java.IO.File(FileUtils.GetTemporaryImageFilePath(this, FileUtils.TEMP_IMAGE_FOLDER, string.Format("{0}.jpeg", "temporaryImage")));
                    Android.Net.Uri fileUri = FileProvider.GetUriForFile(this,
                                                    ApplicationContext.PackageName + ".fileprovider", file);
                    intent.PutExtra(Android.Provider.MediaStore.ExtraOutput, fileUri);
                    StartActivityForResult(intent, Constants.REQUEST_ATTACHED_CAMERA_IMAGE);
                }
            }
        }

        public Task<AttachedImageRequest> SaveImage(AttachedImage attachedImage)
        {
            return Task.Run<AttachedImageRequest>(() =>
            {
                BitmapFactory.Options bmOptions = new BitmapFactory.Options();

                Bitmap bitmap = BitmapFactory.DecodeFile(attachedImage.Path, bmOptions);

                byte[] imageBytes = FileUtils.Get(this, bitmap);
                int size = imageBytes.Length;
                string hexString = HttpUtility.UrlEncode(FileUtils.ByteArrayToString(imageBytes), Encoding.UTF8);
                if (bitmap != null && !bitmap.IsRecycled)
                {
                    bitmap.Recycle();
                }
                Console.WriteLine(string.Format("Hex string {0}", hexString));
                return new AttachedImageRequest()
                {
                    ImageHex = hexString,
                    FileSize = size,
                    FileName = attachedImage.Name
                };
            });
        }

        public string GetTemporaryImageFilePath(string pFolder, string pFileName)
        {
            return FileUtils.GetTemporaryImageFilePath(this, pFolder, pFileName);
        }

        public Task<string> SaveCameraImage(string tempImagePath, string fileName)
        {
            return Task.Run<string>(() =>
            {
                return FileUtils.ProcessCameraImage(this, tempImagePath, fileName);
            });


        }

        public Task<string> SaveGalleryImage(Android.Net.Uri selectedImage, string pTempImagePath, string pFileName)
        {
            return Task.Run<string>(() =>
            {
                return FileUtils.ProcessGalleryImage(this, selectedImage, pTempImagePath, pFileName);
            });

        }


        public void ShowGallery()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent galleryIntent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
                galleryIntent.SetType("image/*");
                StartActivityForResult(Intent.CreateChooser(galleryIntent, GetString(Resource.String.bill_related_feedback_select_images)), Constants.RUNTIME_PERMISSION_GALLERY_REQUEST_CODE);
            }
        }

        public override bool StoragePermissionRequired()
        {
            return true;
        }


        public override bool CameraPermissionRequired()
        {
            return true;
        }



        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public void SetPresenter(UpdatePersonalDetailStepTwoContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }


        public override int ResourceId()
        {   //todo change
            return Resource.Layout.UpdatePersonalDetailStepTwoView;
        }

        
        public void HideIsOwner()
        {
            
        }


        [OnClick(Resource.Id.FrameLayout_proofofconsent)]
        public void OnFrameLayout_proofofconsent(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.OninfoLabelProofOfConsent();
            }
        }


        [OnClick(Resource.Id.FrameLayout_agreement)]
        public void OnFrameLayout_agreement(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.OninfoLabelPermise();
            }
        }



        [OnClick(Resource.Id.FrameLayout_copyOfIC)]
        public void OnFrameLayout_copyOfIC(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.OninfoLabelCopyOfIdentification();
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


        public void ShowinfoLabelProofOfConsent()
        {


            //List<HowDoesProofOfConsentResponseBitmapModel> modelList = MyTNBAppToolTipData.GetHowDoesProofOfConsentToolTipData();

            //if (modelList != null && modelList.Count > 0)
            //{
            //    MyTNBAppToolTipBuilder Tooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
            //        .SetHeaderImageBitmap(modelList[0].ImageBitmap)
            //        .SetTitle(modelList[0].PopUpTitle)
            //        .SetMessage(modelList[0].PopUpBody)
            //        .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
            //        .SetCTAaction(() => { this.SetIsClicked(false); })
            //        .Build();
            //        Tooltip.Show();
            //}
            //else
            //{
            //    //fallback if sitecore cannot select data
            //    //todo sitecore
            //    MyTNBAppToolTipBuilder infoLabelWhoIsRegistered = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
            // .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "consentTitle"))
            // .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "poc"))
            // .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
            // .SetCTAaction(() => { this.SetIsClicked(false); })
            // .Build();
            //    infoLabelWhoIsRegistered.Show();
            //}



                string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT);

                if (!base64Image.IsNullOrEmpty())
                {
                    var imageCache = Base64ToBitmap(base64Image);
                    MyTNBAppToolTipBuilder infoLabelWhoIsRegistered = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                    .SetHeaderImageBitmap(imageCache)
                    .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "consentTitle"))
                    .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "poc"))
                    .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                    .SetCTAaction(() => { this.SetIsClicked(false); })
                    .Build();
                    infoLabelWhoIsRegistered.Show();

                }
                else
                {   /// if sql lite is return null , pulling and deleting already done front
                //     var url = Utility.GetLocalizedLabel("SubmitEnquiry", "imageConsent");
                //     Bitmap imageCache = ImageUtils.GetImageBitmapFromUrl(SiteCoreConfig.SITECORE_URL + url);

                //     MyTNBAppToolTipBuilder infoLabelWhoIsRegistered = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                //    .SetHeaderImageBitmap(imageCache)
                //    .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "consentTitle"))
                //    .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "poc"))
                //    .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                //    .SetCTAaction(() => { this.SetIsClicked(false); })
                //    .Build();
                //     infoLabelWhoIsRegistered.Show();
                    this.SetIsClicked(false);

                }

             





        }

        public void ShowinfoLabelCopyOfIdentification()
        {

         

                string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE);

                if (!base64Image.IsNullOrEmpty())
                {
                    var imageCache = Base64ToBitmap(base64Image);
                    MyTNBAppToolTipBuilder infoLabelWhoIsRegistered = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                    .SetHeaderImageBitmap(imageCache)
                    .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "copyICTitle"))
                    .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "copyIcDet"))
                    .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                    .SetCTAaction(() => { this.SetIsClicked(false); })
                    .Build();
                    infoLabelWhoIsRegistered.Show();

                }
                else
                {
                //     var url = Utility.GetLocalizedLabel("SubmitEnquiry", "imageCopyIC");

                //     Bitmap imageCache = ImageUtils.GetImageBitmapFromUrl(SiteCoreConfig.SITECORE_URL + url);

                //     MyTNBAppToolTipBuilder infoLabelWhoIsRegistered = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                //    .SetHeaderImageBitmap(imageCache)
                //    .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "copyICTitle"))
                //    .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "copyIcDet"))
                //    .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                //    .SetCTAaction(() => { this.SetIsClicked(false); })
                //    .Build();
                //     infoLabelWhoIsRegistered.Show();
                    this.SetIsClicked(false);
                }
            

        }

        public void ShowinfoLabelPermise()
        {



            string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.PERMISE_IMAGE);

            if (!base64Image.IsNullOrEmpty())
            {
                var imageCache = Base64ToBitmap(base64Image);
                MyTNBAppToolTipBuilder infoLabelWhoIsRegistered = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                .SetHeaderImageBitmap(imageCache)
                .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "permisesTitle"))
                .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "permisesContent"))
                .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                .SetCTAaction(() => { this.SetIsClicked(false); })
                .Build();
                infoLabelWhoIsRegistered.Show();
               

            }
            else
            {  // incase base 64 is corrupt or null
               // var url = Utility.GetLocalizedLabel("SubmitEnquiry", "imagePermises");

               // Bitmap imageCache = ImageUtils.GetImageBitmapFromUrl(SiteCoreConfig.SITECORE_URL + url);

               // MyTNBAppToolTipBuilder infoLabelWhoIsRegistered = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
               //.SetHeaderImageBitmap(imageCache)
               //.SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "permisesTitle"))
               //.SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "permisesContent"))
               //.SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
               //.SetCTAaction(() => { this.SetIsClicked(false); })
               //.Build();
               // infoLabelWhoIsRegistered.Show();
                this.SetIsClicked(false);
            }


        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
      
                this.userActionsListener.OnActivityResult(requestCode, resultCode, data);
           
        }

        public void DisableSubmitButton()
        {
            btnNext.Enabled = false;
            btnNext.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        public string GetImageName(int itemCount)
        {
            SimpleDateFormat dateFormatter = new SimpleDateFormat("yyyyMMdd");
            Calendar calendar = Calendar.GetInstance(Locale.Default);
            return GetString(Resource.String.feedback_image_name_convention, dateFormatter.Format(calendar.TimeInMillis), UserSessions.GetCurrentImageCount(PreferenceManager.GetDefaultSharedPreferences(this)) + itemCount);
        }



        public void ShowLoadingImage()
        {
            try
            {
                ADAPTER_TYPE type;
                type = (ADAPTER_TYPE)Enum.Parse(typeof(ADAPTER_TYPE), UserSessions.GetAdapterType(this.mSharedPref));


                if (type.Equals(ADAPTER_TYPE.OWNER_IC)){

                    int position = adapter.ItemCount - 1;
                    AttachedImage attachImage = adapter.GetItemObject(position);
                    if (attachImage != null && attachImage.ViewType == Constants.VIEW_TYPE_DUMMY_RECORD)
                    {
                        attachImage.IsLoading = true;
                        adapter.Update(position, attachImage);
                    }

                }

                if (type.Equals(ADAPTER_TYPE.OWN_IC))
                {

                    int position = ic_adapter.ItemCount - 1;
                    AttachedImage attachImage = ic_adapter.GetItemObject(position);
                    if (attachImage != null && attachImage.ViewType == Constants.VIEW_TYPE_DUMMY_RECORD)
                    {
                        attachImage.IsLoading = true;
                        ic_adapter.Update(position, attachImage);
                    }

                }

                if (type.Equals(ADAPTER_TYPE.SUPPORTING_DOC))
                {

                    int position = SupportingDocAdapter.ItemCount - 1;
                    AttachedImage attachImage = SupportingDocAdapter.GetItemObject(position);
                    if (attachImage != null && attachImage.ViewType == Constants.VIEW_TYPE_DUMMY_RECORD)
                    {
                        attachImage.IsLoading = true;
                        SupportingDocAdapter.Update(position, attachImage);
                    }

                }

                if (type.Equals(ADAPTER_TYPE.PERMISES))
                {

                    int position = permiseAdapter.ItemCount - 1;
                    AttachedImage attachImage = permiseAdapter.GetItemObject(position);
                    if (attachImage != null && attachImage.ViewType == Constants.VIEW_TYPE_DUMMY_RECORD)
                    {
                        attachImage.IsLoading = true;
                        permiseAdapter.Update(position, attachImage);
                    }

                }




            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideLoadingImage()
        {
            try
            {

                ADAPTER_TYPE type;
                type = (ADAPTER_TYPE)Enum.Parse(typeof(ADAPTER_TYPE), UserSessions.GetAdapterType(this.mSharedPref));

                if (type.Equals(ADAPTER_TYPE.OWNER_IC))
                {

                    int position = adapter.ItemCount - 1;
                    AttachedImage attachImage = adapter.GetItemObject(position);
                    if (attachImage != null && attachImage.ViewType == Constants.VIEW_TYPE_DUMMY_RECORD)
                    {
                        attachImage.IsLoading = false;
                        adapter.Update(position, attachImage);
                    }

                    //hide mb file size 
                    TextView_ownerIC.Visibility = ViewStates.Gone;


                }
                if (type.Equals(ADAPTER_TYPE.OWN_IC))
                {

                    int position = ic_adapter.ItemCount - 1;
                    AttachedImage attachImage = ic_adapter.GetItemObject(position);
                    if (attachImage != null && attachImage.ViewType == Constants.VIEW_TYPE_DUMMY_RECORD)
                    {
                        attachImage.IsLoading = false;
                        ic_adapter.Update(position, attachImage);
                    }

                    //hide mb file size 
                    TextView_yourIC_image.Visibility = ViewStates.Gone;

                }
                if (type.Equals(ADAPTER_TYPE.SUPPORTING_DOC))
                {

                    int position = SupportingDocAdapter.ItemCount - 1;
                    AttachedImage attachImage = SupportingDocAdapter.GetItemObject(position);
                    if (attachImage != null && attachImage.ViewType == Constants.VIEW_TYPE_DUMMY_RECORD)
                    {
                        attachImage.IsLoading = false;
                        SupportingDocAdapter.Update(position, attachImage);
                    }

                    //hide mb file size 
                    TextView_proofOfConsent_image.Visibility = ViewStates.Gone;

                }
                if (type.Equals(ADAPTER_TYPE.PERMISES))
                {

                    int position = permiseAdapter.ItemCount - 1;
                    AttachedImage attachImage = permiseAdapter.GetItemObject(position);
                    if (attachImage != null && attachImage.ViewType == Constants.VIEW_TYPE_DUMMY_RECORD)
                    {
                        attachImage.IsLoading = false;
                        permiseAdapter.Update(position, attachImage);
                    }

                    //hide mb file size 
                    TextView_proofOfConsent_image3.Visibility = ViewStates.Gone;

                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

        public void ShowPDF()
        {

            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);

                Intent galleryIntent = new Intent(Intent.ActionGetContent);
                galleryIntent.SetType("application/pdf");
                StartActivityForResult(Intent.CreateChooser(galleryIntent, GetString(Resource.String.bill_related_feedback_select_images)), Constants.RUNTIME_PERMISSION_GALLERY_PDF_REQUEST_CODE);
            }

        }



    }
}