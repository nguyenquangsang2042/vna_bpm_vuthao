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
using Android.Support.V4.Content.Res;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Fragment;
using Newtonsoft.Json;
using Refractored.Controls;
using SQLite;
using static BPMOPMobile.Droid.Class.MinionAction;
using static BPMOPMobile.Droid.Core.Class.MinionActionCore;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    public class AdapterExpandComment : BaseExpandableListAdapter
    {
        MainActivity _mainAct;
        private Context _context;
        private List<BeanExpandComment> _lstData = new List<BeanExpandComment>();
        private List<BeanAttachFile> _lstAttachParent = new List<BeanAttachFile>();
        private ControllerDetailProcess CTRLDetailProcess = new ControllerDetailProcess();
        private ControllerDetailCreateTask CTRLDetailCreateTask = new ControllerDetailCreateTask();
        public event EventHandler<BeanComment> CustomItemClick_Like;
        public event EventHandler<BeanComment> CustomItemClick_Reply;
        public event EventHandler<BeanAttachFile> CustomItemClick_Attach_Detail;
        public event EventHandler<BeanAttachFile> CustomItemClick_Attach_Delete;

        public event EventHandler CustomItemClick_CommentParent_ImgAttach;
        public event EventHandler<CommentEventArgs> CustomItemClick_CommentParent;

        public string _parentComment = "";
        public bool _haveEvent = false;
        public bool _IsReplyView = true;

        private void OnItemClick_Like(BeanComment item)
        {
            if (CustomItemClick_Like != null)
                CustomItemClick_Like(this, item);
        }
        private void OnItemClick_Reply(BeanComment item)
        {
            if (CustomItemClick_Reply != null)
                CustomItemClick_Reply(this, item);
        }
        private void OnItemClick_CommentParent_ImgAttach()
        {
            if (CustomItemClick_CommentParent_ImgAttach != null)
                CustomItemClick_CommentParent_ImgAttach(this, null);
        }
        private void OnItemClick_CommentParent(string content)
        {
            if (CustomItemClick_CommentParent != null)
                CustomItemClick_CommentParent(this, new CommentEventArgs(content, _lstAttachParent));
        }
        public void UpdateListAttachParent(List<BeanAttachFile> _lstAttachParent)
        {
            this._lstAttachParent = _lstAttachParent;
        }
        public AdapterExpandComment(MainActivity _mainAct, Context _context, List<BeanExpandComment> _lstData, bool _IsReplyView)
        {
            this._mainAct = _mainAct;
            this._context = _context;
            this._lstData = _lstData;
            this._IsReplyView = _IsReplyView;
        }
        public override int GroupCount
        {
            get
            {
                if (_lstData != null && _lstData.Count > 0)
                {
                    return _lstData.Count;
                }
                return 1; // để hiển cmt lên
            }
        }

        public override bool HasStableIds
        {
            get
            {
                return false;
            }
        }

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
            if (_lstData.Count > 0)
                if (_lstData[groupPosition].lstChild != null && _lstData[groupPosition].lstChild.Count > 0)
                    return _lstData[groupPosition].lstChild.Count;
            return 0;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            //if (convertView == null)
            //{
            LayoutInflater mInflater = LayoutInflater.From(_context);
            convertView = mInflater.Inflate(Resource.Layout.ItemExpandDetailCreateTask_Comment_Group, null);
            //}
            View _vwMarginLeft = convertView.FindViewById<View>(Resource.Id.vw_ItemExpandDetailCreateTask_Comment_Group_MarginLeft);
            TextView _tvTitle = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_Title);
            LinearLayout _lnParentComment = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailCreateTask_Comment_Group_ParentComment);
            EditText _edtParentComment = convertView.FindViewById<EditText>(Resource.Id.edt_ItemExpandDetailCreateTask_Comment_Group_ParentComment);
            ImageView _imgAttachParent = convertView.FindViewById<ImageView>(Resource.Id.img_ItemExpandDetailCreateTask_Comment_Group_ParentComment_Attach);
            TextView _tvAttachCount = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_AttachCount);
            ImageView _imgCommentParent = convertView.FindViewById<ImageView>(Resource.Id.img_ItemExpandDetailCreateTask_Comment_Group_ParentComment_Comment);
            LinearLayout _lnAttach = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailCreateTask_Comment_Group_Attach);
            LinearLayout _lnLike = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailCreateTask_Comment_Group_Like);
            CircleImageView _imgAvatar = convertView.FindViewById<CircleImageView>(Resource.Id.img_ItemExpandDetailCreateTask_Comment_Group_Avata);
            TextView _tvAvatar = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_Avata);
            TextView _tvName = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_Name);
            TextView _tvDate = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_Date);
            TextView _tvPosition = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_Position);
            TextView _tvComment = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_Comment);
            TextView _tvLike = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_Like);
            TextView _tvReply = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_Reply);
            TextView _tvLikeCount = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_LikeCount);
            LinearLayout _lnAction = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailCreateTask_Comment_Group_Action);
            RecyclerView _recyAttach = convertView.FindViewById<RecyclerView>(Resource.Id.recy_ItemExpandDetailCreateTask_Comment_Group_Attach);

            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
            {
                _tvLike.Text = CmmFunction.GetTitle("TEXT_LIKE", "Thích");
                _tvReply.Text = CmmFunction.GetTitle("TEXT_REPLY", "Trả lời");
            }
            else
            {
                _tvLike.Text = CmmFunction.GetTitle("TEXT_LIKE", "Like");
                _tvReply.Text = CmmFunction.GetTitle("TEXT_REPLY", "Reply");
            }

            _tvTitle.Visibility = ViewStates.Gone;
            _vwMarginLeft.Visibility = ViewStates.Visible;
            _lnParentComment.Visibility = ViewStates.Gone;

            BeanComment _currentItem = _lstData[groupPosition].lstChild[childPosition];

            if (_IsReplyView == true) // Reply thi hien len
            {
                _lnAction.Visibility = ViewStates.Visible;
                _tvLike.Click += (sender, e) =>
                {
                    OnItemClick_Like(_currentItem);
                };
                _tvReply.Click += (sender, e) =>
                {
                    OnItemClick_Reply(_currentItem);
                };
            }
            else
            {
                _lnAction.Visibility = ViewStates.Gone;
            }

            BeanUser _authorUser = CmmFunction.GetBeanUserByID(_currentItem.Author);

            if (_authorUser != null && !String.IsNullOrEmpty(_authorUser.ID))
                CmmDroidFunction.SetAvataByBeanUser(_mainAct, _context, _authorUser.ID, "ID", _imgAvatar, _tvAvatar, 50);
            else
            {
                _tvAvatar.Visibility = ViewStates.Invisible;
                _imgAvatar.Visibility = ViewStates.Invisible;
            }

            if (_authorUser != null && !String.IsNullOrEmpty(_authorUser.FullName))
                _tvName.Text = _authorUser.FullName;
            else
                _tvName.Text = "";

            if (_currentItem.Created.HasValue)
                _tvDate.Text = CTRLDetailProcess.GetFormatDateLang(_currentItem.Created.Value);
            else
                _tvDate.Text = "";

            if (_authorUser != null)
            {
                SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
                string _queryPosition = String.Format(CTRLDetailCreateTask._queryBeanPosition, _authorUser.PositionID);
                List<BeanPosition> _lstPosition = conn.Query<BeanPosition>(_queryPosition);
                conn.Close();

                if (_lstPosition != null && _lstPosition.Count > 0)
                    _tvPosition.Text = _lstPosition[0].Title;
                else
                    _tvPosition.Text = "";
            }
            else
                _tvPosition.Text = "";

            if (!String.IsNullOrEmpty(_currentItem.Content))
                _tvComment.Text = _currentItem.Content;
            else
                _tvComment.Text = "";

            if (!String.IsNullOrEmpty(_currentItem.AttachFiles))
            {
                try
                {
                    List<BeanAttachFile> _lstAttach = JsonConvert.DeserializeObject<List<BeanAttachFile>>(_currentItem.AttachFiles);

                    if (_lstAttach != null && _lstAttach.Count > 0)
                    {
                        _lnAttach.Visibility = ViewStates.Visible;
                        _tvAttachCount.Text = _lstAttach.Count.ToString();
                    }
                    else
                    {
                        _lnAttach.Visibility = ViewStates.Invisible;
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetChildView", ex);
#endif
                    _lnAttach.Visibility = ViewStates.Invisible;
                }
            }
            else
            {
                _lnAttach.Visibility = ViewStates.Invisible;
            }

            if (_currentItem.LikeCount > 0)
            {
                _lnLike.Visibility = ViewStates.Visible;
                _tvLikeCount.Text = _currentItem.LikeCount.ToString();
            }
            else
            {
                _lnLike.Visibility = ViewStates.Invisible;
            }

            if (_currentItem.IsLiked == true) // nếu like -> màu xanh
                _tvLike.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlueEnable)));
            else
                _tvLike.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));

            if (_IsReplyView == true) // child
            {
                _recyAttach.Visibility = ViewStates.Visible;
                try
                {
                    if (!String.IsNullOrEmpty(_currentItem.AttachFiles)) // Gắn List của Comment Parent
                    {
                        List<BeanAttachFile> _lstAttachTemp = JsonConvert.DeserializeObject<List<BeanAttachFile>>(_currentItem.AttachFiles);

                        AdapterCommentAttachFile _adapterCommentAttach = new AdapterCommentAttachFile(_mainAct, _context, _lstAttachTemp, !_IsReplyView);
                        _adapterCommentAttach.CustomItemClick_Detail += click_ItemRecyAttach_Detail;
                        _recyAttach.SetAdapter(_adapterCommentAttach);
                        _recyAttach.SetLayoutManager(new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical));
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetChildView", ex);
#endif
                }
            }
            else
            {
                _recyAttach.Visibility = ViewStates.Gone;
            }

            return convertView;
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
            //if (convertView == null)
            //{
            LayoutInflater mInflater = LayoutInflater.From(_context);
            convertView = mInflater.Inflate(Resource.Layout.ItemExpandDetailCreateTask_Comment_Group, null);
            //}
            TextView _tvTitle = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_Title);
            LinearLayout _lnParentComment = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailCreateTask_Comment_Group_ParentComment);
            LinearLayout _lnItemComment = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailCreateTask_Comment_Group_ItemComment);
            EditText _edtParentComment = convertView.FindViewById<EditText>(Resource.Id.edt_ItemExpandDetailCreateTask_Comment_Group_ParentComment);
            ImageView _imgAttachParent = convertView.FindViewById<ImageView>(Resource.Id.img_ItemExpandDetailCreateTask_Comment_Group_ParentComment_Attach);
            CustomFlexBoxRecyclerView _recyAttachParent = convertView.FindViewById<CustomFlexBoxRecyclerView>(Resource.Id.recy_ItemExpandDetailCreateTask_Comment_Group_ParentComment_Attach);

            TextView _tvAttachCount = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_AttachCount);
            LinearLayout _lnAttach = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailCreateTask_Comment_Group_Attach);
            LinearLayout _lnLike = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailCreateTask_Comment_Group_Like);
            ImageView _imgCommentParent = convertView.FindViewById<ImageView>(Resource.Id.img_ItemExpandDetailCreateTask_Comment_Group_ParentComment_Comment);
            CircleImageView _imgAvatar = convertView.FindViewById<CircleImageView>(Resource.Id.img_ItemExpandDetailCreateTask_Comment_Group_Avata);
            TextView _tvAvatar = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_Avata);
            TextView _tvName = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_Name);
            TextView _tvDate = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_Date);
            TextView _tvPosition = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_Position);
            TextView _tvComment = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_Comment);
            TextView _tvLike = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_Like);
            TextView _tvReply = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_Reply);
            TextView _tvLikeCount = convertView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailCreateTask_Comment_Group_LikeCount);
            LinearLayout _lnAction = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailCreateTask_Comment_Group_Action);
            RecyclerView _recyAttach = convertView.FindViewById<RecyclerView>(Resource.Id.recy_ItemExpandDetailCreateTask_Comment_Group_Attach);

            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
            {
                _tvTitle.Text = CmmFunction.GetTitle("TEXT_COMMENT", "Bình luận");
                _tvLike.Text = CmmFunction.GetTitle("TEXT_LIKE", "Thích");
                _tvReply.Text = CmmFunction.GetTitle("TEXT_REPLY", "Trả lời");
                _edtParentComment.Hint = CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến");
            }
            else
            {
                _tvTitle.Text = CmmFunction.GetTitle("TEXT_COMMENT", "Comment");
                _tvLike.Text = CmmFunction.GetTitle("TEXT_LIKE", "Like");
                _tvReply.Text = CmmFunction.GetTitle("TEXT_REPLY", "Reply");
                _edtParentComment.Hint = CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Leave a comment/ opinion here");
            }

            if (groupPosition == 0 && _IsReplyView == false)
            {
                if (!String.IsNullOrEmpty(_parentComment))
                {
                    _edtParentComment.Text = _parentComment;
                }
                _edtParentComment.NestedScrollingEnabled = true;
                _edtParentComment.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Italic);
                _tvTitle.Visibility = ViewStates.Visible;
                _lnParentComment.Visibility = ViewStates.Visible;

                _imgAttachParent.Click += (sender, e) =>
                {
                    OnItemClick_CommentParent_ImgAttach();
                };
                _imgCommentParent.Click += (sender, e) =>
                {
                    CmmDroidFunction.HideSoftKeyBoard(_edtParentComment, _mainAct);
                    OnItemClick_CommentParent(_parentComment);
                };
                _edtParentComment.TextChanged += (sender, e) =>
                {
                    if (String.IsNullOrEmpty(_edtParentComment.Text))
                        _edtParentComment.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Italic);
                    else
                        _edtParentComment.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);

                    _parentComment = _edtParentComment.Text;
                };

                // Gắn List của Comment Parent
                AdapterCommentAttachFile _adapterCommentAttach = new AdapterCommentAttachFile(_mainAct, _context, _lstAttachParent, !_IsReplyView);
                _adapterCommentAttach.CustomItemClick_Detail += click_ItemRecyAttach_Detail;
                _adapterCommentAttach.CustomItemClick_Delete += click_ItemRecyAttach_Delete;
                _recyAttachParent.SetAdapter(_adapterCommentAttach);
                _recyAttachParent.SetLayoutManager(new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical));
                _recyAttachParent.NestedScrollingEnabled = true;

            }
            else
            {
                _tvTitle.Visibility = ViewStates.Gone;
                _lnParentComment.Visibility = ViewStates.Gone;
            }

            if (_lstData.Count > 0)
            {
                _lnItemComment.Visibility = ViewStates.Visible;
                BeanComment _currentItem = _lstData[groupPosition].parentItem;

                _tvLike.Click += (sender, e) =>
                {
                    OnItemClick_Like(_currentItem);
                };
                _tvReply.Click += (sender, e) =>
                {
                    OnItemClick_Reply(_currentItem);
                };

                BeanUser _authorUser = CmmFunction.GetBeanUserByID(_currentItem.Author);

                if (_authorUser != null && !String.IsNullOrEmpty(_authorUser.ID))
                    CmmDroidFunction.SetAvataByBeanUser(_mainAct, _context, _authorUser.ID, "ID", _imgAvatar, _tvAvatar, 50);
                else
                {
                    _tvAvatar.Visibility = ViewStates.Invisible;
                    _imgAvatar.Visibility = ViewStates.Invisible;
                }

                if (_authorUser != null && !String.IsNullOrEmpty(_authorUser.FullName))
                    _tvName.Text = _authorUser.FullName;
                else
                    _tvName.Text = "";

                if (_currentItem.Created.HasValue)
                    _tvDate.Text = CTRLDetailProcess.GetFormatDateLang(_currentItem.Created.Value);
                else
                    _tvDate.Text = "";

                if (_authorUser != null)
                {
                    SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
                    string _queryPosition = String.Format(CTRLDetailCreateTask._queryBeanPosition, _authorUser.PositionID);
                    List<BeanPosition> _lstPosition = conn.Query<BeanPosition>(_queryPosition);
                    conn.Close();

                    if (_lstPosition != null && _lstPosition.Count>0)
                        _tvPosition.Text = _lstPosition[0].Title;
                    else
                        _tvPosition.Text = "";
                }                  
                else
                    _tvPosition.Text = "";

                if (!String.IsNullOrEmpty(_currentItem.Content))
                    _tvComment.Text = _currentItem.Content;
                else
                    _tvComment.Text = "";

                if (!String.IsNullOrEmpty(_currentItem.AttachFiles))
                {
                    try
                    {
                        List<BeanAttachFile> _lstAttach = JsonConvert.DeserializeObject<List<BeanAttachFile>>(_currentItem.AttachFiles);

                        if (_lstAttach != null && _lstAttach.Count > 0)
                        {
                            _lnAttach.Visibility = ViewStates.Visible;
                            _tvAttachCount.Text = _lstAttach.Count.ToString();
                        }
                        else
                        {
                            _lnAttach.Visibility = ViewStates.Invisible;
                        }
                    }
                    catch (Exception)
                    {
                        _lnAttach.Visibility = ViewStates.Invisible;
                    }
                }
                else
                {
                    _lnAttach.Visibility = ViewStates.Invisible;
                }

                if (_currentItem.LikeCount > 0)
                {
                    _lnLike.Visibility = ViewStates.Visible;
                    _tvLikeCount.Text = _currentItem.LikeCount.ToString();
                }
                else
                {
                    _lnLike.Visibility = ViewStates.Invisible;
                }

                if (_currentItem.IsLiked == true) // nếu like -> màu xanh
                    _tvLike.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlueEnable)));
                else
                    _tvLike.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));

                if (_IsReplyView == true)
                {
                    _recyAttach.Visibility = ViewStates.Visible;
                    try
                    {
                        if (!String.IsNullOrEmpty(_currentItem.AttachFiles)) // Gắn List của Comment Parent
                        {
                            List<BeanAttachFile> _lstAttachTemp = JsonConvert.DeserializeObject<List<BeanAttachFile>>(_currentItem.AttachFiles);

                            AdapterCommentAttachFile _adapterCommentAttach = new AdapterCommentAttachFile(_mainAct, _context, _lstAttachTemp, !_IsReplyView);
                            _adapterCommentAttach.CustomItemClick_Detail += click_ItemRecyAttach_Detail;
                            _recyAttach.SetAdapter(_adapterCommentAttach);
                            _recyAttach.SetLayoutManager(new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical));
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    _recyAttach.Visibility = ViewStates.Gone;
                }
            }
            else
            {
                _lnItemComment.Visibility = ViewStates.Gone;
            }



            return convertView;
        }

        private void click_ItemRecyAttach_Delete(object sender, BeanAttachFile e)
        {
            try
            {
                if (CustomItemClick_Attach_Delete != null)
                    CustomItemClick_Attach_Delete(null, e);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "click_ItemRecyAttach_Delete", ex);
#endif
            }
        }
        private void click_ItemRecyAttach_Detail(object sender, BeanAttachFile e)
        {
            try
            {
                if (CustomItemClick_Attach_Detail != null)
                    CustomItemClick_Attach_Detail(null, e);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "click_ItemRecyAttach_Detail", ex);
#endif
            }
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
    }
}