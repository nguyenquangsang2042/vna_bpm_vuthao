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
using IO.SuperCharge.ShimmerLayoutLib;
using Newtonsoft.Json;
using Refractored.Controls;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ComponentFlowRelated : ComponentBase
    {
        private Activity _mainAct { get; set; }
        private Context _context { get; set; }
        private LinearLayout _parentView { get; set; }
        private CardView _cardViewRecycler { get; set; }
        private LinearLayout _lnTitle { get; set; }
        private LinearLayout _lnShimmer { get; set; }
        private TextView _tvTitle { get; set; }
        private RecyclerView _recyAttachment { get; set; }
        private ViewElement _element { get; set; }
        private AdapterComponentFlowRelated _adapterFlowRelated; // Adapter của Recy List file
        private List<BeanWorkFlowRelated> _lstWorkflowItem = new List<BeanWorkFlowRelated>();
        private BeanWorkflowItem _currentWorkflowItem = new BeanWorkflowItem();
        private BeanNotify _currentNotifyItem = new BeanNotify();
        private ControllerBase CTRLBase = new ControllerBase();

        public ComponentFlowRelated(Activity _mainAct, LinearLayout _parentView, List<BeanWorkFlowRelated> _lstWorkflowItem, BeanWorkflowItem _currentWorkflowItem, BeanNotify _currentNotifyItem)
        {
            this._mainAct = _mainAct;
            this._parentView = _parentView;
            this._lstWorkflowItem = _lstWorkflowItem;
            this._currentWorkflowItem = _currentWorkflowItem;
            this._currentNotifyItem = _currentNotifyItem;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();
            _recyAttachment = new RecyclerView(_mainAct);
            _cardViewRecycler = new CardView(_mainAct);
            _lnShimmer = new LinearLayout(_mainAct);
            _lnTitle = new LinearLayout(_mainAct);
            _tvTitle = new TextView(_mainAct);

            _tvTitle.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? CmmFunction.GetTitle("TEXT_WORKFLOW_RELATE", "Quy trình / Công việc liên kết") : CmmFunction.GetTitle("TEXT_WORKFLOW_RELATE", "Workkflow related");
            _tvTitle.SetTextSize(ComplexUnitType.Sp, 12);
            _tvTitle.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
            _tvTitle.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);

            _tvTitle.Ellipsize = TextUtils.TruncateAt.End;
            _tvTitle.Gravity = GravityFlags.Center;

            _cardViewRecycler.UseCompatPadding = true;
            _cardViewRecycler.Radius = 5f;

            _lnShimmer.Orientation = Android.Widget.Orientation.Vertical;


            _recyAttachment.NestedScrollingEnabled = true;
            _lnTitle.Orientation = Android.Widget.Orientation.Vertical;
        }

        public override void InitializeFrameView(LinearLayout frame)
        {
            _context = frame.Context;
            base.InitializeFrameView(frame);

            int _padding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 3, _mainAct.Resources.DisplayMetrics);
            LinearLayout.LayoutParams _paramsTitle = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams _paramsLnRecy = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
            LinearLayout.LayoutParams _paramsLnShimmer = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);

            _tvTitle.LayoutParameters = _paramsTitle;

            frame.SetPadding(2 * _padding, 2 * _padding, 2 * _padding, 0);
            _tvTitle.SetPadding(_padding, 2 * _padding, _padding, 4 * _padding);
            _lnShimmer.LayoutParameters = _paramsLnShimmer;
            _recyAttachment.LayoutParameters = _paramsLnRecy;

            _cardViewRecycler.AddView(_lnShimmer);
            _lnTitle.AddView(_tvTitle);
            frame.AddView(_lnTitle);
            frame.AddView(_cardViewRecycler);
        }

        protected virtual void HandleTouchDown(object sender, EventArgs e)
        {
            if (_parentView != null)
            {
                MinionActionCore.OnElementFormClick(null, new MinionActionCore.ElementFormClick(_element));
            }
        }

        public override void SetTitle()
        {
            base.SetTitle();
        }

        public override void SetValue()
        {
            base.SetValue();

            if (_lstWorkflowItem != null && _lstWorkflowItem.Count > 0)
            {

                Action action = new Action(() =>
                {
                    _cardViewRecycler.RemoveAllViews();
                    _cardViewRecycler.AddView(_recyAttachment);

                    _adapterFlowRelated = new AdapterComponentFlowRelated(_mainAct, _context, _lstWorkflowItem, _currentWorkflowItem, _currentNotifyItem);
                    _adapterFlowRelated.CustomItemClick += (sender, e) => { };
                    _recyAttachment.SetLayoutManager(new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical));
                    _recyAttachment.SetAdapter(_adapterFlowRelated);

                    _cardViewRecycler.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_context));

                    //// Nếu là người tạo phiếu cha và phiếu đó là soạn thảo -> mới được xóa
                    //if (_currentWorkflowItem.CreatedBy.Equals(CmmVariable.SysConfig.UserId) && _currentWorkflowItem.Status.ToLowerInvariant().Equals("soạn thảo"))
                    //{
                    //    ////_recyAttachment.Alpha = (float)1;
                    //    ////MySwipeHelper mySwipeHelper = new AdapterComponentFlowRelated_SwipeHelper(null, _context, _recyAttachment, 150, _lstWorkflowItem);
                    //}
                    //else
                    //{
                    //    ////_recyAttachment.Alpha = (float)0.5;
                    //}
                });

                int itemHeight = (int)CmmDroidFunction.ConvertDpToPixel(100, _context); // item set cứng 100dp
                if (_lstWorkflowItem.Count > 3) // nếu > 3 thì limit lại (Item view set cứng 100dp -> (100*3) + 50 để biết là có thể scroll)
                {
                    LayoutInflater _inflater = (LayoutInflater)_mainAct.GetSystemService(Android.Content.Context.LayoutInflaterService);
                    for (int i = 0; i < _lstWorkflowItem.Count; i++)
                    {
                        _lnShimmer.AddView(_inflater.Inflate(Resource.Layout.ItemControlFlowRelated_Shimmer, null));
                    }
                    _cardViewRecycler.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, (itemHeight * _lstWorkflowItem.Count) + 10);
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime * 2);
                }
                else // < 3 không cần shimmer -> load thẳng
                {
                    _cardViewRecycler.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                    new Handler().PostDelayed(action, 0);
                }
            }
        }
    }

    public class AdapterComponentFlowRelated : RecyclerView.Adapter
    {
        public Activity _mainAct;
        public Context _context;
        public List<BeanWorkFlowRelated> _lstWorkflowItem = new List<BeanWorkFlowRelated>();
        public EventHandler<BeanUser> CustomItemClick;
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        private BeanWorkflowItem _currentWorkflowItem = new BeanWorkflowItem();
        private BeanNotify _currentNotifyItem = new BeanNotify();

        //public event EventHandler<BeanWorkFlowRelated> CustomItemClick_RemoveItem;
        private void OnItemClick(int position)
        {
            MinionActionCore.OnFlowRelatedClick_WithInnerAction(null, new MinionActionCore.FlowRelatedClick_WithInnerAction(_lstWorkflowItem, (int)EnumFormControlInnerAction.FlowRelated_InnerActionID.View, position));
        }
        public AdapterComponentFlowRelated(Activity _mainAct, Context _context, List<BeanWorkFlowRelated> _lstWorkflowItem, BeanWorkflowItem _currentWorkflowItem, BeanNotify _currentNotifyItem)
        {
            this._mainAct = _mainAct;
            this._lstWorkflowItem = _lstWorkflowItem;
            this._context = _context;
            this._currentWorkflowItem = _currentWorkflowItem;
            this._currentNotifyItem = _currentNotifyItem;
        }
        public void DeleteSwipedItem(int position)
        {
            //OnItemClick_RemoveItem(_lstWorkflowItem[position]);
        }
        public override int ItemCount => _lstWorkflowItem.Count;
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemControlFlowRelated, parent, false);
            AdapterAdapterComponentFlowRelatedHolder holder = new AdapterAdapterComponentFlowRelatedHolder(itemView, OnItemClick);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AdapterAdapterComponentFlowRelatedHolder vh = holder as AdapterAdapterComponentFlowRelatedHolder;
            SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);

            string currentWorkFlowID = _currentWorkflowItem.ID;
            try
            {
                if (position % 2 == 0)
                    vh._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueNavigation)));
                else
                    vh._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clTransparent)));

                // -- Avatar
                if (!String.IsNullOrEmpty(_lstWorkflowItem[position].CreatedBy))
                {
                    List<BeanUser> _lstUser = conn.Query<BeanUser>(String.Format("SELECT ImagePath FROM BeanUser WHERE FullName = '{0}' LIMIT 1 OFFSET 0", _lstWorkflowItem[position].CreatedBy));
                    CmmDroidFunction.SetContentToImageView(_mainAct, vh._imgAvatar, _lstUser[0].ImagePath, 50);
                }
                else
                {
                    vh._imgAvatar.SetImageResource(Resource.Drawable.icon_avatar64);
                }

                if (currentWorkFlowID == _lstWorkflowItem[position].ItemRLID.ToString())  // binding theo Item
                {
                    vh._tvTitle.Text = _lstWorkflowItem[position].WorkflowContent;

                    vh._tvDescription.Text = _lstWorkflowItem[position].ItemCode;

                    if (_lstWorkflowItem[position].Created.HasValue)
                        vh._tvTime.Text = CTRLHomePage.GetFormatDateLang(_lstWorkflowItem[position].Created.Value);
                    else
                        vh._tvTime.Text = "";

                    if (_lstWorkflowItem[position].StatusWorkflowID.HasValue)
                    {
                        vh._tvStatus.Visibility = ViewStates.Visible;
                        vh._tvStatus.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(CTRLHomePage.GetColorByAppStatus(_context, _lstWorkflowItem[position].StatusWorkflowID.Value));
                        string _query = String.Format("SELECT * FROM BeanAppStatus WHERE ID = {0} LIMIT 1 OFFSET 0", _lstWorkflowItem[position].StatusWorkflowID);
                        List<BeanAppStatus> _result = conn.Query<BeanAppStatus>(_query);

                        if (_result != null && _result.Count > 0)
                            vh._tvStatus.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? _result[0].Title : _result[0].TitleEN;
                        else
                            vh._tvStatus.Text = _lstWorkflowItem[position].StatusWorkflow;
                    }
                    else
                    {
                        vh._tvStatus.Visibility = ViewStates.Invisible;
                    }

                    ////vh._tvStatus.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(CTRLHomePage.GetColorByActionID(_context, _lstWorkflowItem[position].StatusWorkflowID));
                    ////string _query = String.Format("SELECT * FROM BeanWorkflowStatus WHERE ID = {0} LIMIT 1 OFFSET 0", _lstWorkflowItem[position].StatusWorkflowID);
                    ////List<BeanWorkflowStatus> _result = conn.Query<BeanWorkflowStatus>(_query);

                    ////if (_result != null && _result.Count > 0)

                    ////    vh._tvStatus.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? _result[0].Title : _result[0].TitleEN;
                    ////else
                    ////    vh._tvStatus.Text = _lstWorkflowItem[position].StatusWorkflow;
                }
                else if (currentWorkFlowID == _lstWorkflowItem[position].ItemID.ToString()) // binding theo Related Item
                {
                    vh._tvTitle.Text = _lstWorkflowItem[position].WorkflowContentRL;

                    vh._tvDescription.Text = _lstWorkflowItem[position].RelatedCode;

                    if (_lstWorkflowItem[position].CreatedRL.HasValue)
                        vh._tvTime.Text = CTRLHomePage.GetFormatDateLang(_lstWorkflowItem[position].CreatedRL.Value);
                    else
                        vh._tvTime.Text = "";

                    if (_lstWorkflowItem[position].StatusWorkflowRLID.HasValue)
                    {
                        vh._tvStatus.Visibility = ViewStates.Visible;
                        vh._tvStatus.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(CTRLHomePage.GetColorByAppStatus(_context, _lstWorkflowItem[position].StatusWorkflowRLID.Value));
                        string _query = String.Format("SELECT * FROM BeanAppStatus WHERE ID = {0} LIMIT 1 OFFSET 0", _lstWorkflowItem[position].StatusWorkflowRLID);
                        List<BeanAppStatus> _result = conn.Query<BeanAppStatus>(_query);

                        if (_result != null && _result.Count > 0)
                            vh._tvStatus.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? _result[0].Title : _result[0].TitleEN;
                        else
                            vh._tvStatus.Text = _lstWorkflowItem[position].StatusWorkflowRL;
                    }
                    else
                    {
                        vh._tvStatus.Visibility = ViewStates.Invisible;
                    }

                    ////vh._tvStatus.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(CTRLHomePage.GetColorByActionID(_context, _lstWorkflowItem[position].StatusWorkflowRLID));
                    ////string _query = String.Format("SELECT * FROM BeanWorkflowStatus WHERE ID = {0} LIMIT 1 OFFSET 0", _lstWorkflowItem[position].StatusWorkflowRLID);
                    ////List<BeanWorkflowStatus> _result = conn.Query<BeanWorkflowStatus>(_query);

                    ////if (_result != null && _result.Count > 0)
                    ////    vh._tvStatus.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? _result[0].Title : _result[0].TitleEN;
                    ////else
                    ////    vh._tvStatus.Text = _lstWorkflowItem[position].StatusWorkflowRL;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnBindViewHolder", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        public class AdapterAdapterComponentFlowRelatedHolder : RecyclerView.ViewHolder
        {
            public LinearLayout _lnAll { get; set; }
            public View _viewIsSelected { get; set; }
            public CircleImageView _imgAvatar { get; set; }
            public TextView _tvTitle { get; set; }
            public TextView _tvDescription { get; set; }
            public TextView _tvTime { get; set; }
            public TextView _tvStatus { get; set; }
            public ImageView _imgIschecked { get; set; }
            public AdapterAdapterComponentFlowRelatedHolder(View itemview, Action<int> listener) : base(itemview)
            {
                _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlFlowRelated_All);
                _viewIsSelected = itemview.FindViewById<View>(Resource.Id.view_ItemControlFlowRelated_LeftSelected);
                _imgAvatar = itemview.FindViewById<CircleImageView>(Resource.Id.img_ItemControlFlowRelated_Avatar);
                _tvTitle = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlFlowRelated_Title);
                _tvDescription = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlFlowRelated_Description);
                _tvTime = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlFlowRelated_Time);
                _tvStatus = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlFlowRelated_Status);
                _imgIschecked = itemview.FindViewById<ImageView>(Resource.Id.img_ItemControlFlowRelated_Checked);
                _lnAll.Click += (sender, e) =>
                {
                    listener(base.LayoutPosition);
                };
            }
        }
    }

    public class AdapterComponentFlowRelated_SwipeHelper : MySwipeHelper
    {
        public Context context;
        public ViewElement _element;
        public List<BeanWorkFlowRelated> _lstWorkflowItem = new List<BeanWorkFlowRelated>();
        public AdapterComponentFlowRelated_SwipeHelper(ViewElement _element, Context context, RecyclerView recyclerView, int buttonWidth, List<BeanWorkFlowRelated> _lstWorkflowItem) : base(context, recyclerView, buttonWidth)
        {
            this._element = _element;
            this.context = context;
            this.recyclerView = recyclerView;
            this.buttonWidth = buttonWidth;
            this._lstWorkflowItem = _lstWorkflowItem;
        }

        public override void InstantiateMyButton(RecyclerView.ViewHolder viewHolder, List<UnderLayoutButton> buffer)
        {
            // First Button
            //buffer.Add(new UnderLayoutButton(context, "Delete", 30, Resource.Drawable.icon_controlinputattach_delete, "#EB342E", new CustomFlowRelatedDeleteButtonClick(this, _lstWorkflowItem)));
            buffer.Add(new UnderLayoutButton(context, "Xóa", 30, 0, "#EB342E", new CustomFlowRelatedDeleteButtonClick(this, _lstWorkflowItem), buttonWidth / 3));

        }
        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            base.OnSwiped(viewHolder, direction);
            if (direction == ItemTouchHelper.Start) // Kéo full cây -> delete luôn
            {
                int position = viewHolder.AdapterPosition;
                //_adapter.DeleteSwipedItem(position);
            }
        }
    }

    internal class CustomFlowRelatedDeleteButtonClick : UnderLayoutButtonListener
    {
        private AdapterComponentFlowRelated_SwipeHelper myImplementSwipeHelper;
        private List<BeanWorkFlowRelated> _lstWorkflowItem = new List<BeanWorkFlowRelated>();
        public CustomFlowRelatedDeleteButtonClick(AdapterComponentFlowRelated_SwipeHelper myImplementSwipeHelper, List<BeanWorkFlowRelated> _lstWorkflowItem)
        {
            this.myImplementSwipeHelper = myImplementSwipeHelper;
            this._lstWorkflowItem = _lstWorkflowItem;
        }

        public void OnClick(int position)
        {
            MinionActionCore.OnFlowRelatedClick_WithInnerAction(null, new MinionActionCore.FlowRelatedClick_WithInnerAction(_lstWorkflowItem, (int)EnumFormControlInnerAction.FlowRelated_InnerActionID.Delete, position));
        }
    }
}