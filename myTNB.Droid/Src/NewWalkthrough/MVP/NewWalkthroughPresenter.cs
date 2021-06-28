using System.Collections.Generic;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.NewWalkthrough.MVP
{
    public class NewWalkthroughPresenter
    {
        List<NewWalkthroughModel> newWalkthroughList = new List<NewWalkthroughModel>();
        NewWalkthroughContract.IView mView;
        public NewWalkthroughPresenter(NewWalkthroughContract.IView view)
        {
            this.mView = view;
            newWalkthroughList = new List<NewWalkthroughModel>();
        }

        public List<NewWalkthroughModel> GenerateNewWalkthroughList(string currentAppNavigation)
        {
            newWalkthroughList = new List<NewWalkthroughModel>();

            if (!string.IsNullOrEmpty(currentAppNavigation) && currentAppNavigation == AppLaunchNavigation.Walkthrough.ToString())
            {
                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title1"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description1"),
                    Image = "walkthrough_img_install_0"
                });

                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title2"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description2"),
                    Image = "walkthrough_img_install_1"
                });

                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title3"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description3"),
                    Image = "walkthrough_img_install_2"
                });

                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title4"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description4"),
                    Image = "walkthrough_img_install_3"
                });

                bool IsRewardsDisabled = MyTNBAccountManagement.GetInstance().IsRewardsDisabled();
                if (!IsRewardsDisabled)
                {
                    newWalkthroughList.Add(new NewWalkthroughModel()
                    {
                        Title = Utility.GetLocalizedLabel("Onboarding", "title5"),
                        Description = Utility.GetLocalizedLabel("Onboarding", "description5"),
                        Image = "walkthrough_img_install_4"
                    });
                }

                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title7"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description7"),
                    Image = "walkthrough_img_install_5"
                });
                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title8"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description8"),
                    Image = "walkthrough_img_install_6"
                });
                if (!MyTNBAccountManagement.GetInstance().IsLargeFontDisabled())
                {
                    newWalkthroughList.Add(new NewWalkthroughModel()
                    {
                        Title = Utility.GetLocalizedLabel("Onboarding", "title9"),
                        Description = Utility.GetLocalizedLabel("Onboarding", "description9"),
                        Image = "walkthrough_img_install_7"
                    });
                }
                if (!MyTNBAccountManagement.GetInstance().IsAppointmentDisabled)
                {
                    newWalkthroughList.Add(new NewWalkthroughModel()
                    {
                        Title = Utility.GetLocalizedLabel("Onboarding", "title10"),
                        Description = Utility.GetLocalizedLabel("Onboarding", "description10"),
                        Image = "walkthrough_img_install_8"
                    });
                }
            }
            else
            {
                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title6"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description6"),
                    Image = "walkthrough_img_install_0"
                });

                bool IsRewardsDisabled = MyTNBAccountManagement.GetInstance().IsRewardsDisabled();
                if (!IsRewardsDisabled)
                {
                    newWalkthroughList.Add(new NewWalkthroughModel()
                    {
                        Title = Utility.GetLocalizedLabel("Onboarding", "title5"),
                        Description = Utility.GetLocalizedLabel("Onboarding", "description5"),
                        Image = "walkthrough_img_update_1"
                    });
                }

                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title7"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description7"),
                    Image = "walkthrough_img_update_2"
                });
                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title8"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description8"),
                    Image = "walkthrough_img_update_3"
                });
                if (!MyTNBAccountManagement.GetInstance().IsLargeFontDisabled())
                {
                    newWalkthroughList.Add(new NewWalkthroughModel()
                    {
                        Title = Utility.GetLocalizedLabel("Onboarding", "title9"),
                        Description = Utility.GetLocalizedLabel("Onboarding", "description9"),
                        Image = "walkthrough_img_update_4"
                    });
                }
                if (!MyTNBAccountManagement.GetInstance().IsAppointmentDisabled)
                {
                    newWalkthroughList.Add(new NewWalkthroughModel()
                    {
                        Title = Utility.GetLocalizedLabel("Onboarding", "title10"),
                        Description = Utility.GetLocalizedLabel("Onboarding", "description10"),
                        Image = "walkthrough_img_update_5"
                    });
                }
                UserEntity activeUser = UserEntity.GetActive();

                if (activeUser != null)
                {
                    bool IsDigitalBillApplied = false; // MyTNBAccountManagement.GetInstance().IsDigitalBilApplied();
                    bool IsBillPostConversion = true;
                    if (IsDigitalBillApplied)
                    {
                        newWalkthroughList.Add(new NewWalkthroughModel()
                        {
                            Title = Utility.GetLocalizedLabel("Onboarding", "title11"),
                            Description = Utility.GetLocalizedLabel("Onboarding", "description11"),
                            Image = "walkthrough_img_install_9"
                        });
                    }
                    else if (IsBillPostConversion)
                    {
                        newWalkthroughList.Add(new NewWalkthroughModel()
                        {
                            Title = Utility.GetLocalizedLabel("Onboarding", "title11"),
                            Description = Utility.GetLocalizedLabel("Onboarding", "description11"),
                            Image = "walkthrough_img_install_9"
                        });
                    }
                    else
                    {
                        newWalkthroughList.Add(new NewWalkthroughModel()
                        {
                            Title = Utility.GetLocalizedLabel("Onboarding", "title11"),
                            Description = Utility.GetLocalizedLabel("Onboarding", "description11"),
                            Image = "walkthrough_img_install_9"
                        });
                    }
                }
            }

            return newWalkthroughList;
        }
    }
}