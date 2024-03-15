using Android.Views;
using Android.Widget;

namespace myTNB.AndroidApp.Src.Utils
{
    public static class ListViewUtils
    {
        public static void SetNoScroll(this ListView listView)
        {
            var listAdapter = listView.Adapter as IListAdapter;
            if (listAdapter != null)
            {

                int numberOfItems = listAdapter.Count;

                // Get total height of all items.
                int totalItemsHeight = 0;
                for (int itemPos = 0; itemPos < numberOfItems; itemPos++)
                {
                    View item = listAdapter.GetView(itemPos, null, listView);
                    item.Measure(0, 0);
                    totalItemsHeight += item.MeasuredHeight;
                }

                // Get total height of all item dividers.
                int totalDividersHeight = listView.DividerHeight *
                        (numberOfItems - 1);

                // Set list height.
                ViewGroup.LayoutParams layoutParams = listView.LayoutParameters;
                layoutParams.Height = totalItemsHeight + totalDividersHeight;
                listView.LayoutParameters = (layoutParams);
                listView.RequestLayout();



            }

        }
    }
}