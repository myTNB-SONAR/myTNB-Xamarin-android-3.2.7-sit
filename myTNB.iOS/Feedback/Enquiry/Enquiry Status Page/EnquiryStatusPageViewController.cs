using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CoreGraphics;
using myTNB.Customs.GenericStatusPage;
using myTNB.Feedback;
using myTNB.Feedback.Enquiry.EnquiryStatusPage;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public partial class EnquiryStatusPageViewController : CustomUIViewController
    {
        public EnquiryStatusPageViewController(IntPtr handle) : base(handle)
        {
        }

        private SubmittedFeedbackDetailsResponseModel _feedbackDetails = new SubmittedFeedbackDetailsResponseModel();

        private string _refNumber;
        private string _refDate;
        private string _refTitle;
        private string _refMessage;

        public string ReferenceNumber
        {
            set
            {
                _refNumber = string.IsNullOrEmpty(value) ? string.Empty : value;
            }
            get { return _refNumber; }
        }
        public string ReferenceDate
        {
            set
            {
                _refDate = string.IsNullOrEmpty(value) ? string.Empty : value;
            }
            get { return _refDate; }
        }

        public string StatusTitle
        {
            set
            {
                _refTitle = string.IsNullOrEmpty(value) ? string.Empty : value;
            }
            get { return _refTitle; }
        }
        public string StatusMessage
        {
            set
            {
                _refMessage = string.IsNullOrEmpty(value) ? string.Empty : value;
            }
            get { return _refMessage; }
        }

        public bool IsSuccess { set; get; }
        public StatusType StatusDisplayType
        {
            set; get;
        }

        private EnquiryStatusPageActions _actions;

        public enum StatusType
        {
            Feedback,
            SSMRApply,
            SSMRDiscontinue,
            SSMRReading,
            Enquiry
        }

        public UIViewController NextViewController
        {
            set; get;
        }

        public override void ViewDidLoad()
        {
            foreach (var view in this.View.Subviews)
            {
                view.RemoveFromSuperview();
            }
            IsGradientRequired = true;
            IsFullGradient = true;
            IsReversedGradient = true;

            //PageName = StatusPageConstants.PageName;
            PageName = EnquiryConstants.Pagename_Enquiry;

            NavigationController.NavigationBarHidden = true;
            base.ViewDidLoad();
            _actions = new EnquiryStatusPageActions(this, NextViewController);
            SetStatusCard();
            AddCTA();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private void SetStatusCard()
        {
            UIView viewCard = new UIView(new CGRect(BaseMargin, DeviceHelper.GetStatusBarHeight() + GetScaledHeight(16), BaseMarginedWidth, ViewHeight / 2))
            {
                BackgroundColor = UIColor.White,
                ClipsToBounds = true
            };
            viewCard.Layer.CornerRadius = 2.0F;
            nfloat imgWidth = GetScaledWidth(64);
            nfloat imgXLoc = (viewCard.Frame.Width - imgWidth) / 2;
            UIImageView imgStatus = new UIImageView(new CGRect(imgXLoc, GetScaledHeight(16), imgWidth, imgWidth))
            {
                Image = UIImage.FromBundle(IsSuccess ? "Circle-With-Check-Green" : "Red - Cross")
            };
            UILabel lblTitle = new UILabel(new CGRect(GetScaledWidth(16), imgStatus.Frame.GetMaxY()
                , viewCard.Frame.Width - GetScaledWidth(32), GetScaledHeight(24)))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.WaterBlue,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Font = TNBFont.MuseoSans_16_500,
                Text = GetI18NValue(EnquiryConstants.thankYouTitle) //string.IsNullOrEmpty(StatusTitle) ? GetText(IsSuccess
                                                                    //? StatusPageConstants.Success : StatusPageConstants.Fail) : StatusTitle
            };
            ResizeLabel(ref lblTitle, GetScaledHeight(24));
            UILabel lblMessage = new UILabel(new CGRect(GetScaledWidth(16), GetYLocationFromFrame(lblTitle.Frame, 4)
                , viewCard.Frame.Width - GetScaledWidth(32), GetScaledHeight(24)))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.CharcoalGrey,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Font = TNBFont.MuseoSans_12_300,
                Text = GetI18NValue(EnquiryConstants.thankYouDescription)
            };
            ResizeLabel(ref lblMessage, GetScaledHeight(16));

            viewCard.AddSubviews(new UIView[] { imgStatus, lblTitle, lblMessage });
            nfloat viewCardHeight = lblMessage.Frame.GetMaxY() + 16.0F;
            if (IsSuccess && StatusDisplayType == StatusType.Enquiry)
            {
                UIView viewLine = GenericLine.GetLine(new CGRect(GetScaledWidth(16), GetYLocationFromFrame(lblMessage.Frame, 16
                    ), viewCard.Frame.Width - GetScaledWidth(32), GetScaledHeight(1)));
                UILabel lblRef = new UILabel(new CGRect(GetScaledWidth(16), GetYLocationFromFrame(viewLine.Frame, 16) //Title SR No
                    , viewCard.Frame.Width - GetScaledWidth(32), GetScaledHeight(14))) 
                {
                    TextAlignment = UITextAlignment.Center,
                    TextColor = MyTNBColor.SilverChalice,
                    Font = TNBFont.MuseoSans_10_300,
                    Text = GetI18NValue(EnquiryConstants.serviceNoTitle)
                };

                UILabel lblRefVal = new UILabel(new CGRect(GetScaledWidth(16), GetYLocationFromFrame(lblRef.Frame, 1)//SR No
                    , viewCard.Frame.Width - GetScaledWidth(32), GetScaledHeight(18))) 
                {
                    TextAlignment = UITextAlignment.Center,
                    TextColor = MyTNBColor.CharcoalGrey,
                    Font = TNBFont.MuseoSans_14_300,
                    Text = ReferenceNumber.ToUpper() ?? TNBGlobal.EMPTY_DATE // SR No
                };

                viewCard.AddSubviews(new UIView[] { viewLine, lblRef, lblRefVal });
                viewCardHeight = lblRefVal.Frame.GetMaxY() + 16.0F;
            }
            viewCard.Frame = new CGRect(viewCard.Frame.X, viewCard.Frame.Y, viewCard.Frame.Width, viewCardHeight);
            View.AddSubview(viewCard);
        }

        private void ResizeLabel(ref UILabel label, nfloat minHeight)
        {
            CGSize newLblMsgSize = GetLabelSize(label, label.Frame.Width, ViewHeight / 2);
            CGRect newMsgFrame = label.Frame;
            newMsgFrame.Height = newLblMsgSize.Height < minHeight ? minHeight : newLblMsgSize.Height;
            label.Frame = newMsgFrame;
        }

        private void AddCTA()
        {
            CustomUIButtonV2 btnPrimary = new CustomUIButtonV2();
            CustomUIButtonV2 btnSecondary = new CustomUIButtonV2();

            if (StatusDisplayType == StatusType.Enquiry)
            {
                if (IsSuccess)
                {
                    GetCTA2(ref btnSecondary, GetI18NValue(EnquiryConstants.backHomeButton), false, _actions.BackToFeedback, false);
                    GetCTA2(ref btnPrimary, GetI18NValue(EnquiryConstants.viewSubmittedEnquiry), true, null, true); //ViewSubmittedEnquiry
                }
                else
                {
                    GetCTA(ref btnPrimary, GetCommonI18NValue(StatusPageConstants.I18N_TryAgain), true, _actions.BackToFeedback, true);

                }
            }
            View.AddSubviews(new UIView[] { btnPrimary, btnSecondary });
        }

        private void GetCTA(ref CustomUIButtonV2 btn, string title, bool isPrimary, Action ctaAction, bool isWhiteBG = false)
        {
            nfloat height = GetScaledHeight(48);
            CGSize size = new CGSize(BaseMarginedWidth, height);
            CGPoint point = new CGPoint(BaseMargin, isPrimary ? ViewHeight - height : ViewHeight - ((height * 2) + GetScaledHeight(6)));
            btn = new CustomUIButtonV2(isWhiteBG)
            {
                Frame = new CGRect(point, size)
            };
            btn.SetTitle(title, UIControlState.Normal);
            btn.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (ctaAction != null) { ctaAction.Invoke(); }
            }));
        }

        private void GetCTA2(ref CustomUIButtonV2 btn, string title, bool isPrimary, Action ctaAction, bool isWhiteBG = false)
        {
            nfloat height = GetScaledHeight(48);
            CGSize size = new CGSize(BaseMarginedWidth, height);
            CGPoint point = new CGPoint(BaseMargin, isPrimary ? ViewHeight - height : ViewHeight - ((height * 2) + GetScaledHeight(6)));
            btn = new CustomUIButtonV2(isWhiteBG)
            {
                Frame = new CGRect(point, size)
            };
            btn.SetTitle(title, UIControlState.Normal);
            btn.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if(ctaAction != null)
                { ctaAction.Invoke(); }
                else { ExecuteGetSubmittedFeedbackDetailsCall(); }
            }));
        }

        internal void ExecuteGetSubmittedFeedbackDetailsCall()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        GetSubmittedFeedbackDetails(ReferenceNumber).ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {

                                if (_feedbackDetails != null && _feedbackDetails.d != null
                                 && _feedbackDetails.d.data != null && _feedbackDetails.d.IsSuccess && _feedbackDetails.d.data.RelationshipWithCA != null)
                                {
                                    UIStoryboard storyBoard = UIStoryboard.FromName("Enquiry", null);
                                    EnquiryDetailsViewController enquiryDetailsViewController =
                                     storyBoard.InstantiateViewController("EnquiryDetailsViewController")
                                     as EnquiryDetailsViewController;

                                    enquiryDetailsViewController._feedbackDetails = _feedbackDetails.d.data;

                                    UINavigationController navController = new UINavigationController(enquiryDetailsViewController);
                                    navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                    PresentViewController(navController, true, null);
                                }
                                else
                                {
                                    DisplayServiceError(_feedbackDetails.d.DisplayMessage);
                                }

                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private Task GetSubmittedFeedbackDetails(string serviceReq)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    serviceReqNo = serviceReq
                };
                _feedbackDetails = serviceManager.OnExecuteAPIV6<SubmittedFeedbackDetailsResponseModel>("GetSubmittedFeedbackWithContactDetails", requestParameter);

            });
        }
    }
}