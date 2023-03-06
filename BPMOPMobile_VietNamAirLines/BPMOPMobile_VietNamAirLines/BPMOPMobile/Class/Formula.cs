﻿using System;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;
using VuThao.Calc;

namespace BPMOPMobile.Class
{
    public class Formula
    {
        #region Constant

        public const string ASSEMBLY_STRING =
            "FormulaTesting, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4bd37aba9d092bd3";

        public const string CLASS_NAME = "FormulaTesting.Formula";
        private const string REGEX_REPLACE_FIELDS = @"(\[)(\d+)(\])";
        private const string REGEX_REPLACE_METHOD = @"(\s)({0})(\()";
        private const string EXPRESSION_COLUMN = "Expression";

        #endregion

        #region Properties

        //private Assembly _FormulaAssembly = null;
        //public Assembly FormulaAssembly
        //{
        //    get { if (_FormulaAssembly == null) _FormulaAssembly = GetAssemblyByFullName(); return _FormulaAssembly; }
        //    set { _FormulaAssembly = value; }
        //}
        //private DataTable _dtCompute;
        //public DataTable dtCompute
        //{
        //    get
        //    {
        //        if (_dtCompute == null)
        //        {
        //            _dtCompute = new DataTable();
        //            _dtCompute.Columns.Add("ID", typeof(int));
        //            _dtCompute.Columns["ID"].AutoIncrement = true;
        //            _dtCompute.Columns["ID"].AutoIncrementSeed = 1;
        //            _dtCompute.Columns["ID"].AutoIncrementStep = 1;
        //            _dtCompute.Columns.Add(EXPRESSION_COLUMN, typeof(object));

        //        }
        //        return _dtCompute;
        //    }
        //    set { _dtCompute = value; }
        //}

        #endregion

        #region Common Method

        //Assembly GetAssemblyByFullName(string strAssembly = ASSEMBLY_STRING)
        //{
        //    Assembly ReturnValue = null;
        //    try
        //    {
        //        Assembly[] AllAssembly = AppDomain.CurrentDomain.GetAssemblies();
        //        ReturnValue = (from Assembly Current in AllAssembly where Current.FullName.Equals(strAssembly) select Current).FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //    return ReturnValue;
        //}

        //public object ExcuteFormula(List<BeanFormulaComponent> components, Dictionary<long, object> Fields = null)
        //{
        //    object Value = null;
        //    if (components == null || components.Count == 0) return Value;
        //    dtCompute.Columns[EXPRESSION_COLUMN].Expression = null;
        //    dtCompute.Clear();

        //    foreach (var item in components)
        //        item.ExcuteResult(this, Fields);

        //    InitColumn_FromListComponent(components, 0);
        //    var row = dtCompute.NewRow();
        //    string Expression = string.Empty;
        //    for (int i = 0; i < components.Count; i++)
        //    {
        //        row["Col" + i] = components[i].Value;
        //        if (i != components.Count - 1)
        //            Expression += ("Col" + i) + " " + components[i].Operator;
        //        else
        //            Expression += ("Col" + i);
        //    }
        //    dtCompute.Rows.Add(row);
        //    dtCompute.Columns[EXPRESSION_COLUMN].Expression = Expression;
        //    return row[EXPRESSION_COLUMN];
        //}

        //private void InitColumn_FromListComponent(List<BeanFormulaComponent> ListComponents, int InitIndex = 0)
        //{
        //    try
        //    {
        //        if (ListComponents == null || ListComponents.Count == 0) return;

        //        for (int i = 0; i < ListComponents.Count; i++)
        //        {
        //            if (!dtCompute.Columns.Contains("Col" + (i + InitIndex)))
        //            {
        //                dtCompute.Columns.Add("Col" + (i + InitIndex), typeof(object));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //}

        public static object[] GetRealParams(MethodInfo method, object[] Params)
        {
            try
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 0)
                    return new object[0];

                var hasParams = parameters[parameters.Length - 1].GetCustomAttributes(typeof(ParamArrayAttribute), false)
                                     .Length > 0;
                var realParams = new object[parameters.Length];
                for (var i = 0; i < parameters.Length - (hasParams ? 1 : 0); i++)
                    if (Params.Length > i)
                    {
                        realParams[i] = Params[i];
                    }
                    else
                    {
                        var param = parameters[i];
                        if (param.HasDefaultValue)
                            realParams[i] = param.DefaultValue;
                        else
                            throw new Exception("Truyền chưa đủ tham số");
                    }

                if (!hasParams) return realParams;


                var lastParamPosition = parameters.Length - 1;
                var paramsType = parameters[lastParamPosition].ParameterType.GetElementType();
                if (paramsType == null) return realParams;
                var extra = Array.CreateInstance(paramsType, Params.Length - lastParamPosition);
                for (var i = 0; i < extra.Length; i++)
                    extra.SetValue(Params[i + lastParamPosition], i);

                realParams[lastParamPosition] = extra;


                return realParams;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Params;
        }

        public static void Exp_EvaluateFunction(string name, FunctionArgs args)
        {
            try
            {
                var myType = typeof(Formula);
                var myMethod = myType.GetMethod(name);

                var myObject = Activator.CreateInstance(myType);
                var listExpression = args.EvaluateParameters();

                var Params = GetRealParams(myMethod, listExpression);

                if (myMethod == null) return;
                var value = myMethod.Invoke(myObject, Params);

                args.Result = value;
            }
            catch (Exception ex)
            {
                //TrackingLog.TrackingErrorToSQL("author: nguyenhn", ex.ToString());
            }
        }

        #endregion

        #region Formula Method

        #region Bool Method

        public object IF(bool condition, object trueValue, object falseValue)
        {
            return condition ? trueValue : falseValue;
        }

        public bool AND(params bool[] arguments)
        {
            var value = false;
            if (arguments == null || arguments.Length == 0) return value;
            value = true;
            foreach (var item in arguments) value = value && item;
            return value;
        }

        public bool OR(params bool[] arguments)
        {
            var value = false;
            if (arguments == null || arguments.Length == 0) return value;
            foreach (var item in arguments) value = value || item;
            return value;
        }

        public bool NOT(bool argument)
        {
            return !argument;
        }

        #endregion

        #region Number Method

        public long INT(object obj)
        {
            long value;
            if (string.IsNullOrEmpty(obj + string.Empty) || !long.TryParse(obj + string.Empty, out value))
                throw new Exception("Parse Int không thành công :" + JsonConvert.SerializeObject(obj));
            return value;
        }

        public double NUMBER(object obj)
        {
            double value;
            if (string.IsNullOrEmpty(obj + string.Empty) || !double.TryParse(obj + string.Empty, out value))
                throw new Exception("Parse NUMBER không thành công :" + JsonConvert.SerializeObject(obj));
            return value;
        }

        public double SUM(params object[] arguments)
        {
            double value = 0;
            if (arguments == null || arguments.Length == 0)
                throw new Exception("SUM không truyền tham số");
            foreach (var item in arguments) value += NUMBER(item);
            return value;
        }

        public double PRODUCT(params object[] arguments)
        {
            double value = 1;
            if (arguments == null || arguments.Length == 0)
                throw new Exception("PRODUCT không truyền tham số");
            foreach (var item in arguments) value *= NUMBER(item);
            return value;
        }

        public double MIN(params object[] arguments)
        {
            double value;
            if (arguments == null || arguments.Length == 0)
                throw new Exception("MIN không truyền tham số");
            value = NUMBER(arguments[0]);
            foreach (var item in arguments) value = Math.Min(value, NUMBER(item));
            return value;
        }

        public double MAX(params object[] arguments)
        {
            double value;
            if (arguments == null || arguments.Length == 0)
                throw new Exception("MAX không truyền tham số");
            value = NUMBER(arguments[0]);
            foreach (var item in arguments) value = Math.Max(value, NUMBER(item));
            return value;
        }

        public double COUNT(params object[] arguments)
        {
            if (arguments == null || arguments.Length == 0)
                throw new Exception("COUNT không truyền tham số");
            double value = 0;
            foreach (var item in arguments)
            {
                var dTest = NUMBER(item);
                value++;
            }

            return value;
        }

        public double POWER(params object[] arguments)
        {
            if (arguments == null || arguments.Length < 2)
                throw new Exception("POWER không truyền đủ tham số");
            return Math.Pow(NUMBER(arguments[0]), NUMBER(arguments[1]));
        }

        public double ROUND(object number, object precision = null)
        {
            var iPrecision = 0;
            if (!string.IsNullOrEmpty(precision + string.Empty))
                int.TryParse(precision + string.Empty, out iPrecision);

            return Math.Round(NUMBER(number), iPrecision);
        }

        public double ROUNDUP(object number, object precision = null)
        {
            var iPrecision = 0;
            if (!string.IsNullOrEmpty(precision + string.Empty))
                int.TryParse(precision + string.Empty, out iPrecision);
            var factor = Math.Pow(10, iPrecision);
            return Math.Ceiling(NUMBER(number) * factor) / factor;
        }

        public double ROUNDDOWN(object number, object precision = null)
        {
            var iPrecision = 0;
            if (!string.IsNullOrEmpty(precision + string.Empty))
                int.TryParse(precision + string.Empty, out iPrecision);
            var factor = Math.Pow(10, iPrecision);
            return Math.Floor(NUMBER(number) * factor) / factor;
        }

        #endregion

        #region DateTime Method

        public DateTime NOW()
        {
            return DateTime.Now;
        }

        public DateTime NOWDATE()
        {
            return DateTime.Now.Date;
        }

        public DateTime? DATE(params object[] arguments)
        {
            var value = default(DateTime?);
            var provider = CultureInfo.InvariantCulture;

            if (arguments == null || arguments.Length == 0 || arguments[0] == null) return value;
            switch (arguments[0])
            {
                case string _:
                    value = arguments.Length >= 2 ? DateTime.ParseExact(arguments[0] + string.Empty, arguments[1] + string.Empty, provider) : DateTime.Parse(arguments[0] + string.Empty);
                    break;
                case DateTime _:
                    value = (DateTime)arguments[0];
                    break;
                default:
                    {
                        if (arguments[0] is DateTime)
                        {
                            value = (DateTime?)arguments[0];
                        }
                        else
                        {
                            var number = new int[7];
                            for (var i = 0; i < 7; i++)
                                if (arguments.Length > i)
                                    number[i] = (int)NUMBER(arguments[i]);
                            value = new DateTime(number[0], number[1], number[2], number[3], number[4], number[5], number[6]);
                        }

                        break;
                    }
            }

            return value;
        }

        public DateTime? DATEONLY(params object[] arguments)
        {
            var myType = typeof(Formula);
            var dateMethod = myType.GetMethod("DATE");
            var myObject = Activator.CreateInstance(myType);
            var realParam = GetRealParams(dateMethod, arguments);

            if (dateMethod == null) return null;
            var dt = dateMethod.Invoke(myObject, realParam);
            var value = (DateTime?)dt;

            if (value.HasValue)
                value = new DateTime(value.Value.Year, value.Value.Month, value.Value.Day);

            return value;
        }

        public int? YEAR(object obj)
        {
            DateTime? dt = DATE(obj);
            return dt.HasValue ? dt.Value.Year : default(int?);
        }

        public int? MONTH(object obj)
        {
            DateTime? dt = DATE(obj);
            return dt.HasValue ? dt.Value.Month : default(int?);
        }

        public int? DAY(object obj)
        {
            DateTime? dt = DATE(obj);
            return dt.HasValue ? dt.Value.Day : default(int?);
        }

        public double? SUBDAY(object obj1, object obj2)
        {
            DateTime? dt1 = DATE(obj1);
            DateTime? dt2 = DATE(obj2);
            TimeSpan? ts = null;
            if (dt1.HasValue && dt2.HasValue)
            {
                ts = dt2.Value - dt1.Value;
            }
            return ts.HasValue ? ts.Value.TotalDays : default(double?);
        }

        public DateTime? ADDDAY(object obj, int days)
        {
            DateTime? dt = DATE(obj);
            return dt.HasValue ? dt.Value.AddDays(days) : default(DateTime?);
        }

        #endregion

        #region String Method

        public string TEXT(params object[] arguments)
        {
            if (arguments == null || arguments.Length == 0)
                throw new Exception("TEXt không truyền đủ tham số");
            if (arguments[0].GetType() == typeof(DateTime))
            {
                var dt = (DateTime)arguments[0];
                if (arguments.Length >= 2)
                {
                    var format = arguments[1].ToString();
                    return dt.ToString(format);
                }

                return dt.ToString();
            }

            if (arguments[0].GetType() == typeof(bool))
            {
                var bValue = (bool)arguments[0];
                //return bValue
                //    ? VuThao.Core.Base.Class.CmmFunc.GetResourceValue("Common", "TrueText")
                //    : VuThao.Core.Base.Class.CmmFunc.GetResourceValue("Common", "FalseText");
            }

            return arguments[0].ToString();
        }

        public int LENGTH(object obj)
        {
            return TEXT(obj).Length;
        }

        public string UPPER(object obj)
        {
            return TEXT(obj).ToUpper();
        }

        public string LOWER(object obj)
        {
            return TEXT(obj).ToLower();
        }

        public string PROPER(object obj)
        {
            var myTi = CultureInfo.CurrentCulture.TextInfo;
            return myTi.ToTitleCase(TEXT(obj));
        }

        public string CONCATENATE(params object[] arguments)
        {
            return string.Concat(arguments);
        }

        public bool EXACT(object left, object right)
        {
            return left.Equals(right);
        }

        public string LEFT(object obj, int iLenght)
        {
            return TEXT(obj).Substring(0, iLenght);
        }

        public string RIGHT(object obj, int iLenght)
        {
            var str = TEXT(obj);
            return str.Substring(str.Length - iLenght);
        }

        public string TRIM(object obj)
        {
            var str = TEXT(obj);
            return str.Trim();
        }

        public string REPT(object obj, int iTimes)
        {
            var str = TEXT(obj);
            var value = string.Empty;
            for (var i = 0; i < iTimes; i++) value += str;
            return value;
        }

        #endregion

        #region Common Method

        public bool ISNULL(object obj)
        {
            return obj == null;
        }

        #endregion

        #endregion

    }
}
