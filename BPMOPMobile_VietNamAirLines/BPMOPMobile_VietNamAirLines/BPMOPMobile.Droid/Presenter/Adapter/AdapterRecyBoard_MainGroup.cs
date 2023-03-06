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
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    
    public class AdapterRecyBoard_MainGroup : RecyclerView.Adapter
    {
        private MainActivity _mainAct;
        private Context _context;
        private List<BeanWorkflow> _lstWorkflow;
        public event EventHandler<BeanWorkflow> CustomItemClick_Favorite;
        public event EventHandler<BeanWorkflow> CustomItemClick_Board;
        public event EventHandler<BeanWorkflow> CustomItemClick_List;
        public event EventHandler<BeanWorkflow> CustomItemClick_Report;
        private bool _allowLoadMore = false;

        private void OnItemClickChild_Favorite(int position)
        {
            if (CustomItemClick_Favorite != null)
                CustomItemClick_Favorite(this, _lstWorkflow[position]);
        }
        private void OnItemClickChild_Board(int position)
        {
            if (CustomItemClick_Board != null)
                CustomItemClick_Board(this, _lstWorkflow[position]);
        }
        private void OnItemClickChild_List(int position)
        {
            if (CustomItemClick_List != null)
                CustomItemClick_List(this, _lstWorkflow[position]);
        }
        private void OnItemClickChild_Report(int position)
        {
            if (CustomItemClick_Report != null)
                CustomItemClick_Report(this, _lstWorkflow[position]);
        }

        public void SetAllowLoadMore(bool _allowLoadMore)
        {
            this._allowLoadMore = _allowLoadMore;
        }
        public void UpdateListData(List<BeanWorkflow> newList)
        {
            try
            {
                this._lstWorkflow = newList;
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetListData", ex);
#endif
            }
        }
        public void UpdateItemData(BeanWorkflow _itemWorkflow)
        {
            for (int i = 0; i < _lstWorkflow.Count; i++)
                if (_lstWorkflow[i].WorkflowID == _itemWorkflow.WorkflowID)
                {
                    _lstWorkflow[i] = _itemWorkflow;
                    break;
                }
        }
        public void AddNewItem(BeanWorkflow e)
        {
            try
            {
                _lstWorkflow.Add(e);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetListData", ex);
#endif
            }
        }
        public void RemoveItem(BeanWorkflow e)
        {
            try
            {
                _lstWorkflow = _lstWorkflow.Where(x => x.WorkflowID != e.WorkflowID).ToList();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetListData", ex);
#endif
            }
        }
        public AdapterRecyBoard_MainGroup(MainActivity _mainAct, Context _context, List<BeanWorkflow> _lstWorkflow)
        {
            this._mainAct = _mainAct;
            this._context = _context;
            this._lstWorkflow = _lstWorkflow;
        }
        public override int ItemCount => _lstWorkflow.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemVer2ExpandBoardChild, parent, false);
            //AdapterRecyBoard_MainGroupViewHolder holder = new AdapterRecyBoard_MainGroupViewHolder(itemView, OnItemClickChild_Favorite, OnItemClickChild_Board, OnItemClickChild_List, OnItemClickChild_Report);
            AdapterRecyBoard_MainGroupViewHolder_Ver2 holder = new AdapterRecyBoard_MainGroupViewHolder_Ver2(itemView, OnItemClickChild_Board,OnItemClickChild_Favorite);
            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            #region Version 1
            //AdapterRecyBoard_MainGroupViewHolder _holder = holder as AdapterRecyBoard_MainGroupViewHolder;
            //_holder._imgFavorite.SetImageResource(Resource.Drawable.icon_ver2_favorite_check);
            //BeanWorkflow _currentWorkflowItem = _lstWorkflow[position];

            //if (position == _lstWorkflow.Count - 1 && _allowLoadMore == true)
            //    _holder._lnLoadMore.Visibility = ViewStates.Visible;
            //else
            //    _holder._lnLoadMore.Visibility = ViewStates.Gone;

            //if (!String.IsNullOrEmpty(_currentWorkflowItem.ImageURL))
            //    CmmDroidFunction.SetAvataByImagePath(_mainAct, _currentWorkflowItem.ImageURL, _holder._imgAvatar, _holder._tvAvatar, 130);
            //else
            //    _holder._imgAvatar.Visibility = ViewStates.Invisible;

            //if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
            //    _holder._tvTitle.Text = !String.IsNullOrEmpty(_currentWorkflowItem.Title) ? _currentWorkflowItem.Title : "";
            //else
            //    _holder._tvTitle.Text = !String.IsNullOrEmpty(_currentWorkflowItem.TitleEN) ? _currentWorkflowItem.TitleEN : "";
            #endregion

            AdapterRecyBoard_MainGroupViewHolder_Ver2 _holder = holder as AdapterRecyBoard_MainGroupViewHolder_Ver2;
            _holder._imgFavorite.SetImageResource(Resource.Drawable.icon_ver2_favorite_check);
            BeanWorkflow _currentWorkflowItem = _lstWorkflow[position];

            if (position % 2 == 1) // tô màu so le
                _holder._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clWhite)));
            else
                _holder._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clVer2BlueNavigation)));

            if (!String.IsNullOrEmpty(_currentWorkflowItem.ImageURL))
                CmmDroidFunction.SetContentToImageView(_mainAct, _holder._imgAvatar, _currentWorkflowItem.ImageURL, 80);
            else
                _holder._imgAvatar.Visibility = ViewStates.Invisible;

            if (_currentWorkflowItem.Favorite == true)
            {
                _holder._imgFavorite.SetImageResource(Resource.Drawable.icon_ver2_favorite_check);
                _holder._imgFavorite.SetColorFilter(new Color(ContextCompat.GetColor(_context, Resource.Color.clVer2BlueMain)));
            }
            else
            {
                _holder._imgFavorite.SetImageResource(Resource.Drawable.icon_ver2_favorite_uncheck);
                _holder._imgFavorite.SetColorFilter(new Color(ContextCompat.GetColor(_context, Resource.Color.clBottomDisable)));
            }

            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                _holder._tvTitle.Text = !String.IsNullOrEmpty(_currentWorkflowItem.Title) ? _currentWorkflowItem.Title : "";
            else
                _holder._tvTitle.Text = !String.IsNullOrEmpty(_currentWorkflowItem.TitleEN) ? _currentWorkflowItem.TitleEN : "";

            _holder._tvDescription.Text = "";
        }

        public class AdapterRecyBoard_MainGroupViewHolder_Ver2 : RecyclerView.ViewHolder
        {
            public LinearLayout _lnAll { get; set; }
            public TextView _tvTitle { get; set; }
            public TextView _tvDescription { get; set; }
            public ImageView _imgAvatar { get; set; }
            public ImageView _imgFavorite { get; set; }
            public AdapterRecyBoard_MainGroupViewHolder_Ver2(View itemview, Action<int> listener, Action<int> listenerFavorite) : base(itemview)
            {
                _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemVer2ExpandBoardChild_All);
                _imgFavorite = itemview.FindViewById<ImageView>(Resource.Id.img_ItemVer2ExpandBoardChild_Favorite);
                _imgAvatar = itemview.FindViewById<ImageView>(Resource.Id.img_ItemVer2ExpandBoardChild_Avatar);
                _tvTitle = itemview.FindViewById<TextView>(Resource.Id.tv_ItemVer2ExpandBoardChild_Title);
                _tvDescription = itemview.FindViewById<TextView>(Resource.Id.tv_ItemVer2ExpandBoardChild_Description);
                _lnAll.Click += (sender, e) => listener(base.LayoutPosition);
                _imgFavorite.Click += (sender, e) => listenerFavorite(base.LayoutPosition);
            }
        }

        public class AdapterRecyBoard_MainGroupViewHolder : RecyclerView.ViewHolder
        {
            public LinearLayout _lnLoadMore { get; set; }
            public LinearLayout _lnAll { get; set; }
            public LinearLayout _lnAllTop { get; set; }
            public ImageView _imgFavorite { get; set; }
            public ImageView _imgAvatar { get; set; }
            public TextView _tvTitle { get; set; }
            public TextView _tvAvatar { get; set; }
            public LinearLayout _lnBoard { get; set; }
            public LinearLayout _lnList { get; set; }
            public LinearLayout _lnReport { get; set; }
            public ImageView _imgBoard { get; set; }
            public ImageView _imgList { get; set; }
            public ImageView _imgReport { get; set; }
            public AdapterRecyBoard_MainGroupViewHolder(View itemview, Action<int> favorite, Action<int> board, Action<int> list, Action<int> report) : base(itemview)
            {
                _lnLoadMore = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandBoardChild_LoadMore);
                _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandBoardChild_All);
                _lnAllTop = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandBoardChild_All_Top);
                _imgFavorite = itemview.FindViewById<ImageView>(Resource.Id.img_ItemExpandBoardChild_Favorite);
                _imgAvatar = itemview.FindViewById<ImageView>(Resource.Id.img_ItemExpandBoardChild_Avatar);
                _tvTitle = itemview.FindViewById<TextView>(Resource.Id.tv_ItemExpandBoardChild_Title);
                _tvAvatar = itemview.FindViewById<TextView>(Resource.Id.tv_ItemExpandBoardChild_Avatar);
                _lnBoard = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandBoardChild_Board);
                _lnList = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandBoardChild_List);
                _lnReport = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandBoardChild_Report);
                _imgBoard = itemview.FindViewById<ImageView>(Resource.Id.img_ItemExpandBoardChild_Board);
                _imgList = itemview.FindViewById<ImageView>(Resource.Id.img_ItemExpandBoardChild_List);
                _imgReport = itemview.FindViewById<ImageView>(Resource.Id.img_ItemExpandBoardChild_Report);

                _lnAllTop.Click += (sender, e) => board(base.LayoutPosition);
                _imgFavorite.Click += (sender, e) => favorite(base.LayoutPosition);
                _lnBoard.Click += (sender, e) => board(base.LayoutPosition);
                _lnList.Click += (sender, e) => list(base.LayoutPosition);
                _lnReport.Click += (sender, e) => report(base.LayoutPosition);
            }
        }
    }
}