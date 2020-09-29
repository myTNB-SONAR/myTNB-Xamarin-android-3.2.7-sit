using System.Collections.Generic;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.Base.Activity;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationDetailActivityLog.Adapter
{
    public class UpdatedDetailsListAdapter : RecyclerView.Adapter
    {
        BaseAppCompatActivity mActivity;
        List<string> services = new List<string>();

        public override int ItemCount => services.Count;
        public UpdatedDetailsListAdapter(BaseAppCompatActivity activity, List<string> data)
        {
            this.mActivity = activity;
            this.services.AddRange(data);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            throw new System.NotImplementedException();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            throw new System.NotImplementedException();
        }
    }
}
