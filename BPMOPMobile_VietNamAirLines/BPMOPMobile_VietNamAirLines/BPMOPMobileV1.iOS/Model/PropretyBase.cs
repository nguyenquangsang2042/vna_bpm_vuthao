using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace BPMOPMobileV1.Model
{
    public class PropretyBase
    {
        public UIFont GetUIFont(string value)
        {
            return UIFont.SystemFontOfSize(Convert.ToInt32(value), UIFontWeight.Regular);
        }

        public UIColor GetUIColor(string value)
        {
            return ExtensionMethods.ToUIColor(value);
        }

        public nint Getnint(string value)
        {
            return new nint(Convert.ToInt32(value));
        }

        public UITextAlignment GetUITextAlignment(string value)
        {
            switch (value.ToLower())
            {
                case "left":
                    return UITextAlignment.Left;
                case "right":
                    return UITextAlignment.Right;
                case "center":
                    return UITextAlignment.Center;
                case "justified":
                    return UITextAlignment.Justified;
                case "natural":
                    return UITextAlignment.Natural;
                default:
                    return UITextAlignment.Left;
            }
        }

        public UILineBreakMode GetUILineBreakMode(string value)
        {
            switch (value.ToLower())
            {
                case "characterWrap":
                    return UILineBreakMode.CharacterWrap;
                case "clip":
                    return UILineBreakMode.Clip;
                case "headtruncation":
                    return UILineBreakMode.HeadTruncation;
                case "middletruncation":
                    return UILineBreakMode.MiddleTruncation;
                case "tailtruncation":
                    return UILineBreakMode.TailTruncation;
                case "wordwrap":
                    return UILineBreakMode.WordWrap;
                default:
                    return UILineBreakMode.CharacterWrap;
            }
        }

        public UIViewContentMode GetUIViewContentMode(string value)
        {
            switch (value.ToLower())
            {
                case "left":
                    return UIViewContentMode.Left;
                case "right":
                    return UIViewContentMode.Right;
                case "top":
                    return UIViewContentMode.Top;
                case "bottom":
                    return UIViewContentMode.Bottom;
                case "center":
                    return UIViewContentMode.Center;
                case "scaleaspectfill":
                    return UIViewContentMode.ScaleAspectFill;
                case "scaleaspectfit":
                    return UIViewContentMode.ScaleAspectFit;
                case "scaletofill":
                    return UIViewContentMode.ScaleToFill;
                default:
                    return UIViewContentMode.ScaleAspectFill;
            }
        }
    }

    public static class ExtensionMethods
    {
        public static UIColor ToUIColor(this string hexString)
        {
            hexString = hexString.Replace("#", "");

            if (hexString.Length == 3)
                hexString = hexString + hexString;

            if (hexString.Length != 6)
                throw new Exception("Invalid hex string");

            int red = Int32.Parse(hexString.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            int green = Int32.Parse(hexString.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            int blue = Int32.Parse(hexString.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);

            return UIColor.FromRGB(red, green, blue);
        }
    }
}