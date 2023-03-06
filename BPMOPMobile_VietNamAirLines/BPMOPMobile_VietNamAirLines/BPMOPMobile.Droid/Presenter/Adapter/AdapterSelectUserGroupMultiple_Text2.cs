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
    public class AdapterSelectUserGroupMultiple_Text2 : RecyclerView.Adapter
    {
        private MainActivity _mainAct;
        private Context _context;
        private List<BeanUserAndGroup> _lstUser = new List<BeanUserAndGroup>();
        public event EventHandler<BeanUserAndGroup> CustomItemClick;
        private ControllerBase CTRLBase = new ControllerBase();
        public AdapterSelectUserGroupMultiple_Text2(MainActivity _mainAct, Context _context, List<BeanUserAndGroup> _lstUser)
        {
            this._mainAct = _mainAct;
            this._context = _context;
            this._lstUser = _lstUser;
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemChooseMultiUser_Text2, parent, false);
            AdapterSelectUserMultiple2_TextHolder holder = new AdapterSelectUserMultiple2_TextHolder(itemView, OnItemClick);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AdapterSelectUserMultiple2_TextHolder _holder = holder as AdapterSelectUserMultiple2_TextHolder;
            _holder._imgDelete.Visibility = ViewStates.Gone;
            _holder._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clWhite)));
            if (!String.IsNullOrEmpty(_lstUser[position].Name))
            {
                _holder._tvName.Text = _lstUser[position].Name;
                if (position != _lstUser.Count - 1)
                    _holder._tvName.Text += "; ";
            }
            else
            {
                _holder._tvName.Text = "";
            }
        }

    }
    public class AdapterSelectUserMultiple2_TextHolder : RecyclerView.ViewHolder
    {
        public LinearLayout _lnAll { get; set; }
        public TextView _tvName { get; set; }
        public ImageView _imgDelete { get; set; }
        public AdapterSelectUserMultiple2_TextHolder(View itemview, Action<int> listener) : base(itemview)
        {
            _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemChooseMultiUser_Text2_All);
            _tvName = itemview.FindViewById<TextView>(Resource.Id.tv_ItemChooseMultiUser_Text2_Name);
            _imgDelete = itemview.FindViewById<ImageView>(Resource.Id.tv_ItemChooseMultiUser_Text2_Delete);
            _imgDelete.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}