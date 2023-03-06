using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using Refractored.Controls;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    class AdapterSelectUserSingle : RecyclerView.Adapter
    {
        private MainActivity _mainAct;
        private Context _context;
        private List<BeanUserAndGroup> _lstUser = new List<BeanUserAndGroup>();
        private BeanUserAndGroup _clickedUser = new BeanUserAndGroup();
        public event EventHandler<BeanUserAndGroup> CustomItemClick;
        private ControllerBase CTRLBase = new ControllerBase();
        public AdapterSelectUserSingle(MainActivity _mainAct, Context _context, List<BeanUserAndGroup> _lstUser, BeanUserAndGroup _clickedUser)
        {
            this._mainAct = _mainAct;
            this._context = _context;
            this._lstUser = _lstUser;
            this._clickedUser = _clickedUser;
        }
        private void OnItemClick(int position)
        {
            if (CustomItemClick != null)
            {
                CustomItemClick(this, _lstUser[position]);
            }
        }
        public override int ItemCount => _lstUser.Count;
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemSelectUserAndGroup, parent, false);
            AdapterAdapterSelectUserSingleViewHolder holder = new AdapterAdapterSelectUserSingleViewHolder(itemView, OnItemClick);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AdapterAdapterSelectUserSingleViewHolder _holder = holder as AdapterAdapterSelectUserSingleViewHolder;

            _holder._viewIsSelected.Visibility = ViewStates.Invisible;

            if (position % 2 == 0) // tô màu so le
            {
                _holder._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clGraySearchUser)));
            }
            else
            {
                _holder._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clWhite)));
            }

            if (_lstUser[position].Type == 1) // 0 là User - 1 là Group
            {
                _holder._tvTitle.SetMaxLines(2);
                _holder._tvEmail.Visibility = ViewStates.Gone;
                _holder._imgAvatar.Visibility = ViewStates.Visible;
                _holder._tvAvatar.Visibility = ViewStates.Gone;

                _holder._imgAvatar.SetImageResource(Resource.Drawable.icon_ver2_group);
            }
            else
            {
                _holder._tvTitle.SetMaxLines(1);
                _holder._tvEmail.Visibility = ViewStates.Visible;

                if (!String.IsNullOrEmpty(_lstUser[position].ImagePath))
                {
                    _holder._imgAvatar.Visibility = ViewStates.Visible;
                    _holder._tvAvatar.Visibility = ViewStates.Gone;

                    CmmDroidFunction.SetAvataByImagePath(_mainAct, _lstUser[position].ImagePath, _holder._imgAvatar, _holder._tvAvatar, 50);
                }
                else
                {
                    _holder._imgAvatar.Visibility = ViewStates.Gone;
                    _holder._tvAvatar.Visibility = ViewStates.Visible;

                    _holder._tvAvatar.Text = CmmFunction.GetAvatarName(_lstUser[position].AccountName);
                    _holder._tvAvatar.BackgroundTintList = ColorStateList.ValueOf(CTRLBase.GetColorByUserName(_context, _lstUser[position].AccountName));
                }
                //_holder._tvAvatar.Text = CmmFunction.GetAvatarName(_lstUser[position].Name);
                //_holder._tvAvatar.BackgroundTintList = ColorStateList.ValueOf(CTRLBase.GetColorByUserName(_context, _lstUser[position].Name));

            }

            if (!String.IsNullOrEmpty(_lstUser[position].Name))
            {
                _holder._tvTitle.Text = _lstUser[position].Name;
            }
            else
            {
                _holder._tvTitle.Text = "";
            }

            if (!String.IsNullOrEmpty(_lstUser[position].Email))
            {
                _holder._tvEmail.Text = _lstUser[position].Email;
            }
            else
            {
                _holder._tvEmail.Text = "";
            }

        }
    }
    public class AdapterAdapterSelectUserSingleViewHolder : RecyclerView.ViewHolder
    {
        public LinearLayout _lnAll { get; set; }
        public View _viewIsSelected { get; set; }
        public TextView _tvAvatar { get; set; }
        public CircleImageView _imgAvatar { get; set; }
        public TextView _tvTitle { get; set; }
        public TextView _tvEmail { get; set; }
        public AdapterAdapterSelectUserSingleViewHolder(View itemview, Action<int> listener) : base(itemview)
        {
            _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemSelectUserAndGroup_All);
            _viewIsSelected = itemview.FindViewById<View>(Resource.Id.view_ItemSelectUserAndGroup_LeftSelected);
            _tvAvatar = itemview.FindViewById<TextView>(Resource.Id.tv_ItemSelectUserAndGroup_Avatar);
            _imgAvatar = itemview.FindViewById<CircleImageView>(Resource.Id.img_ItemSelectUserAndGroup_Avatar);
            _tvTitle = itemview.FindViewById<TextView>(Resource.Id.tv_ItemSelectUserAndGroup_Title);
            _tvEmail = itemview.FindViewById<TextView>(Resource.Id.tv_ItemSelectUserAndGroup_Email);
            _lnAll.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}
