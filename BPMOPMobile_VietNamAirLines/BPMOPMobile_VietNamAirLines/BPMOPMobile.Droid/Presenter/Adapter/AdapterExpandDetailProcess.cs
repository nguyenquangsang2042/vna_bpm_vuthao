using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
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
    public class AdapterExpandDetailProcess : BaseExpandableListAdapter
    {
        MainActivity _mainAct;
        private Context _context;
        private List<BeanStepQTLC> _lstStepQTLC = new List<BeanStepQTLC>();
        private ControllerDetailProcess CTRLDetailProcess = new ControllerDetailProcess();
        public AdapterExpandDetailProcess(MainActivity _mainAct, Context _context, List<BeanStepQTLC> _lstStepQTLC)
        {
            this._mainAct = _mainAct;
            this._context = _context;
            this._lstStepQTLC = _lstStepQTLC;
        }

        public override int GroupCount
        {
            get
            {
                return _lstStepQTLC.Count;
            }
        }
        public override bool HasStableIds
        {
            get
            {
                return false;
            }
        }
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
            return _lstStepQTLC[groupPosition].ListStepQTLC != null ? _lstStepQTLC[groupPosition].ListStepQTLC.Count : 0;
        }
        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            BeanQuaTrinhLuanChuyen _currentItemQTLC = _lstStepQTLC[groupPosition].ListStepQTLC[childPosition];

            if (_currentItemQTLC.Count == 0)// Root View
            {
                LayoutInflater mInflater = LayoutInflater.From(_context);
                View rootView = mInflater.Inflate(Resource.Layout.ItemExpandDetaiProcessChild, null);
                TextView _tvDate = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Date);
                TextView _tvYKien = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_YKien);
                TextView _tvName = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Name);
                TextView _tvPosition = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Position);
                TextView _tvAction = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Action);
                View _vwGroupLine = rootView.FindViewById<View>(Resource.Id.vw_ItemExpandDetaiProcessChild_GroupLine);
                View _vwChildLine = rootView.FindViewById<View>(Resource.Id.vw_ItemExpandDetaiProcessChild_ChildLine);
                TextView _tvAvatar = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Avata);
                CircleImageView _imgAvatar = rootView.FindViewById<CircleImageView>(Resource.Id.img_ItemExpandDetaiProcessChild_Avata);
                View _vwMarginTop = rootView.FindViewById<View>(Resource.Id.vw_ItemExpandDetaiProcessChild_MarginTop);

                if (childPosition == 0) // Item đầu tiên -> margin top
                    _vwMarginTop.Visibility = ViewStates.Visible;
                else
                    _vwMarginTop.Visibility = ViewStates.Gone;

                #region Set Data Root Item

                if (groupPosition == _lstStepQTLC.Count - 1) // Ẩn line Group
                    _vwGroupLine.Visibility = ViewStates.Invisible;
                else
                    _vwGroupLine.Visibility = ViewStates.Visible;

                if (childPosition == _lstStepQTLC[groupPosition].ListStepQTLC.Count - 1) // Ẩn line Child
                    _vwChildLine.Visibility = ViewStates.Invisible;
                else
                    _vwChildLine.Visibility = ViewStates.Visible;

                if (!String.IsNullOrEmpty(_currentItemQTLC.AssignUserAvatar))
                    CmmDroidFunction.SetAvataByImagePath(_mainAct, _currentItemQTLC.AssignUserAvatar, _imgAvatar, _tvAvatar,50);
                else
                {                   
                    _tvAvatar.Visibility = ViewStates.Visible;
                    _imgAvatar.Visibility = ViewStates.Gone;
                }

                if (!String.IsNullOrEmpty(_currentItemQTLC.AssignUserName))
                {
                    _tvAvatar.Text = CmmFunction.GetAvatarName(_currentItemQTLC.AssignUserName);
                    _tvName.Text = _currentItemQTLC.AssignUserName;
                }
                else
                    _tvName.Text = "";

                if (!String.IsNullOrEmpty(_currentItemQTLC.AssignPositionTitle))
                    _tvPosition.Text = "- " + _currentItemQTLC.AssignPositionTitle;
                else
                    _tvPosition.Text = "";

                if (_currentItemQTLC.CompletedDate.HasValue)
                    _tvDate.Text = CTRLDetailProcess.GetFormatDateLang(_currentItemQTLC.CompletedDate.Value);
                else
                    _tvDate.Text = "";

                if (!string.IsNullOrEmpty(_currentItemQTLC.Comment))
                    _tvYKien.Text = _currentItemQTLC.Comment;
                else
                    _tvYKien.Visibility = ViewStates.Gone;

                if (!string.IsNullOrEmpty(_currentItemQTLC.SubmitAction))
                {
                    GradientDrawable _drawable = new GradientDrawable();
                    _drawable.SetStroke(10, new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));
                    _drawable.SetCornerRadius(10);
                    _drawable.SetShape(ShapeType.Rectangle);
                    _drawable.SetColor(CTRLDetailProcess.GetColorByActionIDProcess(_context, _currentItemQTLC.SubmitActionId.Value));

                    _tvAction.Text = _currentItemQTLC.SubmitAction.Trim();
                    _tvAction.Background = _drawable;
                }
                else
                    _tvAction.Visibility = ViewStates.Gone;
                #endregion

                return rootView;
            }
            else // Child View
            {
                LayoutInflater mInflater = LayoutInflater.From(_context);
                View rootView = mInflater.Inflate(Resource.Layout.ItemExpandDetaiProcessChild_Lv2, null);
                TextView _tvDate = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Lv2_Date);
                TextView _tvYKien = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Lv2_YKien);
                TextView _tvName = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Lv2_Name);
                TextView _tvPosition = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Lv2_Position);
                TextView _tvAction = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Lv2_Action);
                View _vwGroupLine = rootView.FindViewById<View>(Resource.Id.vw_ItemExpandDetaiProcessChild_Lv2_GroupLine);
                View _vwChildLine = rootView.FindViewById<View>(Resource.Id.vw_ItemExpandDetaiProcessChild_Lv2_ChildLine);
                TextView _tvAvatar = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Lv2_Avata);
                CircleImageView _imgAvatar = rootView.FindViewById<CircleImageView>(Resource.Id.img_ItemExpandDetaiProcessChild_Lv2_Avata);
                View _viewTop = rootView.FindViewById<View>(Resource.Id.view_ItemExpandDetaiProcessChild_Lv2_Top);
                View _viewBottom = rootView.FindViewById<View>(Resource.Id.view_ItemExpandDetaiProcessChild_Lv2_Bottom);
                View _viewMarginBottom = rootView.FindViewById<View>(Resource.Id.vw_ItemExpandDetaiProcessChild_Lv2_MarginBottom);
                _vwChildLine.Visibility = ViewStates.Invisible; // Luôn ẩn

                if (childPosition == _lstStepQTLC[groupPosition].ListStepQTLC.Count - 1) // Ẩn line bottom + margin bottom thêm
                {
                    _viewBottom.Visibility = ViewStates.Gone;
                    _viewMarginBottom.Visibility = ViewStates.Visible;
                }
                else
                {
                    _viewBottom.Visibility = ViewStates.Visible;
                    _viewMarginBottom.Visibility = ViewStates.Gone;
                }

                #region Set Data Root Item

                if (groupPosition == _lstStepQTLC.Count - 1) // Ẩn line Group
                    _vwGroupLine.Visibility = ViewStates.Invisible;
                else
                    _vwGroupLine.Visibility = ViewStates.Visible;

                if (!String.IsNullOrEmpty(_currentItemQTLC.AssignUserAvatar))
                    CmmDroidFunction.SetAvataByImagePath(_mainAct, _currentItemQTLC.AssignUserAvatar, _imgAvatar, _tvAvatar, 50);
                else
                {
                    _tvAvatar.Visibility = ViewStates.Visible;
                    _imgAvatar.Visibility = ViewStates.Gone;
                }

                if (!String.IsNullOrEmpty(_currentItemQTLC.AssignUserName))
                {
                    _tvAvatar.Text = CmmFunction.GetAvatarName(_currentItemQTLC.AssignUserName);
                    _tvName.Text = _currentItemQTLC.AssignUserName;
                }
                else
                    _tvName.Text = "";

                if (!String.IsNullOrEmpty(_currentItemQTLC.AssignPositionTitle))
                    _tvPosition.Text = "- " + _currentItemQTLC.AssignPositionTitle;
                else
                    _tvPosition.Text = "";


                if (_currentItemQTLC.CompletedDate.HasValue)
                    _tvDate.Text = CTRLDetailProcess.GetFormatDateLang(_currentItemQTLC.CompletedDate.Value);
                else
                    _tvDate.Text = "";

                if (!string.IsNullOrEmpty(_currentItemQTLC.Comment))
                    _tvYKien.Text = _currentItemQTLC.Comment;
                else
                    _tvYKien.Visibility = ViewStates.Gone;

                if (!string.IsNullOrEmpty(_currentItemQTLC.SubmitAction))
                {
                    GradientDrawable _drawable = new GradientDrawable();
                    _drawable.SetStroke(2, new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));
                    _drawable.SetCornerRadius(10);
                    _drawable.SetShape(ShapeType.Rectangle);
                    _drawable.SetColor(CTRLDetailProcess.GetColorByActionIDProcess(_context, _currentItemQTLC.SubmitActionId.Value));

                    _tvAction.Text = _currentItemQTLC.SubmitAction.Trim();
                    _tvAction.Background = _drawable;
                }
                else
                    _tvAction.Visibility = ViewStates.Gone;
                #endregion

                return rootView;
            }
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
            View rootView = convertView;
            if (rootView == null)
            {
                LayoutInflater mInflater = LayoutInflater.From(_context);
                rootView = mInflater.Inflate(Resource.Layout.ItemExpandDetaiProcessGroup, null);
            }
            TextView _tvTitle = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessGroup_Title);
            TextView _tvStatus = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessGroup_Status);
            ImageView _imgStatus = rootView.FindViewById<ImageView>(Resource.Id.img_ItemExpandDetaiProcessGroup);
            View _vwMarginTop = rootView.FindViewById<View>(Resource.Id.vw_ItemExpandDetaiProcessGroup_MarginTop);

            if (groupPosition == 0)
                _vwMarginTop.Visibility = ViewStates.Visible;
            else
                _vwMarginTop.Visibility = ViewStates.Gone;

            if (!String.IsNullOrEmpty(_lstStepQTLC[groupPosition].Action))
                _tvTitle.Text = _lstStepQTLC[groupPosition].Action;
            else
                _tvTitle.Text = "";

            BeanQuaTrinhLuanChuyen firstItemQTLC = _lstStepQTLC[groupPosition].ListStepQTLC[0];
            if (firstItemQTLC.Status)
                _imgStatus.SetColorFilter(new Color(ContextCompat.GetColor(_context, Resource.Color.clActionGreen))); // tint green
            else
                _imgStatus.SetColorFilter(new Color(ContextCompat.GetColor(_context, Resource.Color.clOrangePocess))); // tint orange

            return rootView;
        }
        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }

    }
}