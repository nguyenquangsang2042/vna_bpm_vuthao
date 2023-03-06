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
using Refractored.Controls;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    class AdapterControlLinkedWorkflowListPagingNew : RecyclerView.Adapter
    {
        private MainActivity _mainAct;
        private Context _context;
        public event EventHandler<Tuple<BeanWorkflowItem, bool>> CustomItemClick_CheckedChangeItem; // Event khi user - check uncheck itemview
        private List<BeanWorkflowItem> _lstWorkflowItem = new List<BeanWorkflowItem>();
        private List<BeanWorkflowItem> _lstWorkflowItem_CheckedState = new List<BeanWorkflowItem>(); // list de luu lai nhung item da check
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        public AdapterControlLinkedWorkflowListPagingNew(Context context, MainActivity mainAct, List<BeanWorkflowItem> _lstWorkflowItem, List<BeanWorkflowItem> _lstWorkflowItem_CheckedState)
        {
            this._context = context;
            this._mainAct = mainAct;
            this._lstWorkflowItem = _lstWorkflowItem;
            this._lstWorkflowItem_CheckedState = _lstWorkflowItem_CheckedState;
        }
        private void OnItemClick(int _checkedPosition, bool _isChecked)
        {
            if (CustomItemClick_CheckedChangeItem != null)
            {
                Tuple<BeanWorkflowItem, bool> _res = new Tuple<BeanWorkflowItem, bool>(_lstWorkflowItem[_checkedPosition], _isChecked);
                CustomItemClick_CheckedChangeItem(this, _res);
            }
        }
        public void UpdateListData(List<BeanWorkflowItem> _lstWorkflowItem)
        {
            this._lstWorkflowItem = _lstWorkflowItem;
        }
        public void UpdateListintCheckedState(List<BeanWorkflowItem> _lstWorkflowItem_CheckedState)
        {
            this._lstWorkflowItem_CheckedState = _lstWorkflowItem_CheckedState;
        }
        public override int ItemCount => _lstWorkflowItem.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemControlLinkedWorkflowListDataNew, parent, false);
            ControlLinkedWorkflowListPagingNewHolder holder = new ControlLinkedWorkflowListPagingNewHolder(itemView, OnItemClick);
            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ControlLinkedWorkflowListPagingNewHolder vh = holder as ControlLinkedWorkflowListPagingNewHolder;

            if (_lstWorkflowItem_CheckedState != null)
            {
                if (_lstWorkflowItem_CheckedState.FindAll(x => x.ID.Equals(_lstWorkflowItem[position].ID)).ToList().Count > 0)
                {
                    vh._viewIsSelected.Visibility = ViewStates.Visible;
                    vh._imgIschecked.Visibility = ViewStates.Visible;
                    vh._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clControlBlueEnable)));
                    vh._tvTitle.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomEnable)));
                }
                else
                {
                    vh._viewIsSelected.Visibility = ViewStates.Invisible;
                    vh._imgIschecked.Visibility = ViewStates.Invisible;
                    vh._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));
                    vh._tvTitle.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
                }
            }

            if (!String.IsNullOrEmpty(_lstWorkflowItem[position].WorkflowTitle))
            {
                vh._tvTitle.Text = _lstWorkflowItem[position].WorkflowTitle;
            }
            else
            {
                vh._tvTitle.Text = "";
            }
            if (!String.IsNullOrEmpty(_lstWorkflowItem[position].ActionStatus))
            {
                vh._tvStatus.Visibility = ViewStates.Visible;
                vh._tvStatus.Text = _lstWorkflowItem[position].ActionStatus;
                vh._tvStatus.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(CTRLHomePage.GetColorByActionID(_context, _lstWorkflowItem[position].ActionStatusID.Value));
            }
            else
            {
                vh._tvStatus.Visibility = ViewStates.Gone;
            }
        }
        public class ControlLinkedWorkflowListPagingNewHolder : RecyclerView.ViewHolder
        {
            public LinearLayout _lnAll { get; set; }
            public View _viewIsSelected { get; set; }
            public TextView _tvAvatar { get; set; }
            public CircleImageView _imgAvatar { get; set; }
            public TextView _tvTitle { get; set; }
            public TextView _tvDescription { get; set; }
            public TextView _tvTime { get; set; }
            public TextView _tvStatus { get; set; }
            public ImageView _imgIschecked { get; set; }
            public ControlLinkedWorkflowListPagingNewHolder(View itemview, Action<int, bool> listener) : base(itemview)
            {
                _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlLinkedWorkflowListDataNew_All);
                _viewIsSelected = itemview.FindViewById<View>(Resource.Id.view_ItemControlLinkedWorkflowListDataNew_LeftSelected);
                _tvAvatar = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlLinkedWorkflowListDataNew_Avatar);
                _imgAvatar = itemview.FindViewById<CircleImageView>(Resource.Id.img_ItemControlLinkedWorkflowListDataNew_Avatar);
                _tvTitle = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlLinkedWorkflowListDataNew_Title);
                _tvDescription = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlLinkedWorkflowListDataNew_Description);
                _tvTime = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlLinkedWorkflowListDataNew_Time);
                _tvStatus = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlLinkedWorkflowListDataNew_Status);
                _imgIschecked = itemview.FindViewById<ImageView>(Resource.Id.img_ItemControlLinkedWorkflowListDataNew_Checked);
                _lnAll.Click += (sender, e) =>
                {
                    if (_imgIschecked.Visibility == ViewStates.Visible)
                    {
                        listener(base.LayoutPosition, false);
                    }
                    else
                    {
                        listener(base.LayoutPosition, true);
                    }
                };
            }
        }
    }
}