using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.Components;
using BPMOPMobileV1.iOS.IOSClass;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIKit;
using VuThao.Calc;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class FormWFDetailsProperty : UIViewController
    {
        ViewRow dataObject { get; set; }
        JObject jObject { get; set; }
        UIViewController parentView { get; set; }
        ViewElement element; //gridElement
        int itemIndexSelected = 0;
        bool isAddnew;
        //ViewElement itemChoiceElement { get; set; }
        public FormWFDetailsProperty(IntPtr handle) : base(handle)
        {
        }

        #region override
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewConfiguration();
            LoadContent();
            //CalculatorFomular();

            #region delegate
            BT_delete.TouchUpInside += BT_delete_TouchUpInside;
            BT_accept.TouchUpInside += BT_accept_TouchUpInside;
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            #endregion

            // Perform any additional setup after loading the view, typically from a nib.
        }

        #endregion

        #region private - public method
        public void SetContent(ViewElement _element, ViewRow _dataObject, JObject _jObject, BeanWorkflowItem workflowItem, int _itemIndex, UIViewController _parentView, bool _isAddnew)
        {
            itemIndexSelected = _itemIndex;
            //dynamic control
            element = _element;
            //item choice from element dynamic
            jObject = _jObject;
            //jObject dang viewrow - 5 elements con - 5row
            dataObject = _dataObject;
            parentView = _parentView;
            isAddnew = _isAddnew;
        }

        private void ViewConfiguration()
        {
            BT_delete.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_accept.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            if (!element.Enable)
            {
                BT_delete.Hidden = true;
                BT_accept.Hidden = true;

            }
            else
            {
                if (isAddnew)
                    BT_delete.Hidden = true;
            }

            //BT_close.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
        }

        private void LoadContent()
        {
            try
            {
                lbl_title.Text = element.Title;
                //itemChoiceElement = null;
                if (!isAddnew)
                {
                    table_content.Source = new Control_TableSource(dataObject, this);
                    table_content.ReloadData();
                }
                else
                {
                    table_content.Source = new Control_TableSource(dataObject, this);
                    table_content.ReloadData();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("FormWFDetailsProperty - LoadContent - Err: " + ex.ToString());
            }
        }

        #region fomular
        private void CalculatorFomular(ViewElement _itemElement)
        {
            try
            {
                jObject[_itemElement.InternalName] = Convert.ToDouble(_itemElement.Value);

                string _formula = "[SoLuong]*[DonGia]"; /*[TongTien]=*/
                if (!string.IsNullOrEmpty(_formula))
                {
                    object temp = calcObject(_formula, jObject);

                    if (temp != null)
                    {
                        JToken value = JToken.Parse(temp.ToString());

                        jObject["TongTien"] = value;
                        List<ViewElement> lst_element = new List<ViewElement>();
                        lst_element = dataObject.Elements;

                        // chua ro tam khoa
                        //element = lst_element.Where(w => w.internalName == "TongTien").FirstOrDefault();
                        //element.Value = value.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("FormWFDetailsProperty - CalculatorFomular - Err: " + ex.ToString());

            }
        }

        public object calcObject(string extendCondition, object objData)
        {
            try
            {
                Expression exp = new Expression(extendCondition);
                exp.EvaluateFunction += Exp_EvaluateFunction;
                List<string> lstParaName = GetParaInStr(extendCondition);
                if (lstParaName != null && lstParaName.Count > 0)
                {
                    foreach (string paraName in lstParaName)
                    {
                        object fieldValue = GetPropertyValueByName(objData, paraName);
                        if (!exp.Parameters.ContainsKey(paraName))
                            exp.Parameters.Add(paraName, fieldValue);
                    }
                }

                return exp.Evaluate();
            }
            catch (Exception ex)
            {
                string a = "Formula Calc :" + extendCondition;
                return null;
                //throw new Exception("Formula Calc :" + extendCondition, ex);
            }
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
                Console.WriteLine("FormWFDetailsProperty - Exp_EvaluateFunction - Err: " + ex.ToString());
            }
        }

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

        public object GetPropertyValueByName(object obj, string key)
        {
            object retValue = null;
            Type type = obj.GetType();

            if (type == typeof(JObject))
            {
                retValue = ((JObject)obj)[key].ToObject<object>();
            }
            else
            {
                PropertyInfo perInfo = type.GetProperty(key);
                if (perInfo != null)
                {
                    retValue = perInfo.GetValue(obj);
                }
            }
            return retValue;
        }

        public static List<string> GetParaInStr(string strInput)
        {
            List<string> retValue = new List<string>();

            Regex rgx = new System.Text.RegularExpressions.Regex(@"\[(\d|\w|\.|_)+\]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(strInput);
            if (matches.Count <= 0) return retValue;
            foreach (Match match in matches)
            {
                string itemF = match.Value.Replace("[", "");
                itemF = itemF.Replace("]", "");
                if (!retValue.Contains(itemF))
                    retValue.Add(itemF);
            }
            return retValue;
        }
        #endregion

        #region ControlSelectUser
        public void HandleUserOrGroupMultiChoiceSelected(ViewElement _itemElement, List<BeanUserAndGroup> _userSelected)
        {
            List<BeanUserAndGroup> lst_userChoice = _userSelected;

            if (lst_userChoice != null && lst_userChoice.Count > 0)
            {
                foreach (var item in lst_userChoice)
                {
                    item.Name = item.Name;
                }
            }

            var jsonString = JsonConvert.SerializeObject(lst_userChoice);
            _itemElement.Value = jsonString;
            //change value
            ChangeValueElement(_itemElement);
            table_content.ReloadData();
        }
        public void NavigateToUserOrGroupChoiceView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            if (element != null)
            {
                List<BeanUser> lst_data = new List<BeanUser>();

                ListUserOrGroupView listUserOrGroup = (ListUserOrGroupView)Storyboard.InstantiateViewController("ListUserOrGroupView");
                if (element.DataType == "selectusergroup")
                    listUserOrGroup.setContent(this, false, null, false, element, element.Title);
                else if (element.DataType == "selectusergroupmulti")
                    listUserOrGroup.setContent(this, true, null, false, element, element.Title);
                this.NavigationController.PushViewController(listUserOrGroup, true);
            }
            else
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", "Không tìm thấy thông tin phiếu");
            }
        }
        public void HandleUserOrGroupSingleChoiceSelected(ViewElement _itemElement, BeanUserAndGroup _userSelected)
        {
            List<BeanUserAndGroup> lst_userChoice = new List<BeanUserAndGroup>();
            if (_userSelected != null)
                lst_userChoice.Add(_userSelected);
            var jsonString = JsonConvert.SerializeObject(lst_userChoice);
            _itemElement.Value = jsonString;
            //changevalue
            ChangeValueElement(_itemElement);

            table_content.ReloadData();
        }

        public void NavigatorToUserChoiceView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            if (element != null)
            {
                List<BeanUser> lst_data = new List<BeanUser>();

                ListUserView listUserView = (ListUserView)Storyboard.InstantiateViewController("ListUserView");
                if (element.DataType == "selectuser")
                    listUserView.setContent(this, false, null, false, element, element.Title);
                else if (element.DataType == "selectusermulti")
                    listUserView.setContent(this, true, null, false, element, element.Title);
                this.NavigationController.PushViewController(listUserView, true);
            }
            else
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", "Không tìm thấy thông tin phiếu");
            }
        }
        public void HandleUserMultiChoiceSelected(ViewElement _itemElement, List<BeanUser> _userSelected)
        {
            List<BeanUser> lst_userChoice = _userSelected;
            if (lst_userChoice != null && lst_userChoice.Count > 0)
            {
                foreach (var item in lst_userChoice)
                {
                    item.Name = item.FullName;
                }
            }

            string jsonString = string.Empty;
            if (lst_userChoice != null && lst_userChoice.Count > 0)
                jsonString = JsonConvert.SerializeObject(lst_userChoice);

            _itemElement.Value = jsonString;
            ChangeValueElement(_itemElement);
            table_content.ReloadData();
        }

        private void ChangeValueElement(ViewElement _itemElement)
        {
            //changevalue
            //itemChoiceElement = _itemElement;
            if (isAddnew)
            {
                foreach (var item in dataObject.Elements)
                {
                    if (item.InternalName == _itemElement.InternalName)
                    {
                        item.Value = _itemElement.Value;
                    }
                }
            }
            else if (element.Value.Contains("[{"))
            {
                foreach (var item in jObject)
                {
                    if (item.Key.Contains(_itemElement.InternalName))
                        jObject[_itemElement.InternalName] = _itemElement.Value;
                }
                /*
                var lstProps = jObject.Properties();
                foreach (JProperty property in lstProps)
                {
                    if (string.Compare(property.Name, itemChoiceElement.InternalName) == 0)
                        property.Value = _itemElement.Value;
                }*/
            }
            else
            {
                //JToken value = JToken.Parse(temp.ToString());

                //jObject["TongTien"] = value;
                //List<ViewElement> lst_element = new List<ViewElement>();
                //lst_element = dataObject.Elements;
            }
        }
        public void HandleUserSingleChoiceSelected(ViewElement _itemElement, BeanUser _userSelected)
        {
            List<BeanUser> lst_userChoice = new List<BeanUser>();
            if (_userSelected != null)
                lst_userChoice.Add(_userSelected);

            string jsonString = string.Empty;
            if (lst_userChoice != null && lst_userChoice.Count > 0)
                jsonString = JsonConvert.SerializeObject(lst_userChoice);

            _itemElement.Value = jsonString;
            //changevalue
            ChangeValueElement(_itemElement);
            table_content.ReloadData();
        }
        #endregion

        #region handle input text
        public void NavigatorToEditTextView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            TextViewControlView textViewControlView = (TextViewControlView)Storyboard.InstantiateViewController("TextViewControlView");
            textViewControlView.setContent(this, 1, element);
            this.NavigationController.PushViewController(textViewControlView, true);

        }
        public void HandleSingleLine(ViewElement _element)
        {
            ChangeValueElement(_element);
            table_content.ReloadData();
        }
        public void NavigatorTouchMoreToFullTextView(string title, string value)
        {
            FullTextView fullTextView = (FullTextView)Storyboard.InstantiateViewController("FullTextView");
            fullTextView.SetContent(title, value);
            this.NavigationController.PushViewController(fullTextView, true);
        }

        public void NavigatorToFullTextView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            FullTextView fullTextView = (FullTextView)Storyboard.InstantiateViewController("FullTextView");
            fullTextView.SetContent(element.Title, element.Value);
            this.NavigationController.PushViewController(fullTextView, true);
        }
        #endregion

        #region handle number - currence
        public void NavigatorToEditNumberView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            NumberControlView numberControlView = (NumberControlView)Storyboard.InstantiateViewController("NumberControlView");
            numberControlView.setContent(this, 1, element);

            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            numberControlView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            numberControlView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            numberControlView.TransitioningDelegate = transitioningDelegate;
            this.NavigationController.PushViewController(numberControlView, true);
        }
        public void HandleEditNumber(ViewElement _itemElement)
        {
            CalculatorFomular(_itemElement);
            table_content.ReloadData();
        }
        #endregion

        #region handle DateTime choice
        public void HandleDateTimeChoiceChoice(ViewElement _itemElement)
        {
            //change value
            ChangeValueElement(_itemElement);
            table_content.ReloadData();
        }
        public void NavigatorToDateTimeChoice(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {

            if (element != null)
            {
                PresentationDelegate transitioningDelegate;
                CGRect startFrame = new CGRect(this.View.Frame.Width / 2, this.View.Frame.Height / 2, 0, 0);
                CGSize showSize = new CGSize(384, 450);

                FormCalendarChoice formCalendarChoice = (FormCalendarChoice)Storyboard.InstantiateViewController("FormCalendarChoice");
                formCalendarChoice.setContent(this, element);
                transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                formCalendarChoice.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                formCalendarChoice.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                formCalendarChoice.TransitioningDelegate = transitioningDelegate;
                this.PresentViewControllerAsync(formCalendarChoice, true);
            }
            else
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", "Không tìm thấy thông tin phiếu");
            }
        }
        #endregion

        #region Handle_YesNoResult
        public void Handle_YesNoResult(ViewElement _itemElement)
        {
            //change value
            ChangeValueElement(_itemElement);
            table_content.ReloadData();
        }
        #endregion

        #region handle choice
        public void HandleChoiceSelected(ViewElement _itemElement)
        {
            try
            {
                //change value
                ChangeValueElement(_itemElement);
                table_content.ReloadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FormWFDetailsProperty - HandleChoiceSelected - Err: " + ex.ToString());
            }
        }
        public void NavigatorToChoiceView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            if (element != null)
            {
                MultiChoiceView itemsChoiceView = (MultiChoiceView)Storyboard.InstantiateViewController("MultiChoiceView");
                if (element.DataType == "singlechoice")
                    itemsChoiceView.setContent(this, false, element);
                else if (element.DataType == "multiplechoice")
                    itemsChoiceView.setContent(this, true, element);


                //CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                //CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
                //PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                //itemsChoiceView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                //itemsChoiceView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                //itemsChoiceView.TransitioningDelegate = transitioningDelegate;
                this.NavigationController.PushViewController(itemsChoiceView, true);

            }
            else
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", "Không tìm thấy thông tin phiếu");
            }
        }
        #endregion
        #endregion

        #region events
        private void BT_delete_TouchUpInside(object sender, EventArgs e)
        {

            UIAlertController alert = UIAlertController.Create("Thông báo", CmmFunction.GetTitle("TEXT_DELETE_CONFIRM_DETAILS", "Bạn có chắc muốn xóa chi tiết này không?"), UIAlertControllerStyle.Alert);//"BPM"
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_DELETE", "Xóa"), UIAlertActionStyle.Default, alertAction =>
            {
                alert_confirm_delete_click();
            }));
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"), UIAlertActionStyle.Default, null));
            this.PresentViewController(alert, true, null);
        }

        public void alert_confirm_delete_click()
        {
            if (parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 Parentview = parentView as RequestDetailsV2;
                if (!string.IsNullOrEmpty(element.Value) && element.Value != "[]")
                {
                    JArray rowListItem = JArray.Parse(element.Value);
                    rowListItem.RemoveAt(itemIndexSelected);

                    element.Value = JsonConvert.SerializeObject(rowListItem);

                    Parentview.HandlePropertyDetailsRemove(jObject);
                    this.NavigationController.PopViewController(true);
                }
            }
        }

        private void BT_accept_TouchUpInside(object sender, EventArgs e)
        {
            try
            {
                //kiem tra item requier nhung chua duoc nhap
                int i = 0;
                foreach (var item in dataObject.Elements)
                {
                    if (item.IsRequire && string.IsNullOrEmpty(item.Value))
                    {
                        CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_FIELD_REQUIRE", "Vui lòng nhập đầy đủ thông tin."));
                        return;
                    }
                }

                if (isAddnew)
                {
                    JArray rowListItem = new JArray();
                    JObject ob = null;

                    //ViewElement elementID = new ViewElement();
                    //elementID.ID = null;
                    //elementID.InternalName = "";
                    //elementID.Value = "";
                    //dataObject.Elements.Add(elementID);
                    var lst_element = JsonConvert.SerializeObject(dataObject.Elements);

                    if (element.Value == "[]")
                    {
                        jObject = new JObject();
                        i = 0;
                        foreach (var item in dataObject.Elements)
                        {
                            jObject.Add(item.InternalName, dataObject.Elements[i].Value);
                            i++;
                        }

                        rowListItem.Add(jObject);
                        element.Value = JsonConvert.SerializeObject(rowListItem);
                    }
                    else
                    {
                        rowListItem = JArray.Parse(element.Value);
                        //ob = (JObject)rowListItem[0];
                        jObject = new JObject();

                        i = 0;
                        foreach (var item in dataObject.Elements)
                        {
                            jObject.Add(item.InternalName, dataObject.Elements[i].Value);
                            i++;
                        }
                        rowListItem.Add(jObject);
                        element.Value = JsonConvert.SerializeObject(rowListItem);
                    }
                }
                else
                {
                    JArray rowListItem = new JArray();
                    rowListItem = JArray.Parse(element.Value);
                    rowListItem[itemIndexSelected] = jObject;

                    element.Value = JsonConvert.SerializeObject(rowListItem);
                }

                if (parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 Parentview = parentView as RequestDetailsV2;
                    Parentview.HandleChoiceSelected(element);
                }
                //else if (parentView.GetType() == typeof(WorkflowDetailView))
                //{
                //    WorkflowDetailView Parentview = parentView as WorkflowDetailView;
                //    Parentview.HandleChoiceSelected(element);
                //}
                //else if (parentView.GetType() == typeof(FormWorkFlowDetails))
                //{
                //    FormWorkFlowDetails Parentview = parentView as FormWorkFlowDetails;
                //    Parentview.HandleChoiceSelected(element);
                //}

                this.NavigationController.PopViewController(true);


            }
            catch (Exception ex)
            {
                Console.WriteLine("FormWFDetailsProperty - BT_Accept_TouchInside - Err: " + ex.ToString());
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("K_Mess_ActionFalse", "Thao tác không thực hiện được, vui lòng thử lại."));
            }

        }

        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopViewController(true);
        }
        #endregion

        #region custom
        #region dynamic controls source table
        private class Control_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cell");
            FormWFDetailsProperty parentView;
            ViewRow viewRow;

            //tam an session
            int heightHeader = 1;

            public Control_TableSource(ViewRow _viewRow, FormWFDetailsProperty _parentview)
            {
                viewRow = _viewRow;
                parentView = _parentview;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return 1;
            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                var tiem = viewRow.Elements[indexPath.Row];
                nfloat heightTemp = 0;
                switch (tiem.DataType)
                {
                    case "textinputmultiline":
                        {
                            string value = CmmFunction.StripHTML(tiem.Value);
                            var height_ets = StringExtensions.StringRect(value, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), tableView.Frame.Width - 20);
                            if (height_ets.Height < 55)//52
                            {
                                if (height_ets.Height > 30)
                                    heightTemp = 85;
                                else
                                    heightTemp = 75;
                            }
                            else
                                heightTemp = 115;
                        }
                        break;
                    default:
                        if (viewRow.Elements[indexPath.Row].InternalName == null)
                            heightTemp = 0;
                        else
                            heightTemp = 60;
                        break;
                }
                return heightTemp;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                if (viewRow.Elements != null)
                    return viewRow.Elements.Count();
                else
                    return 0;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                Control_cell_custom cell = new Control_cell_custom(parentView, cellIdentifier, viewRow.Elements[indexPath.Row], indexPath);
                return cell;
            }
        }
        private class Control_cell_custom : UITableViewCell
        {
            FormWFDetailsProperty parentView { get; set; }
            ViewElement element { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            public ComponentBase components;

            public Control_cell_custom(FormWFDetailsProperty _parentView, NSString cellID, ViewElement _element, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
            {
                parentView = _parentView;
                element = _element;
                currentIndexPath = _currentIndexPath;
                Accessory = UITableViewCellAccessory.None;
                viewConfiguration();
            }

            private void viewConfiguration()
            {
                ContentView.BackgroundColor = UIColor.White;

                components = new TemplateValueType(parentView, element, currentIndexPath);
                ContentView.Add(components);
                loadData();
            }

            public void loadData()
            {
                try
                {
                    components.SetTitle();
                    components.SetValue();
                    components.SetEnable();
                    components.SetProprety();
                    components.SetRequire();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("RequestDetailsView - Control_cell_custom - loaddata- ERR: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                components.InitializeFrameView(new CGRect(18, 0, ContentView.Frame.Width - 36, ContentView.Frame.Height));
            }
        }
        #endregion
        #endregion
    }
}