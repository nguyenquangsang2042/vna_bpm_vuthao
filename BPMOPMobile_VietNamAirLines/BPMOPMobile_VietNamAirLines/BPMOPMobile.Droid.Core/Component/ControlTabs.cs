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
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ControlTabs : ControlBase
    {
        private LinearLayout _parentView { get; set; }
        private ViewElement _element { get; set; }

        private RecyclerView _recyTabs;
        private List<KeyValuePair<string, string>> _lstTabs = new List<KeyValuePair<string, string>>();
        private int _selectedIndex = 0;
        private AdapterControlTabs _adapterTabs;
        public int _widthScreenTablet = -1;

        public ControlTabs(Activity _mainAct, LinearLayout _parentView, ViewElement _element, int _widthScreenTablet) : base(_mainAct)
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

            _recyTabs = new RecyclerView(_mainAct);
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
            this._tvValue.Visibility = ViewStates.Gone;
            this._lnLine.Visibility = ViewStates.Gone;
            _lnContent.RemoveView(_lnLine); // Remove line ra để add lại dưới Tab

            int _padding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 12, _mainAct.Resources.DisplayMetrics);
            _recyTabs.SetPadding(_padding, _padding, _padding, _padding);
            frame.AddView(_recyTabs);
            frame.AddView(_lnLine);
        }
        public override void SetProprety()
        {
            //if (_element.ListProprety != null)
            //{
            //    foreach (var item in _element.ListProprety)
            //    {
            //        CmmFunctionCore.SetPropertyValueByNameCustom(_tvValue, item.Key, item.Value);
            //    }
            //}
        }
        public override void SetEnable()
        {
            base.SetEnable();
            _recyTabs.Enabled = _element.Enable;
        }
        public override void SetTitle()
        {
            base.SetTitle();

            _tvTitle.Text = _element.Title;
        }
        public override void SetValue()
        {
            base.SetValue();

            //Value = _element.Value;.
            var selectedValue = _element.Value.Split(new string[] { ";#" }, StringSplitOptions.None)[1].ToLower();
            var arrayValue = _element.DataSource.Split(new string[] { ";#" }, StringSplitOptions.None);
            if (arrayValue.Length > 2)
            {
                string[] arraySetValue = new string[arrayValue.Length / 2];// bo mấy dấu ";#" đi nên /2
                int index = 0;
                for (var i = 0; i < arrayValue.Length; i += 2)
                {
                    _lstTabs.Add(new KeyValuePair<string, string>(arrayValue[i], arrayValue[i + 1]));
                    arraySetValue[index] = arrayValue[i + 1];
                    if (selectedValue == arrayValue[i + 1].ToLower())
                        _selectedIndex = index;
                    index++;
                }
            }
            StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Horizontal);
            _adapterTabs = new AdapterControlTabs(_mainAct, _lstTabs, _selectedIndex, _widthScreenTablet);
            _adapterTabs.ItemClick += Click_ItemTab;
            _recyTabs.SetLayoutManager(staggeredGridLayoutManager);
            _recyTabs.SetAdapter(_adapterTabs);
        }
        /// <summary>
        /// Event khi click vào Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_ItemTab(object sender, int e)
        {
            try
            {
                _selectedIndex = e;
                StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Horizontal);
                _adapterTabs = new AdapterControlTabs(_mainAct, _lstTabs, _selectedIndex, _widthScreenTablet);
                _adapterTabs.ItemClick += Click_ItemTab;
                _recyTabs.SetLayoutManager(staggeredGridLayoutManager);
                _recyTabs.SetAdapter(_adapterTabs);

                //MinionActionCore.OnElementClickAttachmentDetailWorkflow(null, new MinionActionCore.ElementAttachFileClick(_element, _lstAttachment[e]));
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - Click_ItemTab - Error: " + ex.Message);
#endif
            }
        }
    }
    public class AdapterControlTabs : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public Activity _mainAct;
        public List<KeyValuePair<string, string>> _lstTabs;
        public int _selectedPostion = -1;
        public int _widthScreenTablet = -1;
        public AdapterControlTabs(Activity _mainAct, List<KeyValuePair<string, string>> _lstTabs, int _selectedPostion, int _widthScreenTablet)
        {
            this._mainAct = _mainAct;
            this._lstTabs = _lstTabs;
            this._selectedPostion = _selectedPostion;
            this._widthScreenTablet = _widthScreenTablet;
        }
        private void OnItemClick(int obj)
        {
            ItemClick(this, obj);
        }
        public override int ItemCount => _lstTabs.Count;
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemColtrolTabs, parent, false);

            if (_lstTabs.Count > 0)
            {
                int _widthRow;
                if (_widthScreenTablet == -1) // ForPhone
                {
                    DisplayMetrics dm = _mainAct.Resources.DisplayMetrics;
                    _widthRow = (int)(dm.WidthPixels / _lstTabs.Count);
                }
                else // For Tablet
                {
                    _widthRow = (int)(_widthScreenTablet / _lstTabs.Count);
                }
                itemView.LayoutParameters = new ViewGroup.LayoutParams(_widthRow - (int)CmmDroidFunction.ConvertDpToPixel(10, parent.Context), ViewGroup.LayoutParams.WrapContent);
            }

            ControlTabsHolder holder = new ControlTabsHolder(itemView, OnItemClick);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ControlTabsHolder vh = holder as ControlTabsHolder;

            if (position == _selectedPostion)
            {
                vh._tvTitle.SetBackgroundResource(Resource.Drawable.ItemControlTabs_Selected);
                vh._tvTitle.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));
            }
            else
            {
                vh._tvTitle.SetBackgroundResource(Resource.Drawable.ItemControlTabs_UnSelected);
                vh._tvTitle.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlueEnable)));
            }
            if (!String.IsNullOrEmpty(_lstTabs[position].Value))
            {
                vh._tvTitle.Text = _lstTabs[position].Value;
            }
            else
            {
                vh._tvTitle.Text = "";
            }
        }
    }
    public class ControlTabsHolder : RecyclerView.ViewHolder
    {
        public TextView _tvTitle { get; set; }
        public ControlTabsHolder(View itemview, Action<int> listener) : base(itemview)
        {
            _tvTitle = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlTabs_Title);
            itemview.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}