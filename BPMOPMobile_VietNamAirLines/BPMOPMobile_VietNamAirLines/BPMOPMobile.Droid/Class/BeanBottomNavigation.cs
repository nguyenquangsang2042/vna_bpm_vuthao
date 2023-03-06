using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BPMOPMobile.Droid.Class
{
    public class BeanBottomNavigation
    {
        public int FlagNavigation { get; set; }     // theo BPMOPMobile.Droid.Class.EnumBottomNavigationView
        public int DrawableID { get; set; }         // Resource.Drawable.ID
        public int TintColorID { get; set; }        // vị trí cứng của Item trong list nếu có
        public int? StableID { get; set; }          // vị trí cứng của Item trong list nếu có
        public string Title { get; set; }           // Title của button để hiện lên More
    }
}