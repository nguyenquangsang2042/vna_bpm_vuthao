using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Droid.Class;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    public class SharedViewBase
    {
        public AppCompatActivity _mainAct { get; set; }
        public LayoutInflater _inflater{ get; set; }
        public CustomBaseFragment _fragment { get; set; }
        public string _fragmentTag { get; set; }
        public View _rootView { get; set; }

        public SharedViewBase(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView)
        {
            this._inflater = _inflater;
            this._mainAct = _mainAct;
            this._fragment = _fragment;
            this._fragmentTag = _fragmentTag;
            this._rootView = _rootView;
        }
        public virtual void InitializeView()
        {
            // khởi tạo view tùy vào Control
        }

    }
}