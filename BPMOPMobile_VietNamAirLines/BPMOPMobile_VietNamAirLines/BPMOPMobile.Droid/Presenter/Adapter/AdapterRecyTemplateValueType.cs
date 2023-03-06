using System;
using System.Collections.Generic;
using System.Globalization;
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
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Component;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    public class AdapterRecyTemplateValueType : RecyclerView.Adapter
    {
        private MainActivity _mainAct;
        private Context _context;
        private ViewElement _element;
        public List<BeanWFDetailsHeader> _lstHeader = new List<BeanWFDetailsHeader>();
        private List<JObject> ListJObjectRow = new List<JObject>();
        private JObject _clickedObject = new JObject();
        private int _flagView = -1;

        private int _flagAction;

        /// <summary>
        /// Constructor trường hợp edit
        /// </summary>
        public AdapterRecyTemplateValueType(MainActivity _mainAct, Context _context, ViewElement _element, JObject _clickedObject, int _flagView)
        {
            _flagAction = (int)EnumFormControlInnerAction.ControlInputGridDetails_InnerActionID.Edit;

            this._mainAct = _mainAct;
            this._context = _context;
            this._element = _element;
            this._clickedObject = _clickedObject;
            this._flagView = _flagView;

            #region List Component Header
            try
            {
                if (!String.IsNullOrEmpty(_element.DataSource))
                    _lstHeader = JsonConvert.DeserializeObject<List<BeanWFDetailsHeader>>(_element.DataSource);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "AdapterRecyTemplateValueType", ex);
#endif
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
        }

        /// <summary>
        /// Constructor trường hợp tạo mới
        /// </summary>
        public AdapterRecyTemplateValueType(MainActivity _mainAct, Context _context, ViewElement _element, int _flagView)
        {
            _flagAction = (int)EnumFormControlInnerAction.ControlInputGridDetails_InnerActionID.Create;

            this._mainAct = _mainAct;
            this._context = _context;
            this._element = _element;
            this._flagView = _flagView;

            #region List Component Header
            try
            {
                if (!String.IsNullOrEmpty(_element.DataSource))
                    _lstHeader = JsonConvert.DeserializeObject<List<BeanWFDetailsHeader>>(_element.DataSource);
            }
            catch (Exception)
            {
                _lstHeader = new List<BeanWFDetailsHeader>();
            }
            #endregion

            #region Component Value
            try
            {
                foreach (BeanWFDetailsHeader itemHeader in _lstHeader)
                {
                    if (!String.IsNullOrEmpty(itemHeader.internalName))
                        _clickedObject[itemHeader.internalName] = "";
                }
                _clickedObject["ID"] = 0; // bằng 0 là trường hộp tạo mới
            }
            catch (Exception)
            {
                _clickedObject = new JObject();
            }
            #endregion
        }

        public int GetCurrentFlagAction()
        {
            return _flagAction;
        }
        public void UpdateCurrentJObject(JObject _clickedObject)
        {
            try
            {
                this._clickedObject = _clickedObject;

                #region Handle Formula
                foreach (BeanWFDetailsHeader header in _lstHeader)
                {
                    // TESTING -> MỐT RÁP CODE SAU
                    //string _testformula = "[SoLuong]*[DonGia]";
                    //JToken _JTokenFormula = JToken.Parse(CmmFunction.CalculateObject(_testformula, _clickedObject).ToString());

                    if (!String.IsNullOrEmpty(header.formula))
                    {
                        JToken _JTokenFormula = JToken.Parse(CmmFunction.CalculateObject(header.formula, _clickedObject).ToString());
                        switch (_clickedObject[header.internalName].Type) // kiểm tra type của Jtoken cần Update
                        {
                            case JTokenType.Integer:
                            case JTokenType.Float:
                                {
                                    _clickedObject[header.internalName] = double.Parse(_JTokenFormula.ToString());
                                    break;
                                }
                            case JTokenType.String:
                            default: // default = string
                                {
                                    _clickedObject[header.internalName] = _JTokenFormula.ToString();
                                    break;
                                }
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "UpdateCurrentJObject", ex);
#endif
            }
        }
        public JObject GetCurrentJObject()
        {
            return _clickedObject;
        }
        public override int ItemCount => _lstHeader.Count;
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemExpandDetailWorkflow, parent, false);
            AdapterRecyTemplateValueTypeHolder holder = new AdapterRecyTemplateValueTypeHolder(itemView, null);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                BeanWFDetailsHeader _currentHeader = _lstHeader[position];

                if (!String.IsNullOrEmpty(_currentHeader.internalName))
                {
                    ViewElement _elementItem = new ViewElement();
                    _elementItem.Value = "";
                    //_elementItem.Enable = !_currentHeader.viewOnly; // viewonly = true -> Enable = false
                    _elementItem.Enable = _element.Enable;
                    _elementItem.InternalName = _currentHeader.internalName;
                    _elementItem.DataSource = "";
                    _elementItem.Title = _currentHeader.Title;
                    _elementItem.IsRequire = _currentHeader.require;
                    _elementItem.DataType = _currentHeader.dataType;
                    _elementItem.DataSource = _currentHeader.DataSource;

                    try // gán Value
                    {
                        _elementItem.Value = _clickedObject[_currentHeader.internalName.ToString()].ToString();
                    }
                    catch (Exception)
                    {
                        _elementItem.Value = "";
                    }
                    AdapterRecyTemplateValueTypeHolder _holder = holder as AdapterRecyTemplateValueTypeHolder;
                    _holder._lnContent.RemoveAllViews();

                    TemplateValueType _templateValue = new TemplateValueType(_mainAct, _holder._lnContent, _element, _elementItem, _clickedObject, true, _flagView);
                    _templateValue.InitializeFrameView(_holder._lnContent);
                    _templateValue.InitializeCategory();
                    _templateValue.SetTitle();
                    _templateValue.SetValue();
                    _templateValue.SetEnable();
                    _templateValue.SetProprety();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnBindViewHolder", ex);
#endif
            }
        }

        public class AdapterRecyTemplateValueTypeHolder : RecyclerView.ViewHolder
        {
            public LinearLayout _lnContent { get; set; }
            public AdapterRecyTemplateValueTypeHolder(View itemview, Action<int> listener) : base(itemview)
            {
                _lnContent = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailWorkflow_Content);
                //_imgDelete.Click += (sender, e) => listenerDelete(base.LayoutPosition);
            }
        }
    }
}