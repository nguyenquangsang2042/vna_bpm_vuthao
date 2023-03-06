using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using Refractored.Controls;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    class AdapterExpandActionShare_History : BaseExpandableListAdapter
    {
        private MainActivity _mainAct;
        private Context _context;
        private List<BeanGroupShareHistory> _lstGroupShareHistory = new List<BeanGroupShareHistory>();
        private ControllerBase CTRLBase = new ControllerBase();
        public AdapterExpandActionShare_History(MainActivity _mainAct, Context _context, List<BeanGroupShareHistory> _lstGroupShareHistory)
        {
            this._mainAct = _mainAct;
            this._context = _context;
            this._lstGroupShareHistory = _lstGroupShareHistory;
        }
        public override int GroupCount => _lstGroupShareHistory.Count;

        public override bool HasStableIds => false;

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            if (_lstGroupShareHistory[groupPosition].listChild != null && _lstGroupShareHistory[groupPosition].listChild.Count > 0)
                return _lstGroupShareHistory[groupPosition].listChild.Count;
            return 0;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            LayoutInflater mInflater = LayoutInflater.From(_context);         
            View rootView = mInflater.Inflate(Resource.Layout.ItemShareHistory_Child, null);
            LinearLayout _lnAll = rootView.FindViewById<LinearLayout>(Resource.Id.ln_ItemShareHistory_Child_All);
            TextView _tvAvatar = rootView.FindViewById<TextView>(Resource.Id.tv_ItemShareHistory_Child_Avatar);
            CircleImageView _imgAvatar = rootView.FindViewById<CircleImageView>(Resource.Id.img_ItemShareHistory_Child_Avatar);
            TextView _tvTitle = rootView.FindViewById<TextView>(Resource.Id.tv_ItemShareHistory_Child_Title);
            TextView _tvEmail = rootView.FindViewById<TextView>(Resource.Id.tv_ItemShareHistory_Child_Email);

            BeanShareHistory _currentItemHistory = _lstGroupShareHistory[groupPosition].listChild[childPosition];

            if (groupPosition % 2 == 0)
            {
                _lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clVer2BlueNavigation)));
            }
            else
            {
                _lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clWhite)));
            }

            if (!String.IsNullOrEmpty(_currentItemHistory.UserImagePath))
            {
                _imgAvatar.Visibility = ViewStates.Visible;
                _tvAvatar.Visibility = ViewStates.Gone;
                SetAvatarByImagePath(_tvAvatar, _imgAvatar, _currentItemHistory.UserImagePath);
            }

            if (!String.IsNullOrEmpty(_currentItemHistory.UserName))
            {
                _tvTitle.Text = _currentItemHistory.UserName;
                _tvAvatar.Text = CmmFunction.GetAvatarName(_currentItemHistory.UserName);
                _tvAvatar.BackgroundTintList = ColorStateList.ValueOf(CTRLBase.GetColorByUserName(_context, _currentItemHistory.UserName));
            }
            else
                _tvTitle.Text = "";

            if (!String.IsNullOrEmpty(_currentItemHistory.UserPosition))
                _tvEmail.Text = _currentItemHistory.UserPosition;
            else
                _tvEmail.Text = "";


            return rootView;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return groupPosition;
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            LayoutInflater mInflater = LayoutInflater.From(_context);
            View rootView = mInflater.Inflate(Resource.Layout.ItemShareHistory_Group, null);

            LinearLayout _lnAll = rootView.FindViewById<LinearLayout>(Resource.Id.ln_ItemShareHistory_Group_All);
            TextView _tvAvatar = rootView.FindViewById<TextView>(Resource.Id.tv_ItemShareHistory_Group_Avatar);
            CircleImageView _imgAvatar = rootView.FindViewById<CircleImageView>(Resource.Id.img_ItemShareHistory_Group_Avatar);
            TextView _tvTitle = rootView.FindViewById<TextView>(Resource.Id.tv_ItemShareHistory_Group_Title);
            TextView _tvTime = rootView.FindViewById<TextView>(Resource.Id.tv_ItemShareHistory_Group_Time);
            TextView _tvEmail = rootView.FindViewById<TextView>(Resource.Id.tv_ItemShareHistory_Group_Email);
            TextView _tvComment = rootView.FindViewById<TextView>(Resource.Id.tv_ItemShareHistory_Group_Comment); 

            BeanShareHistory _currentItemHistory = _lstGroupShareHistory[groupPosition].parentItem;

            if (groupPosition % 2 == 0)
            {
                _lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clVer2BlueNavigation)));
            }
            else
            {
                _lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clWhite)));
            }

            if (!String.IsNullOrEmpty(_currentItemHistory.UserImagePath))
            {
                _imgAvatar.Visibility = ViewStates.Visible;
                _tvAvatar.Visibility = ViewStates.Gone;
                SetAvatarByImagePath(_tvAvatar, _imgAvatar, _currentItemHistory.UserImagePath);
            }

            if (!String.IsNullOrEmpty(_currentItemHistory.UserName))
            {
                _tvTitle.Text = _currentItemHistory.UserName;
                _tvAvatar.Text = CmmFunction.GetAvatarName(_currentItemHistory.UserName);
                _tvAvatar.BackgroundTintList = ColorStateList.ValueOf(CTRLBase.GetColorByUserName(_context, _currentItemHistory.UserName));
            }
            else
                _tvTitle.Text = "";

            if (_currentItemHistory.DateShared != null)
                _tvTime.Text = CTRLBase.GetFormatDateLang(_currentItemHistory.DateShared);
            else
                _tvTime.Text = "";

            if (!String.IsNullOrEmpty(_currentItemHistory.UserPosition))
                _tvEmail.Text = _currentItemHistory.UserPosition;
            else
                _tvEmail.Text = "";

            if (!String.IsNullOrEmpty(_currentItemHistory.Comment))
            {
                _tvComment.Visibility = ViewStates.Visible;
                _tvComment.Text = _currentItemHistory.Comment;
            }
            else
                _tvComment.Visibility = ViewStates.Gone;



            return rootView;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }

        private async void SetAvatarByImagePath(TextView _tv, ImageView _img, string UserImagePath)
        {
            try
            {
                string imgFilePath = System.IO.Path.Combine(CmmVariable.M_Folder_Avatar + "/", System.IO.Path.GetFileName(UserImagePath) ?? throw new InvalidOperationException());
                string url = CmmVariable.M_Domain + UserImagePath;
                ProviderBase pUser = new ProviderBase();
                bool result;
                if (!File.Exists(imgFilePath))
                {
                    await Task.Run(() =>
                    {
                        result = pUser.DownloadFile(url, imgFilePath, CmmVariable.M_AuthenticatedHttpClient);
                        if (result)
                        {
                            Bitmap myBitmap = BitmapHelper.LoadAndResizeBitmap(imgFilePath, 90, 90);
                            if (myBitmap != null)
                            {
                                _mainAct.RunOnUiThread(() =>
                                {
                                    _img.SetImageBitmap(myBitmap);
                                });
                            }
                            else
                            {
                                _mainAct.RunOnUiThread(() =>
                                {
                                    _img.Visibility = ViewStates.Gone;
                                    _tv.Visibility = ViewStates.Visible;
                                });
                            }
                        }
                    });
                }
                else
                {
                    Bitmap myBitmap = BitmapHelper.LoadAndResizeBitmap(imgFilePath, 90, 90);
                    if (myBitmap != null)
                    {
                        _img.SetImageBitmap(myBitmap);
                    }
                    else
                    {
                        _img.Visibility = ViewStates.Gone;
                        _tv.Visibility = ViewStates.Visible;
                    }
                }
            }
            catch
            {

            }
        }
    }
}