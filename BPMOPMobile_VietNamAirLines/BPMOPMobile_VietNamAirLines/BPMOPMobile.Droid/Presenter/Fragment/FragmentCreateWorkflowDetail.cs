using System;
using System.Collections.Generic;
using System.Globalization;
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
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Component;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Core.Presenter;
using BPMOPMobile.Droid.Presenter.Adapter;
using Com.Google.Android.Flexbox;
using Com.Telerik.Widget.Calendar;
using Jp.Wasabeef;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TEditor;
using TEditor.Abstractions;
using static Android.App.ActionBar;
using static Android.App.DatePickerDialog;
using static BPMOPMobile.Droid.Core.Class.EnumFormControlView;
using static Jp.Wasabeef.RichEditor;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    class FragmentCreateWorkflowDetail : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private LayoutInflater _inflater;
        private LinearLayout _lnActionAll, _lnNoData, _lnData;
        private Dialog _dialogAction, _dialogActionMore, _dialogPopupControl;
        private TextView _tvTitle, _tvNoData, _tvStartDateControlLinkedWorkflow, _tvEndDateControlLinkedWorkflow;
        private View _rootView, _popupViewAddRelated;
        private Dialog _dialogChooseWorkflow, _dialogDetail;
        private List<BeanWorkflowItem> _lstWorkflowItemControlLinkedWorkflow = new List<BeanWorkflowItem>();
        private ImageView _imgBack;
        private ExpandableListView _expandData;
        private List<ViewRow> _lstRowsDetail = new List<ViewRow>();
        private BeanWorkflow _workflow;


        private AdapterDetailCreateWorkflowControl _adapterDetailControl;
        private AdapterControlLinkedWorkflowListPagingNew adapterControlLinkedWorkflowListPagingNew;
        private ViewElement _clickedElement = new ViewElement();
        private ControllerBase CTRLBase = new ControllerBase();
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        private ControllerDetailWorkflow CTRLDetailWorkflow = new ControllerDetailWorkflow();

        public JObject _OBJFORMACTION;
        private List<ViewSection> _LISTSECTION = new List<ViewSection>(); // List lưu Section Dynamic Form
        private ViewRow _LISTACTION = new ViewRow();  // List lưu Dynamic Action
        private List<BeanWorkFlowRelated> _LISTFLOWRELATED = new List<BeanWorkFlowRelated>(); // List lưu Section Dynamic Form
        private List<ViewElement> _lstEditedElement = new List<ViewElement>(); // List lưu những Element có cập nhật để send lên API
        public List<BeanAttachFile> _lstAttFileControl_Deleted = new List<BeanAttachFile>(); // lưu lại những item nào đã bị xóa ra khỏi Control inputattachmenthorizon
        ////private List<BeanQuaTrinhLuanChuyen> _lstQTLC = new List<BeanQuaTrinhLuanChuyen>();
        private ComponentButtonBot _componentButtonBot;  // Coponent của Control Button Bot
        private AdapterDetailExpandControl _adapterDetailExpandControl;
        private ViewElement _clickedElementAttachment; // lưu lại để nếu có gọi OnActivityResult thì update cho nhanh
        private RichEditor richEditor;
        private Android.Net.Uri _mUri;
        private string _previousFragment = "";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override void OnDestroyView()
        {
            base.OnDestroyView();
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            MinionActionCore.ElementFormClickEvent -= Click_ElementFormEvent; // Event Click vào Element Control Form
            MinionActionCore.ElementFormClickEvent_WithInnerAction -= Click_ElementFormEvent_WithInnerAction; // Event Click vào Element Control Form có action bên trong
            MinionActionCore.FlowRelatedClickEvent_WithInnerAction -= Click_FlowRelated_WithInnerAction; // Event Click vào Item của Flow Related
            MinionActionCore.ElementActionClickEvent -= Click_ElementActionEvent; // Event Click vào Element Control Button Bot
            //MinionActionCore.ActivityResultEvent -= Override_OnActivityResult; // Event khi thêm file từ Control Attachment
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _mainAct = (MainActivity)this.Activity;
            _inflater = inflater;
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewCreateWorkflowDetail, null);
                if (_mainAct._drawerLayout != null)
                {
                    _mainAct._drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
                }
                _tvTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewCreateWorkflowDetail_Title);
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewCreateWorkflowDetail_Back);

                _lnActionAll = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewCreateWorkflowDetail_ActionAll);
                _expandData = _rootView.FindViewById<ExpandableListView>(Resource.Id.expand_ViewCreateWorkflowDetail_Data);
                _lnData = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewCreateWorkflowDetail_Data);
                _lnNoData = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewCreateWorkflowDetail_NoData);
                _tvNoData = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewCreateWorkflowDetail_NoData);

                _expandData.SetGroupIndicator(null);
                _expandData.SetChildIndicator(null);
                _expandData.DividerHeight = 0;

                _imgBack.Click += Click_Back;
                SetViewByLanguage();
                GetAndSetDataFromServer();
            }
            MinionActionCore.ElementFormClickEvent += Click_ElementFormEvent; // Event Click vào Element Control Form
            MinionActionCore.ElementFormClickEvent_WithInnerAction += Click_ElementFormEvent_WithInnerAction; // Event Click vào Element Control Form có action bên trong
            MinionActionCore.FlowRelatedClickEvent_WithInnerAction += Click_FlowRelated_WithInnerAction; // Event Click vào Item của Flow Related
            MinionActionCore.ElementActionClickEvent += Click_ElementActionEvent; // Event Click vào Element Control Button Bot
            //MinionActionCore.ActivityResultEvent += Override_OnActivityResult; // Event khi thêm file từ Control Attachment
            return _rootView;
        }
        private void Override_OnActivityResult(object sender, MinionActionCore.ActivityResult e)
        {
            try
            {
                int requestCode = e.requestCode;
                Result resultCode = e.resultCode;
                Intent data = e.data;
                if (requestCode == CmmDroidVariable.M_DetailWorkflow_ChooseFileControlAttachment && resultCode == Result.Ok)
                {
                    Android.Net.Uri _selectedUri = data.Data;
                    if (_selectedUri == null)
                    {
                        return;
                    }


                    string localPath = "";
                    if (_selectedUri.ToString().Contains("primary"))
                    {
                        //orgin_path = AndroidCoreOS.Environment.ExternalStorageDirectory.Path + "/" + uri.Path.Split(':')[1];
                        localPath = _mainAct.GetActualPathFromFile(_selectedUri);
                    }
                    else if (!_selectedUri.ToString().Contains("primary") && _selectedUri.ToString().Contains("externalstorage"))//thẻ nhớ máy
                    {
                        localPath = "/storage/extSdCard/" + _selectedUri.Path.Split(':')[1];
                    }
                    else if (_selectedUri.ToString().ToLower().Contains("fileprovider"))
                    {
                        localPath = _mainAct.GetActualPathFromFile(_selectedUri);
                    }
                    else
                    {
                        localPath = _mainAct.GetActualPathFromFile(_selectedUri);
                    }

                    ParcelFileDescriptor fd = _mainAct.ContentResolver.OpenFileDescriptor(_selectedUri, "r");
                    BeanAttachFile _beanAttachFile = new BeanAttachFile();
                    _beanAttachFile.ID = ""; // File mới ID = ""
                    _beanAttachFile.Title = CmmDroidFunction.GetDisplayNameOfURI(_rootView.Context, _selectedUri) + ";#" + DateTime.Now.ToShortTimeString();
                    _beanAttachFile.Path = localPath; /* _selectedUri.Path; */
                    _beanAttachFile.CreatedBy = CmmVariable.SysConfig.UserId;
                    _beanAttachFile.IsAuthor = true;
                    _beanAttachFile.CreatedName = CmmVariable.SysConfig.DisplayName;
                    _beanAttachFile.CreatedPositon = CmmVariable.SysConfig.PositionTitle;
                    _beanAttachFile.AttachTypeId = null;
                    _beanAttachFile.AttachTypeName = "";
                    ////_beanAttachFile.WorkflowId = _workflowItem.WorkflowID;
                    ////_beanAttachFile.WorkflowItemId = int.Parse(_workflowItem.ID);
                    _beanAttachFile.Size = fd.StatSize; // fd.StatSize là Bytes
                    fd.Close();


                    if (_clickedElementAttachment != null) // Cập nhật lại giá trị cho Control Attachment
                    {
                        List<BeanAttachFile> _lstAttFileControl_Full = JsonConvert.DeserializeObject<List<BeanAttachFile>>(_clickedElementAttachment.Value.Trim());

                        #region Validate
                        if (!String.IsNullOrEmpty(_beanAttachFile.Path) && _lstAttFileControl_Full != null)// Check xem có tồn tại trong list chưa, nếu có rồi -> return
                        {
                            foreach (BeanAttachFile item in _lstAttFileControl_Full)
                            {
                                if (item.Path.Equals(_beanAttachFile.Path))
                                {
                                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("K_exisrta", "File đã tồn tại trong danh sách"),
                                                                               CmmFunction.GetTitle("K_exisrta", "File has been existed in list"));
                                    return;
                                }
                            }
                        }
                        #endregion

                        _lstAttFileControl_Full.Add(_beanAttachFile);
                        UpdateValueForElement(_clickedElementAttachment, JsonConvert.SerializeObject(_lstAttFileControl_Full));
                        _adapterDetailExpandControl.NotifyDataSetChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("BANANA", "Không thể chọn tâp tin này."),
                                                           CmmFunction.GetTitle("BANANA", "Can't choose this file."));

#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Override_OnActivityResult", ex);
#endif
            }
        }
        public FragmentCreateWorkflowDetail(BeanWorkflow _workflow)
        {
            this._workflow = _workflow;
        }
        private void SetViewByLanguage()
        {
            try
            {
                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    _tvTitle.Text = !String.IsNullOrEmpty(_workflow.Title) ? _workflow.Title : "";
                }
                else
                {
                    _tvTitle.Text = !String.IsNullOrEmpty(_workflow.TitleEN) ? _workflow.TitleEN : "";
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentCreateWorkflow", "SetViewByLanguage", ex);
#endif
            }
        }
        private void Click_Back(object sender, EventArgs e)
        {
            try
            {
                _mainAct.HideFragment();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentCreateWorkflowDetail - Click_Back - Error: " + ex.Message);
#endif
            }
        }
        private void Click_GroupExpandDetail(object sender, ExpandableListView.GroupClickEventArgs e)
        {
            try
            {
                if (_LISTSECTION[e.GroupPosition].ShowType == true)
                {
                    _LISTSECTION[e.GroupPosition].ShowType = false;
                }
                else
                {
                    _LISTSECTION[e.GroupPosition].ShowType = true;
                }
                _adapterDetailExpandControl.NotifyDataSetChanged();

                for (int i = 0; i < _adapterDetailExpandControl.GroupCount; i++)
                {
                    if (_LISTSECTION[i].ShowType)
                    {
                        _expandData.ExpandGroup(i);
                    }
                    else
                    {
                        _expandData.CollapseGroup(i);
                    }
                }


            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Click_GroupExpandDetail", ex);
#endif
            }
        }


        #region Data
        private async void GetAndSetDataFromServer()
        {
            try
            {
                CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);

                ////SetEnableView(false);
                await Task.Run(() =>
                {
                    ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                    string _resultString = "";

                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                        _resultString = _pControlDynamic.GetCreateWorkflowControlDynamicForm(_workflow); // List Form control
                    else
                        _resultString = _pControlDynamic.GetCreateWorkflowControlDynamicForm(_workflow, "1033"); // List Form control

                    if (String.IsNullOrEmpty(_resultString))
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            ////_lnData.Visibility = ViewStates.Gone;
                            ////_lnNoData.Visibility = ViewStates.Visible;
                            ////SetEnableView(true);
                            CmmDroidFunction.HideProcessingDialog();
                        });
                        return;
                    }
                    _OBJFORMACTION = JObject.Parse(_resultString);

                    try { _LISTSECTION = JsonConvert.DeserializeObject<List<ViewSection>>(_OBJFORMACTION["form"].ToString()); }
                    catch (Exception) { _LISTSECTION = new List<ViewSection>(); }

                    try { _LISTACTION = JsonConvert.DeserializeObject<ViewRow>(_OBJFORMACTION["action"].ToString()); }
                    catch (Exception) { _LISTACTION = new ViewRow(); }

                    try { _LISTFLOWRELATED = JsonConvert.DeserializeObject<List<BeanWorkFlowRelated>>(_OBJFORMACTION["related"].ToString()); }
                    catch (Exception) { _LISTFLOWRELATED = new List<BeanWorkFlowRelated>(); }

                    _mainAct.RunOnUiThread(() =>
                    {
                        SetData_DynamicControlForm();
                        SetData_DynamiControlAction();
                        ////SetEnableView(true);
                        CmmDroidFunction.HideProcessingDialog();
                    });
                });
            }
            catch (Exception ex)
            {
                _mainAct.RunOnUiThread(() =>
                {
                    ////_lnData.Visibility = ViewStates.Gone;
                    ////_lnNoData.Visibility = ViewStates.Visible;
                    ////SetEnableView(true);
                    CmmDroidFunction.HideProcessingDialog();
                });
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "GetAndSetDataFromServer", ex);
#endif
            }
        }
        private void SetData_DynamicControlForm()
        {
            try
            {
                if (_LISTSECTION != null && _LISTSECTION.Count > 0)
                {
                    for (int i = 0; i < _LISTSECTION.Count; i++)
                        if (_LISTSECTION[i].ViewRows == null || _LISTSECTION[0].ViewRows.Count <= 0)
                            _LISTSECTION.Remove(_LISTSECTION[i]);

                    _adapterDetailExpandControl = new AdapterDetailExpandControl(_mainAct, _rootView.Context, new BeanWorkflowItem(), new BeanNotify(), _LISTSECTION, _LISTFLOWRELATED, new List<BeanTask>(), new List<BeanComment>(), (int)FlagViewControlAttachment.CreateWorkflowDetail, "");
                    _expandData.SetAdapter(_adapterDetailExpandControl);
                    _expandData.GroupClick -= Click_GroupExpandDetail;
                    _expandData.GroupClick += Click_GroupExpandDetail;
                    for (int i = 0; i < _LISTSECTION.Count; i++)
                    {
                        if (_LISTSECTION[i].ShowType)
                            _expandData.ExpandGroup(i);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "SetData_DynamicControlForm", ex);
#endif
            }
        }
        private void SetData_DynamiControlAction()
        {
            try
            {
                ViewElement _elementSave = new ViewElement() { DataType = "buttonbot", IsRequire = false, ID = "11", Title = "Lưu", Value = "Lưu" };
                ViewElement _elementSend = new ViewElement() { DataType = "buttonbot", IsRequire = false, ID = "11", Title = "Gửi", Value = "Gửi" };
                ViewElement _elementAddRelated = new ViewElement() { DataType = "buttonbot", IsRequire = false, ID = "999", Title = "Thêm liên kết", Value = "Thêm liên kết" };

                _LISTACTION.Elements.Add(_elementSave);
                _LISTACTION.Elements.Add(_elementSend);
                _LISTACTION.Elements.Add(_elementAddRelated);


                if (_LISTACTION.Elements != null && _LISTACTION.Elements.Count > 0)
                {
                    _lnActionAll.Visibility = ViewStates.Visible;
                    _componentButtonBot = new ComponentButtonBot(_mainAct, _lnActionAll, _LISTACTION, Resources);
                    _componentButtonBot.InitializeFrameView(_lnActionAll);
                    _componentButtonBot.SetTitle();
                    _componentButtonBot.SetValue();
                    _componentButtonBot.SetEnable();
                    _componentButtonBot.SetProprety();
                }
                else
                {
                    _lnActionAll.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "SetData_DynamiControlAction", ex);
#endif
            }
        }
        #endregion

        #region Custom Control

        #region Dynamic Form
        private void Click_ElementFormEvent(object sender, MinionActionCore.ElementFormClick e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true) // không cho click nhiều lần
                {
                    ViewElement _clickedElement = e.element;

                    switch (_clickedElement.DataType)
                    {
                        case "selectuser":
                            {
                                if (_clickedElement.Enable)
                                    ShowPopup_ControlSelectUser_ControlSelectUserAndGroup(_clickedElement, false);
                                break;
                            }
                        case "selectusergroup":
                            {
                                if (_clickedElement.Enable)
                                    ShowPopup_ControlSelectUser_ControlSelectUserAndGroup(_clickedElement, true);
                                break;
                            }
                        case "selectusermulti":
                            {
                                if (_clickedElement.Enable)
                                    ShowPopup_ControlSelectMultiUser_ControlSelectMultiUserAndGroup(_clickedElement, false);
                                break;
                            }
                        case "selectusergroupmulti":
                            {
                                if (_clickedElement.Enable)
                                    ShowPopup_ControlSelectMultiUser_ControlSelectMultiUserAndGroup(_clickedElement, true);
                                break;
                            }
                        case "date":
                            {
                                if (_clickedElement.Enable)
                                    ShowPopup_ControlDate(_clickedElement);
                                break;
                            }
                        case "datetime":
                            {
                                if (_clickedElement.Enable)
                                    ShowPopup_ControlDateTime(_clickedElement);
                                break;
                            }
                        case "time":
                            break;
                        case "singlechoice":
                            {
                                if (_clickedElement.Enable)
                                    ShowPopup_ControlSingleChoice(_clickedElement);
                                break;
                            }
                        case "singlelookup":
                            {
                                if (_clickedElement.Enable)
                                    ShowPopup_ControlSingleLookup(_clickedElement);
                                break;
                            }
                        case "multiplechoice":
                            {
                                if (_clickedElement.Enable)
                                    ShowPopup_ControlMultiChoice(_clickedElement);
                                break;
                            }
                        case "multiplelookup":
                            {
                                if (_clickedElement.Enable)
                                    ShowPopup_ControlMultiLookup(_clickedElement);
                                break;
                            }
                        case "number":
                            {
                                if (_clickedElement.Enable)
                                    ShowPopup_ControlNumber(_clickedElement);  // Đã bao gồm Enable True và False 
                                break;
                            }
                        case "tabs":
                            {
                                break;
                            }
                        case "attachment":
                            break;
                        case "attachmentvertical":
                            break;
                        case "yesno":
                            {
                                if (_clickedElement.Enable)
                                    Handle_ControlYesNo(_clickedElement);
                                break;
                            }
                        case "tree":
                            break;
                        case "attachmentverticalformframe":
                            break;
                        case "textinput":
                        case "textinputmultiline":
                            {
                                if (_clickedElement.Enable)
                                    ShowPopup_ControlTextInput(_clickedElement);
                                else
                                    ShowPopup_ViewFullInformation(_clickedElement);
                                break;
                            }
                        case "textinputformat": // Text Editor
                            {
                                if (_clickedElement.Enable)
                                    ShowPopup_ControlTextInputFormat(_clickedElement);
                                break;
                            }
                        case "inputattachmenthorizon":
                        case "inputattachmentvertical":
                            {
                                if (_clickedElement.Enable)
                                    ShowPopup_ControlInputAttachmentVertical(_clickedElement);
                                break;
                            }
                        case "inputworkrelated": // quy trình liên kết
                                                 //ControlLinkedWorkflow_ShowPopupChooseWorkflow();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Click_ElementChildExpandDetail", ex);
#endif
            }
        }

        private void Click_ElementFormEvent_WithInnerAction(object sender, MinionActionCore.ElementFormClick_WithInnerAction e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true) // không cho click nhiều lần
                {
                    ViewElement _clickedElement = e.element;
                    int _actionID = e.actionID;
                    int _positonToAction = e.positionToAction;

                    //if (!_clickedElement.Enable)
                    //{
                    //    return;
                    //}
                    switch (_clickedElement.DataType)
                    {
                        case "inputattachmenthorizon":
                        case "inputattachmentvertical":
                            //if (_actionID == (int)EnumFormControlInnerAction.ControlInputAttachmentVertical_InnerActionID.View) // Nếu View file thì không cần check enable
                            //{
                            //    ShowPopup_ControlInputAttachmentVertical_InnerAction(_clickedElement, _actionID, _positonToAction);
                            //}
                            //else if (_clickedElement.Enable == true) // Action khác -> Enable phải = true mới cho thao tác
                            //{
                            //    ShowPopup_ControlInputAttachmentVertical_InnerAction(_clickedElement, _actionID, _positonToAction);
                            //}
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Click_ElementFormEvent_WithInnerAction", ex);
#endif
            }
        }

        private void Click_FlowRelated_WithInnerAction(object sender, MinionActionCore.FlowRelatedClick_WithInnerAction e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true) // không cho click nhiều lần
                {
                    List<BeanWorkFlowRelated> _lstWorkflowRelated = e.lstWorkflowRelated;
                    int _actionID = e.actionID;
                    int _positonToAction = e.positionToAction;

                    switch (_actionID)
                    {
                        case (int)EnumFormControlInnerAction.FlowRelated_InnerActionID.View: // View Full Item -> mở ra trang Detail mới
                            {

                                break;
                            }
                        case (int)EnumFormControlInnerAction.FlowRelated_InnerActionID.Delete: // Xóa file đi
                            {

                                break;
                            }
                    }

                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Click_FlowRelated_WithInnerAction", ex);
#endif
            }
        }

        /// <summary>
        /// Event khi click vào file đính kèm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_ElementChildAttachmentExpandDetail(object sender, MinionActionCore.ElementAttachFileClick e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true) // không cho click nhiều lần
                {
                    ViewElement _clickedElement = e.element;
                    KeyValuePair<string, string> _clickedAttachment = e.attachment;
                    switch (_clickedElement.DataType)
                    {
                        default: break;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - Click_ElementChildAttachmentExpandDetail - Error: " + ex.Message);
#endif
            }
        }

        private void ShowPopup_ControlDate(ViewElement clickedElement)
        {
            try
            {
                //#region Get View - Init Data

                //View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_DatePicker, null);
                //ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_DatePicker_Done);
                //ImageView _imgClose = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_DatePicker_Close);
                //TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_DatePicker_Title);
                //RadCalendarView _calendar = _viewPopupControl.FindViewById<RadCalendarView>(Resource.Id.Calendar_PopupControl_DatePicker);
                //TextView _tvCurrentDate = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_DatePicker_CurrentDate);

                //CTRLBase.InitRadCalendarView(_calendar, _tvCurrentDate);

                //DateTime _initDate;
                //try
                //{
                //    _initDate = DateTime.Parse(clickedElement.Value, new CultureInfo("en", false));
                //}
                //catch (Exception)
                //{
                //    _initDate = DateTime.Now;
                //}

                //if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                //    _tvTitle.Text = _initDate.ToString("dd/MM/yyyy");
                //else
                //    _tvTitle.Text = _initDate.ToString("MM/dd/yyyy");


                //#endregion

                //#region Event
                //_imgClose.Click += (sender, e) =>
                //{
                //    _dialogPopupControl.Dismiss();
                //};
                //_imgDone.Click += delegate
                //{
                //    if (_tvCurrentDate.Text.Contains("/"))
                //    {
                //        DateTime _result = DateTime.Now;
                //        try
                //        {
                //            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                //                _result = DateTime.ParseExact(_tvCurrentDate.Text, "dd/MM/yyyy", null);
                //            else
                //                _result = DateTime.ParseExact(_tvCurrentDate.Text, "MM/dd/yyyy", null);
                //        }
                //        catch (Exception)
                //        {
                //            _result = DateTime.Now;
                //        }

                //        string _resultString = _result.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);

                //        UpdateValueForElement(clickedElement, _resultString);
                //        _dialogPopupControl.Dismiss();
                //    };
                //    #endregion
                //};

                //#region Show View                
                //_dialogPopupControl = new Dialog(_rootView.Context);
                //Window window = _dialogPopupControl.Window;
                //_dialogPopupControl.RequestWindowFeature(1);
                //_dialogPopupControl.SetCanceledOnTouchOutside(false);
                //_dialogPopupControl.SetCancelable(true);
                //window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                //window.SetGravity(GravityFlags.Center);
                //var dm = _mainAct.Resources.DisplayMetrics;

                //_dialogPopupControl.SetContentView(_viewPopupControl);
                //_dialogPopupControl.Show();
                //WindowManagerLayoutParams s = window.Attributes;
                //s.Width = dm.WidthPixels;
                //s.Height = WindowManagerLayoutParams.WrapContent;
                //window.Attributes = s;
                //window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                //#endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "ShowPopup_ControlDate", ex);
#endif
            }
        }

        private void ShowPopup_ControlDateTime(ViewElement clickedElement)
        {
            try
            {
                //#region Get View - Init Data

                //View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_DateTimePicker, null);
                //DatePicker _datePicker = _viewPopupControl.FindViewById<DatePicker>(Resource.Id.dp_PopupControl_DateTimePicker);
                //TimePicker _timePicker = _viewPopupControl.FindViewById<TimePicker>(Resource.Id.tp_PopupControl_DateTimePicker);
                //ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_DateTimePicker_Done);
                //ImageView _imgClose = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_DateTimePicker_Close);
                //TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_DateTimePicker_Title);

                //_timePicker.SetIs24HourView(Java.Lang.Boolean.True);
                //DateTime _initDate = new DateTime();
                //try
                //{
                //    _initDate = DateTime.Parse(clickedElement.Value, new CultureInfo("en", false));
                //    _datePicker.Init(_initDate.Year, _initDate.Month - 1, _initDate.Day, null);
                //    _timePicker.Hour = _initDate.Hour;
                //    _timePicker.Minute = _initDate.Minute;
                //}
                //catch
                //{
                //    _initDate = DateTime.Now;
                //}

                //if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                //    _tvTitle.Text = _initDate.ToString("dd/MM/yyyy");
                //else
                //    _tvTitle.Text = _initDate.ToString("MM/dd/yyyy");

                //#endregion

                //#region Event
                //_imgClose.Click += (sender, e) =>
                //{
                //    _dialogPopupControl.Dismiss();
                //};
                //_imgDone.Click += delegate
                //{
                //    DateTime _resultDate = new DateTime(_datePicker.Year, _datePicker.Month + 1, _datePicker.DayOfMonth, _timePicker.Hour, _timePicker.Minute, 0);
                //    string _resultString = _resultDate.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);

                //    UpdateValueForElement(clickedElement, _resultString);
                //    _adapterDetailExpandControl.UpdateCurrentListSection(_LISTSECTION);
                //    _adapterDetailExpandControl.NotifyDataSetChanged();
                //    _dialogPopupControl.Dismiss();
                //};
                //#endregion

                //#region Show View                
                //_dialogPopupControl = new Dialog(_rootView.Context);
                //Window window = _dialogPopupControl.Window;
                //_dialogPopupControl.RequestWindowFeature(1);
                //_dialogPopupControl.SetCanceledOnTouchOutside(false);
                //_dialogPopupControl.SetCancelable(true);
                //window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                //window.SetGravity(GravityFlags.Center);
                //var dm = _mainAct.Resources.DisplayMetrics;

                //_dialogPopupControl.SetContentView(_viewPopupControl);
                //_dialogPopupControl.Show();
                //WindowManagerLayoutParams s = window.Attributes;
                //s.Width = dm.WidthPixels;
                //s.Height = WindowManagerLayoutParams.WrapContent;
                //window.Attributes = s;
                //window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                //#endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "ShowPopup_ControlDate", ex);
#endif
            }
        }


        private void ShowPopup_ControlSelectUser_Old(ViewElement clickedElement)
        {
            try
            {
                List<BeanUser> _lstUserAll = new List<BeanUser>();
                BeanUser _selectedUser = new BeanUser();

                #region Get View - Init Data
                //View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_ChooseUser, null);
                View _viewPopupControl = null;
                LinearLayout _lnChooseUser = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControl_ChooseUser);
                ImageView _imgCloseChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Close);
                ImageView _imgAcceptChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Accept);
                TextView _tvTitleChooseUser = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_ChooseUser_Title);
                EditText _edtSearchChooseUser = _viewPopupControl.FindViewById<EditText>(Resource.Id.edt_PopupControl_ChooseUser_Search);
                RecyclerView _recyChooseUser = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_ChooseUser);

                if (!String.IsNullOrEmpty(clickedElement.Title))
                    _tvTitleChooseUser.Text = clickedElement.Title;
                else
                    _tvTitleChooseUser.Text = "";

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    _edtSearchChooseUser.Hint = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm ...");
                }
                else
                {
                    _edtSearchChooseUser.Hint = CmmFunction.GetTitle("TEXT_SEARCH", "Search ...");
                }

                SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                string queryNotify = string.Format("SELECT * FROM BeanUser");
                _lstUserAll = conn.Query<BeanUser>(queryNotify);

                string name = clickedElement.Value.Split(";#")[1];
                string queryUser = string.Format("SELECT * FROM BeanUser WHERE FullName = '{0}'", name);
                List<BeanUser> _lstTemp = conn.Query<BeanUser>(queryUser);
                if (_lstTemp != null && _lstTemp.Count > 0)
                {
                    _selectedUser = _lstTemp[0];
                }

                if (_lstUserAll == null) _lstUserAll = new List<BeanUser>();
                if (_selectedUser == null) _selectedUser = new BeanUser();
                AdapterDetailChooseUser _adapterListUser = new AdapterDetailChooseUser(_mainAct, _rootView.Context, _lstUserAll, _selectedUser);
                #endregion

                #region Event
                _edtSearchChooseUser.TextChanged += (sender, e) =>
                {
                    List<BeanUser> _lstSearch = new List<BeanUser>();
                    if (!String.IsNullOrEmpty(_edtSearchChooseUser.Text))
                    {
                        _lstSearch = _lstUserAll.Where(x => CmmFunction.removeSignVietnamese(x.FullName).ToLowerInvariant().Contains(CmmFunction.removeSignVietnamese(_edtSearchChooseUser.Text).ToLowerInvariant())).ToList();
                    }
                    else
                    {
                        _lstSearch = _lstUserAll.ToList();
                    }

                    AdapterDetailChooseUser _adapterListUser = new AdapterDetailChooseUser(_mainAct, _rootView.Context, _lstSearch, _selectedUser);
                    StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);

                    _recyChooseUser.SetAdapter(_adapterListUser);
                    _recyChooseUser.SetLayoutManager(staggeredGridLayoutManager);
                    _adapterListUser.CustomItemClick += (sender, e) =>
                    {
                        _selectedUser = e;
                    };

                };
                _imgCloseChooseUser.Click += (sender, e) =>
                {
                    _dialogPopupControl.Dismiss();
                };
                _imgAcceptChooseUser.Click += delegate
                {
                    //_selectedUser = _adapterListUser.GetUserIsClicked();
                    string _result = "";

                    if (_selectedUser != null)
                    {
                        _result = _selectedUser.ID + ";#" + _selectedUser.FullName;
                    }

                    UpdateValueForElement(clickedElement, _result);
                    _adapterDetailExpandControl.NotifyDataSetChanged();
                    _dialogPopupControl.Dismiss();
                };
                #endregion

                #region Show View                
                _dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
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
                _edtSearchChooseUser.Text = "";
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "ShowPopup_ControlDate", ex);
#endif
            }
        }

        /// <summary>
        /// Popup của 2 control SelectUser - SelectUserAndGroup
        /// </summary>
        /// <param name="clickedElement"></param>
        /// <param name="_isUserAndGroup">check xem là control nào</param>
        private void ShowPopup_ControlSelectUser_ControlSelectUserAndGroup(ViewElement clickedElement, bool _isUserAndGroup)
        {
            try
            {
                List<BeanUserAndGroup> _lstUserAndGroupAll = new List<BeanUserAndGroup>();
                BeanUserAndGroup _selectedUserAndGroup = new BeanUserAndGroup();

                #region Get View - Init Data
                //View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_ChooseUser, null);
                View _viewPopupControl = null;
                LinearLayout _lnChooseUser = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControl_ChooseUser);
                ImageView _imgCloseChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Close);
                ImageView _imgAcceptChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Accept);
                TextView _tvTitleChooseUser = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_ChooseUser_Title);
                EditText _edtSearchChooseUser = _viewPopupControl.FindViewById<EditText>(Resource.Id.edt_PopupControl_ChooseUser_Search);
                RecyclerView _recyChooseUser = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_ChooseUser);

                _imgAcceptChooseUser.Visibility = ViewStates.Invisible;

                if (!String.IsNullOrEmpty(clickedElement.Title))
                    _tvTitleChooseUser.Text = clickedElement.Title;
                else
                    _tvTitleChooseUser.Text = "";

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    _edtSearchChooseUser.Hint = CmmFunction.GetTitle("MESS_REQUIRE_EMAIL", "Vui lòng nhập tên hoặc địa chỉ email");
                }
                else
                {
                    _edtSearchChooseUser.Hint = CmmFunction.GetTitle("MESS_REQUIRE_EMAIL", "Leave a name or email here");
                }

                List<BeanUserAndGroup> _beanUserAndGroup = new List<BeanUserAndGroup>();
                if (!String.IsNullOrEmpty(clickedElement.Value))
                    _beanUserAndGroup = JsonConvert.DeserializeObject<List<BeanUserAndGroup>>(clickedElement.Value);

                if (_beanUserAndGroup != null && _beanUserAndGroup.Count > 0)
                {
                    _selectedUserAndGroup = _beanUserAndGroup[0];
                }

                SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                if (_isUserAndGroup == true)
                {
                    _lstUserAndGroupAll = conn.Query<BeanUserAndGroup>(CTRLDetailWorkflow._queryBeanUserGroup);
                }
                else
                {
                    _lstUserAndGroupAll = conn.Query<BeanUserAndGroup>(CTRLDetailWorkflow._queryBeanUser);
                }

                if (_lstUserAndGroupAll != null && _lstUserAndGroupAll.Count > 0 && _selectedUserAndGroup != null)
                {
                    // Người đã được chọn sẽ không hiển thị vào list
                    _lstUserAndGroupAll = _lstUserAndGroupAll.Where(x => !x.ID.Equals(_selectedUserAndGroup.ID)).ToList();
                }

                if (_lstUserAndGroupAll == null) _lstUserAndGroupAll = new List<BeanUserAndGroup>();
                if (_selectedUserAndGroup == null) _selectedUserAndGroup = new BeanUserAndGroup();
                #endregion

                #region Event
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

                    AdapterSelectUserSingle _adapterListUser = new AdapterSelectUserSingle(_mainAct, _rootView.Context, _lstSearch, _selectedUserAndGroup);
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

                        UpdateValueForElement(clickedElement, _result);
                        _adapterDetailExpandControl.NotifyDataSetChanged();
                        _dialogPopupControl.Dismiss();
                    };

                };
                _imgCloseChooseUser.Click += (sender, e) =>
                {
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

        /// <summary>
        /// Popup của 2 control SelectMultiUser - SelectMultiUserAndGroup
        /// </summary>
        /// <param name="clickedElement"></param>
        /// <param name="_isUserAndGroup">check xem là control nào</param>
        private void ShowPopup_ControlSelectMultiUser_ControlSelectMultiUserAndGroup(ViewElement clickedElement, bool _isUserAndGroup)
        {
            try
            {
                List<BeanUserAndGroup> _lstUserAndGroupAll = new List<BeanUserAndGroup>();
                List<BeanUserAndGroup> _lstSelected = new List<BeanUserAndGroup>();

                #region Get View - Init Data
                //View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_ChooseUser, null);
                View _viewPopupControl = null;
                LinearLayout _lnChooseUser = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControl_ChooseUser);
                ImageView _imgCloseChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Close);
                ImageView _imgAcceptChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Accept);
                TextView _tvTitleChooseUser = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_ChooseUser_Title);
                EditText _edtSearchChooseUser = _viewPopupControl.FindViewById<EditText>(Resource.Id.edt_PopupControl_ChooseUser_Search);
                RecyclerView _recyChooseUser = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_ChooseUser);
                CustomFlexBoxRecyclerView _recySelectedUser = _viewPopupControl.FindViewById<CustomFlexBoxRecyclerView>(Resource.Id.recy_PopupControl_SelectedUser);
                _recySelectedUser.Visibility = ViewStates.Visible;
                _recySelectedUser.SetMaxRowAndRowHeight((int)CmmDroidFunction.ConvertDpToPixel(35, _rootView.Context), 3); // 95 *3
                if (!String.IsNullOrEmpty(clickedElement.Title))
                    _tvTitleChooseUser.Text = clickedElement.Title;
                else
                    _tvTitleChooseUser.Text = "";

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    _edtSearchChooseUser.Hint = CmmFunction.GetTitle("MESS_REQUIRE_EMAIL", "Vui lòng nhập tên hoặc địa chỉ email");
                }
                else
                {
                    _edtSearchChooseUser.Hint = CmmFunction.GetTitle("MESS_REQUIRE_EMAIL", "Leave a name or email here");
                }

                if (!String.IsNullOrEmpty(clickedElement.Value))
                    _lstSelected = JsonConvert.DeserializeObject<List<BeanUserAndGroup>>(clickedElement.Value);

                SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                if (_isUserAndGroup == true)
                {
                    _lstUserAndGroupAll = conn.Query<BeanUserAndGroup>(CTRLDetailWorkflow._queryBeanUserGroup);
                }
                else
                {
                    _lstUserAndGroupAll = conn.Query<BeanUserAndGroup>(CTRLDetailWorkflow._queryBeanUser);
                }

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
                        //_lstSearch = _lstUserAndGroupAll.ToList();
                        _lstSearch = new List<BeanUserAndGroup>();
                    }

                    _adapterListUser = new AdapterSelectUserGroupMultiple(_mainAct, _rootView.Context, _lstSearch, _lstSelected);
                    StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);

                    _recyChooseUser.SetAdapter(_adapterListUser);
                    _recyChooseUser.SetLayoutManager(staggeredGridLayoutManager);
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
                _imgAcceptChooseUser.Click += (sender, e) =>
                {
                    List<BeanUserAndGroup> _lstResult = new List<BeanUserAndGroup>();
                    if (_adapterListUserSelected != null)
                    {
                        _lstResult = _adapterListUserSelected.GetListIsclicked();
                    }
                    if (_lstResult == null) _lstResult = new List<BeanUserAndGroup>();
                    string _result = JsonConvert.SerializeObject(_lstResult);
                    CmmDroidFunction.HideSoftKeyBoard(_edtSearchChooseUser, _mainAct);
                    UpdateValueForElement(clickedElement, _result);
                    _adapterDetailExpandControl.NotifyDataSetChanged();
                    _dialogPopupControl.Dismiss();
                };
                #endregion

                #region Show View                
                _dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
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
        private void ShowPopup_ControlChooseMultiUser(ViewElement clickedElement)
        {
            try
            {
                List<BeanUser> _lstUserAll = new List<BeanUser>();
                List<BeanUser> _lstUserSelected = new List<BeanUser>();

                #region Get View - Init Data
                //View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_ChooseUser, null);
                View _viewPopupControl = null;
                LinearLayout _lnChooseUser = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControl_ChooseUser);
                ImageView _imgCloseChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Close);
                ImageView _imgAcceptChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Accept);
                TextView _tvTitleChooseUser = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_ChooseUser_Title);
                EditText _edtSearchChooseUser = _viewPopupControl.FindViewById<EditText>(Resource.Id.edt_PopupControl_ChooseUser_Search);
                RecyclerView _recyChooseUser = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_ChooseUser);

                if (!String.IsNullOrEmpty(clickedElement.Title))
                    _tvTitleChooseUser.Text = clickedElement.Title;
                else
                    _tvTitleChooseUser.Text = "";

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    _edtSearchChooseUser.Hint = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm");
                }
                else
                {
                    _edtSearchChooseUser.Hint = CmmFunction.GetTitle("TEXT_SEARCH", "Search");
                }

                SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                string queryNotify = string.Format("SELECT * FROM BeanUser");
                _lstUserAll = conn.Query<BeanUser>(queryNotify);

                List<string> _lstName = clickedElement.Value.Split(";#").ToList();
                for (int i = 1; i < _lstName.Count; i = i + 2)
                {
                    string name = _lstName[i];
                    string queryUser = string.Format("SELECT * FROM BeanUser WHERE FullName = '{0}'", name);
                    List<BeanUser> _lstTemp = conn.Query<BeanUser>(queryUser);
                    if (_lstTemp != null && _lstTemp.Count > 0)
                    {
                        _lstUserSelected.AddRange(_lstTemp.ToList());
                    }
                }

                if (_lstUserAll == null) _lstUserAll = new List<BeanUser>();
                if (_lstUserSelected == null) _lstUserSelected = new List<BeanUser>();
                AdapterDetailChooseMultiUser _adapterListUser = new AdapterDetailChooseMultiUser(_mainAct, _rootView.Context, _lstUserSelected, _lstUserSelected);
                #endregion

                #region Event
                _edtSearchChooseUser.TextChanged += (sender, e) =>
                {
                    List<BeanUser> _lstSearch = new List<BeanUser>();
                    if (!String.IsNullOrEmpty(_edtSearchChooseUser.Text))
                    {
                        _lstSearch = _lstUserAll.Where(x => x.FullName.ToLowerInvariant().Contains(_edtSearchChooseUser.Text.ToLowerInvariant())).ToList();
                    }
                    else
                    {
                        _lstSearch = _lstUserAll.ToList();
                    }

                    _adapterListUser = new AdapterDetailChooseMultiUser(_mainAct, _rootView.Context, _lstSearch, _lstUserSelected);
                    StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                    _recyChooseUser.SetAdapter(_adapterListUser);
                    _recyChooseUser.SetLayoutManager(staggeredGridLayoutManager);
                    _adapterListUser.CustomItemClick += (sender, e) =>
                    {
                        //_lstUserSelected = _adapterListUser.GetListIsClicked();
                    };

                };
                _imgCloseChooseUser.Click += (sender, e) =>
                {
                    _dialogPopupControl.Dismiss();
                };
                _imgAcceptChooseUser.Click += delegate
                {
                    _lstUserSelected = _adapterListUser.GetListIsClicked();
                    string _result = "";
                    List<string> _lstResult = new List<string>();
                    if (_lstUserSelected != null && _lstUserSelected.Count > 0)
                    {
                        for (int i = 0; i < _lstUserSelected.Count; i++)
                        {
                            _lstResult.Add(_lstUserSelected[i].ID);
                            _lstResult.Add(_lstUserSelected[i].FullName);
                        }
                        _result = String.Join(";#", _lstResult.ToArray());
                    }
                    UpdateValueForElement(clickedElement, _result);
                    _adapterDetailExpandControl.NotifyDataSetChanged();
                    _dialogPopupControl.Dismiss();
                };
                #endregion

                #region Show View                
                _dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
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
                _edtSearchChooseUser.Text = "";
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "ShowPopup_ControlDate", ex);
#endif
            }
        }

        private void ShowPopup_ControlSingleChoice(ViewElement clickedElement)
        {
            try
            {
                //List<BeanLookupData> _lstLookupData = JsonConvert.DeserializeObject<List<BeanLookupData>>(clickedElement.DataSource);
                //BeanLookupData _selectedLookupItem = _lstLookupData.Where(x => x.Title.Equals(clickedElement.Value)).FirstOrDefault();
                //if (_selectedLookupItem == null)
                //    _selectedLookupItem = new BeanLookupData();

                //#region Get View - Init Data

                //View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_SingleChoice, null);
                //ImageView _imgClose = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Close);
                //TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_SingleChoice_Title);
                //RecyclerView _recyData = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_SingleChoice_Data);
                //ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Done);

                //_imgDone.Visibility = ViewStates.Invisible;

                //if (!String.IsNullOrEmpty(clickedElement.Title))
                //{
                //    _tvTitle.Text = clickedElement.Title;
                //}

                //AdapterFormControlSingleChoice _adapterFormControlSingleChoice = new AdapterFormControlSingleChoice(_mainAct, _rootView.Context, _lstLookupData, _selectedLookupItem);
                //StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                //_recyData.SetAdapter(_adapterFormControlSingleChoice);
                //_recyData.SetLayoutManager(staggeredGridLayoutManager);
                //_adapterFormControlSingleChoice.CustomItemClick += (sender, e) =>
                //{
                //    //_selectedLookupItem = e;
                //    //if (!String.IsNullOrEmpty(_selectedLookupItem.Title))
                //    //{
                //    //    UpdateValueForElement(clickedElement, _selectedLookupItem.Title);
                //    //    _dialogPopupControl.Dismiss();
                //    //}
                //    _selectedLookupItem = e;
                //    if (_selectedLookupItem != null)
                //    {
                //        List<BeanLookupData> _lstResult = new List<BeanLookupData>();
                //        _lstResult.Add(_selectedLookupItem);
                //        UpdateValueForElement(clickedElement, JsonConvert.SerializeObject(_lstResult));
                //        _dialogPopupControl.Dismiss();
                //    }
                //};

                //#endregion

                //#region Event
                //_imgClose.Click += (sender, e) =>
                //{
                //    _dialogPopupControl.Dismiss();
                //};
                //#endregion

                //#region Show View                
                //_dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                //Window window = _dialogPopupControl.Window;
                //_dialogPopupControl.RequestWindowFeature(1);
                //_dialogPopupControl.SetCanceledOnTouchOutside(false);
                //_dialogPopupControl.SetCancelable(true);
                //window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                //window.SetGravity(GravityFlags.Bottom);
                //var dm = _mainAct.Resources.DisplayMetrics;

                //_dialogPopupControl.SetContentView(_viewPopupControl);
                //_dialogPopupControl.Show();
                //WindowManagerLayoutParams s = window.Attributes;
                //s.Width = WindowManagerLayoutParams.MatchParent;
                //s.Height = WindowManagerLayoutParams.MatchParent;
                //window.Attributes = s;
                //window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                //#endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "ShowPopup_ControlDate", ex);
#endif
            }
        }

        private void ShowPopup_ControlMultiChoice(ViewElement clickedElement)
        {
            try
            {
                List<BeanLookupData> _lstLookupData = JsonConvert.DeserializeObject<List<BeanLookupData>>(clickedElement.DataSource);
                List<BeanLookupData> _lstselected = _lstLookupData.Where(x => x.Title.Equals(clickedElement.Value)).ToList();
                if (_lstLookupData != null && _lstLookupData.Count > 0)
                {
                    for (int i = 0; i < _lstLookupData.Count; i++)
                    {
                        if (clickedElement.Value.Contains(_lstLookupData[i].Title))
                            _lstselected.Add(_lstLookupData[i]);
                    }
                }

                if (_lstselected == null)
                    _lstselected = new List<BeanLookupData>();

                #region Get View - Init Data

                //View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_SingleChoice, null);
                //ImageView _imgClose = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Close);
                //TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_SingleChoice_Title);
                //RecyclerView _recyData = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_SingleChoice_Data);
                //ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Done);
                //if (!String.IsNullOrEmpty(clickedElement.Title))
                //{
                //    _tvTitle.Text = clickedElement.Title;
                //}

                ////AdapterFormControlMultiChoice _adapterFormControlMultiChoice = new AdapterFormControlMultiChoice(_mainAct, _rootView.Context, _lstLookupData, _lstselected);
                ////StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                ////_recyData.SetAdapter(_adapterFormControlMultiChoice);
                ////_recyData.SetLayoutManager(staggeredGridLayoutManager);
                ////_adapterFormControlMultiChoice.CustomItemClick += (sender, e) =>
                ////{
                ////    _lstselected = _adapterFormControlMultiChoice.GetListIsClicked();
                ////};
                //#endregion

                //#region Event
                //_imgClose.Click += (sender, e) =>
                //{
                //    _dialogPopupControl.Dismiss();
                //};
                //_imgDone.Click += (sender, e) =>
                //{
                //    string _result = "";
                //    if (_lstselected != null && _lstselected.Count > 0)
                //    {
                //        ////for (int i = 0; i < _lstselected.Count; i++)
                //        ////{
                //        ////    _result += ";#" + _lstselected[i].ID + ";#" + _lstselected[i].Title;
                //        ////}
                //        _result = JsonConvert.SerializeObject(_lstselected);
                //    }
                //    UpdateValueForElement(clickedElement, _result);
                //    _dialogPopupControl.Dismiss();
                //};

                //#endregion

                //#region Show View                
                //_dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                //Window window = _dialogPopupControl.Window;
                //_dialogPopupControl.RequestWindowFeature(1);
                //_dialogPopupControl.SetCanceledOnTouchOutside(false);
                //_dialogPopupControl.SetCancelable(true);
                //window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                //window.SetGravity(GravityFlags.Bottom);
                //var dm = _mainAct.Resources.DisplayMetrics;

                //_dialogPopupControl.SetContentView(_viewPopupControl);
                //_dialogPopupControl.Show();
                //WindowManagerLayoutParams s = window.Attributes;
                //s.Width = WindowManagerLayoutParams.MatchParent;
                //s.Height = WindowManagerLayoutParams.MatchParent;
                //window.Attributes = s;
                //window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "ShowPopup_ControlDate", ex);
#endif
            }
        }
        private void ShowPopup_ControlSingleLookup(ViewElement clickedElement)
        {
            try
            {
                //List<BeanLookupData> _lstLookupData = JsonConvert.DeserializeObject<List<BeanLookupData>>(clickedElement.DataSource);
                //BeanLookupData _selectedLookupItem = _lstLookupData.Where(x => x.Title.Equals(clickedElement.Value)).FirstOrDefault();
                //if (_selectedLookupItem == null)
                //    _selectedLookupItem = new BeanLookupData();

                //#region Get View - Init Data

                //View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_SingleChoice, null);
                //ImageView _imgClose = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Close);
                //TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_SingleChoice_Title);
                //RecyclerView _recyData = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_SingleChoice_Data);
                //ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Done);
                //LinearLayout _lnSearch = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControl_SingleChoice_Search);
                //EditText _edtSearch = _viewPopupControl.FindViewById<EditText>(Resource.Id.edt_PopupControl_SingleChoice_Search);

                //_imgDone.Visibility = ViewStates.Invisible;
                //_lnSearch.Visibility = ViewStates.Visible;
                //if (!String.IsNullOrEmpty(clickedElement.Title))
                //{
                //    _tvTitle.Text = clickedElement.Title;
                //}

                //#endregion

                //#region Event
                //_imgClose.Click += (sender, e) =>
                //{
                //    _dialogPopupControl.Dismiss();
                //};
                //_edtSearch.TextChanged += (sender, e) =>
                //{
                //    //List<BeanLookupData> _lstSearch = new List<BeanLookupData>();
                //    //if (!String.IsNullOrEmpty(_edtSearch.Text))
                //    //{
                //    //    _lstSearch = _lstLookupData.Where(x => x.Title.ToLowerInvariant().Contains(_edtSearch.Text.ToLowerInvariant())).ToList();
                //    //}
                //    //else // Full
                //    //{
                //    //    _lstSearch = _lstLookupData.ToList();
                //    //}
                //    //AdapterFormControlSingleChoice _adapterFormControlSingleChoice = new AdapterFormControlSingleChoice(_mainAct, _rootView.Context, _lstSearch, _selectedLookupItem);
                //    //StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                //    //_recyData.SetAdapter(_adapterFormControlSingleChoice);
                //    //_recyData.SetLayoutManager(staggeredGridLayoutManager);
                //    //_adapterFormControlSingleChoice.CustomItemClick += (sender, e) =>
                //    //{
                //    //    _selectedLookupItem = e;
                //    //    if (!String.IsNullOrEmpty(_selectedLookupItem.Title))
                //    //    {
                //    //        UpdateValueForElement(clickedElement, _selectedLookupItem.Title);
                //    //        _dialogPopupControl.Dismiss();
                //    //    }
                //    //};
                //};
                //#endregion

                //#region Show View                
                //_dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                //Window window = _dialogPopupControl.Window;
                //_dialogPopupControl.RequestWindowFeature(1);
                //_dialogPopupControl.SetCanceledOnTouchOutside(false);
                //_dialogPopupControl.SetCancelable(true);
                //window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                //window.SetGravity(GravityFlags.Bottom);
                //var dm = _mainAct.Resources.DisplayMetrics;

                //_dialogPopupControl.SetContentView(_viewPopupControl);
                //_dialogPopupControl.Show();
                //WindowManagerLayoutParams s = window.Attributes;
                //s.Width = WindowManagerLayoutParams.MatchParent;
                //s.Height = WindowManagerLayoutParams.MatchParent;
                //window.Attributes = s;
                //window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                //#endregion

                //_edtSearch.Text = "";
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "ShowPopup_ControlSingleLookup", ex);
#endif
            }
        }
        private void ShowPopup_ControlMultiLookup(ViewElement clickedElement)
        {
            try
            {
                // chuyển qua xài share view

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "ShowPopup_ControlMultiLookup", ex);
#endif
            }
        }
        private void ShowPopup_ControlTextInput(ViewElement clickedElement)
        {
            try
            {
                //#region Get View - Init Data
                //View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_InputText, null);
                //ImageView _imgBack = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputText_Close);
                //TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_InputText_Title);
                //ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputText_Done);
                //EditText _edtContent = _viewPopupControl.FindViewById<EditText>(Resource.Id.edt_PopupControl_InputText);
                //ImageView _imgClearText = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputText_ClearText);

                ////_edtContent.InputType = InputTypes.ClassNumber;
                //_tvTitle.Text = clickedElement.Title;

                //if (!String.IsNullOrEmpty(clickedElement.Value))
                //    _edtContent.Text = CmmDroidFunction.FormatHTMLToString(clickedElement.Value);
                //else
                //    _edtContent.Text = "";
                //#endregion

                //#region Event
                //_edtContent.TextChanged += (sender, e) =>
                //{
                //    //CultureInfo _culVN = CultureInfo.GetCultureInfo("vi-VN");
                //    //double _customValue;
                //    //if (double.TryParse(_edtContent.Text, out _customValue))
                //    //{
                //    //    _edtContent.Text = _customValue.ToString("N0", _culVN);
                //    //}
                //};
                //_imgBack.Click += (sender, e) =>
                //{
                //    CmmDroidFunction.HideSoftKeyBoard(_edtContent, _mainAct);
                //    _dialogPopupControl.Dismiss();
                //};
                //_imgClearText.Click += (sender, e) =>
                //{
                //    _edtContent.Text = "";
                //};

                //_imgDone.Click += delegate
                //{
                //    string _result = "";
                //    if (!String.IsNullOrEmpty(_edtContent.Text))
                //    {
                //        _result = _edtContent.Text;
                //    }
                //    CmmDroidFunction.HideSoftKeyBoard(_edtContent, _mainAct);
                //    UpdateValueForElement(clickedElement, _result);
                //    _adapterDetailExpandControl.NotifyDataSetChanged();
                //    _dialogPopupControl.Dismiss();
                //};
                //#endregion

                //#region Show View                
                //_dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                //Window window = _dialogPopupControl.Window;
                //_dialogPopupControl.RequestWindowFeature(1);
                //_dialogPopupControl.SetCanceledOnTouchOutside(false);
                //_dialogPopupControl.SetCancelable(true);
                //window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                //window.SetGravity(GravityFlags.Bottom);
                //var dm = _mainAct.Resources.DisplayMetrics;

                //_dialogPopupControl.SetContentView(_viewPopupControl);
                //_dialogPopupControl.Show();
                //WindowManagerLayoutParams s = window.Attributes;
                //s.Width = WindowManagerLayoutParams.MatchParent;
                //s.Height = WindowManagerLayoutParams.MatchParent;
                //window.Attributes = s;
                //#endregion
                //_edtContent.RequestFocus();
                //CmmDroidFunction.ShowSoftKeyBoard(_rootView, _mainAct);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "ShowPopup_ControlTextInput", ex);
#endif
            }
        }
        private void ShowPopup_ControlTextInputFormat(ViewElement clickedElement)
        {
            try
            {
                //bool isChanged = true;
                //#region Get View - Init Data
                //View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_InputTextFormat, null);
                //ImageView _imgBack = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_Close);
                //TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_InputTextFormat_Title);
                //ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_Done);
                //EditText _edtContent = _viewPopupControl.FindViewById<EditText>(Resource.Id.edt_PopupControl_InputTextFormat);
                //RichEditor mEditor = _viewPopupControl.FindViewById<RichEditor>(Resource.Id.editor_PopupControl_InputTextFormat);

                //if (!String.IsNullOrEmpty(clickedElement.Title))
                //{
                //    _tvTitle.Text = clickedElement.Title;
                //}
                //else
                //{
                //    _tvTitle.Text = "";
                //}

                //if (!String.IsNullOrEmpty(clickedElement.Value))
                //{
                //    mEditor.SetHtml(clickedElement.Value);
                //}
                //#endregion

                //#region Event
                //_imgBack.Click += (sender, e) =>
                //{
                //    CmmDroidFunction.HideSoftKeyBoard(_edtContent, _mainAct);
                //    _dialogPopupControl.Dismiss();
                //};
                //_imgDone.Click += delegate
                //{
                //    string _result = "";
                //    if (!String.IsNullOrEmpty(mEditor.GetHtml().ToString()))
                //    {
                //        _result = mEditor.GetHtml().ToString();
                //    }

                //    CmmDroidFunction.HideSoftKeyBoard(_edtContent, _mainAct);
                //    UpdateValueForElement(clickedElement, _result);
                //    _adapterDetailExpandControl.NotifyDataSetChanged();
                //    _dialogPopupControl.Dismiss();
                //};

                //#region Editor Library
                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_undo).Click += delegate { mEditor.Undo(); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_redo).Click += delegate { mEditor.Redo(); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_bold).Click += delegate { mEditor.SetBold(); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_italic).Click += delegate { mEditor.SetItalic(); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_subscript).Click += delegate { mEditor.SetSubscript(); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_superscript).Click += delegate { mEditor.SetSuperscript(); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_strikethrough).Click += delegate { mEditor.SetStrikeThrough(); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_underline).Click += delegate { mEditor.SetUnderline(); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading1).Click += delegate { mEditor.SetHeading(1); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading2).Click += delegate { mEditor.SetHeading(2); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading3).Click += delegate { mEditor.SetHeading(3); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading4).Click += delegate { mEditor.SetHeading(4); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading5).Click += delegate { mEditor.SetHeading(5); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading6).Click += delegate { mEditor.SetHeading(6); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_txt_color).Click += delegate { mEditor.SetTextColor(isChanged ? Color.Black : Color.Red); isChanged = !isChanged; };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_bg_color).Click += delegate { mEditor.SetTextBackgroundColor(isChanged ? Color.Transparent : Color.Yellow); isChanged = !isChanged; };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_indent).Click += delegate { mEditor.SetIndent(); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_outdent).Click += delegate { mEditor.SetOutdent(); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_align_left).Click += delegate { mEditor.SetAlignLeft(); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_align_center).Click += delegate { mEditor.SetAlignCenter(); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_align_right).Click += delegate { mEditor.SetAlignRight(); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_blockquote).Click += delegate { mEditor.SetBlockquote(); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_bullets).Click += delegate { mEditor.SetBullets(); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_bullets).Click += delegate { mEditor.SetBullets(); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_numbers).Click += delegate { mEditor.SetNumbers(); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_image).Click += delegate { mEditor.InsertImage("http://www.1honeywan.com/dachshund/image/7.21/7.21_3_thumb.JPG", "dachshund"); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_link).Click += delegate { mEditor.InsertLink("https://github.com/wasabeef", "wasabeef"); };

                //_viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_checkbox).Click += delegate { mEditor.InsertTodo(); };
                //#endregion

                //#endregion

                //#region Show View                
                //_dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                //Window window = _dialogPopupControl.Window;
                //_dialogPopupControl.RequestWindowFeature(1);
                //_dialogPopupControl.SetCanceledOnTouchOutside(false);
                //_dialogPopupControl.SetCancelable(true);
                //window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                //window.SetGravity(GravityFlags.Bottom);
                //var dm = _mainAct.Resources.DisplayMetrics;

                //_dialogPopupControl.SetContentView(_viewPopupControl);
                //_dialogPopupControl.Show();
                //WindowManagerLayoutParams s = window.Attributes;
                //s.Width = WindowManagerLayoutParams.MatchParent;
                //s.Height = WindowManagerLayoutParams.MatchParent;
                //window.Attributes = s;
                //#endregion
                ////_edtContent.RequestFocus();
                //CmmDroidFunction.ShowSoftKeyBoard(_rootView, _mainAct);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentCreateWorkflow - ControlTextEditor_ShowPopupEditor - Error: " + ex.Message);
#endif
            }
        }
        private void ShowPopup_ControlInputAttachmentVertical(ViewElement clickedElement)
        {
            try
            {
                //_clickedElementAttachment = clickedElement;
                //#region Get View - Init Data
                //DisplayMetrics _displayMetrics = Resources.DisplayMetrics;
                //View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControlAttachmentChooseFile, null);
                //ImageView _imgBack = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControlAttachmentChooseFile_Back);
                //TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControlAttachmentChooseFile_title);
                //TextView _tvInApp = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControlAttachmentChooseFile_InApp);
                //RecyclerView _recyInApp = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControlAttachmentChooseFile_InApp);
                //ListView _lvInApp = _viewPopupControl.FindViewById<ListView>(Resource.Id.lv_PopupControlAttachmentChooseFile_InApp);
                //TextView _tvOther = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControlAttachmentChooseFile_Other);
                //LinearLayout _lnOtherCloud = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControlAttachmentChooseFile_Other_Cloud);
                //LinearLayout _lnOtherLibrary = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControlAttachmentChooseFile_Other_Library);
                //TextView _tvOtherCloud = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControlAttachmentChooseFile_Other_Cloud);
                //TextView _tvOtherLibrary = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControlAttachmentChooseFile_Other_Library);

                //if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                //{
                //    _tvTitle.Text = "Đính kèm tài liệu";
                //    _tvInApp.Text = "Tập tin trong ứng dụng";
                //    _tvOther.Text = "Từ nguồn khác";
                //    _tvOtherCloud.Text = "Google Drive và Thiết bị";
                //    _tvOtherLibrary.Text = "Chọn ảnh từ thư viện";
                //}
                //else
                //{
                //    _tvTitle.Text = "Attach a file";
                //    _tvInApp.Text = "File in app";
                //    _tvOther.Text = "Other Resource";
                //    _tvOtherCloud.Text = "Google Drive And Device";
                //    _tvOtherLibrary.Text = "Choose photo from library";
                //}

                //#endregion




                //List<BeanAttachFileControl> _lstInApp = new List<BeanAttachFileControl>();

                ////ProviderBase pBase = new ProviderBase();
                ////string _queryVTBD = "SELECT * FROM BeanAttachFile";
                ////_lstInApp = pBase.LoadMoreDataT<BeanAttachFileControl>(_queryVTBD, 100, 100, _lstInApp.Count);

                ////if (_lstInApp != null && _lstInApp.Count > 0)
                ////{
                ////    StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                ////    AdapterSharedControlAttachmentImport _adapter = new AdapterSharedControlAttachmentImport(_mainAct, _rootView.Context, _lstInApp);
                ////    _adapter.CustomItemClick += (sender, e) =>
                ////    {
                ////        BeanAttachFileControl _clickedItem = e;
                ////        if (_clickedElement != null) // Update lại datasource của Clicked Element
                ////        {
                ////            Tuple<int, int> _index = CmmDroidFunction.Find_RowIndex_ElementIndex_ListControl(_clickedElement, _lstRowsDetail);

                ////            if (_index.Item1 != -1 && _index.Item2 != -1)
                ////            {
                ////                List<BeanAttachFileControl> _currentListAtt = JsonConvert.DeserializeObject<List<BeanAttachFileControl>>(_lstRowsDetail[_index.Item1].Elements[_index.Item2].Value);
                ////                if (_currentListAtt == null)
                ////                {
                ////                    _currentListAtt = new List<BeanAttachFileControl>();
                ////                }
                ////                _currentListAtt.Add(_clickedItem);

                ////                _lstRowsDetail[_index.Item1].Elements[_index.Item2].Value = JsonConvert.SerializeObject(_currentListAtt);
                ////            }
                ////        }
                ////        _adapterDetailControl.NotifyDataSetChanged();
                ////        _dialogAction.Dismiss();
                ////    };
                ////    _recyInApp.SetLayoutManager(staggeredGridLayoutManager);
                ////    _recyInApp.SetAdapter(_adapter);
                ////}

                //_imgBack.Click += (sender, e) =>
                //{
                //    _dialogAction.Dismiss();
                //};
                //_tvOtherCloud.Click += (sender, e) =>
                //{
                //    Intent i = new Intent(Intent.ActionGetContent);
                //    if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                //    {
                //        i.SetType("*/*");
                //    }
                //    else
                //    {
                //        i.SetType("file/*");
                //    }
                //    i.PutExtra(Intent.ExtraMimeTypes, CmmDroidVariable.M_MimeTypes);
                //    i.AddCategory(Intent.CategoryOpenable);
                //    i.PutExtra(Intent.ExtraLocalOnly, true);
                //    _mainAct.StartActivityForResult(i, CmmDroidVariable.M_DetailWorkflow_ChooseFileControlAttachment);

                //    _dialogAction.Dismiss();

                //    //Intent i = new Intent(Intent.ActionGetContent);
                //    //i.SetType("*/*");
                //    //i.PutExtra(Intent.ExtraMimeTypes, mimeTypes);
                //    //i.AddCategory(Intent.CategoryOpenable);
                //    //i.PutExtra(Intent.ExtraLocalOnly, true);
                //    //_mainAct.StartActivityForResult(Intent.CreateChooser(i, "Open A Local File"), CmmDroidVariable.M_DetailWorkflow_ChooseFileControlAttachment);
                //};
                //_tvOtherLibrary.Click += (sender, e) =>
                //{
                //    Intent i = new Intent(Intent.ActionGetContent);
                //    if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                //    {
                //        i.SetType("*/*");
                //    }
                //    else
                //    {
                //        i.SetType("file/*");
                //    }
                //    i.PutExtra(Intent.ExtraMimeTypes, CmmDroidVariable.M_MimeTypes);
                //    i.AddCategory(Intent.CategoryOpenable);
                //    i.PutExtra(Intent.ExtraLocalOnly, true);
                //    _mainAct.StartActivityForResult(i, CmmDroidVariable.M_DetailWorkflow_ChooseFileControlAttachment);

                //    _dialogAction.Dismiss();
                //};

                //#region Show View
                //_dialogAction = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen_Animation);
                //Window window = _dialogAction.Window;
                //var dm = Resources.DisplayMetrics;
                //window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                //window.SetGravity(GravityFlags.Center);

                //_dialogAction.RequestWindowFeature(1);
                //_dialogAction.SetCanceledOnTouchOutside(false);
                //_dialogAction.SetCancelable(true);
                //_dialogAction.SetContentView(_viewPopupControl);
                //_dialogAction.Show();

                //WindowManagerLayoutParams s = window.Attributes;
                //s.Width = dm.WidthPixels /** 3 / 4*/;
                //s.Height = dm.HeightPixels /** 9 / 10*/;
                //window.Attributes = s;
                //#endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "ShowPopup_ControlInputAttachmentVertical", ex);
#endif
            }
        }
        private void ShowPopup_ControlInputAttachmentVertical_InnerAction(ViewElement clickedElement, int actionID, int positionToAction)
        {
            try
            {
                var data = clickedElement.Value.Trim();
                List<BeanAttachFile> _lstAttFileControl_Full = JsonConvert.DeserializeObject<List<BeanAttachFile>>(data);
                if (_lstAttFileControl_Full == null || _lstAttFileControl_Full.Count == 0)
                {
                    _lstAttFileControl_Full = new List<BeanAttachFile>();
                }

                switch (actionID)
                {
                    case (int)(EnumFormControlInnerAction.ControlInputAttachmentVertical_InnerActionID.Delete): // Xóa item trong RecyclerView
                        {
                            //Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(_mainAct);
                            //alert.SetTitle(CmmDroidFunction.GetApplicationName(_rootView.Context));
                            //if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                            //{
                            //    alert.SetMessage("Xác nhận xóa File");
                            //    alert.SetNegativeButton(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"), (senderAlert, args) => { alert.Dispose(); });
                            //    alert.SetPositiveButton(CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"), (senderAlert, args) =>
                            //    {
                            //        if (!_lstAttFileControl_Full[positionToAction].ID.Equals("")) // file mới thêm local thì không cần add vào list Remove
                            //        {
                            //            _lstAttFileControl_Deleted.Add(_lstAttFileControl_Full[positionToAction]);
                            //        }
                            //        _lstAttFileControl_Full.RemoveAt(positionToAction);
                            //        UpdateValueForElement(clickedElement, JsonConvert.SerializeObject(_lstAttFileControl_Full));
                            //    });
                            //}
                            //else
                            //{
                            //    alert.SetMessage("Confirm delete file");
                            //    alert.SetNegativeButton(CmmFunction.GetTitle("TEXT_CANCEL", "Cancel"), (senderAlert, args) => { alert.Dispose(); });
                            //    alert.SetPositiveButton(CmmFunction.GetTitle("TEXT_AGREE", "Agree"), (senderAlert, args) =>
                            //    {
                            //        if (!_lstAttFileControl_Full[positionToAction].ID.Equals("")) // file mới thêm local thì không cần add vào list Remove
                            //        {
                            //            _lstAttFileControl_Deleted.Add(_lstAttFileControl_Full[positionToAction]);
                            //        }
                            //        _lstAttFileControl_Full.RemoveAt(positionToAction);
                            //        UpdateValueForElement(clickedElement, JsonConvert.SerializeObject(_lstAttFileControl_Full));
                            //        alert.Dispose();
                            //    });
                            //}
                            //Dialog dialog = alert.Create();
                            //dialog.SetCanceledOnTouchOutside(false);
                            //dialog.SetCancelable(false);
                            //dialog.Show();
                            break;
                        }
                    case (int)(EnumFormControlInnerAction.ControlInputAttachmentVertical_InnerActionID.Edit): // Edit item trong RecyclerView
                        {
                            //List<BeanAttachFileCategory> _lstCategory = JsonConvert.DeserializeObject<List<BeanAttachFileCategory>>(clickedElement.DataSource);

                            //string _selectedCategory = "";
                            //if (!String.IsNullOrEmpty(_lstAttFileControl_Full[positionToAction].AttachTypeName))
                            //{
                            //    //_selectedCategory = _lstAttFileControl_Full[positionToAction].Category.Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                            //    _selectedCategory = _lstAttFileControl_Full[positionToAction].AttachTypeName;
                            //}

                            //if (_lstCategory != null && _lstCategory.Count > 0 && !String.IsNullOrEmpty(_selectedCategory))
                            //{
                            //    for (int i = 0; i < _lstCategory.Count; i++)
                            //    {
                            //        if (_lstCategory[i].Title.Contains(_selectedCategory))
                            //            _lstCategory[i].IsSelected = true;
                            //    }
                            //}
                            //else
                            //{
                            //    _lstCategory = new List<BeanAttachFileCategory>();
                            //}

                            //#region Get View - Init Data

                            //View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_SingleChoice, null);
                            //ImageView _imgClose = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Close);
                            //TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_SingleChoice_Title);
                            //RecyclerView _recyData = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_SingleChoice_Data);
                            //ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Done);

                            //_imgDone.Visibility = ViewStates.Invisible;

                            //if (!String.IsNullOrEmpty(clickedElement.Title))
                            //{
                            //    _tvTitle.Text = clickedElement.Title;
                            //}

                            //AdapterFormControlInputAttachmentHorizontal _adapterFormControlSingleChoice = new AdapterFormControlInputAttachmentHorizontal(_mainAct, _rootView.Context, _lstCategory);
                            //StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                            //_recyData.SetAdapter(_adapterFormControlSingleChoice);
                            //_recyData.SetLayoutManager(staggeredGridLayoutManager);
                            //_adapterFormControlSingleChoice.CustomItemClick += (sender, e) =>
                            //{
                            //    if (!String.IsNullOrEmpty(e.Title))
                            //    {
                            //        //// _lstAttFileControl_Full[positionToAction].Category = e.DocumentTypeID + ";#" + e.DocumentTypeValue;
                            //        _lstAttFileControl_Full[positionToAction].AttachTypeId = e.ID;
                            //        _lstAttFileControl_Full[positionToAction].AttachTypeName = e.Title;
                            //        UpdateValueForElement(clickedElement, JsonConvert.SerializeObject(_lstAttFileControl_Full));
                            //        _dialogPopupControl.Dismiss();
                            //        _adapterDetailExpandControl.NotifyDataSetChanged();
                            //    }
                            //};
                            //#endregion

                            //#region Event
                            //_imgClose.Click += (sender, e) =>
                            //{
                            //    _dialogPopupControl.Dismiss();
                            //};
                            //#endregion

                            //#region Show View                
                            //_dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                            //Window window = _dialogPopupControl.Window;
                            //_dialogPopupControl.RequestWindowFeature(1);
                            //_dialogPopupControl.SetCanceledOnTouchOutside(false);
                            //_dialogPopupControl.SetCancelable(true);
                            //window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                            //window.SetGravity(GravityFlags.Bottom);
                            //var dm = _mainAct.Resources.DisplayMetrics;

                            //_dialogPopupControl.SetContentView(_viewPopupControl);
                            //_dialogPopupControl.Show();
                            //WindowManagerLayoutParams s = window.Attributes;
                            //s.Width = WindowManagerLayoutParams.MatchParent;
                            //s.Height = WindowManagerLayoutParams.MatchParent;
                            //window.Attributes = s;
                            //window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                            //#endregion

                            break;
                        }
                    case (int)(EnumFormControlInnerAction.ControlInputAttachmentVertical_InnerActionID.View): // View item trong RecyclerView
                        {
                            if (!_lstAttFileControl_Full[positionToAction].ID.Equals("")) // mở file từ server
                            {
                                CmmDroidFunction.DownloadAndOpenFile(_mainAct, _rootView.Context, CmmVariable.M_Domain + _lstAttFileControl_Full[positionToAction].Path);
                            }
                            else // mở file từ local
                            {
                                if (System.IO.File.Exists(_lstAttFileControl_Full[positionToAction].Path))
                                {
                                    CmmDroidFunction.OpenFile(_mainAct, _rootView.Context, _lstAttFileControl_Full[positionToAction].Path);
                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "ShowPopup_ControlInputAttachmentVertical_InnerAction", ex);
#endif
            }
        }
        private void ShowPopup_ControlNumber(ViewElement clickedElement)
        {
            try
            {
                //#region Get View - Init Data
                //View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_Number, null);
                //ImageView _imgBack = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_Number_Close);
                //TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_Number_Title);
                //ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_Number_Done);
                //EditText _edtContent = _viewPopupControl.FindViewById<EditText>(Resource.Id.edt_PopupControl_Number);
                //ImageView _imgClearText = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_Number_ClearText);

                //_edtContent.InputType = InputTypes.ClassNumber; // Only Number
                //_tvTitle.Text = clickedElement.Title;


                //#endregion

                //#region Event
                //_edtContent.TextChanged += (sender, e) =>
                //{

                //};
                //_edtContent.AfterTextChanged += (sender, e) =>
                //{
                //    double _customValue;
                //    CultureInfo _culVN = CultureInfo.GetCultureInfo("vi-VN");
                //    string _currentTextValue = _edtContent.Text.Trim()/*.Replace(".", "")*/;

                //    if (_currentTextValue.Contains("E+"))
                //    {
                //        if (double.TryParse(_currentTextValue, out _customValue))
                //        {
                //            if (!_customValue.ToString("N0", _culVN).Equals(_edtContent.Text))
                //            {
                //                _edtContent.Text = _customValue.ToString("N0", _culVN);
                //            }
                //        }
                //    }
                //    else
                //    {
                //        if (double.TryParse(_currentTextValue.Replace(".", ""), out _customValue))
                //        {
                //            if (!_customValue.ToString("N0", _culVN).Equals(_edtContent.Text))
                //            {
                //                _edtContent.Text = _customValue.ToString("N0", _culVN);
                //            }
                //        }
                //    }
                //    _edtContent.SetSelection(_edtContent.Text.Length); // focus vào character cuối cùng
                //};
                //_imgBack.Click += (sender, e) =>
                //{
                //    CmmDroidFunction.HideSoftKeyBoard(_edtContent, _mainAct);
                //    _dialogPopupControl.Dismiss();
                //};
                //_imgClearText.Click += (sender, e) =>
                //{
                //    _edtContent.Text = "";
                //};

                //_imgDone.Click += delegate
                //{
                //    string _result = "";
                //    if (!String.IsNullOrEmpty(_edtContent.Text))
                //    {
                //        _result = _edtContent.Text.Trim().Replace(".", "");
                //        if (_result.Contains("."))
                //            _result = _result.Replace(".", "");
                //    }
                //    CmmDroidFunction.HideSoftKeyBoard(_edtContent, _mainAct);
                //    UpdateValueForElement(clickedElement, _result);
                //    _adapterDetailExpandControl.NotifyDataSetChanged();
                //    _dialogPopupControl.Dismiss();
                //};
                //#endregion

                //#region Show View                
                //_dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                //Window window = _dialogPopupControl.Window;
                //_dialogPopupControl.RequestWindowFeature(1);
                //_dialogPopupControl.SetCanceledOnTouchOutside(false);
                //_dialogPopupControl.SetCancelable(true);
                //window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                //window.SetGravity(GravityFlags.Bottom);
                //var dm = _mainAct.Resources.DisplayMetrics;

                //_dialogPopupControl.SetContentView(_viewPopupControl);
                //_dialogPopupControl.Show();
                //WindowManagerLayoutParams s = window.Attributes;
                //s.Width = WindowManagerLayoutParams.MatchParent;
                //s.Height = WindowManagerLayoutParams.MatchParent;
                //window.Attributes = s;
                //#endregion

                //_edtContent.Text = clickedElement.Value;
                //_edtContent.RequestFocus();
                //CmmDroidFunction.ShowSoftKeyBoard(_rootView, _mainAct);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "ShowPopup_ControlNumber", ex);
#endif
            }

        }
        public void ShowPopup_ViewFullInformation(ViewElement clickedElement)
        {
            try
            {
                //#region Get View - Init Data
                //View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_InputText, null);
                //ImageView _imgBack = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputText_Close);
                //TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_InputText_Title);
                //ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputText_Done);
                //EditText _edtContent = _viewPopupControl.FindViewById<EditText>(Resource.Id.edt_PopupControl_InputText);
                //ImageView _imgClearText = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputText_ClearText);
                //LinearLayout _lnEdt = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControl_InputText);

                //LinearLayout.LayoutParams _paramsEdt = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
                //_paramsEdt.SetMargins(15, 15, 15, 30);
                //_lnEdt.LayoutParameters = _paramsEdt; // set cho full màn hình

                //_imgDone.Visibility = ViewStates.Invisible;
                //_edtContent.Focusable = false;
                //_tvTitle.Text = clickedElement.Title;


                //if (!String.IsNullOrEmpty(clickedElement.Value))
                //{
                //    _edtContent.Text = CmmDroidFunction.FormatHTMLToString(clickedElement.Value); // Format vì phòng hờ có Rich Text
                //}
                //else
                //    _edtContent.Text = "";
                //#endregion

                //#region Event
                //_imgBack.Click += (sender, e) =>
                //{
                //    _dialogPopupControl.Dismiss();
                //};
                //#endregion

                //#region Show View                
                //_dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                //Window window = _dialogPopupControl.Window;
                //_dialogPopupControl.RequestWindowFeature(1);
                //_dialogPopupControl.SetCanceledOnTouchOutside(false);
                //_dialogPopupControl.SetCancelable(true);
                //window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                //window.SetGravity(GravityFlags.Bottom);
                //var dm = _mainAct.Resources.DisplayMetrics;

                //_dialogPopupControl.SetContentView(_viewPopupControl);
                //_dialogPopupControl.Show();
                //WindowManagerLayoutParams s = window.Attributes;
                //s.Width = WindowManagerLayoutParams.MatchParent;
                //s.Height = WindowManagerLayoutParams.MatchParent;
                //window.Attributes = s;
                //#endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "ShowPopup_ControlTextInput", ex);
#endif
            }
        }
        private void Handle_ControlYesNo(ViewElement clickedElement)
        {
            try
            {
                Tuple<int, int, int> _index = CTRLDetailWorkflow.FindElementIndexInListSection(clickedElement, _LISTSECTION);
                if (_index.Item1 != -1 && _index.Item2 != -1 && _index.Item3 != -1)
                {
                    _LISTSECTION[_index.Item1].ViewRows[_index.Item2].Elements[_index.Item3].Value = clickedElement.Value;
                }
                UpdateItemForEditedElement(clickedElement);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Handle_ControlYesNo", ex);
#endif
            }
        }
        public void UpdateValueForElement(ViewElement clickedElement, string _newValue)
        {
            try
            {
                Tuple<int, int, int> _index = CTRLBase.FindElementIndexInListSection(clickedElement, _LISTSECTION);
                if (_index.Item1 != -1 && _index.Item2 != -1 && _index.Item3 != -1)
                {
                    _LISTSECTION[_index.Item1].ViewRows[_index.Item2].Elements[_index.Item3].Value = _newValue;
                }
                UpdateItemForEditedElement(clickedElement);
                _adapterDetailExpandControl.UpdateCurrentListSection(_LISTSECTION);
                _adapterDetailExpandControl.NotifyDataSetChanged();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "UpdateValueForElement", ex);
#endif
            }
        }
        /// <summary>
        /// Update Item vào List đã được Edit để send Action đi
        /// </summary>
        /// <param name="EditedElement"></param>
        private void UpdateItemForEditedElement(ViewElement editedElement)
        {
            try
            {
                if (_lstEditedElement == null || _lstEditedElement.Count <= 0)
                    _lstEditedElement = new List<ViewElement>();

                // Nếu Element này đã có trong List Edited -> Update
                for (int i = 0; i < _lstEditedElement.Count; i++)
                {
                    if (_lstEditedElement[i].ID.Equals(editedElement.ID)) // Đã có trong List
                    {
                        _lstEditedElement[i] = editedElement;
                        return;
                    }
                }
                _lstEditedElement.Add(editedElement); // chưa có trong List

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "UpdateItemForEditedElement", ex);
#endif
            }
        }
        #endregion

        #region Dynamic Action
        private void Click_ElementActionEvent(object sender, MinionActionCore.ElementActionClick e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true) // không cho click nhiều lần
                {
                    ViewElement _clickedElement = e.element;
                    //if (_clickedElement.Enable)
                    //{
                    if (_clickedElement.ID.ToLowerInvariant().Equals("more") || _clickedElement.Value.ToLowerInvariant().Equals("more")) // Action More
                    {
                        List<ButtonAction> _lstActionMore = _componentButtonBot._lstActionMore;
                        if (_lstActionMore != null && _lstActionMore.Count > 0)
                        {
                            ShowPopup_ControlActionMore(_lstActionMore);
                        }
                    }
                    else
                    {
                        ButtonAction _clickedBtnAction = new ButtonAction { ID = Convert.ToInt32(_clickedElement.ID), Title = _clickedElement.Title, Value = _clickedElement.Value, Notes = _clickedElement.Notes };
                        Click_Action(_clickedBtnAction);
                    }
                    //}
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Click_ElementAction", ex);
#endif
            }
        }
        private void ShowPopup_ControlActionMore(List<ButtonAction> _LISTACTIONMore)
        {
            try
            {
                if (_LISTACTIONMore != null && _LISTACTIONMore.Count > 0)
                {
                    View scheduleDetail = _inflater.Inflate(Resource.Layout.PopupActionMore, null);
                    ListView _lvAction = scheduleDetail.FindViewById<ListView>(Resource.Id.lv_PopupActionMore);
                    TextView _tvClose = scheduleDetail.FindViewById<TextView>(Resource.Id.tv_PopupActionMore_Close);
                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    {
                        _tvClose.Text = CmmFunction.GetTitle("TEXT_EXIT", "Thoát");
                    }
                    else
                    {
                        _tvClose.Text = CmmFunction.GetTitle("TEXT_EXIT", "Exit");
                    }

                    AdapterControlActionMore _adapterControlActionMore = new AdapterControlActionMore(_rootView.Context, _LISTACTIONMore);
                    _lvAction.Adapter = _adapterControlActionMore;
                    _lvAction.ItemClick += (sender, e) =>
                    {
                        _dialogActionMore.Dismiss();
                        Click_Action(_LISTACTIONMore[e.Position]);
                    };
                    _tvClose.Click += delegate
                    {
                        _dialogActionMore.Dismiss();
                    };

                    _dialogActionMore = new Dialog(_rootView.Context);
                    Window window = _dialogActionMore.Window;
                    _dialogActionMore.RequestWindowFeature(1);
                    _dialogActionMore.SetCanceledOnTouchOutside(false);
                    _dialogActionMore.SetCancelable(true);
                    window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                    window.SetGravity(GravityFlags.Bottom);
                    var dm = Resources.DisplayMetrics;

                    _dialogActionMore.SetContentView(scheduleDetail);
                    _dialogActionMore.Show();
                    WindowManagerLayoutParams s = window.Attributes;
                    s.Width = dm.WidthPixels;
                    s.Height = WindowManagerLayoutParams.WrapContent;
                    window.Attributes = s;
                    window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "ShowPopup_ControlActionMore", ex);
#endif
            }
        }
        private void Click_Action(ButtonAction buttonAction)
        {
            try
            {
                ////////if (CTRLDetailWorkflow.ValidateRequiredForm(_LISTSECTION) == false) // có trường chưa nhập
                ////////{
                ////////    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("K_Action_PleaseChooseUser", "Vui lòng nhập đầy đủ thông tin."),
                ////////                             CmmFunction.GetTitle("K_Action_PleaseChooseUser", "Please fill all data."));

                ////////    return;
                ////////}
                switch (buttonAction.ID)
                {
                    case (int)(999):  // 
                        {
                            Action_AddRelated(buttonAction);
                            break;
                        }
                    default: // 32 - Action Thu hồi
                        {

                            break;
                        }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Click_Action", ex);
#endif
            }
        }
        private void Action_AddRelated(ButtonAction buttonAction)
        {
            try
            {
                #region Get View - Init Data
                View _viewPopupAction = _inflater.Inflate(Resource.Layout.PopupAction_CreateWorkflow_AddRelated, null);
                TextView _tvtitle = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.tv_PopupAction_CreateWorkflow_AddRelated_Detail_Title);
                ImageView _imgClose = _popupViewAddRelated.FindViewById<ImageView>(Resource.Id.img_PopupAction_CreateWorkflow_AddRelated_Action_Close);
                ImageView _imgAccept = _popupViewAddRelated.FindViewById<ImageView>(Resource.Id.img_PopupAction_CreateWorkflow_AddRelated_Action_Accept);
                ImageView _imgSearch = _popupViewAddRelated.FindViewById<ImageView>(Resource.Id.img_PopupAction_CreateWorkflow_AddRelated_Action_Search);

                _tvStartDateControlLinkedWorkflow = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.tv_PopupAction_CreateWorkflow_AddRelated_Action_StartDate);
                LinearLayout _lnStartDate = _popupViewAddRelated.FindViewById<LinearLayout>(Resource.Id.ln_PopupAction_CreateWorkflow_AddRelated_Action_StartDate);

                _tvEndDateControlLinkedWorkflow = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.tv_PopupAction_CreateWorkflow_AddRelated_Action_EndDate);
                LinearLayout _lnEndDate = _popupViewAddRelated.FindViewById<LinearLayout>(Resource.Id.ln_PopupAction_CreateWorkflow_AddRelated_Action_EndDate);

                TextView _tvWorkflow = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.tv_PopupAction_CreateWorkflow_AddRelated_Action_Workflow);
                LinearLayout _lnWorkflow = _popupViewAddRelated.FindViewById<LinearLayout>(Resource.Id.ln_PopupAction_CreateWorkflow_AddRelated_Action_Workflow);

                TextView _tvStatus = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.tv_PopupAction_CreateWorkflow_AddRelated_Action_Status);
                LinearLayout _lnStatus = _popupViewAddRelated.FindViewById<LinearLayout>(Resource.Id.ln_PopupAction_CreateWorkflow_AddRelated_Action_Status);

                EditText _edtKeyword = _popupViewAddRelated.FindViewById<EditText>(Resource.Id.edt_PopupAction_CreateWorkflow_AddRelated_Action_Keyword);
                LinearLayout _lnKeyword = _popupViewAddRelated.FindViewById<LinearLayout>(Resource.Id.ln_PopupAction_CreateWorkflow_AddRelated_Action_Keyword);

                RecyclerView _recyData = _popupViewAddRelated.FindViewById<RecyclerView>(Resource.Id.recy_PopupAction_CreateWorkflow_AddRelated_Category);

                _tvtitle.Text = CmmFunction.GetTitle("TEXT_WORKFLOW_RELATE", "Quy trình liên kết");
                _tvStartDateControlLinkedWorkflow.Hint = CmmFunction.GetTitle("K_CreateWorkflow", "Từ ngày");
                _tvEndDateControlLinkedWorkflow.Hint = CmmFunction.GetTitle("K_CreateWorkflow", "Đến ngày");
                _tvWorkflow.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Quy trình");
                _tvStatus.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Trạng thái");
                _edtKeyword.Hint = CmmFunction.GetTitle("K_CreateWorkflow", "Từ khóa (Mã & nội dung)");

                //if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                //{
                //    if (buttonAction.ID == (int)WorkflowAction.Action.Reject)
                //    {
                //        _tvTitle.Text = CmmFunction.GetTitle("K_Mess_Action_Accept", "Hủy phê duyệt yêu cầu");
                //    }
                //    else if (buttonAction.ID == (int)WorkflowAction.Action.Idea)
                //    {
                //        _tvTitle.Text = CmmFunction.GetTitle("K_Mess_Action_Idea", "Cho ý kiến");
                //    }
                //    else
                //    {
                //        _tvTitle.Text = CmmFunction.GetTitle("TEXT_REJECT_REQUEST", "Từ chối phê duyệt yêu cầu");
                //    }
                //    _edtComment.Hint = CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến");
                //    _tvCancel.Text = CmmFunction.GetTitle("TEXT_EXIT", "Thoát");
                //    _tvAccept.Text = buttonAction.Title;// CmmFunction.GetTitle("TEXT_DONE", "Hoàn tất");
                //}
                //else
                //{
                //    if (buttonAction.ID == (int)WorkflowAction.Action.Reject)
                //    {
                //        _tvTitle.Text = CmmFunction.GetTitle("K_Mess_Action_Accept", "Cancel request");
                //    }
                //    else if (buttonAction.ID == (int)WorkflowAction.Action.Idea)
                //    {
                //        _tvTitle.Text = CmmFunction.GetTitle("K_Mess_Action_Idea", "Idea");
                //    }
                //    else
                //    {
                //        _tvTitle.Text = CmmFunction.GetTitle("TEXT_REJECT_REQUEST", "Reject request");
                //    }
                //    _edtComment.Hint = CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Leave a comment/ opinion here");
                //    _tvCancel.Text = CmmFunction.GetTitle("TEXT_EXIT", "Exit");
                //    _tvAccept.Text = buttonAction.Title;// CmmFunction.GetTitle("TEXT_DONE", "Done");
                //}

                //string _imageName = "icon_bpm_Btn_action_" + buttonAction.ID.ToString();
                //int resId = _mainAct.Resources.GetIdentifier(_imageName.ToLowerInvariant(), "drawable", _mainAct.PackageName);
                //_imgAction.SetImageResource(resId);

                //////ImageViewCompat.SetImageTintList(_imgAction, Android.Content.Res.ColorStateList.ValueOf(Color.ParseColor("#" + buttonAction.Color)));

                //#endregion

                //#region Event
                //_tvAccept.Click += (sender, e) =>
                //{
                //    if (CTRLDetailWorkflow.CheckActionHasComment(_mainAct, _edtComment) == true)
                //    {
                //        CmmDroidFunction.HideSoftKeyBoard(_edtComment, _mainAct);
                //        _dialogAction.Dismiss();
                //        //Action_SendAPI(buttonAction, _edtComment.Text, null);
                //    }
                //};
                //_tvCancel.Click += (sender, e) =>
                //{
                //    CmmDroidFunction.HideSoftKeyBoard(_edtComment, _mainAct);
                //    _dialogAction.Dismiss();
                //};
                //_edtComment.TextChanged += (sender, e) =>
                //{
                //    if (String.IsNullOrEmpty(_edtComment.Text))
                //        _edtComment.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Italic);
                //    else
                //        _edtComment.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                //};
                #endregion

                #region Show View
                _dialogAction = new Dialog(_rootView.Context);
                Window window = _dialogAction.Window;
                _dialogAction.RequestWindowFeature(1);
                _dialogAction.SetCanceledOnTouchOutside(false);
                _dialogAction.SetCancelable(true);
                window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                window.SetGravity(GravityFlags.Center);
                var dm = Resources.DisplayMetrics;

                _dialogAction.SetContentView(_viewPopupAction);
                _dialogAction.Show();
                WindowManagerLayoutParams s = window.Attributes;
                s.Width = dm.WidthPixels;
                s.Height = dm.HeightPixels;
                window.Attributes = s;
                window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                #endregion

                ////_edtComment.Text = "";
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Action_Reject", ex);
#endif
            }
        }

        private void ControlLinkedWorkflow_ShowPopupChooseWorkflow(ViewElement _currentElement)
        {
            try
            {
                _lstWorkflowItemControlLinkedWorkflow = new List<BeanWorkflowItem>();
                List<BeanWorkflowItem> _lstWorkflowIsClicked = new List<BeanWorkflowItem>(); // để lưu tạm những item nào đang dc click (checkbox)


                _lstWorkflowIsClicked = JsonConvert.DeserializeObject<List<BeanWorkflowItem>>(_currentElement.Value);

                if (_lstWorkflowIsClicked == null) _lstWorkflowIsClicked = new List<BeanWorkflowItem>();

                List<string> _lstWorkflow = new List<string>();
                _lstWorkflow.Add("Tất cả");
                _lstWorkflow.Add("Bảng dự trù chi phí đoành thanh niên");
                _lstWorkflow.Add("Bảng dự trù kinh phí công đoàn");
                _lstWorkflow.Add("Bảng dự trù kinh phí đảng ủy");
                _lstWorkflow.Add("Bảng đánh giá năng lực nhân viên");

                List<string> _lstStatus = new List<string>();
                _lstStatus.Add("Tất cả");
                _lstStatus.Add("Chờ phê duyệt");
                _lstStatus.Add("Đã phê duyệt");
                _lstStatus.Add("Không phê duyệt");

                #region Get View 
                DisplayMetrics _displayMetrics = Resources.DisplayMetrics;
                _popupViewAddRelated = _inflater.Inflate(Resource.Layout.PopupControlLinkedWorkflow, null);
                TextView _tvtitle = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.tv_PopupControlLinkedWorkflow_Detail_Title);
                ImageView _imgClose = _popupViewAddRelated.FindViewById<ImageView>(Resource.Id.img_PopupControlLinkedWorkflow_Action_Close);
                ImageView _imgAccept = _popupViewAddRelated.FindViewById<ImageView>(Resource.Id.img_PopupControlLinkedWorkflow_Action_Accept);
                ImageView _imgSearch = _popupViewAddRelated.FindViewById<ImageView>(Resource.Id.img_PopupControlLinkedWorkflow_Action_Search);

                TextView _lblStartDate = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.lbl_PopupControlLinkedWorkflow_Action_StartDate);
                _tvStartDateControlLinkedWorkflow = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.tv_PopupControlLinkedWorkflow_Action_StartDate);
                LinearLayout _lnStartDate = _popupViewAddRelated.FindViewById<LinearLayout>(Resource.Id.ln_PopupControlLinkedWorkflow_Action_StartDate);

                TextView _lblEndDate = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.lbl_PopupControlLinkedWorkflow_Action_EndDate);
                _tvEndDateControlLinkedWorkflow = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.tv_PopupControlLinkedWorkflow_Action_EndDate);
                LinearLayout _lnEndDate = _popupViewAddRelated.FindViewById<LinearLayout>(Resource.Id.ln_PopupControlLinkedWorkflow_Action_EndDate);

                TextView _lblWorkflow = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.lbl_PopupControlLinkedWorkflow_Action_Workflow);
                TextView _tvWorkflow = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.tv_PopupControlLinkedWorkflow_Action_Workflow);
                LinearLayout _lnWorkflow = _popupViewAddRelated.FindViewById<LinearLayout>(Resource.Id.ln_PopupControlLinkedWorkflow_Action_Workflow);

                TextView _lblStatus = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.lbl_PopupControlLinkedWorkflow_Action_Status);
                TextView _tvStatus = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.tv_PopupControlLinkedWorkflow_Action_Status);
                LinearLayout _lnStatus = _popupViewAddRelated.FindViewById<LinearLayout>(Resource.Id.ln_PopupControlLinkedWorkflow_Action_Status);

                TextView _lblKeyword = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.lbl_PopupControlLinkedWorkflow_Action_Keyword);
                EditText _edtKeyword = _popupViewAddRelated.FindViewById<EditText>(Resource.Id.edt_PopupControlLinkedWorkflow_Action_Keyword);
                LinearLayout _lnKeyword = _popupViewAddRelated.FindViewById<LinearLayout>(Resource.Id.ln_PopupControlLinkedWorkflow_Action_Keyword);

                TextView _lblID = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.tv_PopupControlLinkedWorkflow_Category_Label_ID);
                TextView _lblContent = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.tv_PopupControlLinkedWorkflow_Category_Label_Content);
                TextView _lblCreatedName = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.tv_PopupControlLinkedWorkflow_Category_Label_CreatedName);
                TextView _lblCreatedDate = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.tv_PopupControlLinkedWorkflow_Category_Label_CreatedDate);

                RecyclerView _recyData = _popupViewAddRelated.FindViewById<RecyclerView>(Resource.Id.recy_PopupControlLinkedWorkflow_Category);
                RecyclerView _recyDataPaging = _popupViewAddRelated.FindViewById<RecyclerView>(Resource.Id.recy_PopupControlLinkedWorkflow_Category_Paging);
                TextView _tvPagingCount = _popupViewAddRelated.FindViewById<TextView>(Resource.Id.tv_PopupControlLinkedWorkflow_Category_Paging);

                ////_tvStartDateControlLinkedWorkflow.Text = DateTime.Now.ToString("dd/MM/yyyy");
                ////_tvEndDateControlLinkedWorkflow.Text = DateTime.Now.ToString("dd/MM/yyyy");
                _tvWorkflow.Text = _tvStatus.Text = "";
                #endregion

                #region Data + Event

                if (CmmVariable.SysConfig.LangCode == "VN")
                {
                    _tvtitle.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Quy trình / công việc liên kết");
                    _lblStartDate.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Từ ngày");
                    _lblEndDate.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Đến ngày");
                    _lblWorkflow.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Quy trình");
                    _lblStatus.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Trạng thái");
                    _lblKeyword.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Từ khóa");
                    _edtKeyword.Hint = CmmFunction.GetTitle("K_CreateWorkflow", "Nhập mã hoặc nội dung");
                    _lblID.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Mã yêu cầu");
                    _lblContent.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Nội dung");
                    _lblCreatedName.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Người yêu cầu");
                    _lblCreatedDate.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Ngày tạo");
                }
                else
                {
                    _tvtitle.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Linked Workflow / Task");
                    _lblStartDate.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Start date");
                    _lblEndDate.Text = CmmFunction.GetTitle("K_CreateWorkflow", "End Date");
                    _lblWorkflow.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Workflow");
                    _lblStatus.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Status");
                    _lblKeyword.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Keyword");
                    _edtKeyword.Hint = CmmFunction.GetTitle("K_CreateWorkflow", "Enter ID or content");
                    _lblID.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Required ID");
                    _lblContent.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Content");
                    _lblCreatedName.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Created name");
                    _lblCreatedDate.Text = CmmFunction.GetTitle("K_CreateWorkflow", "Created date");
                }

                ProviderBase pBase = new ProviderBase();

                //string _queryVTBD = CTRLHomePage._queryVTBD;
                string _queryVTBD = "";
                _lstWorkflowItemControlLinkedWorkflow = pBase.LoadMoreDataT<BeanWorkflowItem>(_queryVTBD, 100, 100, _lstWorkflowItemControlLinkedWorkflow.Count);

                if (_lstWorkflowItemControlLinkedWorkflow != null && _lstWorkflowItemControlLinkedWorkflow.Count > 0)
                {
                    StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                    adapterControlLinkedWorkflowListPagingNew = new AdapterControlLinkedWorkflowListPagingNew(_rootView.Context, _mainAct, _lstWorkflowItemControlLinkedWorkflow, _lstWorkflowIsClicked);
                    adapterControlLinkedWorkflowListPagingNew.CustomItemClick_CheckedChangeItem += (sender, e) =>
                    {
                        BeanWorkflowItem _clickedWorkflow = (BeanWorkflowItem)e.Item1;
                        bool _clickedState = (bool)e.Item2;
                        if (_clickedState == true) // Add
                        {
                            if (_lstWorkflowIsClicked.FindAll(x => x.ID.Equals(_clickedWorkflow.ID)).ToList().Count <= 0)
                            {
                                _lstWorkflowIsClicked.Add(_clickedWorkflow);
                            }
                        }
                        else // Remove
                        {
                            _lstWorkflowIsClicked.RemoveAll(x => x.ID.Equals(_clickedWorkflow.ID));
                        }
                        adapterControlLinkedWorkflowListPagingNew.NotifyDataSetChanged();
                    };
                    _recyData.SetLayoutManager(staggeredGridLayoutManager);
                    _recyData.SetAdapter(adapterControlLinkedWorkflowListPagingNew);
                }
                _imgClose.Click += delegate
                {
                    _dialogChooseWorkflow.Dismiss();
                };
                _imgAccept.Click += delegate
                {
                    Tuple<int, int> _index = CmmDroidFunction.Find_RowIndex_ElementIndex_ListControl(_currentElement, _lstRowsDetail);
                    if (_lstWorkflowIsClicked != null && _lstWorkflowIsClicked.Count > 0)
                    {
                        _lstRowsDetail[_index.Item1].Elements[_index.Item2].Value = JsonConvert.SerializeObject(_lstWorkflowIsClicked);
                    }
                    else
                    {
                        _lstRowsDetail[_index.Item1].Elements[_index.Item2].Value = "";
                    }
                    _adapterDetailControl.NotifyDataSetChanged();
                    _dialogChooseWorkflow.Dismiss();
                };
                _lnStartDate.Click += delegate
                {
                    //_flagDatePicker = 1;
                    //if (String.IsNullOrEmpty(_tvStartDateControlLinkedWorkflow.Text))
                    //{
                    //    ShowDatePickerDialog(DateTime.Now);
                    //}
                    //else
                    //{
                    //    DateTime _current = DateTime.ParseExact(_tvStartDateControlLinkedWorkflow.Text, "dd/MM/yyyy", null);
                    //    ShowDatePickerDialog(_current);
                    //}
                };
                _lnEndDate.Click += delegate
                {
                    //_flagDatePicker = 2;
                    //if (String.IsNullOrEmpty(_tvEndDateControlLinkedWorkflow.Text))
                    //{
                    //    ShowDatePickerDialog(DateTime.Now);
                    //}
                    //else
                    //{
                    //    DateTime _current = DateTime.ParseExact(_tvEndDateControlLinkedWorkflow.Text, "dd/MM/yyyy", null);
                    //    ShowDatePickerDialog(_current);
                    //}
                };
                _lnWorkflow.Click += delegate
                {
                    _dialogDetail = new Dialog(_rootView.Context, Android.Resource.Style.ThemeMaterialNoActionBarFullscreen);

                    LayoutInflater _layoutInflater = (LayoutInflater)_mainAct.GetSystemService(Context.LayoutInflaterService);
                    View _popupViewDetail = _layoutInflater.Inflate(Resource.Layout.PopupControlLinkedWorkflowDetailWindow, null);
                    TextView _tvtitle = _popupViewDetail.FindViewById<TextView>(Resource.Id.tv_PopupControlLinkedWorkflowDetailWindow_Title);
                    ImageView _imgClose = _popupViewDetail.FindViewById<ImageView>(Resource.Id.img_PopupControlLinkedWorkflowDetailWindow_Close);
                    ListView _lvData = _popupViewDetail.FindViewById<ListView>(Resource.Id.lv_PopupControlLinkedWorkflowDetailWindow);

                    if (CmmVariable.SysConfig.LangCode == "VN")
                    {
                        _tvtitle.Text = "Quy trình";
                    }
                    else
                    {
                        _tvtitle.Text = "Workflow";
                    }

                    AdapterControlLinkedWorkflowDetailChoose _adapterlvCategory = new AdapterControlLinkedWorkflowDetailChoose(_rootView.Context, _lstWorkflow, _mainAct, _tvWorkflow.Text);
                    _lvData.Divider = null;
                    _lvData.Adapter = _adapterlvCategory;
                    _lvData.ItemClick += (sender, e) =>
                    {
                        _tvWorkflow.Text = _lstWorkflow[e.Position].ToString();
                        _dialogDetail.Dismiss();
                    };
                    _imgClose.Click += delegate
                    {
                        _dialogDetail.Dismiss();
                    };

                    Window window = _dialogChooseWorkflow.Window;
                    var dm = Resources.DisplayMetrics;
                    window.SetGravity(GravityFlags.Center);

                    _dialogDetail.RequestWindowFeature(1);
                    _dialogDetail.SetCanceledOnTouchOutside(false);
                    _dialogDetail.SetCancelable(true);
                    _dialogDetail.SetContentView(_popupViewDetail);
                    _dialogDetail.Show();

                    WindowManagerLayoutParams s = window.Attributes;
                    s.Width = dm.WidthPixels /** 3 / 4*/;
                    s.Height = dm.HeightPixels /** 9 / 10*/;
                    window.Attributes = s;
                };
                _lnStatus.Click += delegate
                {
                    _dialogDetail = new Dialog(_rootView.Context, Android.Resource.Style.ThemeMaterialNoActionBarFullscreen);

                    LayoutInflater _layoutInflater = (LayoutInflater)_mainAct.GetSystemService(Context.LayoutInflaterService);
                    View _popupViewDetail = _layoutInflater.Inflate(Resource.Layout.PopupControlLinkedWorkflowDetailWindow, null);
                    TextView _tvtitle = _popupViewDetail.FindViewById<TextView>(Resource.Id.tv_PopupControlLinkedWorkflowDetailWindow_Title);
                    ImageView _imgClose = _popupViewDetail.FindViewById<ImageView>(Resource.Id.img_PopupControlLinkedWorkflowDetailWindow_Close);
                    ListView _lvData = _popupViewDetail.FindViewById<ListView>(Resource.Id.lv_PopupControlLinkedWorkflowDetailWindow);

                    if (CmmVariable.SysConfig.LangCode == "VN")
                    {
                        _tvtitle.Text = "Quy trình";
                    }
                    else
                    {
                        _tvtitle.Text = "Workflow";
                    }

                    AdapterControlLinkedWorkflowDetailChoose _adapterlvCategory = new AdapterControlLinkedWorkflowDetailChoose(_rootView.Context, _lstStatus, _mainAct, _tvStatus.Text);
                    _lvData.Divider = null;
                    _lvData.Adapter = _adapterlvCategory;
                    _lvData.ItemClick += (sender, e) =>
                    {
                        _tvStatus.Text = _lstStatus[e.Position].ToString();
                        _dialogDetail.Dismiss();
                    };
                    _imgClose.Click += delegate
                    {
                        _dialogDetail.Dismiss();
                    };

                    Window window = _dialogChooseWorkflow.Window;
                    var dm = Resources.DisplayMetrics;
                    window.SetGravity(GravityFlags.Center);

                    _dialogDetail.RequestWindowFeature(1);
                    _dialogDetail.SetCanceledOnTouchOutside(false);
                    _dialogDetail.SetCancelable(true);
                    _dialogDetail.SetContentView(_popupViewDetail);
                    _dialogDetail.Show();

                    WindowManagerLayoutParams s = window.Attributes;
                    s.Width = dm.WidthPixels /** 3 / 4*/;
                    s.Height = dm.HeightPixels /** 9 / 10*/;
                    window.Attributes = s;
                };
                _imgSearch.Click += delegate
                {
                    ////DateTime _startDate = DateTime.ParseExact(_tvStartDateControlLinkedWorkflow.Text, "dd/MM/yyyy", null);
                    ////DateTime _endDate = DateTime.ParseExact(_tvEndDateControlLinkedWorkflow.Text, "dd/MM/yyyy", null);
                    ////string _workflow = _tvWorkflow.Text;
                    ////string _status = _tvStatus.Text;
                    ////string _keyword = _edtKeyword.Text;

                    ////if (_startDate != null && _endDate != null)
                    ////{
                    ////    if (DateTime.Compare(_startDate, _endDate) > 0) // _startDate >_endDate
                    ////    {
                    ////        if (CmmVariable.SysConfig.LangCode == "VN")
                    ////        {
                    ////            CmmDroidFunction.ShowAlertDialog(_mainAct, "Thông báo", "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.", null, "Đóng");
                    ////        }
                    ////        else
                    ////        {
                    ////            CmmDroidFunction.ShowAlertDialog(_mainAct, "Alert", "End date must be greater or equal start date.", null, "Close");
                    ////        }
                    ////    }
                    ////    else
                    ////    {
                    ////        _lstWorkflowItemControlLinkedWorkflow = _lstWorkflowItemControlLinkedWorkflow.Where(x => x.IssueDate >= _startDate && x.IssueDate <= _endDate).ToList();
                    ////    }
                    ////}
                    ////if (!String.IsNullOrEmpty(_workflow))
                    ////{
                    ////    _lstWorkflowItemControlLinkedWorkflow = _lstWorkflowItemControlLinkedWorkflow.Where(x => x.WorkflowTitle.Equals(_workflow)).ToList();
                    ////}
                    ////if (!String.IsNullOrEmpty(_status))
                    ////{
                    ////    _lstWorkflowItemControlLinkedWorkflow = _lstWorkflowItemControlLinkedWorkflow.Where(x => x.ActionStatus.Equals(_workflow)).ToList();
                    ////}
                    ////if (!String.IsNullOrEmpty(_keyword))
                    ////{
                    ////    _lstWorkflowItemControlLinkedWorkflow = _lstWorkflowItemControlLinkedWorkflow.Where(x => x.WorkflowTitle.Equals(_workflow)).ToList();
                    ////}
                    ////adapterControlLinkedWorkflowListPagingNew.UpdateListData(_lstWorkflowItemControlLinkedWorkflow);
                    ////adapterControlLinkedWorkflowListPagingNew.NotifyDataSetChanged();

                };
                #endregion

                #region Show View
                _dialogChooseWorkflow = new Dialog(_rootView.Context, Android.Resource.Style.ThemeMaterialNoActionBarFullscreen);
                Window window = _dialogChooseWorkflow.Window;
                var dm = Resources.DisplayMetrics;
                window.SetGravity(GravityFlags.Center);

                _dialogChooseWorkflow.RequestWindowFeature(1);
                _dialogChooseWorkflow.SetCanceledOnTouchOutside(false);
                _dialogChooseWorkflow.SetCancelable(true);
                _dialogChooseWorkflow.SetContentView(_popupViewAddRelated);
                _dialogChooseWorkflow.Show();

                WindowManagerLayoutParams s = window.Attributes;
                s.Width = dm.WidthPixels /** 3 / 4*/;
                s.Height = dm.HeightPixels /** 9 / 10*/;
                window.Attributes = s;
                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentCreateWorkflow - ControlLinkedWorkflow_ShowPopupChooseWorkflow - Error: " + ex.Message);
#endif
            }
        }
        #endregion

        #endregion
    }
}