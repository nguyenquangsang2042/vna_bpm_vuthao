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
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Presenter.Fragment;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    class AdapterExpandBoard_MainGroup : BaseExpandableListAdapter
    {
        private MainActivity _mainAct;
        private Context _context;
        public event EventHandler<BeanWorkflow> CustomItemClickChild_Favorite;
        public event EventHandler<BeanWorkflow> CustomItemClickChild_Board;
        public event EventHandler<BeanWorkflow> CustomItemClickChild_List;
        public event EventHandler<BeanWorkflow> CustomItemClickChild_Report;
        private List<BeanBoardWorkflow> _lstBoardWorkflow = new List<BeanBoardWorkflow>();

        private void OnItemClickChild_Favorite(int GroupPos, int ChildPos)
        {
            if (CustomItemClickChild_Favorite != null)
                CustomItemClickChild_Favorite(this, _lstBoardWorkflow[GroupPos].lstBeanWorkflow[ChildPos]);
        }

        private void OnItemClickChild_Board(int GroupPos, int ChildPos)
        {
            if (CustomItemClickChild_Board != null)
                CustomItemClickChild_Board(this, _lstBoardWorkflow[GroupPos].lstBeanWorkflow[ChildPos]);
        }

        private void OnItemClickChild_List(int GroupPos, int ChildPos)
        {
            if (CustomItemClickChild_List != null)
                CustomItemClickChild_List(this, _lstBoardWorkflow[GroupPos].lstBeanWorkflow[ChildPos]);
        }

        private void OnItemClickChild_Report(int GroupPos, int ChildPos)
        {
            if (CustomItemClickChild_Report != null)
                CustomItemClickChild_Report(this, _lstBoardWorkflow[GroupPos].lstBeanWorkflow[ChildPos]);
        }

        public void UpdateListData(List<BeanBoardWorkflow> newList)
        {
            this._lstBoardWorkflow = newList;
        }

        public void UpdateItemData(BeanWorkflow _itemWorkflow)
        {
            for (int i = 0; i < _lstBoardWorkflow.Count; i++)
                for (int j = 0; j < _lstBoardWorkflow[i].lstBeanWorkflow.Count; j++)
                    if (_lstBoardWorkflow[i].lstBeanWorkflow[j].WorkflowID == _itemWorkflow.WorkflowID)
                    {
                        _lstBoardWorkflow[i].lstBeanWorkflow[j].WorkflowID = _itemWorkflow.WorkflowID;
                        break;
                    }
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

        private int CalculateCurrentPosition(int groupPosition, int childPosition)
        {
            int _result = -1;
            try
            {
                for (int i = 0; i < (groupPosition + 1); i++)
                {
                    _result += _lstBoardWorkflow[i].lstBeanWorkflow.Count;
                }
                _result += (childPosition + 1); // vì child start từ 0
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "CalculateCurrentPosition", ex);
#endif
            }
            return _result;
        }

        public AdapterExpandBoard_MainGroup(MainActivity _mainAct, Context _context, List<BeanBoardWorkflow> _lstBoardWorkflow)
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
            #region Ver 1 - đã cũ
            //LayoutInflater mInflater = LayoutInflater.From(_context);
            //View _rootView = mInflater.Inflate(Resource.Layout.ItemExpandBoardChild, null);

            //LinearLayout _lnAll = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandBoardChild_All);
            //LinearLayout _lnAllTop = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandBoardChild_All_Top);
            //ImageView _imgFavorite = _rootView.FindViewById<ImageView>(Resource.Id.img_ItemExpandBoardChild_Favorite);
            //ImageView _imgAvatar = _rootView.FindViewById<ImageView>(Resource.Id.img_ItemExpandBoardChild_Avatar);
            //TextView _tvTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandBoardChild_Title);
            //TextView _tvAvatar = _rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandBoardChild_Avatar);
            //LinearLayout _lnBoard = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandBoardChild_Board);
            //LinearLayout _lnList = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandBoardChild_List);
            //LinearLayout _lnReport = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandBoardChild_Report);
            //ImageView _imgBoard = _rootView.FindViewById<ImageView>(Resource.Id.img_ItemExpandBoardChild_Board);
            //ImageView _imgList = _rootView.FindViewById<ImageView>(Resource.Id.img_ItemExpandBoardChild_List);
            //ImageView _imgReport = _rootView.FindViewById<ImageView>(Resource.Id.img_ItemExpandBoardChild_Report);

            //BeanWorkflow _currentWorkflowItem = _lstBoardWorkflow[groupPosition].lstBeanWorkflow[childPosition];

            //if (!String.IsNullOrEmpty(_currentWorkflowItem.ImageURL))
            //    CmmDroidFunction.SetAvataByImagePath(_mainAct, _currentWorkflowItem.ImageURL, _imgAvatar, _tvAvatar, 130);
            //else
            //    _imgAvatar.Visibility = ViewStates.Invisible;

            //if (_currentWorkflowItem.Favorite == true)
            //    _imgFavorite.SetImageResource(Resource.Drawable.icon_ver2_favorite_check);
            //else
            //    _imgFavorite.SetImageResource(Resource.Drawable.icon_ver2_favorite_uncheck);

            //if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
            //    _tvTitle.Text = !String.IsNullOrEmpty(_currentWorkflowItem.Title) ? _currentWorkflowItem.Title : "";
            //else
            //    _tvTitle.Text = !String.IsNullOrEmpty(_currentWorkflowItem.TitleEN) ? _currentWorkflowItem.TitleEN : "";

            //_imgFavorite.Click += (sender, e) => { OnItemClickChild_Favorite(groupPosition, childPosition); };
            //_lnBoard.Click += (sender, e) => { OnItemClickChild_Board(groupPosition, childPosition); };
            //_lnList.Click += (sender, e) => { OnItemClickChild_List(groupPosition, childPosition); };
            //_lnReport.Click += (sender, e) => { OnItemClickChild_Report(groupPosition, childPosition); };
            //_lnAllTop.Click += (sender, e) => { OnItemClickChild_Board(groupPosition, childPosition); };
            #endregion

            LayoutInflater mInflater = LayoutInflater.From(_context);
            View _rootView = mInflater.Inflate(Resource.Layout.ItemVer2ExpandBoardChild, null);

            LinearLayout _lnAll = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ItemVer2ExpandBoardChild_All);
            ImageView _imgFavorite = _rootView.FindViewById<ImageView>(Resource.Id.img_ItemVer2ExpandBoardChild_Favorite);
            ImageView _imgAvatar = _rootView.FindViewById<ImageView>(Resource.Id.img_ItemVer2ExpandBoardChild_Avatar);
            TextView _tvTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ItemVer2ExpandBoardChild_Title);
            TextView _tvDescription = _rootView.FindViewById<TextView>(Resource.Id.tv_ItemVer2ExpandBoardChild_Description);

            BeanWorkflow _currentWorkflowItem = _lstBoardWorkflow[groupPosition].lstBeanWorkflow[childPosition];

            if (CalculateCurrentPosition(groupPosition, childPosition) % 2 == 1) // tô màu so le
                _lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clWhite)));
            else
                _lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clVer2BlueNavigation)));

            if (!String.IsNullOrEmpty(_currentWorkflowItem.ImageURL))
            {
                CmmDroidFunction.SetContentToImageView(_mainAct, _imgAvatar, _currentWorkflowItem.ImageURL, 80);
            }
            else
                _imgAvatar.Visibility = ViewStates.Invisible;

            if (_currentWorkflowItem.Favorite == true)
            {
                _imgFavorite.SetImageResource(Resource.Drawable.icon_ver2_favorite_check);
                _imgFavorite.SetColorFilter(new Color(ContextCompat.GetColor(_context, Resource.Color.clVer2BlueMain)));
            }
            else
            {
                _imgFavorite.SetImageResource(Resource.Drawable.icon_ver2_favorite_uncheck);
                _imgFavorite.SetColorFilter(new Color(ContextCompat.GetColor(_context, Resource.Color.clBottomDisable)));
            }

            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                _tvTitle.Text = !String.IsNullOrEmpty(_currentWorkflowItem.Title) ? _currentWorkflowItem.Title : "";
            else
                _tvTitle.Text = !String.IsNullOrEmpty(_currentWorkflowItem.TitleEN) ? _currentWorkflowItem.TitleEN : "";

            _tvDescription.Text = "";

            _lnAll.Click += (sender, e) => { OnItemClickChild_Board(groupPosition, childPosition); };
            _imgFavorite.Click += (sender, e) => { OnItemClickChild_Favorite(groupPosition, childPosition); };

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
            if (GroupCount >= 2) // 1 group khỏi hiện
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
            return new View(_context);
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }

    }
}