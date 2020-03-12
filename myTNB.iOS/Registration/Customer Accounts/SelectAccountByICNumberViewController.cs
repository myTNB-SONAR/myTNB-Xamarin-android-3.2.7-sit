using System;
using UIKit;
using CoreGraphics;
using myTNB.Registration;

namespace myTNB
{
    public partial class SelectAccountByICNumberViewController : CustomUIViewController
    {
        public SelectAccountByICNumberViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = AddAccountConstants.PageName;
            base.ViewDidLoad();
            AddBackButton();
            InitializedSubviews();
        }

        private void InitializedSubviews()
        {
            UIScrollView scrollView = new UIScrollView(new CGRect(0, 0, ViewWidth, ViewHeight)) { BackgroundColor = UIColor.Clear };

            UILabel lblTitle = new UILabel(new CGRect(BaseMargin, BaseMargin, BaseMarginedWidth, 100))
            {
                Text = GetI18NValue(AddAccountConstants.I18N_AddByIDMessage),
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.PowerBlue,
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };
            nfloat newTitleHeight = lblTitle.GetLabelHeight(1000);
            lblTitle.Frame = new CGRect(lblTitle.Frame.Location, new CGSize(lblTitle.Frame.Width, newTitleHeight));

            CustomUIView viewYes = new CustomUIView(new CGRect(BaseMargin, GetYLocationFromFrame(lblTitle.Frame, 16), BaseMarginedWidth, 100)) { BackgroundColor = UIColor.White };
            viewYes.Layer.CornerRadius = GetScaledWidth(4);

            UILabel lblYes = new UILabel(new CGRect(BaseMargin, BaseMargin, viewYes.Frame.Width - (BaseMargin * 2), GetScaledHeight(18)))
            {
                Text = string.Format("{0}.", GetCommonI18NValue(Constants.Common_Yes)),
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.TunaGrey(),
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };
            UILabel yesDescription = new UILabel(new CGRect(BaseMargin, GetYLocationFromFrame(lblYes.Frame, 5), viewYes.Frame.Width - (BaseMargin * 2), 100))
            {
                Text = GetI18NValue(AddAccountConstants.I18N_AddAsOwnerMessage),
                Font = TNBFont.MuseoSans_14_300,
                TextColor = MyTNBColor.TunaGrey(),
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };
            nfloat newYesDescriptionHeight = yesDescription.GetLabelHeight(1000);
            yesDescription.Frame = new CGRect(yesDescription.Frame.Location, new CGSize(yesDescription.Frame.Width, newYesDescriptionHeight));
            viewYes.AddSubviews(new UIView[] { lblYes, yesDescription });
            viewYes.Frame = new CGRect(viewYes.Frame.Location, new CGSize(viewYes.Frame.Width, yesDescription.Frame.GetMaxY() + GetScaledHeight(16)));

            CustomUIView viewNo = new CustomUIView(new CGRect(BaseMargin, GetYLocationFromFrame(viewYes.Frame, 8), BaseMarginedWidth, 100)) { BackgroundColor = UIColor.White };
            viewNo.Layer.CornerRadius = GetScaledWidth(4);

            UILabel lblNo = new UILabel(new CGRect(BaseMargin, BaseMargin, viewNo.Frame.Width - (BaseMargin * 2), GetScaledHeight(18)))
            {
                Text = string.Format("{0}.", GetCommonI18NValue(Constants.Common_No)),
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.TunaGrey(),
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };

            UILabel noDescription = new UILabel(new CGRect(BaseMargin, GetYLocationFromFrame(lblYes.Frame, 5), viewNo.Frame.Width - (BaseMargin * 2), 100))
            {
                Text = GetI18NValue(AddAccountConstants.I18N_AddAsTenantMessage),
                Font = TNBFont.MuseoSans_14_300,
                TextColor = MyTNBColor.TunaGrey(),
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };
            nfloat newNoDescriptionHeight = noDescription.GetLabelHeight(1000);
            noDescription.Frame = new CGRect(noDescription.Frame.Location, new CGSize(noDescription.Frame.Width, newNoDescriptionHeight));
            viewNo.AddSubviews(new UIView[] { lblNo, noDescription });
            viewNo.Frame = new CGRect(viewNo.Frame.Location, new CGSize(viewNo.Frame.Width, noDescription.Frame.GetMaxY() + GetScaledHeight(16)));

            scrollView.AddSubviews(new UIView[] { lblTitle, viewYes, viewNo });
            scrollView.ContentSize = new CGSize(scrollView.Frame.Width, viewNo.Frame.GetMaxY());
            View.AddSubview(scrollView);

            viewYes.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex = 0;
                UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                AddAccountViewController addAccountVC = storyBoard.InstantiateViewController("AddAccountViewController") as AddAccountViewController;
                addAccountVC.isOwner = true;
                NavigationController.PushViewController(addAccountVC, true);
            }));
            viewNo.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                NavigateToPage("AccountRecords", "SelectAccountByRightsViewController");
            }));
        }

        private void AddBackButton()
        {
            NavigationItem.Title = GetI18NValue(AddAccountConstants.I18N_NavTitle);
            View.BackgroundColor = MyTNBColor.LightGrayBG;
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                NavigationController.PopViewController(true);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        private void NavigateToPage(string storyboardName, string viewControllerName)
        {
            if (!string.IsNullOrEmpty(storyboardName) && !string.IsNullOrEmpty(viewControllerName))
            {
                UIStoryboard storyBoard = UIStoryboard.FromName(storyboardName, null);
                UIViewController viewController =
                    storyBoard.InstantiateViewController(viewControllerName) as UIViewController;
                if (viewController != null)
                {
                    NavigationController?.PushViewController(viewController, true);
                }
            }
        }
    }
}