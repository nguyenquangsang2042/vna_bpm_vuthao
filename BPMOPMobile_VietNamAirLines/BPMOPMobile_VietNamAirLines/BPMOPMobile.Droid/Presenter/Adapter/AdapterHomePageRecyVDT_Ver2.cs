using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using Refractored.Controls;
using SQLite;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    public class AdapterHomePageRecyVDT_Ver2 : RecyclerView.Adapter
    {
        public enum SessionCategory // để xác định xem là group theo gì 
        {
            [Description("Đang xử lý - Local - NT.StartDate")]
            InProcess_Local = 0,
            [Description("Đang xử lý - API - NotifyCreate")]
            InProcess_API = 1,

            [Description("Đã xử lý - Local - Modified")]
            Processed_Local = 2,
            [Description("Đã xử lý - API - Modified")]
            Processed_API = 3,
        }

        private MainActivity _mainAct;
        private Context _context;
        public List<BeanAppBaseExt> _lstAppBase = new List<BeanAppBaseExt>();
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        public event EventHandler<BeanAppBaseExt> CustomItemClick;

        private bool _allowLoadMore = false;
        private int _sessionCategory = (int)SessionCategory.InProcess_Local;
        private int _firstIndexToday = -1;
        private int _firstIndexYesterday = -1;
        private int _firstIndexOlder = -1;
        private ProviderBase p_base = new ProviderBase();

        public AdapterHomePageRecyVDT_Ver2(MainActivity _mainAct, Context _context, List<BeanAppBaseExt> _lstAppBase, int _sessionCategory)
        {
            SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                this._mainAct = _mainAct;
                this._context = _context;
                this._lstAppBase = _lstAppBase;

                switch (_sessionCategory)
                {
                    case (int)SessionCategory.InProcess_Local: // StartDate
                        {
                            _firstIndexToday = _lstAppBase.FindIndex(x => x.StartDate.HasValue && x.StartDate.Value.Date == DateTime.Now.Date);
                            _firstIndexYesterday = _lstAppBase.FindIndex(x => x.StartDate.HasValue && x.StartDate.Value.Date == DateTime.Now.Date.AddDays(-1));
                            _firstIndexOlder = _lstAppBase.FindIndex(x => x.StartDate.HasValue && x.StartDate.Value.Date < DateTime.Now.Date.AddDays(-1));
                            break;
                        }
                    case (int)SessionCategory.InProcess_API: // NotifyCreate
                        {
                            _firstIndexToday = _lstAppBase.FindIndex(x => x.NotifyCreated.HasValue && x.NotifyCreated.Value.Date == DateTime.Now.Date);
                            _firstIndexYesterday = _lstAppBase.FindIndex(x => x.NotifyCreated.HasValue && x.NotifyCreated.Value.Date == DateTime.Now.Date.AddDays(-1));
                            _firstIndexOlder = _lstAppBase.FindIndex(x => x.NotifyCreated.HasValue && x.NotifyCreated.Value.Date < DateTime.Now.Date.AddDays(-1));
                            break;
                        }
                    case (int)SessionCategory.Processed_Local:
                    case (int)SessionCategory.Processed_API:
                        {
                            _firstIndexToday = _lstAppBase.FindIndex(x => x.Modified.HasValue && x.Modified.Value.Date == DateTime.Now.Date);
                            _firstIndexYesterday = _lstAppBase.FindIndex(x => x.Modified.HasValue && x.Modified.Value.Date == DateTime.Now.Date.AddDays(-1));
                            _firstIndexOlder = _lstAppBase.FindIndex(x => x.Modified.HasValue && x.Modified.Value.Date < DateTime.Now.Date.AddDays(-1));
                            break;
                        }
                }

                for (int i = 0; i < _lstAppBase.Count; i++)
                {
                    if (_lstAppBase[i].ResourceCategoryId.Value != 16) // Task không có follow
                    {
                        string _workflowItemID = CmmFunction.GetWorkflowItemIDByUrl(_lstAppBase[i].ItemUrl);
                        string _queryFollow = string.Format(CTRLHomePage._queryFavorite, _workflowItemID);
                        List<BeanWorkflowFollow> _lstFollow = conn.Query<BeanWorkflowFollow>(_queryFollow);
                        if (_lstFollow != null && _lstFollow.Count > 0)
                            _lstAppBase[i].IsFollow = _lstFollow[0].Status == 1 ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "AdapterHomePageRecyVDT", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }
        public AdapterHomePageRecyVDT_Ver2(MainActivity _mainAct, Context _context, List<BeanAppBaseExt> _lstAppBase, int _sessionCategory, bool isScreenFollow)
        {
            SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                this._mainAct = _mainAct;
                this._context = _context;
                this._lstAppBase = _lstAppBase;

                switch (_sessionCategory)
                {
                    case (int)SessionCategory.InProcess_Local: // StartDate
                        {
                            _firstIndexToday = _lstAppBase.FindIndex(x => x.StartDate.HasValue && x.StartDate.Value.Date == DateTime.Now.Date);
                            _firstIndexYesterday = _lstAppBase.FindIndex(x => x.StartDate.HasValue && x.StartDate.Value.Date == DateTime.Now.Date.AddDays(-1));
                            _firstIndexOlder = _lstAppBase.FindIndex(x => x.StartDate.HasValue && x.StartDate.Value.Date < DateTime.Now.Date.AddDays(-1));
                            break;
                        }
                    case (int)SessionCategory.InProcess_API: // NotifyCreate
                        {
                            _firstIndexToday = _lstAppBase.FindIndex(x => x.NotifyCreated.HasValue && x.NotifyCreated.Value.Date == DateTime.Now.Date);
                            _firstIndexYesterday = _lstAppBase.FindIndex(x => x.NotifyCreated.HasValue && x.NotifyCreated.Value.Date == DateTime.Now.Date.AddDays(-1));
                            _firstIndexOlder = _lstAppBase.FindIndex(x => x.NotifyCreated.HasValue && x.NotifyCreated.Value.Date < DateTime.Now.Date.AddDays(-1));
                            break;
                        }
                    case (int)SessionCategory.Processed_Local:
                    case (int)SessionCategory.Processed_API:
                        {
                            _firstIndexToday = _lstAppBase.FindIndex(x => x.Modified.HasValue && x.Modified.Value.Date == DateTime.Now.Date);
                            _firstIndexYesterday = _lstAppBase.FindIndex(x => x.Modified.HasValue && x.Modified.Value.Date == DateTime.Now.Date.AddDays(-1));
                            _firstIndexOlder = _lstAppBase.FindIndex(x => x.Modified.HasValue && x.Modified.Value.Date < DateTime.Now.Date.AddDays(-1));
                            break;
                        }
                }
                List<BeanAppBaseExt> _lstAppBaseTemp = new List<BeanAppBaseExt>();
                for (int i = 0; i < _lstAppBase.Count; i++)
                {
                    if (_lstAppBase[i].ResourceCategoryId.Value != 16) // Task không có follow
                    {
                        string _workflowItemID = CmmFunction.GetWorkflowItemIDByUrl(_lstAppBase[i].ItemUrl);
                        string _queryFollow = string.Format(CTRLHomePage._queryFavorite, _workflowItemID);
                        List<BeanWorkflowFollow> _lstFollow = conn.Query<BeanWorkflowFollow>(_queryFollow);
                        if (_lstFollow != null && _lstFollow.Count > 0)
                            _lstAppBase[i].IsFollow = _lstFollow[0].Status == 1 ? true : false;
                        if (isScreenFollow)
                        {
                            if (_lstAppBase[i].IsFollow)
                            {
                                _lstAppBaseTemp.Add(_lstAppBase[i]);
                            }
                        }
                    }

                }
                if (isScreenFollow)
                {
                    this._lstAppBase.Clear();
                    this._lstAppBase = _lstAppBaseTemp;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "AdapterHomePageRecyVDT", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }
        public void LoadMore(List<BeanAppBaseExt> _lstMore)
        {
            SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                switch (_sessionCategory)
                {
                    case (int)SessionCategory.InProcess_Local: // StartDate
                        {
                            if (_firstIndexToday == -1) _firstIndexToday = _lstAppBase.FindIndex(x => x.StartDate.HasValue && x.StartDate.Value.Date == DateTime.Now.Date);
                            if (_firstIndexYesterday == -1) _firstIndexYesterday = _lstAppBase.FindIndex(x => x.StartDate.HasValue && x.StartDate.Value.Date == DateTime.Now.Date.AddDays(-1));
                            if (_firstIndexOlder == -1) _firstIndexOlder = _lstAppBase.FindIndex(x => x.StartDate.HasValue && x.StartDate.Value.Date < DateTime.Now.Date.AddDays(-1));
                            break;
                        }
                    case (int)SessionCategory.InProcess_API: // NotifyCreate
                        {
                            if (_firstIndexToday == -1) _firstIndexToday = _lstAppBase.FindIndex(x => x.NotifyCreated.HasValue && x.NotifyCreated.Value.Date == DateTime.Now.Date);
                            if (_firstIndexYesterday == -1) _firstIndexYesterday = _lstAppBase.FindIndex(x => x.NotifyCreated.HasValue && x.NotifyCreated.Value.Date == DateTime.Now.Date.AddDays(-1));
                            if (_firstIndexOlder == -1) _firstIndexOlder = _lstAppBase.FindIndex(x => x.NotifyCreated.HasValue && x.NotifyCreated.Value.Date < DateTime.Now.Date.AddDays(-1));
                            break;
                        }
                    case (int)SessionCategory.Processed_Local:
                    case (int)SessionCategory.Processed_API:
                        {
                            if (_firstIndexToday == -1) _firstIndexToday = _lstAppBase.FindIndex(x => x.Modified.HasValue && x.Modified.Value.Date == DateTime.Now.Date);
                            if (_firstIndexYesterday == -1) _firstIndexYesterday = _lstAppBase.FindIndex(x => x.Modified.HasValue && x.Modified.Value.Date == DateTime.Now.Date.AddDays(-1));
                            if (_firstIndexOlder == -1) _firstIndexOlder = _lstAppBase.FindIndex(x => x.Modified.HasValue && x.Modified.Value.Date < DateTime.Now.Date.AddDays(-1));
                            break;
                        }
                }

                for (int i = _lstAppBase.Count - _lstMore.Count; i < _lstAppBase.Count; i++)
                {
                    if (_lstAppBase[i].ResourceCategoryId.Value != 16) // Task không có follow
                    {
                        string _workflowItemID = CmmFunction.GetWorkflowItemIDByUrl(_lstAppBase[i].ItemUrl);
                        string _queryFollow = string.Format(CTRLHomePage._queryFavorite, _workflowItemID);
                        List<BeanWorkflowFollow> _lstFollow = conn.Query<BeanWorkflowFollow>(_queryFollow);
                        if (_lstFollow != null && _lstFollow.Count > 0)
                            _lstAppBase[i].IsFollow = _lstFollow[0].Status == 1 ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "LoadMore", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        public void OnClick(int position)
        {
            if (CustomItemClick != null)
            {
                _lstAppBase[position].Read = true;
                NotifyDataSetChanged();
                CustomItemClick(this, _lstAppBase[position]);
            }
        }

        public void SetAllowLoadMore(bool _allowLoadMore)
        {
            this._allowLoadMore = _allowLoadMore;
        }

        public void UpDateItemFollow(string _workflowItemID, bool _IsFollow)
        {
            // Update Follow - Call khi cần Renew Item
            for (int i = 0; i < _lstAppBase.Count; i++)
            {
                if (_lstAppBase[i].ResourceCategoryId.Value != 16) // Task không có follow
                {
                    string tempworkflowItemID = CmmFunction.GetWorkflowItemIDByUrl(_lstAppBase[i].ItemUrl);
                    if (_workflowItemID.Equals(tempworkflowItemID))
                    {
                        _lstAppBase[i].IsFollow = _IsFollow;
                    }
                }
            }
        }

        public override long GetItemId(int position)
        {
            //return base.GetItemId(position);
            return _lstAppBase[position].ID.GetHashCode();
        }

        public override int ItemCount
        {
            get
            {
                return _lstAppBase.Count;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemAppBaseToDoList, parent, false);
            AdapterHomePageRecyVDT_Holder holder = new AdapterHomePageRecyVDT_Holder(itemView, OnClick, _mainAct);
            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                AdapterHomePageRecyVDT_Holder _holder = holder as AdapterHomePageRecyVDT_Holder;

                #region Category (Color + Group)

                if (position % 2 == 1) // tô màu so le
                    _holder._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clWhite)));
                else
                    _holder._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clVer2BlueNavigation)));


                if (position == _firstIndexYesterday) // Category Hôm qua
                {
                    _holder._tvCategory.Text = CmmFunction.GetTitle("TEXT_YESTERDAY", "Hôm qua");
                    _holder._lnCategory.Visibility = ViewStates.Visible;
                    _holder._viewCategory.Visibility = ViewStates.Visible;
                }
                else if (position == _firstIndexOlder) // Category Cũ hơn
                {
                    _holder._tvCategory.Text = CmmFunction.GetTitle("TEXT_OLDER", "Cũ hơn");
                    _holder._lnCategory.Visibility = ViewStates.Visible;
                    _holder._viewCategory.Visibility = ViewStates.Visible;
                }
                else // Category Hôm nay - còn lại ko hiện
                {
                    _holder._lnCategory.Visibility = ViewStates.Gone;
                    _holder._viewCategory.Visibility = ViewStates.Gone;
                }

                #endregion
                BeanAppBaseExt _currentAppBase = _lstAppBase[position];
                /*try
                {
                    p_base.UpdateItemDataNewLoading(_currentAppBase, conn);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("UpdateItemDataNewLoading" + ex);
                }*/
                bool _IsTask = false;
                if (_currentAppBase.ResourceCategoryId.HasValue && _currentAppBase.ResourceSubCategoryId.HasValue)
                {
                    _IsTask = (_currentAppBase.ResourceCategoryId.Value == 16 && _currentAppBase.ResourceSubCategoryId == 0) ? true : false;
                }

                #region Line 1 

                // -- Avatar

                if (!String.IsNullOrEmpty(_currentAppBase.UserImage))
                {
                    CmmDroidFunction.SetContentToImageView(_mainAct, _holder._imgAvatar, _currentAppBase.UserImage, 50);
                }
                else
                {
                    #region get image avatar from beanUser
                    if (!String.IsNullOrEmpty(_currentAppBase.AssignedBy))
                    {
                        //List<BeanUser> _lstUser = conn.Query<BeanUser>(String.Format("SELECT ImagePath FROM BeanUser WHERE ID = '{0}' LIMIT 1 OFFSET 0", _currentAppBase.AssignedBy.ToLowerInvariant()));
                        //if (_lstUser != null && _lstUser.Count > 0)
                        //    CmmDroidFunction.SetContentToImageView(_mainAct, _holder._imgAvatar, _lstUser[0].ImagePath, 50);
                        //else
                        //    _holder._imgAvatar.SetImageResource(Resource.Drawable.icon_avatar64);

                        string _queryBeanUserGroup = @"SELECT Image as ImagePath, 1 as Type FROM BeanGroup WHERE ID = '{0}'
                                                  UNION SELECT ImagePath, 0 as Type FROM BeanUser WHERE ID = '{0}'";

                        List<BeanUserAndGroup> _lstUserAngroup = conn.Query<BeanUserAndGroup>(String.Format(_queryBeanUserGroup, _currentAppBase.AssignedBy.ToLowerInvariant()));

                        if (_lstUserAngroup != null && _lstUserAngroup.Count > 0)
                        {
                            if (_lstUserAngroup[0].Type == 0) // User
                            {
                                CmmDroidFunction.SetContentToImageView(_mainAct, _holder._imgAvatar, _lstUserAngroup[0].ImagePath, 50);
                            }
                            else
                                _holder._imgAvatar.SetImageResource(Resource.Drawable.icon_ver2_group);
                        }
                    }
                    else
                    {
                        _holder._imgAvatar.SetImageResource(Resource.Drawable.icon_avatar64);
                    }
                    #endregion
                }
                _holder._tvTitle.Text = _currentAppBase.Content;

                // -- IsRead
                if (_currentAppBase.Read == true) // Đã đọc
                    _holder._tvTitle.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                else // chưa đọc
                    _holder._tvTitle.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);

                // -- Created
                if (_currentAppBase.Created.HasValue)
                    _holder._tvTime.Text = CmmFunction.GetStringDateTimeLang(_currentAppBase.Created.Value, 0, int.Parse(CmmVariable.SysConfig.LangCode));
                else
                    _holder._tvTime.Visibility = ViewStates.Invisible;
                #endregion

                #region Line 2

                // -- Subtitle
                if (_IsTask)
                {
                    _holder._tvDescription.Text = CmmFunction.GetTitle("TEXT_TASK", "Công việc");
                }
                else
                {
                    if (!string.IsNullOrEmpty(_currentAppBase.WorkflowTitle) && !string.IsNullOrEmpty(_currentAppBase.WorkflowTitleEN))
                        _holder._tvDescription.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) // Content Line 2
                                  ? String.IsNullOrEmpty(_currentAppBase.WorkflowTitle) ? "" : _currentAppBase.WorkflowTitle
                                  : String.IsNullOrEmpty(_currentAppBase.WorkflowTitleEN) ? "" : _currentAppBase.WorkflowTitleEN;
                    else
                    {
                        #region get workflow title from beanWorkflow
                        if (_currentAppBase.WorkflowId.HasValue)
                        {
                            string _query = String.Format(CTRLHomePage._queryWorkflow_ByID, _currentAppBase.WorkflowId); // "SELECT Title, TitleEN FROM BeanWorkflow WHERE WorkflowID = ?";
                            List<BeanWorkflow> _lstWorkflow = conn.Query<BeanWorkflow>(_query);
                            if (_lstWorkflow != null && _lstWorkflow.Count > 0)
                            {
                                _holder._tvDescription.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) // Content Line 2
                                  ? _lstWorkflow[0].Title
                                  : _lstWorkflow[0].TitleEN;
                            }
                            else
                                _holder._tvDescription.Text = "";
                        }
                        else
                        {
                            _holder._tvDescription.Text = "";
                        }
                        #endregion
                    }

                }

                ////// -- Priority Flag
                ////if (_currentAppBase.Priority == 3)
                ////    _holder._imgFlag.Visibility = ViewStates.Visible;
                ////else
                ////    _holder._imgFlag.Visibility = ViewStates.Gone;

                // -- Image Attachment
                if (_currentAppBase.FileCount.HasValue && _currentAppBase.FileCount > 0)
                    _holder._imgAttach.Visibility = ViewStates.Visible;
                else
                    _holder._imgAttach.Visibility = ViewStates.Invisible;

                _holder._imgFlag.Visibility = ViewStates.Gone;

                // -- Image Follow Star
                if (_currentAppBase.IsFollow == true)
                    _holder._imgFavorite.SetImageResource(Resource.Drawable.icon_ver2_star_checked);
                else
                    _holder._imgFavorite.SetImageResource(Resource.Drawable.icon_ver2_star_unchecked);

                #endregion

                #region Line 3
                if (_currentAppBase.StatusGroup != null)
                {

                    if (!string.IsNullOrEmpty(_currentAppBase.StatusText)&& !string.IsNullOrEmpty(_currentAppBase.StatusTextEN))
                        if (!String.IsNullOrEmpty(_currentAppBase.StatusText) && !String.IsNullOrEmpty(_currentAppBase.StatusTextEN))
                        {
                            _holder._tvStatus.Visibility = ViewStates.Visible;
                            _holder._tvStatus.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? _currentAppBase.StatusText : _currentAppBase.StatusTextEN;
                            _holder._tvStatus.BackgroundTintList = ColorStateList.ValueOf(CTRLHomePage.GetColorByAppStatus(_context, _currentAppBase.StatusGroup.Value));
                        }
                        else
                            _holder._tvStatus.Visibility = ViewStates.Invisible;
                    else
                    {
                        #region get status old
                        string query = string.Format("SELECT Title, TitleEN FROM BeanAppStatus WHERE ID = {0} LIMIT 1 OFFSET 0", _currentAppBase.StatusGroup);
                        List<BeanAppStatus> _lstAppStatus = conn.Query<BeanAppStatus>(query);
                        if (_lstAppStatus != null && _lstAppStatus.Count > 0)
                        {
                            _holder._tvStatus.Visibility = ViewStates.Visible;
                            _holder._tvStatus.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? _lstAppStatus[0].Title : _lstAppStatus[0].TitleEN;
                            _holder._tvStatus.BackgroundTintList = ColorStateList.ValueOf(CTRLHomePage.GetColorByAppStatus(_context, _currentAppBase.StatusGroup.Value));
                        }
                        else
                            _holder._tvStatus.Visibility = ViewStates.Invisible;
                        #endregion
                    }
                }
                else
                {
                    _holder._tvStatus.Visibility = ViewStates.Invisible;
                }

                if (_currentAppBase.DueDate.HasValue)
                {
                    _holder._tvStatusTime.SetTextColor(CTRLHomePage.GetColorByDueDate(_context, _currentAppBase.DueDate.Value));
                    //_holder._tvStatusTime.Text = CmmFunction.GetStringDateTimeLang(_currentAppBase.DueDate.Value, 1, int.Parse(CmmVariable.SysConfig.LangCode));
                    if (CmmVariable.SysConfig.LangCode == "1066")
                        _holder._tvStatusTime.Text = _currentAppBase.DueDate.Value.ToString("dd/MM/yy HH:mm");
                    else
                        _holder._tvStatusTime.Text = _currentAppBase.DueDate.Value.ToString("MM/dd/yy HH:mm");
                }
                else
                    _holder._tvStatusTime.Text = "";

                if (_IsTask)
                {
                    // -- Duedate (nếu task complete -> ẩn đi)
                    if (_currentAppBase.DueDate.HasValue && _currentAppBase.StatusGroup.HasValue &&
                        _currentAppBase.StatusGroup != (int)CmmFunction.AppStatusID.Completed)
                    {
                        _holder._tvStatusTime.SetTextColor(CTRLHomePage.GetColorByDueDate(_context, _currentAppBase.DueDate.Value));
                        //_holder._tvStatusTime.Text = CmmFunction.GetStringDateTimeLang(_currentAppBase.DueDate.Value, 1, int.Parse(CmmVariable.SysConfig.LangCode));
                        if (CmmVariable.SysConfig.LangCode == "1066")
                            _holder._tvStatusTime.Text = _currentAppBase.DueDate.Value.ToString("dd/MM/yy HH:mm");
                        else
                            _holder._tvStatusTime.Text = _currentAppBase.DueDate.Value.ToString("MM/dd/yy HH:mm");

                    }
                    else
                        _holder._tvStatusTime.Text = "";
                }
                else
                {
                    // -- Duedate
                    if (_currentAppBase.DueDate.HasValue && _currentAppBase.StatusGroup.HasValue)
                    {
                        switch (_currentAppBase.StatusGroup) // Đã hoàn tất hoặc (da phe duyet - da huy - Từ chối) -> ko hiện Duedate
                        {
                            case (int)CmmFunction.AppStatusID.Completed: // Đã phê duyệt
                            case (int)CmmFunction.AppStatusID.Canceled: // Đã hủy
                            case (int)CmmFunction.AppStatusID.Rejected: // Đã từ chối
                                _holder._tvStatusTime.Text = "";
                                break;
                            default:
                                _holder._tvStatusTime.SetTextColor(CTRLHomePage.GetColorByDueDate(_context, _currentAppBase.DueDate.Value));
                                //_holder._tvStatusTime.Text = CmmFunction.GetStringDateTimeLang(_currentAppBase.DueDate.Value, 1, int.Parse(CmmVariable.SysConfig.LangCode));
                                if (CmmVariable.SysConfig.LangCode == "1066")
                                    _holder._tvStatusTime.Text = _currentAppBase.DueDate.Value.ToString("dd/MM/yy HH:mm");
                                else
                                    _holder._tvStatusTime.Text = _currentAppBase.DueDate.Value.ToString("MM/dd/yy HH:mm"); break;
                        }
                    }
                    else
                        _holder._tvStatusTime.Text = "";
                }


                #endregion

                #region Load more
                if (position == _lstAppBase.Count - 1 && _allowLoadMore == true)
                    _holder._lnLoadMore.Visibility = ViewStates.Visible;
                else
                    _holder._lnLoadMore.Visibility = ViewStates.Gone;
                #endregion
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

        public class AdapterHomePageRecyVDT_Holder : RecyclerView.ViewHolder
        {
            public LinearLayout _lnAll { get; set; }
            // Category
            public LinearLayout _lnCategory { get; set; }
            public TextView _tvCategory { get; set; }
            public View _viewCategory { get; set; }
            // Line 1
            public CircleImageView _imgAvatar { get; set; }
            public TextView _tvTitle { get; set; }
            public TextView _tvTime { get; set; }
            // Line 2
            public TextView _tvDescription { get; set; }
            public ImageView _imgFlag { get; set; }
            public ImageView _imgFavorite { get; set; }
            public ImageView _imgAttach { get; set; }
            // Line 3
            public TextView _tvStatus { get; set; }
            public TextView _tvStatusTime { get; set; }
            public LinearLayout _lnLoadMore { get; set; }
            public AdapterHomePageRecyVDT_Holder(View itemview, Action<int> listener, AppCompatActivity _mainAct) : base(itemview)
            {
                _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemAppBaseToDoList_All);
                _lnCategory = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemAppBaseToDoList_Category);
                _tvCategory = itemview.FindViewById<TextView>(Resource.Id.tv_ItemAppBaseToDoList_Category);
                _viewCategory = itemview.FindViewById<View>(Resource.Id.view_ItemAppBaseToDoList_Category);
                _imgAvatar = itemview.FindViewById<CircleImageView>(Resource.Id.img_ItemAppBaseToDoList_Avatar);
                _tvTitle = itemview.FindViewById<TextView>(Resource.Id.tv_ItemAppBaseToDoList_Title);
                _tvTime = itemview.FindViewById<TextView>(Resource.Id.tv_ItemAppBaseToDoList_Time);
                _tvDescription = itemview.FindViewById<TextView>(Resource.Id.tv_ItemAppBaseToDoList_Description);
                _imgFavorite = itemview.FindViewById<ImageView>(Resource.Id.img_ItemAppBaseToDoList_Favorite);
                _imgFlag = itemview.FindViewById<ImageView>(Resource.Id.img_ItemAppBaseToDoList_Flag);
                _imgAttach = itemview.FindViewById<ImageView>(Resource.Id.img_ItemAppBaseToDoList_AttachFile);
                _tvStatus = itemview.FindViewById<TextView>(Resource.Id.tv_ItemAppBaseToDoList_Status);
                _tvStatusTime = itemview.FindViewById<TextView>(Resource.Id.tv_ItemAppBaseToDoList_StatusTime);
                _lnLoadMore = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemAppBaseToDoList_LoadMore);

                _lnCategory.Click += (sender, e) =>
                {
                    CmmDroidFunction.HideSoftKeyBoard(_mainAct);
                };
                _lnAll.Click += (sender, e) =>
                {
                    listener(base.LayoutPosition);
                };
            }
        }
    }
}