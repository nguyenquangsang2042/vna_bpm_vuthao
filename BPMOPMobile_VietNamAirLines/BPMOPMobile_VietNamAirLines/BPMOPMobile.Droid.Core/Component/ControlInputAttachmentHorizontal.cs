using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using Newtonsoft.Json;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ControlInputAttachmentHorizontal : ControlBase
    {
        private Context _context { get; set; }
        private LinearLayout _parentView { get; set; }
        private LinearLayout _lnTitleImport { get; set; } // chứa _tvTitle và _lnImport
        private LinearLayout _lnImport { get; set; }
        private LinearLayout _lnFileInfo { get; set; }
        private LinearLayout _lnFileInfoChild1 { get; set; }
        private LinearLayout _lnFileInfoChild2 { get; set; }
        private LinearLayout _lnFileInfoChild3 { get; set; }
        private TextView _tvFileInfoChild1 { get; set; }
        private TextView _tvFileInfoChild2 { get; set; }
        private TextView _tvFileInfoChild3 { get; set; }
        private ImageView _imgImport { get; set; }
        private TextView _tvImport { get; set; }
        private RecyclerView _recyAttachment { get; set; }
        private ViewElement _element { get; set; }

        public int _widthScreenTablet = -1;
        public string[] mimeTypes;
        private StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
        private AdapterControlInputAttachmentHorizontal _adapterControlInputAttachmentHorizontal; // Adapter của Recy List file
        private List<BeanAttachFile> _lstAttachment = new List<BeanAttachFile>();

        public ControlInputAttachmentHorizontal(Activity _mainAct, LinearLayout _parentView, ViewElement _element, int _widthScreenTablet) : base(_mainAct)
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
            _lnTitleImport = new LinearLayout(_mainAct);
            _lnFileInfo = new LinearLayout(_mainAct);
            _lnFileInfoChild1 = new LinearLayout(_mainAct);
            _lnFileInfoChild2 = new LinearLayout(_mainAct);
            _lnFileInfoChild3 = new LinearLayout(_mainAct);
            _tvFileInfoChild1 = new TextView(_mainAct);
            _tvFileInfoChild2 = new TextView(_mainAct);
            _tvFileInfoChild3 = new TextView(_mainAct);
            _lnImport = new LinearLayout(_mainAct);
            _imgImport = new ImageView(_mainAct);
            _tvImport = new TextView(_mainAct);

            _tvImport.SetTextSize(ComplexUnitType.Sp, 14);
            _tvImport.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clViolet)));
            _tvImport.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            _tvImport.Ellipsize = TextUtils.TruncateAt.End;
            _tvImport.Gravity = GravityFlags.Center;
            _tvImport.Text = "Tạo mới";

            _tvFileInfoChild1.SetTextSize(ComplexUnitType.Sp, 14);
            _tvFileInfoChild1.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
            _tvFileInfoChild1.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            _tvFileInfoChild1.Ellipsize = TextUtils.TruncateAt.End;
            _tvFileInfoChild1.Gravity = GravityFlags.Center;
            _tvFileInfoChild1.Text = "Tên File";

            _tvFileInfoChild2.SetTextSize(ComplexUnitType.Sp, 14);
            _tvFileInfoChild2.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
            _tvFileInfoChild2.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            _tvFileInfoChild2.Ellipsize = TextUtils.TruncateAt.End;
            _tvFileInfoChild2.Gravity = GravityFlags.Center;
            _tvFileInfoChild2.Text = "Loại tài liệu";

            _imgImport.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clViolet)));

            _recyAttachment.Id = CmmDroidVariable.M_OnActivityResultFileChooserCode;
            _lnFileInfo.Orientation = Orientation.Horizontal;
            _lnTitleImport.Orientation = Orientation.Horizontal;
            _lnTitleImport.SetGravity(GravityFlags.Center);
            _lnImport.Orientation = Orientation.Horizontal;
            _lnImport.SetGravity(GravityFlags.Right);
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
                                            // _lnContent.RemoveView(_tvTitle);

            #region Linear Title + Import
            int _padding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 6, _mainAct.Resources.DisplayMetrics);
            LinearLayout.LayoutParams _paramslnTitleImport = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams _paramsTitle = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams _paramslnImport = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
            LinearLayout.LayoutParams _paramsimgImport = new LinearLayout.LayoutParams((int)CmmDroidFunction.ConvertDpToPixel(18, frame.Context), (int)CmmDroidFunction.ConvertDpToPixel(18, frame.Context));
            _paramsimgImport.SetMargins(_padding, 0, 2 * _padding, 0);

            _lnTitleImport.LayoutParameters = _paramslnTitleImport;
            _tvTitle.LayoutParameters = _paramsTitle;
            _lnImport.LayoutParameters = _paramslnImport;
            _imgImport.LayoutParameters = _paramsimgImport;

            _imgImport.Background = ContextCompat.GetDrawable(frame.Context, Resource.Drawable.icon_tab_createtask);
            _imgImport.SetPadding(_padding, 0, _padding, _padding);

            _recyAttachment.SetRecycledViewPool(new RecyclerView.RecycledViewPool());
            _lnImport.SetPadding(_padding, 0, 2 * _padding, 0);
            _lnImport.Click += Click_lnImport;


            _lnImport.AddView(_imgImport);
            _lnImport.AddView(_tvImport);
            //_lnTitleImport.AddView(_tvTitle);
            _lnTitleImport.AddView(_lnImport);
            #endregion

            LinearLayout.LayoutParams _paramsRecy = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            _paramsRecy.SetMargins(0, 0, 0, 2 * _padding);
            _recyAttachment.LayoutParameters = _paramsRecy;

            frame.AddView(_lnTitleImport);
            frame.AddView(_recyAttachment);
            frame.AddView(_lnLine);
        }

        private void Click_lnImport(object sender, EventArgs e)
        {
            try
            {
                if (_parentView != null)
                {
                    MinionActionCore.OnElementFormClick(null, new MinionActionCore.ElementFormClick(_element));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - Click_lnImport - Error: " + ex.Message);
#endif
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

            _lstAttachment = JsonConvert.DeserializeObject<List<BeanAttachFile>>(data);
            if (_lstAttachment == null || _lstAttachment.Count == 0)
            {
                _lstAttachment = new List<BeanAttachFile>();
            }
            _adapterControlInputAttachmentHorizontal = new AdapterControlInputAttachmentHorizontal(_mainAct, _lstAttachment);
            //_adapterControlInputAttachmentHorizontal.CustomItemClick_RemoveItem += Click_ItemAttach_RemoveItem;
            //_adapterControlInputAttachmentHorizontal.CustomItemClick_AddCategory += Click_ItemAttach_AddCategory;
            _recyAttachment.SetLayoutManager(staggeredGridLayoutManager);
            _recyAttachment.SetAdapter(_adapterControlInputAttachmentHorizontal);
        }

        private void Click_ItemAttach_AddCategory(object sender, BeanAttachFile e)
        {
            try
            {
                if (_parentView != null)
                {
                    MinionActionCore.OnElementClickImportFileDetailCreateWorkflow_Action(null, new MinionActionCore.ElementImportFileClick_Action(_element, e, "AddCategory"));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControlAttachmentImport - Click_ItemAttach_AddCategory - Error: " + ex.Message);
#endif
            }
        }

        private void Click_ItemAttach_RemoveItem(object sender, BeanAttachFile e)
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

        /// <summary>
        /// Event khi click vào Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

    }

    public class AdapterControlInputAttachmentHorizontal : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public Activity _mainAct;
        public List<BeanAttachFile> _lstAttachment;

        public AdapterControlInputAttachmentHorizontal(Activity _mainAct, List<BeanAttachFile> _lstAttachment)
        {
            this._mainAct = _mainAct;
            this._lstAttachment = _lstAttachment;
        }
        private void OnItemClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
        public override int ItemCount => _lstAttachment.Count;
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemControlInputAttachmentHorizontal, parent, false);
            DisplayMetrics dm = _mainAct.Resources.DisplayMetrics;
            int _widthRow = dm.WidthPixels / 3; // hiển thị tối da 3 item

            itemView.LayoutParameters = new ViewGroup.LayoutParams(_widthRow, ViewGroup.LayoutParams.WrapContent);

            HolderControlInputAttachmentHorizontal holder = new HolderControlInputAttachmentHorizontal(itemView, OnItemClick);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            HolderControlInputAttachmentHorizontal vh = holder as HolderControlInputAttachmentHorizontal;
            if (!String.IsNullOrEmpty(_lstAttachment[position].Title))
                vh._tvFileName.Text = _lstAttachment[position].Title;
            else
                vh._tvFileName.Text = "";

        }
    }
    public class HolderControlInputAttachmentHorizontal : RecyclerView.ViewHolder
    {
        public ImageView _imgAvatar { get; set; }
        public TextView _tvFileName { get; set; }
        public HolderControlInputAttachmentHorizontal(View itemview, Action<int> listener) : base(itemview)
        {
            _imgAvatar = itemview.FindViewById<ImageView>(Resource.Id.img_ItemControlInputAttachmentHorizontal_Avatar);
            _tvFileName = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlInputAttachmentHorizontal_FileName);
            itemview.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}
