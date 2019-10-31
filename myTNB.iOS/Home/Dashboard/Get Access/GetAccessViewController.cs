using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public partial class GetAccessViewController : UIViewController
    {
        public GetAccessViewController(IntPtr handle) : base(handle)
        {
        }
        const string NAME_PATTERN = @"^[A-Za-z0-9 _]*[A-Za-z0-9][A-Za-z0-9 \-\\_ _]*$";

        TextFieldHelper _textFieldHelper = new TextFieldHelper();
        UIButton _btnGetAccess;

        UITextField txtFieldName;
        UITextField txtFieldICNo;

        UIView viewLineName;
        UIView viewLineICNo;

        UILabel lblNameTitle;
        UILabel lblICNoTitle;

        UILabel lblNameError;
        UILabel lblICNoError;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationItem.HidesBackButton = true;
            SetNavigationItems();
            SetSubViews();
            SetEvents();
        }

        internal void SetEvents()
        {
            _btnGetAccess.TouchUpInside += (sender, e) =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("GetAccess", null);
                GetAccessSuccessViewController loginVC = storyBoard.InstantiateViewController("GetAccessSuccessViewController") as GetAccessSuccessViewController;
                loginVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                ShowViewController(loginVC, this);
            };
        }

        internal void SetNavigationItems()
        {
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(Constants.IMG_Back), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        internal void SetSubViews()
        {
            //IC Number
            UIView viewICNumber = new UIView((new CGRect(18, 16, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblICNoTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewICNumber.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                    "Common_ICNumber".Translate(),
                    font: MyTNBFont.MuseoSans9,
                    foregroundColor: MyTNBColor.SilverChalice,
                    strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            lblICNoTitle.Hidden = true;
            viewICNumber.AddSubview(lblICNoTitle);

            lblICNoError = new UILabel
            {
                Frame = new CGRect(0, 37, viewICNumber.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                    "Invalid_ICNo".Translate(),
                    font: MyTNBFont.MuseoSans9,
                    foregroundColor: MyTNBColor.Tomato,
                    strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            lblICNoError.Hidden = true;
            viewICNumber.AddSubview(lblICNoError);

            txtFieldICNo = new UITextField
            {
                Frame = new CGRect(0, 12, viewICNumber.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Common_ICNumber".Translate(),
                    font: MyTNBFont.MuseoSans16,
                    foregroundColor: MyTNBColor.SilverChalice,
                    strokeWidth: 0
                ),
                TextColor = MyTNBColor.TunaGrey()
            };
            txtFieldICNo.KeyboardType = UIKeyboardType.NumberPad;
            viewICNumber.AddSubview(txtFieldICNo);

            viewLineICNo = GenericLine.GetLine(new CGRect(0, 36, viewICNumber.Frame.Width, 1));
            viewICNumber.AddSubview(viewLineICNo);

            //FullName 
            UIView viewName = new UIView((new CGRect(18, 83, View.Frame.Width - 36, 51)));
            viewName.BackgroundColor = UIColor.Clear;

            lblNameTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewName.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                    "Common_MaidenName".Translate(),
                    font: MyTNBFont.MuseoSans9,
                    foregroundColor: MyTNBColor.SilverChalice,
                    strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            lblNameTitle.Hidden = true;
            viewName.AddSubview(lblNameTitle);

            lblNameError = new UILabel
            {
                Frame = new CGRect(0, 37, viewName.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                    "Invalid_MaidenName".Translate(),
                    font: MyTNBFont.MuseoSans9,
                    foregroundColor: MyTNBColor.Tomato,
                    strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            lblNameError.Hidden = true;
            viewName.AddSubview(lblNameError);

            txtFieldName = new UITextField
            {
                Frame = new CGRect(0, 12, viewName.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Common_MaidenName".Translate(),
                    font: MyTNBFont.MuseoSans16,
                    foregroundColor: MyTNBColor.SilverChalice,
                    strokeWidth: 0
                ),
                TextColor = MyTNBColor.TunaGrey()
            };
            viewName.AddSubview(txtFieldName);

            viewLineName = GenericLine.GetLine(new CGRect(0, 36, viewName.Frame.Width, 1));
            viewName.AddSubview(viewLineName);

            _btnGetAccess = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, View.Frame.Height - 132, View.Frame.Width - 36, 48)
            };
            _btnGetAccess.SetTitle("Common_GetAccess".Translate(), UIControlState.Normal);
            _btnGetAccess.Font = MyTNBFont.MuseoSans16;
            _btnGetAccess.Layer.CornerRadius = 5.0f;
            _btnGetAccess.BackgroundColor = MyTNBColor.SilverChalice;
            _btnGetAccess.Enabled = false;

            View.AddSubview(viewName);
            View.AddSubview(viewICNumber);
            View.AddSubview(_btnGetAccess);

            _textFieldHelper.CreateTextFieldLeftView(txtFieldName, "Name");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldICNo, "IC");
            _textFieldHelper.CreateDoneButton(txtFieldICNo);

            SetTextFieldEvents(txtFieldName, lblNameTitle, lblNameError, viewLineName, TNBGlobal.CustomerNamePattern);
            SetTextFieldEvents(txtFieldICNo, lblICNoTitle, lblICNoError, viewLineICNo, TNBGlobal.IC_NO_PATTERN);
        }

        internal void SetKeyboard(UITextField textField)
        {
            textField.AutocorrectionType = UITextAutocorrectionType.No;
            textField.AutocapitalizationType = UITextAutocapitalizationType.None;
            textField.SpellCheckingType = UITextSpellCheckingType.No;
            textField.ReturnKeyType = UIReturnKeyType.Done;
        }

        internal void SetTextFieldEvents(UITextField textField, UILabel lblTitle, UILabel lblError, UIView viewLine, string pattern)
        {
            SetKeyboard(textField);
            textField.EditingChanged += (sender, e) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
                textField.LeftViewMode = UITextFieldViewMode.Never;
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);
                lblError.Hidden = isValid || textField.Text.Length == 0;
                viewLine.BackgroundColor = isValid || textField.Text.Length == 0 ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                textField.TextColor = isValid || textField.Text.Length == 0 ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;

                SetGetAccessButtonEnable();
                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                lblTitle.Hidden = false;
            };
            textField.EditingDidEnd += (sender, e) =>
            {
                if (textField.Text.Length == 0)
                    textField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
            };
        }

        internal void SetGetAccessButtonEnable()
        {
            bool isValidICNo = _textFieldHelper.ValidateTextField(txtFieldICNo.Text, TNBGlobal.IC_NO_PATTERN);
            bool isValidName = _textFieldHelper.ValidateTextField(txtFieldName.Text, TNBGlobal.CustomerNamePattern);
            bool isValid = isValidICNo && isValidName;
            _btnGetAccess.Enabled = isValid;
            _btnGetAccess.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
        }
    }
}