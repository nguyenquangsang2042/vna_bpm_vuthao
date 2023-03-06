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
    public class AdapterExpandDetailProcess_Ver2 : BaseExpandableListAdapter
    {
        MainActivity _mainAct;
        private Context _context;
        private List<BeanQuaTrinhLuanChuyen> _lstQTLC = new List<BeanQuaTrinhLuanChuyen>();
        private ControllerDetailProcess CTRLDetailProcess = new ControllerDetailProcess();
        public AdapterExpandDetailProcess_Ver2(MainActivity _mainAct, Context _context, List<BeanQuaTrinhLuanChuyen> _lstQTLC)
        {
            this._mainAct = _mainAct;
            this._context = _context;
            this._lstQTLC = _lstQTLC;
        }

        public override int GroupCount
        {
            get
            {
                return _lstQTLC.Count;
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
            return _lstQTLC[groupPosition].ChildHistory != null ? _lstQTLC[groupPosition].ChildHistory.Count : 0;
        }

        class NotifyTestNewLayoutAdapterViewHolder : Java.Lang.Object
        {
            public LinearLayout _lnAll { get; set; }
            public LinearLayout _lnCategory { get; set; }
            public View _viewLeftSelected { get; set; }
            public TextView _tvCategory { get; set; }
            public View _viewCategory { get; set; }
            public LinearLayout _lnContent { get; set; }
            public TextView _tvAvatar { get; set; }
            public CircleImageView _imgAvatar { get; set; }
            public TextView _tvTitle { get; set; }
            public TextView _tvTime { get; set; }
            public TextView _tvDescription { get; set; }
            public ImageView _imgFlag { get; set; }
            public ImageView _imgAttach { get; set; }
            public RelativeLayout _relaAvatar2 { get; set; }
            public LinearLayout _lnAvatar2 { get; set; }
            public TextView _tvAvatar2 { get; set; }
            public CircleImageView _imgAvatar2 { get; set; }
            public TextView _tvStatus { get; set; }
            public TextView _tvStatusTime { get; set; }
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            BeanQuaTrinhLuanChuyen _currentItemQTLC = _lstQTLC[groupPosition].ChildHistory[childPosition];

            View rootView = convertView;
            if (rootView == null)
            {
                LayoutInflater mInflater = LayoutInflater.From(_context);
                rootView = mInflater.Inflate(Resource.Layout.ItemExpandDetaiProcessChild_Ver2, null);
            }

            TextView _tvDate = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Ver2_Date);
            TextView _tvYKien = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Ver2_YKien);
            TextView _tvName = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Ver2_Name);
            TextView _tvPosition = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Ver2_Position);
            TextView _tvAction = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Ver2_Action);
            View _vwGroupLine = rootView.FindViewById<View>(Resource.Id.vw_ItemExpandDetaiProcessChild_Ver2_GroupLine);
            View _vwChildLine = rootView.FindViewById<View>(Resource.Id.vw_ItemExpandDetaiProcessChild_Ver2_ChildLine);
            TextView _tvAvatar = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Ver2_Avata);
            CircleImageView _imgAvatar = rootView.FindViewById<CircleImageView>(Resource.Id.img_ItemExpandDetaiProcessChild_Ver2_Avata);
            View _vwMarginTop = rootView.FindViewById<View>(Resource.Id.vw_ItemExpandDetaiProcessChild_Ver2_MarginTop);
            RecyclerView _recyChild = rootView.FindViewById<RecyclerView>(Resource.Id.recy_ItemExpandDetaiProcessChild_Ver2_Child);

            if (childPosition == 0) // Item đầu tiên -> margin top
                _vwMarginTop.Visibility = ViewStates.Visible;
            else
                _vwMarginTop.Visibility = ViewStates.Gone;

            #region Set Data Root Item

            if (groupPosition == _lstQTLC.Count - 1) // Ẩn line Group
                _vwGroupLine.Visibility = ViewStates.Invisible;
            else
                _vwGroupLine.Visibility = ViewStates.Visible;

            ////if (childPosition == _lstStepQTLC[groupPosition].ListStepQTLC.Count - 1) // Ẩn line Child
            ////    _vwChildLine.Visibility = ViewStates.Invisible;
            ////else
            ////    _vwChildLine.Visibility = ViewStates.Visible;

            if (!String.IsNullOrEmpty(_currentItemQTLC.AssignUserAvatar))
                CmmDroidFunction.SetAvataByImagePath(_mainAct, _currentItemQTLC.AssignUserAvatar, _imgAvatar, _tvAvatar, 80);
            else
            {
                _tvAvatar.Visibility = ViewStates.Visible;
                _imgAvatar.Visibility = ViewStates.Gone;
            }

            if (!String.IsNullOrEmpty(_currentItemQTLC.AssignUserName))
            {
                _tvAvatar.Text = CmmFunction.GetAvatarName(_currentItemQTLC.AssignUserName.Trim());
                _tvName.Text = _currentItemQTLC.AssignUserName;
            }
            else
                _tvName.Text = "";

            if (!String.IsNullOrEmpty(_currentItemQTLC.AssignPositionTitle))
                _tvPosition.Text = _currentItemQTLC.AssignPositionTitle;
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

            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
            {
                if (!string.IsNullOrEmpty(_currentItemQTLC.SubmitAction))
                {
                    GradientDrawable _drawable = new GradientDrawable();
                    _drawable.SetStroke(2, new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));
                    _drawable.SetCornerRadius(7);
                    _drawable.SetShape(ShapeType.Rectangle);
                    _drawable.SetColor(CTRLDetailProcess.GetColorByActionIDProcess(_context, _currentItemQTLC.SubmitActionId.Value));

                    _tvAction.Text = _currentItemQTLC.SubmitAction.Trim();
                    _tvAction.Background = _drawable;
                }
                else
                    _tvAction.Visibility = ViewStates.Gone;
            }
            else
            {
                if (!string.IsNullOrEmpty(_currentItemQTLC.SubmitActionEN))
                {
                    GradientDrawable _drawable = new GradientDrawable();
                    _drawable.SetStroke(2, new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));
                    _drawable.SetCornerRadius(7);
                    _drawable.SetShape(ShapeType.Rectangle);
                    _drawable.SetColor(CTRLDetailProcess.GetColorByActionIDProcess(_context, _currentItemQTLC.SubmitActionId.Value));

                    _tvAction.Text = _currentItemQTLC.SubmitActionEN.Trim();
                    _tvAction.Background = _drawable;
                }
                else
                    _tvAction.Visibility = ViewStates.Gone;
            }
            #endregion

            #region Recy Child
            if (_currentItemQTLC.ChildHistory != null && _currentItemQTLC.ChildHistory.Count > 0)
            {
                _recyChild.Visibility = ViewStates.Visible;
                AdapterRecyChildProcess_Ver2 adapterRecyChildProcess_Ver2 = new AdapterRecyChildProcess_Ver2(_mainAct, _context, _currentItemQTLC.ChildHistory);
                StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                _recyChild.SetAdapter(adapterRecyChildProcess_Ver2);
                _recyChild.SetLayoutManager(staggeredGridLayoutManager);
            }
            else
            {
                _recyChild.Visibility = ViewStates.Gone;
            }

            #endregion

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

            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
            {
                if (!String.IsNullOrEmpty(_lstQTLC[groupPosition].Title))
                    _tvTitle.Text = _lstQTLC[groupPosition].Title;
                else
                    _tvTitle.Text = "";
            }
            else
            {
                if (!String.IsNullOrEmpty(_lstQTLC[groupPosition].TitleEN))
                    _tvTitle.Text = _lstQTLC[groupPosition].TitleEN;
                else
                    _tvTitle.Text = "";
            }

            if (_lstQTLC[groupPosition].Status)
                _imgStatus.SetColorFilter(new Color(ContextCompat.GetColor(_context, Resource.Color.clActionGreen))); // tint green
            else
                _imgStatus.SetColorFilter(new Color(ContextCompat.GetColor(_context, Resource.Color.clActionYellow))); // tint orange

            return rootView;
        }
        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }

        #region RecyChild
        public class AdapterRecyChildProcess_Ver2 : RecyclerView.Adapter
        {
            private MainActivity _mainAct;
            private Context _context;
            private List<BeanQuaTrinhLuanChuyen> _lstQTLC = new List<BeanQuaTrinhLuanChuyen>();
            private ControllerDetailProcess CTRLDetailProcess = new ControllerDetailProcess();
            public AdapterRecyChildProcess_Ver2(MainActivity _mainAct, Context _context, List<BeanQuaTrinhLuanChuyen> _lstQTLC)
            {
                this._mainAct = _mainAct;
                this._context = _context;
                this._lstQTLC = _lstQTLC;
            }
            public override int ItemCount => _lstQTLC.Count;
            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemExpandDetaiProcessChild_Ver2_Lv2, parent, false);
                AdapterRecyChildProcess_Ver2Holder holder = new AdapterRecyChildProcess_Ver2Holder(itemView, null);
                return holder;
            }
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                AdapterRecyChildProcess_Ver2Holder _holder = holder as AdapterRecyChildProcess_Ver2Holder;

                BeanQuaTrinhLuanChuyen _currentItemQTLC = _lstQTLC[position];

                if (position == _lstQTLC.Count - 1) // Ẩn line bottom + margin bottom thêm
                {
                    ////_viewBottom.Visibility = ViewStates.Gone;
                    _holder._viewMarginBottom.Visibility = ViewStates.Visible;
                }
                else
                {
                    ////_viewBottom.Visibility = ViewStates.Visible;
                    _holder._viewMarginBottom.Visibility = ViewStates.Gone;
                }

                #region Set Data Root Item

                _holder._vwGroupLine.Visibility = ViewStates.Invisible;
                ////if (groupPosition == _lstQTLC.Count - 1) // Ẩn line Group
                ////    _holder._vwGroupLine.Visibility = ViewStates.Invisible;
                ////else
                ////    _holder._vwGroupLine.Visibility = ViewStates.Visible;

                if (!String.IsNullOrEmpty(_currentItemQTLC.AssignUserAvatar))
                    CmmDroidFunction.SetAvataByImagePath(_mainAct, _currentItemQTLC.AssignUserAvatar, _holder._imgAvatar, _holder._tvAvatar, 80);
                else
                {
                    _holder._tvAvatar.Visibility = ViewStates.Visible;
                    _holder._imgAvatar.Visibility = ViewStates.Gone;
                }

                if (!String.IsNullOrEmpty(_currentItemQTLC.AssignUserName))
                {
                    _holder._tvAvatar.Text = CmmFunction.GetAvatarName(_currentItemQTLC.AssignUserName.Trim());
                    _holder._tvName.Text = _currentItemQTLC.AssignUserName;
                }
                else
                    _holder._tvName.Text = "";

                if (!String.IsNullOrEmpty(_currentItemQTLC.AssignPositionTitle))
                    _holder._tvPosition.Text = _currentItemQTLC.AssignPositionTitle;
                else
                    _holder._tvPosition.Text = "";


                if (_currentItemQTLC.CompletedDate.HasValue)
                    _holder._tvDate.Text = CTRLDetailProcess.GetFormatDateLang(_currentItemQTLC.CompletedDate.Value);
                else
                    _holder._tvDate.Text = "";

                if (!string.IsNullOrEmpty(_currentItemQTLC.Comment))
                    _holder._tvYKien.Text = _currentItemQTLC.Comment;
                else
                    _holder._tvYKien.Visibility = ViewStates.Gone;

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    if (!string.IsNullOrEmpty(_currentItemQTLC.SubmitAction))
                    {
                        GradientDrawable _drawable = new GradientDrawable();
                        _drawable.SetStroke(2, new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));
                        _drawable.SetCornerRadius(7);
                        _drawable.SetShape(ShapeType.Rectangle);
                        _drawable.SetColor(CTRLDetailProcess.GetColorByActionIDProcess(_context, _currentItemQTLC.SubmitActionId.Value));

                        _holder._tvAction.Text = _currentItemQTLC.SubmitAction.Trim();
                        _holder._tvAction.Background = _drawable;
                    }
                    else
                        _holder._tvAction.Visibility = ViewStates.Gone;
                }
                else
                {
                    if (!string.IsNullOrEmpty(_currentItemQTLC.SubmitActionEN))
                    {
                        GradientDrawable _drawable = new GradientDrawable();
                        _drawable.SetStroke(2, new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));
                        _drawable.SetCornerRadius(7);
                        _drawable.SetShape(ShapeType.Rectangle);
                        _drawable.SetColor(CTRLDetailProcess.GetColorByActionIDProcess(_context, _currentItemQTLC.SubmitActionId.Value));

                        _holder._tvAction.Text = _currentItemQTLC.SubmitActionEN.Trim();
                        _holder._tvAction.Background = _drawable;
                    }
                    else
                        _holder._tvAction.Visibility = ViewStates.Gone;
                }
                #endregion

            }
        }
        public class AdapterRecyChildProcess_Ver2Holder : RecyclerView.ViewHolder
        {
            public TextView _tvDate { get; set; }
            public TextView _tvYKien { get; set; }
            public TextView _tvName { get; set; }
            public TextView _tvPosition { get; set; }
            public TextView _tvAction { get; set; }
            public View _vwGroupLine { get; set; }
            public View _vwChildLine { get; set; }
            public TextView _tvAvatar { get; set; }
            public CircleImageView _imgAvatar { get; set; }
            public View _viewTop { get; set; }
            public View _viewBottom { get; set; }
            public View _viewMarginBottom { get; set; }

            public AdapterRecyChildProcess_Ver2Holder(View itemview, Action<int> listener) : base(itemview)
            {
                _tvDate = itemview.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Ver2_Lv2_Date);
                _tvYKien = itemview.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Ver2_Lv2_YKien);
                _tvName = itemview.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Ver2_Lv2_Name);
                _tvPosition = itemview.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Ver2_Lv2_Position);
                _tvAction = itemview.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Ver2_Lv2_Action);
                _vwGroupLine = itemview.FindViewById<View>(Resource.Id.vw_ItemExpandDetaiProcessChild_Ver2_Lv2_GroupLine);
                _vwChildLine = itemview.FindViewById<View>(Resource.Id.vw_ItemExpandDetaiProcessChild_Ver2_Lv2_ChildLine);
                _tvAvatar = itemview.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetaiProcessChild_Ver2_Lv2_Avata);
                _imgAvatar = itemview.FindViewById<CircleImageView>(Resource.Id.img_ItemExpandDetaiProcessChild_Ver2_Lv2_Avata);
                _viewTop = itemview.FindViewById<View>(Resource.Id.view_ItemExpandDetaiProcessChild_Ver2_Lv2_Top);
                _viewBottom = itemview.FindViewById<View>(Resource.Id.view_ItemExpandDetaiProcessChild_Ver2_Lv2_Bottom);
                _viewMarginBottom = itemview.FindViewById<View>(Resource.Id.vw_ItemExpandDetaiProcessChild_Ver2_Lv2_MarginBottom);
                //_lnAll.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
        #endregion

    }
}