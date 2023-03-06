using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Core.Controller
{
    public class ControllerDetailProcess : ControllerBase
    {
        public Color GetColorByActionIDProcess(Context _context, int SubmitActionID)
        {
            try
            {
                switch (SubmitActionID)
                {
                    // Chưa có admin can thiệp, thực hiện bổ sung
                    case 12: // Gửi
                    case 1: // Đồng ý
                    case 53: // Thực hiện điều chỉnh
                    case 7: // Yêu cầu bổ sung
                    case 9: // Yêu cầu tham vấn
                    case 10: // Thực hiện tham vấn
                    case 3: // Chuyển xử lý
                    case 6: // Thu hồi
                        return new Color(ContextCompat.GetColor(_context, Resource.Color.clDetailProcessBlue));
                    case 2: // Phê duyệt
                        return new Color(ContextCompat.GetColor(_context, Resource.Color.clDetailProcessGreen));
                    case 4: // Yêu cầu hiệu chỉnh
                        return new Color(ContextCompat.GetColor(_context, Resource.Color.clDetailProcessPink));
                    case 51: // Hủy
                    case 5: // Từ chối
                        return new Color(ContextCompat.GetColor(_context, Resource.Color.clDetailProcessRed));
                    case 47: // Chờ xử lý
                    case 48: // Chờ xử lý
                    case 49: // Chờ xử lý
                    case 50: // Chờ xử lý
                        return new Color(ContextCompat.GetColor(_context, Resource.Color.clDetailProcessYellow));
                    default:
                        return new Color(ContextCompat.GetColor(_context, Resource.Color.clDetailProcessBlue));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("ControllerHomePage", "GetColorByActionID", ex);
#endif
            }
            return new Color(ContextCompat.GetColor(_context, Resource.Color.clStatusBlue));
        }
    }
}