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

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    public class SharedView_PopupActionBase : SharedViewBase
    {
        public int flagView { get; set; }
        public ButtonAction buttonAction { get; set; }
        public List<ButtonAction> lstActionMore { get; set; }

        public SharedView_PopupActionBase(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
        {

        }

        public virtual void InitializeValue_DetailWorkflow(ButtonAction buttonAction)
        {
            flagView = (int)FlagViewControlAction.DetailWorkflow;

            this.buttonAction = buttonAction;
        }

        public virtual void InitializeValue_DetailWorkflow_ActionMore(List<ButtonAction> lstActionMore)
        {
            flagView = (int)FlagViewControlAction.DetailWorkflow;

            this.lstActionMore = lstActionMore;
        }


        public enum FlagViewControlAction
        {
            [Description("DetailWorkflow")]
            DetailWorkflow = 0
        }

    }
}