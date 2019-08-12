using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB.Model;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using UIKit;

namespace myTNB
{
    public class ModelController : UIPageViewControllerDataSource
    {
        public List<OnboardingModel> pageData;

        public UIButton btnSkip;
        public UIButton btnDone;
        public UIView viewNext;
        public int currentIndex = 0;
        public bool isNextTapped = false;

        public ModelController()
        {
            pageData = new List<OnboardingModel>();

            var dashboard = new OnboardingModel
            {
                ImageName = "Dashboard",
                Title = "Onboarding_UsageTitle".Translate(),
                Message = "Onboarding_UsageMessage".Translate()
            };
            pageData.Add(dashboard);

            var billing = new OnboardingModel
            {
                ImageName = "Billing",
                Title = "Onboarding_BillingTitle".Translate(),
                Message = "Onboarding_BillingMessage".Translate()
            };
            pageData.Add(billing);

            var payment = new OnboardingModel
            {
                ImageName = "Payment",
                Title = "Onboarding_PaymentTitle".Translate(),
                Message = "Onboarding_PaymentMessage".Translate()
            };
            pageData.Add(payment);
        }

        public Task SetPageData()
        {
            return Task.Factory.StartNew(() =>
            {
                WalkthroughScreensEntity wsManager = new WalkthroughScreensEntity();
                List<WalkthroughScreensModel> walkThroughScreenList = wsManager.GetAllItems();
                if (walkThroughScreenList.Count > 0)
                {
                    pageData = new List<OnboardingModel>();
                    foreach (var entity in walkThroughScreenList)
                    {
                        OnboardingModel item = new OnboardingModel
                        {
                            Title = entity.Text,
                            Message = entity.SubText,
                            ImageName = entity.Image,
                            IsSitecoreData = true
                        };
                        pageData.Add(item);
                    }
                }
            });
        }

        public GenericPageDataViewController GetViewController(int index, UIStoryboard storyboard)
        {
            if (index >= pageData.Count)
            {
                return null;
            }

            // Create a new view controller and pass suitable data.
            GenericPageDataViewController dataViewController = storyboard.InstantiateViewController("GenericPageDataViewController") as GenericPageDataViewController;
            dataViewController.DataObject = pageData[index];
            dataViewController.PageType = GenericPageViewEnum.Type.Onboarding;
            return dataViewController;
        }

        public int IndexOf(GenericPageDataViewController viewController)
        {
            return pageData.IndexOf(viewController.DataObject);
        }

        #region Page View Controller Data Source

        public override UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            int index = IndexOf((GenericPageDataViewController)referenceViewController);
            currentIndex = index;
            btnSkip.Hidden = index == pageData.Count - 1;
            btnDone.Hidden = index != pageData.Count - 1;
            viewNext.Hidden = index == pageData.Count - 1;
            if (index == -1 || index == pageData.Count - 1)
            {
                return null;
            }
            return GetViewController(index + 1, referenceViewController.Storyboard);
        }

        public override UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            int index = IndexOf((GenericPageDataViewController)referenceViewController);
            currentIndex = index;
            btnSkip.Hidden = index == pageData.Count - 1;
            btnDone.Hidden = index != pageData.Count - 1;
            viewNext.Hidden = index == pageData.Count - 1;
            if (index == -1 || index == 0)
            {
                return null;
            }
            return GetViewController(index - 1, referenceViewController.Storyboard);
        }

        public override nint GetPresentationCount(UIPageViewController pageViewController)
        {
            var appearance = UIPageControl.Appearance;
            appearance.CurrentPageIndicatorTintColor = MyTNBColor.SunGlow;
            appearance.BackgroundColor = UIColor.Clear;
            appearance.PageIndicatorTintColor = UIColor.White;
            return pageData.Count;
        }

        public override nint GetPresentationIndex(UIPageViewController pageViewController)
        {
            if (isNextTapped)
            {
                isNextTapped = false;
                currentIndex++;
                return currentIndex;
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}