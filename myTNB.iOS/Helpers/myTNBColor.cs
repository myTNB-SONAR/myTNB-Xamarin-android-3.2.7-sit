using System;
using UIKit;  namespace myTNB {
    public static class MyTNBColor
    {
        readonly static UIColor _gradientPurpleDarkElement = new UIColor(red: 0.47f, green: 0.19f, blue: 0.83f, alpha: 1.0f);
        readonly static UIColor _gradientPurpleLightElement = new UIColor(red: 0.16f, green: 0.59f, blue: 0.98f, alpha: 1.0f);
        readonly static UIColor _lightGray = UIColor.FromRGBA(255f, 255f, 255f, 0.6f);
        readonly static UIColor _lightGrayBG = new UIColor(red: 0.96f, green: 0.96f, blue: 0.96f, alpha: 1.0f);
        readonly static UIColor _silverChalice = new UIColor(red: 0.65f, green: 0.65f, blue: 0.65f, alpha: 1.0f);
        readonly static UIColor _platinumGrey = new UIColor(red: 0.89f, green: 0.89f, blue: 0.89f, alpha: 1.0f);
        readonly static UIColor _tomato = new UIColor(red: 0.89f, green: 0.20f, blue: 0.10f, alpha: 1.0f);
        readonly static UIColor _sunGlow = new UIColor(red: 1.0f, green: 0.80f, blue: 0.22f, alpha: 1.0f);
        readonly static UIColor _freshGreen = new UIColor(red: 0.13f, green: 0.74f, blue: 0.30f, alpha: 1.0f);
        readonly static UIColor _powerBlue = new UIColor(red: 0.11f, green: 0.47f, blue: 0.79f, alpha: 1.0f);
        readonly static UIColor _sectionGrey = new UIColor(red: 0.97f, green: 0.97f, blue: 0.97f, alpha: 1.0f);
        readonly static UIColor _selectionGrey = new UIColor(red: 0.90f, green: 0.90f, blue: 0.90f, alpha: 1.0f);
        readonly static UIColor _selectionSemiTransparent = UIColor.FromRGBA(255, 255, 255, 60);
        readonly static UIColor _linesGray = new UIColor(red: 0.85f, green: 0.85f, blue: 0.85f, alpha: 1.0f);
        readonly static UIColor _neonBlue = new UIColor(red: 97f / 255.0f, green: 91f / 255.0f, blue: 228f / 255.0f, alpha: 1.0f);
        readonly static UIColor _harleyDavidsonOrange = new UIColor(red: 0.89F, green: 0.20F, blue: 0.10F, alpha: 1.0f);
        readonly static UIColor _denim = new UIColor(red: 0.11F, green: 0.47F, blue: 0.79F, alpha: 1.0f);
        readonly static UIColor _grey = new UIColor(red: 0.55f, green: 0.55f, blue: 0.55f, alpha: 1.0f);
        readonly static UIColor _babyBlue = new UIColor(red: 0.61f, green: 0.82f, blue: 1f, alpha: 1.0f);
        readonly static UIColor _waterBlue = new UIColor(red: 0.11f, green: 0.27f, blue: 0.79f, alpha: 1.0f);
        readonly static UIColor _clearBlue = new UIColor(red: 0.16f, green: 0.59f, blue: 0.98f, alpha: 1.0f);

        /// <summary>         /// A color with RGBA of (0.47, 0.19, 0.83, 1).         /// </summary>         public static UIColor GradientPurpleDarkElement
        {
            get
            {
                return _gradientPurpleDarkElement;
            }
        }
        /// <summary>         /// A color with RGBA of (0.16, 0.59, 0.98, 1).         /// </summary>
        public static UIColor GradientPurpleLightElement
        {
            get
            {

                return _gradientPurpleLightElement;
            }
        }
        /// <summary>         /// A color with RGBA of (255, 255, 255, 0.6).         /// </summary>
        public static UIColor LightGray
        {
            get
            {
                return _lightGray;
            }
        }
        /// <summary>         /// A color with RGBA of (0.96, 0.96, 0.96, 1).         /// </summary>
        public static UIColor LightGrayBG
        {
            get
            {
                return _lightGrayBG;
            }
        }
        /// <summary>         /// A color with RGBA of (0.65, 0.65, 0.65, 1).         /// </summary>
        public static UIColor SilverChalice
        {
            get
            {
                return _silverChalice;
            }
        }
        /// <summary>         /// A color with RGBA of (0.89, 0.89, 0.89, 1).         /// </summary>
        public static UIColor PlatinumGrey
        {
            get
            {
                return _platinumGrey;
            }
        }
        /// <summary>         /// A color with RGBA of (0.89, 0.20, 0.10, 1).         /// </summary>
        public static UIColor Tomato
        {
            get
            {
                return _tomato;
            }
        }
        /// <summary>         /// A color with RGBA of (1, 0.80, 0.22, 1).         /// </summary>
        public static UIColor SunGlow
        {
            get
            {
                return _sunGlow;
            }
        }
        /// <summary>         /// A color with RGBA of (0.13, 0.74, 0.30, 1).         /// </summary>
        public static UIColor FreshGreen
        {
            get
            {
                return _freshGreen;
            }
        }
        /// <summary>         /// A color with RGBA of (0.11, 0.47, 0.79, 1).         /// </summary>
        public static UIColor PowerBlue
        {
            get
            {
                return _powerBlue;
            }
        }
        /// <summary>         /// A color with RGBA of (0.97, 0.97, 0.97, 1).         /// </summary>         public static UIColor SectionGrey
        {
            get
            {
                return _sectionGrey;
            }
        }
        /// <summary>         /// A color with RGBA of (0.90, 0.90, 0.90, 1).         /// </summary>         public static UIColor SelectionGrey
        {
            get
            {
                return _selectionGrey;
            }
        }
        /// <summary>         /// Selection semi transparent color.         /// </summary>         /// <returns>The semi transparent.</returns>         public static UIColor SelectionSemiTransparent
        {
            get
            {
                return _selectionSemiTransparent;
            }
        }
        /// <summary>
        /// Gray for lines
        /// </summary>
        /// <returns>The gray.</returns>
        public static UIColor LinesGray
        {
            get
            {
                return _linesGray;
            }
        }
        /// <summary>
        /// HarleyDavidsonOrange
        /// </summary>
        public static UIColor HarleyDavidsonOrange
        {
            get
            {
                return _harleyDavidsonOrange;
            }
        }
        /// <summary>
        /// Denim
        /// </summary>
        public static UIColor Denim
        {
            get
            {
                return _denim;
            }
        }
        /// <summary>
        /// Tunas the grey.
        /// </summary>
        /// <returns>The grey.</returns>
        /// <param name="customAlpha">Custom alpha.</param>
        public static UIColor TunaGrey(float customAlpha = 1.0f)
        {
            return new UIColor(red: 73.0f / 255.0f, green: 73.0f / 255.0f, blue: 74.0f / 255.0f, alpha: customAlpha);
        }
        /// <summary>
        /// A color with RGBA of (0.55, 0.55, 0.55, 1).
        /// </summary>
        public static UIColor Grey
        {
            get
            {
                return _grey;
            }
        }

        public static UIColor NeonBlue
        {
            get
            {
                return _neonBlue;
            }
        }

        public static UIColor BabyBlue
        {
            get
            {
                return _babyBlue;
            }
        }

        public static UIColor WaterBlue
        {
            get
            {
                return _waterBlue;
            }
        }

        public static UIColor ClearBlue
        {
            get
            {
                return _clearBlue;
            }
        }
    } }