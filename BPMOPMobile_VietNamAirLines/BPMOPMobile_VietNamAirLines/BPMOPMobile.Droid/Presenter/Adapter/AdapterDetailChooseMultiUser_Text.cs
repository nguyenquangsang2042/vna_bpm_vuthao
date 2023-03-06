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
    class AdapterDetailChooseMultiUser_Text : RecyclerView.Adapter
    {
        private MainActivity _mainAct;
        private Context _context;
        private List<BeanUser> _lstUser = new List<BeanUser>();
        public event EventHandler<BeanUser> CustomItemClick;
        private ControllerBase CTRLBase = new ControllerBase();
        public AdapterDetailChooseMultiUser_Text(MainActivity _mainAct, Context _context, List<BeanUser> _lstUser)
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

            RemoveItemListIsClicked(_lstUser[position]);
            NotifyDataSetChanged();
        }
        public void UpdateItemListIsClicked(List<BeanUser> _lstUser)
        {
            this._lstUser = _lstUser;
        }
        public void RemoveItemListIsClicked(BeanUser _user)
        {
            this._lstUser = _lstUser.Where(x => x.ID != _user.ID).ToList();
        }
        public override int ItemCount => _lstUser.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemChooseMultiUser_Text, parent, false);
            AdapterDetailChooseMultiUser_TextViewHolder holder = new AdapterDetailChooseMultiUser_TextViewHolder(itemView, OnItemClick);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AdapterDetailChooseMultiUser_TextViewHolder _holder = holder as AdapterDetailChooseMultiUser_TextViewHolder;
            if (!String.IsNullOrEmpty(_lstUser[position].FullName))
            {
                _holder._tvName.Text = _lstUser[position].FullName;
            }
            else
            {
                _holder._tvName.Text = "";
            }
        }
    }
    public class AdapterDetailChooseMultiUser_TextViewHolder : RecyclerView.ViewHolder
    {
        public LinearLayout _lnAll { get; set; }
        public TextView _tvName { get; set; }
        public ImageView _imgDelete { get; set; }
        public AdapterDetailChooseMultiUser_TextViewHolder(View itemview, Action<int> listener) : base(itemview)
        {
            _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemChooseMultiUser_Text_All);
            _tvName = itemview.FindViewById<TextView>(Resource.Id.tv_ItemChooseMultiUser_Text_Name);
            _imgDelete = itemview.FindViewById<ImageView>(Resource.Id.tv_ItemChooseMultiUser_Text_Delete);
            _imgDelete.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}