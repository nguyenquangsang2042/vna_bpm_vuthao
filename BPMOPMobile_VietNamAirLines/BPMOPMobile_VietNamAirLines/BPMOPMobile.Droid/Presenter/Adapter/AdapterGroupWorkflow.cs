using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Core.Controller;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    
    class AdapterGroupWorkflow : RecyclerView.Adapter
    {
        private MainActivity _mainAct;
        private Context _context;
        private Random rnd = new Random();
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        private ControllerBoard CTRLBoard = new ControllerBoard();
        private int _currentSelectedGroup = 0;
        public event EventHandler<int> CustomItemClick;
        private List<BeanWorkflowItem> _lstWorkflowItem = new List<BeanWorkflowItem>();
        public AdapterGroupWorkflow(Context context, MainActivity mainAct, List<BeanWorkflowItem> _lstWorkflowItem, int _currentSelectedGroup)
        {
            this._context = context;
            this._mainAct = mainAct;
            this._lstWorkflowItem = _lstWorkflowItem;
            this._currentSelectedGroup = _currentSelectedGroup;
        }
        public override int ItemCount => 6;
        private void OnItemClick(int obj)
        {
            CustomItemClick(this, obj);
        }
        public void UpdateCurrentSelectedGroup(int _currentSelectedGroup)
        {
            //int index = _lstWorkflowItem.FindIndex(x=>x.ID.Equals(_currentSelectedGroup.ID));
            this._currentSelectedGroup = _currentSelectedGroup;
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemPopupGroupWorkflow, parent, false);
            AdapterGroupWorkflowViewHolder holder = new AdapterGroupWorkflowViewHolder(itemView, OnItemClick);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AdapterGroupWorkflowViewHolder vh = holder as AdapterGroupWorkflowViewHolder;
            if (position == _currentSelectedGroup)
            {
                vh._imgClicked.Visibility = ViewStates.Visible;
                vh._tvTitle.SetTextColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clBottomEnable)));
            }
            else
            {
                vh._imgClicked.Visibility = ViewStates.Invisible;
                vh._tvTitle.SetTextColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clBottomDisable)));
            }

            vh._tvTitle.Text = "Tất cả " + position.ToString();

            //vh._lnContent.Click += (sender, e) => OnItemClick(_lstWorkflowItem[position]);
        }


        public class AdapterGroupWorkflowViewHolder : RecyclerView.ViewHolder
        {
            public ImageView _imgClicked { get; set; }
            public TextView _tvTitle { get; set; }

            public AdapterGroupWorkflowViewHolder(View itemview, Action<int> listener) : base(itemview)
            {
                _imgClicked = itemview.FindViewById<ImageView>(Resource.Id.img_ItemPopupGroupWorkflow);
                _tvTitle = itemview.FindViewById<TextView>(Resource.Id.img_ItemPopupGroupWorkflow_Name);
                itemview.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}