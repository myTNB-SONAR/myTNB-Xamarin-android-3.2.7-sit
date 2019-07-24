using CoreGraphics;
using UIKit;

namespace myTNB.SSMR
{
    public class SSMROnboardingDataController : BaseDataViewController
    {
        public SSMROnboardingDataController(UIPageViewController controller) : base(controller) { }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
        }

        public override void SetBackground()
        {
            that.View.BackgroundColor = UIColor.Clear;
        }

        public override void OnViewDidLayoutSubViews()
        {
            SetBackground();
        }

        public override void SetSubViews()
        {
            if (SSMRDataObject.IsSitecoreData)
            {
                //Todo: sitecore parse
            }
            else
            {
                //Todo: local parse
            }
            UIImageView imgBackground = new UIImageView(new CGRect(0, 0, that.View.Frame.Width, that.View.Frame.Height * 0.50F))
            {
                Image = UIImage.FromBundle(SSMRDataObject.BGImage ?? string.Empty)
            };
            UIImageView imgItem = new UIImageView(new CGRect(49, 85 + DeviceHelper.GetStatusBarHeight()
                , that.View.Frame.Width - 98, (that.View.Frame.Width - 98) * 1.04329F))
            {
                Image = UIImage.FromBundle(SSMRDataObject.MainImage ?? string.Empty)
            };
            UILabel lblTitle = new UILabel(new CGRect(16, imgItem.Frame.GetMaxY() + 23, that.View.Frame.Width - 31, 19))
            {
                TextColor = MyTNBColor.PowerBlue,
                TextAlignment = UITextAlignment.Center,
                Font = MyTNBFont.MuseoSans16_500,
                Text = SSMRDataObject.Title ?? string.Empty
            };
            UILabel lblDescription = new UILabel(new CGRect(16, lblTitle.Frame.GetMaxY() + 16, that.View.Frame.Width - 32, 80))
            {
                TextColor = MyTNBColor.WarmGrey,
                TextAlignment = UITextAlignment.Center,
                Font = MyTNBFont.MuseoSans12_300,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = SSMRDataObject.Description ?? string.Empty
            };

            that.View.AddSubviews(new UIView[] { imgBackground, imgItem, lblTitle, lblDescription });
        }
    }
}