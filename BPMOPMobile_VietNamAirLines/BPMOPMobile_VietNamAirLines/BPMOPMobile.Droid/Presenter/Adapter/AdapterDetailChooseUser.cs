using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
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
    public class AdapterDetailChooseUser : RecyclerView.Adapter
    {
        private MainActivity _mainAct;
        private Context _context;
        private List<BeanUser> _lstUser = new List<BeanUser>();
        private BeanUser _IsClickedUser = new BeanUser();
        public event EventHandler<BeanUser> CustomItemClick;
        private ControllerBase CTRLBase = new ControllerBase();
        public AdapterDetailChooseUser(MainActivity _mainAct, Context _context, List<BeanUser> _lstUser, BeanUser _IsClickedUser)
        {
            this._mainAct = _mainAct;
            this._context = _context;
            this._lstUser = _lstUser;
            this._IsClickedUser = _IsClickedUser;
        }
        private void OnItemClick(int position)
        {
            if (CustomItemClick != null)
            {
                CustomItemClick(this, _lstUser[position]);
            }
            _IsClickedUser = _lstUser[position];
            NotifyDataSetChanged();
        }
        public BeanUser GetUserIsClicked()
        {
            return _IsClickedUser;
        }
        public override int ItemCount => _lstUser.Count;
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemChooseMultiUser, parent, false);
            AdapterDetailChooseUserViewHolder holder = new AdapterDetailChooseUserViewHolder(itemView, OnItemClick);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AdapterDetailChooseUserViewHolder _holder = holder as AdapterDetailChooseUserViewHolder;

            if (_IsClickedUser != null && !String.IsNullOrEmpty(_IsClickedUser.ID))
            {
                if (_IsClickedUser.ID.Equals(_lstUser[position].ID))
                {
                    _holder._viewIsSelected.Visibility = ViewStates.Visible;
                }
                else
                {
                    _holder._viewIsSelected.Visibility = ViewStates.Invisible;
                }
            }
            else
            {
                _holder._viewIsSelected.Visibility = ViewStates.Invisible;
            }

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

            if (!String.IsNullOrEmpty(_lstUser[position].FullName))
            {
                _holder._tvTitle.Text = _lstUser[position].FullName;
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
    public class AdapterDetailChooseUserViewHolder : RecyclerView.ViewHolder
    {
        public LinearLayout _lnAll { get; set; }
        public View _viewIsSelected { get; set; }
        public TextView _tvAvatar { get; set; }
        public CircleImageView _imgAvatar { get; set; }
        public TextView _tvTitle { get; set; }
        public TextView _tvEmail { get; set; }
        public AdapterDetailChooseUserViewHolder(View itemview, Action<int> listener) : base(itemview)
        {
            _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemChooseMultiUser_All);
            _viewIsSelected = itemview.FindViewById<View>(Resource.Id.view_ItemChooseMultiUser_LeftSelected);
            _tvAvatar = itemview.FindViewById<TextView>(Resource.Id.tv_ItemChooseMultiUser_Avatar);
            _imgAvatar = itemview.FindViewById<CircleImageView>(Resource.Id.img_ItemChooseMultiUser_Avatar);
            _tvTitle = itemview.FindViewById<TextView>(Resource.Id.tv_ItemChooseMultiUser_Title);
            _tvEmail = itemview.FindViewById<TextView>(Resource.Id.tv_ItemChooseMultiUser_Email);

            _lnAll.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}