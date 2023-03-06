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
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Presenter.Adapter;
using BPMOPMobile.Droid.Presenter.Fragment;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    public class SharedView_PopupControlSingleChoice : SharedView_PopupControlBase
    {
        private string _IDNullOption = "_IDNullOption"; // để phân biệt với các option từ server
        public SharedView_PopupControlSingleChoice(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
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
                
                _imgDone.Visibility = ViewStates.Invisible;
                _imgDone.LayoutParameters.Width = 0;

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

                            //BeanLookupData _itemNull = new BeanLookupData() { ID = _IDNullOption, Title = CmmFunction.GetTitle("TEXT_CHOOSE_CONTENT", "Chọn nội dung..."), IsSelected = true };
                            //_lstLookupData.Add(_itemNull);

                            if (!String.IsNullOrEmpty(elementParent.DataSource))
                            {
                                _lstLookupData.AddRange(JsonConvert.DeserializeObject<List<BeanLookupData>>(elementParent.DataSource));
                                if (_lstLookupData != null && _lstLookupData.Count > 0 && !String.IsNullOrEmpty(elementParent.Value)) // Value có giá trị mới làm
                                {
                                    for (int i = 0; i < _lstLookupData.Count; i++)
                                    {
                                        if (elementParent.Value.Contains(_lstLookupData[i].ID))
                                        {
                                            _lstLookupData[i].IsSelected = true;
                                            //_itemNull.IsSelected = false;
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

                            //BeanLookupData _itemNull = new BeanLookupData() { ID = _IDNullOption, Title = CmmFunction.GetTitle("TEXT_CHOOSE_CONTENT", "Chọn nội dung..."), IsSelected = true };
                            //_lstLookupData.Add(_itemNull);

                            if (!String.IsNullOrEmpty(elementPopup.DataSource))
                            {
                                _lstLookupData = JsonConvert.DeserializeObject<List<BeanLookupData>>(elementPopup.DataSource);
                                if (_lstLookupData != null && _lstLookupData.Count > 0 && !String.IsNullOrEmpty(elementPopup.Value)) // Value có giá trị mới làm
                                {
                                    for (int i = 0; i < _lstLookupData.Count; i++)
                                    {
                                        if (elementPopup.Value.Contains(_lstLookupData[i].ID))
                                        {
                                            _lstLookupData[i].IsSelected = true;
                                            //_itemNull.IsSelected = false;
                                            break; // vì chỉ có 1 thằng dc chọn nên break luôn
                                        }
                                    }
                                }
                            }
                            break;
                        }
                }

                AdapterFormControlSingleChoice _adapterFormControlSingleChoice = new AdapterFormControlSingleChoice((MainActivity)base._mainAct, _rootView.Context, _lstLookupData);
                StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                _recyData.SetAdapter(_adapterFormControlSingleChoice);
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
                _imgClose.Click += (sender, e) =>
                {
                    _dialogPopupControl.Dismiss();
                };

                if (elementParent.Enable)
                {
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

                    _adapterFormControlSingleChoice.CustomItemClick += (sender, e) =>
                    {
                        BeanLookupData _selectedLookupItem = e;
                        if (_selectedLookupItem != null)
                        {
                            List<BeanLookupData> _lstResult = new List<BeanLookupData>();
                            _lstResult.Add(_selectedLookupItem);

                            switch (base.flagView)
                            {
                                case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                                default:
                                    {
                                        FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;

                                        //if (e.ID.Equals(_IDNullOption)) // Bỏ chọn
                                        //    _frag.UpdateValueForElement(elementParent, "");
                                        //else
                                        _frag.UpdateValueForElement(elementParent, JsonConvert.SerializeObject(_lstResult));

                                        _frag._adapterDetailExpandControl.NotifyDataSetChanged();
                                        _dialogPopupControl.Dismiss();
                                    }
                                    break;
                                case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                                    {
                                        //// Chỉ cần title của item được chọn
                                        //_frag.UpdateValueForPopupGridDetail(elementParent, elementPopup, JObjectChild, _selectedLookupItem.Title.ToString());
                                        FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;

                                        //if (e.ID.Equals(_IDNullOption)) // Bỏ chọn
                                        //    _frag.UpdateValueForPopupGridDetail(elementParent, elementPopup, JObjectChild, "");
                                        //else
                                        _frag.UpdateValueForPopupGridDetail(elementParent, elementPopup, JObjectChild, JsonConvert.SerializeObject(_lstResult));

                                        _dialogPopupControl.Dismiss();
                                        break;
                                    }
                            }
                        }
                    };
                }
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