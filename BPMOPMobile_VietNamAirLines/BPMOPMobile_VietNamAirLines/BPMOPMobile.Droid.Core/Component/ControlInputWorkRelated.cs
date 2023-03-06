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
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using Newtonsoft.Json;
using Refractored.Controls;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ControlInputWorkRelated : ControlBase
    {
        private Context _context { get; set; }
        private LinearLayout _parentView { get; set; }
        private LinearLayout _lnFileInfo { get; set; }
        private LinearLayout _lnFileInfoChild1 { get; set; }
        private LinearLayout _lnFileInfoChild2 { get; set; }
        private LinearLayout _lnFileInfoChild3 { get; set; }
        private TextView _tvFileInfoChild1 { get; set; }
        private TextView _tvFileInfoChild2 { get; set; }
        private TextView _tvFileInfoChild3 { get; set; }
        private RecyclerView _recyAttachment { get; set; }
        private ViewElement _element { get; set; }

        public int _widthScreenTablet = -1;
        public string[] mimeTypes;
        private StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
        private AdapterControlLinkedWorkflow adapterControlLinkedWorkflow; // Adapter của Recy List file
        private List<BeanWorkflowItem> _lstWorkflowItem = new List<BeanWorkflowItem>();

        public ControlInputWorkRelated(Activity _mainAct, LinearLayout _parentView, ViewElement _element, int _widthScreenTablet) : base(_mainAct)
        {
            this._mainAct = _mainAct;
            this._parentView = _parentView;
            this._element = _element;
            this._widthScreenTablet = _widthScreenTablet;
            InitializeComponent();
        }
        public override void InitializeComponent()
        {
            base.InitializeComponent();
            _recyAttachment = new RecyclerView(_mainAct);
            _lnFileInfo = new LinearLayout(_mainAct);
            _lnFileInfoChild1 = new LinearLayout(_mainAct);
            _lnFileInfoChild2 = new LinearLayout(_mainAct);
            _lnFileInfoChild3 = new LinearLayout(_mainAct);
            _tvFileInfoChild1 = new TextView(_mainAct);
            _tvFileInfoChild2 = new TextView(_mainAct);
            _tvFileInfoChild3 = new TextView(_mainAct);

            _tvTitle.SetTextSize(ComplexUnitType.Sp, 16);
            _tvTitle.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clViolet)));
            _tvTitle.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            _tvTitle.Ellipsize = TextUtils.TruncateAt.End;
            _tvTitle.Gravity = GravityFlags.Center;

            _tvFileInfoChild1.SetTextSize(ComplexUnitType.Sp, 14);
            _tvFileInfoChild1.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
            _tvFileInfoChild1.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            _tvFileInfoChild1.Ellipsize = TextUtils.TruncateAt.End;
            _tvFileInfoChild1.Gravity = GravityFlags.Center;
            _tvFileInfoChild1.Text = "Mã phiếu";

            _tvFileInfoChild2.SetTextSize(ComplexUnitType.Sp, 14);
            _tvFileInfoChild2.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
            _tvFileInfoChild2.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            _tvFileInfoChild2.Ellipsize = TextUtils.TruncateAt.End;
            _tvFileInfoChild2.Gravity = GravityFlags.Center;
            _tvFileInfoChild2.Text = "Nội dung";

            _tvFileInfoChild3.SetTextSize(ComplexUnitType.Sp, 14);
            _tvFileInfoChild3.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
            _tvFileInfoChild3.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            _tvFileInfoChild3.Ellipsize = TextUtils.TruncateAt.End;
            _tvFileInfoChild3.Gravity = GravityFlags.Center;
            _tvFileInfoChild3.Text = "Tình trạng";

            _recyAttachment.Id = CmmDroidVariable.M_OnActivityResultFileChooserCode;
            _lnFileInfo.Orientation = Android.Widget.Orientation.Horizontal;
        }
        public override void InitializeFrameView(LinearLayout frame)
        {
            if (_element.Hidden == true) // Check xem có ẩn view hay không
                return;

            _context = frame.Context;
            base.InitializeFrameView(frame);
            _tvValue.Visibility = ViewStates.Gone;
            _lnLine.Visibility = ViewStates.Gone;
            _lnContent.RemoveView(_lnLine); // Remove line ra để add lại sau

            #region Linear Title 
            int _padding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 6, _mainAct.Resources.DisplayMetrics);
            LinearLayout.LayoutParams _paramslnTitleImport = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams _paramsTitle = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);

            _tvTitle.LayoutParameters = _paramsTitle;
            _tvTitle.Click += HandleTouchDown;
            _recyAttachment.SetRecycledViewPool(new RecyclerView.RecycledViewPool());

            #endregion

            #region Linear Workflow Info
            /*
            LinearLayout.LayoutParams _paramsLnInfo = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent, 0.9f);
            LinearLayout.LayoutParams _paramsLnInfoChild1 = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WrapContent, 0.3f);
            LinearLayout.LayoutParams _paramsLnInfoChild2 = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WrapContent, 0.3f);
            LinearLayout.LayoutParams _paramsLnInfoChild3 = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WrapContent, 0.3f);

            _lnFileInfo.LayoutParameters = _paramsLnInfo;
            _lnFileInfo.SetPadding(_padding, _padding, _padding, _padding);
            _lnFileInfo.Background = ContextCompat.GetDrawable(frame.Context, Resource.Drawable.textcornergray);

            _lnFileInfoChild1.LayoutParameters = _paramsLnInfoChild1;
            _lnFileInfoChild2.LayoutParameters = _paramsLnInfoChild2;
            _lnFileInfoChild3.LayoutParameters = _paramsLnInfoChild3;

            _lnFileInfoChild1.SetPadding(_padding, _padding, _padding, _padding);
            _lnFileInfoChild2.SetPadding(_padding, _padding, _padding, _padding);
            _lnFileInfoChild3.SetPadding(_padding, _padding, _padding, _padding);

            _lnFileInfoChild1.AddView(_tvFileInfoChild1);
            _lnFileInfoChild2.AddView(_tvFileInfoChild2);
            _lnFileInfoChild3.AddView(_tvFileInfoChild3);

            _lnFileInfo.AddView(_lnFileInfoChild1);
            _lnFileInfo.AddView(_lnFileInfoChild2);
            _lnFileInfo.AddView(_lnFileInfoChild3);
            */
            #endregion

            LinearLayout.LayoutParams _paramsRecy = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            _paramsRecy.SetMargins(0, 0, 0, 2 * _padding);
            _recyAttachment.LayoutParameters = _paramsRecy;
            //frame.AddView(_lnFileInfo);
            frame.AddView(_recyAttachment);
            frame.AddView(_lnLine);
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

            _tvTitle.Text = _element.Title;

            if (_element.IsRequire && _element.Enable)
            {
                _tvTitle.Text += " (*)";
                CmmDroidFunction.SetTextViewHighlightControl(_mainAct, _tvTitle);
            }
        }
        public override void SetValue()
        {
            base.SetValue();
            var data = _element.Value.Trim();
            //if (data.Contains(";#"))
            //{
            //    var _arrAttachment = data.Split(new string[] { ";#" }, StringSplitOptions.None);
            //    if (_arrAttachment.Length > 2)
            //    {
            //        for (var i = 0; i < _arrAttachment.Length; i += 2)
            //        {
            //            KeyValuePair<string, string> item = new KeyValuePair<string, string>(_arrAttachment[i], _arrAttachment[i + 1]);
            //            _lstAttachment.Add(item);
            //        }
            //    }
            //    else
            //        _lstAttachment.Add(new KeyValuePair<string, string>(_arrAttachment[0], _arrAttachment[1]));
            //}             
            _lstWorkflowItem = JsonConvert.DeserializeObject<List<BeanWorkflowItem>>(data);
            if (_lstWorkflowItem == null || _lstWorkflowItem.Count == 0)
            {
                _lstWorkflowItem = new List<BeanWorkflowItem>();
            }
            adapterControlLinkedWorkflow = new AdapterControlLinkedWorkflow(_mainAct, _context, _lstWorkflowItem, _widthScreenTablet);
            adapterControlLinkedWorkflow.CustomItemClick_RemoveItem += Click_ItemAttach_RemoveItem;
            _recyAttachment.SetLayoutManager(staggeredGridLayoutManager);
            _recyAttachment.SetAdapter(adapterControlLinkedWorkflow);

            ItemTouchHelper.Callback customTouchHelper = new CustomItemTouchHelper(_context, _mainAct, adapterControlLinkedWorkflow);
            ItemTouchHelper itemTouchHelper = new ItemTouchHelper(customTouchHelper);
            itemTouchHelper.AttachToRecyclerView(_recyAttachment);

        }

        private void Click_ItemAttach_RemoveItem(object sender, BeanWorkflowItem e)
        {
            try
            {
                if (_parentView != null)
                {
                    MinionActionCore.OnElementClickImportFileDetailCreateWorkflow_Action(null, new MinionActionCore.ElementImportFileClick_Action(_element, e, "RemoveItem"));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControlAttachmentImport - Click_ItemAttach_RemoveItem - Error: " + ex.Message);
#endif
            }
        }

    }
    public class CustomItemTouchHelper : ItemTouchHelper.Callback
    {
        private AdapterControlLinkedWorkflow _adapter;
        private Context _context;
        private Activity _mainAct;
        public CustomItemTouchHelper(Context _context, Activity _mainAct, AdapterControlLinkedWorkflow _adapter)
        {
            this._adapter = _adapter;
            this._context = _context;
            this._mainAct = _mainAct;
        }
        public override int GetMovementFlags(RecyclerView p0, RecyclerView.ViewHolder p1)
        {
            int dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
            int swipeFlags = ItemTouchHelper.Start /*| ItemTouchHelper.End*/;
            return MakeMovementFlags(dragFlags, swipeFlags);
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
        {
            return true;
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            if (direction == ItemTouchHelper.End)
            {

            }
            else if (direction == ItemTouchHelper.Start)
            {
                int position = viewHolder.AdapterPosition;
                _adapter.DeleteSwipedItem(position);
            }

        }
        public override void OnChildDraw(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
        {
            if (actionState == ItemTouchHelper.ActionStateSwipe)
            {
                View itemView = viewHolder.ItemView;
                Paint p = new Paint();
                ColorFilter filter = new PorterDuffColorFilter(new Color(ContextCompat.GetColor(_context, Resource.Color.clActionRed)), PorterDuff.Mode.SrcIn);
                p.SetColorFilter(filter);
                if (dX > 0)
                {
                    c.DrawRect((float)itemView.Left, (float)itemView.Top, dX, (float)itemView.Bottom, p);
                }
                else
                {
                    c.DrawRect((float)itemView.Right + dX, (float)itemView.Top, (float)itemView.Right, (float)itemView.Bottom, p);
                }

                Drawable icon = ContextCompat.GetDrawable(_context, Resource.Drawable.icon_attach);
                icon.SetBounds(itemView.Right - 5, itemView.Top, itemView.Right - 5, itemView.Top + icon.IntrinsicHeight);
                icon.Draw(c);
            }
            base.OnChildDraw(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);
        }
        public override float GetSwipeEscapeVelocity(float defaultValue)
        {
            return 0.1f * defaultValue;
        }
        public override float GetSwipeVelocityThreshold(float defaultValue)
        {
            return 5.0f * defaultValue;
        }
        private void drawButtons(Canvas c, RecyclerView.ViewHolder viewHolder)
        {
            float buttonWidthWithoutPadding = /*buttonWidth*/ 200 - 20;
            float corners = 16;

            View itemView = viewHolder.ItemView;
            Paint p = new Paint();

            RectF leftButton = new RectF(itemView.Left, itemView.Top, itemView.Left + buttonWidthWithoutPadding, itemView.Bottom);
            p.Color = Color.Blue;
            c.DrawRoundRect(leftButton, corners, corners, p);
            drawText("EDIT", c, leftButton, p);

            RectF rightButton = new RectF(itemView.Right - buttonWidthWithoutPadding, itemView.Top, itemView.Right, itemView.Bottom);
            p.Color = Color.Red; //(Color.RED);
            c.DrawRoundRect(rightButton, corners, corners, p);
            drawText("DELETE", c, rightButton, p);

            //buttonInstance = null;
            //if (buttonShowedState == ButtonsState.LEFT_VISIBLE)
            //{
            //    buttonInstance = leftButton;
            //}
            //else if (buttonShowedState == ButtonsState.RIGHT_VISIBLE)
            //{
            //    buttonInstance = rightButton;
            //}
        }

        private void drawText(String text, Canvas c, RectF button, Paint p)
        {
            float textSize = 60;
            p.Color = Color.White;
            p.AntiAlias = true;
            p.TextSize = textSize;

            float textWidth = p.MeasureText(text);
            c.DrawText(text, (int)(button.CenterX() - (textWidth / 2)), (int)(button.CenterY() + (int)textSize / 2), p);
        }

    }

    public class AdapterControlLinkedWorkflow : RecyclerView.Adapter
    {
        public Activity _mainAct;
        public Context _context;
        public List<BeanWorkflowItem> _lstWorkflowItem = new List<BeanWorkflowItem>();
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        private int _widthScreenTablet = -1;

        public event EventHandler<BeanWorkflowItem> CustomItemClick_RemoveItem;

        public AdapterControlLinkedWorkflow(Activity _mainAct, Context _context, List<BeanWorkflowItem> _lstWorkflowItem, int _widthScreenTablet)
        {
            this._mainAct = _mainAct;
            this._lstWorkflowItem = _lstWorkflowItem;
            this._widthScreenTablet = _widthScreenTablet;
            this._context = _context;
        }
        public void DeleteSwipedItem(int position)
        {
            OnItemClick_RemoveItem(_lstWorkflowItem[position]);
        }
        private void OnItemClick_RemoveItem(BeanWorkflowItem obj)
        {
            CustomItemClick_RemoveItem(this, obj);
        }
        public override int ItemCount => _lstWorkflowItem.Count;
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (_widthScreenTablet == -1) // Phone
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemControlLinkedWorkflow_Phone, parent, false);
                AdapterControlControlLinkedWorkflowViewHolder_Phone holder = new AdapterControlControlLinkedWorkflowViewHolder_Phone(itemView);
                return holder;
            }
            else
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemControlLinkedWorkflow, parent, false);
                AdapterControlControlLinkedWorkflowViewHolder holder = new AdapterControlControlLinkedWorkflowViewHolder(itemView);
                return holder;
            }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (_widthScreenTablet == -1) // Phone
            {
                AdapterControlControlLinkedWorkflowViewHolder_Phone vh = holder as AdapterControlControlLinkedWorkflowViewHolder_Phone;
                if (!String.IsNullOrEmpty(_lstWorkflowItem[position].WorkflowTitle))
                {
                    vh._tvTitle.Text = _lstWorkflowItem[position].WorkflowTitle;
                }
                else
                {
                    vh._tvTitle.Text = "";
                }
                if (!String.IsNullOrEmpty(_lstWorkflowItem[position].ActionStatus))
                {
                    vh._tvStatus.Visibility = ViewStates.Visible;
                    vh._tvStatus.Text = _lstWorkflowItem[position].ActionStatus;
                    vh._tvStatus.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(CTRLHomePage.GetColorByActionID(_context, _lstWorkflowItem[position].ActionStatusID.Value));
                }
                else
                {
                    vh._tvStatus.Visibility = ViewStates.Invisible;
                }
                vh._imgIschecked.Click += (sender, e) => OnItemClick_RemoveItem(_lstWorkflowItem[position]);
                vh._imgIschecked.Visibility = ViewStates.Gone;
            }
            else
            {
                AdapterControlControlLinkedWorkflowViewHolder vh = holder as AdapterControlControlLinkedWorkflowViewHolder;
                if (!String.IsNullOrEmpty(_lstWorkflowItem[position].WorkflowTitle))
                {
                    vh._tvTitle.Text = _lstWorkflowItem[position].WorkflowTitle;
                }
                else
                {
                    vh._tvTitle.Text = "";
                }
                if (!String.IsNullOrEmpty(_lstWorkflowItem[position].ActionStatus))
                {
                    vh._tvStatus.Visibility = ViewStates.Visible;
                    vh._tvStatus.Text = _lstWorkflowItem[position].ActionStatus;
                    vh._tvStatus.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(CTRLHomePage.GetColorByActionID(_context, _lstWorkflowItem[position].ActionStatusID.Value));
                }
                else
                {
                    vh._tvStatus.Visibility = ViewStates.Invisible;
                }
                vh._imgIschecked.Click += (sender, e) => OnItemClick_RemoveItem(_lstWorkflowItem[position]);
                vh._imgIschecked.Visibility = ViewStates.Gone;
            }
        }
    }
    public class AdapterControlControlLinkedWorkflowViewHolder : RecyclerView.ViewHolder
    {
        public LinearLayout _lnAll { get; set; }
        public View _viewIsSelected { get; set; }
        public TextView _tvAvatar { get; set; }
        public CircleImageView _imgAvatar { get; set; }
        public TextView _tvTitle { get; set; }
        public TextView _tvDescription { get; set; }
        public TextView _tvTime { get; set; }
        public TextView _tvStatus { get; set; }
        public ImageView _imgIschecked { get; set; }
        public AdapterControlControlLinkedWorkflowViewHolder(View itemview) : base(itemview)
        {
            _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlLinkedWorkflow_All);
            _viewIsSelected = itemview.FindViewById<View>(Resource.Id.view_ItemControlLinkedWorkflow_LeftSelected);
            _tvAvatar = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlLinkedWorkflow_Avatar);
            _imgAvatar = itemview.FindViewById<CircleImageView>(Resource.Id.img_ItemControlLinkedWorkflow_Avatar);
            _tvTitle = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlLinkedWorkflow_Title);
            _tvDescription = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlLinkedWorkflow_Description);
            _tvTime = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlLinkedWorkflow_Time);
            _tvStatus = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlLinkedWorkflow_Status);
            _imgIschecked = itemview.FindViewById<ImageView>(Resource.Id.img_ItemControlLinkedWorkflow_Checked);
        }
    }
    public class AdapterControlControlLinkedWorkflowViewHolder_Phone : RecyclerView.ViewHolder
    {
        public LinearLayout _lnAll { get; set; }
        public View _viewIsSelected { get; set; }
        public TextView _tvAvatar { get; set; }
        public CircleImageView _imgAvatar { get; set; }
        public TextView _tvTitle { get; set; }
        public TextView _tvDescription { get; set; }
        public TextView _tvTime { get; set; }
        public TextView _tvStatus { get; set; }
        public ImageView _imgIschecked { get; set; }
        public AdapterControlControlLinkedWorkflowViewHolder_Phone(View itemview) : base(itemview)
        {
            _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlLinkedWorkflow_Phone_All);
            _viewIsSelected = itemview.FindViewById<View>(Resource.Id.view_ItemControlLinkedWorkflow_Phone_LeftSelected);
            _tvAvatar = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlLinkedWorkflow_Phone_Avatar);
            _imgAvatar = itemview.FindViewById<CircleImageView>(Resource.Id.img_ItemControlLinkedWorkflow_Phone_Avatar);
            _tvTitle = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlLinkedWorkflow_Phone_Title);
            _tvDescription = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlLinkedWorkflow_Phone_Description);
            _tvTime = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlLinkedWorkflow_Phone_Time);
            _tvStatus = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlLinkedWorkflow_Phone_Status);
            _imgIschecked = itemview.FindViewById<ImageView>(Resource.Id.img_ItemControlLinkedWorkflow_Phone_Checked);
        }
    }
}
