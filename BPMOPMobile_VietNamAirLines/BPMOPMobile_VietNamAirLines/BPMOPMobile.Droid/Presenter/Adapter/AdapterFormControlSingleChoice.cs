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
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Core.Controller;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    class AdapterFormControlSingleChoice : RecyclerView.Adapter
    {
        private MainActivity _mainAct;
        private Context _context;
        private List<BeanLookupData> _lstItem = new List<BeanLookupData>();
        public event EventHandler<BeanLookupData> CustomItemClick;
        private ControllerBase CTRLBase = new ControllerBase();
        public AdapterFormControlSingleChoice(MainActivity _mainAct, Context _context, List<BeanLookupData> _lstItem)
        {
            this._mainAct = _mainAct;
            this._context = _context;
            this._lstItem = _lstItem;
        }
        private void OnItemClick(int position)
        {
            if (CustomItemClick != null)
            {
                CustomItemClick(this, _lstItem[position]);
            }
            //_IsClickedItem = _lstItem[position];
            NotifyDataSetChanged();
        }
        public override int ItemCount => _lstItem.Count;
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemFormControlChoice, parent, false);
            AdapterFormControlSingleChoiceHolder holder = new AdapterFormControlSingleChoiceHolder(itemView, OnItemClick);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AdapterFormControlSingleChoiceHolder _holder = holder as AdapterFormControlSingleChoiceHolder;

            if (_lstItem[position].IsSelected == true)
            {
                _holder._imgIsChecked.Visibility = ViewStates.Visible;
                _holder._tvTitle.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomEnable)));
            }
            else
            {
                _holder._imgIsChecked.Visibility = ViewStates.Gone;
                _holder._tvTitle.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
            }

            if (!String.IsNullOrEmpty(_lstItem[position].Title))
            {
                _holder._tvTitle.Text = _lstItem[position].Title;
            }
            else
            {
                _holder._tvTitle.Text = "";
            }
        }
    }
    public class AdapterFormControlSingleChoiceHolder : RecyclerView.ViewHolder
    {
        public LinearLayout _lnAll { get; set; }
        public TextView _tvTitle { get; set; }
        public ImageView _imgIsChecked { get; set; }
        public AdapterFormControlSingleChoiceHolder(View itemview, Action<int> listener) : base(itemview)
        {
            _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemFormControlChoice);
            _tvTitle = itemview.FindViewById<TextView>(Resource.Id.tv_ItemFormControlChoice_Title);
            _imgIsChecked = itemview.FindViewById<ImageView>(Resource.Id.img_ItemFormControlChoice_Check);
            _lnAll.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}