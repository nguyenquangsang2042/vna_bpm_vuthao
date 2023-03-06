using System;
using System.Collections.Generic;
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
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using Newtonsoft.Json;
using Refractored.Controls;
using SQLite;
using static BPMOPMobile.Droid.Core.Class.MinionActionCore;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ComponentComment : ComponentBase
    {
        private Activity _mainAct { get; set; }
        private Context _context { get; set; }
        private LinearLayout _parentView { get; set; }
        private TextView _tvTitle { get; set; }
        private CardView _cardViewParentComment { get; set; }
        private LinearLayout _lnParentComment { get; set; }
        private LinearLayout _lnParentCommentContent { get; set; }
        private LinearLayout _lnParentCommentAttach { get; set; }
        private LinearLayout _lnTopLine { get; set; }
        private EditText _edtParentComment { get; set; }
        private ImageView _imgAttachParentComment { get; set; }
        private ImageView _imgCommentParentComment { get; set; }
        private RecyclerView _recyAttachParentComment_Other { get; set; } // List đính kèm của Image
        private RecyclerView _recyAttachParentComment_Image { get; set; } // List đính kèm của Image
        private RecyclerView _recyListComment { get; set; }

        public event EventHandler CustomClick_CommentParent_ImgAttach;
        public event EventHandler<CommentEventArgs> CustomClick_CommentParent_ImgComment; // Click cmt -> phải có nội dung cmt và list file

        public event EventHandler<BeanAttachFile> CustomItemClick_ItemListAttachParent_Detail; // xem chi tiết attach file parent
        public event EventHandler<BeanAttachFile> CustomItemClick_ItemListAttachParent_Delete; // xóa attach file parent

        public event EventHandler<BeanComment> CustomItemClick_ItemListComment_tvLike; // Bấm vào like trên comment
        public event EventHandler<BeanComment> CustomItemClick_ItemListComment_tvReply; // bấm vào reply trên comment
        public event EventHandler<BeanAttachFile> CustomItemClick_ItemListComment_AttachDetail; // bấm vào file đính kèm trên comment

        private AdapterParentAttachFile _adapterParentAttach;
        private AdapterParentAttachFile_Image _adapterParentAttach_Image;
        private AdapterListComment _adapterListComment;

        private List<BeanComment> _lstComment = new List<BeanComment>();
        private List<BeanAttachFile> _lstParentAttach = new List<BeanAttachFile>();
        private bool _IsReplyView;
        private string _parentCommentContent = "";

        private LinearLayout.LayoutParams _paramsRecyComment;

        private bool _FlagRecalculateView = false; // nếu đặt trong scrollview thì bật cái này lên

        public ComponentComment(Activity _mainAct, LinearLayout _parentView, List<BeanComment> _lstComment, bool _IsReplyView = false)
        {
            this._mainAct = _mainAct;
            this._parentView = _parentView;
            this._lstComment = _lstComment;
            this._IsReplyView = _IsReplyView;
            InitializeComponent();
        }

        public void InitFlagRecalculateView(bool _FlagRecalculateView)
        {
            this._FlagRecalculateView = _FlagRecalculateView;
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            _tvTitle = new TextView(_mainAct);
            _cardViewParentComment = new CardView(_mainAct);
            _lnParentComment = new LinearLayout(_mainAct);
            _lnParentCommentContent = new LinearLayout(_mainAct);
            _lnParentCommentAttach = new LinearLayout(_mainAct);
            _lnTopLine = new LinearLayout(_mainAct);
            _edtParentComment = new EditText(_mainAct);
            _imgAttachParentComment = new ImageView(_mainAct);
            _imgCommentParentComment = new ImageView(_mainAct);
            _recyAttachParentComment_Image = new RecyclerView(_mainAct);
            _recyAttachParentComment_Other = new RecyclerView(_mainAct);
            _recyListComment = new RecyclerView(_mainAct);


            _tvTitle.SetTextSize(ComplexUnitType.Sp, 12);
            _tvTitle.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
            _tvTitle.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            _tvTitle.Ellipsize = TextUtils.TruncateAt.End;
            _tvTitle.Gravity = GravityFlags.Left;

            _edtParentComment.SetTextSize(ComplexUnitType.Sp, 14);
            _edtParentComment.SetMaxLines(3);
            _edtParentComment.SetLines(3);
            _edtParentComment.SetSingleLine(false);
            _edtParentComment.ImeOptions = Android.Views.InputMethods.ImeAction.None;
            _edtParentComment.VerticalScrollBarEnabled = true;
            _edtParentComment.InputType = InputTypes.ClassText | InputTypes.TextFlagMultiLine;
            _edtParentComment.Gravity = GravityFlags.Top;
            _edtParentComment.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Italic);
            _edtParentComment.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.transparent))); // bỏ underline 

            _cardViewParentComment.SetBackgroundResource(Resource.Drawable.textcornerstrokegraywhitesolid2);
            _cardViewParentComment.UseCompatPadding = true;
            _cardViewParentComment.Radius = 5f;

            //_lnParentComment.SetBackgroundResource(Resource.Drawable.textcornerstrokegraywhitesolid2);
            _lnParentComment.Orientation = Orientation.Vertical;

            _lnParentCommentContent.Orientation = Orientation.Horizontal;
            _lnParentCommentAttach.Orientation = Orientation.Vertical;

            _lnTopLine.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clControlGrayLight)));
            _lnTopLine.Orientation = Orientation.Vertical;

            _imgAttachParentComment.SetImageResource(Resource.Drawable.icon_ver2_attach);
            _imgCommentParentComment.SetImageResource(Resource.Drawable.icon_ver2_comment);


            _edtParentComment.Hint = CmmFunction.GetTitle("TEXT_HINT_REQUIRE_COMMENT", "Nhập ý kiến");

        }

        public override void InitializeFrameView(LinearLayout frame)
        {
            base.InitializeFrameView(frame);

            int _padding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 3, _mainAct.Resources.DisplayMetrics);
            LinearLayout.LayoutParams _paramsFrame = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams _paramsParentComment = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams _paramsEdtParentComment = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WrapContent, 1);
            LinearLayout.LayoutParams _paramsImgParentComment = new LinearLayout.LayoutParams((int)CmmDroidFunction.ConvertDpToPixel(30, frame.Context), (int)CmmDroidFunction.ConvertDpToPixel(30, frame.Context));
            _paramsRecyComment = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams _paramslnTopLine = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, (int)CmmDroidFunction.ConvertDpToPixel(1, frame.Context));

            _paramsParentComment.SetMargins(0, _padding, 0, 2 * _padding);
            _paramsEdtParentComment.SetMargins(0, 0, 2 * _padding, 0);
            _paramsImgParentComment.SetMargins(0, 2 * _padding, 3 * _padding, 0);
            _paramsRecyComment.SetMargins(0, _padding, 0, 0);
            _paramslnTopLine.SetMargins(0, _padding, 0, 2 * _padding);

            //_lnParentComment.LayoutParameters = _paramsParentComment;
            //_lnParentCommentContent.LayoutParameters = _paramsParentComment;
            //_lnParentCommentAttach.LayoutParameters = _paramsParentComment;

            _edtParentComment.LayoutParameters = _paramsEdtParentComment;
            _imgAttachParentComment.LayoutParameters = _paramsImgParentComment;
            _imgCommentParentComment.LayoutParameters = _paramsImgParentComment;

            _recyAttachParentComment_Other.LayoutParameters = _paramsParentComment;
            _recyAttachParentComment_Image.LayoutParameters = _paramsParentComment;

            _lnTopLine.LayoutParameters = _paramslnTopLine;
            _recyListComment.LayoutParameters = _paramsParentComment;

            _tvTitle.SetPadding(0, _padding, 0, 2 * _padding);
            _lnParentComment.SetPadding(_padding, _padding, _padding, _padding);
            _imgAttachParentComment.SetPadding(2 * _padding, 2 * _padding, 2 * _padding, 2 * _padding);
            _imgCommentParentComment.SetPadding(2 * _padding, 2 * _padding, 2 * _padding, 2 * _padding);

            _lnParentCommentContent.AddView(_edtParentComment);
            _lnParentCommentContent.AddView(_imgAttachParentComment);
            _lnParentCommentContent.AddView(_imgCommentParentComment);
            _lnParentCommentAttach.AddView(_recyAttachParentComment_Image);
            _lnParentCommentAttach.AddView(_recyAttachParentComment_Other);
            _lnParentComment.AddView(_lnParentCommentContent);
            _lnParentComment.AddView(_lnParentCommentAttach);
            //_lnRecyListComment.AddView(_recyListComment);

            _edtParentComment.TextChanged += TextChanged_edtParentComment;
            _imgAttachParentComment.Click += Click_imgParentAttach;
            _imgCommentParentComment.Click += Click_imgParentComment;

            if (_IsReplyView == false)
            {
                _cardViewParentComment.AddView(_lnParentComment);
                frame.SetPadding(2 * _padding, 2 * _padding, 2 * _padding, 2 * _padding);
                frame.AddView(_lnTopLine);
                frame.AddView(_tvTitle);
                frame.AddView(_cardViewParentComment);
            }
            else // View reply  ko cần padding -> đụng bottom keyboard
            {
                frame.SetPadding(0, 0, 0, 0);
            }
            //frame.AddView(_lnRecyListComment);
            frame.AddView(_recyListComment);
        }

        public override void SetTitle()
        {
            base.SetTitle();

            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                _tvTitle.Text = CmmFunction.GetTitle("TEXT_COMMENT", "Bình luận");
            else
                _tvTitle.Text = CmmFunction.GetTitle("TEXT_COMMENT", "Comment");
        }

        public override void SetValue()
        {
            try
            {
                base.SetValue();

                if (!String.IsNullOrEmpty(_parentCommentContent))
                    _edtParentComment.Text = _parentCommentContent;

                if (_lstParentAttach != null && _lstParentAttach.Count >= 0)
                {
                    if (_lstParentAttach.Count > 0)
                        _lstParentAttach = CmmDroidFunction.ClassifyListAttachFile(_lstParentAttach).ToList(); // phân loại nếu ảnh thì IsImage = true

                    _adapterParentAttach = new AdapterParentAttachFile(_mainAct, _parentView.Context, _lstParentAttach, true);
                    _adapterParentAttach.CustomItemClick_Detail += Click_ItemListAttachParent_Detail;
                    _adapterParentAttach.CustomItemClick_Delete += Click_ItemListAttachParent_Delete;
                    _recyAttachParentComment_Other.SetAdapter(_adapterParentAttach);
                    _recyAttachParentComment_Other.SetLayoutManager(new LinearLayoutManager(_parentView.Context, LinearLayoutManager.Vertical, false));

                    _adapterParentAttach_Image = new AdapterParentAttachFile_Image(_mainAct, _parentView.Context, _lstParentAttach, true);
                    _adapterParentAttach_Image.CustomItemClick_Detail += Click_ItemListAttachParent_Detail;
                    _adapterParentAttach_Image.CustomItemClick_Delete += Click_ItemListAttachParent_Delete;
                    _recyAttachParentComment_Image.SetAdapter(_adapterParentAttach_Image);
                    _recyAttachParentComment_Image.SetLayoutManager(new LinearLayoutManager(_parentView.Context, LinearLayoutManager.Horizontal, false));
                }

                if (_lstComment != null && _lstComment.Count > 0)
                {
                    _lstComment = CloneListComment(_lstComment);

                    if (_FlagRecalculateView == true) // đặt trong scrollview -> cần tính lại
                        _adapterListComment = new AdapterListComment(_mainAct, _parentView.Context, _lstComment, _IsReplyView, _mainAct.Resources.DisplayMetrics.WidthPixels - (int)CmmDroidFunction.ConvertDpToPixel(12, _parentView.Context));
                    else
                        _adapterListComment = new AdapterListComment(_mainAct, _parentView.Context, _lstComment, _IsReplyView);

                    _adapterListComment.CustomItemClick_Like += Click_ItemListComment_tvLike;
                    _adapterListComment.CustomItemClick_Reply += Click_ItemListComment_tvReply;
                    _adapterListComment.CustomItemClick_Attach_Detail += Click_ItemListComment_AttachDetail;

                    _recyListComment.SetItemViewCacheSize(_lstComment.Count);
                    _recyListComment.SetAdapter(_adapterListComment);
                    _recyListComment.SetLayoutManager(new LinearLayoutManager(_parentView.Context, LinearLayoutManager.Vertical, false));


                    if (_FlagRecalculateView == true) // nếu trong scrollview
                    {
                        _recyListComment.Post(() =>
                        {
                            //// tổng chiều cao đã bị ẩn của các LN Action Child Item - Set cứng view là 20dp           
                            //int _lnActionGoneHeight = ((int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 20, _mainAct.Resources.DisplayMetrics) + 5) * _adapterListComment.GetChildCommentCount();
                            ////int _recyHeight = CmmDroidFunction.GetRecyclerViewHeight(_mainAct, _recyListComment) - _lnActionGoneHeight;

                            //Update Ver2 lúc nào cũng hiện trả lời + thích -> thua
                            int _recyHeight = CmmDroidFunction.GetRecyclerViewHeight(_mainAct, _recyListComment);
                            int _screenHeight = _mainAct.Resources.DisplayMetrics.HeightPixels;

                            if (_recyHeight > _screenHeight) // Limit lại và cho scroll inside
                            {
                                _paramsRecyComment.Width = LinearLayout.LayoutParams.WrapContent;
                                _paramsRecyComment.Height = _screenHeight;
                                _recyListComment.LayoutParameters = _paramsRecyComment;
                            }
                            else
                            {
                                _paramsRecyComment.Width = LinearLayout.LayoutParams.WrapContent;
                                _paramsRecyComment.Height = _recyHeight;
                                _recyListComment.LayoutParameters = _paramsRecyComment;
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetValue", ex);
#endif
            }
        }

        private void TextChanged_edtParentComment(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(_edtParentComment.Text))
                    _edtParentComment.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Italic);
                else
                    _edtParentComment.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);

                _parentCommentContent = _edtParentComment.Text;
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "TextChanged_edtParentComment", ex);
#endif
            }
        }

        private void Click_imgParentAttach(object sender, EventArgs e)
        {
            try
            {
                if (CustomClick_CommentParent_ImgAttach != null)
                    CustomClick_CommentParent_ImgAttach(this, null);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgParentAttach", ex);
#endif
            }
        }

        private void Click_imgParentComment(object sender, EventArgs e)
        {
            try
            {
                if (CustomClick_CommentParent_ImgComment != null)
                    CustomClick_CommentParent_ImgComment(this, new CommentEventArgs(_parentCommentContent, _lstParentAttach));
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgParentComment", ex);
#endif
            }
        }

        private void Click_ItemListAttachParent_Delete(object sender, BeanAttachFile e)
        {
            try
            {
                if (CustomItemClick_ItemListAttachParent_Delete != null)
                    CustomItemClick_ItemListAttachParent_Delete(sender, e);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemListAttachParent_Delete", ex);
#endif
            }
        }

        private void Click_ItemListAttachParent_Detail(object sender, BeanAttachFile e)
        {
            try
            {
                if (CustomItemClick_ItemListAttachParent_Detail != null)
                    CustomItemClick_ItemListAttachParent_Detail(sender, e);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemListAttachParent_Detail", ex);
#endif
            }
        }

        private void Click_ItemListComment_tvLike(object sender, BeanComment e)
        {
            try
            {
                if (CustomItemClick_ItemListComment_tvLike != null)
                    CustomItemClick_ItemListComment_tvLike(sender, e);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemListComment_tvLike", ex);
#endif
            }
        }

        private void Click_ItemListComment_tvReply(object sender, BeanComment e)
        {
            try
            {
                if (CustomItemClick_ItemListComment_tvReply != null)
                    CustomItemClick_ItemListComment_tvReply(sender, e);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemListComment_tvReply", ex);
#endif
            }
        }

        private void Click_ItemListComment_AttachDetail(object sender, BeanAttachFile e)
        {
            try
            {
                if (CustomItemClick_ItemListComment_AttachDetail != null)
                    CustomItemClick_ItemListComment_AttachDetail(sender, e);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemListComment_AttachDetail", ex);
#endif
            }
        }

        public void UpdateListParentAttach(List<BeanAttachFile> _lstParentAttach)
        {
            try
            {
                this._lstParentAttach = CmmDroidFunction.ClassifyListAttachFile(_lstParentAttach).ToList(); // phân loại nếu ảnh thì IsImage = true

                if (_adapterParentAttach_Image != null)
                {
                    _adapterParentAttach_Image.UpdateListAttach(this._lstParentAttach);
                    _adapterParentAttach_Image.NotifyDataSetChanged();
                }

                if (_adapterParentAttach != null)
                {
                    _adapterParentAttach.UpdateListAttach(this._lstParentAttach);
                    _adapterParentAttach.NotifyDataSetChanged();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "UpdateListParentAttach", ex);
#endif
            }
        }

        /// <summary>
        /// Clone về đúng dạng cho adapter
        /// </summary>
        /// <param name="_lstComment"></param>
        /// <returns></returns>
        public List<BeanComment> CloneListComment(List<BeanComment> _lstComment)
        {
            List<BeanComment> _res = new List<BeanComment>();
            try
            {
                _lstComment = _lstComment.OrderByDescending(x => x.Created).ToList();
                List<BeanComment> _lstParent = _lstComment.Where(x => x.ParentCommentId == null).ToList(); // List cha thằng mới nhất lên trên cùng 12/01/20
                foreach (BeanComment parentItem in _lstParent)
                {
                    List<BeanComment> _lstChild = _lstComment.Where(x => x.ParentCommentId == parentItem.ID).OrderBy(x => x.Created).ToList(); // List con thằng cũ nhất lên trên cùng 12/01/20
                    _res.Add(parentItem);
                    _res.AddRange(_lstChild);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "CloneListComment", ex);
#endif
            }
            return _res;
        }

        public string getCurrentParentCommentContent()
        {
            return _parentCommentContent;
        }

        public void UpdateCurrentParentCommentContent(string content)
        {
            this._parentCommentContent = content;
        }

        #region Adapter Parent Attach

        /// <summary>
        /// Adapter của đính kèm Ảnh
        /// </summary>
        public class AdapterParentAttachFile_Image : RecyclerView.Adapter
        {
            private Activity _mainAct;
            private Context _context;
            private List<BeanAttachFile> _lstData = new List<BeanAttachFile>();

            public event EventHandler<BeanAttachFile> CustomItemClick_Detail;
            public event EventHandler<BeanAttachFile> CustomItemClick_Delete;

            bool _showImgDelete = true;
            public int ViewHolderHeight = -1;

            public AdapterParentAttachFile_Image(Activity _mainAct, Context _context, List<BeanAttachFile> _lstData, bool _showImgDelete)
            {
                try
                {
                    this._mainAct = _mainAct;
                    this._context = _context;
                    this._showImgDelete = _showImgDelete;

                    // chỉ lấy file ảnh
                    this._lstData = _lstData.Where(x => x.IsImage == true).ToList();
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError("AdapterParentAttachFile_Image", "AdapterParentAttachFile_Image", ex);
#endif
                }
            }

            private void OnItemClickDetail(int position)
            {
                if (CustomItemClick_Detail != null)
                    CustomItemClick_Detail(this, _lstData[position]);

                NotifyDataSetChanged();
            }

            private void OnItemClickDelete(int position)
            {
                if (CustomItemClick_Delete != null)
                    CustomItemClick_Delete(this, _lstData[position]);

                //RemoveItemListIsClicked(_lstUser[position]);
                NotifyDataSetChanged();
            }

            public void UpdateListAttach(List<BeanAttachFile> _lstData)
            {
                try
                {
                    if (this._lstData != null)
                        this._lstData = new List<BeanAttachFile>();

                    this._lstData = _lstData.Where(x => x.IsImage == true).ToList();
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "UpdateListAttach", ex);
#endif
                }
            }

            public override int ItemCount => _lstData.Count;

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemComponentComment_AttachParent_Image, parent, false);
                AdapterCommentAttachFile_ImageHolder holder = new AdapterCommentAttachFile_ImageHolder(itemView, OnItemClickDetail, OnItemClickDelete);
                return holder;
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                try
                {
                    AdapterCommentAttachFile_ImageHolder _holder = holder as AdapterCommentAttachFile_ImageHolder;

                    if (_showImgDelete == true)
                        _holder._imgDelete.Visibility = ViewStates.Visible;
                    else
                        _holder._imgDelete.Visibility = ViewStates.Gone;

                    //CmmDroidFunction.LoadImageUniversalURL(_mainAct, _context, _holder._imageContent, CmmVariable.M_Domain + _lstData[position].Url);

                    if (!String.IsNullOrEmpty(_lstData[position].Path)) // offline từ máy
                    {

                        string filePath = System.IO.Path.Combine(CmmVariable.M_Folder_Avatar + "/", System.IO.Path.GetFileName(_lstData[position].Path) ?? throw new InvalidOperationException());

                        Bitmap myBitmap = BitmapHelper.LoadAndResizeBitmap(_lstData[position].Path, 80, 80);
                        if (myBitmap != null)
                        {
                            _holder._imageContent.SetImageBitmap(myBitmap);
                            _holder._imageContent.StartAnimation(AnimationUtils.LoadAnimation(_mainAct, Resource.Animation.anim_clickview));
                        }
                        else
                            _holder._imageContent.SetImageResource(Resource.Drawable.img_ver3_error);
                    }
                    else // từ server
                    {
                        Action action = new Action(() =>
                        {
                            CmmDroidFunction.SetContentToImageView(_mainAct, _holder._imageContent, _lstData[position].Url, 150, Resource.Drawable.img_ver3_error);
                        });
                        new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnBindViewHolder", ex);
#endif
                }
            }

            public class AdapterCommentAttachFile_ImageHolder : RecyclerView.ViewHolder
            {
                public LinearLayout _lnAll { get; set; }
                public RelativeLayout _relaImage { get; set; }
                public TextView _tvTitle { get; set; }
                public ImageView _imgDelete { get; set; }
                public ImageView _imageContent { get; set; }
                public AdapterCommentAttachFile_ImageHolder(View itemview, Action<int> listenerDetail, Action<int> listenerDelete) : base(itemview)
                {
                    _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemComponentComment_AttachParent_Image_All);
                    _relaImage = itemview.FindViewById<RelativeLayout>(Resource.Id.rela_ItemComponentComment_AttachParent_Image);
                    _imageContent = itemview.FindViewById<ImageView>(Resource.Id.img_ItemComponentComment_AttachParent_Image);
                    _imgDelete = itemview.FindViewById<ImageView>(Resource.Id.img_ItemComponentComment_AttachParent_Image_Delete);

                    _imageContent.Click += (sender, e) => listenerDetail(base.LayoutPosition);
                    _imgDelete.Click += (sender, e) => listenerDelete(base.LayoutPosition);
                }
            }
        }

        /// <summary>
        /// Adapter của đính kèm khác
        /// </summary>
        public class AdapterParentAttachFile : RecyclerView.Adapter
        {
            private Activity _mainAct;
            private Context _context;
            private List<BeanAttachFile> _lstData = new List<BeanAttachFile>();
            private ControllerBase CTRLBase = new ControllerBase();

            public event EventHandler<BeanAttachFile> CustomItemClick_Detail;
            public event EventHandler<BeanAttachFile> CustomItemClick_Delete;

            bool _showImgDelete = true;
            public int ViewHolderHeight = -1;

            public AdapterParentAttachFile(Activity _mainAct, Context _context, List<BeanAttachFile> _lstData, bool _showImgDelete)
            {
                try
                {
                    this._mainAct = _mainAct;
                    this._context = _context;
                    this._showImgDelete = _showImgDelete;
                    // lấy các file ko phải ảnh
                    this._lstData = _lstData.Where(x => x.IsImage == false).ToList();
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError("AdapterParentAttachFile", "AdapterParentAttachFile", ex);
#endif
                }
            }

            private void OnItemClickDetail(int position)
            {
                if (CustomItemClick_Detail != null)
                    CustomItemClick_Detail(this, _lstData[position]);

                NotifyDataSetChanged();
            }

            private void OnItemClickDelete(int position)
            {
                if (CustomItemClick_Delete != null)
                    CustomItemClick_Delete(this, _lstData[position]);

                //RemoveItemListIsClicked(_lstUser[position]);
                NotifyDataSetChanged();
            }

            public void UpdateListAttach(List<BeanAttachFile> _lstData)
            {
                try
                {
                    if (this._lstData != null)
                        this._lstData = new List<BeanAttachFile>();

                    this._lstData = _lstData.Where(x => x.IsImage == false).ToList();
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "UpdateListAttach", ex);
#endif
                }
            }

            public override int ItemCount => _lstData.Count;

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemComponentComment_AttachParent, parent, false);
                AdapterCommentAttachFileHolder holder = new AdapterCommentAttachFileHolder(itemView, OnItemClickDetail, OnItemClickDelete);
                return holder;
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                try
                {
                    AdapterCommentAttachFileHolder _holder = holder as AdapterCommentAttachFileHolder;

                    if (!String.IsNullOrEmpty(_lstData[position].Url)) // check URL server trc -> nếu ko có thì xài Path
                        _holder._imgExtension.SetImageResource(CTRLBase.GetResourceIDAttachment(_lstData[position].Url));
                    else
                        _holder._imgExtension.SetImageResource(CTRLBase.GetResourceIDAttachment(_lstData[position].Path));

                    if (!String.IsNullOrEmpty(_lstData[position].Title))
                    {
                        _holder._tvTitle.Text = CmmDroidFunction.GetFormatTitleFile(_lstData[position].Title);
                    }

                    else
                        _holder._tvTitle.Text = "";

                    if (_showImgDelete == true)
                        _holder._imgDelete.Visibility = ViewStates.Visible;
                    else
                        _holder._imgDelete.Visibility = ViewStates.Gone;

                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnBindViewHolder", ex);
#endif
                }
            }

            public class AdapterCommentAttachFileHolder : RecyclerView.ViewHolder
            {
                public LinearLayout _lnAll { get; set; }
                public TextView _tvTitle { get; set; }
                public ImageView _imgDelete { get; set; }
                public ImageView _imgExtension { get; set; }
                public AdapterCommentAttachFileHolder(View itemview, Action<int> listenerDetail, Action<int> listenerDelete) : base(itemview)
                {
                    _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemComponentComment_AttachParent_All);
                    _tvTitle = itemview.FindViewById<TextView>(Resource.Id.tv_ItemComponentComment_AttachParent_Title);
                    _imgDelete = itemview.FindViewById<ImageView>(Resource.Id.img_ItemComponentComment_AttachParent_Delete);
                    _imgExtension = itemview.FindViewById<ImageView>(Resource.Id.img_ItemComponentComment_AttachParent_Extension);
                    _tvTitle.Click += (sender, e) => listenerDetail(base.LayoutPosition);
                    _imgDelete.Click += (sender, e) => listenerDelete(base.LayoutPosition);
                }
            }
        }
        #endregion

        #region Adapter List Comment
        public class AdapterListComment : RecyclerView.Adapter
        {
            private Activity _mainAct;
            private Context _context;
            private List<BeanComment> _lstData = new List<BeanComment>();
            private ControllerDetailProcess CTRLDetailProcess = new ControllerDetailProcess();
            private ControllerDetailCreateTask CTRLDetailCreateTask = new ControllerDetailCreateTask();
            public event EventHandler<BeanComment> CustomItemClick_Like;
            public event EventHandler<BeanComment> CustomItemClick_Reply;
            public event EventHandler<BeanAttachFile> CustomItemClick_Attach_Detail; // Click vào AttachFile của comment

            bool _IsReplyView = false;
            int _widthView = -1;

            public int _lnActionHeight = -1; // lưu lại để trừ tổng chiều cao list cho chính xác
            private void OnItemClick_Like(int position)
            {
                if (CustomItemClick_Like != null)
                    CustomItemClick_Like(this, _lstData[position]);
            }
            private void OnItemClick_Reply(int position)
            {
                if (CustomItemClick_Reply != null)
                    CustomItemClick_Reply(this, _lstData[position]);
            }
            private void click_ItemRecyAttach_Detail(object sender, BeanAttachFile e)
            {
                try
                {
                    if (CustomItemClick_Attach_Detail != null)
                        CustomItemClick_Attach_Detail(null, e);
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "click_ItemRecyAttach_Detail", ex);
#endif
                }
            }

            public int GetChildCommentCount()
            {
                int count = 0;
                try
                {

                    foreach (BeanComment _currentItem in _lstData)
                    {
                        if (!String.IsNullOrEmpty(_currentItem.ParentCommentId)) // CMT CHILD
                            count++;
                    }
                }
                catch (Exception) { }
                return count;
            }

            public AdapterListComment(Activity _mainAct, Context _context, List<BeanComment> _lstData, bool _IsReplyView, int _widthView = -1)
            {
                this._mainAct = _mainAct;
                this._context = _context;
                this._lstData = _lstData;
                this._IsReplyView = _IsReplyView;
                this._widthView = _widthView;
            }

            public override int ItemCount => _lstData.Count;

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemComponentComment_ListComment, parent, false);
                AdapterListCommentHolder holder = new AdapterListCommentHolder(itemView, OnItemClick_Like, OnItemClick_Reply, _widthView);
                return holder;
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                try
                {
                    AdapterListCommentHolder _holder = holder as AdapterListCommentHolder;

                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    {
                        _holder._tvReply.Text = CmmFunction.GetTitle("TEXT_REPLY", "Trả lời");
                    }
                    else
                    {
                        _holder._tvReply.Text = CmmFunction.GetTitle("TEXT_REPLY", "Reply");
                    }

                    BeanComment _currentItem = _lstData[position];
                    BeanUser _authorUser = CmmFunction.GetBeanUserByID(_currentItem.Author);

                    if (!String.IsNullOrEmpty(_currentItem.ParentCommentId)) // CMT CHILD
                    {
                        _holder._vwMarginLeft.Visibility = ViewStates.Visible;
                        //if (_IsReplyView == true)
                        //    _holder._lnAction.Visibility = ViewStates.Visible;
                        //else
                        //    _holder._lnAction.Visibility = ViewStates.Gone;
                    }
                    else // CMT PARENT
                    {
                        _holder._vwMarginLeft.Visibility = ViewStates.Gone;
                        _holder._lnAction.Visibility = ViewStates.Visible;
                    }

                    #region Line 1

                    if (_authorUser != null && !String.IsNullOrEmpty(_authorUser.ID))
                        CmmDroidFunction.SetAvataByBeanUser(_mainAct, _context, _authorUser.ID, "ID", _holder._imgAvatar, _holder._tvAvatar, 50);
                    else
                    {
                        _holder._tvAvatar.Visibility = ViewStates.Invisible;
                        _holder._imgAvatar.Visibility = ViewStates.Invisible;
                    }

                    if (_authorUser != null && !String.IsNullOrEmpty(_authorUser.FullName))
                        _holder._tvName.Text = _authorUser.FullName;
                    else
                        _holder._tvName.Text = "";

                    if (_currentItem.Created.HasValue)
                        _holder._tvDate.Text = CTRLDetailProcess.GetFormatDateLang(_currentItem.Created.Value);
                    else
                        _holder._tvDate.Text = "";

                    #endregion

                    #region Line 2

                    if (_authorUser != null)
                    {
                        try
                        {
                            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
                            string _queryPosition = String.Format(CTRLDetailCreateTask._queryBeanPosition, _authorUser.PositionID);
                            List<BeanPosition> _lstPosition = conn.Query<BeanPosition>(_queryPosition);
                            conn.Close();
                            if (_lstPosition != null && _lstPosition.Count > 0)
                                _holder._tvPosition.Text = _lstPosition[0].Title;
                            else
                                _holder._tvPosition.Text = "";
                        }
                        catch (Exception ex)
                        {
                            _holder._tvPosition.Text = "";
                        }

                    }
                    else
                        _holder._tvPosition.Text = "";

                    if (!String.IsNullOrEmpty(_currentItem.AttachFiles))
                    {
                        try
                        {
                            List<BeanAttachFile> _lstAttach = JsonConvert.DeserializeObject<List<BeanAttachFile>>(_currentItem.AttachFiles);

                            if (_lstAttach != null && _lstAttach.Count > 0)
                            {
                                _holder._lnAttach.Visibility = ViewStates.Visible;
                                _holder._tvAttachCount.Text = _lstAttach.Count.ToString();
                            }
                            else
                            {
                                _holder._lnAttach.Visibility = ViewStates.Invisible;
                            }
                        }
                        catch (Exception)
                        {
                            _holder._lnAttach.Visibility = ViewStates.Invisible;
                        }
                    }
                    else
                        _holder._lnAttach.Visibility = ViewStates.Invisible;

                    if (_currentItem.LikeCount > 0)
                    {
                        _holder._lnLike.Visibility = ViewStates.Visible;
                        _holder._tvLikeCount.Text = _currentItem.LikeCount.ToString();
                    }
                    else
                    {
                        _holder._lnLike.Visibility = ViewStates.Invisible;
                    }

                    #endregion

                    #region Line 3

                    if (!String.IsNullOrEmpty(_currentItem.Content))
                        _holder._tvComment.Text = _currentItem.Content;
                    else
                        _holder._tvComment.Text = "";

                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                        _holder._tvLike.Text = CmmFunction.GetTitle("TEXT_LIKE", "Thích");
                    else
                        _holder._tvLike.Text = CmmFunction.GetTitle("TEXT_LIKE", "Like");

                    if (_currentItem.IsLiked == true) // nếu like -> màu xanh
                    {
                        //_holder._tvLike.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlueEnable)));

                        if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                            _holder._tvLike.Text = CmmFunction.GetTitle("TEXT_UNLIKE", "Bỏ thích");
                        else
                            _holder._tvLike.Text = CmmFunction.GetTitle("TEXT_UNLIKE", "Unlike");
                    }
                    else
                    {
                        //_holder._tvLike.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                        if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                            _holder._tvLike.Text = CmmFunction.GetTitle("TEXT_LIKE", "Thích");
                        else
                            _holder._tvLike.Text = CmmFunction.GetTitle("TEXT_LIKE", "Like");
                    }

                    try
                    {
                        if (!String.IsNullOrEmpty(_currentItem.AttachFiles)) // Gắn List của Comment Parent
                        {
                            _holder._recyAttach_Image.Visibility = ViewStates.Visible;
                            _holder._recyAttach.Visibility = ViewStates.Visible;

                            List<BeanAttachFile> _lstAttachTemp = CmmDroidFunction.ClassifyListAttachFile(JsonConvert.DeserializeObject<List<BeanAttachFile>>(_currentItem.AttachFiles));

                            //AdapterListComment_AttachFile _adapterCommentAttach = new AdapterListComment_AttachFile(_mainAct, _context, _lstAttachTemp, false);
                            //_adapterCommentAttach.CustomItemClick_Detail += click_ItemRecyAttach_Detail;
                            //_holder._recyAttach.SetAdapter(_adapterCommentAttach);
                            //_holder._recyAttach.SetLayoutManager(new LinearLayoutManager(_context, LinearLayoutManager.Vertical, false));

                            AdapterParentAttachFile _adapterCommentAttach = new AdapterParentAttachFile(_mainAct, _context, _lstAttachTemp, false);
                            _adapterCommentAttach.CustomItemClick_Detail += click_ItemRecyAttach_Detail;
                            _holder._recyAttach.SetAdapter(_adapterCommentAttach);
                            _holder._recyAttach.SetLayoutManager(new LinearLayoutManager(_context, LinearLayoutManager.Vertical, false));


                            AdapterParentAttachFile_Image _adapterCommentAttach_Image = new AdapterParentAttachFile_Image(_mainAct, _context, _lstAttachTemp, false);
                            _adapterCommentAttach_Image.CustomItemClick_Detail += click_ItemRecyAttach_Detail;
                            _holder._recyAttach_Image.SetAdapter(_adapterCommentAttach_Image);
                            _holder._recyAttach_Image.SetLayoutManager(new LinearLayoutManager(_context, LinearLayoutManager.Vertical, false));
                        }
                        else
                        {
                            _holder._recyAttach_Image.Visibility = ViewStates.Gone;
                            _holder._recyAttach.Visibility = ViewStates.Gone;
                        }
                    }
                    catch (Exception ex)
                    {
                        _holder._recyAttach_Image.Visibility = ViewStates.Gone;
                        _holder._recyAttach.Visibility = ViewStates.Gone;
#if DEBUG
                        CmmDroidFunction.WriteTrackingError(this.GetType().Name, "AdapterListComment", ex);
#endif
                    }
                    #endregion

                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnBindViewHolder", ex);
#endif
                }
            }

            public class AdapterListCommentHolder : RecyclerView.ViewHolder
            {
                public LinearLayout _lnAll { get; set; }
                public View _vwMarginLeft { get; set; }
                public TextView _tvTitle { get; set; }
                public LinearLayout _lnParentComment { get; set; }
                public EditText _edtParentComment { get; set; }
                public ImageView _imgAttachParent { get; set; }
                public TextView _tvAttachCount { get; set; }
                public ImageView _imgCommentParent { get; set; }
                public LinearLayout _lnAttach { get; set; }
                public LinearLayout _lnLike { get; set; }
                public CircleImageView _imgAvatar { get; set; }
                public TextView _tvAvatar { get; set; }
                public TextView _tvName { get; set; }
                public TextView _tvDate { get; set; }
                public TextView _tvPosition { get; set; }
                public TextView _tvComment { get; set; }
                public TextView _tvLike { get; set; }
                public TextView _tvReply { get; set; }
                public TextView _tvLikeCount { get; set; }
                public LinearLayout _lnAction { get; set; }
                public RecyclerView _recyAttach_Image { get; set; }
                public RecyclerView _recyAttach { get; set; }
                public AdapterListCommentHolder(View itemview, Action<int> listenerLike, Action<int> listenerComment, int widthView) : base(itemview)
                {
                    _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemComponentComment_ListComment_All);
                    _vwMarginLeft = itemview.FindViewById<View>(Resource.Id.vw_ItemComponentComment_ListComment_MarginLeft);
                    _tvAttachCount = itemview.FindViewById<TextView>(Resource.Id.tv_ItemComponentComment_ListComment_AttachCount);
                    _lnAttach = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemComponentComment_ListComment_Attach);
                    _lnLike = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemComponentComment_ListComment_Like);
                    _imgAvatar = itemview.FindViewById<CircleImageView>(Resource.Id.img_ItemComponentComment_ListComment_Avata);
                    _tvAvatar = itemview.FindViewById<TextView>(Resource.Id.tv_ItemComponentComment_ListComment_Avata);
                    _tvName = itemview.FindViewById<TextView>(Resource.Id.tv_ItemComponentComment_ListComment_Name);
                    _tvDate = itemview.FindViewById<TextView>(Resource.Id.tv_ItemComponentComment_ListComment_Date);
                    _tvPosition = itemview.FindViewById<TextView>(Resource.Id.tv_ItemComponentComment_ListComment_Position);
                    _tvComment = itemview.FindViewById<TextView>(Resource.Id.tv_ItemComponentComment_ListComment_Comment);
                    _tvLike = itemview.FindViewById<TextView>(Resource.Id.tv_ItemComponentComment_ListComment_Like);
                    _tvReply = itemview.FindViewById<TextView>(Resource.Id.tv_ItemComponentComment_ListComment_Reply);
                    _tvLikeCount = itemview.FindViewById<TextView>(Resource.Id.tv_ItemComponentComment_ListComment_LikeCount);
                    _lnAction = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemComponentComment_ListComment_Action);
                    _recyAttach_Image = itemview.FindViewById<RecyclerView>(Resource.Id.recy_ItemComponentComment_ListComment_Attach_Image);
                    _recyAttach = itemview.FindViewById<RecyclerView>(Resource.Id.recy_ItemComponentComment_ListComment_Attach);

                    if (widthView != -1) // tính lại
                    {
                        _lnAll.LayoutParameters = new LinearLayout.LayoutParams(widthView, LinearLayout.LayoutParams.WrapContent);
                    }

                    _tvLike.Click += (sender, e) => listenerLike(base.LayoutPosition);
                    _tvReply.Click += (sender, e) => listenerComment(base.LayoutPosition);
                }
            }
        }
        #endregion
    }
}