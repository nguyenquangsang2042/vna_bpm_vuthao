using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Component;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Fragment;
using static BPMOPMobile.Droid.Core.Class.MinionActionCore;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    public class AdapterDetailRecyDynamicControl : RecyclerView.Adapter
    {
        private Context _context;
        private MainActivity _mainAct;
        private int _flagView = -1;
        private ControllerComment CTRLComment = new ControllerComment();
        private BeanNotify _currentNotifyItem = new BeanNotify();                               // Current BeanNotify
        private BeanWorkflowItem _currentWorkflowItem = new BeanWorkflowItem();                 // Current BeanWorkflowItem
        private List<ViewSection> _lstSection;                                                  // List Section
        private List<BeanAttachFile> _lstAttachComment = new List<BeanAttachFile>();            // List Attach File
        private List<BeanWorkFlowRelated> _lstFlowRelated = new List<BeanWorkFlowRelated>();    // List QTLK
        private List<BeanTask> _lstTask = new List<BeanTask>();                                 // List Task
        private List<BeanComment> _lstComment = new List<BeanComment>();                        // List Comment

        private CustomBaseFragment _previousFragment;
        private string _previousFragmentName;

        // Comment Component
        public event EventHandler<CommentEventArgs> CustomItemClick_CommentParent_ImgComment;
        public event EventHandler<CommentEventArgs> CustomItemClick_CommentParent_ImgAttach;
        public event EventHandler<BeanAttachFile> CustomItemClick_Attach_Delete;
        public event EventHandler<BeanAttachFile> CustomItemClick_Attach_Detail;
        public event EventHandler<TaskListItemClick> CustomItemClick_TaskListItem;

        private ComponentComment componentComment;
        private string _commentContent = "";
        private string _OtherResourceId = "";

        public AdapterDetailRecyDynamicControl(MainActivity _mainAct, Context context, BeanWorkflowItem _currentWorkflowItem, BeanNotify _currentNotifyItem,
        List<ViewSection> _lstSection, List<BeanWorkFlowRelated> _lstFlowRelated, List<BeanTask> _lstTask, List<BeanComment> _lstComment, int _flagView, string _OtherResourceId)
        {
            this._mainAct = _mainAct;
            this._context = context;
            // Current Item
            this._currentWorkflowItem = _currentWorkflowItem;
            this._currentNotifyItem = _currentNotifyItem;
            // Dynamic Form
            this._lstSection = _lstSection;
            this._lstFlowRelated = _lstFlowRelated;
            this._lstTask = _lstTask;
            this._lstComment = _lstComment;
            // Other
            this._flagView = _flagView;
            this._OtherResourceId = _OtherResourceId;
        }

        public override int ItemCount
        {
            get
            {
                //int groupcount = _lstSection.Count + 3; // +3 cho FlowRelated, Task, Comment
                int groupcount = 3; // +3 cho FlowRelated, Task, Comment nam o cuoi
                int childcount = 0;
                for (int i = 0; i < _lstSection.Count; i++)
                {
                    if (_lstSection[i].ViewRows != null)
                        childcount += _lstSection[i].ViewRows.Count;
                }
                return groupcount + childcount;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemExpandDetailWorkflow, parent, false);
            AdapterDetailRecyControlHolder holder = new AdapterDetailRecyControlHolder(itemView, null);
            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AdapterDetailRecyControlHolder _holder = holder as AdapterDetailRecyControlHolder;
            try
            {
                _holder._lnContent.RemoveAllViews();
                if (position < ItemCount - 3) // Control Dynamic
                {
                    ViewRow _viewRow = _lstSection[0].ViewRows[position];
                    ComponentBase _components;
                    if (_viewRow.Elements != null && _viewRow.Elements.Count > 0)
                    {
                        switch (_viewRow.RowType)
                        {
                            case 1:
                                _components = new ComponentRow1(_mainAct, _holder._lnContent, _viewRow.Elements[0], -1, true, _flagView);
                                break;
                            case 2:
                                _components = new ComponentRow2(_mainAct, _holder._lnContent, _viewRow, -1, false, _flagView);
                                break;
                            case 3:
                                _components = new ComponentRow3(_mainAct, _holder._lnContent, _viewRow, -1, false, _flagView);
                                break;
                            default:
                                _components = new ComponentRow1(_mainAct, _holder._lnContent, _viewRow.Elements[0], -1, true, _flagView);
                                break;
                        }
                        _components.InitializeFrameView(_holder._lnContent);
                        _components.SetTitle();
                        _components.SetValue();
                        _components.SetEnable();
                        _components.SetProprety();
                    }
                }
                else //  FlowRelated, Task, Comment 
                {
                    if (position == ItemCount - 3)
                    {
                        if (_lstFlowRelated != null && _lstFlowRelated.Count > 0) // Nếu có Quy trình liên kết mới hiện
                        {
                            ComponentFlowRelated _flowRelated = new ComponentFlowRelated(_mainAct, _holder._lnContent, _lstFlowRelated, _currentWorkflowItem, _currentNotifyItem);
                            _flowRelated.InitializeFrameView(_holder._lnContent);
                            _flowRelated.SetTitle();
                            _flowRelated.SetValue();
                            _flowRelated.SetEnable();
                            _flowRelated.SetProprety();
                        }
                    }
                    else if (position == ItemCount - 2)
                    {
                        if (_lstTask != null && _lstTask.Count > 0) // Nếu có Task mới hiện
                        {
                            ComponentTaskList _taskList = new ComponentTaskList(_mainAct, _holder._lnContent, _lstTask);
                            _taskList.InitializeFrameView(_holder._lnContent);
                            _taskList.SetTitle();
                            _taskList.SetValue();
                            _taskList.SetEnable();
                            _taskList.SetProprety();

                            _taskList.TaskListItemClickEvent += Click_TaskListItem;
                        }
                    }
                    else if (position == ItemCount - 1)
                    {
                        if (_lstComment != null && _lstComment.Count >= 0) // Nếu có Comment mới hiện, ko có cmt hiện edittext
                        {
                            componentComment = new ComponentComment(_mainAct, _holder._lnContent, _lstComment);

                            if (!String.IsNullOrEmpty(_commentContent))
                                componentComment.UpdateCurrentParentCommentContent(_commentContent);

                            componentComment.InitFlagRecalculateView(false);
                            componentComment.InitializeFrameView(_holder._lnContent);
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
                }
            }
            catch (Exception ex)
            {

            }
        }

        public class AdapterDetailRecyControlHolder : RecyclerView.ViewHolder
        {
            public LinearLayout _lnContent { get; set; }
            public AdapterDetailRecyControlHolder(View itemview, Action<int> listener) : base(itemview)
            {
                _lnContent = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailWorkflow_Content);
                if (listener != null)
                {
                    _lnContent.Click += (sender, e) => listener(base.LayoutPosition);
                }
            }
        }

        public void UpdateCurrentListSection(List<ViewSection> _lstSection)
        {
            this._lstSection = _lstSection;
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