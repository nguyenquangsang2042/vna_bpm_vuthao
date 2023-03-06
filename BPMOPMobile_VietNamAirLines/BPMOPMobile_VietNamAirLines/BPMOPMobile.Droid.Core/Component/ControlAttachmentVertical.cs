using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ControlAttachmentVertical : ControlBase
    {
        private LinearLayout _parentView { get; set; }
        private ViewElement _element { get; set; }
        private RecyclerView _recyAttachment;
        List<KeyValuePair<string, string>> _lstAttachment = new List<KeyValuePair<string, string>>();

        public ControlAttachmentVertical(Activity _mainAct, LinearLayout _parentView, ViewElement _element) : base(_mainAct)
        {
            this._mainAct = _mainAct;
            this._parentView = _parentView;
            this._element = _element;
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
            _recyAttachment.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 3 * (int)CmmDroidFunction.ConvertDpToPixel(40, frame.Context)); // default 3 item

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

            StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
            AdapterControlAttachmentVertical _adapterAttachFile = new AdapterControlAttachmentVertical(_mainAct, _lstAttachment);
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
    public class AdapterControlAttachmentVertical : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public Activity _mainAct;
        public List<KeyValuePair<string, string>> _lstAttachment;

        public AdapterControlAttachmentVertical(Activity _mainAct, List<KeyValuePair<string, string>> _lstAttachment)
        {
            this._mainAct = _mainAct;
            this._lstAttachment = _lstAttachment;
        }
        private void OnItemClick(int obj)
        {
            ItemClick(this, obj);

        }
        public override int ItemCount => _lstAttachment.Count;
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemControlAttachmentVertical, parent, false);
            ControlAttachmentVerticalHolder holder = new ControlAttachmentVerticalHolder(itemView, OnItemClick);
            int a = itemView.Height;
            int b = itemView.MeasuredHeight;
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ControlAttachmentVerticalHolder vh = holder as ControlAttachmentVerticalHolder;
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
    public class ControlAttachmentVerticalHolder : RecyclerView.ViewHolder
    {
        public ImageView _imgAvatar { get; set; }
        public TextView _tvFileName { get; set; }
        public ControlAttachmentVerticalHolder(View itemview, Action<int> listener) : base(itemview)
        {
            _imgAvatar = itemview.FindViewById<ImageView>(Resource.Id.img_ItemControlAttachmentVertical_Avatar);
            _tvFileName = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlAttachmentVertical_FileName);
            itemview.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}