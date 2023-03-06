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
using Android.Support.V4.Content.Res;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static BPMOPMobile.Droid.Core.Component.ControlInputGridDetails.AdapterListGridData;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ControlInputGridDetails : ControlBase
    {
        private Context _context { get; set; }
        private LinearLayout _parentView { get; set; }
        private LinearLayout _lnTitleImport { get; set; } // chứa _tvTitle và _lnImport
        private LinearLayout _lnImport { get; set; }
        private CardView _cardViewDetail { get; set; }
        private ImageView _imgImport { get; set; }
        private TextView _tvImport { get; set; }
        private RecyclerView _recyListGridData { get; set; } // Recy to nhất
        private CustomEnabledHorizontalScrollView _scrollListGridData { get; set; }

        private ViewElement _element { get; set; }
        private List<BeanWFDetailsHeader> _lstHeader = new List<BeanWFDetailsHeader>();
        private List<JObject> ListJObjectRow = new List<JObject>();
        private AdapterListGridData _adapterListGridData;

        public int _flagView;

        public ControlInputGridDetails(Activity _mainAct, LinearLayout _parentView, ViewElement _element, int _flagView) : base(_mainAct)
        {
            this._parentView = _parentView;
            this._element = _element;
            this._flagView = _flagView;
            InitializeComponent();
        }
        public override void InitializeComponent()
        {
            base.InitializeComponent();

            _recyListGridData = new RecyclerView(_mainAct);
            _scrollListGridData = new CustomEnabledHorizontalScrollView(_mainAct);

            _cardViewDetail = new CardView(_mainAct);
            _lnTitleImport = new LinearLayout(_mainAct);
            _lnImport = new LinearLayout(_mainAct);
            _tvImport = new TextView(_mainAct);
            _imgImport = new ImageView(_mainAct);

            _lnTitleImport.Orientation = Orientation.Horizontal;
            _lnTitleImport.SetGravity(GravityFlags.Center);

            _lnImport.Orientation = Orientation.Horizontal;
            _lnImport.SetGravity(GravityFlags.Right);

            _tvTitle.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            _tvTitle.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
            _tvTitle.SetMaxLines(1);
            _tvTitle.Ellipsize = TextUtils.TruncateAt.End;

            _tvImport.SetTextSize(ComplexUnitType.Sp, 12);
            _tvImport.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
            _tvImport.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
            _tvImport.Ellipsize = TextUtils.TruncateAt.End;
            _tvImport.Gravity = GravityFlags.Center;

            _imgImport.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomEnable)));

            _cardViewDetail.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGray)));
            _cardViewDetail.UseCompatPadding = true;
            _cardViewDetail.Radius = 5f;

            _scrollListGridData.setEnableScrolling(true); // disable scrolling

            _tvImport.Text = CmmFunction.GetTitle("TEXT_ADDNEW_ATTACHMENT", "Tạo mới");

        }
        public override void InitializeFrameView(LinearLayout frame)
        {
            if (_element.Hidden == true) // Check xem có ẩn view hay không
                return;

            _context = frame.Context;
            base.InitializeFrameView(frame);
            _tvValue.Visibility = ViewStates.Gone;
            _lnContent.RemoveView(_tvTitle); // Remove ra lát Add lại

            int _padding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 6, _mainAct.Resources.DisplayMetrics);
            LinearLayout.LayoutParams _paramsRecyListGridData = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams _paramsCardView = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams _paramslnTitleImport = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent, 1);
            LinearLayout.LayoutParams _paramsTitle = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WrapContent, 1);
            LinearLayout.LayoutParams _paramslnImport = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams _paramsimgImport = new LinearLayout.LayoutParams((int)CmmDroidFunction.ConvertDpToPixel(20, frame.Context), (int)CmmDroidFunction.ConvertDpToPixel(20, frame.Context));
            _paramsCardView.SetMargins(_padding, 0, _padding, _padding);
            _paramsimgImport.SetMargins(_padding, 0, 2 * _padding, 0);
            _paramslnTitleImport.SetMargins(0, 0, 0, _padding);

            //_recyListGridData.Background = ContextCompat.GetDrawable(frame.Context, Resource.Drawable.textcornerstrokegraywhitesolid2);
            _imgImport.Background = ContextCompat.GetDrawable(frame.Context, Resource.Drawable.icon_ver2_addfile);

            _lnTitleImport.LayoutParameters = _paramslnTitleImport;
            _tvTitle.LayoutParameters = _paramsTitle;
            _lnImport.LayoutParameters = _paramslnImport;
            _imgImport.LayoutParameters = _paramsimgImport;
            _cardViewDetail.LayoutParameters = _paramsCardView;
            _scrollListGridData.LayoutParameters = _paramsRecyListGridData;

            _lnTitleImport.SetPadding(_padding, 0, _padding, 0);
            _imgImport.SetPadding(_padding, 0, _padding, 0);
            _lnImport.SetPadding(_padding, 0, 2 * _padding, 0);

            _lnImport.AddView(_imgImport);
            _lnImport.AddView(_tvImport);
            _lnTitleImport.AddView(_tvTitle);
            _lnTitleImport.AddView(_lnImport);

            _scrollListGridData.AddView(_recyListGridData);

            if (_element.Enable) // enable mới cho click
            {
                _lnImport.Visibility = ViewStates.Visible;
                _lnImport.Click += Click_lnImport;
            }
            else
            {
                _lnImport.Visibility = ViewStates.Invisible;
            }
            _cardViewDetail.AddView(_scrollListGridData);
            frame.AddView(_lnTitleImport);
            frame.AddView(_cardViewDetail);
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
            try
            {
                base.SetValue();
                var data = _element.Value.Trim();

                #region List Component Header
                try
                {
                    if (!String.IsNullOrEmpty(_element.DataSource))
                    {
                        _lstHeader = JsonConvert.DeserializeObject<List<BeanWFDetailsHeader>>(_element.DataSource);
                        _lstHeader = _lstHeader.Where(x => !String.IsNullOrEmpty(x.internalName)).ToList(); // bỏ trường hợp # ra
                    }
                }
                catch (Exception)
                {
                    _lstHeader = new List<BeanWFDetailsHeader>();
                }
                #endregion

                #region List Component Value
                try
                {
                    if (!String.IsNullOrEmpty(_element.Value))
                    {
                        ListJObjectRow = JsonConvert.DeserializeObject<List<JObject>>(_element.Value);
                    }
                }
                catch (Exception)
                {
                    ListJObjectRow = new List<JObject>();
                }

                #endregion        
                _adapterListGridData = new AdapterListGridData(_mainAct, _context, _lstHeader, ListJObjectRow);
                _adapterListGridData.CustomItemClick += Click_ItemRecyGrid;
                _recyListGridData.SetAdapter(_adapterListGridData);
                _recyListGridData.SetLayoutManager(new LinearLayoutManager(_context, LinearLayoutManager.Vertical, false));
                _recyListGridData.NestedScrollingEnabled = true;
                _recyListGridData.HorizontalScrollBarEnabled = true;
                //_recyListGridData.Post(() =>
                //{
                //    CmmDroidFunction.SetRecyclerViewHeight(_recyListGridData);
                //});
                // MySwipeHelper mySwipeHelper = new AdapterListGridData_SwiperHelper(_context, _recyListGridData, 150);
            }
            catch (Exception ex)
            {
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetValue", ex);
            }
        }

        private void Click_ItemRecyGrid(object sender, JObject e)
        {
            try
            {
                if (!String.IsNullOrEmpty(_element.Value))
                {
                    ListJObjectRow = JsonConvert.DeserializeObject<List<JObject>>(_element.Value);
                    int index = ListJObjectRow.FindIndex(x => x.ToString() == e.ToString());

                    if (_parentView != null)
                    {
                        MinionActionCore.OnElementFormClickEvent_WithInnerAction(null, new MinionActionCore.ElementFormClick_WithInnerAction(_element, (int)EnumFormControlInnerAction.ControlInputGridDetails_InnerActionID.Edit, index, _flagView));
                    }
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemRecyGrid", ex);
#endif
            }
        }

        private void Click_lnImport(object sender, EventArgs e)
        {
            try
            {
                if (_parentView != null)
                {
                    MinionActionCore.OnElementFormClickEvent_WithInnerAction(null, new MinionActionCore.ElementFormClick_WithInnerAction(_element, (int)EnumFormControlInnerAction.ControlInputGridDetails_InnerActionID.Create, -1, _flagView));
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnImport", ex);
#endif
            }
        }

        public class AdapterListGridData : RecyclerView.Adapter
        {
            public Activity _mainAct;
            public Context _context;
            public event EventHandler<JObject> CustomItemClick;
            public List<BeanWFDetailsHeader> _lstHeader = new List<BeanWFDetailsHeader>();
            public List<KeyValuePair<string, float>> _lstColumnTextLength = new List<KeyValuePair<string, float>>();
            public List<JObject> _lstJObjectRow = new List<JObject>();
            public View _itemView_Header, _itemView_Value;
            public bool _isShowSumColumn = false;
            public int _allRowCount = -1;
            public int _dpPerCharacter = 5; // 1 character = 10dp

            public AdapterListGridData(Activity _mainAct, Context _context, List<BeanWFDetailsHeader> _lstHeader, List<JObject> _lstJObjectRow)
            {
                this._mainAct = _mainAct;
                this._context = _context;
                this._lstHeader = _lstHeader;
                this._lstJObjectRow = _lstJObjectRow;

                foreach (BeanWFDetailsHeader itemHeader in _lstHeader) // check xem có show ko
                {
                    if (itemHeader.isSum == true)
                    {
                        _isShowSumColumn = true;
                        break;
                    }
                }
                _lstColumnTextLength = GetListColumnTextLength(_lstHeader, _lstJObjectRow); // List Length
            }

            private void OnItemClick(int position)
            {
                if (_isShowSumColumn == true) // show thêm cột Sum -> bỏ row cuối cùng 
                {
                    if (position < _allRowCount - 1) // click vào dòng khác Sum -> event
                        if (CustomItemClick != null)
                            CustomItemClick(this, _lstJObjectRow[position - 1]);
                }
                else
                {
                    if (CustomItemClick != null)
                        CustomItemClick(this, _lstJObjectRow[position - 1]);
                }
            }

            private List<KeyValuePair<string, float>> GetListSumColumnValue(List<BeanWFDetailsHeader> _lstHeader, List<JObject> _lstJObjectRow)
            {
                List<KeyValuePair<string, float>> _lstResult = new List<KeyValuePair<string, float>>();
                try
                {
                    foreach (BeanWFDetailsHeader _itemHeader in _lstHeader)
                    {
                        if (_itemHeader.isSum == true)
                        {
                            foreach (var obj in _lstJObjectRow)
                            {
                                if (obj.ContainsKey(_itemHeader.internalName))
                                {
                                    float _sumValue = _lstJObjectRow.Sum(x => ((int)x[_itemHeader.internalName]));
                                    _lstResult.Add(new KeyValuePair<string, float>(_itemHeader.internalName, _sumValue));
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SumColumnValue", ex);
#endif
                }
                return _lstResult;
            }

            private List<KeyValuePair<string, float>> GetListColumnTextLength(List<BeanWFDetailsHeader> _lstHeader, List<JObject> _lstJObjectRow)
            {
                List<KeyValuePair<string, float>> _lstResult = new List<KeyValuePair<string, float>>();
                try
                {
                    foreach (BeanWFDetailsHeader _itemHeader in _lstHeader)
                    {
                        float _maxColumnLength = _itemHeader.Title.Length; // Lấy title length làm chuẩn

                        if (_lstJObjectRow != null && _lstJObjectRow.Count > 0)
                        {
                            float _maxObjectLength;
                            try
                            {
                                _maxObjectLength = _lstJObjectRow
                                                    .Where(x => x.ContainsKey(_itemHeader.internalName))// loại mấy thằng null ra
                                                    .Max(x => (x[_itemHeader.internalName].ToString().Length));
                            }
                            catch (Exception)
                            {
                                _maxObjectLength = 0;
                            }

                            if (_maxObjectLength > _maxColumnLength)
                            {
                                _maxColumnLength = _maxObjectLength;
                                if (_maxColumnLength > 30) // limit lại nếu quá dài
                                    _maxColumnLength = 30;
                            }
                        }
                        if (_itemHeader.internalName != null) // bỏ # ra
                            _lstResult.Add(new KeyValuePair<string, float>(_itemHeader.internalName, _maxColumnLength));
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SumColumnValue", ex);
#endif
                }
                return _lstResult;
            }

            /// <summary>
            /// Hàm để binding dữ liệu lên 1 record của Value Cell
            /// </summary>
            private void BindingValueData(BeanWFDetailsHeader _itemHeader, JObject _currentJObjectRow, TextView _tvContent)
            {
                try
                {
                    try
                    {

                        switch (_itemHeader.dataType.ToLowerInvariant()) //_elementChild.InternalName
                        {
                            case "selectuser":
                            case "selectusergroup":
                                {
                                    string _data = (_currentJObjectRow[_itemHeader.internalName]).ToString();
                                    List<BeanUserAndGroup> _beanUserAndGroup = JsonConvert.DeserializeObject<List<BeanUserAndGroup>>(_data);
                                    if (_beanUserAndGroup != null && _beanUserAndGroup.Count > 0)
                                    {
                                        _tvContent.Text = _beanUserAndGroup[0].Name;
                                    }
                                    else
                                        _tvContent.Text = "";
                                    break;
                                }
                            case "selectusermulti":
                            case "selectusergroupmulti":
                                {
                                    string _data = (_currentJObjectRow[_itemHeader.internalName]).ToString();
                                    List<string> _lstName = new List<string>();
                                    List<BeanUserAndGroup> _beanUserAndGroups = JsonConvert.DeserializeObject<List<BeanUserAndGroup>>(_data);
                                    if (_beanUserAndGroups != null && _beanUserAndGroups.Count > 0)
                                    {
                                        for (int i = 0; i < _beanUserAndGroups.Count; i++)
                                            _lstName.Add(_beanUserAndGroups[i].Name.Trim());

                                        _tvContent.Text = String.Join("; ", _lstName.ToArray());
                                    }
                                    else
                                        _tvContent.Text = "";
                                    break;
                                }
                            case "date":
                                {
                                    DateTime dateValue = DateTime.Parse(_currentJObjectRow[_itemHeader.internalName].ToString());
                                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                                        _tvContent.Text = dateValue.ToString(@"dd/MM/yy").ToLower();
                                    else
                                        _tvContent.Text = dateValue.ToString(@"MM/dd/yy").ToLower();
                                    break;
                                }
                            case "time":
                            case "datetime":
                                {
                                    DateTime dateValue = DateTime.Parse(_currentJObjectRow[_itemHeader.internalName].ToString());
                                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                                        _tvContent.Text = dateValue.ToString("dd/MM/yy HH:mm").ToLower();
                                    else
                                        _tvContent.Text = dateValue.ToString("MM/dd/yy HH:mm").ToLower();
                                    break;
                                }


                            case "singlechoice":
                            case "singlelookup":
                                {
                                    string _data = (_currentJObjectRow[_itemHeader.internalName]).ToString();
                                    List<BeanLookupData> _lstValue = JsonConvert.DeserializeObject<List<BeanLookupData>>(_data);
                                    if (_lstValue != null && _lstValue.Count > 0)
                                    {
                                        _tvContent.Text = _lstValue[0].Title.ToString();
                                    }
                                    else
                                        _tvContent.Text = "";
                                }
                                break;
                            case "multiplechoice":
                            case "multiplelookup":
                                {
                                    string _data = (_currentJObjectRow[_itemHeader.internalName]).ToString();
                                    string _result = "";

                                    List<BeanLookupData> _lstObject = JsonConvert.DeserializeObject<List<BeanLookupData>>(_data);
                                    List<string> _lstValue = new List<string>();

                                    if (_lstObject != null && _lstObject.Count > 0)
                                    {
                                        foreach (BeanLookupData item in _lstObject)
                                        {
                                            _lstValue.Add(item.Title);
                                        }
                                        _result = String.Join(", ", _lstValue.ToArray());
                                    }
                                    _tvContent.Text = _result;
                                    break;
                                }
                            case "number":
                                {
                                    //CultureInfo _culVN = CultureInfo.GetCultureInfo("vi-VN");
                                    //_tvContent.Text = ((float)_currentJObjectRow[_itemHeader.internalName.ToString()]).ToString("N0", _culVN);

                                    ViewElement _element = new ViewElement()
                                    {
                                        DataSource = _itemHeader.DataSource,
                                        Value = ((float)_currentJObjectRow[_itemHeader.internalName.ToString()]).ToString()
                                    };
                                    _tvContent.Text = CmmFunction.GetFormatControlDecimal(_element);
                                    break;
                                }
                            case "yesno":
                            case "textinputmultiline":
                            case "textinputformat": // Text Editor
                            case "text":
                            default:
                                {
                                    _tvContent.Text = _currentJObjectRow[_itemHeader.internalName.ToString()].ToString().Trim();
                                    break;
                                }
                        }
                    }
                    catch (Exception)
                    {
                        _tvContent.Text = "";
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnBindViewHolder", ex);
#endif
                }

            }

            /// <summary>
            /// Hàm để binding dữ liệu lên 1 record của Sum Cell
            /// </summary>
            private void BindingSumData(BeanWFDetailsHeader _itemHeader, List<JObject> _lstJObjectRow, TextView _tvContent)
            {
                _tvContent.Text = "";
                try
                {
                    List<KeyValuePair<string, float>> _lstSum = GetListSumColumnValue(_lstHeader, _lstJObjectRow);

                    if (!String.IsNullOrEmpty(_itemHeader.internalName))
                    {
                        try
                        {
                            float _value = _lstSum.Where(x => x.Key.Equals(_itemHeader.internalName)).FirstOrDefault().Value;
                            if (_value > 0)
                            {
                                CultureInfo _culVN = CultureInfo.GetCultureInfo("vi-VN");
                                _tvContent.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
                                _tvContent.Text = _value.ToString("N0", _culVN);
                            }
                        }
                        catch (Exception) { }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "BindingSumData", ex);
#endif
                }
            }

            public override int ItemCount
            {
                get
                {
                    if (_isShowSumColumn == true) // show thêm cột Sum
                        _allRowCount = (_lstJObjectRow.Count + 1) + 1; // rowCount + Header + Sum                       
                    else
                        _allRowCount = _lstJObjectRow.Count + 1; // rowCount + Header

                    return _allRowCount;
                }

            }

            public override int GetItemViewType(int position)
            {
                if (position == 0)
                {
                    return 0;
                }
                return 1;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                if (viewType == 0) // Header
                {
                    View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemControlInputGridDetails_Recy, parent, false);
                    AdapterListGridData_ViewHolder_Header holder = new AdapterListGridData_ViewHolder_Header(itemView, null);
                    return holder;
                }
                else
                {
                    View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemControlInputGridDetails_Recy, parent, false);
                    AdapterListGridData_ViewHolder_Value holder = new AdapterListGridData_ViewHolder_Value(itemView, OnItemClick);
                    return holder;
                }
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                try
                {
                    if (holder is AdapterListGridData_ViewHolder_Header) // Header
                    {
                        #region Render Header
                        AdapterListGridData_ViewHolder_Header _holder = holder as AdapterListGridData_ViewHolder_Header;
                        _holder._lnContent.RemoveAllViews();
                        _holder._lnContent.Foreground = null;

                        foreach (BeanWFDetailsHeader _itemHeader in _lstHeader)
                        {
                            float _textlength = _lstColumnTextLength.Where(x => x.Key == _itemHeader.internalName).FirstOrDefault().Value;
                            if (_textlength == 0) _textlength = 3; // trường hợp #
                            else if(_textlength<10)
                            {
                                _textlength = 30;
                            }    
                            LinearLayout.LayoutParams _paramsItemRow = new LinearLayout.LayoutParams((int)CmmDroidFunction.ConvertDpToPixel((_textlength * _dpPerCharacter) + 25, _context),
                                (int)CmmDroidFunction.ConvertDpToPixel(45, _context)); // + thêm 2 cái margin Left Right là 20dp

                            #region Get View
                            //if (_itemView_Header != null)
                            //
                            _itemView_Header = LayoutInflater.From(_context).Inflate(Resource.Layout.ItemControlInputGridDetails_Recy_Header, _holder._lnContent, false);
                            //}
                            AdapterListGridData_ViewHolder_Header_Item _holderItem = new AdapterListGridData_ViewHolder_Header_Item(_itemView_Header, null);

                            _holderItem._lnContent.LayoutParameters = _paramsItemRow;

                            //GradientDrawable _drawable = new GradientDrawable();
                            //_drawable.SetCornerRadii(new float[] { 10, 10, 10, 10, 0, 0, 0, 0 });//{mTopLeftRadius, mTopLeftRadius, mTopRightRadius, mTopRightRadius, mBottomRightRadius, mBottomRightRadius, mBottomLeftRadius, mBottomLeftRadius}
                            //_drawable.SetShape(ShapeType.Rectangle);
                            //_drawable.SetColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clGray)));
                            //_holder._lnContent.Background = _drawable;
                            _holder._lnContent.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clGray)));
                            #endregion

                            #region Bind Header Data
                            if (!String.IsNullOrEmpty(_itemHeader.Title))
                                _holderItem._tvContent.Text = _itemHeader.Title;
                            else
                                _holderItem._tvContent.Text = "";
                            #endregion

                            _holder._lnContent.AddView(_holderItem.ItemView);
                        }
                        #endregion
                    }
                    else if (holder is AdapterListGridData_ViewHolder_Value) // Value - Sum
                    {
                        #region Render Value

                        AdapterListGridData_ViewHolder_Value _holder = holder as AdapterListGridData_ViewHolder_Value;

                        #region View Rule

                        _holder._lnContent.RemoveAllViews();

                        // Tô màu so le 
                        if (position % 2 == 0)
                            //_holder._lnContent.Background = ContextCompat.GetDrawable(_context, Resource.Drawable.textcontrolgrid_grayitem);
                            _holder._lnContent.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueNavigation)));
                        else
                            _holder._lnContent.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));

                        #endregion

                        foreach (BeanWFDetailsHeader _itemHeader in _lstHeader)
                        {
                            #region Get View
                            //if (_itemView_Value != null)
                            //
                            _itemView_Value = LayoutInflater.From(_context).Inflate(Resource.Layout.ItemControlInputGridDetails_Recy_Value, _holder._lnContent, false);
                            //}
                            AdapterListGridData_ViewHolder_Value_Item _holderItem = new AdapterListGridData_ViewHolder_Value_Item(_itemView_Value, null);

                            float _textlength = _lstColumnTextLength.Where(x => x.Key == _itemHeader.internalName).FirstOrDefault().Value;
                            if (_textlength == 0) _textlength = 3; // trường hợp #

                            LinearLayout.LayoutParams _paramsItemRow = new LinearLayout.LayoutParams((int)CmmDroidFunction.ConvertDpToPixel((_textlength * _dpPerCharacter) + 25, _context),
                                (int)CmmDroidFunction.ConvertDpToPixel(45, _context)); // + thêm 2 cái margin Left Right là 20dp

                            _holderItem._lnContent.LayoutParameters = _paramsItemRow;
                            #endregion

                            if (position == _allRowCount - 1 && _isShowSumColumn == true) // Sum Cell
                            {
                                BindingSumData(_itemHeader, _lstJObjectRow, _holderItem._tvContent);
                            }
                            else // Value Cell - Position # Cell
                            {
                                if (!String.IsNullOrEmpty(_itemHeader.internalName))
                                {
                                    JObject _currentJObjectRow = _lstJObjectRow[position - 1]; // 0 là parent nên phải -1
                                    BindingValueData(_itemHeader, _currentJObjectRow, _holderItem._tvContent);
                                }
                                else // trường hợp #
                                    _holderItem._tvContent.Text = position.ToString();
                            }
                            _holder._lnContent.AddView(_holderItem.ItemView);
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnBindViewHolder", ex);
#endif
                }
            }

            #region View Holder
            public class AdapterListGridData_ViewHolder_Header : RecyclerView.ViewHolder
            {
                public LinearLayout _lnContent { get; set; }
                public AdapterListGridData_ViewHolder_Header(View itemview, Action<int> listener) : base(itemview)
                {
                    _lnContent = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlInputGridDetails_Recy_Content);

                    //_lnAll.Click += (sender, e) =>
                    //{
                    //    listener(base.LayoutPosition);
                    //};
                }
            }
            public class AdapterListGridData_ViewHolder_Value : RecyclerView.ViewHolder
            {
                public LinearLayout _lnContent { get; set; }
                public AdapterListGridData_ViewHolder_Value(View itemview, Action<int> listener) : base(itemview)
                {
                    _lnContent = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlInputGridDetails_Recy_Content);

                    _lnContent.Click += (sender, e) =>
                    {
                        if (listener != null)
                            listener(base.LayoutPosition);
                    };
                }
            }
            public class AdapterListGridData_ViewHolder_Header_Item : RecyclerView.ViewHolder
            {
                public LinearLayout _lnContent { get; set; }
                public TextView _tvContent { get; set; }
                public AdapterListGridData_ViewHolder_Header_Item(View itemview, Action<int> listener) : base(itemview)
                {
                    _lnContent = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlInputGridDetails_Recy_Header);
                    _tvContent = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlInputGridDetails_Recy_Header);
                }
            }
            public class AdapterListGridData_ViewHolder_Value_Item : RecyclerView.ViewHolder
            {
                public LinearLayout _lnContent { get; set; }
                public TextView _tvContent { get; set; }
                public AdapterListGridData_ViewHolder_Value_Item(View itemview, Action<int> listener) : base(itemview)
                {
                    _lnContent = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlInputGridDetails_Recy_Value);
                    _tvContent = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlInputGridDetails_Recy_Value);
                }
            }
            #endregion
        }
    }
}