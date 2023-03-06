using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Core.Controller
{
    public class ControllerDetailShare : ControllerBase
    {
        //public string _queryShareUserGroup = @"SELECT ID, Title as Name, Title as AccountName, Description as Email, 1 as Type FROM BeanGroup 
        //                                      UNION SELECT ID, FullName as Name, AccountName as AccountName, Email, 0 as Type FROM BeanUser";
        public string _queryShareUserGroup = @"SELECT ID, Title as Name, Title as AccountName, Description as Email, Description as ImagePath, 1 as Type FROM BeanGroup  
                                              UNION SELECT ID, FullName as Name, AccountName as AccountName, Email, ImagePath as ImagePath, 0 as Type FROM BeanUser";
        /// <summary>
        /// Clone qua List dữ liệu cho Adapter Expand
        /// </summary>
        /// <param name="_lstHistory"></param>
        /// <returns></returns>
        public List<BeanGroupShareHistory> CloneListGroupShareHistory(List<BeanShareHistory> _lstHistory)
        {
            List<BeanGroupShareHistory> _result = new List<BeanGroupShareHistory>();
            try
            {
                List<BeanShareHistory> _lstParent = _lstHistory.Where(x => x.ParentId == null).ToList();
                List<BeanShareHistory> _lstChild = _lstHistory.Where(x => x.ParentId != null).ToList();

                foreach (BeanShareHistory item in _lstParent)
                    _result.Add(new BeanGroupShareHistory() { parentItem = item, listChild = new List<BeanShareHistory>() });

                foreach (BeanShareHistory item in _lstChild)
                {
                    for (int i = 0; i < _result.Count; i++)
                    {
                        if (_result[i].parentItem.ID == item.ParentId)
                            _result[i].listChild.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("ControllerDetailShare", "GetColorByActionID", ex);
#endif
            }
            return _result;
        }
    }
}