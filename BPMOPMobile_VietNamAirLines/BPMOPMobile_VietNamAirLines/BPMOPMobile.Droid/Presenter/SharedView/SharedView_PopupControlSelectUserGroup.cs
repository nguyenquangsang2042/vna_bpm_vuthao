using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
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
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Adapter;
using BPMOPMobile.Droid.Presenter.Fragment;
using Com.Google.Android.Flexbox;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    public class SharedView_PopupControlSelectUserGroup : SharedView_PopupControlBase
    {
        private ControllerDetailWorkflow CTRLDetailWorkflow = new ControllerDetailWorkflow();
        private bool _isUserAndGroup { get; set; }
        public SharedView_PopupControlSelectUserGroup(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView, bool _isUserAndGroup) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
        {
            this._isUserAndGroup = _isUserAndGroup;
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
                View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_ChooseUser, null);
                LinearLayout _lnChooseUser = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControl_ChooseUser);
                LinearLayout _lnSearch = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControl_ChooseUser_Search);
                ImageView _imgCloseChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Close);
                ImageView _imgAcceptChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Accept);
                ImageView _imgDeleteChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Delete);
                TextView _tvTitleChooseUser = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_ChooseUser_Title);
                EditText _edtSearchChooseUser = _viewPopupControl.FindViewById<EditText>(Resource.Id.edt_PopupControl_ChooseUser_Search);
                RecyclerView _recyChooseUser = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_ChooseUser);
                CustomFlexBoxRecyclerView _recySelectedUser = _viewPopupControl.FindViewById<CustomFlexBoxRecyclerView>(Resource.Id.recy_PopupControl_SelectedUser);

                _imgAcceptChooseUser.Visibility = ViewStates.Invisible; // chọn single người thì luôn ẩn vì chọn từ adapter
                _imgAcceptChooseUser.LayoutParameters.Width = 0;

                _recySelectedUser.Visibility = ViewStates.Visible;

                _recySelectedUser.SetMaxRowAndRowHeight((int)CmmDroidFunction.ConvertDpToPixel(35, _rootView.Context), 1); // 95 *3


                if (_isUserAndGroup == true)
                    _edtSearchChooseUser.Hint = CmmFunction.GetTitle("MESS_REQUIRE_USERGROUP", "Vui lòng chọn người hoặc nhóm để thực hiện");
                else
                    _edtSearchChooseUser.Hint = CmmFunction.GetTitle("MESS_REQUIRE_USER", "Vui lòng chọn người thực hiện");

                List<BeanUserAndGroup> _lstUserAndGroupAll = new List<BeanUserAndGroup>();
                BeanUserAndGroup _selectedUserAndGroup = new BeanUserAndGroup();

                switch (base.flagView)
                {
                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                    default:
                        {
                            _tvTitleChooseUser.Text = elementParent.Title;
                            if (!String.IsNullOrEmpty(elementParent.Value))
                            {
                                List<BeanUserAndGroup> _beanUserAndGroup = JsonConvert.DeserializeObject<List<BeanUserAndGroup>>(elementParent.Value);
                                if (_beanUserAndGroup != null && _beanUserAndGroup.Count > 0)
                                    _selectedUserAndGroup = _beanUserAndGroup[0];
                            }

                            if (elementParent.Enable)
                            {
                                _lnSearch.Visibility = ViewStates.Visible;
                                _imgDeleteChooseUser.Visibility = ViewStates.Visible;
                                CmmDroidFunction.ShowSoftKeyBoard(_rootView, _mainAct);
                            }
                            else
                            {
                                _lnSearch.Visibility = ViewStates.Gone;
                                _imgDeleteChooseUser.Visibility = ViewStates.Gone;
                            }

                            break;
                        }
                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                        {
                            _tvTitleChooseUser.Text = elementPopup.Title;
                            if (!String.IsNullOrEmpty(elementPopup.Value))
                            {
                                List<BeanUserAndGroup> _beanUserAndGroup = JsonConvert.DeserializeObject<List<BeanUserAndGroup>>(elementPopup.Value);
                                if (_beanUserAndGroup != null && _beanUserAndGroup.Count > 0)
                                    _selectedUserAndGroup = _beanUserAndGroup[0];
                            }

                            if (elementPopup.Enable)
                            {
                                _lnSearch.Visibility = ViewStates.Visible;
                                _imgDeleteChooseUser.Visibility = ViewStates.Visible;
                                CmmDroidFunction.ShowSoftKeyBoard(_rootView, _mainAct);
                            }
                            else
                            {
                                _lnSearch.Visibility = ViewStates.Gone;
                                _imgDeleteChooseUser.Visibility = ViewStates.Gone;
                            }

                            break;
                        }
                }

                SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);

                _lstUserAndGroupAll = conn.Query<BeanUserAndGroup>(_isUserAndGroup == true
                                                                    ? CTRLDetailWorkflow._queryBeanUserGroup
                                                                    : CTRLDetailWorkflow._queryBeanUser);
                conn.Close();

                if (_lstUserAndGroupAll != null && _lstUserAndGroupAll.Count > 0 && _selectedUserAndGroup != null)
                {
                    // Người đã được chọn sẽ không hiển thị vào list
                    _lstUserAndGroupAll = _lstUserAndGroupAll.Where(x => !x.ID.Equals(_selectedUserAndGroup.ID)).ToList();
                }

                AdapterSelectUserGroupMultiple_Text _adapterListUserSelected = null;
                if (_selectedUserAndGroup.ID != null)
                {
                    List<BeanUserAndGroup> _lstSelected = new List<BeanUserAndGroup>();
                    _lstSelected.Add(_selectedUserAndGroup);

                    _adapterListUserSelected = new AdapterSelectUserGroupMultiple_Text((MainActivity)base._mainAct, _rootView.Context, _lstSelected, elementParent.Enable);
                    FlexboxLayoutManager flexboxLayoutManager = new FlexboxLayoutManager(_rootView.Context);
                    flexboxLayoutManager.FlexDirection = FlexDirection.Row;
                    flexboxLayoutManager.JustifyContent = JustifyContent.FlexStart;

                    _recySelectedUser.SetAdapter(_adapterListUserSelected);
                    _recySelectedUser.SetLayoutManager(flexboxLayoutManager);
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
                #endregion

                #region Event
                if ((elementParent != null && elementParent.Enable) || (elementPopup != null && elementPopup.Enable))
                {
                    _imgDeleteChooseUser.Click += (sender, e) =>
                    {
                        CmmDroidFunction.HideSoftKeyBoard(_edtSearchChooseUser, _mainAct);
                        switch (base.flagView)
                        {
                            case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                                {
                                    FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                    _frag.UpdateValueForElement(elementParent, "");
                                    _frag._adapterDetailExpandControl.NotifyDataSetChanged();
                                    break;
                                }
                            case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                                {
                                    FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                    _frag.UpdateValueForPopupGridDetail(elementParent, elementPopup, JObjectChild, "");
                                    break;
                                }
                        }
                        _dialogPopupControl.Dismiss();
                    };

                    _edtSearchChooseUser.TextChanged += (sender, e) =>
                    {
                        List<BeanUserAndGroup> _lstSearch = new List<BeanUserAndGroup>();
                        if (!String.IsNullOrEmpty(_edtSearchChooseUser.Text))
                        {
                            _lstSearch = _lstUserAndGroupAll.Where(x => x.Email != null).ToList();
                            _lstSearch = _lstSearch.Where(x => CmmFunction.removeSignVietnamese(x.Name).ToLowerInvariant().Contains(CmmFunction.removeSignVietnamese(_edtSearchChooseUser.Text).ToLowerInvariant())
                                                            || CmmFunction.removeSignVietnamese(x.Email).ToLowerInvariant().Contains(CmmFunction.removeSignVietnamese(_edtSearchChooseUser.Text).ToLowerInvariant())).ToList();
                        }
                        else
                        {
                            _lstSearch = new List<BeanUserAndGroup>();
                        }

                        AdapterSelectUserSingle _adapterListUser = new AdapterSelectUserSingle((MainActivity)base._mainAct, _rootView.Context, _lstSearch, _selectedUserAndGroup);
                        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);

                        _recyChooseUser.SetAdapter(_adapterListUser);
                        _recyChooseUser.SetLayoutManager(staggeredGridLayoutManager);


                        _adapterListUser.CustomItemClick += (sender, e) =>
                        {
                            CmmDroidFunction.HideSoftKeyBoard(_edtSearchChooseUser, _mainAct);
                            _selectedUserAndGroup = e;
                            string _result = "";

                            if (_selectedUserAndGroup != null)
                            {
                                List<BeanUserAndGroup> _lstResult = new List<BeanUserAndGroup>();
                                _lstResult.Add(_selectedUserAndGroup);
                                _result = JsonConvert.SerializeObject(_lstResult);
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

                        if (_adapterListUserSelected != null)
                        {
                            _adapterListUserSelected.CustomItemClick += (sender, e) =>
                            {
                                CmmDroidFunction.HideSoftKeyBoard(_edtSearchChooseUser, _mainAct);
                                switch (base.flagView)
                                {
                                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                                        {
                                            FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                            _frag.UpdateValueForElement(elementParent, "");
                                            _frag._adapterDetailExpandControl.NotifyDataSetChanged();
                                            break;
                                        }
                                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                                        {
                                            FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                            _frag.UpdateValueForPopupGridDetail(elementParent, elementPopup, JObjectChild, "");
                                            break;
                                        }
                                }
                                _dialogPopupControl.Dismiss();
                            };
                        }
                    };
                }
                _imgCloseChooseUser.Click += (sender, e) =>
                {
                    CmmDroidFunction.HideSoftKeyBoard(_edtSearchChooseUser, _mainAct);
                    _dialogPopupControl.Dismiss();

                };
                #endregion

                _edtSearchChooseUser.Text = "";
                _edtSearchChooseUser.RequestFocus();
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