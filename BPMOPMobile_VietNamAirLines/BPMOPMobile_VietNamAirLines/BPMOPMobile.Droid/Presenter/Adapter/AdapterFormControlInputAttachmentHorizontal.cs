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
    class AdapterFormControlInputAttachmentHorizontal : RecyclerView.Adapter
    {
        private MainActivity _mainAct;
        private Context _context;
        private List<BeanAttachFileCategory> _lstItem = new List<BeanAttachFileCategory>();
        private BeanLookupData _IsClickedItem = new BeanLookupData();
        public event EventHandler<BeanAttachFileCategory> CustomItemClick;
        private ControllerBase CTRLBase = new ControllerBase();
        public AdapterFormControlInputAttachmentHorizontal(MainActivity _mainAct, Context _context, List<BeanAttachFileCategory> _lstItem)
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
            _lstItem[position].IsSelected = true;
            NotifyDataSetChanged();
        }
        public BeanLookupData GetUserIsClicked()
        {
            return _IsClickedItem;
        }
        public override int ItemCount => _lstItem.Count;
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemFormControlChoice, parent, false);
            AdapterFormControlInputAttachmentHorizontalHolder holder = new AdapterFormControlInputAttachmentHorizontalHolder(itemView, OnItemClick);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AdapterFormControlInputAttachmentHorizontalHolder _holder = holder as AdapterFormControlInputAttachmentHorizontalHolder;

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
    public class AdapterFormControlInputAttachmentHorizontalHolder : RecyclerView.ViewHolder
    {
        public LinearLayout _lnAll { get; set; }
        public TextView _tvTitle { get; set; }
        public ImageView _imgIsChecked { get; set; }
        public AdapterFormControlInputAttachmentHorizontalHolder(View itemview, Action<int> listener) : base(itemview)
        {
            _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemFormControlChoice);
            _tvTitle = itemview.FindViewById<TextView>(Resource.Id.tv_ItemFormControlChoice_Title);
            _imgIsChecked = itemview.FindViewById<ImageView>(Resource.Id.img_ItemFormControlChoice_Check);
            _lnAll.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}