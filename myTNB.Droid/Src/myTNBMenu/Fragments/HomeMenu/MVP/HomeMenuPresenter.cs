using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Api;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Requests;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuPresenter : HomeMenuContract.IUserActionsListener
    {
        internal readonly string TAG = typeof(HomeMenuPresenter).Name;
        private HomeMenuContract.IView mView;

        private static bool FirstTimeMyServiceInitiate = true;

        private static bool FirstTimeNewFAQInitiate = true;

        CancellationTokenSource cts;

        public HomeMenuPresenter(HomeMenuContract.IView mView)
        {
            this.mView = mView;
            this.mView?.SetPresenter(this);
        }


        public void Start()
        {
            this.mView.SetMyServiceRecycleView();
            this.mView.SetNewFAQRecycleView();
        }

        public async Task InitiateMyService()
        {
            if (MyServiceEntity.Count() == 0 || FirstTimeMyServiceInitiate)
            {
                await GetMyServiceService();
            }
            else
            {
                ReadMyServiceFromCache();
            }
        }

        public async Task RetryMyService()
        {
            await GetMyServiceService();
        }

        public async Task InitiateNewFAQ()
        {
            if (NewFAQEntity.Count() == 0 || FirstTimeNewFAQInitiate)
            {
                await GetNewFAQService();
            }
            else
            {
                ReadNewFAQFromCache();
            }
        }

        private void ReadMyServiceFromCache()
        {
            List<MyServiceEntity> cachedDBList = new List<MyServiceEntity>();
            List<MyService> cachedList = new List<MyService>();
            cachedDBList = MyServiceEntity.GetAll();
            for (int i = 0; i < cachedDBList.Count; i++)
            {
                cachedList.Add(new MyService()
                {
                    ServiceCategoryId = cachedDBList[i].ServiceCategoryId,
                    serviceCategoryName = cachedDBList[i].serviceCategoryName
                });
            }
            cachedList.Sort((a, b) => {
                int bValue = int.Parse(b.ServiceCategoryId);
                int aValue = int.Parse(a.ServiceCategoryId);
                return aValue.CompareTo(bValue);
            });
            this.mView.SetMyServiceResult(cachedList);
        }

        private void ReadNewFAQFromCache()
        {
            List<NewFAQEntity> cachedDBList = new List<NewFAQEntity>();
            List<NewFAQ> cachedList = new List<NewFAQ>();
            cachedDBList = NewFAQEntity.GetAll();
            for (int i = 0; i < cachedDBList.Count; i++)
            {
                cachedList.Add(new NewFAQ()
                {
                    ID = cachedDBList[i].ID,
                    Image = cachedDBList[i].Image,
                    BGStartColor = cachedDBList[i].BGStartColor,
                    BGEndColor = cachedDBList[i].BGEndColor,
                    BGDirection = cachedDBList[i].BGDirection,
                    Title = cachedDBList[i].Title,
                    Description = cachedDBList[i].Description,
                    TopicBodyTitle = cachedDBList[i].TopicBodyTitle,
                    TopicBodyContent = cachedDBList[i].TopicBodyContent,
                    CTA = cachedDBList[i].CTA,
                    Tags = cachedDBList[i].Tags,
                    TargetItem = cachedDBList[i].TargetItem
                });
            }
            this.mView.SetNewFAQResult(cachedList);
        }

        private async Task GetMyServiceService()
        {
            try
            {
                cts = new CancellationTokenSource();
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
                var httpClient = new HttpClient(new HttpLoggingHandler()) { BaseAddress = new Uri("http://10.215.128.191:89") };
                var getServiceAPI = RestService.For<IGetServiceApi>(httpClient);

#else
                var getServiceAPI = RestService.For<IGetServiceApi>(Constants.SERVER_URL.END_POINT);
#endif
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = Constants.DEFAULT_LANG.ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                GetServicesResponse getServicesResponse = await getServiceAPI.GetService(new GetServiceRequests()
                {
                    usrInf = currentUsrInf
                }, cts.Token);


                if (getServicesResponse.Data.ErrorCode == "7200" && getServicesResponse.Data.Data.Count > 0)
                {
                    MyServiceEntity.RemoveAll();
                    List<MyService> fetchList = new List<MyService>();
                    foreach(MyService service in getServicesResponse.Data.Data)
                    {
                        fetchList.Add(service);
                        MyServiceEntity.InsertOrReplace(service);
                    }
                    fetchList.Sort((a, b) => {
                        int bValue = int.Parse(b.ServiceCategoryId);
                        int aValue = int.Parse(a.ServiceCategoryId);
                        return aValue.CompareTo(bValue);
                    });
                    this.mView.SetMyServiceResult(fetchList);
                }
                else
                {
                    ReadMyServiceFromCache();
                    if (int.Parse(getServicesResponse.Data.ErrorCode) >= 8000 && int.Parse(getServicesResponse.Data.ErrorCode) < 9000)
                    {
                        this.mView.ShowMyServiceRetryOptions(getServicesResponse.Data.DisplayMessage);
                    }
                    else
                    {
                        this.mView.ShowMyServiceRetryOptions(null);
                    }
                }
                
                FirstTimeMyServiceInitiate = false;

            }
            catch (System.OperationCanceledException cancelledException)
            {
                ReadMyServiceFromCache();
                this.mView.ShowMyServiceRetryOptions(null);
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                ReadMyServiceFromCache();
                this.mView.ShowMyServiceRetryOptions(null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                ReadMyServiceFromCache();
                this.mView.ShowMyServiceRetryOptions(null);
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        private async Task GetNewFAQService()
        {
            try
            {
                await Task.Delay(3000);
                NewFAQEntity.RemoveAll();
                List<NewFAQ> dummyList = new List<NewFAQ>();
                NewFAQ newItem = new NewFAQ();
                for (int i = 0; i < 6; i++)
                {
                    if (i == 0)
                    {
                        newItem = new NewFAQ()
                        {
                            ID = "0",
                            Title = "How do I reset my password?"
                        };
                    }
                    else if (i == 1)
                    {
                        newItem = new NewFAQ()
                        {
                            ID = "1",
                            Title = "Learn how to read your meter."
                        };
                    }
                    else if (i == 2)
                    {
                        newItem = new NewFAQ()
                        {
                            ID = "2",
                            Title = "Check out how you can apply for AutoPay."
                        };
                        
                    }
                    else if (i == 3)
                    {
                        newItem = new NewFAQ()
                        {
                            ID = "3",
                            Title = "How can I contact TNB?"
                        };
                    }
                    else if (i == 4)
                    {
                        newItem = new NewFAQ()
                        {
                            ID = "4",
                            Title = "How do i pay my bills through myTNB app?"
                        };
                    }
                    else if (i == 5)
                    {
                        newItem = new NewFAQ()
                        {
                            ID = "5",
                            Title = "What’s new on this app?"
                        };
                    }
                    dummyList.Add(newItem);
                    NewFAQEntity.InsertOrReplace(newItem);
                }
                this.mView.SetNewFAQResult(dummyList);

                FirstTimeNewFAQInitiate = false;
            }
            catch (TaskCanceledException timeoutEx)
            {
                System.Diagnostics.Debug.WriteLine(timeoutEx);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error {0}", ex.Message);
            }
        }

        public List<MyService> LoadShimmerServiceList(int count)
        {
            if(count <= 0)
            {
                count = 1;
            }

            List<MyService> list = new List<MyService>();
            for (int i = 0; i < count; i++)
            {
                list.Add(new MyService()
                {
                    serviceCategoryName = ""
                });
            }

            return list;
        }

        public List<NewFAQ> LoadShimmerFAQList(int count)
        {
            if (count <= 0)
            {
                count = 1;
            }

            List<NewFAQ> list = new List<NewFAQ>();
            for (int i = 0; i < count; i++)
            {
                list.Add(new NewFAQ()
                {
                    Title = ""
                });
            }

            return list;
        }
    }
}
