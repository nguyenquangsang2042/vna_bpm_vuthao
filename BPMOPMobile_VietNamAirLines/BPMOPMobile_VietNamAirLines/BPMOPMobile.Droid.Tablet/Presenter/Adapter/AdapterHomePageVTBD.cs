using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Tablet;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    [Obsolete]
    class AdapterHomePageVTBD : BaseAdapter<BeanWorkflowItem>
    {
        private MainActivity _mainAct;
        private Context context;
        private List<string> _lstCategoryFlag = new List<string>(); // flag này để lưu lại trạng thái item[n-1] để xem có hiện category item[n] không
        private List<BeanWorkflowItem> _lstbeanWorkflowItems = new List<BeanWorkflowItem>();
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        public event EventHandler<BeanWorkflowItem> CustomItemClick;
        private int _clickedPosition = -1;
        public AdapterHomePageVTBD(Context context, List<BeanWorkflowItem> _lstbeanWorkflowItems, MainActivity mainAct, int _clickedPosition = -1)
        {
            this.context = context;
            this._lstbeanWorkflowItems = _lstbeanWorkflowItems;
            this._clickedPosition = _clickedPosition;
            _mainAct = mainAct;
        }
        public override BeanWorkflowItem this[int position] => _lstbeanWorkflowItems[position];

        public override int Count
        {
            get
            {
                // khởi tạo list Flag Category
                _lstCategoryFlag = new List<string>();
                for (int i = 0; i < _lstbeanWorkflowItems.Count; i++)
                {
                    if (_lstbeanWorkflowItems[i].Created.HasValue)
                    {
                        if (_lstbeanWorkflowItems[i].Created.Value.Date == DateTime.Now.Date) // Hôm nay
                        {
                            if (CmmVariable.SysConfig.LangCode == "VN")
                            {
                                _lstCategoryFlag.Add("Hôm nay");
                            }
                            else
                            {
                                _lstCategoryFlag.Add("Today");
                            }
                        }
                        else if (_lstbeanWorkflowItems[i].Created.Value.Date == DateTime.Now.Date.AddDays(-1)) // Hôm qua
                        {
                            if (CmmVariable.SysConfig.LangCode == "VN")
                            {
                                _lstCategoryFlag.Add("Hôm qua");
                            }
                            else
                            {
                                _lstCategoryFlag.Add("Yesterday");
                            }
                        }
                        else if (_lstbeanWorkflowItems[i].Created.Value.Date < DateTime.Now.Date.AddDays(-1)) // Cũ hơn
                        {
                            if (CmmVariable.SysConfig.LangCode == "VN")
                            {
                                _lstCategoryFlag.Add("Cũ hơn");
                            }
                            else
                            {
                                _lstCategoryFlag.Add("Older");
                            }
                        }
                    }
                }
                return _lstbeanWorkflowItems.Count;
            }
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            NotifyTestNewLayoutAdapterViewHolder holder;

            #region Get View
            //if (convertView == null)
            //{
            convertView = LayoutInflater.From(context).Inflate(Resource.Layout.ItemHomePageToDoList, null);
            holder = new NotifyTestNewLayoutAdapterViewHolder
            {
                _lnAll = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemHomePageToDoList_All),
                _lnCategory = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemHomePageToDoList_Category),
                _tvCategory = convertView.FindViewById<TextView>(Resource.Id.tv_ItemHomePageToDoList_Category),
                _viewCategory = convertView.FindViewById<View>(Resource.Id.view_ItemHomePageToDoList_Category),
                _viewLeftSelected = convertView.FindViewById<View>(Resource.Id.view_ItemHomePageToDoList_LeftSelected),
                _lnContent = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemHomePageToDoList_Content),
                _tvAvatar = convertView.FindViewById<TextView>(Resource.Id.tv_ItemHomePageToDoList_Avatar),
                _tvTitle = convertView.FindViewById<TextView>(Resource.Id.tv_ItemHomePageToDoList_Title),
                _tvTime = convertView.FindViewById<TextView>(Resource.Id.tv_ItemHomePageToDoList_Time),
                _tvDescription = convertView.FindViewById<TextView>(Resource.Id.tv_ItemHomePageToDoList_Description),
                _imgFlag = convertView.FindViewById<ImageView>(Resource.Id.img_ItemHomePageToDoList_Flag),
                _imgAttach = convertView.FindViewById<ImageView>(Resource.Id.img_ItemHomePageToDoList_AttachFile),
                _tvStatusTime = convertView.FindViewById<TextView>(Resource.Id.tv_ItemHomePageToDoList_StatusTime),
            };
            convertView.Tag = holder;
            //}
            //else
            //{
            //    holder = (NotifyTestNewLayoutAdapterViewHolder)convertView.Tag;
            //}
            #endregion

            if (_clickedPosition != -1 && _clickedPosition == position)
            {
                holder._viewLeftSelected.Visibility = ViewStates.Visible;
                holder._lnContent.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clItemListEnable)));
            }
            else
            {
                holder._viewLeftSelected.Visibility = ViewStates.Invisible;
                holder._lnContent.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));
            }

            #region Category
            if (position == 0)
            {
                holder._tvCategory.Text = _lstCategoryFlag[position];
                holder._lnCategory.Visibility = ViewStates.Visible;
                holder._viewCategory.Visibility = ViewStates.Gone;
            }
            else if (position > 0) // so sánh với Item trước đó
            {
                if (_lstCategoryFlag[position].Equals(_lstCategoryFlag[position - 1]))
                {
                    holder._lnCategory.Visibility = ViewStates.Gone;
                    holder._viewCategory.Visibility = ViewStates.Gone;
                }
                else
                {
                    holder._tvCategory.Text = _lstCategoryFlag[position];
                    holder._lnCategory.Visibility = ViewStates.Visible;
                    holder._viewCategory.Visibility = ViewStates.Visible;
                }
            }
            #endregion

            #region Line 1
            if (!String.IsNullOrEmpty(_lstbeanWorkflowItems[position].CreatedByName))
            {
                holder._tvAvatar.Text = _lstbeanWorkflowItems[position].CreatedByName.Substring(0, 2).ToString().ToUpper();
                Random rnd = new Random();
                var color = Color.Rgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                holder._tvAvatar.BackgroundTintList = ColorStateList.ValueOf(color);
            }
            else
            {
                holder._tvAvatar.Visibility = ViewStates.Invisible;
            }
            if (!String.IsNullOrEmpty(_lstbeanWorkflowItems[position].WorkflowTitle))
            {
                holder._tvTitle.Text = _lstbeanWorkflowItems[position].WorkflowTitle;
            }
            else
            {
                holder._tvTitle.Visibility = ViewStates.Invisible;
            }
            if (_lstbeanWorkflowItems[position].Created.HasValue)
            {
                holder._tvTime.Text = CTRLHomePage.GetDateTimeCondition_CreatedDay(_lstbeanWorkflowItems[position].Created.Value, CmmVariable.SysConfig.LangCode);
            }
            else
            {
                holder._tvTime.Visibility = ViewStates.Invisible;
            }
            #endregion

            #region Line 2
            if (!String.IsNullOrEmpty(_lstbeanWorkflowItems[position].AssignedToName))
            {
                if (CmmVariable.SysConfig.LangCode == "VN")
                {
                    holder._tvDescription.Text = "Đến: " + _lstbeanWorkflowItems[position].AssignedToName;
                }
                else
                {
                    holder._tvDescription.Text = "To: " + _lstbeanWorkflowItems[position].AssignedToName;
                }
            }
            else
            {
                holder._tvDescription.Visibility = ViewStates.Invisible;
            }
            ////if (_lstbeanWorkflowItems[position].p == 1)
            ////{
            ////    holder._imgFlag.Visibility = ViewStates.Visible;
            ////}
            ////else
            ////{
            ////    holder._imgFlag.Visibility = ViewStates.Invisible;
            ////}
            ////if (_lstbeanWorkflowItems[position].HasValue && _lstbeanWorkflowItems[position].HasFile.HasValue == true)
            ////{
            ////    holder._imgAttach.Visibility = ViewStates.Visible;
            ////}
            ////else
            ////{
            ////    holder._imgAttach.Visibility = ViewStates.Invisible;
            ////}
            holder._imgFlag.Visibility = ViewStates.Gone;
            holder._imgAttach.Visibility = ViewStates.Gone;
            #endregion

            #region Line 3
            if (_lstbeanWorkflowItems[position].IssueDate.HasValue)
            {
                holder._tvStatusTime.Text = CTRLHomePage.GetDateTimeCondition_CreatedDay(_lstbeanWorkflowItems[position].IssueDate.Value, CmmVariable.SysConfig.LangCode);
                if (_lstbeanWorkflowItems[position].IssueDate.Value.Date >= DateTime.Now.Date && _lstbeanWorkflowItems[position].IssueDate.Value.Date < DateTime.Now.AddDays(1).Date) // Today
                {
                    holder._tvStatusTime.SetTextColor(new Color(ContextCompat.GetColor(context, Resource.Color.clIcon))); // yellow
                }
                else if (_lstbeanWorkflowItems[position].IssueDate.Value.Date < DateTime.Now.Date) // Over
                {
                    holder._tvStatusTime.SetTextColor(new Color(ContextCompat.GetColor(context, Resource.Color.clRedMain)));
                }
                else // Inbox
                {
                    holder._tvStatusTime.SetTextColor(new Color(ContextCompat.GetColor(context, Resource.Color.clTextSmall))); // gray                   
                }
            }
            else
            {
                holder._tvStatusTime.Visibility = ViewStates.Gone;
            }
            #endregion

            holder._lnAll.Click += (sender, e) => OnClick(_lstbeanWorkflowItems[position], position);
            return convertView;
        }
        public void UpdateSelectedPosition(BeanWorkflowItem _item)
        {
            _clickedPosition = _lstbeanWorkflowItems.IndexOf(_item);
        }
        void OnClick(BeanWorkflowItem position, int _testposition)
        {
            if (CustomItemClick != null)
                CustomItemClick(this, position);
        }
        class NotifyTestNewLayoutAdapterViewHolder : Java.Lang.Object
        {
            public LinearLayout _lnAll { get; set; }
            public LinearLayout _lnCategory { get; set; }
            public View _viewLeftSelected { get; set; }
            public TextView _tvCategory { get; set; }
            public View _viewCategory { get; set; }
            public LinearLayout _lnContent { get; set; }
            public TextView _tvAvatar { get; set; }
            public TextView _tvTitle { get; set; }
            public TextView _tvTime { get; set; }
            public TextView _tvDescription { get; set; }
            public ImageView _imgFlag { get; set; }
            public ImageView _imgAttach { get; set; }
            public TextView _tvStatusTime { get; set; }
        }
    }
}