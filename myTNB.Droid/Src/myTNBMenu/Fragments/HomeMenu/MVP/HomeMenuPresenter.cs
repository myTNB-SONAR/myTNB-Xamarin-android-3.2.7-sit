using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Models;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuPresenter : HomeMenuContract.IUserActionsListener
    {
        internal readonly string TAG = typeof(HomeMenuPresenter).Name;
        private HomeMenuContract.IView mView;

        private static bool FirstTimeMyServiceInitiate = true;

        private static bool FirstTimeNewFAQInitiate = true;

        System.Timers.Timer timer;

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
                    BgStartColor = cachedDBList[i].BgStartColor,
                    BgEndColor = cachedDBList[i].BgEndColor,
                    BgDirection = cachedDBList[i].BgDirection,
                    Title = cachedDBList[i].Title,
                    Description = cachedDBList[i].Description,
                    TopicBodyTitle = cachedDBList[i].TopicBodyTitle,
                    TopicBodyContent = cachedDBList[i].TopicBodyContent,
                    CTA = cachedDBList[i].CTA,
                    Tag = cachedDBList[i].Tag,
                });
            }
            this.mView.SetNewFAQResult(cachedList);
        }

        private async Task GetMyServiceService()
        {
            try
            {
                await Task.Delay(3000);
                MyServiceEntity.RemoveAll();
                List<MyService> dummyList = new List<MyService>();
                MyService newItem = new MyService();
                for (int i = 0; i < 5; i++)
                {
                    if (i == 0)
                    {
                        newItem = new MyService()
                        {
                            ServiceCategoryId = "0",
                            serviceCategoryName = "Apply for Self<br/>Meter Reading"
                        };
                    }
                    else if (i == 1)
                    {
                        newItem = new MyService()
                        {
                            ServiceCategoryId = "1",
                            serviceCategoryName = "Check<br/>Status"
                        };
                    }
                    else if (i == 2)
                    {
                        newItem = new MyService()
                        {
                            ServiceCategoryId = "2",
                            serviceCategoryName = "Give Us<br/>Feedback"
                        };
                    }
                    else if (i == 3)
                    {
                        newItem = new MyService()
                        {
                            ServiceCategoryId = "3",
                            serviceCategoryName = "Set<br/>Appointments"
                        };
                    }
                    else if (i == 4)
                    {
                        newItem = new MyService()
                        {
                            ServiceCategoryId = "4",
                            serviceCategoryName = "Apply for<br/>AutoPay"
                        };
                    }
                    dummyList.Add(newItem);
                    MyServiceEntity.InsertOrReplace(newItem);
                }

                this.mView.SetMyServiceResult(dummyList);

                FirstTimeMyServiceInitiate = false;

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
