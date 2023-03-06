using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Presenter.Adapter;
using BPMOPMobile.Droid.Presenter.Fragment;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    class SharedView_PopupControlMultiChoice : SharedView_PopupControlBase
    {
        public SharedView_PopupControlMultiChoice(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
        {

        }

        public override void InitializeValue_Master(ViewElement clickedElement)
        {
            base.InitializeValue_Master(clickedElement);
        }

        public override void InitializeValue_InputGridDetail(ViewElement elementParent, ViewElement elementPopup, JObject JObjectChild)
        {
            base.InitializeValue_InputGridDetail(elementParent, elementPopup, JObjectChild);
        }

        public override void InitializeView()
        {
            base.InitializeView();
            try
            {
                #region Get View - Init Data

                View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_SingleChoice, null);
                ImageView _imgClose = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Close);
                ImageView _imgDelete = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Delete);
                TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_SingleChoice_Title);
                RecyclerView _recyData = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_SingleChoice_Data);
                ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Done);

                List<BeanLookupData> _lstLookupData = new List<BeanLookupData>();

                if (elementParent.Enable)
                {
                    _imgDone.Visibility = ViewStates.Visible;
                    _imgDelete.Visibility = ViewStates.Visible;
                }
                else
                {
                    _imgDone.Visibility = ViewStates.Invisible;
                    _imgDelete.Visibility = ViewStates.Invisible;
                }

                switch (base.flagView)
                {
                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                    default:
                        {
                            _tvTitle.Text = elementParent.Title;
                            _lstLookupData = JsonConvert.DeserializeObject<List<BeanLookupData>>(elementParent.DataSource);

                            if (_lstLookupData != null && _lstLookupData.Count > 0 && !String.IsNullOrEmpty(elementParent.Value))
                            {
                                List<BeanLookupData> _lstValue = JsonConvert.DeserializeObject<List<BeanLookupData>>(elementParent.Value);

                                if (_lstValue == null || _lstValue.Count == 0)
                                    break;
                                foreach (var itemLookup in _lstLookupData)
                                {
                                    foreach (var itemValue in _lstValue)
                                    {
                                        if (itemLookup.ID == itemValue.ID && itemLookup.IsSelected == false) // item nào check qua rồi ko cần check lại.
                                            itemLookup.IsSelected = true;
                                    }
                                }
                            }
                            break;
                        }
                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                        {
                            _tvTitle.Text = elementPopup.Title;
                            _lstLookupData = JsonConvert.DeserializeObject<List<BeanLookupData>>(elementPopup.DataSource);

                            if (_lstLookupData != null && _lstLookupData.Count > 0 && !String.IsNullOrEmpty(elementPopup.Value))
                            {
                                List<BeanLookupData> _lstValue = JsonConvert.DeserializeObject<List<BeanLookupData>>(elementPopup.Value);

                                if (_lstValue == null || _lstValue.Count == 0)
                                    break;
                                foreach (var itemLookup in _lstLookupData)
                                {
                                    foreach (var itemValue in _lstValue)
                                    {
                                        if (itemLookup.ID == itemValue.ID && itemLookup.IsSelected == false) // item nào check qua rồi ko cần check lại.
                                            itemLookup.IsSelected = true;
                                    }
                                }
                            }
                            break;
                        }
                }

                AdapterFormControlMultiChoice _adapterFormControlMultiChoice = new AdapterFormControlMultiChoice((MainActivity)base._mainAct, _rootView.Context, _lstLookupData);
                StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                _recyData.SetAdapter(_adapterFormControlMultiChoice);
                _recyData.SetLayoutManager(staggeredGridLayoutManager);
                #endregion

                #region Show View                
                Dialog _dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                Window window = _dialogPopupControl.Window;
                _dialogPopupControl.RequestWindowFeature(1);
                _dialogPopupControl.SetCanceledOnTouchOutside(false);
                _dialogPopupControl.SetCancelable(true);
                window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                window.SetGravity(GravityFlags.Bottom);
                var dm = _mainAct.Resources.DisplayMetrics;

                _dialogPopupControl.SetContentView(_viewPopupControl);
                _dialogPopupControl.Show();
                WindowManagerLayoutParams s = window.Attributes;
                s.Width = WindowManagerLayoutParams.MatchParent;
                s.Height = WindowManagerLayoutParams.MatchParent;
                window.Attributes = s;
                window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                #endregion

                #region Event

                if (elementParent.Enable) // Nếu control dc Enable mới cho click
                {
                    _adapterFormControlMultiChoice.CustomItemClick += (sender, e) =>
                    {
                        for (int i = 0; i < _lstLookupData.Count; i++)
                        {
                            if (_lstLookupData[i].ID.Equals(e.ID))
                            {
                                _lstLookupData[i].IsSelected = !_lstLookupData[i].IsSelected;
                                _adapterFormControlMultiChoice.NotifyDataSetChanged();
                                break;
                            }
                        }
                    };
                }

                _imgClose.Click += (sender, e) =>
                {
                    _dialogPopupControl.Dismiss();
                };

                _imgDelete.Click += (sender, e) =>
                {
                    for (int i = 0; i < _lstLookupData.Count; i++)
                    {
                        _lstLookupData[i].IsSelected = false;
                    }
                    _imgDone.PerformClick();
                };

                _imgDone.Click += (sender, e) =>
                {
                    string _result = "";

                    List<BeanLookupData> _lstselected = _lstLookupData.Where(x => x.IsSelected == true).ToList(); // lấy ra những thằng đang chọn

                    if (_lstselected != null && _lstselected.Count > 0)
                    {
                        _result = JsonConvert.SerializeObject(_lstselected);
                    }

                    switch (base.flagView)
                    {
                        case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                            {
                                FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                _frag.UpdateValueForElement(elementParent, _result);
                                _frag._adapterDetailExpandControl.NotifyDataSetChanged();
                                break;
                            }
                        case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                            {
                                FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                _frag.UpdateValueForPopupGridDetail(elementParent, elementPopup, JObjectChild, _result);
                                break;
                            }
                    }
                    _dialogPopupControl.Dismiss();
                };

                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "InitializeView", ex);
#endif
            }
        }
    }
}