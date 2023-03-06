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
using Android.Support.V4.Content;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Core.Controller
{
    public class ControllerDetailWorkflow : ControllerBase
    {
        //public string _queryBeanUserGroup_Old = @"SELECT ID, Title as Name, Description as Email, 1 as Type FROM BeanGroup 
        //                UNION SELECT ID, FullName as Name, Email, 0 as Type FROM BeanUser";

        public string _queryBeanUserGroup = @"SELECT ID, Title as Name, Title as AccountName, Description as Email, Description as ImagePath, 1 as Type FROM BeanGroup 
                                              UNION SELECT ID, FullName as Name, AccountName as AccountName, Email, ImagePath as ImagePath, 0 as Type FROM BeanUser";

        public string _queryBeanUser = @"SELECT ID, FullName as Name, AccountName as AccountName, Email, ImagePath as ImagePath, 0 as Type FROM BeanUser";
        public string _queryBeanGroup = @"SELECT ID, Title as Name, Title as AccountName, Description as Email, Description as ImagePath, 1 as Type FROM BeanGroup";

        public string _queryUpdateFavorite = @"UPDATE BeanWorkflowFollow SET Status = {0} WHERE WorkflowItemId = '{1}'";

        public Color GetColorByAction(Context _context, string _action)
        {
            if (_action.ToLowerInvariant().Contains("duyệt"))
            {
                return new Color(ContextCompat.GetColor(_context, Resource.Color.clGreen));
            }
            else if (_action.ToLowerInvariant().Contains("bổ sung thông tin") || _action.ToLowerInvariant().Contains("yêu cầu hiệu chỉnh") || _action.ToLowerInvariant().Contains("thu hồi"))
            {
                return new Color(ContextCompat.GetColor(_context, Resource.Color.clOrange));
            }
            else if (_action.ToLowerInvariant().Contains("từ chối") || _action.ToLowerInvariant().Contains("hủy"))
            {
                return new Color(ContextCompat.GetColor(_context, Resource.Color.clRed));
            }
            else
            {
                return new Color(ContextCompat.GetColor(_context, Resource.Color.clBlue));
            }
        }
        /// <summary>
        /// Nếu Flexbox quá 3 dòng - > giới hạn chiều cao lại và cho Scroll, nếu không cho wrap_content
        /// </summary>
        /// <param name="_recy"></param>
        public void SetFlexBoxChoiceUserHeight(Android.Support.V7.Widget.RecyclerView _recySelectedUser, int _rowHeight = 95, int _rowLimit = 3)
        {
            try
            {
                // Set thuộc tính lại để lấy ra chiều cao thực tế
                _recySelectedUser.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                _recySelectedUser.Post(() =>
                {
                    int realHeight = _recySelectedUser.MeasuredHeight;
                    if (realHeight >= (_rowHeight * _rowLimit))
                    {
                        _recySelectedUser.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, _rowHeight * _rowLimit);
                    }
                });
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("ControllerDetailWorkflow", "SetFlexBoxChoiceUserHeight", ex);
#endif
            }
        }

        public void SetTitleByItem(TextView _tvTitle, BeanWorkflowItem _workflowItem, BeanNotify _notifyItem)
        {
            try
            {
                if (_notifyItem != null)
                {
                    if (!String.IsNullOrEmpty(_notifyItem.Content)) _tvTitle.Text = _notifyItem.Content;
                }
                else
                {
                    if (!String.IsNullOrEmpty(_workflowItem.Content)) _tvTitle.Text = _workflowItem.Content;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("ControllerDetailWorkflow", "SetTitleByItem", ex);
#endif
            }
        }
        public ViewElement GetElementAttachFromSection(List<ViewSection> _LISTSECTION)
        {
            foreach (ViewSection _itemSection in _LISTSECTION)
                foreach (ViewRow _itemRow in _itemSection.ViewRows)
                    foreach (ViewElement _itemElement in _itemRow.Elements)
                        if (_itemElement.DataType.Equals("inputattachmenthorizon"))
                            return _itemElement;
            return null;
        }

        /// <summary>
        /// Check xem Edittext có comment chưa, chưa có -> hiện AlertDialog + return false 
        /// </summary>
        /// <param name="_edtComment"></param>
        public bool CheckActionHasComment(Activity _mainAct, EditText _edtComment)
        {
            bool _res = false;
            try
            {
                if (!string.IsNullOrEmpty(_edtComment.Text))
                {
                    _res = true;
                }
                else
                {
                    Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(_mainAct);
                    alert.SetTitle(CmmVariable.SysConfig.LangCode==CmmDroidVariable.M_SysLangVN?"Thông báo": "Notification");
                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    {
                        alert.SetMessage(CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến"));
                        alert.SetNegativeButton(CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"), (senderAlert, args) =>
                        {
                            alert.Dispose();
                        });
                    }
                    else
                    {
                        alert.SetMessage(CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Leave a comment/ opinion here"));
                        alert.SetNegativeButton(CmmFunction.GetTitle("TEXT_AGREE", "Agree"), (senderAlert, args) =>
                        {
                            alert.Dispose();
                        });
                    }
                    Dialog dialog = alert.Create();
                    dialog.SetCanceledOnTouchOutside(false);
                    dialog.SetCancelable(false);
                    dialog.Show();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("ControllerDetailWorkflow", "CheckEdtHasComment", ex);
#endif
            }
            return _res;
        }

        /// <summary>
        /// Check xem có beanUser selected chưa, chưa có -> hiện AlertDialog + return false 
        /// </summary>
        /// <param name="_edtComment"></param>
        public bool CheckActionHasSelectedUser(Activity _mainAct, BeanUser _selectedUser)
        {
            bool _res = false;
            try
            {
                if (_selectedUser != null && !String.IsNullOrEmpty(_selectedUser.ID))
                {
                    _res = true;
                }
                else
                {
                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("K_Action_PleaseChooseUser", "Vui lòng chọn người để thực hiện."),
                                                               CmmFunction.GetTitle("K_Action_PleaseChooseUser", "Please choose user to do action."));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("ControllerDetailWorkflow", "CheckEdtHasComment", ex);
#endif
            }
            return _res;
        }

        public List<BeanExpandComment> CloneListExpandComment(List<BeanComment> _lstComment)
        {
            List<BeanExpandComment> _res = new List<BeanExpandComment>();
            try
            {
                _lstComment = _lstComment.OrderBy(x => x.Created).ToList();
                List<BeanComment> _lstParent = _lstComment.Where(x => x.ParentCommentId == null).ToList();
                foreach (BeanComment item in _lstParent)
                {
                    BeanExpandComment _temp = new BeanExpandComment();
                    _temp.parentItem = item;
                    _temp.lstChild = _lstComment.Where(x => x.ParentCommentId == item.ID).ToList();
                    _res.Add(_temp);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("ControllerDetailWorkflow", "ValidateRequiredForm", ex);
#endif
            }
            return _res;
        }
    }
}