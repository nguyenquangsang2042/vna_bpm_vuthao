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
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using Java.IO;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ControlAttachment : ControlBase
    {
        private LinearLayout _parentView { get; set; }
        private ViewElement _element { get; set; }
        private RecyclerView _recyAttachment;
        public int _widthScreenTablet = -1;
        List<KeyValuePair<string, string>> _lstAttachment = new List<KeyValuePair<string, string>>();

        public ControlAttachment(Activity _mainAct, LinearLayout _parentView, ViewElement _element, int _widthScreenTablet) : base(_mainAct)
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

            //if (_lnContent != null)
            //{
            //    _lnContent.Click += HandleTouchDown;
            //}
        }
        public override void InitializeFrameView(LinearLayout frame)
        {
            if (_element.Hidden == true) // Check xem có ẩn view hay không
                return;

            base.InitializeFrameView(frame);
            _tvValue.Visibility = ViewStates.Gone;
            _lnLine.Visibility = ViewStates.Gone;
            _lnContent.RemoveView(_lnLine); // Remove line ra để add lại sau

            _lnContent.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);



            frame.AddView(_recyAttachment);
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
            if (data.Contains(";#"))
            {
                var _arrAttachment = data.Split(new string[] { ";#" }, StringSplitOptions.None);
                if (_arrAttachment.Length > 2)
                {
                    for (var i = 0; i < _arrAttachment.Length; i += 2)
                    {
                        KeyValuePair<string, string> item = new KeyValuePair<string, string>(_arrAttachment[i], _arrAttachment[i + 1]);
                        _lstAttachment.Add(item);
                    }
                }
                else
                    _lstAttachment.Add(new KeyValuePair<string, string>(_arrAttachment[0], _arrAttachment[1]));
            }

            StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Horizontal);
            AdapterControlAttachmentHorizontal _adapterAttachFile = new AdapterControlAttachmentHorizontal(_mainAct, _lstAttachment, _widthScreenTablet);
            _adapterAttachFile.ItemClick += Click_ItemAttach;
            _recyAttachment.SetLayoutManager(staggeredGridLayoutManager);
            _recyAttachment.SetAdapter(_adapterAttachFile);
        }
        /// <summary>
        /// Event khi click vào Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_ItemAttach(object sender, int e)
        {
            MinionActionCore.OnElementClickAttachmentDetailWorkflow(null, new MinionActionCore.ElementAttachFileClick(_element, _lstAttachment[e]));
        }
    }
    public class AdapterControlAttachmentHorizontal : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public Activity _mainAct;
        public List<KeyValuePair<string, string>> _lstAttachment;
        public int _widthScreenTablet = -1;

        public AdapterControlAttachmentHorizontal(Activity _mainAct, List<KeyValuePair<string, string>> _lstAttachment, int _widthScreenTablet)
        {
            this._mainAct = _mainAct;
            this._lstAttachment = _lstAttachment;
            this._widthScreenTablet = _widthScreenTablet;
        }
        private void OnItemClick(int obj)
        {
            ItemClick(this, obj);
        }
        public override int ItemCount => _lstAttachment.Count;
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemControlAttachmentHorizontal, parent, false);
            int _widthRow;
            if (_widthScreenTablet == -1) // ForPhone
            {
                DisplayMetrics dm = _mainAct.Resources.DisplayMetrics;
                _widthRow = dm.WidthPixels / 3;
            }
            else // For Tablet
            {
                _widthRow = _widthScreenTablet / 3;
            }
            itemView.LayoutParameters = new ViewGroup.LayoutParams(_widthRow, ViewGroup.LayoutParams.WrapContent);

            ControlAttachmentHorizontal holder = new ControlAttachmentHorizontal(itemView, OnItemClick);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ControlAttachmentHorizontal vh = holder as ControlAttachmentHorizontal;
            KeyValuePair<string, string> _currentAttachment = _lstAttachment[position];
            if (!String.IsNullOrEmpty(_currentAttachment.Value))
            {
                vh._tvFileName.Text = _currentAttachment.Value;
            }
            else
            {
                vh._tvFileName.Text = "";
            }
            if (_currentAttachment.Key == "1" || _currentAttachment.Key == "3") // để hình pdf
            {
                //_img.Image = UIImage.FromFile("Icons/icon_attachment_pdf.png");
            }
            else // hình khác
            {

            }
        }
    }
    public class ControlAttachmentHorizontal : RecyclerView.ViewHolder
    {
        public ImageView _imgAvatar { get; set; }
        public TextView _tvFileName { get; set; }
        public ControlAttachmentHorizontal(View itemview, Action<int> listener) : base(itemview)
        {
            _imgAvatar = itemview.FindViewById<ImageView>(Resource.Id.img_ItemControlAttachmentHorizontal_Avatar);
            _tvFileName = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlAttachmentHorizontal_FileName);
            itemview.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}