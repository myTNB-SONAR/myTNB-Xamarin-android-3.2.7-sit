using CoreGraphics;

using UIKit;
namespace myTNB.Dashboard.DashboardComponents
{
    public class SmartMeterComponent
    {
        UIView _parentView;
        UIView _viewSmartMeter;
        UILabel _lblSmartMeterTitle;
        UILabel _lblSmartMeterSubtitle;
        //public UIButton _btnLearnMore;
        public SmartMeterComponent(UIView view)
        {
            _parentView = view;
        }

        internal void CreateComponent()
        {
            float viewHeight = (float)_parentView.Frame.Height - (217f + 40f);
            _viewSmartMeter = new UIView(new CGRect(42, 130, _parentView.Frame.Width - 84, viewHeight));

            var imgWidth = _viewSmartMeter.Frame.Width * .51;
            var imgHeight = _viewSmartMeter.Frame.Width * .61;
            var xLocation = _viewSmartMeter.Frame.Width / 2 - imgWidth / 2;
            UIImageView imgViewEmpty = new UIImageView(new CGRect(xLocation, 0, imgWidth, imgHeight));
            imgViewEmpty.Image = UIImage.FromBundle("Empty-Chart-Data");
            imgViewEmpty.ContentMode = UIViewContentMode.ScaleAspectFit;
            _viewSmartMeter.AddSubview(imgViewEmpty);

            UIView viewContent = new UIView(new CGRect(0, imgViewEmpty.Frame.GetMaxY() + 20, _viewSmartMeter.Frame.Width, 95));

            _lblSmartMeterTitle = new UILabel(new CGRect(0, 0, _viewSmartMeter.Frame.Width, 16));
            _lblSmartMeterTitle.TextAlignment = UITextAlignment.Center;
            _lblSmartMeterTitle.Font = myTNBFont.MuseoSans12_300();
            _lblSmartMeterTitle.Text = "Component_WelcomeToSmartMeter".Translate();
            _lblSmartMeterTitle.TextColor = UIColor.White;
            viewContent.AddSubview(_lblSmartMeterTitle);

            _lblSmartMeterSubtitle = new UILabel(new CGRect(0, _lblSmartMeterTitle.Frame.GetMaxY() + 1, _viewSmartMeter.Frame.Width, 16));
            _lblSmartMeterSubtitle.TextAlignment = UITextAlignment.Center;
            _lblSmartMeterSubtitle.Font = myTNBFont.MuseoSans12_300();
            _lblSmartMeterSubtitle.Text = "Component_SeeSmartMeter".Translate();
            _lblSmartMeterSubtitle.TextColor = UIColor.White;
            viewContent.AddSubview(_lblSmartMeterSubtitle);

            //_btnLearnMore = new UIButton(UIButtonType.Custom);
            //_btnLearnMore.Frame = new CGRect(90, 47, _viewSmartMeter.Frame.Width - 180, 48);
            //_btnLearnMore.Layer.CornerRadius = 4;
            //_btnLearnMore.Layer.BorderColor = UIColor.White.CGColor;
            //_btnLearnMore.Layer.BorderWidth = 1;
            //_btnLearnMore.SetTitle("Coming Soon", UIControlState.Normal);
            //_btnLearnMore.Font = myTNBFont.MuseoSans16();
            //_btnLearnMore.SetTitleColor(UIColor.White, UIControlState.Normal);
            //viewContent.AddSubview(_btnLearnMore);

            _viewSmartMeter.AddSubview(viewContent);
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _viewSmartMeter;
        }
    }
}