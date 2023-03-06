using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    public class AdapterExpandCreateWorkflow_Main : BaseExpandableListAdapter
    {
        private MainActivity _mainAct;
        private Context _context;
        public event EventHandler<BeanWorkflow> CustomItemClickChild;
        private List<BeanBoardWorkflow> _lstBoardWorkflow = new List<BeanBoardWorkflow>();

        private void OnItemClickChild(object sender, BeanWorkflow e)
        {
            if (CustomItemClickChild != null)
                CustomItemClickChild(this, e);
        }
        public void UpdateListData(List<BeanBoardWorkflow> newList)
        {
            this._lstBoardWorkflow = newList;
        }
        public List<BeanBoardWorkflow> GetListData()
        {
            return _lstBoardWorkflow;
        }
        public void UpdateItemFavorite(BeanWorkflow itemUpdate)
        {
            try
            {
                for (int i = 0; i < _lstBoardWorkflow.Count; i++)
                    for (int j = 0; j < _lstBoardWorkflow[i].lstBeanWorkflow.Count; j++)
                        if (_lstBoardWorkflow[i].lstBeanWorkflow[j].WorkflowID == itemUpdate.WorkflowID)
                        {
                            _lstBoardWorkflow[i].lstBeanWorkflow[j] = itemUpdate;
                            break;
                        }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetListData", ex);
#endif
            }
        }
        public AdapterExpandCreateWorkflow_Main(MainActivity _mainAct, Context _context, List<BeanBoardWorkflow> _lstBoardWorkflow)
        {
            this._context = _context;
            this._mainAct = _mainAct;
            this._lstBoardWorkflow = _lstBoardWorkflow;
        }
        public override int GroupCount => _lstBoardWorkflow.Count;

        public override bool HasStableIds => false;

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            return _lstBoardWorkflow[groupPosition].lstBeanWorkflow.Count;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            LayoutInflater mInflater = LayoutInflater.From(_context);

            View _rootView = convertView;
            RecyclerView _recyChild;
            if (_rootView == null)
            {
                _rootView = mInflater.Inflate(Resource.Layout.ItemExpandCreateWorkflowChild, null);
                _recyChild = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ItemExpandCreateWorkflowChild);
            }
            else
            {
                _recyChild = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ItemExpandCreateWorkflowChild);
            }

            if (childPosition == 0) // Load cho item đầu là dc
            {
                if (_lstBoardWorkflow[groupPosition].lstBeanWorkflow != null && _lstBoardWorkflow[groupPosition].lstBeanWorkflow.Count > 0)
                {
                    _recyChild.Visibility = ViewStates.Visible;
                    AdapterExpandCreateWorkflow_Recychild adapter = new AdapterExpandCreateWorkflow_Recychild(_mainAct, _context, _lstBoardWorkflow[groupPosition].lstBeanWorkflow);
                    StaggeredGridLayoutManager layoutmanager = new StaggeredGridLayoutManager(3, LinearLayoutManager.Vertical);
                    adapter.CustomItemClick -= OnItemClickChild;
                    adapter.CustomItemClick += OnItemClickChild;
                    _recyChild.SetAdapter(adapter);
                    _recyChild.SetLayoutManager(layoutmanager);

                    if (_lstBoardWorkflow[groupPosition].IsExpand == true)
                        SetRecyclerViewHeight(_recyChild, _lstBoardWorkflow[groupPosition].lstBeanWorkflow.Count, 3);
                    else
                        RemoveRecyclerViewHeight(_recyChild);
                }
                else
                    _recyChild.Visibility = ViewStates.Gone;
            }
            else
            {
                _recyChild.Visibility = ViewStates.Gone;
            }

            return _rootView;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return groupPosition;
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            LayoutInflater mInflater = LayoutInflater.From(_context);
            View _rootView = mInflater.Inflate(Resource.Layout.ItemExpandBoardGroup, null);
            ImageView _imgExpand = _rootView.FindViewById<ImageView>(Resource.Id.img_ItemExpandBoardGroup_Expand);
            TextView _tvTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandBoardGroup_Title);

            BeanWorkflowCategory _currentCategoryItem = _lstBoardWorkflow[groupPosition].beanWorkflowCategory;

            if (isExpanded == true)
                _imgExpand.Rotation = 0;
            else
                _imgExpand.Rotation = -90;

            if (!String.IsNullOrEmpty(_currentCategoryItem.Title))
                _tvTitle.Text = _currentCategoryItem.Title;
            else
                _tvTitle.Text = "";

            return _rootView;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
        public override void OnGroupCollapsed(int groupPosition)
        {
            this._lstBoardWorkflow[groupPosition].IsExpand = false;
            base.OnGroupCollapsed(groupPosition);
        }
        public override void OnGroupExpanded(int groupPosition)
        {
            this._lstBoardWorkflow[groupPosition].IsExpand = true;
            base.OnGroupExpanded(groupPosition);
        }

        private void SetRecyclerViewHeight(RecyclerView _recy, int listCount, int columnCount)
        {
            int _integer = (int)(listCount / columnCount);
            int _decimal = listCount % columnCount;
            int _totalHeight = (_integer * (int)CmmDroidFunction.ConvertDpToPixel(150, _context));
            if (_decimal > 0)
                _totalHeight += (int)CmmDroidFunction.ConvertDpToPixel(150, _context);

            _recy.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, _totalHeight + 10);
        }
        private void RemoveRecyclerViewHeight(RecyclerView _recy)
        {
            _recy.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 0);
        }
        #region Recy Child
        public class AdapterExpandCreateWorkflow_Recychild : RecyclerView.Adapter
        {
            private MainActivity _mainAct;
            private Context _context;
            private List<BeanWorkflow> _lstData;

            public event EventHandler<BeanWorkflow> CustomItemClick;
            public AdapterExpandCreateWorkflow_Recychild(MainActivity mainAct, Context context, List<BeanWorkflow> _lstData)
            {
                this._context = context;
                this._mainAct = mainAct;
                this._lstData = _lstData;
            }
            private void OnItemClick(int obj)
            {
                if (CustomItemClick != null)
                    CustomItemClick(this, _lstData[obj]);
            }
            public override int ItemCount => _lstData.Count;
            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemExpandCreateWorkflow, parent, false);
                ItemExpandCreateWorkflowViewHolder holder = new ItemExpandCreateWorkflowViewHolder(itemView, OnItemClick);
                return holder;
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                ItemExpandCreateWorkflowViewHolder _holder = holder as ItemExpandCreateWorkflowViewHolder;

                if (!String.IsNullOrEmpty(_lstData[position].ImageURL))
                {
                    _holder._imgWorkflow.Visibility = ViewStates.Visible;
                    CmmDroidFunction.SetAvataByImagePath(_mainAct, _lstData[position].ImageURL, _holder._imgWorkflow, _holder._tvAvatar, 130);
                }
                else
                    _holder._imgWorkflow.Visibility = ViewStates.Invisible;

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    _holder._tvTitle.Text = !String.IsNullOrEmpty(_lstData[position].Title) ? _lstData[position].Title : "";
                }
                else
                {
                    _holder._tvTitle.Text = !String.IsNullOrEmpty(_lstData[position].TitleEN) ? _lstData[position].TitleEN : "";
                }
            }
            public class ItemExpandCreateWorkflowViewHolder : RecyclerView.ViewHolder
            {
                public LinearLayout _lnAll { get; set; }
                public ImageView _imgWorkflow { get; set; }
                public TextView _tvAvatar { get; set; }
                public TextView _tvTitle { get; set; }

                public ItemExpandCreateWorkflowViewHolder(View itemview, Action<int> listener) : base(itemview)
                {
                    _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandCreateWorkflow);
                    _imgWorkflow = itemview.FindViewById<ImageView>(Resource.Id.img_ItemExpandCreateWorkflow_Avatar);
                    _tvAvatar = itemview.FindViewById<TextView>(Resource.Id.tv_ItemExpandCreateWorkflow_Avatar);
                    _tvTitle = itemview.FindViewById<TextView>(Resource.Id.tv_ItemExpandCreateWorkflow_Title);
                    itemview.Click += (sender, e) => listener(base.LayoutPosition);
                }
            }

        }
        #endregion
    }
}