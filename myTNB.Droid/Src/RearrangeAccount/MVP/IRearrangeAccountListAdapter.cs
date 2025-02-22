﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace myTNB.AndroidApp.Src.RearrangeAccount.MVP
{
    public interface IRearrangeAccountListAdapter
    {
        /// <summary>
        /// Responsbile for ensuring the correct visiblity of the cells in the ListView. Suggested Usage:
        /// Set its value to int.MinValue in the adapter's constructor. In the GetView(...) method, determine if mMobileCellPosition == position
        /// and if this is true, set the cell.Visibility to ViewState.Invisibile. Otherwise, set the visible to ViewState.Visible.
        /// </summary>
        /// <value>The m mobile cell position.</value>
        int mMobileCellPosition { get; set; }

        /// <summary>
        /// Responsible for updating the mMobileCellPosition variable, updaing the dataset, and calling NotifyDataSetChange on the Adapter
        /// Example: 
        /// var oldValue = Items [indexOne];
        ///	Items [indexOne] = Items [indexTwo];
        ///	Items [indexTwo] = oldValue;
        ///	mMobileCellPosition = indexTwo;
        ///	NotifyDataSetChanged ();
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        void SwapItems(int from, int to);

        bool GetIsChange();

        void NotifyChanged();
    }
}
