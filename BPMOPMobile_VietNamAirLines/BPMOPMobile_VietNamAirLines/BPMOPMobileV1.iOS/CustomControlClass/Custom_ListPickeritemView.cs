using System;
using System.Collections.Generic;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.CustomControlClass
{
    public class Custom_ListPickeritemView : UIView
    {
        public UIPickerView uIPicker;
        public List<ClassMenu> lst_items { get; set; }
        public UIViewController viewController { get; set; }
        public UITextField inputView { get; set; }
        public bool ItemNoIcon { get; set; }
        public int RowHeigth => 34;
        
        private void HandleItemSelect(ClassMenu _item)
        {
            if (viewController != null && viewController.GetType() == typeof(FormAddWorkRelatedView))
            {
                FormAddWorkRelatedView controller = (FormAddWorkRelatedView)viewController;
                controller.HandlePickerResult(_item);
            }
            //else if (viewController != null && viewController.GetType() == typeof(CreateNewTaskView))
            //{
            //    CreateNewTaskView controller = (CreateNewTaskView)viewController;
            //    controller.HandleMenuOptionResult(_menu);
            //}
        }

        private Custom_ListPickeritemView()
        {
            this.BackgroundColor = UIColor.White;

            uIPicker = new UIPickerView();
            //uIPicker.BackgroundColor = UIColor.LightGray.ColorWithAlpha(0.1f);
            uIPicker.BackgroundColor = UIColor.White;
            this.AddSubview(uIPicker);

        }

        private static Custom_ListPickeritemView instance = null;
        public static Custom_ListPickeritemView Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Custom_ListPickeritemView();
                }
                return instance;
            }
        }

        public void InitFrameView(CGRect frame)
        {
            this.Frame = frame;

            uIPicker.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
        }

        public void PickerLoadData()
        {
            
            var pickermodel = new PickerItems_ModelSource(lst_items, this);
            uIPicker.Model = pickermodel;
        }

        #region custom views
        #region table data source user
        private class PickerItems_ModelSource : UIPickerViewModel
        {
            List<ClassMenu> lst_item;
            NSString cellIdentifier = new NSString("cellMenuOption");
            Custom_ListPickeritemView parentView;

            public PickerItems_ModelSource(List<ClassMenu> _items, Custom_ListPickeritemView _parentview)
            {
                parentView = _parentview;
                lst_item = _items;
            }

            public override nint GetComponentCount(UIPickerView pickerView)
            {
                //return (lst_item != null) ? lst_item.Count : 0;
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                return lst_item.Count;
            }

            public override nfloat GetComponentWidth(UIPickerView picker, nint component)
            {
                if (component == 0)
                    return 240f;
                else
                    return 40f;
            }

            public override nfloat GetRowHeight(UIPickerView picker, nint component)
            {
                return 40f;
            }

            public override void Selected(UIPickerView pickerView, nint row, nint component)
            {
                var itemSelected = lst_item[(int)row];
                parentView.HandleItemSelect(itemSelected);
            }

            public override string GetTitle(UIPickerView pickerView, nint row, nint component)
            {
                var item = lst_item[(int)row];
                return item.title;
            }
        }
        #endregion
        #endregion
    }
}
