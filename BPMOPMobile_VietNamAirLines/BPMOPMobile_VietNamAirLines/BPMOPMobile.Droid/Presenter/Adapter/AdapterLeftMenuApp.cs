using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    public class AdapterLeftMenuApp : RecyclerView.Adapter
    {
        public AppCompatActivity _mainAct;
        public Context _context;
        public List<BeanWorkflow> _lstWorkflow = new List<BeanWorkflow>();
        public event EventHandler<BeanWorkflow> CustomItemClick;
        public AdapterLeftMenuApp(AppCompatActivity _mainAct, Context _context, List<BeanWorkflow> _lstWorkflow)
        {
            this._mainAct = _mainAct;
            this._context = _context;
            this._lstWorkflow = _lstWorkflow;
        }
        private void OnItemClick(int position)
        {
            if (CustomItemClick != null)
                CustomItemClick(this, _lstWorkflow[position]);
        }


        public override int ItemCount => _lstWorkflow.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemLeftMenuApp, parent, false);
            AdapterLeftMenuAppHolder holder = new AdapterLeftMenuAppHolder(itemView, OnItemClick);
            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                AdapterLeftMenuAppHolder _holder = holder as AdapterLeftMenuAppHolder;
                BeanWorkflow _currentWorkflowItem = _lstWorkflow[position];

                if (!String.IsNullOrEmpty(_currentWorkflowItem.ImageURL))
                    CmmDroidFunction.SetAvataByImagePath2(_mainAct, _currentWorkflowItem.ImageURL, _holder._img, 50);
                else
                    _holder._img.Visibility = ViewStates.Invisible;

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    _holder._tvTitle.Text = !String.IsNullOrEmpty(_currentWorkflowItem.Title) ? _currentWorkflowItem.Title : "";
                else
                    _holder._tvTitle.Text = !String.IsNullOrEmpty(_currentWorkflowItem.TitleEN) ? _currentWorkflowItem.TitleEN : "";
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnBindViewHolder", ex);
#endif
            }

        }

        public class AdapterLeftMenuAppHolder : RecyclerView.ViewHolder
        {
            public LinearLayout _lnAll { get; set; }
            public TextView _tvTitle { get; set; }
            public ImageView _img { get; set; }
            public AdapterLeftMenuAppHolder(View itemview, Action<int> listener) : base(itemview)
            {
                _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemLeftMenuApp_All);
                _tvTitle = itemview.FindViewById<TextView>(Resource.Id.tv_ItemLeftMenuApp_Title);
                _img = itemview.FindViewById<ImageView>(Resource.Id.img_ItemLeftMenuApp);
                _lnAll.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}