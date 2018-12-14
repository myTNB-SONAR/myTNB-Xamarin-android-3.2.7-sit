using System;
using System.Collections.Generic;
using System.Drawing;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB.Home.More.FAQ
{
    public class FAQDataSource : UITableViewSource
    {
        List<FAQDataModel> _faqList = new List<FAQDataModel>();
        List<FAQsModel> _siteCoreFaqList = new List<FAQsModel>();
        bool _isSiteCoreContent = false;

        public FAQDataSource(List<FAQDataModel> faqList)
        {
            _faqList = faqList;
        }

        public FAQDataSource(List<FAQsModel> faqList, bool isSiteCoreContent)
        {
            _siteCoreFaqList = faqList;
            _isSiteCoreContent = isSiteCoreContent;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return _isSiteCoreContent ? _siteCoreFaqList.Count : _faqList.Count;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return 1;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            string question = _isSiteCoreContent ? _siteCoreFaqList[(int)section].Question : _faqList[(int)section].title;
            CGSize newSize = GetLabelSize(question, myTNBFont.MuseoSans16());
            UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, newSize.Height + 32));
            view.BackgroundColor = myTNBColor.SectionGrey();
            UILabel lblSectionTitle = new UILabel(new CGRect(18, 16, tableView.Frame.Width - 36, newSize.Height));
            lblSectionTitle.Text = question;
            lblSectionTitle.Font = myTNBFont.MuseoSans16();
            lblSectionTitle.TextColor = myTNBColor.PowerBlue();
            lblSectionTitle.Lines = 0;
            lblSectionTitle.LineBreakMode = UILineBreakMode.WordWrap;
            lblSectionTitle.TextAlignment = UITextAlignment.Left;
            view.Add(lblSectionTitle);
            return view;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            string answer = _isSiteCoreContent ? _siteCoreFaqList[indexPath.Section].Answer : _faqList[indexPath.Section].details;
            CGSize newSize = GetLabelSize(answer, myTNBFont.MuseoSans14());
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            var cell = tableView.DequeueReusableCell("FAQViewCell", indexPath) as FAQViewCell;

            NSMutableParagraphStyle style = new NSMutableParagraphStyle();
            style.Alignment = UITextAlignment.Justified;
            NSError htmlAnswerError = null;
            NSAttributedString htmlAnswerPeriod = new NSAttributedString(answer
                                                                           , new NSAttributedStringDocumentAttributes 
                                                                                { 
                                                                                    DocumentType = NSDocumentType.HTML,
                                                                                    StringEncoding = NSStringEncoding.UTF8
                                                                                }
                                                                           , ref htmlAnswerError);
            NSMutableAttributedString mutableHTMLAnswer = new NSMutableAttributedString(htmlAnswerPeriod);

            UIStringAttributes answerAttributes = new UIStringAttributes
            {
                Font = myTNBFont.MuseoSans14(),
                ForegroundColor = myTNBColor.TunaGrey()
            };

            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                ForegroundColor = myTNBColor.PowerBlue(),
                Font = myTNBFont.MuseoSans14(),
                UnderlineStyle = NSUnderlineStyle.Single,
                UnderlineColor = myTNBColor.PowerBlue()
            };

            mutableHTMLAnswer.AddAttributes(answerAttributes, new NSRange(0, htmlAnswerPeriod.Length));


            UITextView txtViewAnswer = new UITextView();
            txtViewAnswer.Editable = false;
            txtViewAnswer.ScrollEnabled = false;
            txtViewAnswer.TextAlignment = UITextAlignment.Justified;
            txtViewAnswer.AttributedText = mutableHTMLAnswer;
            txtViewAnswer.WeakLinkTextAttributes = linkAttributes.Dictionary;

            cell.txtViewAnswer.AttributedText = mutableHTMLAnswer;
            cell.txtViewAnswer.WeakLinkTextAttributes = linkAttributes.Dictionary;
            CGSize answerNewSize = txtViewAnswer.SizeThatFits(new CGSize(cellWidth - 36, 1000f));
            cell.txtViewAnswer.Frame = new CGRect(18
                                                     , 16
                                             , cellWidth - 36
                                             , answerNewSize.Height);

            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return (_isSiteCoreContent
                    ? GetLabelSize(_siteCoreFaqList[(int)section].Question, myTNBFont.MuseoSans16()).Height
                    : GetLabelSize(_faqList[(int)section].title, myTNBFont.MuseoSans16()).Height) + 32;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            string content = _isSiteCoreContent
                ? _siteCoreFaqList[indexPath.Section].Answer
                : _faqList[indexPath.Section].details;

            NSMutableParagraphStyle style = new NSMutableParagraphStyle();
            style.Alignment = UITextAlignment.Justified;
            NSError htmlAnswerError = null;
            NSAttributedString htmlAnswerPeriod = new NSAttributedString(content
                                                                           , new NSAttributedStringDocumentAttributes
                                                                               {
                                                                                   DocumentType = NSDocumentType.HTML,
                                                                                   StringEncoding = NSStringEncoding.UTF8
                                                                               }
                                                                           , ref htmlAnswerError);
            NSMutableAttributedString mutableHTMLAnswer = new NSMutableAttributedString(htmlAnswerPeriod);

            UIStringAttributes answerAttributes = new UIStringAttributes
            {
                Font = myTNBFont.MuseoSans14(),
                ForegroundColor = myTNBColor.TunaGrey()
            };

            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                ForegroundColor = myTNBColor.PowerBlue(),
                Font = myTNBFont.MuseoSans14(),
                UnderlineStyle = NSUnderlineStyle.Single,
                UnderlineColor = myTNBColor.PowerBlue()
            };

            mutableHTMLAnswer.AddAttributes(answerAttributes, new NSRange(0, htmlAnswerPeriod.Length));


            UITextView txtViewAnswer = new UITextView();
            txtViewAnswer.Editable = false;
            txtViewAnswer.ScrollEnabled = false;
            txtViewAnswer.TextAlignment = UITextAlignment.Justified;
            txtViewAnswer.AttributedText = mutableHTMLAnswer;
            txtViewAnswer.WeakLinkTextAttributes = linkAttributes.Dictionary;
            CGSize answerNewSize = txtViewAnswer.SizeThatFits(new CGSize(cellWidth - 36, 1000f));

            return answerNewSize.Height + 32;
        }

        CGSize GetLabelSize(string text, UIFont font)
        {
            UILabel label = new UILabel(new CGRect(18, 0, UIApplication.SharedApplication.KeyWindow.Frame.Width - 36, 1000));
            label.Font = font;
            label.Text = text;
            label.Lines = 0;
            label.LineBreakMode = UILineBreakMode.WordWrap;
            label.TextAlignment = UITextAlignment.Left;
            return label.Text.StringSize(label.Font, new SizeF((float)label.Frame.Width, 1000F));
        }
    }
}