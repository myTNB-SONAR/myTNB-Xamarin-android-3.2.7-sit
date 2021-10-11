using System;
using myTNB;
using myTNB_Android.Src.Bills.NewBillRedesign.Model;
using System.Collections.Generic;
using myTNB.Mobile;
using System.Threading.Tasks;
using myTNB_Android.Src.Utils;
using myTNB.SitecoreCMS.Services;
using Android.App;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.SitecoreCMS.Model;
using myTNB_Android.Src.Database.Model;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Bills.NewBillRedesign.MVP
{
    public class NBRDiscoverMorePresenter : NBRDiscoverMoreContract.IUserActionsListener
    {
        private readonly NBRDiscoverMoreContract.IView view;
        private NBRDiscoverMoreModel DiscoverMoreModel;

        public NBRDiscoverMorePresenter(NBRDiscoverMoreContract.IView view)
        {
            this.view = view;
            this.view?.SetPresenter(this);
        }

        public void OnInitialize()
        {
            this.view?.SetUpViews();
            OnStart();
        }

        public void OnStart()
        {
            this.view?.UpdateView(true);
            GetNewBillDesignContent();
        }

        public Task GetNewBillDesignContent()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());

                    NewBillDesignTimeStampResponseModel timestampModel = getItemsService.GetNewBillDesignTimestampItem();
                    if (timestampModel.Status.Equals("Success") && timestampModel.Data != null && timestampModel.Data.Count > 0)
                    {
                        if (SitecoreCmsEntity.IsNeedUpdates(SitecoreCmsEntity.SITE_CORE_ID.NEW_BILL_DESIGN, timestampModel.Data[0].Timestamp))
                        {
                            NewBillDesignResponseModel responseModel = getItemsService.GetNewBillDesignItem();
                            SitecoreCmsEntity.InsertSiteCoreItem(SitecoreCmsEntity.SITE_CORE_ID.NEW_BILL_DESIGN, JsonConvert.SerializeObject(responseModel.Data), timestampModel.Data[0].Timestamp);
                        }
                    }
                    PrepareData();
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            });
        }

        private void PrepareData()
        {
            string jsonData = SitecoreCmsEntity.GetItemById(SitecoreCmsEntity.SITE_CORE_ID.NEW_BILL_DESIGN);
            if (jsonData != null)
            {
                try
                {
                    DiscoverMoreModel = new NBRDiscoverMoreModel
                    {
                        DiscoverMoreItemList = new List<NBRDiscoverMoreModel.DiscoverMoreItem>()
                    };

                    List<NewBillDesignModelEntity> newBillDesignDataList = JsonConvert.DeserializeObject<List<NewBillDesignModelEntity>>(jsonData);
                    if (newBillDesignDataList.Count > 0)
                    {
                        var headerModel = newBillDesignDataList.Find(model => { return model.IsHeader == true; });
                        DiscoverMoreModel.Title = headerModel.Title;
                        DiscoverMoreModel.Description = headerModel.Description;
                        DiscoverMoreModel.Banner1 = headerModel.Image1;
                        DiscoverMoreModel.Banner2 = headerModel.Image2;
                        DiscoverMoreModel.IsZoomable = headerModel.IsZoomable;

                        var footerModel = newBillDesignDataList.Find(model => { return model.IsFooter == true; });
                        DiscoverMoreModel.FooterMessage = footerModel.Description;

                        List<NewBillDesignModelEntity> itemList = new List<NewBillDesignModelEntity>();
                        itemList.AddRange(newBillDesignDataList.FindAll(x => x.IsHeader == false && x.IsFooter == false));

                        foreach (var item in itemList)
                        {
                            NBRDiscoverMoreModel.DiscoverMoreItem itemModel = new NBRDiscoverMoreModel.DiscoverMoreItem
                            {
                                Title = item.Title,
                                Content = item.Description,
                                Banner = item.Image1
                            };
                            DiscoverMoreModel.DiscoverMoreItemList.Add(itemModel);
                        }
                    }
                    this.view?.RenderContent(DiscoverMoreModel ?? new NBRDiscoverMoreModel());
                }
                catch (Exception e)
                {
                    PrepareDataFromLocal();
                    this.view?.RenderContent(DiscoverMoreModel ?? new NBRDiscoverMoreModel());
                }
            }
            else
            {
                PrepareDataFromLocal();
                this.view?.RenderContent(DiscoverMoreModel ?? new NBRDiscoverMoreModel());
            }
        }

        private void PrepareDataFromLocal()
        {
            try
            {
                var localContent = LanguageManager.Instance.GetSelectorsByPage<BillRedesignModel>("NewBillDesignComms");
                if (localContent.ContainsKey("newBillDesignContent"))
                {
                    List<BillRedesignModel> discoverMoreList = new List<BillRedesignModel>();
                    DiscoverMoreModel = new NBRDiscoverMoreModel
                    {
                        DiscoverMoreItemList = new List<NBRDiscoverMoreModel.DiscoverMoreItem>()
                    };
                    discoverMoreList = localContent["newBillDesignContent"];
                    if (discoverMoreList.Count > 0)
                    {
                        BillRedesignModel headerModel = discoverMoreList.Find(model => { return model.IsHeader == true; });
                        DiscoverMoreModel.Title = headerModel.Title;
                        DiscoverMoreModel.Description = headerModel.Description;
                        DiscoverMoreModel.Banner1 = headerModel.Image1;
                        DiscoverMoreModel.Banner2 = headerModel.Image2;

                        BillRedesignModel footerModel = discoverMoreList.Find(model => { return model.IsFooter == true; });
                        DiscoverMoreModel.FooterMessage = footerModel.Description;

                        List<BillRedesignModel> itemList = new List<BillRedesignModel>();
                        itemList.AddRange(discoverMoreList.FindAll(x => x.IsHeader == false && x.IsFooter == false));

                        foreach (var item in itemList)
                        {
                            NBRDiscoverMoreModel.DiscoverMoreItem itemModel = new NBRDiscoverMoreModel.DiscoverMoreItem
                            {
                                Title = item.Title,
                                Content = item.Description,
                                Banner = item.Image1
                            };
                            DiscoverMoreModel.DiscoverMoreItemList.Add(itemModel);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void Start() { }
    }
}
