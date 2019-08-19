using System;
using System.Collections.Generic;
using CoreGraphics;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class SSMRDropDownComponent
    {
        SSMRReadingHistoryViewController _controller;
        private readonly UIView _parentView;
        public UIView _containerView, _dropDownContainer, _viewRightBtn;
        UIView _viewTitleBar;
        nfloat titleBarHeight = 24f;
        nfloat lastRowMaxY = 0f;

        public SSMRDropDownComponent(SSMRReadingHistoryViewController controller, UIView parentView)
        {
            _controller = controller;
            _parentView = parentView;
        }

        private void CreateComponent()
        {
            _containerView = new UIView(new CGRect(0, 0, _parentView.Frame.Width, _parentView.Frame.Height))
            {
                BackgroundColor = MyTNBColor.Black60
            };

            CreateCloseButton();
            CreateDropDown();
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        public UIView GetView()
        {
            return _containerView;
        }

        public void SetRightButtonRecognizer(UITapGestureRecognizer recognizer)
        {
            _viewRightBtn.AddGestureRecognizer(recognizer);
        }

        private void CreateCloseButton()
        {
            int yLocation = 26;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                yLocation = 50;
            }
            _viewTitleBar = new UIView(new CGRect(0, yLocation, _containerView.Frame.Width, titleBarHeight));
            _viewRightBtn = new UIView(new CGRect(_containerView.Frame.Width - 40, 0, 24, titleBarHeight));
            UIImageView imgViewRightBtn = new UIImageView(new CGRect(0, 0, 24, titleBarHeight))
            {
                Image = UIImage.FromBundle("IC-Action-Delete-White")
            };
            _viewRightBtn.AddSubview(imgViewRightBtn);
            _viewTitleBar.AddSubview(_viewRightBtn);
            _containerView.AddSubview(_viewTitleBar);
        }

        private void CreateDropDown()
        {
            nfloat padding = 16f;
            nfloat width = _containerView.Frame.Width - (padding * 2);
            _dropDownContainer = new UIView(new CGRect(padding, _viewTitleBar.Frame.GetMaxY() + padding, width, 211f))
            {
                BackgroundColor = UIColor.White
            };
            _dropDownContainer.Layer.CornerRadius = 6.0f;
            _containerView.AddSubview(_dropDownContainer);
        }

        public void CreateMoreOptions(List<MoreOptionsItemModel> options)
        {
            foreach (var option in options)
            {
                UIView rowView = CreateListRow(option);
                rowView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    //_controller.OnMoreOptionSelected(option);
                }));
                _dropDownContainer.AddSubview(rowView);
            }
            CGRect dropDownContainerFrame = _dropDownContainer.Frame;
            dropDownContainerFrame.Height = lastRowMaxY;
            _dropDownContainer.Frame = dropDownContainerFrame;
        }

        private UIView CreateListRow(MoreOptionsItemModel model)
        {
            nfloat padding = 16f;
            UIView rowView = new UIView(new CGRect(padding, lastRowMaxY, _dropDownContainer.Frame.Width - (padding * 2), 1000f))
            {
                BackgroundColor = UIColor.Clear,
            };
            UILabel label = GetLabel(rowView, model);
            rowView.AddSubview(label);
            UIView line = new UIView(new CGRect(0, label.Frame.GetMaxY() + padding, rowView.Frame.Width, 1))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };
            rowView.AddSubview(line);

            CGRect rowFrame = rowView.Frame;
            rowFrame.Height = line.Frame.GetMaxY();
            rowView.Frame = rowFrame;
            lastRowMaxY = rowFrame.GetMaxY();

            return rowView;
        }

        private UILabel GetLabel(UIView container, MoreOptionsItemModel model)
        {
            nfloat padding = 16f;
            nfloat width = container.Frame.Width;
            UILabel label = new UILabel(new CGRect(0, padding, width, 20f))
            {
                BackgroundColor = UIColor.Clear,
                Font = model.isHighlighted ? MyTNBFont.MuseoSans14_500 : MyTNBFont.MuseoSans14_300,
                TextColor = model.isHighlighted ? MyTNBColor.Tomato : MyTNBColor.CharcoalGrey,
                Text = model?.MenuName ?? string.Empty
            };
            CGSize labelNewSize = label.SizeThatFits(new CGSize(width, 1000f));
            CGRect frame = label.Frame;
            frame.Height = labelNewSize.Height;
            label.Frame = frame;
            return label;
        }
    }
}
