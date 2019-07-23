using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;

namespace myTNB.SSMR.Onboarding
{
    public class SSMROnboardingModelController : UIPageViewControllerDataSource
    {
        public List<SSMROnboardingModel> pageData;
        public int currentIndex;
        public bool isNextTapped;
        public bool isSkipTapped;
        private Dictionary<string, string> I18NDictionary;

        public SSMROnboardingModelController()
        {
            I18NDictionary = LanguageManager.Instance.GetValuesByPage("SSMROnboarding");
            pageData = new List<SSMROnboardingModel>();
            var item1 = new SSMROnboardingModel
            {
                BGImage = SSMRConstants.IMG_BGOnboarding1,
                MainImage = SSMRConstants.IMG_Onboarding1,
                Title = GetI18NValue(SSMRConstants.I18N_Title1),
                Description = GetI18NValue(SSMRConstants.I18N_Description1),
            };
            pageData.Add(item1);

            var item2 = new SSMROnboardingModel
            {
                BGImage = SSMRConstants.IMG_BGOnboarding2,
                MainImage = SSMRConstants.IMG_Onboarding2,
                Title = GetI18NValue(SSMRConstants.I18N_Title2),
                Description = GetI18NValue(SSMRConstants.I18N_Description2),
            };
            pageData.Add(item2);

            var item3 = new SSMROnboardingModel
            {
                BGImage = SSMRConstants.IMG_BGOnboarding3,
                MainImage = SSMRConstants.IMG_Onboarding3,
                Title = GetI18NValue(SSMRConstants.I18N_Title3),
                Description = GetI18NValue(SSMRConstants.I18N_Description3),
            };
            pageData.Add(item3);
        }

        public Task SetPageData()
        {
            return Task.Factory.StartNew(() =>
            {
                /*WalkthroughScreensEntity wsManager = new WalkthroughScreensEntity();
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
                }*/
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
            dataViewController.SSMRDataObject = pageData[index];
            dataViewController.PageType = GenericPageViewEnum.Type.SSMR;
            return dataViewController;
        }

        public int IndexOf(GenericPageDataViewController viewController)
        {
            return pageData.IndexOf(viewController.SSMRDataObject);
        }

        public string GetI18NValue(string key)
        {
            return I18NDictionary.ContainsKey(key) ? I18NDictionary[key] : string.Empty;
        }

        #region Page View Controller Data Source

        public override UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            int index = IndexOf((GenericPageDataViewController)referenceViewController);
            currentIndex = index;
            // btnSkip.Hidden = index == pageData.Count - 1;
            // btnDone.Hidden = index != pageData.Count - 1;
            // viewNext.Hidden = index == pageData.Count - 1;
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
            // btnSkip.Hidden = index == pageData.Count - 1;
            // btnDone.Hidden = index != pageData.Count - 1;
            // viewNext.Hidden = index == pageData.Count - 1;
            if (index == -1 || index == 0)
            {
                return null;
            }
            return GetViewController(index - 1, referenceViewController.Storyboard);
        }

        public override nint GetPresentationCount(UIPageViewController pageViewController)
        {
            var appearance = UIPageControl.Appearance;
            appearance.CurrentPageIndicatorTintColor = MyTNBColor.WaterBlue;
            appearance.BackgroundColor = UIColor.Clear;
            appearance.PageIndicatorTintColor = MyTNBColor.VeryLightPinkTwo;
            return pageData.Count;
        }

        public override nint GetPresentationIndex(UIPageViewController pageViewController)
        {
            if (isSkipTapped)
            {
                isSkipTapped = false;
               // currentIndex++;
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
