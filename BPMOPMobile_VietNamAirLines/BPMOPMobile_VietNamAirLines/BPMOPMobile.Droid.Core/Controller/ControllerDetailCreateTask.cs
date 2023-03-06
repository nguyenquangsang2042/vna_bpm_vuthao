using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Core.Controller
{
    public class ControllerDetailCreateTask : ControllerBase
    {
        public enum FlagActionPermission
        {
            [Description("Không thao tác")]
            NoAction = -1,
            [Description("Tạo mới")]
            CreateNew = 0,
            [Description("Người tạo update")]
            CreatorUpdate = 1,
            [Description("Người xử lý update")]
            HandlerUpdate = 2,
            [Description("Người taọ đồng thời là người xử lý update")]
            CreatorHandlerUpdate = 3,
        }
        public enum FlagUserPermission // Check xem là người nào: -1: người xem, 1: người tạo, 2: người xử lý
        {
            [Description("Người xem")]
            Viewer = -1,
            [Description("Người tạo")]
            Creator = 1,
            [Description("Người xử lý")]
            Handler = 2,
            [Description("Người tạo đồng thời là người xử lý")]
            CreatorAndHandler = 3,
        }

        public enum ActionStatusID // ID của Status phiếu
        {
            [Description("Hủy")]
            Cancel = 4,
            [Description("Hoàn tất")]
            Completed = 2,
            [Description("Tạm hoãn")]
            Hold = 3,
            [Description("Đang thực hiện")]
            InProgress = 1,
            [Description("Chưa thực hiện")]
            NoProcess = 0,
        }
        public string _querGroupToBeanUserGroup = @"SELECT ID, Title as Name, Title as AccountName, Description as Email, Description as ImagePath, 1 as Type FROM BeanGroup WHERE ID= '{0}'";
        public string _querUserToBeanUserGroup = @"SELECT ID, FullName as Name, AccountName as AccountName, Email, ImagePath as ImagePath, 0 as Type FROM BeanUser WHERE ID= '{0}'";
        public string _queryBeanUser = @"SELECT * FROM BeanUser WHERE ID = '{0}'";
        public string _queryBeanPosition = @"SELECT * FROM BeanPosition WHERE ID = {0}";

        public void SetToolbarItem_Selected(Activity _mainAct, TextView _tv, View _vw)
        {
            _tv.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
            _vw.Visibility = ViewStates.Visible;
        }
        public void SetToolbarItem_NotSelected(Activity _mainAct, TextView _tv, View _vw)
        {
            _tv.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
            _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
            _vw.Visibility = ViewStates.Invisible;
        }
        public List<BeanUserAndGroup> QueryInfoListAssign(List<BeanUserAndGroup> _lstAssign)
        {
            List<BeanUserAndGroup> _res = new List<BeanUserAndGroup>();
            SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                foreach (BeanUserAndGroup _item in _lstAssign)
                {
                    if (_item.Type == 0) // User
                    {
                        List<BeanUserAndGroup> _temp = conn.Query<BeanUserAndGroup>(String.Format(_querUserToBeanUserGroup, _item.ID));
                        if (_temp != null && _temp.Count > 0)
                            _res.Add(_temp[0]);
                    }
                    else // Group
                    {
                        List<BeanUserAndGroup> _temp = conn.Query<BeanUserAndGroup>(String.Format(_querGroupToBeanUserGroup, _item.ID));
                        if (_temp != null && _temp.Count > 0)
                            _res.Add(_temp[0]);
                    }
                }
            }
            catch (Exception ex)
            {

#if DEBUG
                CmmDroidFunction.WriteTrackingError("ControllerDetailCreateTask", "QueryInfoListAssign", ex);
#endif
            }
            conn.Close();
            return _res;
        }
        public List<BeanLookupData> GetListLookUpTinhTrang()
        {
            //0: chưa xử lý
            //1: dang xử lý
            //2: hoàn tất
            //3 tạm hoãn
            //4 hủy
            List<BeanLookupData> _lstLookupData = new List<BeanLookupData>();
            _lstLookupData.Add(new BeanLookupData() { ID = "1", Title = GetStatusNameByID(1) });
            _lstLookupData.Add(new BeanLookupData() { ID = "3", Title = GetStatusNameByID(3) });
            _lstLookupData.Add(new BeanLookupData() { ID = "2", Title = GetStatusNameByID(2) });
            return _lstLookupData;
        }
        public string GetStatusNameByID(int Status)
        {
            try
            {
                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    switch (Status)
                    {
                        case (int)ActionStatusID.Cancel: // 4
                            return CmmFunction.GetTitle("TEXT_CANCEL", "Hủy");
                        case (int)ActionStatusID.Completed: // 2
                            return CmmFunction.GetTitle("TEXT_COMPLETED", "Hoàn tất");
                        case (int)ActionStatusID.Hold: // 3
                            return CmmFunction.GetTitle("TEXT_HOLD", "Tạm hoãn");
                        case (int)ActionStatusID.InProgress: // 1
                            return CmmFunction.GetTitle("TEXT_INPROGRESS", "Đang thực hiện");
                        default:
                            return CmmFunction.GetTitle("TEXT_NOPROCESS", "Chưa thực hiện");
                    }
                }
                else
                {
                    switch (Status)
                    {
                        case (int)ActionStatusID.Cancel: // 4
                            return CmmFunction.GetTitle("TEXT_CANCEL", "Cancel");
                        case (int)ActionStatusID.Completed: // 2
                            return CmmFunction.GetTitle("TEXT_COMPLETED", "Completed");
                        case (int)ActionStatusID.Hold: // 3
                            return CmmFunction.GetTitle("TEXT_HOLD", "Hold");
                        case (int)ActionStatusID.InProgress: // 1
                            return CmmFunction.GetTitle("TEXT_INPROGRESS", "In progress");
                        default:
                            return CmmFunction.GetTitle("TEXT_NOPROCESS", "No process");
                    }
                }
            }
            catch (Exception ex)
            {

#if DEBUG
                CmmDroidFunction.WriteTrackingError("ControllerDetailCreateTask", "GetStatusNameByID", ex);
#endif
            }
            return "";
        }
        public Color GetStatusColorByID(Context _context, int Status)
        {
            try
            {
                switch (Status)
                {
                    case (int)ActionStatusID.Cancel: // "Hủy"
                        return new Color(ContextCompat.GetColor(_context, Resource.Color.clTaskStatusRed));
                    case (int)ActionStatusID.Completed: // "Hoàn tất"
                        return new Color(ContextCompat.GetColor(_context, Resource.Color.clTaskStatusGreen));
                    case (int)ActionStatusID.Hold: // "Tạm hoãn"
                        return new Color(ContextCompat.GetColor(_context, Resource.Color.clTaskStatusRed));
                    case (int)ActionStatusID.InProgress: // Đang thực hiện
                        return new Color(ContextCompat.GetColor(_context, Resource.Color.clTaskStatusBlue));
                    default: // 0 - Chưa thực hiện
                        return new Color(ContextCompat.GetColor(_context, Resource.Color.clTaskStatusGray));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("ControllerDetailCreateTask", "GetStatusNameByID", ex);
#endif
            }
            return new Color(ContextCompat.GetColor(_context, Resource.Color.clTaskStatusBlue));
        }
    
        /// <summary>
        /// Remove background, disable view của từng control
        /// </summary>
        public void SetViewControl_NotEdited(LinearLayout _lnContent, TextView _tvContent)
        {           
            try
            {
                if (_lnContent != null)
                {
                    _lnContent.Enabled = false;
                    _lnContent.SetBackgroundColor(Color.Transparent);
                }
                if (_tvContent != null)
                {
                    _tvContent.SetPadding(0, 0, 0, 0);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewControl_NotEdited", ex);
#endif
            }
        }

        /// <summary>
        /// Remove background, disable view của từng control
        /// </summary>
        public void SetViewControl_Edited(LinearLayout _lnContent, TextView _tvContent)
        {
            try
            {
                if (_lnContent != null)
                {
                    _lnContent.Enabled = true;
                    //_lnContent.SetBackgroundColor(Color.Transparent);
                    _lnContent.SetPadding(6, 6, 6, 6);
                    _lnContent.SetBackgroundResource(Resource.Drawable.edtcornerstrokegray);

                }
                if (_tvContent != null)
                {
                    _tvContent.SetPadding(6, 6, 6, 6);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewControl_NotEdited", ex);
#endif
            }
        }
    }
}