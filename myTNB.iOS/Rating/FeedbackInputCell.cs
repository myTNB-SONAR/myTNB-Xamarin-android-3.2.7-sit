using CoreGraphics;
using myTNB.Customs;
using myTNB.Enums;
using myTNB.Model;
using System;
using System.Collections.Generic;
using UIKit;

namespace myTNB
{
    public partial class FeedbackInputCell : UITableViewCell
    {
        public FeedbackTextView feedbackTextView;
        public UITextField txtFieldComments;
        public UILabel lblQuestion;

        private UIView ratingView, commentView, parentView, viewRating
            , viewLine;

        private UILabel lblRateTitle, lblFeedbackSubTitle, lblNumber;
        private UIImageView iconFeedback;

        private nfloat textWidth;
        private List<QuestionInputOption> choices;
        private FeedbackQuestionModel questionCache;

        private int verticalMargin = 16;
        private int horizontalMargin = 18;
        private float starsViewWidth = 184;
        private float TXTVIEW_DEFAULT_MARGIN = 24f;
        private float COMMENT_VIEW_HEIGHT = 64f;

        public QuestionTypeEnum QuestionType;
        public int Rating;

        public FeedbackInputCell(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        public FeedbackInputCell(string cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            Initialize();
        }

        private void Initialize()
        {
            nfloat parentViewWidth = ContentView.Bounds.Width - horizontalMargin * 2;
            parentView = new UIView(new CGRect(horizontalMargin, verticalMargin
                , parentViewWidth, ContentView.Bounds.Height - verticalMargin * 2))
            {
                UserInteractionEnabled = false
            };

            lblNumber = new UILabel
            {
                Frame = new CGRect(0, 0, 20, 18),
                Font = MyTNBFont.MuseoSans18_500,
                TextColor = MyTNBColor.PowerBlue,
                TextAlignment = UITextAlignment.Left,
                Lines = 0
            };
            parentView.AddSubview(lblNumber);
            SelectionStyle = UITableViewCellSelectionStyle.None;

            textWidth = parentViewWidth - lblNumber.Frame.Width - 5 - horizontalMargin;
            lblQuestion = new UILabel
            {
                Frame = new CGRect(lblNumber.Frame.GetMaxX() + 5, 0, textWidth, 60),
                Font = MyTNBFont.MuseoSans18_500,
                TextColor = MyTNBColor.PowerBlue,
                TextAlignment = UITextAlignment.Left,
                Lines = 0,
            };

            parentView.AddSubview(lblQuestion);

            CreateRatingQuestion(lblQuestion.Frame);
            CreateCommentQuestion(lblQuestion.Frame);
            CreateRatingSubViews(viewRating);
            ContentView.AddSubview(parentView);
        }

        internal void UpdateCell(FeedbackQuestionModel question, int questionNumber, int defaultRating, double tableViewWidth)
        {
            if (ContentView.Bounds.Width > tableViewWidth)
            {
                nfloat adjWidth = (nfloat)((ContentView.Bounds.Width - tableViewWidth) * -1.0f);
                ViewHelper.AdjustFrameWidth(lblQuestion, adjWidth);
                ViewHelper.AdjustFrameWidth(lblFeedbackSubTitle, adjWidth);
                ViewHelper.AdjustFrameWidth(feedbackTextView, adjWidth);
                ViewHelper.AdjustFrameWidth(viewLine, adjWidth);
                ViewHelper.AdjustFrameWidth(ratingView, adjWidth);
                ViewHelper.AdjustFrameWidth(lblRateTitle, adjWidth);
                nfloat adjX = (ratingView.Frame.Width / 2) - (starsViewWidth / 2);
                ViewHelper.AdjustFrameSetX(viewRating, adjX);
            }

            lblNumber.Text = string.Format("{0}.", questionNumber + 1);
            lblQuestion.Text = question.Question;
            lblNumber.SizeToFit();
            lblQuestion.SizeToFit();

            questionCache = question;
            QuestionType = question.Kind;
            choices = question.InputOptionValue;
            Rating = defaultRating;

            switch (question.Kind)
            {
                case QuestionTypeEnum.Rating:
                    {
                        ratingView.Hidden = false;
                        commentView.Hidden = true;
                        ViewHelper.AdjustFrameSetY(ratingView, lblQuestion.Frame.GetMaxY() + verticalMargin);
                        ChooseRating(Rating - 1);
                    }
                    break;
                case QuestionTypeEnum.MultilineComment:
                    {
                        ratingView.Hidden = true;
                        commentView.Hidden = false;
                        ViewHelper.AdjustFrameSetY(commentView, lblQuestion.Frame.GetMaxY() + verticalMargin);
                    }
                    break;
            }
        }

        /// <summary>
        /// Creates the rating question.
        /// </summary>
        /// <param name="baseFrame">Base frame.</param>
        private void CreateRatingQuestion(CGRect baseFrame)
        {
            ratingView = new UIView(new CGRect(0, baseFrame.GetMaxY() + verticalMargin, ContentView.Frame.Width, 64));

            // stars parent
            viewRating = new UIView(new CGRect((ratingView.Frame.Width / 2) - (starsViewWidth / 2), verticalMargin, starsViewWidth, 32));

            lblRateTitle = new UILabel(new CGRect(0, viewRating.Frame.GetMaxY() + verticalMargin, ratingView.Frame.Width, 16))
            {
                Font = MyTNBFont.MuseoSans14_300,
                TextColor = MyTNBColor.TunaGrey(),
                TextAlignment = UITextAlignment.Center
            };

            ratingView.AddSubviews(new UIView[] { lblRateTitle, viewRating });
            ContentView.AddSubview(ratingView);
        }

        /// <summary>
        /// Creates the rating sub views.
        /// </summary>
        /// <param name="containerView">Container view.</param>
        private void CreateRatingSubViews(UIView containerView)
        {
            UIView viewStar;
            UIImageView imgStar;
            int xLocation = 0;
            int count = 5; //choices?.Count ?? 0;

            for (int index = 0; index < count; index++)
            {
                viewStar = new UIView(new CGRect(xLocation, 0, 32, 32))
                {
                    Tag = index
                };
                imgStar = new UIImageView(new CGRect(0, 0, 32, 32))
                {
                    Image = UIImage.FromBundle("IC-Action-Rating-Active")
                };
                viewStar.AddSubview(imgStar);

                UITapGestureRecognizer tapGesture = new UITapGestureRecognizer((sender) =>
                {
                    UIView view = sender.View as UIView;

                    if (view != null)
                    {
                        int itemIndex = (int)view.Tag;
                        ChooseRating(itemIndex);
                    }
                });

                tapGesture.NumberOfTapsRequired = 1;
                viewStar.AddGestureRecognizer(tapGesture);
                viewStar.UserInteractionEnabled = true;
                containerView.AddSubview(viewStar);
                xLocation += 32 + 6;
            }
        }

        /// <summary>
        /// Chooses the rating from the UI.
        /// </summary>
        /// <param name="index">Index.</param>
        private void ChooseRating(int index)
        {
            UIImageView imgView;
            int count = choices?.Count ?? 0;

            if (index >= count)
            {
                return;
            }

            Rating = index + 1;
            questionCache.Answer = Rating.ToString();

            if (index > -1)
            {
                lblRateTitle.Text = choices[index].InputOptionValues;
            }

            for (int i = 0; i < count; i++)
            {
                imgView = viewRating.Subviews[i].Subviews[0] as UIImageView;
                if (imgView != null)
                {
                    imgView.Image = UIImage.FromBundle(i <= index
                        ? "IC-Action-Rating-Active" : "IC-Action-Rating-Inactive");
                }
            }
        }

        /// <summary>
        /// Creates the comment question.
        /// </summary>
        /// <param name="baseFrame">Base frame.</param>
        private void CreateCommentQuestion(CGRect baseFrame)
        {
            commentView = new UIView(new CGRect(baseFrame.X + horizontalMargin, baseFrame.GetMaxY() + verticalMargin, textWidth, COMMENT_VIEW_HEIGHT));


            feedbackTextView = new FeedbackTextView
            {
                Frame = new CGRect(24, 0, textWidth, 36),
                Editable = true,
                Font = MyTNBFont.MuseoSans16_300,
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.TunaGrey(),
                BackgroundColor = UIColor.Clear,
                EnablesReturnKeyAutomatically = true,
                TranslatesAutoresizingMaskIntoConstraints = true,
                ScrollEnabled = true,
            };

            iconFeedback = new UIImageView(new CGRect(0, 5, 24, 24))
            {
                Image = UIImage.FromBundle("IC-Feedback")
            };

            lblFeedbackSubTitle = new UILabel(new CGRect(0, feedbackTextView.Frame.GetMaxY() + 3, textWidth, 16))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans9_300
            };

            feedbackTextView.SetPlaceholder(LanguageUtility.GetCommonI18NValue(Constants.Common_Comments));
            feedbackTextView.CreateDoneButton();

            viewLine = new UIView(new CGRect(0, feedbackTextView.Frame.GetMaxY() + 1, textWidth, 1));
            viewLine.BackgroundColor = MyTNBColor.SilverChalice;
            commentView.AddSubviews(new UIView[] { feedbackTextView, lblFeedbackSubTitle, iconFeedback, viewLine });
            ContentView.AddSubview(commentView);
            SetTextViewEvents(feedbackTextView, viewLine);
            HandleFeedbackTextViewChange();
        }

        /// <summary>
        /// Sets the text view events.
        /// </summary>
        /// <param name="textView">Text view.</param>
        /// <param name="lineView">Line view.</param>
        private void SetTextViewEvents(FeedbackTextView textView, UIView lineView)
        {
            feedbackTextView.SetKeyboard();
            textView.Changed += (sender, e) =>
            {
                FeedbackTextView txtView = sender as FeedbackTextView;
                if (txtView == feedbackTextView)
                {
                    HandleFeedbackTextViewChange();

                    CGRect frame = new CGRect();
                    frame = feedbackTextView.Frame;
                    frame.Height = feedbackTextView.ContentSize.Height <= TNBGlobal.FEEDBACK_FIELD_MAX_HEIGHT
                        ? feedbackTextView.ContentSize.Height : TNBGlobal.FEEDBACK_FIELD_MAX_HEIGHT;
                    feedbackTextView.Frame = frame;

                    ViewHelper.AdjustFrameSetHeight(commentView, COMMENT_VIEW_HEIGHT + feedbackTextView.Frame.Height);
                    ViewHelper.AdjustFrameSetY(lineView, feedbackTextView.Frame.GetMaxY() + 3f);
                    ViewHelper.AdjustFrameSetY(lblFeedbackSubTitle, lineView.Frame.GetMaxY() + 3f);

                    if (txtView.Text.Length > 0)
                    {
                        feedbackTextView.SetPlaceholderHidden(true);
                        iconFeedback.Hidden = true;
                        frame.X = 0;
                        feedbackTextView.Frame = frame;
                    }
                    else
                    {
                        feedbackTextView.SetPlaceholderHidden(false);
                        iconFeedback.Hidden = false;
                        frame.X = TXTVIEW_DEFAULT_MARGIN;
                        feedbackTextView.Frame = frame;
                    }

                    if (questionCache != null)
                    {
                        questionCache.Answer = feedbackTextView.Text;
                    }
                }
            };
            textView.ShouldEndEditing = (sender) =>
            {
                iconFeedback.Hidden = feedbackTextView.Text.Length > 0;

                return true;
            };
            textView.ShouldChangeText += (txtView, range, replacementString) =>
            {
                if (txtView == feedbackTextView)
                {
                    nint newLength = textView.Text.Length + replacementString.Length - range.Length;
                    return newLength <= TNBGlobal.FeedbackMaxCharCount;
                }
                return true;
            };
            textView.ShouldBeginEditing = (sender) =>
            {
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
                return true;
            };
        }

        /// <summary>
        /// Handles the feedback text change.
        /// </summary>
        private void HandleFeedbackTextViewChange()
        {
            int charCount = TNBGlobal.FeedbackMaxCharCount - feedbackTextView.Text.Length;
            lblFeedbackSubTitle.Text = string.Format(LanguageUtility.GetCommonI18NValue(charCount > 1
                ? Constants.Common_CharactersLeft : Constants.Common_CharacterLeft), charCount);
        }
    }
}