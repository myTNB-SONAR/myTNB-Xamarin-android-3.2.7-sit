using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;

using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class DashboardAccountsDataSource : UITableViewSource
    {
        Dictionary<string, List<DueAmountDataModel>> _displayedAccounts;
        List<string> keys;
        Action<DueAmountDataModel> OnRowSelected;
        EventHandler OnTableViewScroll;

        const string CellIdentifier = "DashboardAccountCell";
        float horizontalMargin = 24.0f;

        Dictionary<string, List<DueAmountDataModel>> DisplayedAccounts
        {
            get
            {
                return _displayedAccounts;
            }
            set
            {
                if (value != null)
                {
                    _displayedAccounts = value;
                    keys = new List<string>(_displayedAccounts.Keys);
                }
                else
                {
                    _displayedAccounts = new Dictionary<string, List<DueAmountDataModel>>();
                    keys = new List<string>();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:myTNB.DashboardAccountsDataSource"/> class.
        /// </summary>
        /// <param name="accountsForDisplay">Accounts for display.</param>
        /// <param name="onSelectRow">On select row.</param>
        /// <param name="onScroll">On scroll.</param>
        public DashboardAccountsDataSource(Dictionary<string, List<DueAmountDataModel>> accountsForDisplay,
                                           Action<DueAmountDataModel> onSelectRow, EventHandler onScroll)
        {
            DisplayedAccounts = accountsForDisplay;
            OnRowSelected = onSelectRow;
            OnTableViewScroll = onScroll;
        }

        /// <summary>
        /// Gets the cell.
        /// </summary>
        /// <returns>The cell.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellIdentifier) as DashboardAccountCell;

            if (cell == null)
            {
                cell = new DashboardAccountCell(CellIdentifier);
            }

            if (IsIndexPathValid(indexPath))
            {
                var accts = DisplayedAccounts[keys[indexPath.Section]];
                var acct = accts[indexPath.Row];
                cell.UpdateCell(acct);
            }
            return cell;
        }

        /// <summary>
        /// Handles when the row is selected.
        /// </summary>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (IsIndexPathValid(indexPath))
            {
                var accts = DisplayedAccounts[keys[indexPath.Section]];
                var acct = accts[indexPath.Row];

                if (OnRowSelected != null && acct != null)
                {
                    OnRowSelected(acct);
                }

            }
        }

        /// <summary>
        /// Rowses the in section.
        /// </summary>
        /// <returns>The in section.</returns>
        /// <param name="tableview">Tableview.</param>
        /// <param name="section">Section.</param>
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            int sectionCount = keys?.Count ?? 0;
            int rowCount = 0;
            if (section < sectionCount)
            {
                var key = keys[(int)section];
                if (DisplayedAccounts.ContainsKey(key))
                {
                    var accts = DisplayedAccounts[key];
                    rowCount = accts?.Count ?? 0;
                }
            }
            return rowCount;
        }

        /// <summary>
        /// Numbers the of sections.
        /// </summary>
        /// <returns>The of sections.</returns>
        /// <param name="tableView">Table view.</param>
        public override nint NumberOfSections(UITableView tableView)
        {
            return keys?.Count ?? 0;
        }

        /// <summary>
        /// Gets the height for row.
        /// </summary>
        /// <returns>The height for row.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 66.0f;
        }

        /// <summary>
        /// Gets the height for header.
        /// </summary>
        /// <returns>The height for header.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="section">Section.</param>
        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return 36.0f;
        }

        /// <summary>
        /// Gets the view for header.
        /// </summary>
        /// <returns>The view for header.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="section">Section.</param>
        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var title = keys[(int)section];
            var amountTitle = string.Compare(title, "Dashboard_RESectionHeader".Translate()) == 0
                                    ? "Dashboard_RESectionSubHeader".Translate()
                                    : "Dashboard_SectionHeader".Translate();
            float sectionHeight = 36.0f;

            UIView sectionView = new UIView(new CGRect(0, 0, tableView.Bounds.Width, sectionHeight));
            sectionView.BackgroundColor = UIColor.FromWhiteAlpha(1, 0.15f);
            var titleWidth = sectionView.Frame.Width * 0.55f;
            UILabel lblTitle = new UILabel
            {
                Frame = new CGRect(horizontalMargin, 0, titleWidth, sectionView.Frame.Height),
                Text = title,
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = UIColor.White,
            };

            UILabel lblAmountTitle = new UILabel
            {
                Frame = new CGRect(lblTitle.Frame.GetMaxX() + 1, 0, sectionView.Frame.Width - titleWidth - horizontalMargin * 2, sectionView.Frame.Height),
                Text = amountTitle,
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Right
            };
            sectionView.AddSubviews(new UIView[] { lblTitle, lblAmountTitle });
            return sectionView;
        }

        /// <summary>
        /// Checks if the index path is valid.
        /// </summary>
        /// <returns><c>true</c>, if index path valid was ised, <c>false</c> otherwise.</returns>
        /// <param name="indexPath">Index path.</param>
        private bool IsIndexPathValid(NSIndexPath indexPath)
        {
            bool res = false;
            int section = indexPath.Section;
            int row = indexPath.Row;

            int sectionCount = keys?.Count ?? 0;
            int rowCount = 0;
            if (section < sectionCount)
            {
                var key = keys[section];
                if (DisplayedAccounts.ContainsKey(key))
                {
                    var accts = DisplayedAccounts[key];
                    rowCount = accts?.Count ?? 0;

                    if (row < rowCount)
                    {
                        res = true;
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Scrolled the specified scrollView.
        /// </summary>
        /// <param name="scrollView">Scroll view.</param>
        public override void Scrolled(UIScrollView scrollView)
        {
            OnTableViewScroll?.Invoke(scrollView, null);
        }
    }
}
