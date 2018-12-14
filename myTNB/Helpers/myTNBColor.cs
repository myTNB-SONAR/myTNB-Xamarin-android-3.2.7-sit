using UIKit;  namespace myTNB {
    public static class myTNBColor
    {
        /// <summary>         /// A color with RGBA of (0.47, 0.19, 0.83, 1).         /// </summary>         public static UIColor GradientPurpleDarkElement()
        {
            return new UIColor(red: 0.47f, green: 0.19f, blue: 0.83f, alpha: 1.0f);
        }
        /// <summary>         /// A color with RGBA of (0.16, 0.59, 0.98, 1).         /// </summary>
		public static UIColor GradientPurpleLightElement()
        {
            return new UIColor(red: 0.16f, green: 0.59f, blue: 0.98f, alpha: 1.0f);
        }
        /// <summary>         /// A color with RGBA of (255, 255, 255, 0.6).         /// </summary>
		public static UIColor LightGray()
        {
            return UIColor.FromRGBA(255f, 255f, 255f, 0.6f);
        }
        /// <summary>         /// A color with RGBA of (0.96, 0.96, 0.96, 1).         /// </summary>
		public static UIColor LightGrayBG()
        {
            return new UIColor(red: 0.96f, green: 0.96f, blue: 0.96f, alpha: 1.0f);
        }         /// <summary>         /// A color with RGBA of (0.65, 0.65, 0.65, 1).         /// </summary>
		public static UIColor SilverChalice()
        {
            return new UIColor(red: 0.65f, green: 0.65f, blue: 0.65f, alpha: 1.0f);
        }
        /// <summary>         /// A color with RGBA of (0.89, 0.89, 0.89, 1).         /// </summary>
		public static UIColor PlatinumGrey()
        {
            return new UIColor(red: 0.89f, green: 0.89f, blue: 0.89f, alpha: 1.0f);
        }
        /// <summary>         /// A color with RGBA of (0.89, 0.20, 0.10, 1).         /// </summary>
		public static UIColor Tomato()
        {
            return new UIColor(red: 0.89f, green: 0.20f, blue: 0.10f, alpha: 1.0f);
        }         /// <summary>         /// A color with RGBA of (1, 0.80, 0.22, 1).         /// </summary>
		public static UIColor SunGlow()
        {
            return new UIColor(red: 1.0f, green: 0.80f, blue: 0.22f, alpha: 1.0f);
        }
        /// <summary>         /// A color with RGBA of (0.13, 0.74, 0.30, 1).         /// </summary>
		public static UIColor FreshGreen()
        {
            return new UIColor(red: 0.13f, green: 0.74f, blue: 0.30f, alpha: 1.0f);
        }
        /// <summary>         /// A color with RGBA of (0.29, 0.29, 0.29, 1).         /// </summary>
		public static UIColor TunaGrey()
        {
            return new UIColor(red: 0.29f, green: 0.29f, blue: 0.29f, alpha: 1.0f);
        }
        /// <summary>         /// A color with RGBA of (0.11, 0.47, 0.79, 1).         /// </summary>
		public static UIColor PowerBlue()
        {
            return new UIColor(red: 0.11f, green: 0.47f, blue: 0.79f, alpha: 1.0f);
        }
        /// <summary>         /// A color with RGBA of (0.97, 0.97, 0.97, 1).         /// </summary>         public static UIColor SectionGrey()         {             return new UIColor(red: 0.97f, green: 0.97f, blue: 0.97f, alpha: 1.0f);         }
        /// <summary>         /// A color with RGBA of (0.90, 0.90, 0.90, 1).         /// </summary>         public static UIColor SelectionGrey()         {             return new UIColor(red: 0.90f, green: 0.90f, blue: 0.90f, alpha: 1.0f);         }         /// <summary>         /// Selection semi transparent color.         /// </summary>         /// <returns>The semi transparent.</returns>         public static UIColor SelectionSemiTransparent()         {             return UIColor.FromRGBA(255, 255, 255, 60);         }
    } }