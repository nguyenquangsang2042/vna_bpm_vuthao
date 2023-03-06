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
    public class SharedView_PopupControlSingleLookup : SharedView_PopupControlBase
    {
        public SharedView_PopupControlSingleLookup(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
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
                LinearLayout _lnSearch = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControl_SingleChoice_Search);
                EditText _edtSearch = _viewPopupControl.FindViewById<EditText>(Resource.Id.edt_PopupControl_SingleChoice_Search);

                _imgDone.Visibility = ViewStates.Invisible;
                _imgDone.LayoutParameters.Width = 0;

                _lnSearch.Visibility = ViewStates.Gone;

                if (elementParent.Enable)
                {
                    _imgDelete.Visibility = ViewStates.Visible;
                }
                else
                {
                    _imgDelete.Visibility = ViewStates.Gone;
                }

                List<BeanLookupData> _lstLookupData = new List<BeanLookupData>();

                switch (base.flagView)
                {
                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                    default:
                        {
                            _tvTitle.Text = elementParent.Title;

                            if (!String.IsNullOrEmpty(elementParent.DataSource))
                            {
                                _lstLookupData = JsonConvert.DeserializeObject<List<BeanLookupData>>(elementParent.DataSource);
                                if (_lstLookupData != null && _lstLookupData.Count > 0 && !String.IsNullOrEmpty(elementParent.Value)) // Value có giá trị mới làm
                                {
                                    for (int i = 0; i < _lstLookupData.Count; i++)
                                    {
                                        if (elementParent.Value.Contains(_lstLookupData[i].ID))
                                        {
                                            _lstLookupData[i].IsSelected = true;
                                            break; // vì chỉ có 1 thằng dc chọn nên break luôn
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                        {
                            _tvTitle.Text = elementPopup.Title;

                            if (!String.IsNullOrEmpty(elementPopup.DataSource))
                            {
                                _lstLookupData = JsonConvert.DeserializeObject<List<BeanLookupData>>(elementParent.DataSource);
                                if (_lstLookupData != null && _lstLookupData.Count > 0 && !String.IsNullOrEmpty(elementParent.Value)) // Value có giá trị mới làm
                                {
                                    for (int i = 0; i < _lstLookupData.Count; i++)
                                    {
                                        if (elementParent.Value.Contains(_lstLookupData[i].ID))
                                        {
                                            _lstLookupData[i].IsSelected = true;
                                            break; // vì chỉ có 1 thằng dc chọn nên break luôn
                                        }
                                    }
                                }
                            }
                            break;
                        }
                }

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
                _imgClose.Click += (sender, e) =>
                {
                    _dialogPopupControl.Dismiss();
                };

                _imgDelete.Click += (sender, e) =>
                {
                    switch (base.flagView)
                    {
                        case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                        default:
                            {
                                FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                _frag.UpdateValueForElement(elementParent, "");
                                _frag._adapterDetailExpandControl.NotifyDataSetChanged();
                                _dialogPopupControl.Dismiss();
                            }
                            break;
                        case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                            {
                                FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                _frag.UpdateValueForPopupGridDetail(elementParent, elementPopup, JObjectChild, "");
                                _dialogPopupControl.Dismiss();
                                break;
                            }
                    }
                };

                _edtSearch.TextChanged += (sender, e) =>
                {
                    List<BeanLookupData> _lstSearch = new List<BeanLookupData>();
                    if (!String.IsNullOrEmpty(_edtSearch.Text))
                    {
                        _lstSearch = _lstLookupData.Where(x => x.Title.ToLowerInvariant().Contains(_edtSearch.Text.ToLowerInvariant())).ToList();
                    }
                    else // Full
                    {
                        _lstSearch = _lstLookupData.ToList();
                    }
                    AdapterFormControlSingleChoice _adapterFormControlSingleChoice = new AdapterFormControlSingleChoice((MainActivity)base._mainAct, _rootView.Context, _lstSearch);
                    StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                    _recyData.SetAdapter(_adapterFormControlSingleChoice);
                    _recyData.SetLayoutManager(staggeredGridLayoutManager);

                    if (elementParent.Enable)
                    {
                        _adapterFormControlSingleChoice.CustomItemClick += (sender, e) =>
                        {
                            BeanLookupData _selectedLookupItem = e;
                            List<BeanLookupData> _lstSelected = new List<BeanLookupData>();
                            _lstSelected.Add(_selectedLookupItem);

                            if (!String.IsNullOrEmpty(_selectedLookupItem.Title))
                            {
                                switch (base.flagView)
                                {
                                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                                    default:
                                        {
                                            FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                            _frag.UpdateValueForElement(elementParent, JsonConvert.SerializeObject(_lstSelected));
                                            _frag._adapterDetailExpandControl.NotifyDataSetChanged();
                                            break;
                                        }
                                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                                        {
                                            FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                            _frag.UpdateValueForPopupGridDetail(elementParent, elementPopup, JObjectChild, JsonConvert.SerializeObject(_lstSelected));
                                            break;
                                        }
                                }
                            }
                            _dialogPopupControl.Dismiss();
                        };
                    }
                };
                #endregion

                _edtSearch.Text = "";
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