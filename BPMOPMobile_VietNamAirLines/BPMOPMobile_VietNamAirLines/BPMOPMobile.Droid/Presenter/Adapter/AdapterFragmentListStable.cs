using System;
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
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using Newtonsoft.Json.Linq;
using Refractored.Controls;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    class AdapterFragmentListStable : RecyclerView.Adapter
    {
        private MainActivity _mainAct;
        private Context _context;
        private List<JObject> _lstData = new List<JObject>();
        private List<BeanWFDetailsHeader> _lstHeader = new List<BeanWFDetailsHeader>();
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        public event EventHandler<JObject> CustomItemClick;

        private BeanWFDetailsHeader _headerTitle = null;
        private BeanWFDetailsHeader _headerAssignedTo = null;
        private BeanWFDetailsHeader _headerCreated = null;
        private BeanWFDetailsHeader _headerStatus = null;
        private BeanWFDetailsHeader _headerIsFollow = null;
        private BeanWFDetailsHeader _headerFileCount = null;
        private BeanWFDetailsHeader _headerDuedate = null;

        private bool _allowLoadMore = false;
        //private int _firstIndexToday = -1;
        private int _firstIndexYesterday = -1;
        private int _firstIndexOlder = -1;

        public AdapterFragmentListStable(Context _context, List<BeanWFDetailsHeader> _lstHeader, List<JObject> _lstData, MainActivity _mainAct)
        {
            try
            {
                this._context = _context;
                this._lstHeader = _lstHeader;
                this._lstData = _lstData;
                this._mainAct = _mainAct;

                foreach (var item in _lstHeader)
                {
                    switch (item.internalName.ToLowerInvariant())
                    {
                        case "title":
                        case "tieude":
                            _headerTitle = item; break;
                        case "assignedto":
                            _headerAssignedTo = item; break;
                        case "created":
                            _headerCreated = item; break;
                        case "status":
                            _headerStatus = item; break;
                        case "isfollow":
                            _headerIsFollow = item; break;
                        case "filecount":
                            _headerFileCount = item; break;
                        case "duedate":
                            _headerDuedate = item; break;
                    }
                }


                //_firstIndexToday = _lstData.FindIndex(x => x.Created.Value.Date == DateTime.Now.Date);
                //_firstIndexYesterday = _lstData.FindIndex(x => x.Created.Value.Date == DateTime.Now.Date.AddDays(-1));
                //_firstIndexOlder = _lstData.FindIndex(x => x.Created.Value.Date < DateTime.Now.Date.AddDays(-1));


                //SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                //for (int i = 0; i < _lstData.Count; i++)
                //{
                //    string _queryFollow = string.Format(CTRLHomePage._queryFavorite, _lstData[i].ID);
                //    List<BeanWorkflowFollow> _lstFollow = conn.Query<BeanWorkflowFollow>(_queryFollow);
                //    if (_lstFollow != null && _lstFollow.Count > 0)
                //        _lstData[i].IsFollow = _lstFollow[0].Status == 1 ? true : false;
                //}
                //conn.Close();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "AdapterFragmentList", ex);
#endif
            }
        }

        public void OnClick(int position)
        {
            if (CustomItemClick != null)
                CustomItemClick(this, _lstData[position]);
        }
        public void SetAllowLoadMore(bool _allowLoadMore)
        {
            this._allowLoadMore = _allowLoadMore;
        }
        public void LoadMore(List<JObject> _lstMore)
        {
            try
            {
                //if (_firstIndexToday == -1) _firstIndexToday = _lstData.FindIndex(x => x.Created.Value.Date == DateTime.Now.Date);
                //if (_firstIndexYesterday == -1) _firstIndexYesterday = _lstData.FindIndex(x => x.Created.Value.Date == DateTime.Now.Date.AddDays(-1));
                //if (_firstIndexOlder == -1) _firstIndexOlder = _lstData.FindIndex(x => x.Created.Value.Date < DateTime.Now.Date.AddDays(-1));

                //SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                //for (int i = _lstData.Count - _lstMore.Count; i < _lstData.Count; i++)
                //{
                //    string _queryFollow = string.Format(CTRLHomePage._queryFavorite, _lstData[i].ID);
                //    List<BeanWorkflowFollow> _lstFollow = conn.Query<BeanWorkflowFollow>(_queryFollow);
                //    if (_lstFollow != null && _lstFollow.Count > 0)
                //        _lstData[i].IsFollow = _lstFollow[0].Status == 1 ? true : false;
                //}
                //conn.Close();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "AdapterHomePageRecyVTBD", ex);
#endif
            }
        }
        public void UpDateItemFollow(JObject _itemUpdate)
        {
            //// Update Follow
            //// Call khi cần Renew Item
            //for (int i = 0; i < _lstData.Count; i++)
            //{
            //    if (_lstData[i].ID.Equals(_itemUpdate.ID))
            //    {
            //        _lstData[i].IsFollow = _itemUpdate.IsFollow;
            //        break;
            //    }
            //}
        }

        public override int ItemCount
        {
            get
            {
                return _lstData.Count;
            }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemAppBaseToDoList, parent, false);
            AdapterFragmentListStable_Holder holder = new AdapterFragmentListStable_Holder(itemView, OnClick, _mainAct);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                AdapterFragmentListStable_Holder _holder = holder as AdapterFragmentListStable_Holder;

                JObject _currentItem = _lstData[position];

                #region Category

                if (position % 2 == 1) // tô màu so le
                    _holder._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clWhite)));
                else
                    _holder._lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clVer2BlueNavigation)));

                if (position == _firstIndexYesterday) // Category Hôm qua
                {
                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                        _holder._tvCategory.Text = CmmFunction.GetTitle("TEXT_YESTERDAY", "Hôm qua");
                    else
                        _holder._tvCategory.Text = CmmFunction.GetTitle("TEXT_YESTERDAY", "Yesterday");

                    _holder._lnCategory.Visibility = ViewStates.Visible;
                    _holder._viewCategory.Visibility = ViewStates.Visible;
                }
                else if (position == _firstIndexOlder) // Category Cũ hơn
                {
                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                        _holder._tvCategory.Text = CmmFunction.GetTitle("TEXT_OLDER", "Cũ hơn");
                    else
                        _holder._tvCategory.Text = CmmFunction.GetTitle("TEXT_OLDER", "Older");

                    _holder._lnCategory.Visibility = ViewStates.Visible;
                    _holder._viewCategory.Visibility = ViewStates.Visible;
                }
                else // Category Hôm nay - còn lại ko hiện
                {
                    _holder._lnCategory.Visibility = ViewStates.Gone;
                    _holder._viewCategory.Visibility = ViewStates.Gone;
                }

                #endregion

                #region Line 1

                // Avatar
                _holder._imgAvatar.SetImageResource(Resource.Drawable.icon_avatar64);
                if (_headerAssignedTo != null)
                {
                    string value = CmmFunction.GetRawValueByHeader(_headerAssignedTo, _currentItem);
                    if (!String.IsNullOrEmpty(value))
                    {
                        string[] valueSearch = value.Trim().ToLowerInvariant().Split(",");
                        string _queryBeanUserGroup = @"SELECT Image as ImagePath, 1 as Type FROM BeanGroup WHERE ID = '{0}'
                                             UNION SELECT ImagePath, 0 as Type FROM BeanUser WHERE ID = '{0}'";
                        List<BeanUserAndGroup> _lstUserAngroup = conn.Query<BeanUserAndGroup>(String.Format(_queryBeanUserGroup, valueSearch[0].ToLowerInvariant()));
                        if (_lstUserAngroup != null && _lstUserAngroup.Count > 0)
                        {
                            if (_lstUserAngroup[0].Type == 0) // User
                                CmmDroidFunction.SetContentToImageView(_mainAct, _holder._imgAvatar, _lstUserAngroup[0].ImagePath, 50);
                            else // Group
                                _holder._imgAvatar.SetImageResource(Resource.Drawable.icon_ver2_group);
                        }
                    }
                }

                // Title
                if (_headerTitle != null)
                    _holder._tvTitle.Text = CmmFunction.GetRawValueByHeader(_headerTitle, _currentItem);
                else
                    _holder._tvTitle.Text = "";

                // Created
                _holder._tvTime.Text = "";
                if (_headerCreated != null)
                {
                    string value = CmmFunction.GetRawValueByHeader(_headerCreated, _currentItem);
                    if (!String.IsNullOrEmpty(value))
                    {
                        DateTime _created = DateTime.Parse(value);
                        _holder._tvTime.Text = CmmFunction.GetStringDateTimeLang(_created, 0, int.Parse(CmmVariable.SysConfig.LangCode));
                    }
                }
                #endregion

                #region Line 2

                // Description
                _holder._tvDescription.Visibility = ViewStates.Invisible;
                if (_headerAssignedTo != null && _headerStatus != null)
                {
                    string valueStatus = CmmFunction.GetRawValueByHeader(_headerStatus, _currentItem);
                    string valueAssignedTo = CmmFunction.GetRawValueByHeader(_headerAssignedTo, _currentItem);

                    if (!String.IsNullOrEmpty(valueStatus) && !String.IsNullOrEmpty(valueAssignedTo))
                    {
                        string[] _lstFullName = CTRLHomePage.GetArrayFullNameFromArrayID(valueAssignedTo.ToString().Trim().ToLowerInvariant().Split(","));

                        if (_lstFullName != null && _lstFullName.Length > 0)
                        {
                            _holder._tvDescription.Visibility = ViewStates.Visible;
                            if (int.Parse(valueStatus) == (int)CmmFunction.AppStatusID.Completed) // da phe duyet
                                _holder._tvDescription.Text = CmmFunction.GetTitle("TEXT_TITLE_APPROVED", "Phê duyệt: ") + _lstFullName[0];
                            else if (int.Parse(valueStatus) == (int)CmmFunction.AppStatusID.Canceled) // da huy
                                _holder._tvDescription.Text = CmmFunction.GetTitle("TEXT_TITLE_CANCEL", "Hủy: ") + _lstFullName[0];
                            else if (int.Parse(valueStatus) == (int)CmmFunction.AppStatusID.Rejected) // Từ chối
                                _holder._tvDescription.Text = CmmFunction.GetTitle("TEXT_TITLE_REJECTED", "Từ chối: ") + _lstFullName[0];
                            else
                                CTRLHomePage.SetTextView_FormatMultiUser2(_context, _holder._tvDescription, _lstFullName, IsHighLightColor: false);
                        }
                    }
                }

                // IsFollow
                _holder._imgFavorite.SetImageResource(Resource.Drawable.icon_ver2_star_unchecked);
                if (_headerIsFollow != null)
                {
                    string value = CmmFunction.GetRawValueByHeader(_headerIsFollow, _currentItem);
                    if (value.ToString().Equals("1"))
                        _holder._imgFavorite.SetImageResource(Resource.Drawable.icon_ver2_star_checked);
                }

                _holder._imgAttach.Visibility = ViewStates.Invisible;
                if (_headerFileCount != null)
                {
                    string value = CmmFunction.GetRawValueByHeader(_headerFileCount, _currentItem);
                    int _res = -1;
                    if (!String.IsNullOrEmpty(value))
                    {
                        int.TryParse(value, out _res);
                        if (_res >0)
                            _holder._imgAttach.Visibility = ViewStates.Visible;
                    }
                }

                _holder._imgFlag.Visibility = ViewStates.Gone;
                #endregion

                #region Line 3

                // Status + Duedate
                _holder._tvStatus.Visibility = ViewStates.Invisible;
                _holder._tvStatusTime.Text = "";
                if (_headerStatus != null)
                {
                    string value = CmmFunction.GetRawValueByHeader(_headerStatus, _currentItem);
                    string query = string.Format("SELECT ID, Title, TitleEN FROM BeanAppStatus WHERE ID = {0} LIMIT 1 OFFSET 0", !String.IsNullOrEmpty(value) ? value : "-123");
                    List<BeanAppStatus> _lstAppStatus = conn.Query<BeanAppStatus>(query);

                    if (_lstAppStatus != null && _lstAppStatus.Count > 0)
                    {
                        _holder._tvStatus.Visibility = ViewStates.Visible;
                        _holder._tvStatus.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? _lstAppStatus[0].Title : _lstAppStatus[0].TitleEN;
                        _holder._tvStatus.BackgroundTintList = ColorStateList.ValueOf(CTRLHomePage.GetColorByAppStatus(_context, _lstAppStatus[0].ID));

                        #region Handle Duedate
                        if (_headerDuedate != null)
                        {
                            string valueDueDate = CmmFunction.GetRawValueByHeader(_headerDuedate, _currentItem);
                            switch (_lstAppStatus[0].ID) // da phe duyet - da huy - Từ chối ko hiện
                            {
                                case (int)CmmFunction.AppStatusID.Completed: // Đã phê duyệt
                                case (int)CmmFunction.AppStatusID.Canceled: // Đã hủy
                                case (int)CmmFunction.AppStatusID.Rejected: // Đã từ chối
                                    _holder._tvStatusTime.Text = "";
                                    break;
                                default:
                                    DateTime _dueDate = DateTime.Parse(valueDueDate.ToString());
                                    _holder._tvStatusTime.SetTextColor(CTRLHomePage.GetColorByDueDate(_context, _dueDate));
                                    _holder._tvStatusTime.Text = CmmFunction.GetStringDateTimeLang(_dueDate, 1, int.Parse(CmmVariable.SysConfig.LangCode));
                                    break;
                            }
                        }
                        #endregion
                    }
                }
                #endregion

                #region Load more
                if (position == _lstData.Count - 1 && _allowLoadMore == true)
                    _holder._lnLoadMore.Visibility = ViewStates.Visible;
                else
                    _holder._lnLoadMore.Visibility = ViewStates.Gone;
                #endregion
            }
            catch (Exception ex)
            {

            }
            finally
            {
                conn.Close();
            }
        }

        public class AdapterFragmentListStable_Holder : RecyclerView.ViewHolder
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

            public AdapterFragmentListStable_Holder(View itemview, Action<int> listener, AppCompatActivity _mainAct) : base(itemview)
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