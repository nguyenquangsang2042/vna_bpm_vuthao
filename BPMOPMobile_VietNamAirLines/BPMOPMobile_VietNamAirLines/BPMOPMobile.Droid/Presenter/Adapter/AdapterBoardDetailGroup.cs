using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Fragment;
using Refractored.Controls;
using static Android.Views.GestureDetector;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    public class AdapterBoardDetailGroup : RecyclerView.Adapter, Listener
    {
        private MainActivity _mainAct;
        private Context _context;
        public event EventHandler<BeanWorkflowItem> CustomItemClick;
        private ControllerBase CTRLBase = new ControllerBase();
        ////private AdapterBoardDetailGroup_Child _adapterChild;
        private FragmentBoardDetailGroup_customlibrary _fragmentBoardDetailGroup;
        private RecyclerView _recyParent;
        public static List<int> _lstRecyClerViewID = new List<int>();

        private List<BeanBoardStepDefine> _lstStepDefine = new List<BeanBoardStepDefine>();

        public AdapterBoardDetailGroup(MainActivity _mainAct, Context _context, FragmentBoardDetailGroup_customlibrary _fragmentBoardDetailGroup, RecyclerView _recyParent, List<BeanBoardStepDefine> _lstStepDefine)
        {
            this._mainAct = _mainAct;
            this._context = _context;
            this._fragmentBoardDetailGroup = _fragmentBoardDetailGroup;
            this._recyParent = _recyParent;
            this._lstStepDefine = _lstStepDefine;
            _lstRecyClerViewID = new List<int>();
        }

        public void UpdateListData(List<BeanBoardStepDefine> _lstStepDefine)
        {
            this._lstStepDefine = _lstStepDefine;
        }
        public List<BeanBoardStepDefine> GetListData()
        {
            return _lstStepDefine;
        }
        public int GetListDataCount()
        {
            return _lstStepDefine.Count;
        }

        private void OnItemClick(int position)
        {
            if (CustomItemClick != null)
            {
                ////CustomItemClick(this, _lstWorkflowItem[position]);
            }
        }
        public override int ItemCount => _lstStepDefine.Count;
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemBoardDetailGroup, parent, false);
            AdapterBoardDetailGroupViewHolder holder = new AdapterBoardDetailGroupViewHolder(itemView, OnItemClick);
            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AdapterBoardDetailGroupViewHolder _holder = holder as AdapterBoardDetailGroupViewHolder;
            #region Get View
            DisplayMetrics displaymetrics = _mainAct.Resources.DisplayMetrics;
            int _widthItem = (displaymetrics.WidthPixels / 10) * 8; // Lấy 9/10 widthScreen
            _holder._lnAll.LayoutParameters = new ViewGroup.LayoutParams(_widthItem, ViewGroup.LayoutParams.MatchParent);
            _holder._recyChild.Id = position; //View.GenerateViewId(); // Gắn ID cho Event Drag and Drop

            if (_lstRecyClerViewID.IndexOf(_holder._recyChild.Id) == -1) // chưa có mới add
            {
                _lstRecyClerViewID.Add(_holder._recyChild.Id);
            }

            #endregion

            #region Handle Data
            BeanWorkflowStepDefine _currentStepDefineItem = _lstStepDefine[position].itemStepDefine;
            if (!String.IsNullOrEmpty(_currentStepDefineItem.Title))
                _holder._tvName.Text = _currentStepDefineItem.Title;
            else
                _holder._tvName.Text = "";

            AdapterBoardDetailGroup_Child _adapterChild;
            if (position >= (_lstStepDefine.Count - 2)) // Disable 2 list phê duyệt và từ chối
            {
                _adapterChild = new AdapterBoardDetailGroup_Child(_mainAct, _context, _fragmentBoardDetailGroup, _recyParent, _lstStepDefine[position].lstWorkflowItem.ToList(), this, true);
            }
            else
            {
                _adapterChild = new AdapterBoardDetailGroup_Child(_mainAct, _context, _fragmentBoardDetailGroup, _recyParent, _lstStepDefine[position].lstWorkflowItem.ToList(), this, false);
            }

            ////if (_holder._recyChild.GetAdapter() != null) // Đã có adapter rồi
            ////{
            ////    AdapterBoardDetailGroup_Child _tempAdapter = (AdapterBoardDetailGroup_Child)_holder._recyChild.GetAdapter();
            ////    _tempAdapter.UpdateListDataSouce(_lstStepDefine[position].lstWorkflowItem);
            ////    _tempAdapter.NotifyDataSetChanged();
            ////}
            ////else
            ////{
            ////    _holder._recyChild.SetAdapter(_adapterChild);
            ////    _holder._recyChild.SetLayoutManager(new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical));
            ////    _holder._recyChild.SetOnDragListener(_adapterChild.getDragInstance());
            ////}

            _adapterChild.CustomItemClick -= Click_ItemListChild;
            _adapterChild.CustomItemClick += Click_ItemListChild;
            _holder._recyChild.SetAdapter(_adapterChild);
            ////_holder._recyChild.SetLayoutManager(new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical));
            _holder._recyChild.SetLayoutManager(new LinearLayoutManager(_context, LinearLayoutManager.Vertical, false));
            _holder._recyChild.SetOnDragListener(_adapterChild.getDragInstance());
            #endregion
        }
        private void Click_ItemListChild(object sender, BeanWorkflowItem e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(e, null, "FragmentBoardDetailGroup");
                    _mainAct.AddFragment(_mainAct.SupportFragmentManager, detailWorkFlow, "FragmentBoardDetailGroup", 0);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemListChild", ex);
#endif
            }
        }
        public void SetVisibleListData(RecyclerView _recy, bool _IsVisible)
        {
            //_recy.Visibility = _IsVisible ? ViewStates.Visible : ViewStates.Invisible;
        }
        public class AdapterBoardDetailGroupViewHolder : RecyclerView.ViewHolder
        {
            public LinearLayout _lnAll { get; set; }
            public TextView _tvName { get; set; }
            public TextView _tvNoData { get; set; }
            public RecyclerView _recyChild { get; set; }
            public LinearLayout _lnNoData { get; set; }
            public AdapterBoardDetailGroupViewHolder(View itemview, Action<int> listener) : base(itemview)
            {
                _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemBoardDetailGroup_All);
                _tvName = itemview.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroup_Title);
                _recyChild = itemview.FindViewById<RecyclerView>(Resource.Id.recy_ItemBoardDetailGroup_Child);
                _lnNoData = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemBoardDetailGroup_Child_NoData);
                _tvNoData = itemview.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroup_Child_NoData);
                //_recyChild.AddOnItemTouchListener(this); // Để chặn On Touch ở ngoài
                //_imgDelete.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }


        #region Adapter List RecyclerView Child
        public class AdapterBoardDetailGroup_Child : RecyclerView.Adapter
        {
            public MainActivity _mainAct;
            public Context _context;
            private List<BeanWorkflowItem> _lstData = new List<BeanWorkflowItem>();
            public event EventHandler<BeanWorkflowItem> CustomItemClick;
            public event EventHandler<BeanWorkflowItem> CustomItemLongClick;
            private ControllerBoard CTRLBoard = new ControllerBoard();
            private FragmentBoardDetailGroup_customlibrary _fragmentBoardDetailGroup;
            private bool _IsDisableDrag = false;
            private Listener listener;
            public static View _OnTouchView; // View call khi OnTouch()
            private RecyclerView _recyParent; // List cha chứa các List con
            public List<BeanWorkflowItem> GetListDataSouce()
            {
                return _lstData;
            }
            public void UpdateListDataSouce(List<BeanWorkflowItem> _lstData)
            {
                this._lstData = _lstData.ToList();
            }
            public CustomDragAndDropListener getDragInstance()
            {
                if (listener != null)
                {
                    return new CustomDragAndDropListener(_fragmentBoardDetailGroup, _recyParent, listener);
                }
                else
                {
                    Log.Error("ListAdapter", "Listener wasn't initialized!");
                    return null;
                }
            }
            private void OnItemClick(int position)
            {
                try
                {
                    if (CmmDroidFunction.PreventMultipleClick() == true)
                    {
                        FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(_lstData[position], null, "FragmentBoardDetailGroup");
                        _mainAct.AddFragment(_mainAct.SupportFragmentManager, detailWorkFlow, "FragmentBoardDetailGroup", 0);
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnItemClick", ex);
#endif
                }
            }
            private void OnItemLongClick(View tempOnTouchView)
            {
                try
                {
                    if (_IsDisableDrag == false)
                    {
                        CmmDroidFunction.ShowVibrateEvent();
                        _OnTouchView = tempOnTouchView;
                        //_OnTouchView.Alpha = (float)0.5;

                        ClipData data = ClipData.NewPlainText("", "");
                        View.DragShadowBuilder shadowBuilder = new View.DragShadowBuilder(_OnTouchView);

                        if ((int)Build.VERSION.SdkInt >= 24)
                        {
                            _OnTouchView.StartDragAndDrop(data, shadowBuilder, _OnTouchView, (int)Android.Views.DragFlags.Opaque);
                        }
                        else
                        {
                            ////_OnTouchView.StartDrag(data, shadowBuilder, _OnTouchView, (int)Android.Views.DragFlags.Opaque);
                        }

                        LinearLayout _lnOverride = _OnTouchView.FindViewById<LinearLayout>(Resource.Id.ln_ItemBoardDetailGroup_RecyChild_Override);
                        _lnOverride.Visibility = ViewStates.Visible;
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnItemLongClick", ex);
#endif
                }
            }
            public AdapterBoardDetailGroup_Child(MainActivity _mainAct, Context _context, FragmentBoardDetailGroup_customlibrary _fragmentBoardDetailGroup, RecyclerView _recyParent, List<BeanWorkflowItem> _lstData, Listener listener, bool _IsDisableDrag)
            {
                this._mainAct = _mainAct;
                this._context = _context;
                this._fragmentBoardDetailGroup = _fragmentBoardDetailGroup;
                this._lstData = _lstData;
                this._recyParent = _recyParent;
                this.listener = listener;
                this._IsDisableDrag = _IsDisableDrag;
            }
            public override int ItemCount
            {
                get
                {
                    if (_lstData != null && _lstData.Count > 0)
                        return _lstData.Count;
                    else
                        return 0;
                }
            }
            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemBoardDetailGroup_RecyChild, parent, false);
                AdapterBoardDetailGroup_ChildViewHolder holder = new AdapterBoardDetailGroup_ChildViewHolder(itemView, OnItemClick, OnItemLongClick);
                return holder;
            }
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                AdapterBoardDetailGroup_ChildViewHolder _holder = holder as AdapterBoardDetailGroup_ChildViewHolder;

                //_holder._lnAll.SetOnTouchListener(this);
                _holder._lnAll.SetOnDragListener(new CustomDragAndDropListener(_fragmentBoardDetailGroup, _recyParent, listener));
                _holder._lnAll.Tag = position; // Gắn Tag để lát biết drag item nào

                ////if (_lstData[position].ActionStatusID == CTRLBoard.ApprovedID) // phê duyệt -> xanh
                ////    _holder._cardAll.SetCardBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clStatusGreen)));
                ////else if (_lstData[position].ActionStatusID == CTRLBoard.RejectedID) // Từ chối -> Đỏ
                ////    _holder._cardAll.SetCardBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clStatusRed)));
                ////else
                ////    _holder._cardAll.SetCardBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));

                #region Line 1
                if (_lstData[position].Created.HasValue)
                {
                    _holder._tvDate.Text = CTRLBoard.GetFormatDateLang(_lstData[position].Created.Value);
                }
                else
                    _holder._tvDate.Text = "";

                #endregion

                #region Line 2

                if (!String.IsNullOrEmpty(_lstData[position].AssignedTo))
                {
                    string[] valueSearch = _lstData[position].AssignedTo.ToLowerInvariant().Split(",");
                    CmmDroidFunction.SetAvataByBeanUser(_mainAct, _context, valueSearch[0], "ID", _holder._imgAvatar, _holder._tvAvatar, 30);
                }
                else
                {
                    _holder._tvAvatar.Visibility = ViewStates.Invisible;
                    _holder._imgAvatar.Visibility = ViewStates.Invisible;
                }

                if (!String.IsNullOrEmpty(_lstData[position].Content))
                    _holder._tvTitle.Text = _lstData[position].Content;
                else
                    _holder._tvTitle.Visibility = ViewStates.Invisible;

                #endregion

                #region Line 3

                _holder._tvCountAttach.Text = "0";
                _holder._tvCountComment.Text = "0";

                if (!String.IsNullOrEmpty(_lstData[position].AssignedTo))
                {
                    string[] valueSearch = _lstData[position].AssignedTo.ToLowerInvariant().Split(",");
                    CmmDroidFunction.SetAvataByBeanUser(_mainAct, _context, valueSearch[0], "ID", _holder._imgAvatar2, _holder._tvAvatar2, 30);

                    if (valueSearch.Length > 1)
                        _holder._tvCountPeople.Text = "+" + (valueSearch.Length - 1).ToString();
                    else
                        _holder._tvCountPeople.Text = "";
                }
                else
                {
                    _holder._tvAvatar2.Visibility = ViewStates.Invisible;
                    _holder._imgAvatar2.Visibility = ViewStates.Invisible;
                    _holder._tvCountPeople.Text = "";
                }


                #endregion
            }
            public class AdapterBoardDetailGroup_ChildViewHolder : RecyclerView.ViewHolder
            {
                public LinearLayout _lnAll { get; set; }
                public LinearLayout _lnData { get; set; }
                public CardView _cardAll { get; set; }
                public TextView _tvDate { get; set; }
                public ImageView _imgSubCribe { get; set; }
                public RelativeLayout _relaAvatar { get; set; }
                public TextView _tvAvatar { get; set; }
                public CircleImageView _imgAvatar { get; set; }
                public TextView _tvTitle { get; set; }
                public TextView _tvCountAttach { get; set; }
                public TextView _tvCountComment { get; set; }
                public RelativeLayout _relaAvatar2 { get; set; }
                public TextView _tvAvatar2 { get; set; }
                public CircleImageView _imgAvatar2 { get; set; }
                public TextView _tvCountPeople { get; set; }

                public AdapterBoardDetailGroup_ChildViewHolder(View itemview, Action<int> listener, Action<View> listenerLong) : base(itemview)
                {
                    _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemBoardDetailGroup_RecyChild_All);
                    _lnData = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemBoardDetailGroup_RecyChild_Data);
                    _cardAll = itemview.FindViewById<CardView>(Resource.Id.card_ItemBoardDetailGroup_RecyChild_All);
                    _tvDate = itemview.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroup_RecyChild_Date);
                    _imgSubCribe = itemview.FindViewById<ImageView>(Resource.Id.img_ItemBoardDetailGroup_RecyChild_Subcribe);
                    _relaAvatar = itemview.FindViewById<RelativeLayout>(Resource.Id.rela_ItemBoardDetailGroup_RecyChild_Avatar);
                    _tvAvatar = itemview.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroup_RecyChild_Avatar);
                    _imgAvatar = itemview.FindViewById<CircleImageView>(Resource.Id.img_ItemBoardDetailGroup_RecyChild_Avatar);
                    _tvTitle = itemview.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroup_RecyChild_Title);
                    _tvCountAttach = itemview.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroup_RecyChild_CountAttach);
                    _tvCountComment = itemview.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroup_RecyChild_CountComment);
                    _relaAvatar2 = itemview.FindViewById<RelativeLayout>(Resource.Id.rela_ItemBoardDetailGroup_RecyChild_Avatar2);
                    _tvAvatar2 = itemview.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroup_RecyChild_Avatar2);
                    _imgAvatar2 = itemview.FindViewById<CircleImageView>(Resource.Id.img_ItemBoardDetailGroup_RecyChild_Avatar2);
                    _tvCountPeople = itemview.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroup_RecyChild_CountPeople);

                    //_lnAll.Click += (sender, e) => listener(base.LayoutPosition);
                    //_imgSubCribe.Click += (sender, e) => listener(base.LayoutPosition);
                    //_lnAll.LongClick += (sender, e) => listenerLong(_lnAll);

                    _cardAll.Click += (sender, e) => listener(base.LayoutPosition);
                    _cardAll.LongClick += (sender, e) => listenerLong(_lnAll);
                }
            }
        }
        #endregion

        #region Custom Drag And Drop
        public class CustomDragAndDropListener : Java.Lang.Object, View.IOnDragListener
        {
            private bool isDropped = false;
            private Listener listener;
            private RecyclerView _recyParent; // List cha
            private RecyclerViewDragScrollHelper _recyclerDragScroller = new RecyclerViewDragScrollHelper();
            private FragmentBoardDetailGroup_customlibrary _fragmentBoardDetailGroup;
            private int initX = 0;
            public CustomDragAndDropListener(FragmentBoardDetailGroup_customlibrary _fragmentBoardDetailGroup, RecyclerView _recyParent, Listener listener)
            {
                this._recyParent = _recyParent;
                this.listener = listener;
                this._fragmentBoardDetailGroup = _fragmentBoardDetailGroup;
                _recyclerDragScroller = new RecyclerViewDragScrollHelper(_recyParent);
            }

            public bool OnDrag(View v, DragEvent e)
            {
                int viewId = v.Id;
                //_recyclerDragScroller.HandleDrag(v, e);
                //return true;

                switch (e.Action)
                {
                    case DragAction.Drop:
                        {
                            isDropped = true;
                            int positionTarget = -1;
                            View viewSource = (View)e.LocalState;

                            const int _lnItemAll = Resource.Id.ln_ItemBoardDetailGroup_RecyChild_All;

                            RecyclerView _Recytarget;
                            if (AdapterBoardDetailGroup._lstRecyClerViewID.IndexOf(viewId) != -1) // Kéo thả vào View Blank của RecyclerView
                            {
                                _Recytarget = v.RootView.FindViewById<RecyclerView>(viewId);
                            }
                            else // Kéo thả vào View của item khác
                            {
                                switch (viewId)
                                {
                                    case _lnItemAll:
                                        //_Recytarget = (RecyclerView)v.RootView.FindViewById(Resource.Id.recy_ItemBoardDetailGroup_Child);
                                        _Recytarget = (RecyclerView)v.Parent;
                                        positionTarget = (int)v.Tag;
                                        break;
                                    default:
                                        _Recytarget = (RecyclerView)v.Parent;
                                        positionTarget = (int)v.Tag;
                                        break;
                                }
                            }
                            if (viewSource != null)
                            {
                                try
                                {
                                    #region Call View RecyclerView Source +  RecyclerView Target

                                    //RecyclerView Source
                                    RecyclerView _RecySource = (RecyclerView)viewSource.Parent;
                                    AdapterBoardDetailGroup_Child adapterSource = (AdapterBoardDetailGroup_Child)_RecySource.GetAdapter();
                                    int positionSource = (int)viewSource.Tag;
                                    int sourceId = _RecySource.Id;

                                    AdapterBoardDetailGroup_Child adapterTarget = (AdapterBoardDetailGroup_Child)_Recytarget.GetAdapter();
                                    #endregion

                                    #region Handle Data
                                    if (sourceId == _Recytarget.Id) // Target trùng 
                                    {
                                        break;
                                    }
                                    else if (Math.Abs(sourceId - _Recytarget.Id) > 1) // Chỉ chi Action 2 list liền kề
                                    {
                                        CmmDroidFunction.ShowAlertDialog(adapterSource._mainAct, CmmFunction.GetTitle("BANANA", "Chỉ được thao tác trên hai bước liền kề"),
                                           CmmFunction.GetTitle("BANANA", "Can only do action on two adjacent steps"));
                                        break;
                                    }
                                    else
                                    {
                                        _RecySource.Animation = AnimationUtils.LoadAnimation(adapterSource._context, Resource.Animation.anim_fade_in);
                                        _Recytarget.Animation = AnimationUtils.LoadAnimation(adapterSource._context, Resource.Animation.anim_fade_in);

                                        // Remove Item đã Bị Drag ra
                                        List<BeanWorkflowItem> listSource = adapterSource.GetListDataSouce().ToList();
                                        List<BeanWorkflowItem> listTarget = adapterTarget.GetListDataSouce().ToList();
                                        BeanWorkflowItem _itemAction = listSource[positionSource];

                                        if (_RecySource.Id < _Recytarget.Id) // Drag qua phải -> Action đồng ý
                                        {
                                            _fragmentBoardDetailGroup.SetDragAction_NextPrevious(_RecySource, _Recytarget, positionSource, true);

                                            //if (_fragmentBoardDetailGroup.SetDragAction_Next(_itemAction) == true) // thành công
                                            //{
                                            //    AdapterBoardDetailGroup _parentAdapter = (AdapterBoardDetailGroup)_recyParent.GetAdapter(); // Cập nhật list parent 
                                            //    List<BeanBoardStepDefine> _lstStepDefine = _parentAdapter.GetListData();

                                            //    listSource.RemoveAt(positionSource);
                                            //    listTarget.Add(_itemAction);

                                            //    _lstStepDefine[_RecySource.Id].lstWorkflowItem = listSource; // Id start từ 1
                                            //    _lstStepDefine[_Recytarget.Id].lstWorkflowItem = listTarget;  // Id start từ 1
                                            //    _parentAdapter.UpdateListData(_lstStepDefine);
                                            //    _parentAdapter.NotifyDataSetChanged();
                                            //}
                                        }
                                        else
                                        {
                                            _fragmentBoardDetailGroup.SetDragAction_NextPrevious(_RecySource, _Recytarget, positionSource, false);
                                        }

                                        //listSource.RemoveAt(positionSource);
                                        //////adapterSource.UpdateListDataSouce(listSource);
                                        //////adapterSource.NotifyDataSetChanged();

                                        //if (listSource == null || listSource.Count == 0)
                                        //    listener.SetVisibleListData(_RecySource, false);
                                        //else
                                        //    listener.SetVisibleListData(_RecySource, true);


                                        //customListTarget.Add(_itemAction); // Add thêm item dc Drag vào
                                        //////adapterTarget.UpdateListDataSouce(customListTarget);
                                        //////adapterTarget.NotifyDataSetChanged();

                                        ////Update RecyclerView Parent


                                        //if (customListTarget == null || customListTarget.Count == 0)
                                        //listener.SetVisibleListData(_Recytarget, false);
                                        //else
                                        //    listener.SetVisibleListData(_Recytarget, true);

                                    }
                                    #endregion
                                }
                                catch (Exception ex)
                                {
#if DEBUG
                                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnDrag", ex);
#endif
                                    break;
                                }
                            }
                            break;
                        }
                    case DragAction.Location: // Keo ngang
                        {
                            try
                            {
                                //if (_recyParent != null)
                                //{
                                //    if (CmmDroidFunction.PreventMultipleClick(2000) == true)
                                //    {
                                //        int positionTarget = -1;
                                //        View viewSource = (View)e.LocalState;
                                //        const int _lnItemAll = Resource.Id.ln_ItemBoardDetailGroup_RecyChild_All;
                                //        RecyclerView _RecySource = (RecyclerView)viewSource.Parent;
                                //        RecyclerView _Recytarget;
                                //        switch (viewId)
                                //        {
                                //            case _lnItemAll:
                                //                _Recytarget = (RecyclerView)v.Parent;
                                //                break;
                                //            default:
                                //                _Recytarget = (RecyclerView)v.Parent;
                                //                break;
                                //        }


                                //        StaggeredGridLayoutManager _temp = (StaggeredGridLayoutManager)_recyParent.GetLayoutManager();

                                //        int[] position = new int[5];
                                //        if (_Recytarget.Id < _RecySource.Id) // Qua trái
                                //        {
                                //            _temp.FindFirstVisibleItemPositions(position);
                                //            _recyParent.SmoothScrollToPosition(position[0] - 1);
                                //        }
                                //        else // Qua phải
                                //        {
                                //            _temp.FindLastVisibleItemPositions(position);
                                //            _recyParent.SmoothScrollToPosition(position[0] + 1);
                                //        }
                                //    }
                                //    break;
                                //}
                            }
                            catch (Exception)
                            {

                            }
                            break;
                        }
                    case DragAction.Ended: // Thả ra giữa chừng
                        {
                            if (AdapterBoardDetailGroup_Child._OnTouchView != null)
                            {
                                //AdapterBoardDetailGroup_Child._OnTouchView.Alpha = 1; // Bỏ làm mờ item đã bị thao tác trên Source List
                                LinearLayout _lnOverride = AdapterBoardDetailGroup_Child._OnTouchView.FindViewById<LinearLayout>(Resource.Id.ln_ItemBoardDetailGroup_RecyChild_Override);
                                _lnOverride.Visibility = ViewStates.Gone;
                            }
                            break;
                        }
                }

                if (!isDropped && e.LocalState != null)
                {
                    ((View)e.LocalState).Visibility = ViewStates.Visible;
                }
                return true;
            }
        }
        #endregion

        #region Action

        #endregion
    }
    public interface Listener
    {
        void SetVisibleListData(RecyclerView _recy, bool _IsVisible);
    }


}