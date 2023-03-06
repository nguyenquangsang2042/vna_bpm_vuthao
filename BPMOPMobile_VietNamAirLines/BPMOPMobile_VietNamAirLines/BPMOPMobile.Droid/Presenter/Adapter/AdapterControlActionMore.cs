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
    class AdapterControlActionMore : BaseAdapter<ButtonAction>
    {
        private Context _context;
        private List<ButtonAction> _lstAction;
        public AdapterControlActionMore(Context _context, List<ButtonAction> _lstAction)
        {
            this._context = _context;
            this._lstAction = _lstAction;
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
            LayoutInflater _inflater = LayoutInflater.From(_context);
            View rootView = _inflater.Inflate(Resource.Layout.ItemPopupAction, null);

            TextView _tvTitle = rootView.FindViewById<TextView>(Resource.Id.tv_ItemPopupAction);
            ImageView _img = rootView.FindViewById<ImageView>(Resource.Id.img_ItemPopupAction);
            View _viewLine = rootView.FindViewById<View>(Resource.Id.view_ItemPopupAction);

            if (position == _lstAction.Count - 1)
                _viewLine.Visibility = ViewStates.Gone;
            else
                _viewLine.Visibility = ViewStates.Visible;

            if (!String.IsNullOrEmpty(_lstAction[position].Title))
                _tvTitle.Text = _lstAction[position].Title;
            else
                _tvTitle.Text = "";

            string _strImageName = "icon_bpm_Btn_action_" + _lstAction[position].ID.ToString();
            int _drawableID = _context.Resources.GetIdentifier(_strImageName.ToLowerInvariant(), "drawable", _context.PackageName);
            _img.SetImageResource(_drawableID);

            return rootView;
        }
    }
}