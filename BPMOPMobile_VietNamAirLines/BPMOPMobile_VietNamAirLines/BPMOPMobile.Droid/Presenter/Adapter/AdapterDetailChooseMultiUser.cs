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
    public class AdapterDetailChooseMultiUser : RecyclerView.Adapter
    {
        private MainActivity _mainAct;
        private Context _context;
        private List<BeanUser> _lstUser = new List<BeanUser>();
        private List<BeanUser> _lstUser_IsClicked = new List<BeanUser>();
        public event EventHandler<BeanUser> CustomItemClick;
        private ControllerBase CTRLBase = new ControllerBase();
        public AdapterDetailChooseMultiUser(MainActivity _mainAct, Context _context, List<BeanUser> _lstUser, List<BeanUser> _lstUser_IsClicked)
        {
            this._mainAct = _mainAct;
            this._context = _context;
            this._lstUser = _lstUser;
            this._lstUser_IsClicked = _lstUser_IsClicked;
        }
        private void OnItemClick(int position)
        {
            if (CustomItemClick != null)
            {
                CustomItemClick(this, _lstUser[position]);
            }

            if (_lstUser_IsClicked.Where(x => x.ID.Equals(_lstUser[position].ID)).ToList().Count > 0) // Đã có trong List
            {
                RemoveItemListIsClicked(_lstUser[position]);
            }
            else // chưa có -> Add vào
            {
                AddItemListIsClicked(_lstUser[position]);
            }
            NotifyDataSetChanged();
        }
        public List<BeanUser> GetListIsClicked()
        {
            return _lstUser_IsClicked;
        }
        public void UpdateListIsClicked(List<BeanUser> _lstUser_IsClicked)
        {
            this._lstUser_IsClicked = _lstUser_IsClicked;
        }
        public void AddItemListIsClicked(BeanUser _user)
        {
            this._lstUser_IsClicked.Add(_user);
        }
        public void RemoveItemListIsClicked(BeanUser _user)
        {
            this._lstUser_IsClicked = _lstUser_IsClicked.Where(x => x.ID != _user.ID).ToList();
        }
        public override int ItemCount => _lstUser.Count;
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemChooseMultiUser, parent, false);
            AdapterDetailChooseMultiUserViewHolder holder = new AdapterDetailChooseMultiUserViewHolder(itemView, OnItemClick);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AdapterDetailChooseMultiUserViewHolder _holder = holder as AdapterDetailChooseMultiUserViewHolder;

            if (_lstUser_IsClicked != null && _lstUser_IsClicked.Count > 0)
            {
                if (_lstUser_IsClicked.Where(x => x.ID.Equals(_lstUser[position].ID)).ToList().Count > 0)
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

            if (!String.IsNullOrEmpty(_lstUser[position].FullName))
            {
                ////SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                ////string queryNotify = string.Format("SELECT * FROM BeanUser WHERE AccountName = '{0}'", _lstUser[position].AccountName);
                ////List<BeanUser> lstUser = conn.Query<BeanUser>(queryNotify);
                ////if (lstUser != null && lstUser.Count > 0)
                ////{
                ////    _holder._imgAvatar.Visibility = ViewStates.Visible;
                ////    _holder._tvAvatar.Visibility = ViewStates.Gone;

                ////    CmmDroidFunction.SetAvataByImagePath(_mainAct, lstUser[0].ImagePath, _holder._imgAvatar);
                ////}
                ////else
                ////{
                _holder._imgAvatar.Visibility = ViewStates.Gone;
                _holder._tvAvatar.Visibility = ViewStates.Visible;

                _holder._tvAvatar.Text = CmmFunction.GetAvatarName(_lstUser[position].FullName);
                _holder._tvAvatar.BackgroundTintList = ColorStateList.ValueOf(CTRLBase.GetColorByUserName(_context, _lstUser[position].FullName));
                ////}
            }
            else
            {
                _holder._imgAvatar.Visibility = ViewStates.Gone;
                _holder._tvAvatar.Visibility = ViewStates.Visible;
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
    public class AdapterDetailChooseMultiUserViewHolder : RecyclerView.ViewHolder
    {
        public LinearLayout _lnAll { get; set; }
        public View _viewIsSelected { get; set; }
        public TextView _tvAvatar { get; set; }
        public CircleImageView _imgAvatar { get; set; }
        public TextView _tvTitle { get; set; }
        public TextView _tvEmail { get; set; }
        public AdapterDetailChooseMultiUserViewHolder(View itemview, Action<int> listener) : base(itemview)
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