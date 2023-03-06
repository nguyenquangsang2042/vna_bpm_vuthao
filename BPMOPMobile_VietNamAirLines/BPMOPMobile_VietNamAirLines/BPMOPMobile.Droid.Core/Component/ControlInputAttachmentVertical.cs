using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
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
using Com.Tubb.Smrv;
using Newtonsoft.Json;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ControlInputAttachmentVertical : ControlBase
    {
        private Context _context { get; set; }
        private LinearLayout _parentView { get; set; }
        private CardView _cardViewExpandable { get; set; }
        private LinearLayout _lnCardView { get; set; } // linear parent Card View
        private LinearLayout _lnExpandable { get; set; }
        private LinearLayout _lnTitleImport { get; set; } // chứa _tvTitle và _lnImport
        private LinearLayout _lnImport { get; set; }
        private LinearLayout _lnFileInfo { get; set; }
        private LinearLayout _lnFileInfoChild2 { get; set; }
        private LinearLayout _lnFileInfoChild3 { get; set; }
        private TextView _tvFileInfoChild2 { get; set; }
        private TextView _tvFileInfoChild3 { get; set; }
        private ImageView _imgImport { get; set; }
        private TextView _tvImport { get; set; }
        private ExpandableListView _expandAttachment { get; set; }
        private ViewElement _element { get; set; }

        public int _widthScreenTablet = -1;
        private int _flagView; // Flag này để ẩn 1 số chức năng vì trang chi tiết ko cần hiển thị đủ - 1: DetailWorkflow, 2: DetailAttachFile, 3 - DetailCreateTask
        private AdapterExpandControlInputAttachmentVertical adapterExpandAttachment;
        private List<BeanAttachFile> _lstAttachment = new List<BeanAttachFile>();
        private ControllerDetailAttachFile CTRLDetailAttachFile = new ControllerDetailAttachFile();

        public ControlInputAttachmentVertical(Activity _mainAct, LinearLayout _parentView, ViewElement _element, int _widthScreenTablet, int _flagView) : base(_mainAct)
        {
            this._mainAct = _mainAct;
            this._parentView = _parentView;
            this._element = _element;
            this._widthScreenTablet = _widthScreenTablet;
            this._flagView = _flagView;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();
            _expandAttachment = new AnimatedExpandableListView(_mainAct);
            _cardViewExpandable = new CardView(_mainAct);
            _lnCardView = new LinearLayout(_mainAct);
            _lnExpandable = new LinearLayout(_mainAct);
            _lnTitleImport = new LinearLayout(_mainAct);
            _lnFileInfo = new LinearLayout(_mainAct);
            _lnFileInfoChild2 = new LinearLayout(_mainAct);
            _lnFileInfoChild3 = new LinearLayout(_mainAct);
            _tvFileInfoChild2 = new TextView(_mainAct);
            _tvFileInfoChild3 = new TextView(_mainAct);
            _lnImport = new LinearLayout(_mainAct);
            _imgImport = new ImageView(_mainAct);
            _tvImport = new TextView(_mainAct);

            _lnExpandable.Orientation = Orientation.Vertical;

            _tvTitle.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            _tvTitle.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));

            _tvImport.SetTextSize(ComplexUnitType.Sp, 12);
            _tvImport.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
            _tvImport.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
            _tvImport.Ellipsize = TextUtils.TruncateAt.End;
            _tvImport.Gravity = GravityFlags.Center;

            _tvFileInfoChild2.SetTextSize(ComplexUnitType.Sp, 12);
            _tvFileInfoChild2.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
            _tvFileInfoChild2.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            _tvFileInfoChild2.Ellipsize = TextUtils.TruncateAt.End;
            _tvFileInfoChild2.Gravity = GravityFlags.Center;

            _tvFileInfoChild3.SetTextSize(ComplexUnitType.Sp, 12);
            _tvFileInfoChild3.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
            _tvFileInfoChild3.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            _tvFileInfoChild3.Ellipsize = TextUtils.TruncateAt.End;
            _tvFileInfoChild3.Gravity = GravityFlags.Center;

            _tvImport.Text = CmmFunction.GetTitle("TEXT_ADDNEW_ATTACHMENT", "Tạo mới");
            _tvFileInfoChild2.Text = CmmFunction.GetTitle("TEXT_CONTROL_DOCUMENTNAME", "Tên tài liệu");
            _tvFileInfoChild3.Text = CmmFunction.GetTitle("TEXT_CONTROL_CREATOR", "Người tạo");

            _imgImport.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomEnable)));

            _cardViewExpandable.UseCompatPadding = true;
            _cardViewExpandable.Radius = 5f;

            _lnFileInfo.Orientation = Orientation.Horizontal;
            _lnTitleImport.Orientation = Orientation.Horizontal;
            _lnTitleImport.SetGravity(GravityFlags.Center);
            _lnImport.Orientation = Orientation.Horizontal;
            _lnImport.SetGravity(GravityFlags.Right);
            _tvImport.Gravity = GravityFlags.Bottom;

            _lnCardView.Orientation = Orientation.Vertical;

            _expandAttachment.SetGroupIndicator(null);
            _expandAttachment.SetChildIndicator(null);
            _expandAttachment.DividerHeight = 0;
            _expandAttachment.NestedScrollingEnabled = true;
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
            _lnContent.RemoveView(_tvTitle);

            #region Linear Title + Import
            int _padding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 6, _mainAct.Resources.DisplayMetrics);
            LinearLayout.LayoutParams _paramslnTitleImport = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams _paramsTitle = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams _paramslnImport = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams _paramsimgImport = new LinearLayout.LayoutParams((int)CmmDroidFunction.ConvertDpToPixel(20, frame.Context), (int)CmmDroidFunction.ConvertDpToPixel(20, frame.Context));
            _paramsimgImport.SetMargins(_padding, 0, 2 * _padding, 0);
            _paramslnTitleImport.SetMargins(0, 0, 0, _padding);
            _lnTitleImport.LayoutParameters = _paramslnTitleImport;
            _tvTitle.LayoutParameters = _paramsTitle;
            _lnImport.LayoutParameters = _paramslnImport;
            _imgImport.LayoutParameters = _paramsimgImport;

            _imgImport.Background = ContextCompat.GetDrawable(frame.Context, Resource.Drawable.icon_ver2_addfile);
            _imgImport.SetPadding(_padding, 0, _padding, 0);

            //_recyAttachment.SetRecycledViewPool(new RecyclerView.RecycledViewPool());
            _lnImport.SetPadding(_padding, 0, 2 * _padding, 0);

            _lnTitleImport.Click += delegate { };

            if (_element.Enable) // enable mới cho click
            {
                _lnImport.Visibility = ViewStates.Visible;
                _lnImport.Click += Click_lnImport;
            }
            else
            {
                _lnImport.Visibility = ViewStates.Invisible;
            }

            _lnImport.AddView(_imgImport);
            _lnImport.AddView(_tvImport);
            _lnTitleImport.AddView(_tvTitle);
            _lnTitleImport.AddView(_lnImport);

            #endregion

            #region Linear File Info

            LinearLayout.LayoutParams _paramsLnInfo = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent/*, 1.0f*/);
            LinearLayout.LayoutParams _paramsLnInfoChild2 = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WrapContent, 0.65f);
            LinearLayout.LayoutParams _paramsLnInfoChild3 = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WrapContent, 0.35f);

            _lnFileInfo.LayoutParameters = _paramsLnInfo;
            _lnFileInfo.SetPadding(_padding, 2 * _padding, _padding, 2 * _padding);
            _lnFileInfo.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clGraySearchUser)));

            _lnFileInfoChild2.LayoutParameters = _paramsLnInfoChild2;
            _lnFileInfoChild3.LayoutParameters = _paramsLnInfoChild3;

            _lnFileInfoChild2.SetPadding(_padding, _padding, _padding, _padding);
            _lnFileInfoChild3.SetPadding(_padding, _padding, _padding, _padding);

            _lnFileInfoChild2.AddView(_tvFileInfoChild2);
            _lnFileInfoChild3.AddView(_tvFileInfoChild3);

            _lnFileInfo.AddView(_lnFileInfoChild2);
            _lnFileInfo.AddView(_lnFileInfoChild3);

            #endregion

            LinearLayout.LayoutParams _paramsCardView = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);

            _cardViewExpandable.LayoutParameters = _paramsCardView;
            _lnCardView.LayoutParameters = _paramsCardView;

            frame.SetPadding(_padding, 0, _padding, 0);

            if (_flagView == (int)EnumFormControlView.FlagViewControlAttachment.DetailWorkflow || _flagView == (int)EnumFormControlView.FlagViewControlAttachment.DetailCreateTask) // Trang chi tiết file đính kèm chỉ hiện List
            {
                frame.AddView(_lnTitleImport);
                _lnCardView.AddView(_lnFileInfo);
            }

            _lnCardView.AddView(_lnExpandable);
            _cardViewExpandable.AddView(_lnCardView);
            frame.AddView(_cardViewExpandable);
            frame.AddView(_lnLine);
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

            int groupHeight = (int)CmmDroidFunction.ConvertDpToPixel(45, _context); // Group View height = 45dp
            int childHeight = (int)CmmDroidFunction.ConvertDpToPixel(60, _context); // child item view height = 60dp

            _lstAttachment = JsonConvert.DeserializeObject<List<BeanAttachFile>>(data);
            if (_lstAttachment == null || _lstAttachment.Count == 0)
            {
                _lstAttachment = new List<BeanAttachFile>();
            }
            List<BeanGroupAttachFile> _lstGroupAtt = CTRLDetailAttachFile.CloneListAttToGroup(_lstAttachment);

            Action action = new Action(() =>
            {
                adapterExpandAttachment = new AdapterExpandControlInputAttachmentVertical(_mainAct, _context, _element, _lstAttachment, _lstGroupAtt, _flagView);
                _expandAttachment.SetAdapter(adapterExpandAttachment);

                for (int i = 0; i < adapterExpandAttachment.GroupCount; i++)
                {
                    _expandAttachment.ExpandGroup(i);
                }

                _expandAttachment.GroupExpand += (sender, e) =>
                {
                    // Mở rộng ra -> + Height 
                    _expandAttachment.LayoutParameters.Height += adapterExpandAttachment.GetChildrenCount(e.GroupPosition) * childHeight;
                    adapterExpandAttachment.NotifyDataSetChanged();
                };
                _expandAttachment.GroupCollapse += (sender, e) =>
                {
                    // thu hẹp lại -> + Height 
                    _expandAttachment.LayoutParameters.Height -= adapterExpandAttachment.GetChildrenCount(e.GroupPosition) * childHeight;
                    adapterExpandAttachment.NotifyDataSetChanged();
                };

                _expandAttachment.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, _lnExpandable.LayoutParameters.Height);

                _lnCardView.RemoveView(_lnExpandable);
                _lnCardView.AddView(_expandAttachment);
                _expandAttachment.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_context));
            });

            _lnExpandable.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, (childHeight * _lstAttachment.Count) + (groupHeight * _lstGroupAtt.Count));
            if (_lstAttachment.Count > 4) // Shimmer
            {
                LayoutInflater _inflater = (LayoutInflater)_mainAct.GetSystemService(Android.Content.Context.LayoutInflaterService);
                for (int i = 0; i < _lstAttachment.Count + _lstGroupAtt.Count; i++)
                {
                    _lnExpandable.AddView(_inflater.Inflate(Resource.Layout.ItemControlInputAttachmentVertical_Shimmer, null));
                }
                new Handler().PostDelayed(action, 100 * _lstAttachment.Count);
            }
            else
                new Handler().PostDelayed(action, 0);
        }

        private void Click_lnImport(object sender, EventArgs e)
        {
            try
            {
                if (_parentView != null)
                {
                    //MinionActionCore.OnElementFormClick(null, new MinionActionCore.ElementFormClick(_element));
                    MinionActionCore.OnElementFormClickEvent_WithInnerAction(null, new MinionActionCore.ElementFormClick_WithInnerAction(_element, (int)EnumFormControlInnerAction.ControlInputAttachmentVertical_InnerActionID.Create, -1, _flagView));
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - Click_lnImport - Error: " + ex.Message);
#endif
            }
        }


        #region ExpandableListView 
        public class AdapterExpandControlInputAttachmentVertical : CustomAnimatedExpandableAdapter
        {
            private Context _context;
            private Activity _mainAct;
            private List<BeanAttachFile> _lstAllAttachFile = new List<BeanAttachFile>();
            private List<BeanGroupAttachFile> _lstGroupAttachFile = new List<BeanGroupAttachFile>();
            private ViewElement _element;
            private int _flagView;


            public AdapterExpandControlInputAttachmentVertical(Activity _mainAct, Context _context, ViewElement _element, List<BeanAttachFile> _lstAllAttachFile, List<BeanGroupAttachFile> _lstGroupAttachFile, int _flagView)
            {
                this._context = _context;
                this._mainAct = _mainAct;
                this._lstAllAttachFile = _lstAllAttachFile;
                this._lstGroupAttachFile = _lstGroupAttachFile;
                this._element = _element;
                this._flagView = _flagView;
            }

            public override int GroupCount => _lstGroupAttachFile.Count;

            public override bool HasStableIds => false;

            public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
            {
                return childPosition;
            }

            public override long GetChildId(int groupPosition, int childPosition)
            {
                return childPosition;
            }


            public override int getRealChildrenCount(int groupPosition)
            {
                if (_lstGroupAttachFile[groupPosition].AttachFiles != null)
                    return _lstGroupAttachFile[groupPosition].AttachFiles.Count;
                else
                    return 0;
            }

            public override View getRealChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
            {
                View rootView = convertView;
                LayoutInflater mInflater = LayoutInflater.From(_context);
                rootView = mInflater.Inflate(Resource.Layout.ItemControlInputAttachmentVerticalChild, null);

                if (childPosition == 0) // chỉ load cho item đầu tiên, mấy item sau ẩn
                {
                    RecyclerView _recyListAttach = rootView.FindViewById<RecyclerView>(Resource.Id.recy_ItemControlInputAttachmentVerticalChild);
                    AdapterControlAttachmentImport adapterControlAttachmentImport = new AdapterControlAttachmentImport(_mainAct, _context, _lstGroupAttachFile[groupPosition].AttachFiles);
                    adapterControlAttachmentImport.CustomItemClick_WatchFullItem += Click_ItemAttach_WatchFullItem;
                    StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                    _recyListAttach.SetLayoutManager(staggeredGridLayoutManager);
                    _recyListAttach.SetAdapter(adapterControlAttachmentImport);

                    if (_element.Enable) // Nếu ko phải - > add event swipe cho recy (Trang chi tiết đính kèm sẽ bỏ)
                    {
                        if (_flagView == (int)EnumFormControlView.FlagViewControlAttachment.DetailWorkflow || _flagView == (int)EnumFormControlView.FlagViewControlAttachment.DetailCreateTask)
                        {
                            MySwipeHelper mySwipeHelper = new AdapterControlAttachmentImport_SwipeHelper(_element, _context, _recyListAttach, (int)(_mainAct.Resources.DisplayMetrics.WidthPixels * 0.15), _lstGroupAttachFile[groupPosition].AttachFiles, groupPosition, _flagView);
                        }
                    }
                }
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
                LayoutInflater mInflater = LayoutInflater.From(_context);
                rootView = mInflater.Inflate(Resource.Layout.ItemControlInputAttachmentVerticalGroup, null);

                ImageView _imgExpand = rootView.FindViewById<ImageView>(Resource.Id.img_ItemControlInputAttachmentVerticalGroup_Expand);
                TextView _tvtitle = rootView.FindViewById<TextView>(Resource.Id.tv_ItemControlInputAttachmentVerticalGroup_Title);

                if (isExpanded == true)
                {
                    _imgExpand.Rotation = -90;
                }
                else
                {
                    _imgExpand.Rotation = 180;
                }

                if (!String.IsNullOrEmpty(_lstGroupAttachFile[groupPosition].Category))
                {
                    if (_lstGroupAttachFile[groupPosition].Category.Contains(";#"))
                    {
                        _tvtitle.Text = _lstGroupAttachFile[groupPosition].Category.Split(";#")[1];
                    }
                    else
                    {
                        _tvtitle.Text = _lstGroupAttachFile[groupPosition].Category;
                    }
                    if (_lstGroupAttachFile[groupPosition].AttachFiles != null && _lstGroupAttachFile.Count > 0) // Gắn thêm count vào
                    {
                        _tvtitle.Text += String.Format(" ({0})", _lstGroupAttachFile[groupPosition].AttachFiles.Count);
                    }
                }
                else
                {
                    _tvtitle.Text = "";
                }
                return rootView;
            }

            public override bool IsChildSelectable(int groupPosition, int childPosition)
            {
                return true;
            }

            private void Click_ItemAttach_WatchFullItem(object sender, BeanAttachFile clickedItem)
            {
                try
                {
                    // tìm ra position item đó -> trả ra
                    List<BeanAttachFile> _lstAttachFileFull = JsonConvert.DeserializeObject<List<BeanAttachFile>>(_element.Value.Trim());
                    int position = CmmDroidFunction.FindIndexOfItemInListAttach(clickedItem, _lstAttachFileFull);

                    if (position != -1) // Tìm ra
                        MinionActionCore.OnElementFormClickEvent_WithInnerAction(null, new MinionActionCore.ElementFormClick_WithInnerAction(_element, (int)EnumFormControlInnerAction.ControlInputAttachmentVertical_InnerActionID.View, position, _flagView));
                }
                catch (System.Exception ex)
                {
#if DEBUG
                    Console.WriteLine("Author: khoahd - ControlAttachmentImport - Click_ItemAttach_RemoveItem - Error: " + ex.Message);
#endif
                }
            }
        }
        #endregion

        #region RecyclerView Child View
        public class AdapterControlAttachmentImport : RecyclerView.Adapter
        {
            public Activity _mainAct;
            public Context _context;
            public List<BeanAttachFile> _lstAttachment = new List<BeanAttachFile>();
            public event EventHandler<BeanAttachFile> CustomItemClick_WatchFullItem;
            private ControllerBase CTRLBase = new ControllerBase();
            public int recyHeight = 0;

            public AdapterControlAttachmentImport(Activity _mainAct, Context _context, List<BeanAttachFile> _lstAttachment)
            {
                this._mainAct = _mainAct;
                this._lstAttachment = _lstAttachment;
                this._context = _context;
            }
            public int getRecyHeight()
            {
                return recyHeight;
            }
            private void OnItemClick_WatchFullItem(int position)
            {
                if (CustomItemClick_WatchFullItem != null)
                {
                    Action action = new Action(() =>
                    {
                        CustomItemClick_WatchFullItem(this, _lstAttachment[position]);
                    });
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
                }
            }
            public override int ItemCount => _lstAttachment.Count;
            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemControlInputAttachmentVertical, parent, false);
                AdapterControlAttachmentImportViewHolder holder = new AdapterControlAttachmentImportViewHolder(itemView, OnItemClick_WatchFullItem);
                return holder;
            }
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                AdapterControlAttachmentImportViewHolder vh = holder as AdapterControlAttachmentImportViewHolder;

                if (position % 2 == 0) // tô màu so le
                    vh._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clVer2BlueNavigation)));
                else
                    vh._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clWhite)));

                if (String.IsNullOrEmpty(_lstAttachment[position].ID)) // file mới -> hiện icon lên
                    vh._imgNewFile.Visibility = ViewStates.Visible;
                else
                    vh._imgNewFile.Visibility = ViewStates.Gone;

                vh._imgExtension.SetImageResource(CTRLBase.GetResourceIDAttachment(_lstAttachment[position].Path));

                if (!string.IsNullOrEmpty(_lstAttachment[position].Title))
                {
                    if (_lstAttachment[position].Title.Contains(";#"))
                    {
                        vh._tvName.Text = _lstAttachment[position].Title.Split(new string[] { ";#" }, StringSplitOptions.None)[0];
                    }
                    else
                    {
                        vh._tvName.Text = _lstAttachment[position].Title;
                    }
                }
                else
                {
                    vh._tvName.Text = "";
                }

                try
                {
                    vh._tvSize.Text = CmmDroidFunction.GetFormatFileSize(_lstAttachment[position].Size);
                }
                catch (Exception)
                {
                    vh._tvSize.Text = "";
                }

                vh._tvCategory.Text = _lstAttachment[position].CreatedName;

                vh._tvPosition.Text = _lstAttachment[position].CreatedPositon;
            }
        }
        public class AdapterControlAttachmentImportViewHolder : RecyclerView.ViewHolder
        {
            public TextView _tvName { get; set; }
            public TextView _tvSize { get; set; }
            public ImageView _imgExtension { get; set; }
            public ImageView _imgNewFile { get; set; }
            public TextView _tvCategory { get; set; }
            public LinearLayout _lnName { get; set; }
            public LinearLayout _lnCategory { get; set; }
            public LinearLayout _lnAll { get; set; }
            public LinearLayout _lnDelete { get; set; }
            public ImageView _imgDelete { get; set; }
            public TextView _tvPosition { get; set; }
            public AdapterControlAttachmentImportViewHolder(View itemview, Action<int> listener) : base(itemview)
            {
                _tvName = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlInputAttachmentVertical_Name);
                _tvSize = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlInputAttachmentVertical_Size);
                _imgExtension = itemview.FindViewById<ImageView>(Resource.Id.img_ItemControlInputAttachmentVertical_Extension);
                _tvCategory = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlInputAttachmentVertical_Category);
                _tvPosition = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlInputAttachmentVertical_Position);
                _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlInputAttachmentVertical);
                _lnName = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlInputAttachmentVertical_Name);
                _lnCategory = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlInputAttachmentVertical_Category);
                _imgNewFile = itemview.FindViewById<ImageView>(Resource.Id.img_ItemControlInputAttachmentVertical_NewFile);

                _lnAll.Click += (sender, e) =>
                {
                    listener(base.LayoutPosition);
                };
            }
        }
        public class AdapterControlAttachmentImport_SwipeHelper : MySwipeHelper
        {
            public Context context;
            public ViewElement _element;
            public List<BeanAttachFile> _lstAttachFile; // List full tất cả
            public int groupPosition;
            public int _flagView;
            public AdapterControlAttachmentImport_SwipeHelper(ViewElement _element, Context context, RecyclerView recyclerView, int buttonWidth, List<BeanAttachFile> _lstAttachFile, int groupPosition, int _flagView) : base(context, recyclerView, buttonWidth)
            {
                this._element = _element;
                this.context = context;
                this.recyclerView = recyclerView;
                this.buttonWidth = buttonWidth;
                this._lstAttachFile = _lstAttachFile;
                this.groupPosition = groupPosition;
                this._flagView = _flagView;
            }

            public override void InstantiateMyButton(RecyclerView.ViewHolder viewHolder, List<UnderLayoutButton> buffer)
            {
                // First Button
                buffer.Add(new UnderLayoutButton(context, "Delete", 30, Resource.Drawable.icon_ver2_star_controlattch_delete, "#EB342E", new CustomDeleteButtonClick(this, _element, groupPosition, _flagView), buttonWidth / 3));

                // Second
                buffer.Add(new UnderLayoutButton(context, "Update", 30, Resource.Drawable.icon_ver2_star_controlattch_edit, "#335FB3", new CustomEditButtonClick(this, _element, groupPosition, _flagView), buttonWidth / 3));
            }
            /// <summary>
            /// Hàm này override ra để Disable Edit những item mà user không đủ quyền 
            /// </summary>
            /// <param name="recyclerView"></param>
            /// <param name="viewHolder"></param>
            /// <returns></returns>
            public override int GetSwipeDirs(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
            {
                int pos = viewHolder.AdapterPosition;
                if (_lstAttachFile[pos].IsAuthor == true) // Được phép chỉnh sửa
                {
                    return base.GetSwipeDirs(recyclerView, viewHolder);
                }
                else
                {
                    return 0;
                }

            }
        }
        internal class CustomDeleteButtonClick : UnderLayoutButtonListener
        {
            private ControlInputAttachmentVertical.AdapterControlAttachmentImport_SwipeHelper myImplementSwipeHelper;
            private ViewElement _element;
            private int groupPosition;
            private int _flagView;
            private ControllerDetailAttachFile CTRLDetailAttachFile = new ControllerDetailAttachFile();
            public CustomDeleteButtonClick(ControlInputAttachmentVertical.AdapterControlAttachmentImport_SwipeHelper myImplementSwipeHelper, ViewElement _element, int groupPosition, int _flagView)
            {
                this.myImplementSwipeHelper = myImplementSwipeHelper;
                this._element = _element;
                this.groupPosition = groupPosition;
                this._flagView = _flagView;
            }

            public void OnClick(int position)
            {
                List<BeanAttachFile> _lstAttachFileFull = JsonConvert.DeserializeObject<List<BeanAttachFile>>(_element.Value.Trim());
                List<BeanGroupAttachFile> _lstGroup = CTRLDetailAttachFile.CloneListAttToGroup(_lstAttachFileFull);
                BeanAttachFile clickedItem = _lstGroup[groupPosition].AttachFiles[position];
                position = CmmDroidFunction.FindIndexOfItemInListAttach(clickedItem, _lstAttachFileFull);
                if (position != -1)
                    MinionActionCore.OnElementFormClickEvent_WithInnerAction(null, new MinionActionCore.ElementFormClick_WithInnerAction(_element, (int)EnumFormControlInnerAction.ControlInputAttachmentVertical_InnerActionID.Delete, position, _flagView));
            }
        }
        internal class CustomEditButtonClick : UnderLayoutButtonListener
        {
            private ControlInputAttachmentVertical.AdapterControlAttachmentImport_SwipeHelper myImplementSwipeHelper;
            private ViewElement _element;
            private int groupPosition;
            private int _flagView;
            private ControllerDetailAttachFile CTRLDetailAttachFile = new ControllerDetailAttachFile();
            public CustomEditButtonClick(ControlInputAttachmentVertical.AdapterControlAttachmentImport_SwipeHelper myImplementSwipeHelper, ViewElement _element, int groupPosition, int _flagView)
            {
                this.myImplementSwipeHelper = myImplementSwipeHelper;
                this._element = _element;
                this.groupPosition = groupPosition;
                this._flagView = _flagView;
            }

            public void OnClick(int position)
            {
                List<BeanAttachFile> _lstAttachFileFull = JsonConvert.DeserializeObject<List<BeanAttachFile>>(_element.Value.Trim());
                List<BeanGroupAttachFile> _lstGroup = CTRLDetailAttachFile.CloneListAttToGroup(_lstAttachFileFull);
                BeanAttachFile clickedItem = _lstGroup[groupPosition].AttachFiles[position];
                position = CmmDroidFunction.FindIndexOfItemInListAttach(clickedItem, _lstAttachFileFull);
                if (position != -1)
                    MinionActionCore.OnElementFormClickEvent_WithInnerAction(null, new MinionActionCore.ElementFormClick_WithInnerAction(_element, (int)EnumFormControlInnerAction.ControlInputAttachmentVertical_InnerActionID.Edit, position, _flagView));
            }
        }
        #endregion
    }


}