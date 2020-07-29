using CoreGraphics;
using Foundation;
using myTNB.Feedback;
using myTNB.Home.Feedback;
using myTNB.Model;
using System;
using System.Drawing;
using UIKit;

namespace myTNB
{
    public partial class EnquiryDetailsViewController : CustomUIViewController
    {
        public EnquiryDetailsViewController(IntPtr handle) : base(handle) { }

        public SubmittedFeedbackDetailsDataModel _feedbackDetails = new SubmittedFeedbackDetailsDataModel();

        private UIView viewContainer;
        private UIScrollView _svContainer;
        private UIView _viewTitleSection;
        private UIView _container1;
        private UILabel lblTitleStatus;
        private UIView _container2;
        private UILabel lblTitleStatus2;
        private UILabel lblValueStatus2;
        private UILabel lblAccountNumber;
        private UIView _container3;
        private UILabel lblTitlePhoto;
        private UIView _containerImage;
        private UIView _containerFeedbackUpdateDetails;

        private UIView _containerRelation;
        private UILabel lblTitleRelationship;
        private UILabel lblRelationShipDesc;
        private UILabel lblTitleRelationshipOther;
        private UILabel lblRelationShipOther;
        private UIView _viewTitleSection2;
        private UIView _container4;
        private UILabel lblTitleContactName;
        private UILabel lblContactName;
        private UILabel lblTitleContactEmail;
        private UILabel lblContactEmail;
        private UILabel lblTitleContactMobile;
        private UILabel lblContactMobile;

        public override void ViewDidLoad()
        {
            PageName = EnquiryConstants.Pagename_Enquiry;

            base.ViewDidLoad();
            AddBackButton();
            AddScrollView();
            _containerOne();
            AddSectionTitle();
            _containerTwo();
            CreateFeedbackUpdateDetails();
            _containerPhoto();
            AddSectionTitle2();
            AddContainer4();
            UpdateContentSize();

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        private void AddBackButton()
        {
            NavigationController.NavigationItem.HidesBackButton = true;
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);

            });
            NavigationItem.LeftBarButtonItem = btnBack;

            Title = GetI18NValue(EnquiryConstants.submittedEnquiryTitle);
        }

        private void AddScrollView()
        {
            _svContainer = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            View.AddSubview(_svContainer);
        }

        public void _containerOne()
        {
            var firstAttributes = new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.SilverChalice,
                Font = TNBFont.MuseoSans_16_500
            };

            _container1 = new UIView(new CGRect(0, 0, View.Frame.Width, 80))
                {
                    BackgroundColor = UIColor.White
                };

                lblTitleStatus = new UILabel(new CGRect(18, 16, _container1.Frame.Width - 18, 24))
                {
                    Font = TNBFont.MuseoSans_16_500,
                    TextColor = MyTNBColor.CharcoalGrey,
                    LineBreakMode = UILineBreakMode.WordWrap,
                };

                string statusCode = _feedbackDetails.StatusCode;
                switch (statusCode)
                {
                    case "CL01":
                        {
                            //Created
                            firstAttributes = new UIStringAttributes
                        {
                            ForegroundColor = MyTNBColor.CharcoalGrey,
                            Font = TNBFont.MuseoSans_16_500
                        };
                        break;
                        }
                    case "CL02":
                        {
                            //In Progress
                            firstAttributes = new UIStringAttributes
                        {
                            ForegroundColor = MyTNBColor.SunGlow,
                            Font = TNBFont.MuseoSans_16_500
                        };
                        break;
                        }
                    case "CL03":
                    case "CL04":
                        {
                            //Completed
                            firstAttributes = new UIStringAttributes
                        {
                            ForegroundColor = MyTNBColor.FreshGreen,
                            Font = TNBFont.MuseoSans_16_500
                        };
                        break;
                        }
                    case "CL06":
                        {
                            //Cancelled
                            firstAttributes = new UIStringAttributes
                        {
                            ForegroundColor = MyTNBColor.Tomato,
                            Font = TNBFont.MuseoSans_16_500
                        };
                        break;
                        }
                }
            var prettyString = new NSMutableAttributedString("Status : " + _feedbackDetails.StatusDesc ?? string.Empty);
            prettyString.SetAttributes(firstAttributes.Dictionary, new NSRange(8, _feedbackDetails.StatusDesc.Length + 1 ));
            lblTitleStatus.AttributedText = prettyString;


            lblAccountNumber = new UILabel(new CGRect(18, lblTitleStatus.Frame.GetMaxY() + 4, _container1.Frame.Width, 24))
                {
                    Font = TNBFont.MuseoSans_14_300,
                    TextColor = MyTNBColor.TunaGrey(),
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Text = _feedbackDetails.AccountNum 
                };

                _container1.AddSubviews(new UIView[] { lblTitleStatus, lblAccountNumber });
                _svContainer.AddSubview(_container1);

            
        }
        private void AddSectionTitle()
        {
            _viewTitleSection = new UIView(new CGRect(0, _container1.Frame.GetMaxY(), View.Frame.Width, GetScaledHeight(48)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };

            UILabel lblSectionTitle = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(24)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = _feedbackDetails.RelationshipWithCA == 0 ? GetI18NValue(EnquiryConstants.enquiryDetailsTitle) : GetI18NValue(EnquiryConstants.reqUpdate)
            };

            _viewTitleSection.AddSubview(lblSectionTitle);
            _svContainer.AddSubview(_viewTitleSection);
        }

        private void _containerTwo()
        {
            _container2 = new UIView(new CGRect(0, _viewTitleSection.Frame.GetMaxY(), View.Frame.Width, GetScaledHeight(48)))
            {
                BackgroundColor = UIColor.White
            };

            lblTitleStatus2 = new UILabel(new CGRect(18, 16, _container2.Frame.Width - 18, 14))
            {
                Font = TNBFont.MuseoSans_10_500,
                TextColor = MyTNBColor.SilverChalice,
                Text = GetI18NValue(EnquiryConstants.messageHint).ToUpper()
            };

            lblValueStatus2 = new UILabel(new CGRect(18, lblTitleStatus2.Frame.GetMaxY() + 4, _container2.Frame.Width - 18, 18))
            {
                Font = TNBFont.MuseoSans_14_300,
                TextColor = MyTNBColor.TunaGrey(),
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Text = _feedbackDetails.FeedbackMessage ?? string.Empty
            };
            CGSize newSize = GetTitleLabelSize(_feedbackDetails.FeedbackMessage);
            lblValueStatus2.Frame = new CGRect(18, lblTitleStatus2.Frame.GetMaxY() + 4, _container2.Frame.Width - 18, GetScaledHeight(newSize.Height) + 16);
            _container2.Frame = new CGRect(0, _viewTitleSection.Frame.GetMaxY(), View.Frame.Width, lblValueStatus2.Frame.GetMaxY());


            _container2.AddSubviews(new UIView[] { lblTitleStatus2, lblValueStatus2 });
            _svContainer.AddSubview(_container2);

        }

        private void CreateFeedbackUpdateDetails()
        {
            if (_feedbackDetails.RelationshipWithCA != 0)
            {
                _containerRelation = new UIView(new CGRect(0, _viewTitleSection.Frame.GetMaxY(), View.Frame.Width, GetScaledHeight(48)))
                {
                    BackgroundColor = UIColor.White
                };

                lblTitleRelationship = new UILabel(new CGRect(18, 16, _containerRelation.Frame.Width - 18, 12))
                {
                    Font = TNBFont.MuseoSans_10_500,
                    TextColor = MyTNBColor.SilverChalice,
                    Text = GetI18NValue(EnquiryConstants.relationshipTitle).ToUpper()
                };

                lblRelationShipDesc = new UILabel(new CGRect(18, lblTitleRelationship.Frame.GetMaxY() + 4, _containerRelation.Frame.Width - 18, 24))
                {
                    Font = TNBFont.MuseoSans_16_300,
                    Text = _feedbackDetails.RelationshipWithCADesc
                };
                _containerRelation.AddSubviews(lblTitleRelationship, lblRelationShipDesc);
                _svContainer.AddSubviews(_containerRelation);
                _containerRelation.Frame = new CGRect(0, _viewTitleSection.Frame.GetMaxY(), View.Frame.Width, lblRelationShipDesc.Frame.GetMaxY() + 16);

                if (_feedbackDetails.RelationshipWithCA == 5) //add specify relatioship
                {
                    lblRelationShipDesc.Text = GetI18NValue(EnquiryConstants.othersTitle); //"Others";
                    lblTitleRelationshipOther = new UILabel(new CGRect(18, lblRelationShipDesc.Frame.GetMaxY() + 16, _containerRelation.Frame.Width - 18, 12))
                    {
                        Font = TNBFont.MuseoSans_10_500,
                        TextColor = MyTNBColor.SilverChalice,
                        Text = GetI18NValue(EnquiryConstants.otherRelationshipHint).ToUpper()
                    };

                    lblRelationShipOther = new UILabel(new CGRect(18, lblTitleRelationshipOther.Frame.GetMaxY() + 4, _containerRelation.Frame.Width - 18, 24))
                    {
                        Font = TNBFont.MuseoSans_16_300,
                        Text = _feedbackDetails.RelationshipWithCADesc
                    };
                    _containerRelation.AddSubviews(lblTitleRelationshipOther, lblRelationShipOther);
                    _containerRelation.Frame = new CGRect(0, _viewTitleSection.Frame.GetMaxY(), View.Frame.Width, lblRelationShipOther.Frame.GetMaxY() + 16);
                    _svContainer.AddSubviews(_containerRelation);
                }

            }

            if (_feedbackDetails.FeedbackUpdateDetails != null && _feedbackDetails.FeedbackUpdateDetails.Count > 0)
            {
                _containerFeedbackUpdateDetails = new UIView(new CGRect(0, _feedbackDetails.RelationshipWithCA != 0 ? _containerRelation.Frame.GetMaxY() : _viewTitleSection.Frame.GetMaxY(), View.Frame.Width, GetScaledHeight(48)))
                {
                    BackgroundColor = UIColor.White
                };

                nfloat x = _feedbackDetails.RelationshipWithCA != 0 ? 0 : 16;
                foreach (FeedbackUpdateDetailsModels item in _feedbackDetails.FeedbackUpdateDetails)
                {
                    UIView viewContainer = new UIView(new CGRect(18, x, _containerFeedbackUpdateDetails.Frame.Width - 18, 40));

                    UILabel lblView = new UILabel(new CGRect(0, 0, viewContainer.Frame.Width, 12))
                    {
                        Font = TNBFont.MuseoSans_10_500,
                        TextColor = MyTNBColor.SilverChalice,
                        Text = item.FeedbackUpdInfoTypeDesc.ToUpper()
                    };

                    UILabel lblViewInfo = new UILabel(new CGRect(0, lblView.Frame.GetMaxY() + 4, viewContainer.Frame.Width -18, 24))
                    {
                        Font = TNBFont.MuseoSans_16_300,
                        Text = item.FeedbackUpdInfoValue,
                        LineBreakMode = UILineBreakMode.WordWrap,
                        Lines = 0,
                    };
                    CGSize newSize = lblViewInfo.SizeThatFits(new CGSize(View.Frame.Width - 23, GetScaledHeight(160)));
                    lblViewInfo.Frame = new CGRect(0, lblView.Frame.GetMaxY() + 4, viewContainer.Frame.Width -18, newSize.Height);

                    if (item.FeedbackUpdInfoType == 1)
                    {
                        string icNo = item?.FeedbackUpdInfoValue;
                        if (!string.IsNullOrEmpty(icNo) && icNo.Length > 4)
                        {
                            string lastDigit = icNo.Substring(icNo.Length - 4);
                            icNo = "•••••• •• " + lastDigit;
                            string maskedICNo = icNo;
                            lblViewInfo.Text = maskedICNo;
                        }

                    }

                    viewContainer.AddSubviews(lblView, lblViewInfo);
                    viewContainer.Frame = new CGRect(18, x, _containerFeedbackUpdateDetails.Frame.Width - 18, lblViewInfo.Frame.GetMaxY() + 16);
                    _containerFeedbackUpdateDetails.AddSubview(viewContainer);
                    x += lblViewInfo.Frame.GetMaxY() + 16;
                }
                _containerFeedbackUpdateDetails.Frame = new CGRect(0, _feedbackDetails.RelationshipWithCA != 0 ? _containerRelation.Frame.GetMaxY() : _viewTitleSection.Frame.GetMaxY(), View.Frame.Width, x);

                _svContainer.AddSubview(_containerFeedbackUpdateDetails);
            }
        }

        private void _containerPhoto()
        {
            if (_feedbackDetails.FeedbackImage != null && _feedbackDetails.FeedbackImage.Count > 0)
            {
                _container3 = new UIView(new CGRect(0, _containerFeedbackUpdateDetails != null ? _containerFeedbackUpdateDetails.Frame.GetMaxY() : _container2.Frame.GetMaxY(), View.Frame.Width, GetScaledHeight(48)))
                {
                    BackgroundColor = UIColor.White
                };

                lblTitlePhoto = new UILabel(new CGRect(18, 16, _container3.Frame.Width - 18, 14))
                {
                    Font = TNBFont.MuseoSans_10_500,
                    TextColor = MyTNBColor.SilverChalice,
                    Text = GetI18NValue(EnquiryConstants.supportingDocTitle).ToUpper()
                };

                _containerImage = new UIView(new CGRect(0, lblTitlePhoto.Frame.GetMaxY() + 4, View.Frame.Width, GetScaledHeight(48)))
                {
                    BackgroundColor = UIColor.White
                };

                int x = 0;
                UIImageHelper imgHelper = new UIImageHelper();
                foreach (FeedbackImageModel item in _feedbackDetails.FeedbackImage)
                {
                    UIView viewContainer = new UIView(new CGRect(18, x, _containerImage.Frame.Width - 36, 48));
                    viewContainer.Layer.BorderWidth = 1;
                    viewContainer.Layer.CornerRadius = 2.0f;
                    viewContainer.Layer.BorderColor = MyTNBColor.VeryLightPink.CGColor;

                    UIImageView imgView = new UIImageView(new CGRect(8, 8, 32, 32))
                    {
                        Image = imgHelper.ConvertHexToUIImage(item.imageHex)
                    };
                    imgView.Layer.CornerRadius = 2.0f;
                    imgView.Layer.MasksToBounds = true;

                    UILabel imgViewName = new UILabel(new CGRect(52, 8, viewContainer.Frame.Width, 32))
                    {
                        Font = TNBFont.MuseoSans_12_300,
                        TextColor = MyTNBColor.WaterBlue,
                        Text = FeedbackFileNameHelper.GenerateFileName(),
                    };
                    viewContainer.AddSubview(imgViewName);

                    viewContainer.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                    {
                        OnImageClick(imgView.Image, item.fileName);
                    }));
                    viewContainer.AddSubview(imgView);
                    _containerImage.AddSubview(viewContainer);
                    x += 48 + 7;
                }
                _containerImage.Frame = new CGRect(0, lblTitlePhoto.Frame.GetMaxY() + 4, View.Frame.Width, x + 9);

                _container3.AddSubviews(new UIView[] { lblTitlePhoto, _containerImage });
                _container3.Frame = new CGRect(0, _containerFeedbackUpdateDetails != null ? _containerFeedbackUpdateDetails.Frame.GetMaxY() : _container2.Frame.GetMaxY(), View.Frame.Width, _containerImage.Frame.GetMaxY());
                _svContainer.AddSubview(_container3);
            }

        }

        private void AddSectionTitle2()
        {
            try
            {
                _viewTitleSection2 = new UIView(new CGRect(0, _container3 != null ? _container3.Frame.GetMaxY() : _containerFeedbackUpdateDetails.Frame.GetMaxY(), View.Frame.Width, GetScaledHeight(48)))
                {
                    BackgroundColor = MyTNBColor.LightGrayBG
                };
            }
            catch (Exception ex)
            {
                _viewTitleSection2 = new UIView(new CGRect(0, _container2.Frame.GetMaxY(), View.Frame.Width, GetScaledHeight(48)))
                {
                    BackgroundColor = MyTNBColor.LightGrayBG
                };
            }
            UILabel lblSectionTitle = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(24)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(EnquiryConstants.contactDetailsTitle)
            };

            _viewTitleSection2.AddSubview(lblSectionTitle);
            _svContainer.AddSubview(_viewTitleSection2);
        }

        private void AddContainer4()
        {
            _container4 = new UIView(new CGRect(0, _viewTitleSection2.Frame.GetMaxY(), View.Frame.Width, GetScaledHeight(172)))
            {
                BackgroundColor = UIColor.White
            };

            lblTitleContactName = new UILabel(new CGRect(18, 16, _container4.Frame.Width - 18, 12))
            {
                Font = TNBFont.MuseoSans_10_500,
                TextColor = MyTNBColor.SilverChalice,
                Text = GetCommonI18NValue("fullname").ToUpper()
            };

            lblContactName = new UILabel(new CGRect(18, lblTitleContactName.Frame.GetMaxY() + 4, _container4.Frame.Width - 18, 24))
            {
                Font = TNBFont.MuseoSans_16_300,
                Text = _feedbackDetails.ContactName
            };
            _container4.AddSubviews(lblTitleContactName, lblContactName);


            lblTitleContactEmail = new UILabel(new CGRect(18, lblContactName.Frame.GetMaxY() + 16, _container4.Frame.Width - 18, 12))
            {
                Font = TNBFont.MuseoSans_10_500,
                TextColor = MyTNBColor.SilverChalice,
                Text = GetCommonI18NValue("emailAddress").ToUpper()
            };

            lblContactEmail = new UILabel(new CGRect(18, lblTitleContactEmail.Frame.GetMaxY() + 4, _container4.Frame.Width - 18, 24))
            {
                Font = TNBFont.MuseoSans_16_300,
                Text = _feedbackDetails.ContactEmailAddress
            };
            _container4.AddSubviews(lblTitleContactEmail, lblContactEmail);


            lblTitleContactMobile = new UILabel(new CGRect(18, lblContactEmail.Frame.GetMaxY() + 16, _container4.Frame.Width - 18, 12))
            {
                Font = TNBFont.MuseoSans_10_500,
                TextColor = MyTNBColor.SilverChalice,
                Text = GetCommonI18NValue("mobileNumber").ToUpper()
            };

            lblContactMobile = new UILabel(new CGRect(18, lblTitleContactMobile.Frame.GetMaxY() + 4, _container4.Frame.Width - 18, 24))
            {
                Font = TNBFont.MuseoSans_16_300,
                Text = _feedbackDetails.ContactMobileNo
            };
            _container4.AddSubviews(lblTitleContactMobile, lblContactMobile);
            _svContainer.AddSubviews(_container4);

        }

        private nfloat GetScrollHeight()
        {
            return (nfloat)((_container4.Frame.GetMaxY()));
        }

        private void UpdateContentSize()
        {
            _svContainer.ContentSize = new CGRect(0f, 0f, View.Frame.Width, GetScrollHeight() + 64).Size;
        }

        private CGSize GetTitleLabelSize(string text)
        {
            UILabel label = new UILabel(new CGRect(18, 30, UIApplication.SharedApplication.KeyWindow.Frame.Width - 36, 1000))
            {
                Font = MyTNBFont.MuseoSans14,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = text
            };
            return label.Text.StringSize(label.Font, new SizeF((float)label.Frame.Width, 1000F));
        }

        internal void OnImageClick(UIImage image, string fileName)
        {
            viewContainer = new UIView(UIScreen.MainScreen.Bounds)
            {
                BackgroundColor = UIColor.Black
            };

            UILabel lblFileName = new UILabel(new CGRect(0, DeviceHelper.IsIphoneXUpResolution() ? 44 : 0, viewContainer.Frame.Width, 24))
            {
                BackgroundColor = UIColor.Black,
                Font = MyTNBFont.MuseoSans16,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Text = fileName
            };

            UIView viewClose = new UIView(new CGRect(viewContainer.Frame.Width - 70, DeviceHelper.IsIphoneXUpResolution() ? 44 : 0, 60, 24));
            UILabel lblClose = new UILabel(new CGRect(0, 0, 60, 24))
            {
                BackgroundColor = UIColor.Black,
                Font = MyTNBFont.MuseoSans16,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Right,
                Text = GetCommonI18NValue(Constants.Common_Close)
            };
            viewClose.AddSubview(lblClose);
            viewClose.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                viewContainer.Hidden = true;
                viewContainer.RemoveFromSuperview();
                UIApplication.SharedApplication.StatusBarHidden = false;
            }));

            float imgWidth = 0;
            float imgHeight = 0;
            if (image != null)
            {
                if (image.Size.Width < image.Size.Height)
                {
                    if (image.Size.Width < View.Frame.Width)
                    {
                        imgWidth = (float)image.Size.Width;
                        if (image.Size.Height < View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 44 : 24))
                        {
                            imgHeight = (float)image.Size.Height;
                        }
                        else
                        {
                            imgHeight = (float)View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 44 : 24);
                        }
                    }
                    else
                    {
                        imgWidth = (float)View.Frame.Width;
                        float ratio = (float)(image.Size.Width / image.Size.Height);
                        imgHeight = imgWidth / ratio;
                        if (imgHeight > View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 44 : 24))
                        {
                            imgHeight = (float)View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 44 : 24);
                        }
                    }
                }
                else
                {
                    if (image.Size.Width < View.Frame.Width)
                    {
                        imgWidth = (float)image.Size.Width;
                        imgHeight = (float)image.Size.Height;
                    }
                    else
                    {
                        imgWidth = (float)View.Frame.Width;
                        float ratio = (float)(image.Size.Width / image.Size.Height);
                        imgHeight = imgWidth / ratio;
                        if (imgHeight > View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 44 : 24))
                        {
                            imgHeight = (float)View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 44 : 24);
                        }
                    }
                }

                UIImageView imgView = new UIImageView(new CGRect((viewContainer.Frame.Width / 2) - (imgWidth / 2)
                    , lblFileName.Frame.GetMaxY() + 10f, imgWidth, imgHeight))
                {
                    Image = image
                };

                viewContainer.AddSubviews(new UIView[] { lblFileName, viewClose, imgView });

                UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
                currentWindow.AddSubview(viewContainer);
                viewContainer.Hidden = false;
                UIApplication.SharedApplication.StatusBarHidden = true;
            }

        }

        private void ResizeLabel(ref UILabel label, nfloat minHeight)
        {
            CGSize newLblMsgSize = GetLabelSize(label, label.Frame.Width, ViewHeight / 2);
            CGRect newMsgFrame = label.Frame;
            newMsgFrame.Height = newLblMsgSize.Height < minHeight ? minHeight : newLblMsgSize.Height;
            label.Frame = newMsgFrame;
        }
    }
}