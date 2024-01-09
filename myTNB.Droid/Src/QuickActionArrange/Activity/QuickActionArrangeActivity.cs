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
    public class QuickActionArrangeActivity : BaseActivityCustom, RearrangeQuickActionListAdapter.OnItemDismissListener, IItemClickListener, RearrangeQuickActionListAdapter.OnItemDragAndMoveListener, AddIconAdapter.IItemClickListener
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

        [BindView(Resource.Id.recyclerViewAddIcon)]
        RecyclerView recyclerViewAddIcon;

        [BindView(Resource.Id.txttitleRearrangeQuickAction)]
        TextView txttitleRearrangeQuickAction;

        [BindView(Resource.Id.txttitleExtraIcon)]
        TextView txttitleExtraIcon;

        public readonly static int REARRANGE_QUICK_ACTION_ACTIVITY_CODE = 8811;

        List<MyServiceModel> masterDataListIcon = new List<MyServiceModel>();

        List<MyServiceModel> currentIconList = new List<MyServiceModel>();

        List<MyServiceModel> masterDataListIconFiltered = new List<MyServiceModel>();

        List<Feature> lockedListFeatureIcon = new List<Feature>();

        List<Feature> extraListFeatureIcon = new List<Feature>();

        List<Feature> listIconNew = new List<Feature>();

        List<Feature> masterDataListFeatureModel = new List<Feature>();

        List<int> listAddIconCard = new List<int>();

        private RearrangeQuickActionListAdapter RearrangeQuickActionListAdapter;

        private QuickActionLockedAndExtraAdapter quickActionLockedAndExtraAdapter;

        private AddIconAdapter addIconAdapter;

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
                        masterDataListIcon = DeSerialze<List<MyServiceModel>>(extras.GetString("RearrangeQuickAction"));
                        currentIconList = masterDataListIcon;
                    }
                }
                TextViewUtils.SetMuseoSans500Typeface(btnSubmit);
                TextViewUtils.SetMuseoSans300Typeface(txttitleExtraIcon, txttitleRearrangeQuickAction);
                TextViewUtils.SetTextSize16(btnSubmit);
                TextViewUtils.SetTextSize14(txttitleExtraIcon, txttitleRearrangeQuickAction);

                txttitleExtraIcon.Text = GetLabelByLanguage("extraIconTitle");
                txttitleRearrangeQuickAction.Text = GetLabelByLanguage("defaultIconTitle");
                btnSubmit.Text = GetLabelByLanguage("saveChangesBtn");

                PopulateQuickActionList();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void PopulateQuickActionList()
        {
            try
            {
                List<MyServiceModel> tempCurrentIconList = new List<MyServiceModel>();
                Feature tempCurrentModel;
                currentIconList.RemoveAll(x => x.ServiceType == MobileEnums.ServiceEnum.VIEWLESS);
                currentIconList.RemoveAll(x => x.ServiceType == MobileEnums.ServiceEnum.VIEWMORE);

                listIconNew = UserSessions.GetQuickActionList();

                masterDataListIconFiltered = masterDataListIcon
                                    .Join(listIconNew, item1 => item1.ServiceId, item2 => item2.ServiceId, (item1, item2) => new
                                    {
                                        OriginalItem = item1,
                                        Order = listIconNew.IndexOf(item2)
                                    })
                                    .OrderBy(pair => pair.Order)
                                    .Select(pair => pair.OriginalItem)
                                    .ToList();

                if (currentIconList != null && currentIconList.Count > 0 && currentIconList.Count <= 8)
                {
                    foreach (var item in currentIconList)
                    {
                        var iconModelLocked = listIconNew?.Find(x => x.ServiceId == item.ServiceId);
                        if (iconModelLocked != null)
                        {
                            if (iconModelLocked.isLocked)
                            {
                                lockedListFeatureIcon.Add(iconModelLocked);
                            }
                            else
                            {
                                tempCurrentIconList.Add(item);
                            }
                            masterDataListFeatureModel.Add(iconModelLocked);
                        }
                        else
                        {
                            tempCurrentModel = new Feature
                            {
                                isAvailable = false,
                                isLocked = false,
                                ServiceName = item.ServiceName,
                                ServiceId = item.ServiceId
                            };

                            masterDataListFeatureModel.Add(tempCurrentModel);
                            extraListFeatureIcon.Add(tempCurrentModel);
                        }   
                    }

                    listAddIconCard.Clear();
                    int totalAfterDeduct = 8 - listIconNew.Count;
                    for (int i = 0; i < totalAfterDeduct; i++)
                    {
                        listAddIconCard.Add(i);
                    }

                    var updatedList = tempCurrentIconList
                            .Join(listIconNew, item1 => item1.ServiceId, item2 => item2.ServiceId, (item1, item2) => new
                            {
                                OriginalItem = item1,
                                Order = listIconNew.IndexOf(item2)
                            })
                            .OrderBy(pair => pair.Order)
                            .Select(pair => pair.OriginalItem)
                            .ToList();

                    currentIconList = updatedList;
                }
                else
                {
                    for (int i = 8; i <= currentIconList.Count; i++)
                    {
                        var iconModel = listIconNew?.Find(x => x.ServiceId == currentIconList[i].ServiceId);

                        tempCurrentModel = new Feature
                        {
                            isAvailable = false,
                            isLocked = iconModel.isLocked,
                            ServiceName = iconModel.ServiceName,
                            ServiceId = iconModel.ServiceId
                        };

                        extraListFeatureIcon.Add(tempCurrentModel);
                        //extraListFeatureIcon.Add(currentIconList[i]);
                        currentIconList.RemoveAt(i);
                    }

                    foreach (var item in currentIconList)
                    {
                        var iconModel = listIconNew?.Find(x => x.ServiceId == item.ServiceId);

                        if (iconModel.isLocked)
                        {
                            lockedListFeatureIcon.Add(iconModel);
                            //listLockedQuickAction.Add(item);
                        }
                        else
                        {
                            tempCurrentIconList.Add(item);
                        }
                    }
                    currentIconList = tempCurrentIconList;
                }
                DisableSaveButton();
                PopulateView();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
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
                        AddIconCard(listAddIconCard);
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

        public void lockedList()
        {
            if (lockedListFeatureIcon != null && lockedListFeatureIcon.Count > 0)
            {
                // Initialize RecyclerView
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerViewLockIcon.SetLayoutManager(layoutManager);

                // Initialize and set adapter
                quickActionLockedAndExtraAdapter = new QuickActionLockedAndExtraAdapter(this, lockedListFeatureIcon);
                recyclerViewLockIcon.SetAdapter(quickActionLockedAndExtraAdapter);
            }
            else
            {
                recyclerViewLockIcon.Visibility = ViewStates.Gone;
            }
        }

        public void canRemoveOrRearrangeList()
        {
            if (currentIconList != null && currentIconList.Count > 0)
            {
                // Initialize RecyclerView
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerView.SetLayoutManager(layoutManager);

                // Initialize and set adapter
                RearrangeQuickActionListAdapter = new RearrangeQuickActionListAdapter(currentIconList, this, recyclerView);
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
            if (extraListFeatureIcon != null && extraListFeatureIcon.Count > 0)
            {
                txttitleExtraIcon.Visibility = ViewStates.Visible;
                recyclerViewExtraIcon.Visibility = ViewStates.Visible;

                // Initialize RecyclerView
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerViewExtraIcon.SetLayoutManager(layoutManager);

                // Initialize and set adapter
                quickActionLockedAndExtraAdapter = new QuickActionLockedAndExtraAdapter(this, extraListFeatureIcon);
                quickActionLockedAndExtraAdapter.SetItemClickListener(this);
                recyclerViewExtraIcon.SetAdapter(quickActionLockedAndExtraAdapter);
                quickActionLockedAndExtraAdapter.NotifyDataSetChanged();
            }
            else
            {
                //btnNewIcon.Visibility = ViewStates.Gone;
                //txttitleExtraIcon.Visibility = ViewStates.Gone;
                recyclerViewExtraIcon.Visibility = ViewStates.Gone;
            }
        }

        public void AddIconCard(List<int> totalAddIconCard)
        {
            // Initialize RecyclerView
            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            recyclerViewAddIcon.SetLayoutManager(layoutManager);

            // Initialize and set adapter
            addIconAdapter = new AddIconAdapter(this, totalAddIconCard);
            addIconAdapter.SetItemClickListener(this);
            recyclerViewAddIcon.SetAdapter(addIconAdapter);
            addIconAdapter.NotifyDataSetChanged();
        }

        public void OnItemClickAddIcon()
        {
            if (extraListFeatureIcon != null && extraListFeatureIcon.Count > 0)
            {
                var iconModel = extraListFeatureIcon[0];
                MyServiceModel item = masterDataListIcon?.Find(x => x.ServiceId == iconModel.ServiceId);
                extraListFeatureIcon?.RemoveAt(0);
                currentIconList.Add(item);

                if (currentIconList.Count == 1)
                {
                    canRemoveOrRearrangeList();
                }
                RearrangeQuickActionListAdapter.NotifyDataSetChanged();
                AddItemCardReset();
                extraList();
                ButtonEnableDisable();
            }
        }

        public void OnItemDismiss(int adapterPosition)
        {
            Feature tempCurrentModel;
            for (int i = 0; i < currentIconList.Count; i++)
            {
                if (i == adapterPosition)
                {
                    var iconModel = masterDataListFeatureModel?.Find(x => x.ServiceId == currentIconList[i].ServiceId);

                    tempCurrentModel = new Feature
                    {
                        isAvailable = false,
                        isLocked = iconModel.isLocked,
                        ServiceName = iconModel.ServiceName,
                        ServiceId = iconModel.ServiceId
                    };

                    extraListFeatureIcon.Add(tempCurrentModel);
                    RearrangeQuickActionListAdapter.removeItemFromList(adapterPosition);
                }
            }
            AddItemCardReset();
            extraList();
            ButtonEnableDisable();
        }

        public void AddItemCardReset()
        {
            listAddIconCard.Clear();
            int totalIconList = 8 - (currentIconList.Count + lockedListFeatureIcon.Count);
            if (totalIconList > 0)
            {
                for (int i = 0; i < totalIconList; i++)
                {
                    listAddIconCard.Add(i);
                }
                AddIconCard(listAddIconCard);
            }
        }

        public void OnItemClick(int position)
        {
            if (extraListFeatureIcon != null && extraListFeatureIcon.Count > 0)
            {
                var iconModel = extraListFeatureIcon[position];
                MyServiceModel item = masterDataListIcon?.Find(x => x.ServiceId == iconModel.ServiceId);

                extraListFeatureIcon.RemoveAt(position);
                currentIconList.Add(item);

                if (currentIconList.Count == 1)
                {
                    canRemoveOrRearrangeList();
                }
                RearrangeQuickActionListAdapter.NotifyDataSetChanged();
                extraList();
                ButtonEnableDisable();
            }

            AddItemCardReset();
        }

        public void OnItemDragAndMove(bool flag)
        {
            if (flag)
            {
                ButtonEnableDisable();
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
           int indexStartSkip = lockedListFeatureIcon.Count;
           return masterDataListIconFiltered.Count >= indexStartSkip &&
                  masterDataListIconFiltered.Skip(indexStartSkip).SequenceEqual(currentIconList);
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
                        List<Feature> listFinal = new List<Feature>();
                        //<MyServiceModel> concatenatedList = listLockedQuickAction.Concat(currentIconList).ToList();

                        var concatenatedList = lockedListFeatureIcon.Cast<dynamic>().Concat(currentIconList.Cast<dynamic>()).ToList();

                        //fixListCurrentIcon
                        var updatedList = masterDataListFeatureModel
                                    .Join(concatenatedList, item1 => item1.ServiceId, item2 => item2.ServiceId, (item1, item2) => new
                                    {
                                        OriginalItem = item1,
                                        Order = concatenatedList.IndexOf(item2)
                                    })
                                    .OrderBy(pair => pair.Order)
                                    .Select(pair => pair.OriginalItem)
                                    .ToList();

                        // Update myServicesList with the sorted order
                        listFinal.Clear();
                        listFinal.AddRange(updatedList);
                        UserSessions.RemoveQuickActionList();
                        UserSessions.SetQuickActionList(listFinal);

                        Intent result = new Intent();
                        result.PutExtra("IconList", "true");
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

