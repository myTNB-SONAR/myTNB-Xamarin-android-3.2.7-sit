﻿using System.Collections.Generic;
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

        private string GetBackgroundName(List<NewWalkthroughModel> list)
        {
            var bgName = string.Empty;

            var modSix = list.Count % 6;
            bgName = "OnboardingBG" + modSix;

            return bgName;
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
                    Image = "walkthrough_img_install_0",
                    Background = GetBackgroundName(newWalkthroughList)
                });

                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title2"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description2"),
                    Image = "walkthrough_img_install_1",
                    Background = GetBackgroundName(newWalkthroughList)
                });

                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title3"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description3"),
                    Image = "walkthrough_img_install_2",
                    Background = GetBackgroundName(newWalkthroughList)
                });

                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title4"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description4"),
                    Image = "walkthrough_img_install_3",
                    Background = GetBackgroundName(newWalkthroughList)
                });

                bool IsRewardsDisabled = MyTNBAccountManagement.GetInstance().IsRewardsDisabled();
                if (!IsRewardsDisabled)
                {
                    newWalkthroughList.Add(new NewWalkthroughModel()
                    {
                        Title = Utility.GetLocalizedLabel("Onboarding", "title5"),
                        Description = Utility.GetLocalizedLabel("Onboarding", "description5"),
                        Image = "walkthrough_img_install_4",
                        Background = GetBackgroundName(newWalkthroughList)
                    });
                }

                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title7"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description7"),
                    Image = "walkthrough_img_install_5",
                    Background = GetBackgroundName(newWalkthroughList)
                });
                if (!MyTNBAccountManagement.GetInstance().IsLargeFontDisabled())
                {
                    newWalkthroughList.Add(new NewWalkthroughModel()
                    {
                        Title = Utility.GetLocalizedLabel("Onboarding", "title9"),
                        Description = Utility.GetLocalizedLabel("Onboarding", "description9"),
                        Image = "walkthrough_img_install_7",
                        Background = GetBackgroundName(newWalkthroughList)
                    });
                }
                if (!MyTNBAccountManagement.GetInstance().IsAppointmentDisabled)
                {
                    newWalkthroughList.Add(new NewWalkthroughModel()
                    {
                        Title = Utility.GetLocalizedLabel("Onboarding", "title10"),
                        Description = Utility.GetLocalizedLabel("Onboarding", "description10"),
                        Image = "walkthrough_img_install_8",
                        Background = GetBackgroundName(newWalkthroughList)
                    });
                }
                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title11"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description11"),
                    Image = "walkthrough_img_install_9",
                    Background = GetBackgroundName(newWalkthroughList)
                });
                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title12"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description12"),
                    Image = "walkthrough_img_install_10",
                    Background = GetBackgroundName(newWalkthroughList)
                });
                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title13"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description13"),
                    Image = "walkthrough_img_install_11",
                    Background = GetBackgroundName(newWalkthroughList)
                });
                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title14"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description14"),
                    Image = "walkthrough_img_install_6",
                    Background = GetBackgroundName(newWalkthroughList)
                });
            }
            else
            {
                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title6"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description6"),
                    Image = "walkthrough_img_install_0",
                    Background = GetBackgroundName(newWalkthroughList)
                });

                bool IsRewardsDisabled = MyTNBAccountManagement.GetInstance().IsRewardsDisabled();
                if (!IsRewardsDisabled)
                {
                    newWalkthroughList.Add(new NewWalkthroughModel()
                    {
                        Title = Utility.GetLocalizedLabel("Onboarding", "title5"),
                        Description = Utility.GetLocalizedLabel("Onboarding", "description5"),
                        Image = "walkthrough_img_update_1",
                        Background = GetBackgroundName(newWalkthroughList)
                    });
                }

                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title7"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description7"),
                    Image = "walkthrough_img_update_2",
                    Background = GetBackgroundName(newWalkthroughList)
                });
                
                if (!MyTNBAccountManagement.GetInstance().IsLargeFontDisabled())
                {
                    newWalkthroughList.Add(new NewWalkthroughModel()
                    {
                        Title = Utility.GetLocalizedLabel("Onboarding", "title9"),
                        Description = Utility.GetLocalizedLabel("Onboarding", "description9"),
                        Image = "walkthrough_img_update_4",
                        Background = GetBackgroundName(newWalkthroughList)
                    });
                }
                if (!MyTNBAccountManagement.GetInstance().IsAppointmentDisabled)
                {
                    newWalkthroughList.Add(new NewWalkthroughModel()
                    {
                        Title = Utility.GetLocalizedLabel("Onboarding", "title10"),
                        Description = Utility.GetLocalizedLabel("Onboarding", "description10"),
                        Image = "walkthrough_img_update_5",
                        Background = GetBackgroundName(newWalkthroughList)
                    });
                }
                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title11"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description11"),
                    Image = "walkthrough_img_install_9",
                    Background = GetBackgroundName(newWalkthroughList)
                });
                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title12"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description12"),
                    Image = "walkthrough_img_install_10",
                    Background = GetBackgroundName(newWalkthroughList)
                });
                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title13"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description13"),
                    Image = "walkthrough_img_install_11",
                    Background = GetBackgroundName(newWalkthroughList)
                });
                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = Utility.GetLocalizedLabel("Onboarding", "title14"),
                    Description = Utility.GetLocalizedLabel("Onboarding", "description14"),
                    Image = "walkthrough_img_install_6",
                    Background = GetBackgroundName(newWalkthroughList)
                });
                
                /* UserEntity activeUser = UserEntity.GetActive();

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
                 }*/
            }

            return newWalkthroughList;
        }
    }
}