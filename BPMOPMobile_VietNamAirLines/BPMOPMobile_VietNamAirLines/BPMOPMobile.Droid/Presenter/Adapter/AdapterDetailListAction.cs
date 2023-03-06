using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    class AdapterDetailListAction : BaseAdapter<ButtonAction>
    {
        private Context context;
        private List<ButtonAction> _lstAction;
        public AdapterDetailListAction(Context context, List<ButtonAction> lstAction)
        {
            this.context = context;
            _lstAction = lstAction;
        }

        public override ButtonAction this[int position]
        {
            get
            {
                return _lstAction[position];
            }
        }

        public override int Count
        {
            get
            {
                return _lstAction.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View rootView = new View(context);
            if (position == 0 || position == 1) // bỏ 2 item đầu
            {

            }
            else
            {
                LayoutInflater mInflater = LayoutInflater.From(context);
                rootView = mInflater.Inflate(Resource.Layout.ItemPopupAction, null);
                TextView tvDate = rootView.FindViewById<TextView>(Resource.Id.tv_ItemPopupAction);
                ImageView img = rootView.FindViewById<ImageView>(Resource.Id.img_ItemPopupAction);
                View view = rootView.FindViewById<View>(Resource.Id.view_ItemPopupAction);
                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {                    
                    tvDate.Text = _lstAction[position].Title;
                }
                else
                {
                    tvDate.Text = _lstAction[position].Value;
                }
                string nameOfImage = "icon_Btn_action_new_" + _lstAction[position].ID.ToString();
                int resId = context.Resources.GetIdentifier(nameOfImage.ToLowerInvariant(), "mipmap", context.PackageName);
                img.SetImageResource(resId);
                ImageViewCompat.SetImageTintList(img, Android.Content.Res.ColorStateList.ValueOf(Color.ParseColor("#" + _lstAction[position].Color)));
                if (position == _lstAction.Count - 1)
                {
                    view.Visibility = ViewStates.Gone;
                }
                else
                {
                    view.Visibility = ViewStates.Visible;
                }
            }
            return rootView;
        }
    }
}