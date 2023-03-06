using System;
using System.Collections.Generic;
using System.Reflection;
using SQLite;

namespace BPMOPMobileV1
{
    public class BeanBase
    {
        /// <summary>
        /// Lấy đị chỉ Url API xử lý trên mục Bean tương ứng
        /// </summary>
        /// <returns></returns>
        public virtual string GetApiUrlExec()
        {
            return "";
        }

        /// <summary>
        /// Lấy đường dẫn Url tương ứng lấy dữ liệu từ Server trong phương thức get data
        /// </summary>
        /// <returns></returns>
        public virtual string GetServerUrl()
        {
            return "";
        }

        /// <summary>
        /// Lấy Các Biến Post có định tương ứng với từng đối tượng
        /// </summary>
        /// <returns></returns>
        public virtual List<KeyValuePair<string, string>> GetParaPost()
        {
            return null;
        }

        /// <summary>
        /// Lấy các khóa chính trong table
        /// </summary>
        /// <param name="type">Kiểu dữ liệu (kiển bean ánh xạ table)</param>
        /// <returns></returns>
        public static List<string> GetPriKey(Type type)
        {
            return GetLstProName(type, typeof(PrimaryKeyAttribute));
        }

        /// <summary>
        /// Lấy các khóa chính trong table
        /// </summary>
        /// <param name="type">Kiểu dữ liệu (kiển bean ánh xạ table)</param>
        /// <returns></returns>
        public static List<string> GetPriKeyS(Type type)
        {

            return GetLstProName(type, typeof(PrimaryKeySAttribute));
        }

        public static List<string> GetLstProName(Type type, Type typeAttr)
        {
            List<string> retValue = new List<string>();

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            foreach (var p in props)
            {
                if (p.GetCustomAttributes(typeAttr, true).Length > 0)
                {
                    retValue.Add(p.Name);
                }
            }
            return retValue;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeySAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class HtmlEncodeAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class MobileKeyDiffAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class HtmlRemoveAttribute : Attribute
    {
    }
}
