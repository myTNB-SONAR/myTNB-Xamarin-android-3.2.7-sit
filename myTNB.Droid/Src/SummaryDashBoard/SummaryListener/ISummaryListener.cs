using System;
using myTNB_Android.Src.SummaryDashBoard.Models;

namespace myTNB_Android.Src.SummaryDashBoard.SummaryListener
{
    public interface ISummaryListener
    {
        void OnClick(SummaryDashBoardDetails summaryDashBoardDetails);
    }
}
