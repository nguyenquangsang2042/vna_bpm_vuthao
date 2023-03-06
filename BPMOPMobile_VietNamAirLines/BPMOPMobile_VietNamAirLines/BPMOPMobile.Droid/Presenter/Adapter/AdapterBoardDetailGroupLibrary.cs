using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Allyants.BoardViewLib;
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
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using Refractored.Controls;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    public class AdapterBoardDetailGroupLibrary : BoardAdapter
    {
        private List<BeanBoardStepDefine> _lstStepDefine = new List<BeanBoardStepDefine>();

        public MainActivity _mainAct;
        public Context _context;
        private ControllerBoard CTRLBoard = new ControllerBoard();

        public AdapterBoardDetailGroupLibrary(MainActivity _mainAct, Context context, List<BeanBoardStepDefine> _lstStepDefine) : base(context)
        {
            this._lstStepDefine = _lstStepDefine;
            this._mainAct = _mainAct;
            this._context = context;
        }
        public List<BeanBoardStepDefine> GetListData()
        {
            return _lstStepDefine;
        }
        public void UpdateListData(List<BeanBoardStepDefine> _lstStepDefine)
        {
            this._lstStepDefine = _lstStepDefine;
        }
        public void CallBackData(List<BeanBoardStepDefine> _lstStepDefine)
        {
            this._lstStepDefine = new List<BeanBoardStepDefine>();
            NotifyAll();
            this._lstStepDefine = _lstStepDefine;
            NotifyAll();

        }
        public BeanWorkflowItem GetItemByPostion(int columnPosition, int itemPosition)
        {
            return _lstStepDefine[columnPosition].lstWorkflowItem[itemPosition];
        }

        public override int ColumnCount => _lstStepDefine.Count;

        public override Java.Lang.Object CreateFooterObject(int columnPosition)
        {
            return null;
        }

        public override View CreateFooterView(Context context, Java.Lang.Object footerObject, int columnPosition)
        {
            return null;
        }

        public override Java.Lang.Object CreateHeaderObject(int columnPosition)
        {
            return null;
        }

        public override View CreateHeaderView(Context context, Java.Lang.Object headerObject, int columnPosition)
        {
            headerObject = LayoutInflater.From(context).Inflate(Resource.Layout.ItemBoardDetailGroupLibrary, null);
            View _convertView = (View)headerObject;

            AdapterBoardDetailGroupLibrary_ColumnViewHolder _holder = null;
            _holder = new AdapterBoardDetailGroupLibrary_ColumnViewHolder
            {
                _lnAll = _convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemBoardDetailGroupLibrary_All),
                _tvName = _convertView.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroupLibrary_Title),
                _tvNoData = _convertView.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroupLibrary_Child_NoData),
            };

            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                _holder._tvNoData.Text = CmmFunction.GetTitle("TEXT_NODATA", "Không có dữ liệu");
            else
                _holder._tvNoData.Text = CmmFunction.GetTitle("TEXT_NODATA", "No data");

            if (_lstStepDefine[columnPosition].lstWorkflowItem != null && _lstStepDefine[columnPosition].lstWorkflowItem.Count > 0)
            {
                _holder._lnAll.SetBackgroundResource(Resource.Drawable.textgrayboard_top2corner);
                _holder._tvNoData.Visibility = ViewStates.Gone;
            }
            else
            {
                _holder._lnAll.SetBackgroundResource(Resource.Drawable.textgrayboard_4corner);
                _holder._tvNoData.Visibility = ViewStates.Visible;
            }

            BeanWorkflowStepDefine _currentStepDefineItem = _lstStepDefine[columnPosition].itemStepDefine;
            if (!string.IsNullOrEmpty(_currentStepDefineItem.Title))
                _holder._tvName.Text = _currentStepDefineItem.Title;
            else
                _holder._tvName.Text = "";



            return (View)headerObject;
        }

        public override Java.Lang.Object CreateItemObject(int columnPosition, int itemPosition)
        {
            return null;
        }

        public override View CreateItemView(Context context, Java.Lang.Object headerObject, Java.Lang.Object item, int columnPosition, int itemPosition)
        {
            AdapterBoardDetailGroupLibrary_ChildViewHolder _holder = null;
            View _convertView = (View)headerObject;

            //if (_convertView == null)
            //{
            _convertView = LayoutInflater.From(context).Inflate(Resource.Layout.ItemBoardDetailGroup_RecyChild, null);
            _holder = new AdapterBoardDetailGroupLibrary_ChildViewHolder
            {
                _lnAll = _convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemBoardDetailGroup_RecyChild_All),
                _lnData = _convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemBoardDetailGroup_RecyChild_Data),
                _cardAll = _convertView.FindViewById<CardView>(Resource.Id.card_ItemBoardDetailGroup_RecyChild_All),
                _tvDate = _convertView.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroup_RecyChild_Date),
                _imgSubCribe = _convertView.FindViewById<ImageView>(Resource.Id.img_ItemBoardDetailGroup_RecyChild_Subcribe),
                _relaAvatar = _convertView.FindViewById<RelativeLayout>(Resource.Id.rela_ItemBoardDetailGroup_RecyChild_Avatar),
                _tvAvatar = _convertView.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroup_RecyChild_Avatar),
                _imgAvatar = _convertView.FindViewById<CircleImageView>(Resource.Id.img_ItemBoardDetailGroup_RecyChild_Avatar),
                _tvTitle = _convertView.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroup_RecyChild_Title),
                _tvCountAttach = _convertView.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroup_RecyChild_CountAttach),
                _tvCountComment = _convertView.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroup_RecyChild_CountComment),
                _relaAvatar2 = _convertView.FindViewById<RelativeLayout>(Resource.Id.rela_ItemBoardDetailGroup_RecyChild_Avatar2),
                _tvAvatar2 = _convertView.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroup_RecyChild_Avatar2),
                _imgAvatar2 = _convertView.FindViewById<CircleImageView>(Resource.Id.img_ItemBoardDetailGroup_RecyChild_Avatar2),
                _tvCountPeople = _convertView.FindViewById<TextView>(Resource.Id.tv_ItemBoardDetailGroup_RecyChild_CountPeople),
            };
            //    _convertView.Tag = _holder;
            //}
            //else
            //{
            //    _holder = (AdapterBoardDetailGroupLibrary_ChildViewHolder)_convertView.Tag;
            //}
            BeanWorkflowItem _currentWorkflowItem = _lstStepDefine[columnPosition].lstWorkflowItem[itemPosition];


            if (itemPosition == _lstStepDefine[columnPosition].lstWorkflowItem.Count - 1)
            {
                _holder._lnAll.SetBackgroundResource(Resource.Drawable.textgrayboard_bot2corner);
            }
            else
            {
                _holder._lnAll.SetBackgroundResource(Resource.Color.clGrayNavigator);
            }

            ////if (_currentWorkflowItem.ActionStatusID == CTRLBoard.ApprovedID) // phê duyệt -> xanh
            ////    _holder._cardAll.SetCardBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clStatusGreen)));
            ////else if (_currentWorkflowItem.ActionStatusID == CTRLBoard.RejectedID) // Từ chối -> Đỏ
            ////    _holder._cardAll.SetCardBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clStatusRed)));
            ////else
            ////    _holder._cardAll.SetCardBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));

            if (_currentWorkflowItem.IsConsul == true) // đang chờ tham vấn 
                _holder._cardAll.SetCardBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clStatusBlue)));
            if (_currentWorkflowItem.IsInfo == true) // đang chờ bổ sung
                _holder._cardAll.SetCardBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBoardOrangeRequestInfo)));

            #region Line 1
            if (_currentWorkflowItem.DueDate.HasValue)
                _holder._tvDate.Text = CTRLBoard.GetFormatDateLang(_currentWorkflowItem.DueDate.Value);
            else
                _holder._tvDate.Text = "";

            if (_currentWorkflowItem.IsFollow == true)
            {
                _holder._imgSubCribe.Visibility = ViewStates.Visible;
                _holder._imgSubCribe.SetImageResource(Resource.Drawable.icon_ver2_star_checked);
            }
            else
            {
                _holder._imgSubCribe.Visibility = ViewStates.Invisible;
                //_holder._imgSubCribe.SetImageResource(Resource.Drawable.icon_ver2_star_unchecked);
            }
            #endregion

            #region Line 2

            if (!string.IsNullOrEmpty(_currentWorkflowItem.CreatedBy))
            {
                string[] valueSearch = _currentWorkflowItem.CreatedBy.ToLowerInvariant().Split(",");
                CmmDroidFunction.SetAvataByBeanUser(_mainAct, _context, valueSearch[0], "ID", _holder._imgAvatar, _holder._tvAvatar, 50);
            }
            else
            {
                _holder._tvAvatar.Visibility = ViewStates.Invisible;
                _holder._imgAvatar.Visibility = ViewStates.Invisible;
            }

            if (!string.IsNullOrEmpty(_currentWorkflowItem.Content))
                _holder._tvTitle.Text = _currentWorkflowItem.Content;
            else
                _holder._tvTitle.Visibility = ViewStates.Invisible;

            #endregion

            #region Line 3

            _holder._tvCountAttach.Text = _currentWorkflowItem.FileCount.ToString();
            _holder._tvCountComment.Text = _currentWorkflowItem.CommentCount.ToString();

            if (!string.IsNullOrEmpty(_currentWorkflowItem.AssignedTo))
            {
                string[] valueSearch = _currentWorkflowItem.AssignedTo.ToLowerInvariant().Split(",");
                CmmDroidFunction.SetAvataByBeanUser(_mainAct, _context, valueSearch[0], "ID", _holder._imgAvatar2, _holder._tvAvatar2, 50);

                if (valueSearch.Length > 1)
                    _holder._tvCountPeople.Text = "+" + (valueSearch.Length - 1).ToString();
                else
                    _holder._tvCountPeople.Text = "";
            }
            else
            {
                _holder._tvAvatar2.Visibility = ViewStates.Invisible;
                _holder._imgAvatar2.Visibility = ViewStates.Invisible;
                _holder._tvCountPeople.Text = "";
            }
            #endregion


            //if (columnPosition > _lstStepDefine.Count - 3) // lock 2 cột cuối
            //{
            //    finalView.Enabled = false;
            //}
            //else
            //{
            //    finalView.Enabled = true;
            //}


            return (View)_convertView;
        }
        public override int GetItemCount(int columnPosition)
        {
            if (columnPosition > _lstStepDefine.Count - 1)
                return 0;

            if (_lstStepDefine[columnPosition].lstWorkflowItem != null && _lstStepDefine[columnPosition].lstWorkflowItem.Count > 0)
            {
                return _lstStepDefine[columnPosition].lstWorkflowItem.Count;
            }
            return 0;
        }

        public override bool IsColumnLocked(int columnPosition)
        {
            return true; // lock cột lại
        }

        public override bool IsItemLocked(int columnPosition)
        {
            //if (columnPosition > _lstStepDefine.Count - 3)
            //{
            //    return true;
            //}

            return false;
        }

        public override int MaxItemCount(int columnPosition)
        {
            return int.MaxValue;
        }

        private class AdapterBoardDetailGroupLibrary_ColumnViewHolder : Java.Lang.Object
        {
            public LinearLayout _lnAll { get; set; }
            public TextView _tvName { get; set; }
            public TextView _tvNoData { get; set; }
            public RecyclerView _recyChild { get; set; }
        }

        private class AdapterBoardDetailGroupLibrary_ChildViewHolder : Java.Lang.Object
        {
            public LinearLayout _lnAll { get; set; }
            public LinearLayout _lnData { get; set; }
            public CardView _cardAll { get; set; }
            public TextView _tvDate { get; set; }
            public ImageView _imgSubCribe { get; set; }
            public RelativeLayout _relaAvatar { get; set; }
            public TextView _tvAvatar { get; set; }
            public CircleImageView _imgAvatar { get; set; }
            public TextView _tvTitle { get; set; }
            public TextView _tvCountAttach { get; set; }
            public TextView _tvCountComment { get; set; }
            public RelativeLayout _relaAvatar2 { get; set; }
            public TextView _tvAvatar2 { get; set; }
            public CircleImageView _imgAvatar2 { get; set; }
            public TextView _tvCountPeople { get; set; }

        }
    }
}