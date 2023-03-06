using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Component;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Fragment;
using static BPMOPMobile.Droid.Class.MinionAction;
using static BPMOPMobile.Droid.Core.Class.MinionActionCore;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    public class AdapterDetailExpandControl : BaseExpandableListAdapter
    {
        private Context _context;
        private MainActivity _mainAct;
        private List<ViewSection> _lstSection;
        private int _widthScreenTablet = -1;
        private int _flagView = -1;
        private string _OtherResourceId = "";
        private List<BeanWorkFlowRelated> _lstFlowRelated = new List<BeanWorkFlowRelated>();
        private List<BeanTask> _lstTask = new List<BeanTask>();
        private List<BeanComment> _lstComment = new List<BeanComment>();
        private List<BeanAttachFile> _lstAttachComment = new List<BeanAttachFile>();
        private BeanWorkflowItem _currentWorkflowItem = new BeanWorkflowItem();
        private BeanNotify _currentNotifyItem = new BeanNotify();
        private ControllerComment CTRLComment = new ControllerComment();

        public event EventHandler<CommentEventArgs> CustomItemClick_CommentParent_ImgComment;
        public event EventHandler<CommentEventArgs> CustomItemClick_CommentParent_ImgAttach;
        public event EventHandler<BeanAttachFile> CustomItemClick_Attach_Delete;
        public event EventHandler<BeanAttachFile> CustomItemClick_Attach_Detail;
        public event EventHandler<TaskListItemClick> CustomItemClick_TaskListItem;

        private string _previousFragmentName;
        private CustomBaseFragment _previousFragment;

        private ComponentComment componentComment;
        private string _commentContent = "";

        public AdapterDetailExpandControl(MainActivity _mainAct, Context context, BeanWorkflowItem _currentWorkflowItem, BeanNotify _currentNotifyItem,
            List<ViewSection> _lstSection, List<BeanWorkFlowRelated> _lstFlowRelated, List<BeanTask> _lstTask, List<BeanComment> _lstComment, int _flagView, string _OtherResourceId)
        {
            this._mainAct = _mainAct;
            this._context = context;

            this._currentWorkflowItem = _currentWorkflowItem;
            this._currentNotifyItem = _currentNotifyItem;

            this._lstSection = _lstSection;
            this._lstFlowRelated = _lstFlowRelated;
            this._lstTask = _lstTask;
            this._lstComment = _lstComment;

            this._flagView = _flagView;
            this._OtherResourceId = _OtherResourceId;
        }

        public void UpdateCurrentListSection(List<ViewSection> _lstSection)
        {
            this._lstSection = _lstSection;
            ////////NotifyDataSetChanged();
            ////////NotifyDataSetInvalidated();
        }

        public override int GroupCount
        {
            get
            {
                return _lstSection.Count + 3; // +3 cho FlowRelated, Task, Comment
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
            try
            {
                if (_lstSection[groupPosition].ViewRows != null && _lstSection[groupPosition].ViewRows.Count > 0)
                {
                    return _lstSection[groupPosition].ViewRows.Count;
                }
            }
            catch (Exception)
            {

            }
            return 0;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            LayoutInflater mInflater = LayoutInflater.From(_context);
            View rootView = mInflater.Inflate(Resource.Layout.ItemExpandDetailWorkflow, null);
            LinearLayout _lnContent = rootView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailWorkflow_Content);

            ViewRow _viewRow = _lstSection[groupPosition].ViewRows[childPosition];
            ComponentBase _components;

            if (_viewRow.Elements != null && _viewRow.Elements.Count > 0)
            {
                switch (_viewRow.RowType)
                {
                    case 1:
                        _components = new ComponentRow1(_mainAct, _lnContent, _viewRow.Elements[0], -1, true, _flagView);
                        break;
                    case 2:
                        _components = new ComponentRow2(_mainAct, _lnContent, _viewRow, -1, false, _flagView);
                        break;
                    case 3:
                        _components = new ComponentRow3(_mainAct, _lnContent, _viewRow, -1, false, _flagView);
                        break;
                    default:
                        _components = new ComponentRow1(_mainAct, _lnContent, _viewRow.Elements[0], -1, true, _flagView);
                        break;
                }
                _components.InitializeFrameView(_lnContent);
                _components.SetTitle();
                _components.SetValue();
                _components.SetEnable();
                _components.SetProprety();
            }
            return rootView;
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

            if (groupPosition >= _lstSection.Count) // Flow Related - Task
            {
                View rootView = mInflater.Inflate(Resource.Layout.ItemExpandDetailWorkflow, null);
                LinearLayout _lnContent = rootView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailWorkflow_Content);

                if (groupPosition == _lstSection.Count)
                {
                    if (_lstFlowRelated != null && _lstFlowRelated.Count > 0) // Nếu có Quy trình liên kết mới hiện
                    {
                        //_lnContent.AddView(new TextView(_context));
                        //Action action = new Action(() =>
                        //{
                        ComponentFlowRelated _flowRelated = new ComponentFlowRelated(_mainAct, _lnContent, _lstFlowRelated, _currentWorkflowItem, _currentNotifyItem);
                        _flowRelated.InitializeFrameView(_lnContent);
                        _flowRelated.SetTitle();
                        _flowRelated.SetValue();
                        _flowRelated.SetEnable();
                        _flowRelated.SetProprety();
                        //});
                        //new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime - 100);
                    }
                }
                else if (groupPosition == _lstSection.Count + 1)
                {
                    if (_lstTask != null && _lstTask.Count > 0) // Nếu có Task mới hiện
                    {
                        ComponentTaskList _taskList = new ComponentTaskList(_mainAct, _lnContent, _lstTask);
                        _taskList.InitializeFrameView(_lnContent);
                        _taskList.SetTitle();
                        _taskList.SetValue();
                        _taskList.SetEnable();
                        _taskList.SetProprety();

                        _taskList.TaskListItemClickEvent += Click_TaskListItem;
                    }
                }
                else if (groupPosition == _lstSection.Count + 2)
                {
                    if (_lstComment != null && _lstComment.Count >= 0) // Nếu có Comment mới hiện, ko có cmt hiện edittext
                    {
                        componentComment = new ComponentComment(_mainAct, _lnContent, _lstComment);

                        if (!String.IsNullOrEmpty(_commentContent))
                            componentComment.UpdateCurrentParentCommentContent(_commentContent);

                        componentComment.InitFlagRecalculateView(false);
                        componentComment.InitializeFrameView(_lnContent);
                        componentComment.SetTitle();
                        componentComment.SetValue();
                        componentComment.SetEnable();
                        componentComment.SetProprety();

                        //Event Component
                        componentComment.CustomClick_CommentParent_ImgComment += Click_CommentParent_ImgComment;
                        componentComment.CustomClick_CommentParent_ImgAttach += Click_CommentParent_ImgAttach;

                        componentComment.CustomItemClick_ItemListAttachParent_Detail += Click_ItemListAttachParent_Detail;
                        componentComment.CustomItemClick_ItemListAttachParent_Delete += Click_ItemListAttachParent_Delete;

                        componentComment.CustomItemClick_ItemListComment_tvLike += Click_ItemListComment_Like;
                        componentComment.CustomItemClick_ItemListComment_tvReply += Click_ItemListComment_Reply;
                        componentComment.CustomItemClick_ItemListComment_AttachDetail += Click_ItemListAttachParent_Detail;

                        if (_lstAttachComment != null && _lstAttachComment.Count > 0) // lưu lại trạng thái
                        {
                            componentComment.UpdateListParentAttach(this._lstAttachComment);
                        }
                    }
                }
                return rootView;
            }
            else // Nếu > 1 ViewSection mới load lên
            {
                if (_lstSection.Count > 1)
                {
                    View rootView = mInflater.Inflate(Resource.Layout.ItemExpandDetailWorkflow, null);
                    LinearLayout _lnContent = rootView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailWorkflow_Content);
                    ComponentSection _section = new ComponentSection(_mainAct, _lnContent, _lstSection[groupPosition], groupPosition, _widthScreenTablet);
                    _section.InitializeFrameView(_lnContent);
                    _section.UpdateContentSection();
                    return rootView;
                }
                else
                {
                    View rootView = mInflater.Inflate(Resource.Layout.ItemExpandDetailWorkflow, null);
                    return rootView;
                }
            }
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }

        #region Component Comment Event

        public void UpdateFragmentName(CustomBaseFragment _previousFragment, string _previousFragmentName)
        {
            this._previousFragment = _previousFragment;
            this._previousFragmentName = _previousFragmentName;
        }
        public void UpdateListAttachComment(List<BeanAttachFile> _lstAttachComment)
        {
            try
            {

                this._lstAttachComment = _lstAttachComment;

                if (componentComment != null)
                {
                    componentComment.UpdateListParentAttach(this._lstAttachComment);
                    _commentContent = componentComment.getCurrentParentCommentContent();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "UpdateListAttachComment", ex);
#endif
            }
        }
        public List<BeanAttachFile> GetListAttachComment()
        {
            if (_lstAttachComment != null && _lstAttachComment.Count > 0)
                return _lstAttachComment;
            return new List<BeanAttachFile>();
        }

        private void Click_TaskListItem(object sender, TaskListItemClick e)
        {
            try
            {
                if (CustomItemClick_TaskListItem != null)
                    CustomItemClick_TaskListItem(this, e);
            }
            catch (Exception ex)
            {
                CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_TaskListItem", ex);
#endif
            }
        }

        private void Click_CommentParent_ImgComment(object sender, CommentEventArgs e)
        {
            try
            {
                if (CustomItemClick_CommentParent_ImgComment != null)
                    CustomItemClick_CommentParent_ImgComment(this, e);
            }
            catch (Exception ex)
            {
                CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_itemExpandComment_Reply", ex);
#endif
            }
        }
        private void Click_CommentParent_ImgAttach(object sender, System.EventArgs e)
        {
            try
            {
                if (CustomItemClick_CommentParent_ImgAttach != null)
                    CustomItemClick_CommentParent_ImgAttach(this, null);
            }
            catch (Exception ex)
            {
                CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_itemExpandComment_Reply", ex);
#endif
            }
        }
        private async void Click_ItemListComment_Like(object sender, BeanComment e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);
                    await Task.Run(() =>
                    {
                        ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                        bool _result = _pControlDynamic.SetLikeComment(e.ID, !e.IsLiked);

                        _mainAct.RunOnUiThread(() =>
                        {
                            if (_result == true)
                            {
                                #region Update View Value
                                e.IsLiked = !e.IsLiked;
                                if (e.IsLiked == true)
                                    e.LikeCount = e.LikeCount + 1;
                                else
                                    e.LikeCount = e.LikeCount - 1 < 0 ? 0 : e.LikeCount - 1; // nếu <0 thì gán = 0
                                #endregion

                                CTRLComment.UpdateIsLikedComment(_OtherResourceId, e.IsLiked, e.LikeCount); // Update Sqlite
                                NotifyDataSetChanged();
                                CmmDroidFunction.HideProcessingDialog();
                            }
                            else
                            {
                                CmmDroidFunction.HideProcessingDialog();
                                CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"),
                                    CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                            }
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_itemExpandComment_Reply", ex);
#endif
            }
        }
        private void Click_ItemListComment_Reply(object sender, BeanComment e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    if (_previousFragment != null && !String.IsNullOrEmpty(_previousFragmentName))
                    {
                        FragmentReplyComment _fragmentReplyComment = new FragmentReplyComment(_previousFragment, _previousFragmentName, e, _lstComment, _OtherResourceId, ((int)CmmFunction.CommentResourceCategoryID.WorkflowItem).ToString());
                        _mainAct.AddFragment(_mainAct.SupportFragmentManager, _fragmentReplyComment, "FragmentReplyComment", 0);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_itemExpandComment_Reply", ex);
#endif
            }
        }
        private void Click_ItemListAttachParent_Detail(object sender, BeanAttachFile e)
        {
            try
            {
                if (CustomItemClick_Attach_Detail != null)
                    CustomItemClick_Attach_Detail(this, e);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemExpandComment_Attach_Detail", ex);
#endif
            }
        }
        private void Click_ItemListAttachParent_Delete(object sender, BeanAttachFile e)
        {
            try
            {
                if (CustomItemClick_Attach_Delete != null)
                    CustomItemClick_Attach_Delete(this, e);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemExpandComment_Attach_Delete", ex);
#endif
            }
        }

        #endregion
    }
}