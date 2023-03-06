//using System;
//using Newtonsoft.Json;
//using System.Collections.Generic;
//namespace BPMOPMobileV1.iOS.IOSClass
//{
//    public static class ExtensionCopy
//    {
//        #region coppy list
//        public static List<T> CopyAll<T>(this List<T> list)
//        {
//            List<T> ret = new List<T>();
//            string tmpStr = JsonConvert.SerializeObject(list);
//            ret = JsonConvert.DeserializeObject<List<T>>(tmpStr);
//            return ret;
//        }
//        public static T CopyAll<T>(this T list)
//        {
//            T ret;
//            string tmpStr = JsonConvert.SerializeObject(list);
//            ret = JsonConvert.DeserializeObject<T>(tmpStr);
//            return ret;
//        }
//        #endregion
//    }
//}
