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

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    class AdapterDetailControlAttachmentAddCategory : BaseAdapter<string>
    {
        private MainActivity _mainAct;
        private Context context;
        private List<string> _lstData = new List<string>(); // flag này để lưu lại trạng thái item[n-1] để xem có hiện category item[n] không
        public AdapterDetailControlAttachmentAddCategory(Context context, List<string> _lstData, MainActivity mainAct)
        {
            this.context = context;
            this._lstData = _lstData;
            this._mainAct = mainAct;
        }
        public override string this[int position] => _lstData[position];

        public override int Count => _lstData.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            AdapterDetailControlAttachmentAddCategoryHolder holder;
            convertView = LayoutInflater.From(context).Inflate(Resource.Layout.ItemPopupControlAttachmentAddCategory, null);
            holder = new AdapterDetailControlAttachmentAddCategoryHolder
            {
                _tvCategory = convertView.FindViewById<TextView>(Resource.Id.tv_ItemPopupControlAttachmentAddCategory),
            };
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
        class AdapterDetailControlAttachmentAddCategoryHolder : Java.Lang.Object
        {
            public TextView _tvCategory { get; set; }
        }
    }
}