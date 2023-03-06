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
    /// <summary>
    /// Rất quan trọng, không được chỉnh dưới mọi hình thức
    /// </summary>
    public enum EnumBottomNavigationView
    {
        // App Parent

        HomePage = 1,

        SingleListVDT = 2,

        SingleListVTBD = 3,

        Board = 4,
        Follow=11,

        // App Child

        ChildAppMore = -1, // trường hợp đặc biệt của More

        ChildAppHomePage = 5,

        ChildAppSingleListVDT = 6,

        ChildAppSingleListVTBD = 7,

        ChildAppKanban = 8, // Board DetailGroup

        ChildAppList = 9,

        ChildAppReport = 10,
    }
}