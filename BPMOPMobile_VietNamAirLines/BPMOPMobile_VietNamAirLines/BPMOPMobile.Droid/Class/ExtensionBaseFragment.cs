using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Core.Controller;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPMOPMobile.Droid.Class.Extension
{
    public static class ExtensionBaseFragment
    {
        public static MainActivity _mainAct;
        public static View _rootView;
        public static SQLiteConnection conn;
        public static ProviderBase _pBase;
        public static ProviderControlDynamic _pDynamic;
        public static ProviderUser _pUser;
        public static ControllerBase _cBase;
    }
}