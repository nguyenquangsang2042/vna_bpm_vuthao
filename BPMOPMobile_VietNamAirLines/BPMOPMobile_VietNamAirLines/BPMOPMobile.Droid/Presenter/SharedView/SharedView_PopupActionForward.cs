using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content.Res;
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
using Refractored.Controls;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    /// <summary>
    /// Xài cho Action_Forward_RequestIdea
    /// </summary>
    class SharedView_PopupActionForward : SharedView_PopupActionBase
    {
        private float popupScaleWidth_Full = 1;
        private ControllerDetailWorkflow CTRLDetailWorkflow = new ControllerDetailWorkflow();
        private BeanWorkflowItem _workflowItem = new BeanWorkflowItem();
        public SharedView_PopupActionForward(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView, float popupScaleWidth_Full) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
        {

            this.popupScaleWidth_Full = popupScaleWidth_Full;

            switch (flagView)
            {
                case (int)FlagViewControlAction.DetailWorkflow:
                    {
                        FragmentDetailWorkflow _frag = new FragmentDetailWorkflow();
                        _workflowItem = _frag._workflowItem;
                        break;
                    }
            }

        }

        public override void InitializeValue_DetailWorkflow(ButtonAction buttonAction)
        {
            base.InitializeValue_DetailWorkflow(buttonAction);
        }

        public override void InitializeView()
        {
            base.InitializeView();
            try
            {
                #region Get View - Init Data
                View _viewPopupAction = _inflater.Inflate(Resource.Layout.PopupAction_ChooseUserAndAction, null);

                LinearLayout _lnChooseUser = _viewPopupAction.FindViewById<LinearLayout>(Resource.Id.ln_PopupAction_ChooseUserAndAction_ChooseUser);
                ImageView _imgCloseChooseUser = _viewPopupAction.FindViewById<ImageView>(Resource.Id.img_PopupAction_ChooseUserAndAction_ChooseUser_Close);
                ImageView _imgAcceptChooseUser = _viewPopupAction.FindViewById<ImageView>(Resource.Id.img_PopupAction_ChooseUserAndAction_ChooseUser_Accept);
                ImageView _imgChooseUser = _viewPopupAction.FindViewById<ImageView>(Resource.Id.img_PopupAction_ChooseUserAndAction_ChooseUser);
                TextView _tvTitleChooseUser = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_ChooseUserAndAction_ChooseUser_Title);
                EditText _edtSearchChooseUser = _viewPopupAction.FindViewById<EditText>(Resource.Id.edt_PopupAction_ChooseUserAndAction_ChooseUser_Search);
                RecyclerView _recyChooseUser = _viewPopupAction.FindViewById<RecyclerView>(Resource.Id.recy_PopupAction_ChooseUserAndAction_ChooseUser);

                LinearLayout _lnAction = _viewPopupAction.FindViewById<LinearLayout>(Resource.Id.ln_PopupAction_ChooseUserAndAction_Action);
                LinearLayout _lnActionSelectedUser = _viewPopupAction.FindViewById<LinearLayout>(Resource.Id.ln_PopupAction_ChooseUserAndAction_Action_SelectedUser);
                ImageView _imgAction = _viewPopupAction.FindViewById<ImageView>(Resource.Id.img_PopupAction_ChooseUserAndAction_Action);
                TextView _tvTitleAction = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_ChooseUserAndAction_Action_Title);
                //RecyclerView _recyUserAction = _viewPopupAction.FindViewById<RecyclerView>(Resource.Id.recy_PopupAction_ChooseUserAndAction_Action_User);
                ImageView _imgUserAction = _viewPopupAction.FindViewById<ImageView>(Resource.Id.img_PopupAction_ChooseUserAndAction_Action_User);
                EditText _edtCommentAction = _viewPopupAction.FindViewById<EditText>(Resource.Id.edt_PopupAction_ChooseUserAndAction_Action_Comment);
                TextView _tvCloseAction = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_ChooseUserAndAction_Action_Close);
                TextView _tvDoneAction = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_ChooseUserAndAction_Action_Done);
                LinearLayout _lnChooseUserAction = _viewPopupAction.FindViewById<LinearLayout>(Resource.Id.ln_PopupAction_ChooseUserAndAction_Action_User);
                CircleImageView _imgAvatarAction = _viewPopupAction.FindViewById<CircleImageView>(Resource.Id.img_PopupAction_ChooseUserAndAction_Action_Avatar);
                TextView _tvAvatarAction = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_ChooseUserAndAction_Action_Avatar);
                TextView _tvUserNameAction = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_ChooseUserAndAction_Action_UserName);
                TextView _tvEmailAction = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_ChooseUserAndAction_Action_Email);
                TextView _tvPleaseChooseUserAction = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_ChooseUserAndAction_Action_PleaseChooseUser);

                _lnActionSelectedUser.Visibility = ViewStates.Gone;
                _lnAction.Visibility = ViewStates.Visible;
                _lnChooseUser.Visibility = ViewStates.Gone;

                // Action
                _tvTitleAction.Text = buttonAction.Title;
                _tvPleaseChooseUserAction.Text = CmmFunction.GetTitle("TEXT_CONTROL_CHOOSE_USERS", "Tìm người");
                _edtCommentAction.Hint = CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến");
                _tvCloseAction.Text = CmmFunction.GetTitle("TEXT_EXIT", "Thoát");
                _tvDoneAction.Text = buttonAction.Title;
                // Choose User
                _tvTitleChooseUser.Text = CmmFunction.GetTitle("MESS_REQUIRE_USER", "Vui lòng chọn người thực hiện");
                _tvPleaseChooseUserAction.Text = CmmFunction.GetTitle("TEXT_CONTROL_CHOOSE_USERS", "Tìm người");
                _edtSearchChooseUser.Hint = CmmFunction.GetTitle("TEXT_HINT_USER_EMAIL", "Vui lòng nhập tên hoặc địa chỉ email...");

                string _imageName = "icon_bpm_Btn_action_" + buttonAction.ID.ToString();
                int resId = _mainAct.Resources.GetIdentifier(_imageName.ToLowerInvariant(), "drawable", _mainAct.PackageName);
                _imgAction.SetImageResource(resId);

                SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                List<BeanUser> _lstUserAll = new List<BeanUser>();
                BeanUser _selectedUser = new BeanUser();

                #endregion

                #region Show View           
                Dialog _dialogAction = new Dialog(_rootView.Context);
                Window window = _dialogAction.Window;
                _dialogAction.RequestWindowFeature(1);
                _dialogAction.SetCanceledOnTouchOutside(false);
                _dialogAction.SetCancelable(true);
                window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                window.SetGravity(GravityFlags.Center);
                var dm = _mainAct.Resources.DisplayMetrics;

                _dialogAction.SetContentView(_viewPopupAction);
                _dialogAction.Show();
                WindowManagerLayoutParams s = window.Attributes;
                s.Width = (int)(dm.WidthPixels * popupScaleWidth_Full);
                s.Height = WindowManagerLayoutParams.WrapContent;
                window.Attributes = s;
                window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                #endregion

                #region Event
                _edtSearchChooseUser.TextChanged += (sender, e) =>
                {

                    if (!String.IsNullOrEmpty(_edtSearchChooseUser.Text))
                    {
                        string queryNotify = string.Format("SELECT * FROM BeanUser WHERE ( FullName LIKE '%{0}%' OR Email LIKE '%{0}%') AND Email <> '{1}' ORDER BY FullName ", _edtSearchChooseUser.Text, CmmVariable.SysConfig.Email);

                        if (buttonAction.ID == (int)(WorkflowAction.Action.RequestIdea) && !String.IsNullOrEmpty(_workflowItem.AssignedTo)) // Tham vấn check ko hiện mấy thằng trong bước đó, Foward thì bình thường
                        {
                            string _currentAssignedTo = String.Format("('{0}')", _workflowItem.AssignedTo);
                            _currentAssignedTo = _currentAssignedTo.Replace(",", "','").ToLowerInvariant();
                            queryNotify = string.Format("SELECT * FROM BeanUser WHERE ( FullName LIKE '%{0}%' OR Email LIKE '%{0}%') AND Email <> '{1}' AND ID NOT IN {2} ORDER BY FullName ", _edtSearchChooseUser.Text, CmmVariable.SysConfig.Email, _currentAssignedTo);
                        }
                        _lstUserAll = conn.Query<BeanUser>(queryNotify);
                    }
                    else
                    {
                        string queryNotify = string.Format("SELECT * FROM BeanUser WHERE Email <> '{0}' ORDER BY FullName ", CmmVariable.SysConfig.Email);

                        if (buttonAction.ID == (int)(WorkflowAction.Action.RequestIdea) && !String.IsNullOrEmpty(_workflowItem.AssignedTo)) // Tham vấn check ko hiện mấy thằng trong bước đó, Foward thì bình thường
                        {
                            string _currentAssignedTo = String.Format("('{0}')", _workflowItem.AssignedTo);
                            _currentAssignedTo = _currentAssignedTo.Replace(",", "','").ToLowerInvariant();
                            queryNotify = string.Format("SELECT * FROM BeanUser WHERE Email <> '{0}' AND ID NOT IN {1} ORDER BY FullName ", CmmVariable.SysConfig.Email, _currentAssignedTo);

                        }
                        _lstUserAll = conn.Query<BeanUser>(queryNotify);
                    }
                    if (_lstUserAll != null && _lstUserAll.Count >= 0)
                    {
                        AdapterDetailChooseUser _adapterListUser = new AdapterDetailChooseUser((MainActivity)base._mainAct, _rootView.Context, _lstUserAll, _selectedUser);
                        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);

                        _recyChooseUser.SetAdapter(_adapterListUser);
                        _recyChooseUser.SetLayoutManager(staggeredGridLayoutManager);
                        _adapterListUser.CustomItemClick += (sender, e) =>
                        {
                            _tvPleaseChooseUserAction.Visibility = ViewStates.Gone;
                            _lnActionSelectedUser.Visibility = ViewStates.Visible;
                            _selectedUser = e;

                            if (_selectedUser != null && !String.IsNullOrEmpty(_selectedUser.ID))
                            {
                                _lnChooseUser.Visibility = ViewStates.Gone;
                                _lnAction.Visibility = ViewStates.Visible;

                                Window window = _dialogAction.Window;
                                WindowManagerLayoutParams s = window.Attributes;
                                var dm = _mainAct.Resources.DisplayMetrics;
                                s.Width = (int)(dm.WidthPixels * popupScaleWidth_Full);
                                s.Height = WindowManagerLayoutParams.WrapContent;
                                window.Attributes = s;

                                #region Set Data for Action View
                                if (!String.IsNullOrEmpty(_selectedUser.ImagePath))
                                {
                                    _imgAvatarAction.Visibility = ViewStates.Visible;
                                    _tvAvatarAction.Visibility = ViewStates.Gone;

                                    CmmDroidFunction.SetAvataByImagePath(_mainAct, _selectedUser.ImagePath, _imgAvatarAction, _tvAvatarAction, 50);
                                }
                                else
                                {
                                    _imgAvatarAction.Visibility = ViewStates.Gone;
                                    _tvAvatarAction.Visibility = ViewStates.Visible;

                                    _tvAvatarAction.Text = CmmFunction.GetAvatarName(_selectedUser.AccountName);
                                    _tvAvatarAction.BackgroundTintList = ColorStateList.ValueOf(CTRLDetailWorkflow.GetColorByUserName(_rootView.Context, _selectedUser.AccountName));
                                }

                                if (!String.IsNullOrEmpty(_selectedUser.FullName))
                                {
                                    _tvUserNameAction.Text = _selectedUser.FullName;
                                }
                                else
                                {
                                    _tvUserNameAction.Text = "";
                                }

                                if (!String.IsNullOrEmpty(_selectedUser.Email))
                                {
                                    _tvEmailAction.Text = _selectedUser.Email;
                                }
                                else
                                {
                                    _tvEmailAction.Text = "";
                                }

                                #endregion
                            }
                            else
                            {
                                CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("K_Action_PleaseChooseUser", "Vui lòng chọn người để thực hiện."),
                                                                           CmmFunction.GetTitle("K_Action_PleaseChooseUser", "Please choose user to do action."));
                            }
                        };
                    }
                };

                _imgCloseChooseUser.Click += (sender, e) =>
                {
                    ////_dialogAction.Dismiss();

                    _lnChooseUser.Visibility = ViewStates.Gone;
                    _lnAction.Visibility = ViewStates.Visible;

                    Window window = _dialogAction.Window;
                    WindowManagerLayoutParams s = window.Attributes;
                    var dm = _mainAct.Resources.DisplayMetrics;
                    s.Width = (int)(dm.WidthPixels * popupScaleWidth_Full);
                    s.Height = WindowManagerLayoutParams.WrapContent;
                    window.Attributes = s;
                };

                _imgAcceptChooseUser.Click += (sender, e) =>
                {

                };

                _edtCommentAction.TextChanged += (sender, e) =>
                {
                    if (String.IsNullOrEmpty(_edtCommentAction.Text))
                        _edtCommentAction.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Italic);
                    else
                        _edtCommentAction.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                };

                _lnChooseUserAction.Click += (sender, e) =>
                {
                    _lnAction.Visibility = ViewStates.Gone;
                    _lnChooseUser.Visibility = ViewStates.Visible;

                    Window window = _dialogAction.Window;
                    WindowManagerLayoutParams s = window.Attributes;
                    var dm = _mainAct.Resources.DisplayMetrics;
                    s.Width = (int)(dm.WidthPixels);
                    s.Height = (int)(dm.HeightPixels);
                    window.Attributes = s;
                };

                _tvCloseAction.Click += (sender, e) =>
                {
                    _dialogAction.Dismiss();
                };

                _tvDoneAction.Click += (sender, e) =>
                {
                    CmmDroidFunction.HideSoftKeyBoard(_edtCommentAction, _mainAct);
                    if (CTRLDetailWorkflow.CheckActionHasSelectedUser(_mainAct, _selectedUser) == true)
                    {
                        if (CTRLDetailWorkflow.CheckActionHasComment(_mainAct, _edtCommentAction) == true)
                        {
                            switch (flagView)
                            {
                                case (int)FlagViewControlAction.DetailWorkflow:
                                    {
                                        FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;

                                        KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>("userValues", _selectedUser.AccountName);
                                        List<KeyValuePair<string, string>> _lstExtent = new List<KeyValuePair<string, string>>();
                                        _lstExtent.Add(keyValuePair);

                                        _frag.Action_SendAPI(buttonAction, !String.IsNullOrEmpty(_edtCommentAction.Text) ? _edtCommentAction.Text : "", _lstExtent);

                                        break;
                                    }
                            }
                            _dialogAction.Dismiss();
                        }
                    }
                };
                #endregion

                _edtSearchChooseUser.Text = ""; // set text để call event text changed lần đầu
                _edtCommentAction.Text = "";
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