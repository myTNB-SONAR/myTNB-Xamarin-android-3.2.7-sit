using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using myTNB.Mobile;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MyHome.Model;
using myTNB_Android.Src.QuickActionArrange.Adapter;
using myTNB_Android.Src.QuickActionArrange.Model;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using static myTNB_Android.Src.QuickActionArrange.Adapter.QuickActionLockedAndExtraAdapter;

namespace myTNB_Android.Src.QuickActionArrange.Activity
{
    [Activity(Label = "Rearrange Quick Action Icon", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class QuickActionArrangeActivity : BaseActivityCustom, RearrangeQuickActionListAdapter.OnItemDismissListener, IItemClickListener, RearrangeQuickActionListAdapter.OnItemDragAndMoveListener
    {
        [BindView(Resource.Id.rootView)]
        RelativeLayout rootView;

        [BindView(Resource.Id.btnSubmit)]
        Button btnSubmit;

        [BindView(Resource.Id.recyclerView)]
        RecyclerView recyclerView;

        [BindView(Resource.Id.recyclerViewLockIcon)]
        RecyclerView recyclerViewLockIcon;

        [BindView(Resource.Id.recyclerViewExtraIcon)]
        RecyclerView recyclerViewExtraIcon;

        [BindView(Resource.Id.txttitleRearrangeQuickAction)]
        TextView txttitleRearrangeQuickAction;

        [BindView(Resource.Id.txttitleExtraIcon)]
        TextView txttitleExtraIcon;

        [BindView(Resource.Id.btnNewIcon)]
        ImageButton btnNewIcon;

        public readonly static int REARRANGE_QUICK_ACTION_ACTIVITY_CODE = 8811;

        private List<MyServiceModel> listExistingQuickAction;

        List<MyServiceModel> listCurrentQuickAction = new List<MyServiceModel>();

        List<MyServiceModel> listLockedQuickAction = new List<MyServiceModel>();

        List<MyServiceModel> listExtraQuickAction = new List<MyServiceModel>();

        List<ArrangeQuickActionModel> updatedListCurrentIcon = new List<ArrangeQuickActionModel>();

        List<ArrangeQuickActionModel> lockedListCurrentIcon = new List<ArrangeQuickActionModel>();

        List<ArrangeQuickActionModel> extraListCurrentIcon = new List<ArrangeQuickActionModel>();

        private RearrangeQuickActionListAdapter RearrangeQuickActionListAdapter;

        private QuickActionLockedAndExtraAdapter quickActionLockedAndExtraAdapter;

        RecyclerView.LayoutManager layoutManager;

        private string PAGE_ID = "RearrangeQuickAction";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    if (extras.ContainsKey("RearrangeQuickAction"))
                    {
                        listCurrentQuickAction = DeSerialze<List<MyServiceModel>>(extras.GetString("RearrangeQuickAction"));
                        listExistingQuickAction = listCurrentQuickAction;
                    }
                }
                TextViewUtils.SetMuseoSans500Typeface(btnSubmit);
                TextViewUtils.SetMuseoSans300Typeface(txttitleExtraIcon, txttitleRearrangeQuickAction);
                TextViewUtils.SetTextSize16(btnSubmit);
                TextViewUtils.SetTextSize14(txttitleExtraIcon, txttitleRearrangeQuickAction);

                txttitleExtraIcon.Text = GetLabelByLanguage("extraIconTitle");
                txttitleRearrangeQuickAction.Text = GetLabelByLanguage("defaultIconTitle");
                btnSubmit.Text = GetLabelByLanguage("saveChangesBtn");

                btnNewIcon.Click += BtnNewIcon_Click;
                PopulateQuickActionList();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void BtnNewIcon_Click(object sender, EventArgs e)
        {
            if (listExtraQuickAction != null && listExtraQuickAction.Count > 0)
            {
                MyServiceModel item = listExtraQuickAction?.ElementAtOrDefault(0);
                extraListCurrentIcon?.RemoveAt(0);
                listExtraQuickAction?.RemoveAt(0);
                listCurrentQuickAction.Add(item);
                RearrangeQuickActionListAdapter.NotifyDataSetChanged();
                extraList();
                if (extraListCurrentIcon.Count == 0 && listExtraQuickAction.Count == 0)
                {
                    ButtonEnableDisable();
                }
                else if (listCurrentQuickAction.Count + listLockedQuickAction.Count <= 8
                    && (extraListCurrentIcon.Count > 0 && listExtraQuickAction.Count > 0))
                {
                    DisableSaveButton();
                }
                else
                {
                    ButtonEnableDisable();
                }
            }
        }

        public void PopulateQuickActionList()
        {
            List<MyServiceModel> lockedList = new List<MyServiceModel>();
            List<MyServiceModel> tempCurrentIconList = new List<MyServiceModel>();
            MyServiceModel tempModel = new MyServiceModel();
            ArrangeQuickActionModel tempCurrentModel;
            listCurrentQuickAction.RemoveAll(x => x.ServiceType == MobileEnums.ServiceEnum.VIEWLESS);
            listCurrentQuickAction.RemoveAll(x => x.ServiceType == MobileEnums.ServiceEnum.VIEWMORE);

            if (listCurrentQuickAction != null && listCurrentQuickAction.Count > 0 && listCurrentQuickAction.Count <= 8)
            {
                foreach (var item in listCurrentQuickAction)
                {
                    if (item.ServiceType == MobileEnums.ServiceEnum.VIEWBILL || item.ServiceType == MobileEnums.ServiceEnum.PAYBILL)
                    {
                        tempCurrentModel = new ArrangeQuickActionModel
                        {
                            IsDeleted = false,
                            IsLocked = true,
                            IsUserDeleted = false,
                            ServiceName = item.ServiceName,
                            ServiceType = item.ServiceType
                        };

                        lockedListCurrentIcon.Add(tempCurrentModel);
                        listLockedQuickAction.Add(item);
                    }
                    else
                    {
                        tempCurrentIconList.Add(item);
                    }
                }
                listCurrentQuickAction = tempCurrentIconList;
            }
            else if (listCurrentQuickAction != null && listCurrentQuickAction.Count > 0 && listCurrentQuickAction.Count >= 9)
            {
                for (int i = 8; i <= listCurrentQuickAction.Count; i++)
                {
                    tempCurrentModel = new ArrangeQuickActionModel
                    {
                        IsDeleted = false,
                        IsLocked = true,
                        IsUserDeleted = false,
                        ServiceName = listCurrentQuickAction[i].ServiceName,
                        ServiceType = listCurrentQuickAction[i].ServiceType
                    };

                    extraListCurrentIcon.Add(tempCurrentModel);
                    listExtraQuickAction.Add(listCurrentQuickAction[i]);
                    listCurrentQuickAction.RemoveAt(i);
                }

                foreach (var item in listCurrentQuickAction)
                {
                    if (item.ServiceType == MobileEnums.ServiceEnum.VIEWBILL || item.ServiceType == MobileEnums.ServiceEnum.PAYBILL)
                    {
                        tempCurrentModel = new ArrangeQuickActionModel
                        {
                            IsDeleted = false,
                            IsLocked = true,
                            IsUserDeleted = false,
                            ServiceName = item.ServiceName,
                            ServiceType = item.ServiceType
                        };

                        lockedListCurrentIcon.Add(tempCurrentModel);
                        listLockedQuickAction.Add(item);
                    }
                    else
                    {
                        tempCurrentIconList.Add(item);
                    }
                }
                listCurrentQuickAction = tempCurrentIconList;
            }
            DisableSaveButton();
            PopulateView();
        }

        public void lockedList()
        {
            if (listLockedQuickAction != null && listLockedQuickAction.Count > 0)
            {
                // Initialize RecyclerView
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerViewLockIcon.SetLayoutManager(layoutManager);

                // Initialize and set adapter
                quickActionLockedAndExtraAdapter = new QuickActionLockedAndExtraAdapter(this, lockedListCurrentIcon);
                recyclerViewLockIcon.SetAdapter(quickActionLockedAndExtraAdapter);
            }
            else
            {
                recyclerViewLockIcon.Visibility = ViewStates.Gone;
            }
        }

        public void canRemoveOrRearrangeList()
        {
            if (listCurrentQuickAction != null && listCurrentQuickAction.Count > 0)
            {
                // Initialize RecyclerView
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerView.SetLayoutManager(layoutManager);

                // Initialize and set adapter
                RearrangeQuickActionListAdapter = new RearrangeQuickActionListAdapter(listCurrentQuickAction, this, recyclerView);
                RearrangeQuickActionListAdapter.SetOnItemDismissListener(this);
                RearrangeQuickActionListAdapter.SetOnItemDragAndMoveListener(this);
                recyclerView.SetAdapter(RearrangeQuickActionListAdapter);

                // Set up ItemTouchHelper
                ItemTouchHelper.Callback callback = new RearrangeSwipeItemTouchHelperCallback(RearrangeQuickActionListAdapter, this);
                ItemTouchHelper itemTouchHelper = new ItemTouchHelper(callback);
                itemTouchHelper.AttachToRecyclerView(recyclerView);
            }
        }

        public void extraList()
        {
            if (listExtraQuickAction != null && listExtraQuickAction.Count > 0 && extraListCurrentIcon.Count == listExtraQuickAction.Count)
            {
                txttitleExtraIcon.Visibility = ViewStates.Visible;
                recyclerViewExtraIcon.Visibility = ViewStates.Visible;

                // Initialize RecyclerView
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerViewExtraIcon.SetLayoutManager(layoutManager);

                // Initialize and set adapter
                quickActionLockedAndExtraAdapter = new QuickActionLockedAndExtraAdapter(this, extraListCurrentIcon);
                quickActionLockedAndExtraAdapter.SetItemClickListener(this);
                recyclerViewExtraIcon.SetAdapter(quickActionLockedAndExtraAdapter);
                quickActionLockedAndExtraAdapter.NotifyDataSetChanged();
            }
            else
            {
                btnNewIcon.Visibility = ViewStates.Gone;
                txttitleExtraIcon.Visibility = ViewStates.Gone;
                recyclerViewExtraIcon.Visibility = ViewStates.Gone;
            }
        }

        public void PopulateView()
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        lockedList();
                        canRemoveOrRearrangeList();
                        extraList();
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnItemDismiss(int adapterPosition)
        {
            ArrangeQuickActionModel tempCurrentModel;
            for (int i = 0; i < listCurrentQuickAction.Count; i++)
            {
                if (i == adapterPosition)
                {
                    tempCurrentModel = new ArrangeQuickActionModel
                    {
                        IsDeleted = true,
                        IsLocked = false,
                        IsUserDeleted = true,
                        ServiceName = listCurrentQuickAction[i].ServiceName,
                        ServiceType = listCurrentQuickAction[i].ServiceType
                    };
                    extraListCurrentIcon.Add(tempCurrentModel);
                    listExtraQuickAction.Add(listCurrentQuickAction[i]);
                    RearrangeQuickActionListAdapter.removeItemFromList(adapterPosition);
                }
            }
            extraList();
            if ((listCurrentQuickAction.Count + listLockedQuickAction.Count <= 8) &&
                listExtraQuickAction != null && listExtraQuickAction.Count > 0 &&
                extraListCurrentIcon.Count == listExtraQuickAction.Count)
            {
                btnNewIcon.Visibility = ViewStates.Visible;
                DisableSaveButton();
            }
            else
            {
                ButtonEnableDisable();
            }
        }

        public void OnItemClick(int position)
        {
            if (listExtraQuickAction != null && listExtraQuickAction.Count > 0)
            {
                MyServiceModel item = listExtraQuickAction?.ElementAtOrDefault(position);
                extraListCurrentIcon.RemoveAt(position);
                listExtraQuickAction.RemoveAt(position);
                listCurrentQuickAction.Add(item);
                RearrangeQuickActionListAdapter.NotifyDataSetChanged();
                extraList();
                if (extraListCurrentIcon.Count == 0 && listExtraQuickAction.Count == 0)
                {
                    ButtonEnableDisable();
                }
                else if (listCurrentQuickAction.Count + listLockedQuickAction.Count <= 8
                    && (extraListCurrentIcon.Count > 0 && listExtraQuickAction.Count > 0))
                {
                    DisableSaveButton();
                }
                else
                {
                    ButtonEnableDisable();
                }
            }
        }

        public void OnItemDragAndMove(bool flag)
        {
            if (flag)
            {
                listCurrentQuickAction = RearrangeQuickActionListAdapter.GetCurrentList();
                if ((listCurrentQuickAction.Count + listLockedQuickAction.Count <= 8) &&
                    listExtraQuickAction != null && listExtraQuickAction.Count > 0)
                {
                    DisableSaveButton();
                }
                else
                {
                    ButtonEnableDisable();
                }
            }
        }

        public void ButtonEnableDisable()
        {
            if (CompareListChanges())
            {
                DisableSaveButton();
            }
            else
            {
                EnableSaveButton();
            }
        }

        public bool CompareListChanges()
        {
           int indexStartSkip = listLockedQuickAction.Count;
           return listExistingQuickAction.Count >= indexStartSkip &&
                  listExistingQuickAction.Skip(indexStartSkip).SequenceEqual(listCurrentQuickAction);
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.QuickActionRearrangeLayout;
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Rearrange Accounts");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        [OnClick(Resource.Id.btnSubmit)]
        void OnSubmit(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);

                OnSave();
            }
        }

        private Snackbar mRearrangeSnackbar;
        private void OnSave()
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        ShowProgressDialog();
                        // Concatenate the lists and assign the result to a new list
                        List<MyServiceModel> concatenatedList = listLockedQuickAction.Concat(listCurrentQuickAction).ToList();

                        Intent result = new Intent();
                        result.PutExtra("IconList", JsonConvert.SerializeObject(concatenatedList));
                        SetResult(Result.Ok, result);
                        Finish();
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnBackPressed()
        {
            if (btnSubmit.Enabled)
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);

                    MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(GetLabelByLanguage("rearrangeTitle"))
                        .SetMessage(GetLabelByLanguage("rearrangeMsg"))
                        .SetCTALabel(GetLabelCommonByLanguage("no"))
                        .SetCTAaction(() =>
                        {
                            SetResult(Result.Canceled);
                            this.Finish();
                        })
                        .SetSecondaryCTAaction(() =>
                        {
                            OnSave();
                        })
                        .SetSecondaryCTALabel(GetLabelCommonByLanguage("yes"))
                        .Build().Show();
                }
            }
            else
            {
                SetResult(Result.Canceled);
                this.Finish();
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }

        public void EnableSaveButton()
        {
            btnSubmit.Enabled = true;
            btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
        }

        public void DisableSaveButton()
        {
            btnSubmit.Enabled = false;
            btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}

