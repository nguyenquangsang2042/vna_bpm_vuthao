using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Core.Controller
{
    public class ControllerLeftMenu : ControllerBase
    {
        public void SetView_Selected(Activity _mainAct, TextView _tv, ImageView _img, TextView _tvCount = null)
        {
            try
            {
                if (_tv != null && _img != null)
                {
                    _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain)));
                    _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain)));
                    _tv.SetTypeface(_tv.Typeface, TypefaceStyle.Bold);
                    _img.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomEnable)));
                    if (_tvCount != null) // nếu có mới đổi màu
                    {
                        _tvCount.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clOrangeFilter)));
                        _tvCount.SetTypeface(_tvCount.Typeface, TypefaceStyle.Normal);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentLeftMenu", "SetView_Selected", ex);
#endif
            }
        }
        public void SetView_NotSelected(Activity _mainAct, TextView _tv, ImageView _img, TextView _tvCount = null)
        {
            try
            {
                if (_tv != null && _img != null)
                {
                    _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                    _tv.SetTypeface(null, TypefaceStyle.Normal);
                    _img.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                    if (_tvCount != null) // nếu có mới đổi màu
                    {
                        _tvCount.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain)));
                        _tvCount.SetTypeface(null, TypefaceStyle.Normal);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentLeftMenu", "SetView_NotSelected", ex);
#endif
            }
        }
    }
}