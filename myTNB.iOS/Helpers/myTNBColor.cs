using System;
using UIKit;  namespace myTNB {
    public static class MyTNBColor
    {
        /// <summary>         /// A color with RGBA of (0.47, 0.19, 0.83, 1).         /// </summary>         public static UIColor GradientPurpleDarkElement
        {
            get
            {
                return new UIColor(red: 0.47f, green: 0.19f, blue: 0.83f, alpha: 1.0f);
            }
        }
        /// <summary>         /// A color with RGBA of (0.16, 0.59, 0.98, 1).         /// </summary>
		public static UIColor GradientPurpleLightElement
        {
            get
            {
                return new UIColor(red: 0.16f, green: 0.59f, blue: 0.98f, alpha: 1.0f);
            }
        }
        /// <summary>         /// A color with RGBA of (255, 255, 255, 0.6).         /// </summary>
		public static UIColor LightGray
        {
            get
            {
                return UIColor.FromRGBA(255f, 255f, 255f, 0.6f);
            }
        }
        /// <summary>         /// A color with RGBA of (0.96, 0.96, 0.96, 1).         /// </summary>
		public static UIColor LightGrayBG
        {
            get
            {
                return new UIColor(red: 0.96f, green: 0.96f, blue: 0.96f, alpha: 1.0f);
            }
        }         /// <summary>         /// A color with RGBA of (0.65, 0.65, 0.65, 1).         /// </summary>
		public static UIColor SilverChalice
        {
            get
            {
                return new UIColor(red: 0.65f, green: 0.65f, blue: 0.65f, alpha: 1.0f);
            }
        }
        /// <summary>         /// A color with RGBA of (0.89, 0.89, 0.89, 1).         /// </summary>
		public static UIColor PlatinumGrey
        {
            get
            {
                return new UIColor(red: 0.89f, green: 0.89f, blue: 0.89f, alpha: 1.0f);
            }
        }
        /// <summary>         /// A color with RGBA of (0.89, 0.20, 0.10, 1).         /// </summary>
		public static UIColor Tomato
        {
            get
            {
                return new UIColor(red: 0.89f, green: 0.20f, blue: 0.10f, alpha: 1.0f);
            }
        }         /// <summary>         /// A color with RGBA of (1, 0.80, 0.22, 1).         /// </summary>
		public static UIColor SunGlow
        {
            get
            {
                return new UIColor(red: 1.0f, green: 0.80f, blue: 0.22f, alpha: 1.0f);
            }
        }
        /// <summary>         /// A color with RGBA of (0.13, 0.74, 0.30, 1).         /// </summary>
		public static UIColor FreshGreen
        {
            get
            {
                return new UIColor(red: 0.13f, green: 0.74f, blue: 0.30f, alpha: 1.0f);
            }
        }
        /// <summary>         /// A color with RGBA of (0.29, 0.29, 0.29, 1).         /// </summary>
		public static UIColor TunaGrey(float customAlpha = 1.0f)
        {
            return new UIColor(red: 73.0f / 255.0f, green: 73.0f / 255.0f, blue: 74.0f / 255.0f, alpha: customAlpha);
        }
        /// <summary>         /// A color with RGBA of (0.11, 0.47, 0.79, 1).         /// </summary>
		public static UIColor PowerBlue
        {
            get
            {
                return new UIColor(red: 0.11f, green: 0.47f, blue: 0.79f, alpha: 1.0f);
            }
        }
        /// <summary>         /// A color with RGBA of (0.97, 0.97, 0.97, 1).         /// </summary>         public static UIColor SectionGrey         {
            get
            {
                return new UIColor(red: 0.97f, green: 0.97f, blue: 0.97f, alpha: 1.0f);
            }         }
        /// <summary>         /// A color with RGBA of (0.90, 0.90, 0.90, 1).         /// </summary>         public static UIColor SelectionGrey         {
            get
            {
                return new UIColor(red: 0.90f, green: 0.90f, blue: 0.90f, alpha: 1.0f);
            }         }
        /// <summary>         /// Selection semi transparent color.         /// </summary>         /// <returns>The semi transparent.</returns>         public static UIColor SelectionSemiTransparent         {
            get
            {
                return UIColor.FromRGBA(255, 255, 255, 60);
            }         }

        /// <summary>
        /// Gray for lines
        /// </summary>
        /// <returns>The gray.</returns>
        public static UIColor LinesGray
        {
            get
            {
                return new UIColor(red: 0.85f, green: 0.85f, blue: 0.85f, alpha: 1.0f);
            }
        }
    } }