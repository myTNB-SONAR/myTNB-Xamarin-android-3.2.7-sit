using System;
using System.Diagnostics;
using CoreGraphics;
using myTNB.Customs.GenericStatusPage;
using myTNB.Feedback.Enquiry.EnquiryStatusPage;
using UIKit;

namespace myTNB
{
    public partial class EnquiryStatusPageViewController : CustomUIViewController
    {
        public EnquiryStatusPageViewController (IntPtr handle) : base (handle)
        {
        }

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
            SSMRReading
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
            PageName = StatusPageConstants.PageName;
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

        string GetDate()
        {
            if (string.IsNullOrEmpty(ReferenceDate) || string.IsNullOrWhiteSpace(ReferenceDate))
            {
                return TNBGlobal.EMPTY_DATE;
            }
            try
            {
                string date = DateHelper.GetFormattedDate(ReferenceDate.Split(' ')[0], "dd MMM yyyy");
                string time = ReferenceDate.Split(' ')[1];
                int hr = int.Parse(time.Split(':')[0]);
                int min = int.Parse(time.Split(':')[1]);
                int sec = int.Parse(time.Split(':')[2]);

                TimeSpan timespan = new TimeSpan(hr, min, sec);
                DateTime dt = DateTime.Today.Add(timespan);
                string displayTime = dt.ToString("hh:mm tt");
                string formattedDate = date + " " + displayTime;
                return formattedDate;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return TNBGlobal.EMPTY_DATE;
            }
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
                Image = UIImage.FromBundle(IsSuccess ? StatusPageConstants.IMG_Success : StatusPageConstants.IMG_Fail)
            };
            UILabel lblTitle = new UILabel(new CGRect(GetScaledWidth(16), imgStatus.Frame.GetMaxY()
                , viewCard.Frame.Width - GetScaledWidth(32), GetScaledHeight(24)))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.WaterBlue,
                Font = TNBFont.MuseoSans_16_500,
                Text = string.IsNullOrEmpty(StatusTitle) ? GetText(IsSuccess
                    ? StatusPageConstants.Success : StatusPageConstants.Fail) : StatusTitle
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
                Text = string.IsNullOrEmpty(StatusMessage) ? GetText(IsSuccess
                    ? StatusPageConstants.SuccessMessage : StatusPageConstants.FailMessage) : StatusMessage
            };
            ResizeLabel(ref lblMessage, GetScaledHeight(16));

            viewCard.AddSubviews(new UIView[] { imgStatus, lblTitle, lblMessage });
            nfloat viewCardHeight = lblMessage.Frame.GetMaxY() + 16.0F;
            if (IsSuccess && StatusDisplayType != StatusType.SSMRReading)
            {
                UIView viewLine = GenericLine.GetLine(new CGRect(GetScaledWidth(16), GetYLocationFromFrame(lblMessage.Frame, 16
                    ), viewCard.Frame.Width - GetScaledWidth(32), GetScaledHeight(1)));
                UILabel lblRef = new UILabel(new CGRect(GetScaledWidth(16), GetYLocationFromFrame(viewLine.Frame, 16)
                    , viewCard.Frame.Width * 0.60F, GetScaledHeight(14)))
                {
                    TextAlignment = UITextAlignment.Left,
                    TextColor = MyTNBColor.SilverChalice,
                    Font = TNBFont.MuseoSans_10_300,
                    Text = GetText(StatusPageConstants.ReferenceTitle).ToUpper()
                };
                UILabel lblDate = new UILabel(new CGRect(viewCard.Frame.Width - (viewCard.Frame.Width * 0.60F) - GetScaledWidth(16)
                    , GetYLocationFromFrame(viewLine.Frame, 16), viewCard.Frame.Width * 0.60F, GetScaledHeight(14)))
                {
                    TextAlignment = UITextAlignment.Right,
                    TextColor = MyTNBColor.SilverChalice,
                    Font = TNBFont.MuseoSans_10_300,
                    Text = GetText(StatusPageConstants.DateTitle).ToUpper()
                };
                UILabel lblRefVal = new UILabel(new CGRect(GetScaledWidth(16), GetYLocationFromFrame(lblRef.Frame, 1)
                    , viewCard.Frame.Width * 0.60F, GetScaledHeight(18)))
                {
                    TextAlignment = UITextAlignment.Left,
                    TextColor = MyTNBColor.CharcoalGrey,
                    Font = TNBFont.MuseoSans_14_300,
                    Text = ReferenceNumber.ToUpper() ?? TNBGlobal.EMPTY_DATE
                };
                string refDate = ReferenceDate ?? TNBGlobal.EMPTY_DATE;
                if (StatusDisplayType == StatusType.Feedback)
                {
                    refDate = GetDate();
                }
                UILabel lblDateVal = new UILabel(new CGRect(viewCard.Frame.Width - (viewCard.Frame.Width * 0.60F) - GetScaledWidth(16)
                    , GetYLocationFromFrame(lblDate.Frame, 1), viewCard.Frame.Width * 0.60F, GetScaledHeight(18)))
                {
                    TextAlignment = UITextAlignment.Right,
                    TextColor = MyTNBColor.CharcoalGrey,
                    Font = TNBFont.MuseoSans_14_300,
                    Text = refDate
                };
                viewCard.AddSubviews(new UIView[] { viewLine, lblRef, lblDate, lblRefVal, lblDateVal });
                viewCardHeight = lblDateVal.Frame.GetMaxY() + 16.0F;
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

        private string GetText(string key)
        {
            string value = string.Empty;
            if (StatusDisplayType == StatusType.Feedback)
            {
                bool isKeyExist = StatusPageConstants.FeedbackI18NDictionary.ContainsKey(key);
                if (isKeyExist)
                {
                    value = GetI18NValue(StatusPageConstants.FeedbackI18NDictionary[key]);
                }
            }
            else if (StatusDisplayType == StatusType.SSMRApply)
            {
                bool isKeyExist = StatusPageConstants.SSMRApplyI18NDictionary.ContainsKey(key);
                if (isKeyExist)
                {
                    value = GetI18NValue(StatusPageConstants.SSMRApplyI18NDictionary[key]);
                }
            }
            else if (StatusDisplayType == StatusType.SSMRDiscontinue)
            {
                bool isKeyExist = StatusPageConstants.SSMRDiscontinueI18NDictionary.ContainsKey(key);
                if (isKeyExist)
                {
                    value = GetI18NValue(StatusPageConstants.SSMRDiscontinueI18NDictionary[key]);
                }
            }
            else if (StatusDisplayType == StatusType.SSMRReading)
            {
                bool isKeyExist = StatusPageConstants.SSMRReadingI18NDictionary.ContainsKey(key);
                if (isKeyExist)
                {
                    value = GetI18NValue(StatusPageConstants.SSMRReadingI18NDictionary[key]);
                }
            }
            return value;
        }

        private void AddCTA()
        {
            CustomUIButtonV2 btnPrimary = new CustomUIButtonV2();
            CustomUIButtonV2 btnSecondary = new CustomUIButtonV2();

            if (StatusDisplayType == StatusType.Feedback)
            {
                GetCTA(ref btnPrimary, GetI18NValue(StatusPageConstants.I18N_BackToFeedback), true, _actions.BackToFeedback);
                btnPrimary.BackgroundColor = MyTNBColor.FreshGreen;
                btnPrimary.Layer.BorderWidth = 0;
            }
            else if (StatusDisplayType == StatusType.SSMRApply)
            {
                if (IsSuccess)
                {
                    GetCTA(ref btnPrimary, GetCommonI18NValue(StatusPageConstants.I18N_BacktoHome), true, _actions.BackToHome, true);
                }
                else
                {
                    GetCTA(ref btnSecondary, GetCommonI18NValue(StatusPageConstants.I18N_BacktoHome), false, _actions.BackToHome);
                    GetCTA(ref btnPrimary, GetCommonI18NValue(StatusPageConstants.I18N_TryAgain), true, _actions.SSMRTryAgain, true);
                }
            }
            else if (StatusDisplayType == StatusType.SSMRDiscontinue)
            {
                if (IsSuccess)
                {
                    GetCTA(ref btnPrimary, GetCommonI18NValue(StatusPageConstants.I18N_BacktoHome), true, _actions.BackToHome, true);
                }
                else
                {
                    GetCTA(ref btnSecondary, GetI18NValue(StatusPageConstants.I18N_SSMRBackToReadingHistory), false, _actions.ViewReadingHistory);
                    GetCTA(ref btnPrimary, GetCommonI18NValue(StatusPageConstants.I18N_TryAgain), true, _actions.SSMRTryAgain, true);
                }
            }
            else if (StatusDisplayType == StatusType.SSMRReading)
            {
                if (IsSuccess)
                {
                    GetCTA(ref btnPrimary, GetI18NValue(StatusPageConstants.I18N_SSMRViewReadHistory), true, _actions.ViewReadingHistory, true);
                }
                else
                {
                    GetCTA(ref btnPrimary, GetCommonI18NValue(StatusPageConstants.I18N_TryAgain), true, _actions.SSMRReadingTryAgain, true);

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
    }
}