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
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using Newtonsoft.Json;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    public class AdapterDetailCreateTask_Attachment : RecyclerView.Adapter
    {
        public AppCompatActivity _mainAct;
        public Context _context;
        public List<BeanAttachFile> _lstAttachment = new List<BeanAttachFile>();
        public event EventHandler<BeanAttachFile> CustomItemClick_ViewItem;
        public event EventHandler<BeanAttachFile> CustomItemClick_SaveItem;
        public event EventHandler<BeanAttachFile> CustomItemClick_DeleteItem;

        private ControllerDetailCreateTask CTRLDetailCreateTask = new ControllerDetailCreateTask();
        private int _flagUserPermission;
        private bool _IsClickFromAction;
        private BeanTask _parentItem;


        public AdapterDetailCreateTask_Attachment(AppCompatActivity _mainAct, Context _context, List<BeanAttachFile> _lstAttachment, BeanTask _parentItem, int _flagUserPermission, bool _IsClickFromAction)
        {
            this._mainAct = _mainAct;
            this._context = _context;
            this._lstAttachment = _lstAttachment;
            this._parentItem = _parentItem;
            this._flagUserPermission = _flagUserPermission;
            this._IsClickFromAction = _IsClickFromAction;
        }
        public void OnCustomItemClick_ViewItem(int position)
        {
            if (CustomItemClick_ViewItem != null)
            {
                CustomItemClick_ViewItem(this, _lstAttachment[position]);
            }
        }
        public void OnItemClick_SaveItem(int position)
        {
            if (CustomItemClick_SaveItem != null)
                CustomItemClick_SaveItem(this, _lstAttachment[position]);
        }
        public void OnItemClick_DeleteItem(int position)
        {
            if (CustomItemClick_DeleteItem != null)
                CustomItemClick_DeleteItem(this, _lstAttachment[position]);
        }
        public void UpdateListData(List<BeanAttachFile> _lstAttachment)
        {
            this._lstAttachment = _lstAttachment;
        }

        public override int ItemCount => _lstAttachment.Count;
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemControlInputAttachmentVertical, parent, false);
            AdapterDetailCreateTask_AttachmentHolder holder = new AdapterDetailCreateTask_AttachmentHolder(itemView, OnCustomItemClick_ViewItem);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                AdapterDetailCreateTask_AttachmentHolder vh = holder as AdapterDetailCreateTask_AttachmentHolder;
                if (position % 2 == 0) // tô màu so le
                {
                    vh._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clVer2BlueNavigation)));
                }
                else
                {
                    vh._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clWhite)));
                }

                if (String.IsNullOrEmpty(_lstAttachment[position].ID)) // file mới -> hiện icon lên
                    vh._imgNewFile.Visibility = ViewStates.Visible;
                else
                    vh._imgNewFile.Visibility = ViewStates.Gone;

                if (!string.IsNullOrEmpty(_lstAttachment[position].Path))
                {
                    string exten = _lstAttachment[position].Path;
                    if (exten != null && (exten.ToLower().Contains(".doc") || exten.ToLower().Contains(".docx")))
                    {
                        vh._imgExtension.SetImageResource(Resource.Drawable.icon_word);
                    }
                    else if (exten != null && (exten.ToLower().Contains(".txt")))
                    {
                        vh._imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_txt);
                    }
                    else if (exten != null && (exten.ToLower().Contains(".png") || exten.ToLower().Contains(".jpeg") || exten.ToLower().Contains(".jpg")))
                    {
                        vh._imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_photo);
                    }
                    else if (exten != null && (exten.ToLower().Contains(".xls") || exten.ToLower().Contains(".xlsx")))
                    {
                        vh._imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_excel);
                    }
                    else if (exten != null && exten.ToLower().Contains(".pdf"))
                    {
                        vh._imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_pdf);
                    }
                    else if (exten != null && exten.ToLower().Contains(".ppt"))
                    {
                        vh._imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_ppt);
                    }
                    else
                    {
                        vh._imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_other);
                    }
                }
                else
                {
                    vh._imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_other);
                }

                if (!string.IsNullOrEmpty(_lstAttachment[position].Title))
                {
                    if (_lstAttachment[position].Title.Contains(";#"))
                    {
                        vh._tvName.Text = _lstAttachment[position].Title.Split(new string[] { ";#" }, StringSplitOptions.None)[0];
                    }
                    else
                    {
                        vh._tvName.Text = _lstAttachment[position].Title;
                    }
                }
                else
                {
                    vh._tvName.Text = "";
                }

                try
                {
                    vh._tvSize.Text = CmmDroidFunction.GetFormatFileSize(_lstAttachment[position].Size);
                }
                catch (Exception)
                {
                    vh._tvSize.Text = "";
                }

                if (!string.IsNullOrEmpty(_lstAttachment[position].CreatedBy))
                {
                    string queryUser = String.Format(CTRLDetailCreateTask._queryBeanUser, _lstAttachment[position].CreatedBy);
                    List<BeanUser> _lstUser = conn.Query<BeanUser>(queryUser);
                    if (_lstUser != null && _lstUser.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_lstUser[0].FullName))
                            vh._tvCategory.Text = _lstUser[0].FullName;
                        else
                            vh._tvCategory.Text = "";

                        if (!String.IsNullOrEmpty(_lstUser[0].ID))
                        {
                            try
                            {
                                string _queryPosition = String.Format(CTRLDetailCreateTask._queryBeanPosition, _lstUser[0].PositionID);
                                List<BeanPosition> _lstPosition = conn.Query<BeanPosition>(_queryPosition);
                                if (_lstPosition != null && _lstPosition.Count > 0)
                                    vh._tvPosition.Text = _lstPosition[0].Title;
                                else
                                    vh._tvPosition.Text = "";
                            }
                            catch (Exception)
                            {
                                vh._tvPosition.Text = "";
                            }

                        }
                        else
                            vh._tvCategory.Text = "";
                    }
                }
                else
                {
                    vh._tvCategory.Text = "";
                    vh._tvPosition.Text = "";
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnBindViewHolder", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        public class AdapterDetailCreateTask_AttachmentHolder : RecyclerView.ViewHolder
        {
            public TextView _tvName { get; set; }
            public TextView _tvSize { get; set; }
            public ImageView _imgExtension { get; set; }
            public ImageView _imgNewFile { get; set; }
            public TextView _tvCategory { get; set; }
            public LinearLayout _lnName { get; set; }
            public LinearLayout _lnCategory { get; set; }
            public LinearLayout _lnAll { get; set; }
            public LinearLayout _lnDelete { get; set; }
            public ImageView _imgDelete { get; set; }
            public TextView _tvPosition { get; set; }
            public AdapterDetailCreateTask_AttachmentHolder(View itemview, Action<int> listener) : base(itemview)
            {
                _tvName = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlInputAttachmentVertical_Name);
                _tvSize = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlInputAttachmentVertical_Size);
                _imgExtension = itemview.FindViewById<ImageView>(Resource.Id.img_ItemControlInputAttachmentVertical_Extension);
                _tvCategory = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlInputAttachmentVertical_Category);
                _tvPosition = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlInputAttachmentVertical_Position);
                _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlInputAttachmentVertical);
                _lnName = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlInputAttachmentVertical_Name);
                _lnCategory = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlInputAttachmentVertical_Category);
                _imgNewFile = itemview.FindViewById<ImageView>(Resource.Id.img_ItemControlInputAttachmentVertical_NewFile);

                _lnAll.Click += (sender, e) =>
                {
                    listener(base.LayoutPosition);
                };
            }
        }
        public class AdapterDetailCreateTask_Attachment_SwipeHelper : MySwipeHelper
        {
            public Context context;
            public List<BeanAttachFile> _lstAttachFile;
            public AdapterDetailCreateTask_Attachment_SwipeHelper(Context context, RecyclerView recyclerView, int buttonWidth, List<BeanAttachFile> _lstAttachFile) : base(context, recyclerView, buttonWidth)
            {
                this.context = context;
                this.recyclerView = recyclerView;
                this.buttonWidth = buttonWidth;
                this._lstAttachFile = _lstAttachFile;
            }

            public override void InstantiateMyButton(RecyclerView.ViewHolder viewHolder, List<UnderLayoutButton> buffer)
            {
                AdapterDetailCreateTask_Attachment _currentAdapter = (AdapterDetailCreateTask_Attachment)recyclerView.GetAdapter();

                if (_lstAttachFile[viewHolder.LayoutPosition].CreatedBy.ToLowerInvariant().Equals(CmmVariable.SysConfig.UserId.ToLowerInvariant())) // thằng tạo file mới dc xóa
                {
                    if (_currentAdapter._flagUserPermission == (int)ControllerDetailCreateTask.FlagUserPermission.Creator
                     || _currentAdapter._flagUserPermission == (int)ControllerDetailCreateTask.FlagUserPermission.Handler
                     || _currentAdapter._flagUserPermission == (int)ControllerDetailCreateTask.FlagUserPermission.CreatorAndHandler)
                    {
                        if (_currentAdapter._parentItem != null && _currentAdapter._parentItem.Status != 2) // chưa done thì dc xóa
                            buffer.Add(new UnderLayoutButton(context, "Delete", 30, Resource.Drawable.icon_ver2_star_controlattch_delete, "#EB342E", new CustomDeleteButtonClick(this, recyclerView, _lstAttachFile), buttonWidth / 3));

                        else if (_currentAdapter._parentItem == null && _currentAdapter._flagUserPermission == (int)ControllerDetailCreateTask.FlagUserPermission.Creator) // người tạo mới vào -> dc xóa
                            buffer.Add(new UnderLayoutButton(context, "Delete", 30, Resource.Drawable.icon_ver2_star_controlattch_delete, "#EB342E", new CustomDeleteButtonClick(this, recyclerView, _lstAttachFile), buttonWidth / 3));

                    }
                }

                buffer.Add(new UnderLayoutButton(context, "Save", 30, Resource.Drawable.icon_ver2_download_small, "#28B0FF", new CustomSaveButtonClick(this, recyclerView, _lstAttachFile), buttonWidth / 3));

                //if (_currentAdapter._flagIsClickFromAction != true) // Click từ Action vào không có action save
                //{
                // }
            }
            /// <summary>
            /// Hàm này override ra để Disable Edit những item mà user không đủ quyền 
            /// </summary>
            /// <param name="recyclerView"></param>
            /// <param name="viewHolder"></param>
            /// <returns></returns>
            public override int GetSwipeDirs(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
            {
                int pos = viewHolder.AdapterPosition;

                AdapterDetailCreateTask_Attachment _currentAdapter = (AdapterDetailCreateTask_Attachment)recyclerView.GetAdapter();
                //if (_currentAdapter._lstAttachment[pos].IsAuthor == true) // Được phép chỉnh sửa
                //{
                return base.GetSwipeDirs(recyclerView, viewHolder);
                //}
                //else
                //{
                //    return 0;
                //}

            }
        }
        public class CustomSaveButtonClick : UnderLayoutButtonListener
        {
            private AdapterDetailCreateTask_Attachment_SwipeHelper myImplementSwipeHelper;
            private ControllerDetailAttachFile CTRLDetailAttachFile = new ControllerDetailAttachFile();
            private List<BeanAttachFile> _lstAttachFile = new List<BeanAttachFile>();
            private RecyclerView recyclerView;
            public CustomSaveButtonClick(AdapterDetailCreateTask_Attachment_SwipeHelper myImplementSwipeHelper, RecyclerView recyclerView, List<BeanAttachFile> _lstAttachFile)
            {
                this.myImplementSwipeHelper = myImplementSwipeHelper;
                this._lstAttachFile = _lstAttachFile;
                this.recyclerView = recyclerView;
            }

            public void OnClick(int position)
            {
                if (position != -1)
                {
                    AdapterDetailCreateTask_Attachment _currentAdapter = (AdapterDetailCreateTask_Attachment)recyclerView.GetAdapter();
                    _currentAdapter.OnItemClick_SaveItem(position);
                }
            }
        }
        internal class CustomDeleteButtonClick : UnderLayoutButtonListener
        {
            private AdapterDetailCreateTask_Attachment_SwipeHelper myImplementSwipeHelper;
            private ControllerDetailAttachFile CTRLDetailAttachFile = new ControllerDetailAttachFile();
            private List<BeanAttachFile> _lstAttachFile = new List<BeanAttachFile>();
            private RecyclerView recyclerView;
            public CustomDeleteButtonClick(AdapterDetailCreateTask_Attachment_SwipeHelper myImplementSwipeHelper, RecyclerView recyclerView, List<BeanAttachFile> _lstAttachFile)
            {
                this.myImplementSwipeHelper = myImplementSwipeHelper;
                this._lstAttachFile = _lstAttachFile;
                this.recyclerView = recyclerView;
            }

            public void OnClick(int position)
            {
                if (position != -1)
                {
                    AdapterDetailCreateTask_Attachment _currentAdapter = (AdapterDetailCreateTask_Attachment)recyclerView.GetAdapter();
                    _currentAdapter.OnItemClick_DeleteItem(position);
                }
            }
        }
    }
}