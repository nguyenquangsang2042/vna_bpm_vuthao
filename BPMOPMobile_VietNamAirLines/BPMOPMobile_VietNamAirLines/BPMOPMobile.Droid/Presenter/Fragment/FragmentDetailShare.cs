using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content.Res;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Adapter;
using Com.Google.Android.Flexbox;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Presenter.Fragment
{

    class FragmentDetailShare : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private LayoutInflater _inflater;
        private LinearLayout _lnShare, _lnChooseUser, _lnAll;
        private View _rootView;
        private TextView _tvName, _tvUser, _tvComment, _tvShareHistory;
        private Dialog _dialogAction, _dialogPopupControl;
        private ImageView _imgBack, _imgDone, _imgUser;
        private EditText _edtComment;
        private ExpandableListView _expandHistory;
        private BeanWorkflowItem _workflowItem = new BeanWorkflowItem();
        private BeanNotify _notifyItem = new BeanNotify();
        private CustomFlexBoxRecyclerView _recyUser;
        private AdapterSelectUserGroupMultiple_Text _adapterListUserText;
        private AdapterExpandActionShare_History _adapterShareHistory;
        private ControllerDetailShare CTRLDetailShare = new ControllerDetailShare();
        private FragmentDetailWorkflow _fragmentDetailWorkflow;

        private List<BeanUserAndGroup> _lstCurrentUserGroup = new List<BeanUserAndGroup>();
        private List<BeanShareHistory> _lstShareHistory = new List<BeanShareHistory>();
        private List<KeyValuePair<string, string>> _lstExtent = new List<KeyValuePair<string, string>>();

        public FragmentDetailShare() { /* Prevent Darkmode */ }

        public FragmentDetailShare(FragmentDetailWorkflow _fragmentDetailWorkflow, BeanWorkflowItem _workflowItem, BeanNotify _notifyItem)
        {
            this._workflowItem = _workflowItem;
            this._notifyItem = _notifyItem;
            this._fragmentDetailWorkflow = _fragmentDetailWorkflow;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _inflater = inflater;
            _mainAct = (MainActivity)this.Activity;
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewDetailShare, null);
                _lnAll = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailShare_All);
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailShare_Back);
                _imgDone = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailShare_Done);
                _recyUser = _rootView.FindViewById<CustomFlexBoxRecyclerView>(Resource.Id.recy_ViewDetailShare_User);
                _tvName = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailShare_Name);
                _tvUser = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailShare_User);
                _tvShareHistory = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailShare_History);
                _lnChooseUser = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailShare_User);
                _tvComment = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailShare_Comment);
                _imgUser = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailShare_User);
                _edtComment = _rootView.FindViewById<EditText>(Resource.Id.edt_ViewDetailShare_Comment);
                _expandHistory = _rootView.FindViewById<ExpandableListView>(Resource.Id.expand_ViewDetailShare_History);
                _lnShare = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailShare_History);
                _recyUser.SetMaxRowAndRowHeight((int)CmmDroidFunction.ConvertDpToPixel(35, _rootView.Context), 3);

                _expandHistory.SetGroupIndicator(null);
                _expandHistory.SetChildIndicator(null);
                _expandHistory.DividerHeight = 0;
                _expandHistory.GroupClick += (sender, e) => { }; // để disable group collapse

                _lnChooseUser.Click += Click_imgUser;
                _imgUser.Click += Click_imgUser;
                _imgBack.Click += Click_imgBack;
                _imgDone.Click += Click_imgDone;
                _edtComment.TextChanged += TextChanged_edtComment;

                _lnAll.Click += (sender, e) => { };
            }
            SetView();
            GetAndSetHistoryFromServer();
            SetData();

            return _rootView;
        }

        private void GetAndSetHistoryFromServer()
        {
            try
            {
                ProviderControlDynamic pDynamic = new ProviderControlDynamic();
                _lstShareHistory = pDynamic.GetListShareHistory(_workflowItem);

                if (_lstShareHistory != null && _lstShareHistory.Count > 0)
                {
                    _lnShare.Visibility = ViewStates.Visible;
                    List<BeanGroupShareHistory> _lstGroupHistory = CTRLDetailShare.CloneListGroupShareHistory(_lstShareHistory);
                    _adapterShareHistory = new AdapterExpandActionShare_History(_mainAct, _rootView.Context, _lstGroupHistory);
                    _expandHistory.SetAdapter(_adapterShareHistory);

                    _expandHistory.SetAdapter(_adapterShareHistory);
                    for (int i = 0; i < _adapterShareHistory.GroupCount; i++)
                    {
                        _expandHistory.ExpandGroup(i);
                    }
                }
                else
                {
                    _lnShare.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
                _lnShare.Visibility = ViewStates.Gone;
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetListHistoryFromServer", ex);
#endif
            }
        }

        #region Event
        private void SetView()
        {
            try
            {
                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    _tvName.Text = CmmFunction.GetTitle("TEXT_SHARE", "Chia sẻ");
                    _tvUser.Text = CmmFunction.GetTitle("TEXT_SHARE_USERORGROUP", "Chọn người hoặc group muốn chia sẻ");
                    _tvComment.Text = CmmFunction.GetTitle("TEXT_SHARE_IDEA", "Ý kiến");
                    _tvShareHistory.Text = CmmFunction.GetTitle("TEXT_SHARE_HISTORY", "Lịch sử chia sẻ");
                    ////_edtComment.Hint = CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến");
                }
                else
                {
                    _tvName.Text = CmmFunction.GetTitle("TEXT_SHARE", "Share");
                    _tvUser.Text = CmmFunction.GetTitle("TEXT_SHARE_USERORGROUP", "Share with user or group");
                    _tvComment.Text = CmmFunction.GetTitle("TEXT_SHARE_IDEA", "Idea");
                    _tvShareHistory.Text = CmmFunction.GetTitle("TEXT_SHARE_HISTORY", "Sharing history");
                    ////_edtComment.Hint = CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Leave a comment/ opinion here");
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - SetView - Error: " + ex.Message);
#endif
            }
        }

        private void Click_imgBack(object sender, EventArgs e)
        {
            try
            {
                _mainAct.HideFragment();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgBack", ex);
#endif
            }
        }

        private void Click_imgDone(object sender, EventArgs e)
        {
            try
            {
                if (_lstCurrentUserGroup == null || _lstCurrentUserGroup.Count <= 0)
                {
                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("MESS_REQUIRE_USERGROUP", "Vui lòng chọn người để thực hiện."),
                                                               CmmFunction.GetTitle("MESS_REQUIRE_USERGROUP", "Please choose user to do action."));

                    return;
                }
                else
                {
                    List<string> _lstAccountName = new List<string>();
                    for (int i = 0; i < _lstCurrentUserGroup.Count; i++)
                    {
                        if (_lstCurrentUserGroup[i].Type == 0) // User -> Lấy AccountName
                        {
                            ////SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                            ////string query = string.Format("SELECT * FROM BeanUser WHERE FullName = '{0}'", _lstCurrentUserGroup[i].Name);
                            ////List<BeanUser> lstUser = conn.Query<BeanUser>(query);
                            ////if (lstUser != null && lstUser.Count > 0)
                            ////{
                            ////    _lstAccountName.Add(lstUser[0].AccountName);
                            ////}
                            ////conn.Close();
                            if (!String.IsNullOrEmpty(_lstCurrentUserGroup[i].AccountName))
                                _lstAccountName.Add(_lstCurrentUserGroup[i].AccountName);
                        }
                        else
                            _lstAccountName.Add(_lstCurrentUserGroup[i].Name);
                    }
                    string _userValues = String.Join(";", _lstAccountName.ToArray());

                    ButtonAction buttonAction = new ButtonAction() { Value = "Share" };
                    _lstExtent = new List<KeyValuePair<string, string>>();
                    _lstExtent.Add(new KeyValuePair<string, string>("userValues", _userValues));

                    Action_SendAPI(buttonAction, _edtComment.Text, _lstExtent);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgDone", ex);
#endif
            }
        }

        private void Click_imgUser(object sender, EventArgs e)
        {
            try
            {
                List<BeanUserAndGroup> _lstUserAndGroupAll = new List<BeanUserAndGroup>();
                List<BeanUserAndGroup> _lstSelected = _lstCurrentUserGroup.ToList();

                #region Get View - Init Data
                View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_ChooseUser, null);
                LinearLayout _lnChooseUser = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControl_ChooseUser);
                ImageView _imgCloseChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Close);
                ImageView _imgAcceptChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Accept);
                ImageView _imgDeleteChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Delete);
                TextView _tvTitleChooseUser = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_ChooseUser_Title);
                EditText _edtSearchChooseUser = _viewPopupControl.FindViewById<EditText>(Resource.Id.edt_PopupControl_ChooseUser_Search);
                RecyclerView _recyChooseUser = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_ChooseUser);
                CustomFlexBoxRecyclerView _recySelectedUser = _viewPopupControl.FindViewById<CustomFlexBoxRecyclerView>(Resource.Id.recy_PopupControl_SelectedUser);

                _recySelectedUser.Visibility = ViewStates.Visible;
                _imgDeleteChooseUser.Visibility = ViewStates.Visible;
                _recySelectedUser.SetMaxRowAndRowHeight((int)CmmDroidFunction.ConvertDpToPixel(35, _rootView.Context), 3); // 95 *3

                _tvTitleChooseUser.Text = CmmFunction.GetTitle("TEXT_TITLE_USERGROUP", "Chọn người hoặc nhóm");
                _edtSearchChooseUser.Hint = CmmFunction.GetTitle("MESS_REQUIRE_USERGROUP", "Vui lòng chọn người hoặc nhóm để thực hiện");

                SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                ////_lstUserAndGroupAll = conn.Query<BeanUserAndGroup>(CTRLDetailWorkflow._queryBeanUserGroup);
                _lstUserAndGroupAll = conn.Query<BeanUserAndGroup>(CTRLDetailShare._queryShareUserGroup);
                conn.Close();

                if (_lstUserAndGroupAll != null && _lstUserAndGroupAll.Count > 0 && _lstSelected != null && _lstSelected.Count > 0) // Người đã được chọn sẽ không hiển thị vào list     
                {
                    for (int i = 0; i < _lstSelected.Count; i++)
                    {
                        _lstUserAndGroupAll = _lstUserAndGroupAll.Where(x => !x.ID.Equals(_lstSelected[i].ID)).ToList();
                    }
                }

                if (_lstUserAndGroupAll == null) _lstUserAndGroupAll = new List<BeanUserAndGroup>();
                if (_lstSelected == null) _lstSelected = new List<BeanUserAndGroup>();

                AdapterSelectUserGroupMultiple _adapterListUser = new AdapterSelectUserGroupMultiple(_mainAct, _rootView.Context, _lstUserAndGroupAll, _lstSelected);
                AdapterSelectUserGroupMultiple_Text _adapterListUserSelected = new AdapterSelectUserGroupMultiple_Text(_mainAct, _rootView.Context, _lstSelected);
                FlexboxLayoutManager flexboxLayoutManager = new FlexboxLayoutManager(_rootView.Context);
                flexboxLayoutManager.FlexDirection = FlexDirection.Row;
                flexboxLayoutManager.JustifyContent = JustifyContent.FlexStart;

                _recySelectedUser.SetAdapter(_adapterListUserSelected);
                _recySelectedUser.SetLayoutManager(flexboxLayoutManager);
                #endregion

                #region Event
                _edtSearchChooseUser.TextChanged += async (sender, e) =>
                {
                    await Task.Run(() =>
                    {
                        List<BeanUserAndGroup> _lstSearch = new List<BeanUserAndGroup>();
                        if (!String.IsNullOrEmpty(_edtSearchChooseUser.Text))
                        {
                            _lstSearch = _lstUserAndGroupAll.Where(x => x.Email != null && x.Email != CmmVariable.SysConfig.Email).ToList();
                            _lstSearch = _lstSearch.Where(x => CmmFunction.removeSignVietnamese(x.Name).ToLowerInvariant().Contains(CmmFunction.removeSignVietnamese(_edtSearchChooseUser.Text).ToLowerInvariant())
                                         || CmmFunction.removeSignVietnamese(x.Email).ToLowerInvariant().Contains(CmmFunction.removeSignVietnamese(_edtSearchChooseUser.Text).ToLowerInvariant())).ToList();
                        }
                        else
                        {
                            _lstSearch = new List<BeanUserAndGroup>();
                        }

                        _adapterListUser = new AdapterSelectUserGroupMultiple(_mainAct, _rootView.Context, _lstSearch, _lstSelected);
                        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);

                        _mainAct.RunOnUiThread(() =>
                        {
                            _recyChooseUser.SetAdapter(_adapterListUser);
                            _recyChooseUser.SetLayoutManager(staggeredGridLayoutManager);
                        });
                        _adapterListUser.CustomItemClick += (sender, e) =>
                        {
                            BeanUserAndGroup _clickedItem = e;
                            if (_clickedItem != null)
                            {
                                _lstSelected.Add(e);
                                _lstUserAndGroupAll = _lstUserAndGroupAll.Where(x => !x.ID.Equals(_clickedItem.ID)).ToList();

                                _adapterListUser.UpdateCurrentList(_lstUserAndGroupAll);
                                _adapterListUser.NotifyDataSetChanged();
                                _adapterListUserSelected.UpdateItemListIsClicked(_lstSelected);
                                _adapterListUserSelected.NotifyDataSetChanged();
                                _edtSearchChooseUser.Text = _edtSearchChooseUser.Text; // để set Adapter lại
                                _edtSearchChooseUser.SetSelection(_edtSearchChooseUser.Text.Length);

                                _recySelectedUser.SmoothScrollToPosition(_lstSelected.Count); // focus lại vi trí cuối cùng
                            }
                            _edtSearchChooseUser.Text = "";
                        };
                    });
                };
                _imgCloseChooseUser.Click += (sender, e) =>
                {
                    CmmDroidFunction.HideSoftKeyBoard(_edtSearchChooseUser, _mainAct);
                    _dialogPopupControl.Dismiss();

                };
                _adapterListUserSelected.CustomItemClick += (sender, e) =>
                {
                    BeanUserAndGroup _clickedItem = e;
                    if (_clickedItem != null)
                    {
                        _lstUserAndGroupAll.Add(e);
                        _lstSelected = _lstSelected.Where(x => !x.ID.Equals(_clickedItem.ID)).ToList(); // Loại Item vừa click ra

                        _adapterListUser.UpdateCurrentList(_lstUserAndGroupAll);
                        _adapterListUser.NotifyDataSetChanged();
                        _adapterListUserSelected.UpdateItemListIsClicked(_lstSelected);
                        _adapterListUserSelected.NotifyDataSetChanged();
                        _edtSearchChooseUser.Text = _edtSearchChooseUser.Text; // để set Adapter lại
                        _edtSearchChooseUser.SetSelection(_edtSearchChooseUser.Text.Length);
                    }
                };

                _imgDeleteChooseUser.Click += (sender, e) =>
                {
                    // Update cho Data bên ngoài
                    _lstCurrentUserGroup = new List<BeanUserAndGroup>();
                    _adapterListUserText.UpdateItemListIsClicked(_lstCurrentUserGroup);
                    _adapterListUserText.NotifyDataSetChanged(); // notify list ở ngoài    
                    SetData();

                    CmmDroidFunction.HideSoftKeyBoard(_edtSearchChooseUser, _mainAct);
                    _dialogPopupControl.Dismiss();
                };

                _imgAcceptChooseUser.Click += (sender, e) =>
                {
                    List<BeanUserAndGroup> _lstResult = new List<BeanUserAndGroup>();
                    if (_adapterListUserSelected != null)
                    {
                        _lstResult = _adapterListUserSelected.GetListIsclicked();
                    }
                    if (_lstResult == null) _lstResult = new List<BeanUserAndGroup>();

                    // Update cho Data bên ngoài
                    _lstCurrentUserGroup = _lstResult;
                    _adapterListUserText.UpdateItemListIsClicked(_lstCurrentUserGroup);
                    _adapterListUserText.NotifyDataSetChanged(); // notify list ở ngoài    
                    SetData();

                    CmmDroidFunction.HideSoftKeyBoard(_edtSearchChooseUser, _mainAct);
                    _dialogPopupControl.Dismiss();
                };
                #endregion

                #region Show View                
                _dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                Window window = _dialogPopupControl.Window;
                _dialogPopupControl.RequestWindowFeature(1);
                _dialogPopupControl.SetCanceledOnTouchOutside(false);
                _dialogPopupControl.SetCancelable(true);
                window.SetGravity(GravityFlags.Bottom);
                window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                var dm = _mainAct.Resources.DisplayMetrics;

                _dialogPopupControl.SetContentView(_viewPopupControl);
                _dialogPopupControl.Show();
                WindowManagerLayoutParams s = window.Attributes;
                s.Width = WindowManagerLayoutParams.MatchParent;
                s.Height = WindowManagerLayoutParams.MatchParent;
                window.Attributes = s;
                #endregion

                _edtSearchChooseUser.Text = "";
                _edtSearchChooseUser.RequestFocus();

                CmmDroidFunction.ShowSoftKeyBoard(_rootView, _mainAct);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "ShowPopup_ControlDate", ex);
#endif
            }
        }

        private void TextChanged_edtComment(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(_edtComment.Text))
                    _edtComment.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Italic);
                else
                    _edtComment.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgBack", ex);
#endif
            }
        }
        #endregion

        #region Data
        private void SetData()
        {
            try
            {
                if (_lstCurrentUserGroup != null && _lstCurrentUserGroup.Count > 0)
                {
                    _adapterListUserText = new AdapterSelectUserGroupMultiple_Text(_mainAct, _rootView.Context, _lstCurrentUserGroup);
                    FlexboxLayoutManager layoutManager = new FlexboxLayoutManager(_rootView.Context);
                    layoutManager.FlexDirection = FlexDirection.Row;
                    layoutManager.JustifyContent = JustifyContent.FlexStart;

                    _adapterListUserText.CustomItemClick += (sender, e) =>
                    {
                        _lstCurrentUserGroup = _lstCurrentUserGroup.Where(x => x.ID != e.ID).ToList();
                        _adapterListUserText.UpdateItemListIsClicked(_lstCurrentUserGroup);
                        _adapterListUserText.NotifyDataSetChanged();
                    };
                    _recyUser.SetAdapter(_adapterListUserText);
                    _recyUser.SetLayoutManager(layoutManager);
                }
                else
                {
                    _lstCurrentUserGroup = new List<BeanUserAndGroup>();
                    _adapterListUserText = new AdapterSelectUserGroupMultiple_Text(_mainAct, _rootView.Context, _lstCurrentUserGroup);
                    FlexboxLayoutManager layoutManager = new FlexboxLayoutManager(_rootView.Context);
                    _recyUser.SetAdapter(_adapterListUserText);
                    _recyUser.SetLayoutManager(layoutManager);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailProcess", "SetData", ex);
#endif
            }
        }

        private async void Action_SendAPI(ButtonAction buttonAction, string comment, List<KeyValuePair<string, string>> _lstExtent)
        {
            try
            {
                CmmDroidFunction.HideSoftKeyBoard(_edtComment, _mainAct);
                CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);

                await Task.Run(() =>
                {
                    #region Get FormDefineInfo
                    JArray jArrayForm = JArray.Parse(_fragmentDetailWorkflow._OBJFORMACTION["form"].ToString());
                    string _formDefineInfo = jArrayForm[0]["FormDefineInfo"].ToString();
                    #endregion

                    bool _result = false;
                    ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();

                    // Nếu Action Có comment -> Add thêm cột idea
                    if (!string.IsNullOrEmpty(comment))
                    {
                        KeyValuePair<string, string> _KeyValueComment = new KeyValuePair<string, string>("idea", comment);
                        if (_lstExtent == null) _lstExtent = new List<KeyValuePair<string, string>>();
                        _lstExtent.Add(_KeyValueComment);
                    }

                    string _messageAPI = "";

                    _result = _pControlDynamic.SendControlDynamicAction(buttonAction.Value, _workflowItem.ID, _formDefineInfo, "", ref _messageAPI, new List<KeyValuePair<string, string>>(), _lstExtent);

                    if (_result)
                    {
                        ProviderBase pBase = new ProviderBase();
                        pBase.UpdateAllMasterData(true);
                        pBase.UpdateAllDynamicData(true);
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.HideProcessingDialog();
                            Click_imgBack(null, null);
                        });
                    }
                    else
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.HideProcessingDialog();
                            if (!String.IsNullOrEmpty(_messageAPI))
                                CmmDroidFunction.ShowAlertDialog(_mainAct, _messageAPI, _messageAPI);
                            else
                                CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"),
                                   CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Action_SendAPI", ex);
#endif
            }
        }
        #endregion
    }
}
