using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Core.Controller;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    public class AdapterSelectUserGroupMultiple_Text : RecyclerView.Adapter
    {
        private MainActivity _mainAct;
        private Context _context;
        private List<BeanUserAndGroup> _lstUser = new List<BeanUserAndGroup>();
        public event EventHandler<BeanUserAndGroup> CustomItemClick;
        public bool showDeleteButton;
        public AdapterSelectUserGroupMultiple_Text(MainActivity _mainAct, Context _context, List<BeanUserAndGroup> _lstUser, bool showDeleteButton = true)
        {
            this._mainAct = _mainAct;
            this._context = _context;
            this._lstUser = _lstUser;
            this.showDeleteButton = showDeleteButton;
        }
        private void OnItemClick(int position)
        {
            if (CustomItemClick != null)
            {
                CustomItemClick(this, _lstUser[position]);
            }
            //RemoveItemListIsClicked(_lstUser[position]);
            NotifyDataSetChanged();
        }
        public List<BeanUserAndGroup> GetListIsclicked()
        {
            if (_lstUser != null && _lstUser.Count > 0)
                return _lstUser;
            else
                return new List<BeanUserAndGroup>();
        }
        public void UpdateItemListIsClicked(List<BeanUserAndGroup> _lstUser)
        {
            this._lstUser = _lstUser;
        }
        public void RemoveItemListIsClicked(BeanUserAndGroup _user)
        {
            this._lstUser = _lstUser.Where(x => x.ID != _user.ID).ToList();
        }
        public override int ItemCount => _lstUser.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemChooseMultiUser_Text, parent, false);
            AdapterSelectUserMultiple_TextHolder holder = new AdapterSelectUserMultiple_TextHolder(itemView, OnItemClick);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AdapterSelectUserMultiple_TextHolder _holder = holder as AdapterSelectUserMultiple_TextHolder;
            if (!String.IsNullOrEmpty(_lstUser[position].Name))
                _holder._tvName.Text = _lstUser[position].Name;
            else
                _holder._tvName.Text = "";

            if (showDeleteButton)
                _holder._imgDelete.Visibility = ViewStates.Visible;
            else
                _holder._imgDelete.Visibility = ViewStates.Gone;
        }

    }
    public class AdapterSelectUserMultiple_TextHolder : RecyclerView.ViewHolder
    {
        public LinearLayout _lnAll { get; set; }
        public TextView _tvName { get; set; }
        public ImageView _imgDelete { get; set; }
        public AdapterSelectUserMultiple_TextHolder(View itemview, Action<int> listener) : base(itemview)
        {
            _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemChooseMultiUser_Text_All);
            _tvName = itemview.FindViewById<TextView>(Resource.Id.tv_ItemChooseMultiUser_Text_Name);
            _imgDelete = itemview.FindViewById<ImageView>(Resource.Id.tv_ItemChooseMultiUser_Text_Delete);
            _imgDelete.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}