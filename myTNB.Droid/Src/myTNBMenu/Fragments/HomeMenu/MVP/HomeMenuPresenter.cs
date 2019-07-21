using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Models;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuPresenter : HomeMenuContract.IUserActionsListener
    {
        internal readonly string TAG = typeof(HomeMenuPresenter).Name;
        private HomeMenuContract.IView mView;

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

        public async Task GetMyServiceService()
        {
            try
            {
                await Task.Delay(3000);
                List<MyService> dummyList = new List<MyService>();
                for (int i = 0; i < 5; i++)
                {
                    if (i == 0)
                    {
                        dummyList.Add(new MyService()
                        {
                            ServiceCategoryId = "0",
                            serviceCategoryName = "Apply for Self<br/>Meter Reading"
                        });
                    }
                    else if (i == 1)
                    {
                        dummyList.Add(new MyService()
                        {
                            ServiceCategoryId = "1",
                            serviceCategoryName = "Check<br/>Status"
                        });
                    }
                    else if (i == 2)
                    {
                        dummyList.Add(new MyService()
                        {
                            ServiceCategoryId = "2",
                            serviceCategoryName = "Give Us<br/>Feedback"
                        });
                    }
                    else if (i == 3)
                    {
                        dummyList.Add(new MyService()
                        {
                            ServiceCategoryId = "3",
                            serviceCategoryName = "Set<br/>Appointments"
                        });
                    }
                    else if (i == 4)
                    {
                        dummyList.Add(new MyService()
                        {
                            ServiceCategoryId = "4",
                            serviceCategoryName = "Apply for<br/>AutoPay"
                        });
                    }
                }
                this.mView.SetMyServiceResult(dummyList);
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

        public async Task GetNewFAQService()
        {
            try
            {
                await Task.Delay(3000);
                List<NewFAQ> dummyList = new List<NewFAQ>();
                for (int i = 0; i < 6; i++)
                {
                    if (i == 0)
                    {
                        dummyList.Add(new NewFAQ()
                        {
                            ID = "0",
                            Title = "How do I reset my password?"
                        });
                    }
                    else if (i == 1)
                    {
                        dummyList.Add(new NewFAQ()
                        {
                            ID = "1",
                            Title = "Learn how to read your meter."
                        });
                    }
                    else if (i == 2)
                    {
                        dummyList.Add(new NewFAQ()
                        {
                            ID = "2",
                            Title = "Check out how you can apply for AutoPay."
                        });
                    }
                    else if (i == 3)
                    {
                        dummyList.Add(new NewFAQ()
                        {
                            ID = "3",
                            Title = "How can I contact TNB?"
                        });
                    }
                    else if (i == 4)
                    {
                        dummyList.Add(new NewFAQ()
                        {
                            ID = "4",
                            Title = "How do i pay my bills through myTNB app?"
                        });
                    }
                    else if (i == 5)
                    {
                        dummyList.Add(new NewFAQ()
                        {
                            ID = "5",
                            Title = "What’s new on this app?"
                        });
                    }
                }
                this.mView.SetNewFAQResult(dummyList);
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
