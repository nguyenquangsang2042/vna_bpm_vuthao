using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using Newtonsoft.Json.Linq;
using SQLite;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    public class AdapterFragmentListDynamic : RecyclerView.Adapter
    {
        public AppCompatActivity _mainAct;
        public Context _context;
        public event EventHandler<JObject> CustomItemClick;
        public List<BeanWFDetailsHeader> _lstHeader = new List<BeanWFDetailsHeader>();
        public List<JObject> _lstJObjectRow = new List<JObject>();

        private BeanWFDetailsHeader _headerTitle = null;
        private BeanWFDetailsHeader _headerIsFollow = null;
        private BeanWFDetailsHeader _headerFileCount = null;

        public bool _isShowSumColumn = false;
        private bool _allowLoadMore = false;

        public AdapterFragmentListDynamic(AppCompatActivity _mainAct, Context _context, List<BeanWFDetailsHeader> _lstHeader, List<JObject> _lstJObjectRow)
        {
            this._mainAct = _mainAct;
            this._context = _context;
            this._lstJObjectRow = _lstJObjectRow;

            //this._lstHeader = _lstHeader;
            List<BeanWFDetailsHeader> _lstHeaderTemp = _lstHeader.ToList();
            foreach (var item in _lstHeaderTemp)
            {
                switch (item.internalName.ToLowerInvariant())
                {
                    case "title":
                    case "tieude":
                        _headerTitle = item; break;
                    case "isfollow":
                        _headerIsFollow = item; break;
                    case "filecount":
                        _headerFileCount = item; break;
                    default:
                        this._lstHeader.Add(item); // list này ko bao gồm các header cứng
                        break;
                }
            }

        }

        public void SetAllowLoadMore(bool _allowLoadMore)
        {
            this._allowLoadMore = _allowLoadMore;
        }

        public void LoadMore(List<JObject> _lstMore)
        {
            try
            {

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "LoadMore", ex);
#endif
            }
        }

        private void OnItemClick(int position)
        {
            if (CustomItemClick != null)
            {
                CustomItemClick(this, _lstJObjectRow[position]);
            }
        }

        public override int ItemCount => _lstJObjectRow.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemDynamicList, parent, false);
            AdapterDynamicListGroupHolder holder = new AdapterDynamicListGroupHolder(itemView, OnItemClick);
            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                AdapterDynamicListGroupHolder _holder = holder as AdapterDynamicListGroupHolder;
                _holder._lnContent.RemoveAllViews();

                if (position % 2 == 0)
                    _holder._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueTint)));
                else
                    _holder._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));

                #region Binding Stable Value
                View _rowViewHeader = LayoutInflater.From(_holder.ItemView.Context).Inflate(Resource.Layout.ItemDynamicListItem_Header, null);
                TextView _tvValueHeader = _rowViewHeader.FindViewById<TextView>(Resource.Id.tv_ItemDynamicListItem_Header_Value);
                ImageView _imgHeaderFavorite = _rowViewHeader.FindViewById<ImageView>(Resource.Id.img_ItemDynamicListItem_Header_Favorite);
                ImageView _imgHeaderFlag = _rowViewHeader.FindViewById<ImageView>(Resource.Id.img_ItemDynamicListItem_Header_Flag);
                ImageView _imgHeaderAttach = _rowViewHeader.FindViewById<ImageView>(Resource.Id.img_ItemDynamicListItem_Header_AttachFile);

                _imgHeaderFlag.Visibility = ViewStates.Gone;

                if (_headerTitle != null)
                    _tvValueHeader.Text = CmmFunction.GetRawValueByHeader(_headerTitle, _lstJObjectRow[position]);
                else
                    _tvValueHeader.Text = "";

                _imgHeaderFavorite.SetImageResource(Resource.Drawable.icon_ver2_star_unchecked);
                if (_headerIsFollow != null)
                {
                    string value = CmmFunction.GetRawValueByHeader(_headerIsFollow, _lstJObjectRow[position]);
                    if (value.ToString().Equals("1"))
                        _imgHeaderFavorite.SetImageResource(Resource.Drawable.icon_ver2_star_checked);
                }

                _imgHeaderAttach.Visibility = ViewStates.Invisible;
                if (_headerFileCount != null)
                {
                    string value = CmmFunction.GetRawValueByHeader(_headerFileCount, _lstJObjectRow[position]);
                    int _res = -1;
                    if (!String.IsNullOrEmpty(value))
                    {
                        int.TryParse(value, out _res);
                        if (_res > 0)
                            _imgHeaderAttach.Visibility = ViewStates.Visible;
                    }
                }
                _holder._lnContent.AddView(_rowViewHeader);
                #endregion

                #region Binding Dynamic Value
                for (int i = 0; i < _lstHeader.Count; i++)
                {
                    View _rowView = LayoutInflater.From(_holder.ItemView.Context).Inflate(Resource.Layout.ItemDynamicListItem, null);
                    TextView _tvTitle = _rowView.FindViewById<TextView>(Resource.Id.tv_ItemDynamicListItem_Title);
                    TextView _tvValue = _rowView.FindViewById<TextView>(Resource.Id.tv_ItemDynamicListItem_Value);

                    _tvTitle.Text = (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? _lstHeader[i].Title : _lstHeader[i].TitleEN) + ":";
                    _tvTitle.SetMaxWidth((int)(_mainAct.Resources.DisplayMetrics.WidthPixels * 0.3));

                    _tvValue.Text = CmmFunction.GetFormattedValueByHeader(_lstHeader[i], _lstJObjectRow[position]);
                    _holder._lnContent.AddView(_rowView);
                }
                #endregion

                #region Load more
                if (position == _lstJObjectRow.Count - 1 && _allowLoadMore == true)
                    _holder._lnLoadMore.Visibility = ViewStates.Visible;
                else
                    _holder._lnLoadMore.Visibility = ViewStates.Gone;
                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnBindViewHolder", ex);
#endif
            }
        }

        public class AdapterDynamicListGroupHolder : RecyclerView.ViewHolder
        {
            public LinearLayout _lnAll { get; set; }
            public LinearLayout _lnContent { get; set; }
            public LinearLayout _lnLoadMore { get; set; }
            public AdapterDynamicListGroupHolder(View itemview, Action<int> listener) : base(itemview)
            {
                _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemDynamicList_All);
                _lnContent = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemDynamicList_Content);
                _lnLoadMore = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemDynamicList_LoadMore);
                _lnAll.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }

    }
}