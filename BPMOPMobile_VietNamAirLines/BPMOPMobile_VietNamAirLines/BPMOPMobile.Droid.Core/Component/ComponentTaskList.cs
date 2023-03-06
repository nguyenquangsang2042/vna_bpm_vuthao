using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using Newtonsoft.Json;
using Refractored.Controls;
using SQLite;
using static Android.Widget.ExpandableListView;
using static BPMOPMobile.Droid.Core.Class.MinionActionCore;
using static BPMOPMobile.Droid.Core.Component.ComponentTaskList.AdapterExpandComponentTaskList;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ComponentTaskList : ComponentBase
    {
        private Activity _mainAct { get; set; }
        private Context _context { get; set; }
        private LinearLayout _parentView { get; set; }
        private LinearLayout _lnExpandable { get; set; }
        private LinearLayout _lnTitle { get; set; }
        private TextView _tvTitle { get; set; }
        private ExpandableListView _expandTaskList { get; set; }

        private List<BeanTask> _lstTask = new List<BeanTask>();

        private bool _IsDetailCreateTaskView; // nếu là view DetailCreateTask sẽ ẩn Title

        public event EventHandler<TaskListItemClick> TaskListItemClickEvent; // Event khi click vào 1 Task trên danh sách

        public ComponentTaskList(Activity _mainAct, LinearLayout _parentView, List<BeanTask> _lstTask, bool _IsDetailCreateTaskView = false)
        {
            this._mainAct = _mainAct;
            this._parentView = _parentView;
            this._lstTask = _lstTask;
            this._IsDetailCreateTaskView = _IsDetailCreateTaskView;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            _lnExpandable = new LinearLayout(_mainAct);
            _expandTaskList = new ExpandableListView(_mainAct);

            _lnTitle = new LinearLayout(_mainAct);
            _tvTitle = new TextView(_mainAct);

            _tvTitle.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? CmmFunction.GetTitle("TEXT_TASKLIST", "Danh sách công việc") : CmmFunction.GetTitle("TEXT_TASKLIST", "Task list");
            _tvTitle.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            _tvTitle.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
            _tvTitle.SetTextSize(ComplexUnitType.Sp, 12);

            _expandTaskList.SetGroupIndicator(null);
            _expandTaskList.SetChildIndicator(null);
            _expandTaskList.DividerHeight = 0;
            _expandTaskList.NestedScrollingEnabled = true;

            _lnTitle.Orientation = Android.Widget.Orientation.Vertical;
            _lnExpandable.Orientation = Android.Widget.Orientation.Vertical;
        }

        public override void InitializeFrameView(LinearLayout frame)
        {
            _context = frame.Context;
            base.InitializeFrameView(frame);

            int _padding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 3, _mainAct.Resources.DisplayMetrics);
            LinearLayout.LayoutParams _paramsTitle = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams _paramsLine = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, (int)TypedValue.ApplyDimension(ComplexUnitType.Px, 1, _mainAct.Resources.DisplayMetrics));
            LinearLayout.LayoutParams _paramsRecy = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);

            _paramsLine.SetMargins(0, 3 * _padding, 0, 3 * _padding);
            _paramsRecy.SetMargins(0, 0, 0, 2 * _padding);

            _tvTitle.LayoutParameters = _paramsTitle;
            _expandTaskList.LayoutParameters = _paramsRecy;

            frame.SetPadding(2 * _padding, 2 * _padding, 2 * _padding, 0);
            _tvTitle.SetPadding(0, 3 * _padding, _padding, 6 * _padding);

            _lnTitle.AddView(_tvTitle);

            _expandTaskList.NestedScrollingEnabled = true;

            if (_IsDetailCreateTaskView == false) // View chi tiết Task ko có Title
            {
                frame.AddView(_lnTitle);
            }
            frame.AddView(_lnExpandable);
        }

        protected virtual void HandleTouchDown(object sender, EventArgs e)
        {

        }

        public override void SetTitle()
        {
            base.SetTitle();
        }

        public override void SetValue()
        {
            base.SetValue();

            if (_lstTask != null && _lstTask.Count > 0) // trường hợp load ở Fragment Detail Workflow          
            {
                int itemHeight = (int)CmmDroidFunction.ConvertDpToPixel(100, _context); // item 3 cấp đều 100 dp
                List<BeanExpandTask> _lstExpandTaskClone = CloneListExpandTask(_lstTask);

                if (_lstExpandTaskClone != null && _lstExpandTaskClone.Count > 0)
                {
                    Action action = new Action(() =>
                    {
                        AdapterExpandComponentTaskList _adapterComponentTaskList = new AdapterExpandComponentTaskList(_mainAct, _context, _lnExpandable, _lstExpandTaskClone);
                        _adapterComponentTaskList.CustomItemClick_Detail += Click_ItemListTask;
                        _expandTaskList.SetAdapter(_adapterComponentTaskList);

                        for (int i = 0; i < _adapterComponentTaskList.GroupCount; i++)
                            _expandTaskList.ExpandGroup(i);

                        _expandTaskList.GroupClick += (sender, e) =>
                        {
                            // Handle height
                            int totalChildItemCount = _lstExpandTaskClone[e.GroupPosition].lstChild.Count;
                            foreach (var item in _lstExpandTaskClone[e.GroupPosition].lstChild)
                            {
                                totalChildItemCount += item.lstChild.Count;
                            }

                            // Handle base
                            if (_expandTaskList.IsGroupExpanded(e.GroupPosition)) // Đang Expand -> collapse lại
                            {
                                _expandTaskList.CollapseGroup(e.GroupPosition);
                                _lnExpandable.LayoutParameters.Height -= itemHeight * totalChildItemCount;
                            }
                            else
                            {
                                _expandTaskList.ExpandGroup(e.GroupPosition);
                                _lnExpandable.LayoutParameters.Height += itemHeight * totalChildItemCount;
                            }
                        };

                        _lnExpandable.RemoveAllViews();
                        _lnExpandable.AddView(_expandTaskList);
                        _lnExpandable.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_context));
                    });

                    #region Handle Shimmer

                    _lnExpandable.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, (itemHeight * _lstTask.Count) + 20);
                    if (_lstTask.Count > 5) // nếu > 5 thì limit lại (Item view set cứng 100dp -> (90*5) + 50 để biết là có thể scroll)
                    {
                        LayoutInflater _inflater = (LayoutInflater)_mainAct.GetSystemService(Android.Content.Context.LayoutInflaterService);
                        for (int i = 0; i < _lstTask.Count + 2; i++)
                        {
                            _lnExpandable.AddView(_inflater.Inflate(Resource.Layout.ItemControlTaskList_Shimmer, null));
                        }
                        new Handler().PostDelayed(action, 100 * _lstTask.Count);
                    }
                    else
                    {
                        new Handler().PostDelayed(action, 0);
                    }
                    #endregion
                }
            }
        }

        private void Click_ItemListTask(object sender, TaskListItemClick e)
        {
            try
            {
                if (TaskListItemClickEvent != null)
                    TaskListItemClickEvent(sender, e);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemListComment_AttachDetail", ex);
#endif
            }
        }

        /// <summary>
        /// Clone List Task về dạng đúng Adapter
        /// </summary>
        /// <param name="_lstTask"></param>
        /// <returns></returns>
        public List<BeanExpandTask> CloneListExpandTask(List<BeanTask> _lstTask)
        {
            List<BeanExpandTask> _result = new List<BeanExpandTask>();
            try
            {
                _lstTask = _lstTask.OrderBy(x => x.ID).ToList();
                List<BeanTask> _lstParent = _lstTask.Where(x => x.Parent == 0).ToList();
                if (_lstParent != null && _lstParent.Count > 0) // từ cấp parent
                {
                    foreach (BeanTask _parentItem in _lstParent) // Xử lý Level 1
                    {
                        BeanExpandTask _itemLV1 = new BeanExpandTask();
                        _itemLV1.groupItem = _parentItem;
                        _itemLV1.lstChild = new List<BeanExpandTask>();
                        List<BeanTask> _lstchildLV1 = _lstTask.Where(x => x.Parent == _parentItem.ID).ToList();
                        foreach (BeanTask _childLv1Item in _lstchildLV1) // Xử lý Level 2
                        {
                            BeanExpandTask _itemLV2 = new BeanExpandTask();
                            _itemLV2.groupItem = _childLv1Item;
                            _itemLV2.lstChild = new List<BeanExpandTask>();
                            HandleExpandLV3(_lstTask, _itemLV2.lstChild, _childLv1Item); // Xử lý Level 3
                            _itemLV1.lstChild.Add(_itemLV2);
                        }
                        _result.Add(_itemLV1);
                    }
                }
                else // xử lý từ cấp con
                {
                    foreach (BeanTask _parentItem in _lstTask) // Xử lý Level 1
                    {
                        BeanExpandTask _itemLV1 = new BeanExpandTask();
                        _itemLV1.groupItem = _parentItem;
                        _itemLV1.lstChild = new List<BeanExpandTask>();
                        List<BeanTask> _lstchildLV1 = _lstTask.Where(x => x.Parent == _parentItem.ID).ToList();
                        foreach (BeanTask _childLv1Item in _lstchildLV1) // Xử lý Level 2
                        {
                            BeanExpandTask _itemLV2 = new BeanExpandTask();
                            _itemLV2.groupItem = _childLv1Item;
                            _itemLV2.lstChild = new List<BeanExpandTask>();
                            HandleExpandLV3(_lstTask, _itemLV2.lstChild, _childLv1Item); // Xử lý Level 3
                            _itemLV1.lstChild.Add(_itemLV2);
                        }
                        _result.Add(_itemLV1);
                    }
                }
            }
            catch (System.Exception ex)
            {
                _result = new List<BeanExpandTask>();
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoard", "CloneListComponentTaskList", ex);
#endif
            }
            return _result;
        }

        private static void HandleExpandLV3(List<BeanTask> _lstTask, List<BeanExpandTask> _result, BeanTask _parentItem)
        {
            List<BeanTask> _lstchild = _lstTask.Where(x => x.Parent == _parentItem.ID).ToList();
            foreach (BeanTask parentItem in _lstchild) // Handle ChildList
            {
                BeanExpandTask _itemChild = new BeanExpandTask();
                _itemChild.groupItem = parentItem;
                _itemChild.lstChild = new List<BeanExpandTask>();
                _result.Add(_itemChild);
                HandleExpandLV3(_lstTask, _result, parentItem);
            }
        }

        public class AdapterExpandComponentTaskList : BaseExpandableListAdapter
        {
            private Activity _mainAct;
            private Context _context;
            private LinearLayout _lnExpandParent;
            private List<BeanExpandTask> _lstGroupTask = new List<BeanExpandTask>();
            public event EventHandler<TaskListItemClick> CustomItemClick_Detail;
            private int itemHeight;

            public void OnItemClickDetail(TaskListItemClick e)
            {
                if (CustomItemClick_Detail != null)
                    CustomItemClick_Detail(this, e);
            }

            public AdapterExpandComponentTaskList(Activity _mainAct, Context _context, LinearLayout _lnExpandParent, List<BeanExpandTask> _lstGroupTask)
            {
                this._mainAct = _mainAct;
                this._context = _context;
                this._lstGroupTask = _lstGroupTask;
                this._lnExpandParent = _lnExpandParent;
                itemHeight = (int)CmmDroidFunction.ConvertDpToPixel(100, _context);
            }

            public override int GroupCount => _lstGroupTask.Count;

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
                if (_lstGroupTask[groupPosition].lstChild != null && _lstGroupTask[groupPosition].lstChild.Count > 0)
                    return _lstGroupTask[groupPosition].lstChild.Count;
                return 0;
            }

            public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
            {
                if (childPosition == 0) // chỉ đổ thằng đầu tiên
                {
                    List<BeanExpandTask> _currentLstChild = _lstGroupTask[groupPosition].lstChild;

                    #region Get View
                    ComponentTaskList_ChildHolder _holder;
                    //if (convertView == null)
                    //{
                    convertView = LayoutInflater.From(_context).Inflate(Resource.Layout.ItemControlTaskList_Child, null);
                    _holder = new ComponentTaskList_ChildHolder
                    {
                        _lnAll = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlTaskList_All),
                        _expandChild = convertView.FindViewById<ExpandableListView>(Resource.Id.expand_ItemControlTaskList_Child)
                    };
                    _holder._expandChild.SetGroupIndicator(null);
                    _holder._expandChild.SetChildIndicator(null);
                    _holder._expandChild.Foreground = null;
                    _holder._expandChild.DividerHeight = 0;
                    convertView.Tag = _holder;
                    //}
                    //else
                    //{
                    //    _holder = (ComponentTaskList_ChildHolder)convertView.Tag;
                    //}

                    #endregion

                    if (_currentLstChild != null && _currentLstChild.Count > 0)
                    {
                        #region Init Properties
                        if (groupPosition % 2 == 0)
                        {
                            GradientDrawable _drawable = new GradientDrawable();
                            _drawable.SetCornerRadii(new float[] { 0, 0, 0, 0, 10, 10, 10, 10 }); //{mTopLeftRadius, mTopLeftRadius, mTopRightRadius, mTopRightRadius, mBottomRightRadius, mBottomRightRadius, mBottomLeftRadius, mBottomLeftRadius}
                            _drawable.SetShape(ShapeType.Rectangle);
                            _drawable.SetColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueNavigation)));
                            _holder._lnAll.Background = _drawable;
                        }
                        else
                        {
                            GradientDrawable _drawable = new GradientDrawable();
                            _drawable.SetColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));
                            _holder._lnAll.Background = _drawable;
                        }

                        AdapterExpandComponentTaskList_Child _adapter = new AdapterExpandComponentTaskList_Child(_mainAct, _context, this, _currentLstChild, childPosition == (_lstGroupTask[groupPosition].lstChild.Count - 1) ? true : false);
                        _holder._expandChild.Visibility = ViewStates.Visible;
                        _holder._expandChild.SetAdapter(_adapter);
                        _holder._expandChild.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, itemHeight * _adapter.GroupCount);
                        #endregion

                        #region Event

                        _holder._expandChild.GroupCollapse += (sender, e) =>
                        {
                            _holder._expandChild.LayoutParameters.Height -= _adapter.GetChildrenCount(e.GroupPosition) * itemHeight;
                        };
                        _holder._expandChild.GroupExpand += (sender, e) =>
                        {
                            _holder._expandChild.LayoutParameters.Height += _adapter.GetChildrenCount(e.GroupPosition) * itemHeight;
                        };

                        _holder._expandChild.ChildClick += (sender, e) =>
                        {
                            try
                            {
                                AdapterExpandComponentTaskList_Child _curAdapter = (AdapterExpandComponentTaskList_Child)_holder._expandChild.ExpandableListAdapter;
                                BeanExpandTask _clickedItem = _curAdapter.GetItemChildClick(e.GroupPosition, e.ChildPosition);
                                OnItemClickDetail(new MinionActionCore.TaskListItemClick(1, _clickedItem));
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetChildView", ex);
#endif
                            }
                        };

                        for (int i = 0; i < _adapter.GroupCount; i++)
                            _holder._expandChild.ExpandGroup(i);

                        #endregion
                    }
                    else
                    {
                        _holder._expandChild.Visibility = ViewStates.Gone;
                    }

                    return convertView;
                }
                else
                    return new View(_context);
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
                ComponentTaskList_GroupHolder _holder;
                BeanTask _currentTaskItem = _lstGroupTask[groupPosition].groupItem;

                #region Get View
                if (convertView == null)
                {
                    convertView = LayoutInflater.From(_context).Inflate(Resource.Layout.ItemControlTaskList_Group, null);
                    _holder = new ComponentTaskList_GroupHolder
                    {
                        _lnAll = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlTaskList_Group_All),
                        _lnContentClick = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlTaskList_Group_ContentClick),
                        _vwMarginTop = convertView.FindViewById<View>(Resource.Id.vw_ItemControlTaskList_Group_MarginTop),
                        _vwGroupLine = convertView.FindViewById<View>(Resource.Id.vw_ItemControlTaskList_GroupLine),
                        _relaGroup = convertView.FindViewById<RelativeLayout>(Resource.Id.rela_ItemControlTaskList_Child_Group),
                        _tvTitle = convertView.FindViewById<TextView>(Resource.Id.tv_ItemControlTaskList_Group_Title),
                        _imgCollapsed = convertView.FindViewById<ImageView>(Resource.Id.img_ItemControlTaskList_Group),
                        _imgAvatar = convertView.FindViewById<CircleImageView>(Resource.Id.img_ItemControlTaskList_Group_Avata),
                        _tvName = convertView.FindViewById<TextView>(Resource.Id.tv_ItemControlTaskList_Group_Name),
                        _tvDate = convertView.FindViewById<TextView>(Resource.Id.tv_ItemControlTaskList_Group_Date),
                        _tvPosition = convertView.FindViewById<TextView>(Resource.Id.tv_ItemControlTaskList_Group_Position),
                        _tvAction = convertView.FindViewById<TextView>(Resource.Id.tv_ItemControlTaskList_Group_Action),
                    };
                    convertView.Tag = _holder;

                    _holder._lnContentClick.Click += (sender, e) =>
                    {
                        OnItemClickDetail(new MinionActionCore.TaskListItemClick(1, _lstGroupTask[groupPosition]));
                    };
                }
                else
                {
                    _holder = (ComponentTaskList_GroupHolder)convertView.Tag;
                }

                #endregion

                #region View Rule

                if (groupPosition % 2 == 0)
                {
                    GradientDrawable _drawable = new GradientDrawable();
                    if (isExpanded == true) // đang expand -> 2 coners
                    {
                        _drawable.SetCornerRadii(new float[] { 10, 10, 10, 10, 0, 0, 0, 0 }); //{mTopLeftRadius, mTopLeftRadius, mTopRightRadius, mTopRightRadius, mBottomRightRadius, mBottomRightRadius, mBottomLeftRadius, mBottomLeftRadius}
                    }
                    else // 4 corners
                    {
                        _drawable.SetCornerRadius(10);
                    }
                    _drawable.SetShape(ShapeType.Rectangle);
                    _drawable.SetColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueNavigation)));
                    _holder._lnAll.Background = _drawable;
                }
                else
                {
                    GradientDrawable _drawable = new GradientDrawable();
                    _drawable.SetColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));
                    _holder._lnAll.Background = _drawable;
                }

                if (isExpanded == true)
                {
                    _holder._vwGroupLine.Visibility = ViewStates.Visible;
                    _holder._imgCollapsed.SetImageResource(Resource.Drawable.icon_ver2_show);
                }
                else
                {
                    _holder._vwGroupLine.Visibility = ViewStates.Invisible;
                    _holder._imgCollapsed.SetImageResource(Resource.Drawable.icon_ver2_close);
                }

                if (_lstGroupTask[groupPosition].lstChild == null || _lstGroupTask[groupPosition].lstChild.Count == 0) // Ẩn line Group
                    _holder._relaGroup.Visibility = ViewStates.Invisible;
                else
                    _holder._relaGroup.Visibility = ViewStates.Visible;

                #endregion

                #region Set Data

                SetDataItemTaskToView(_currentTaskItem, _mainAct, _context,
                   /* Line 1 */ _holder._tvTitle, _holder._imgAvatar, _holder._tvName,
                   /* Line 2 */ _holder._tvDate,
                   /* Line 3 */ _holder._tvPosition, _holder._tvAction);

                #endregion

                return convertView;
            }

            public override bool IsChildSelectable(int groupPosition, int childPosition)
            {
                return true;
            }

            public class ComponentTaskList_ChildHolder : Java.Lang.Object
            {
                public LinearLayout _lnAll { get; set; }
                public ExpandableListView _expandChild { get; set; }
            }
            public class ComponentTaskList_GroupHolder : Java.Lang.Object
            {
                public LinearLayout _lnAll { get; set; }
                public LinearLayout _lnContentClick { get; set; }
                public View _vwMarginTop { get; set; }
                public View _vwGroupLine { get; set; }
                public RelativeLayout _relaGroup { get; set; }
                public TextView _tvTitle { get; set; }
                public ImageView _imgCollapsed { get; set; }
                public CircleImageView _imgAvatar { get; set; }
                public TextView _tvName { get; set; }
                public TextView _tvDate { get; set; }
                public TextView _tvPosition { get; set; }
                public TextView _tvAction { get; set; }
            }
        }

        public class AdapterExpandComponentTaskList_Child : BaseExpandableListAdapter
        {
            private Activity _mainAct;
            private Context _context;
            private List<BeanExpandTask> _lstGroupTask = new List<BeanExpandTask>();
            private AdapterExpandComponentTaskList _adapterParent;
            private bool _isLastChild;
            public AdapterExpandComponentTaskList_Child(Activity _mainAct, Context _context, AdapterExpandComponentTaskList _adapterParent, List<BeanExpandTask> _lstGroupTask, bool _isLastChild)
            {
                this._mainAct = _mainAct;
                this._context = _context;
                this._adapterParent = _adapterParent;
                this._lstGroupTask = _lstGroupTask;
                this._isLastChild = _isLastChild;
            }

            public BeanExpandTask GetItemChildClick(int groupPosition, int childPosition)
            {
                return _lstGroupTask[groupPosition].lstChild[childPosition];
            }

            public override int GroupCount => _lstGroupTask.Count;

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
                if (_lstGroupTask[groupPosition].lstChild != null && _lstGroupTask[groupPosition].lstChild.Count > 0)
                    return _lstGroupTask[groupPosition].lstChild.Count;
                return 0;
            }

            public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
            {
                ComponentTaskListChild_Holder_Lv2 _holder;
                BeanTask _currentTaskItem = _lstGroupTask[groupPosition].lstChild[childPosition].groupItem;

                #region Get View
                if (convertView == null)
                {
                    convertView = LayoutInflater.From(_context).Inflate(Resource.Layout.ItemControlTaskList_Child_Lv2, null);
                    _holder = new ComponentTaskListChild_Holder_Lv2
                    {
                        _vwMarginTop = convertView.FindViewById<View>(Resource.Id.vw_ItemControlTaskList_Child_Lv2_MarginTop),
                        _vwGroupHorizon = convertView.FindViewById<View>(Resource.Id.vw_ItemControlTaskList_Child_Lv2_GroupHorizon),
                        _lnGroup1 = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlTaskList_Child_Lv2_Group1),
                        _vwGroup1 = convertView.FindViewById<View>(Resource.Id.vw_ItemControlTaskList_Child_Lv2_Group1),
                        _lnGroup2 = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlTaskList_Child_Lv2_Group2),
                        _vwGroup2 = convertView.FindViewById<View>(Resource.Id.vw_ItemControlTaskList_Child_Lv2_Group2),
                        //_relaGroup = convertView.FindViewById<RelativeLayout>(Resource.Id.rela_ItemControlTaskList_Child_Group),
                        _tvTitle = convertView.FindViewById<TextView>(Resource.Id.tv_ItemControlTaskList_Child_Lv2_Title),
                        _imgAvatar = convertView.FindViewById<CircleImageView>(Resource.Id.img_ItemControlTaskList_Child_Lv2_Avata),
                        _tvName = convertView.FindViewById<TextView>(Resource.Id.tv_ItemControlTaskList_Child_Lv2_Name),
                        _tvDate = convertView.FindViewById<TextView>(Resource.Id.tv_ItemControlTaskList_Child_Lv2_Date),
                        _tvPosition = convertView.FindViewById<TextView>(Resource.Id.tv_ItemControlTaskList_Child_Lv2_Position),
                        _tvAction = convertView.FindViewById<TextView>(Resource.Id.tv_ItemControlTaskList_Child_Lv2_Action),
                        _vwItemSpacing = convertView.FindViewById<View>(Resource.Id.vw_ItemControlTaskList_Child_Lv2_ItemSpacing),
                    };
                    convertView.Tag = _holder;
                }
                else
                {
                    _holder = (ComponentTaskListChild_Holder_Lv2)convertView.Tag;
                }

                #endregion

                #region View Rule

                if (childPosition == _lstGroupTask[groupPosition].lstChild.Count - 1) // Item cuối -> rút ngắn line đi
                {
                    _holder._vwGroup2.LayoutParameters = new LinearLayout.LayoutParams((int)CmmDroidFunction.ConvertDpToPixel(1.5f, _context), (int)CmmDroidFunction.ConvertDpToPixel(10, _context));
                    _holder._vwItemSpacing.Visibility = ViewStates.Invisible;
                }
                else // chưa phải cuối -> thêm line cách giữa 2 item
                {
                    _holder._vwGroup2.LayoutParameters = new LinearLayout.LayoutParams((int)CmmDroidFunction.ConvertDpToPixel(1.5f, _context), LinearLayout.LayoutParams.MatchParent);
                    _holder._vwItemSpacing.Visibility = ViewStates.Visible;
                }

                if (/*_isLastChild == true*/ groupPosition == _lstGroupTask.Count - 1) // Item của group cuối -> ẩn line ngoài đi
                {
                    _holder._vwGroup1.Visibility = ViewStates.Gone;
                }
                else
                {
                    _holder._vwGroup1.Visibility = ViewStates.Visible;
                }

                #endregion
                SetDataItemTaskToView(_currentTaskItem, _mainAct, _context,
                   /* Line 1 */ _holder._tvTitle, _holder._imgAvatar, _holder._tvName,
                   /* Line 2 */ _holder._tvDate,
                   /* Line 3 */ _holder._tvPosition, _holder._tvAction);

                return convertView;
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
                ComponentTaskListChild_Holder_Lv1 _holder;
                BeanExpandTask _currentItem = _lstGroupTask[groupPosition];
                BeanTask _currentTaskItem = _currentItem.groupItem;

                #region Get View
                if (convertView == null)
                {
                    convertView = LayoutInflater.From(_context).Inflate(Resource.Layout.ItemControlTaskList_Child_Lv1, null);
                    _holder = new ComponentTaskListChild_Holder_Lv1
                    {
                        _lnContentClick = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlTaskList_Child_Lv1_ContentClick),
                        _vwMarginTop = convertView.FindViewById<View>(Resource.Id.vw_ItemControlTaskList_Child_Lv1_MarginTop),
                        _vwGroupHorizon = convertView.FindViewById<View>(Resource.Id.vw_ItemControlTaskList_Child_Lv1_GroupHorizon),
                        _lnGroup1 = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlTaskList_Child_Lv1_Group1),
                        _vwGroup1 = convertView.FindViewById<View>(Resource.Id.vw_ItemControlTaskList_Child_Lv1_Group1),
                        _lnGroup2 = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlTaskList_Child_Lv1_Group2),
                        _vwGroup2 = convertView.FindViewById<View>(Resource.Id.vw_ItemControlTaskList_Child_Lv1_Group2),
                        _imgGroup2 = convertView.FindViewById<ImageView>(Resource.Id.img_ItemControlTaskList_Child_Lv1_Group2),
                        //_relaGroup = convertView.FindViewById<RelativeLayout>(Resource.Id.rela_ItemControlTaskList_Child_Group),
                        _tvTitle = convertView.FindViewById<TextView>(Resource.Id.tv_ItemControlTaskList_Child_Lv1_Title),
                        _imgAvatar = convertView.FindViewById<CircleImageView>(Resource.Id.img_ItemControlTaskList_Child_Lv1_Avata),
                        _tvName = convertView.FindViewById<TextView>(Resource.Id.tv_ItemControlTaskList_Child_Lv1_Name),
                        _tvDate = convertView.FindViewById<TextView>(Resource.Id.tv_ItemControlTaskList_Child_Lv1_Date),
                        _tvPosition = convertView.FindViewById<TextView>(Resource.Id.tv_ItemControlTaskList_Child_Lv1_Position),
                        _tvAction = convertView.FindViewById<TextView>(Resource.Id.tv_ItemControlTaskList_Child_Lv1_Action),
                    };
                    convertView.Tag = _holder;

                    _holder._lnContentClick.Click += (sender, e) =>
                    {
                        //MinionActionCore.OnTaskListItemClick(null, new MinionActionCore.TaskListItemClick(1, _currentItem));

                        if (_adapterParent != null)
                        {
                            _adapterParent.OnItemClickDetail(new MinionActionCore.TaskListItemClick(1, _currentItem));
                        }

                    };

                }
                else
                {
                    _holder = (ComponentTaskListChild_Holder_Lv1)convertView.Tag;
                }
                #endregion

                #region View Rule

                if (isExpanded == true)
                {
                    _holder._vwGroup2.Visibility = ViewStates.Visible;
                    _holder._imgGroup2.SetImageResource(Resource.Drawable.icon_ver2_show);
                }
                else
                {
                    _holder._vwGroup2.Visibility = ViewStates.Invisible;
                    _holder._imgGroup2.SetImageResource(Resource.Drawable.icon_ver2_close);
                }

                if (_currentItem.lstChild != null && _currentItem.lstChild.Count > 0) // còn child -> xử lý hiện group
                {
                    _holder._imgGroup2.Visibility = ViewStates.Visible;
                    _holder._lnGroup2.Visibility = ViewStates.Visible;
                    _holder._vwGroupHorizon.LayoutParameters = new LinearLayout.LayoutParams((int)CmmDroidFunction.ConvertDpToPixel(10, _context), (int)CmmDroidFunction.ConvertDpToPixel(1.5f, _context));
                }
                else
                {
                    _holder._imgGroup2.Visibility = ViewStates.Gone;
                    _holder._lnGroup2.Visibility = ViewStates.Gone;
                    _holder._vwGroupHorizon.LayoutParameters = new LinearLayout.LayoutParams((int)CmmDroidFunction.ConvertDpToPixel(30, _context), (int)CmmDroidFunction.ConvertDpToPixel(1.5f, _context));
                }

                if (groupPosition == _lstGroupTask.Count - 1) // Ẩn line dọc Item của cuối cùng đi
                {
                    _holder._vwGroup1.LayoutParameters = new LinearLayout.LayoutParams((int)CmmDroidFunction.ConvertDpToPixel(1.5f, _context), (int)CmmDroidFunction.ConvertDpToPixel(17, _context));
                }
                else
                {
                    _holder._vwGroup1.LayoutParameters = new LinearLayout.LayoutParams((int)CmmDroidFunction.ConvertDpToPixel(1.5f, _context), LinearLayout.LayoutParams.MatchParent);
                }

                #endregion

                SetDataItemTaskToView(_currentTaskItem, _mainAct, _context,
                   /* Line 1 */ _holder._tvTitle, _holder._imgAvatar, _holder._tvName,
                   /* Line 2 */ _holder._tvDate,
                   /* Line 3 */ _holder._tvPosition, _holder._tvAction);

                return convertView;
            }

            public override bool IsChildSelectable(int groupPosition, int childPosition)
            {
                return true;
            }

            public class ComponentTaskListChild_Holder_Lv1 : Java.Lang.Object
            {
                public LinearLayout _lnContentClick { get; set; }
                public View _vwMarginTop { get; set; }
                public View _vwGroupHorizon { get; set; }
                public LinearLayout _lnGroup1 { get; set; }
                public View _vwGroup1 { get; set; }
                public LinearLayout _lnGroup2 { get; set; }
                public View _vwGroup2 { get; set; }
                public ImageView _imgGroup2 { get; set; }

                // ------------------------------------------------------
                public RelativeLayout _relaGroup { get; set; }
                public TextView _tvTitle { get; set; }
                public CircleImageView _imgAvatar { get; set; }
                public TextView _tvName { get; set; }
                public TextView _tvDate { get; set; }
                public TextView _tvPosition { get; set; }
                public TextView _tvAction { get; set; }
            }
            public class ComponentTaskListChild_Holder_Lv2 : Java.Lang.Object
            {
                public View _vwMarginTop { get; set; }
                public View _vwGroupHorizon { get; set; }
                public LinearLayout _lnGroup1 { get; set; }
                public View _vwGroup1 { get; set; }
                public LinearLayout _lnGroup2 { get; set; }
                public View _vwGroup2 { get; set; }
                public ImageView _imgGroup2 { get; set; }

                // ------------------------------------------------------
                public RelativeLayout _relaGroup { get; set; }
                public TextView _tvTitle { get; set; }
                public CircleImageView _imgAvatar { get; set; }
                public TextView _tvName { get; set; }
                public TextView _tvDate { get; set; }
                public TextView _tvPosition { get; set; }
                public TextView _tvAction { get; set; }
                public View _vwItemSpacing { get; set; }
            }
        }

        public static void SetDataItemTaskToView(BeanTask _currentTaskItem, Activity _mainAct, Context _context,
            /* Line 1 */ TextView _tvTitle, CircleImageView _imgAvatar, TextView _tvName,
            /* Line 2 */ TextView _tvDate,
            /* Line 3 */ TextView _tvPosition, TextView _tvAction)
        {
            ControllerDetailCreateTask CTRLCreateTask = new ControllerDetailCreateTask();
            ControllerHomePage CTRLHomePage = new ControllerHomePage();
            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                BeanUser _userItem = null;

                #region Line 1
                if (!String.IsNullOrEmpty(_currentTaskItem.Title))
                    _tvTitle.Text = _currentTaskItem.Title;
                else
                    _tvTitle.Text = "";

                if (String.IsNullOrEmpty(_currentTaskItem.AssignedId)) // nếu rỗng là phân công cho group
                {
                    // ---- Set Avatar ----
                    _imgAvatar.SetImageResource(Resource.Drawable.icon_ver2_group);

                    // ---- Set Name ----
                    if (!String.IsNullOrEmpty(_currentTaskItem.AssignedName)) // group -> gán tên vào
                        _tvName.Text = _currentTaskItem.AssignedName;
                    else
                        _tvName.Text = "";
                }
                else // phân công User
                {
                    // ---- Query Data ----
                    List<BeanUser> _lstUser = conn.Query<BeanUser>(String.Format("SELECT FullName, ImagePath, Position FROM BeanUser WHERE ID = '{0}' LIMIT 1 OFFSET 0", _currentTaskItem.AssignedId.ToString()));

                    // ---- Set Avatar ----
                    if (_lstUser != null && _lstUser.Count > 0)
                    {
                        _userItem = _lstUser[0];

                        if (_userItem != null && !String.IsNullOrEmpty(_userItem.ImagePath))
                        {
                            //CmmDroidFunction.SetAvataByImagePath(_mainAct, _userItem.ImagePath, _imgAvatar, 50);
                            CmmDroidFunction.SetContentToImageView(_mainAct, _imgAvatar, _lstUser[0].ImagePath, 50);
                        }
                        else
                        {
                            _imgAvatar.SetImageResource(Resource.Drawable.icon_avatar64);
                        }

                        // ---- Set Name ----
                        if (!String.IsNullOrEmpty(_currentTaskItem.AssignedName))
                        {
                            if (_currentTaskItem.AssignedName.Contains("+")) // nhiều người -> load đúng lên
                                _tvName.Text = _currentTaskItem.AssignedName;
                            else // 1 người hoặc 1 group -> load bean User
                            {
                                if (_userItem != null && !String.IsNullOrEmpty(_userItem.FullName))
                                    _tvName.Text = _userItem.FullName;
                            }
                        }
                        else
                            _tvName.Text = "";
                    }

                }
                #endregion

                #region Line 2

                if (_currentTaskItem.DueDate.HasValue)
                {
                    _tvDate.Text = CTRLCreateTask.GetFormatDateLang(_currentTaskItem.DueDate.Value);
                    _tvDate.SetTextColor(CTRLHomePage.GetColorByDueDate(_context, _currentTaskItem.DueDate.Value));
                }
                else
                    _tvDate.Text = "";
                #endregion

                #region Line 3
                if (_userItem != null && !String.IsNullOrEmpty(_userItem.Position))
                    _tvPosition.Text = _userItem.Position;
                else
                    _tvPosition.Text = "";

                //if (String.IsNullOrEmpty(_currentTaskItem.AssignedPosition))
                //    _holder._tvPosition.Text = _currentTaskItem.AssignedPosition;
                //else
                //    _holder._tvPosition.Text = "";

                _tvAction.Text = CTRLCreateTask.GetStatusNameByID(_currentTaskItem.Status);
                _tvAction.BackgroundTintList = ColorStateList.ValueOf(CTRLCreateTask.GetStatusColorByID(_context, _currentTaskItem.Status));
                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("", "SetDataItemTaskToView", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

    }
}