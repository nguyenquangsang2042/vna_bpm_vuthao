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

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    class AdapterControlLinkedWorkflowDetailChoose : BaseAdapter<string>
    {
        private MainActivity _mainAct;
        private Context _context;
        private List<string> _lstData = new List<string>();
        private string _isChoosen;
        public AdapterControlLinkedWorkflowDetailChoose(Context context, List<string> _lstData, MainActivity mainAct, string _isChoosen)
        {
            this._context = context;
            this._lstData = _lstData;
            this._mainAct = mainAct;
            this._isChoosen = _isChoosen;
        }
        public override string this[int position] => _lstData[position];

        public override int Count => _lstData.Count;

        public override long GetItemId(int position)
        {
            return position;
        }
        public void UpdateIsChoosen(string _isChoosen)
        {
            this._isChoosen = _isChoosen;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            AdapterControlLinkedWorkflowDetailChooseHolder holder;
            convertView = LayoutInflater.From(_context).Inflate(Resource.Layout.ItemControlLinkedWorkflowDetailChoose, null);
            holder = new AdapterControlLinkedWorkflowDetailChooseHolder
            {
                _tvCategory = convertView.FindViewById<TextView>(Resource.Id.tv_ItemControlLinkedWorkflowDetailChoose),
                _imgIsChoosen = convertView.FindViewById<ImageView>(Resource.Id.img_ItemControlLinkedWorkflowDetailChoose),
            };

            if (_lstData[position].Equals(_isChoosen))
            {
                holder._tvCategory.SetTextColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clBottomEnable)));
                holder._imgIsChoosen.Visibility = ViewStates.Visible;
            }
            else
            {
                holder._tvCategory.SetTextColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clBlack)));
                holder._imgIsChoosen.Visibility = ViewStates.Invisible;
            }


            if (!String.IsNullOrEmpty(_lstData[position]))
            {
                holder._tvCategory.Text = _lstData[position];
            }
            else
            {
                holder._tvCategory.Text = "";
            }
            return convertView;
        }
        class AdapterControlLinkedWorkflowDetailChooseHolder : Java.Lang.Object
        {
            public TextView _tvCategory { get; set; }
            public ImageView _imgIsChoosen { get; set; }
        }
    }
}