using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ComponentBase
    {
        public enum EnumDynamicControlCategory
        {
            [Description("Lưới chi tiết")]
            Detail = 0,
            [Description("Lưới chi tiết của control Grid Input Detail")]
            TemplateValue = 1,
        }

        /// <summary>
        /// Get and set giá trị cho compnent
        /// </summary>
        /// <returns></returns>
        public virtual string Value { get; set; }
        public virtual LinearLayout Frame { get; set; }
        public virtual int Category { get; set; }

        /// <summary>
        /// Để xác định xem là Control trường hợp nào để tick event
        /// </summary>
        /// <param name="Category"></param>
        public virtual void InitializeCategory(int Category = (int)EnumDynamicControlCategory.Detail)
        {
            this.Category = Category;
        }

        /// <summary>
        /// Cấu hình component mặc định gồm tiều đề và giá trị
        /// </summary>
        public virtual void InitializeComponent() { }

        /// <summary>
        /// Cấu hình frame mặc định tiều đề và giá trị với input là CGRect
        /// </summary>
        /// <returns></returns>
        public virtual void InitializeFrameView(LinearLayout frame)
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
    }
}