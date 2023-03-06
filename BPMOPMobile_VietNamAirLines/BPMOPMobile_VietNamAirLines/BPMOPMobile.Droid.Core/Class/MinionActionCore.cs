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
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Core.Class
{
    public class MinionActionCore
    {
        // Event khi click vào control Form trang Detail
        public static event EventHandler<ElementFormClick> ElementFormClickEvent;
        public class ElementFormClick : EventArgs
        {
            public ViewElement element { get; set; }
            public ElementFormClick(ViewElement element)
            {
                this.element = element;
            }
        }
        public static void OnElementFormClick(object sender, ElementFormClick e)
        {
            if (ElementFormClickEvent != null)
            {
                ElementFormClickEvent(sender, e);
            }
        }

        //------------------------------------------------------------------------------------------------------------------
        // Event khi click vào control Form trang Detail - nhưng có thêm Action nhỏ bên trong 
        // VD:  control input file có thêm nút xóa, thêm, tạo mới

        public static event EventHandler<ElementFormClick_WithInnerAction> ElementFormClickEvent_WithInnerAction;
        public class ElementFormClick_WithInnerAction : EventArgs
        {
            public ViewElement element { get; set; }
            public int actionID { get; set; } // Kiểu Enum - int của Action sẽ thực hiện VD: 1 - thêm, 2 - sửa, ...
            public int positionToAction { get; set; } // Item nào sẽ thực hiện trong list (nếu có) VD: item 1 trên 3 (-1 là không có)
            public int flagViewID { get; set; } // thực hiện ở view nào
            public ElementFormClick_WithInnerAction(ViewElement element, int actionID, int positionToAction, int flagViewID)
            {
                this.element = element;
                this.actionID = actionID;
                this.positionToAction = positionToAction;
                this.flagViewID = flagViewID;
            }
        }
        public static void OnElementFormClickEvent_WithInnerAction(object sender, ElementFormClick_WithInnerAction e)
        {
            if (ElementFormClickEvent_WithInnerAction != null)
            {
                ElementFormClickEvent_WithInnerAction(sender, e);
            }
        }
      
        //------------------------------------------------------------------------------------------------------------------
        // Event khi click vào control Button Bot trang Detail

        public static event EventHandler<ElementActionClick> ElementActionClickEvent;
        public class ElementActionClick : EventArgs
        {
            public ViewElement element { get; set; }
            public ElementActionClick(ViewElement element)
            {
                this.element = element;
            }
        }
        public static void OnElementActionClick(object sender, ElementActionClick e)
        {
            if (ElementActionClickEvent != null)
            {
                ElementActionClickEvent(sender, e);
            }
        }

        //------------------------------------------------------------------------------------------------------------------
        // Event khi click vào control Grid -> mở ra popup gồm các trường theo Header -> click vào item bên trong nữa

        public static event EventHandler<ElementGridChildActionClick> ElementGridChildActionClickEvent;
        public class ElementGridChildActionClick : EventArgs
        {
            public ViewElement elementParent { get; set; }
            public ViewElement elementChild { get; set; }
            public JObject jObjectChild { get; set; }
            public int flagView { get; set; }
            public ElementGridChildActionClick(ViewElement elementParent, ViewElement elementChild, JObject jObjectChild, int flagView)
            {
                this.elementParent = elementParent;
                this.elementChild = elementChild;
                this.jObjectChild = jObjectChild;
                this.flagView = flagView;
            }
        }
        public static void OnElementGridChildActionClick(object sender, ElementGridChildActionClick e)
        {
            if (ElementGridChildActionClickEvent != null)
            {
                ElementGridChildActionClickEvent(sender, e);
            }
        }

        //------------------------------------------------------------------------------------------------------------------
        // Event khi click vào Công việc liên kết trang Detail - nhưng có thêm Action nhỏ bên trong 
        // VD:  control input file có thêm nút xóa, thêm, tạo mới

        public static event EventHandler<FlowRelatedClick_WithInnerAction> FlowRelatedClickEvent_WithInnerAction;
        public class FlowRelatedClick_WithInnerAction : EventArgs
        {
            public List<BeanWorkFlowRelated> lstWorkflowRelated { get; set; }
            public int actionID { get; set; } // Kiểu Enum - int của Action sẽ thực hiện VD: 1 - thêm, 2 - sửa, ...
            public int positionToAction { get; set; } // Item nào sẽ thực hiện trong list (nếu có) VD: item 1 trên 3 (-1 là không có)
            public FlowRelatedClick_WithInnerAction(List<BeanWorkFlowRelated> lstWorkflowRelated, int actionID, int positionToAction)
            {
                this.lstWorkflowRelated = lstWorkflowRelated;
                this.actionID = actionID;
                this.positionToAction = positionToAction;
            }
        }
        public static void OnFlowRelatedClick_WithInnerAction(object sender, FlowRelatedClick_WithInnerAction e)
        {
            if (FlowRelatedClickEvent_WithInnerAction != null)
            {
                FlowRelatedClickEvent_WithInnerAction(sender, e);
            }
        }
        //------------------------------------------------------------------------------------------------------------------
        // Event khi click vào Item của component Task

        public class TaskListItemClick : EventArgs
        {
            public int _viewID { get; set; } // 1 là View chi tiết - 2 là chi tiết tạo mới
            public BeanExpandTask _clickedItem { get; set; }
            public TaskListItemClick(int _viewID, BeanExpandTask _clickedItem)
            {
                this._viewID = _viewID;
                this._clickedItem = _clickedItem;
            }
        }

        //------------------------------------------------------------------------------------------------------------------

        public static event EventHandler<ElementAttachFileClick> ElementClickAttachmentDetailWorkflow;
        public class ElementAttachFileClick : EventArgs
        {
            public ViewElement element { get; set; }
            public KeyValuePair<string, string> attachment;
            public ElementAttachFileClick(ViewElement element, KeyValuePair<string, string> attachment)
            {
                this.element = element;
                this.attachment = attachment;
            }
        }
        public static void OnElementClickAttachmentDetailWorkflow(object sender, ElementAttachFileClick e)
        {
            if (ElementClickAttachmentDetailWorkflow != null)
            {
                ElementClickAttachmentDetailWorkflow(sender, e);
            }
        }



        //------------------------------------------------------------------------------------------------------------------

        public static event EventHandler<ElementImportFileClick> ElementClickImportFileDetailCreateWorkflow;
        public class ElementImportFileClick : EventArgs
        {
            public Android.Net.Uri uri { get; set; }
            public ElementImportFileClick(Android.Net.Uri uri)
            {
                this.uri = uri;
            }
        }
        public static void OnElementClickImportFileDetailCreateWorkflow(object sender, ElementImportFileClick e)
        {
            if (ElementClickImportFileDetailCreateWorkflow != null)
            {
                ElementClickImportFileDetailCreateWorkflow(sender, e);
            }
        }

        //------------------------------------------------------------------------------------------------------------------

        public static event EventHandler<ElementImportFileClick_Action> ElementClickImportFileDetailCreateWorkflow_Action;
        public class ElementImportFileClick_Action : EventArgs
        {
            public object[] Args { get; set; }
            public ElementImportFileClick_Action(params object[] Args)
            {
                this.Args = Args;
            }
        }
        public static void OnElementClickImportFileDetailCreateWorkflow_Action(object sender, ElementImportFileClick_Action e)
        {
            if (ElementClickImportFileDetailCreateWorkflow_Action != null)
            {
                ElementClickImportFileDetailCreateWorkflow_Action(sender, e);
            }
        }

        //------------------------------------------------------------------------------------------------------------------

        public static event EventHandler<ActivityResult> ActivityResultEvent;
        public class ActivityResult : EventArgs
        {
            public int requestCode { get; set; }
            public Result resultCode { get; set; }
            public Intent data { get; set; }
            public int viewID { get; set; }
            public ActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data, int viewID)
            {
                this.requestCode = requestCode;
                this.resultCode = resultCode;
                this.data = data;
                this.viewID = viewID;
            }
        }
        public static void OnActivityResultEvent(object sender, ActivityResult e)
        {
            if (ActivityResultEvent != null)
            {
                ActivityResultEvent(sender, e);
            }
        }

        //------------------------------------------------------------------------------------------------------------------

        public class CommentEventArgs : EventArgs
        {
            public string _content { get; set; }
            public List<BeanAttachFile> _lstAttachFile { get; set; }
            public CommentEventArgs(string _content, List<BeanAttachFile> _lstAttachFile)
            {
                this._content = _content;
                this._lstAttachFile = _lstAttachFile;
            }
        }
    }
}