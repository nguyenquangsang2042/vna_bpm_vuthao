using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.Components
{
    public class ComponentBase : UIView
    {
        /// <summary>
        /// Cấu hình component mặc định gồm tiều đề và giá trị
        /// </summary>
        public virtual void InitializeComponent() { }

        /// <summary>
        /// Cấu hình frame mặc định tiều đề và giá trị với input là CGRect
        /// </summary>
        /// <returns></returns>
        public virtual void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;
        }

        /// <summary>
        /// Gán tiêu để cho component
        /// </summary>
        public virtual void SetTitle() { }

        /// <summary>
        /// Gán giá trị cho component
        /// </summary>
        public virtual void SetValue() { }

        /// <summary>
        /// Gán các thuộc tính cơ bản cho element
        /// </summary>
        public virtual void SetProprety() { }

        /// <summary>
        /// Bật tắt action cho component
        /// </summary>
        public virtual void SetEnable() { }

        /// <summary>
        /// Get and set giá trị cho compnent
        /// </summary>
        /// <returns></returns>
        public virtual string Value { get; set; }

        /// <summary>
        /// Bật tắt cần thiết cho tiêu đề
        /// </summary>
        public virtual void SetRequire() { }

        /// <summary>
        /// Trả về indexPath của component
        /// </summary>
        public virtual NSIndexPath IndexPath { get; }
    }
}