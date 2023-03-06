using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Class;
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    public class SharedView_PopupControlBase : SharedViewBase
    {
        public int flagView { get; set; }
        public ViewElement elementParent { get; set; }
        public ViewElement elementPopup { get; set; }  // Nếu là popup của control input grid detail mới xài cái này
        public JObject JObjectChild { get; set; } // Nếu là popup của control input grid detail mới xài cái này

        public SharedView_PopupControlBase(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
        {

        }

        /// <summary>
        /// Control động trên lưới Master
        /// </summary>
        public virtual void InitializeValue_Master(ViewElement elementParent)
        {
            flagView = (int)FlagViewControlDynamic.DetailWorkflow;

            this.elementParent = elementParent;
        }

        /// <summary>
        /// Control động trên popup lưới Detail
        /// </summary>
        public virtual void InitializeValue_InputGridDetail(ViewElement elementParent, ViewElement elementPopup, JObject JObjectChild)
        {
            flagView = (int)FlagViewControlDynamic.DetailWorkflow_InputGridDetail;

            this.elementParent = elementParent;
            this.elementPopup = elementPopup;
            this.JObjectChild = JObjectChild;
        }

        public virtual void InitializeValue_Master_CreateTask(ViewElement elementParent)
        {
            flagView = (int)FlagViewControlDynamic.DetailCreateTask;

            this.elementParent = elementParent;
        }

        public enum FlagViewControlDynamic
        {
            [Description("DetailWorkflow - Control")]
            DetailWorkflow = 0,

            [Description("DetailWorkflow - Control of popup InputGridDetail")]
            DetailWorkflow_InputGridDetail = 1,

            [Description("DetailCreateTask - Control")]
            DetailCreateTask = 2,
        }
    }
}