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
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Core.Controller
{
    public class ControllerDetailAttachFile : ControllerBase
    {
        public List<BeanGroupAttachFile> CloneListAttToGroup(List<BeanAttachFile> _lstAttachFile)
        {
            List<BeanGroupAttachFile> _result = new List<BeanGroupAttachFile>();
            try
            {
                List<string> _lstStrCategory = _lstAttachFile.GroupBy(x => x.AttachTypeName).Select(y => y.First().AttachTypeName).Distinct().ToList();
                BeanGroupAttachFile _itemGroupOther = new BeanGroupAttachFile() { Category = CmmFunction.GetTitle("TEXT_ADDNEW_ATTACHMENT", "Tạo mới"), AttachFiles = new List<BeanAttachFile>() };
                foreach (string _category in _lstStrCategory)
                {
                    if (!String.IsNullOrEmpty(_category))
                    {
                        List<BeanAttachFile> _lstByCategory = _lstAttachFile.Where(x => x.AttachTypeName != null && x.AttachTypeName.Equals(_category)).ToList();
                        _result.Add(new BeanGroupAttachFile() { Category = _category, AttachFiles = _lstByCategory });
                    }
                    else // File thêm mới -> ID = "0"
                    {
                        List<BeanAttachFile> _lstByCategory = _lstAttachFile.Where(x => x.AttachTypeName == _category).ToList();
                        _itemGroupOther.AttachFiles.AddRange(_lstByCategory);
                        ////if (_result.Where(x => x.Category.Equals("Khác")).ToList().Count == 0) // chưa có -> Add
                        ////{
                        ////    _itemGroupOther.AttachFiles.AddRange(_lstByCategory);
                        ////    _result.Add(_itemGroupOther);
                        ////}
                        ////else
                        ////{
                        ////    _itemGroupOther.AttachFiles.AddRange(_lstByCategory);
                        ////}
                    }
                }
                if (_itemGroupOther.AttachFiles != null && _itemGroupOther.AttachFiles.Count > 0) // Có loại khác mới Add
                    _result.Add(_itemGroupOther);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("ControllerDetailAttachFile", "CloneListAttToGroup", ex);
#endif
            }
            return _result;
        }
    }
}