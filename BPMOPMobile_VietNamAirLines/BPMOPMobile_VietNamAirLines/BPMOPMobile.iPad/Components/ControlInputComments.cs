using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
using BPMOPMobile.iPad.ViewControllers.Applications;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using UIKit;

namespace BPMOPMobile.iPad.Components
{
    public class ControlInputComments : ControlBase
    {
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }
        List<BeanComment> lstComment = new List<BeanComment>();
        List<BeanAttachFile> lst_addAttachFile = new List<BeanAttachFile>();
        List<BeanAttachFile> lst_attachment = new List<BeanAttachFile>();
        List<BeanAttachFile> lst_attachment_img = new List<BeanAttachFile>();
        List<BeanAttachFile> lst_attachment_doc = new List<BeanAttachFile>();
        public UITableView tableView_attachment, table_comment;
        UICollectionView collection_attach;
        CollectionAttachmentThumb_Source collectionAttachmentThumb_Source;
        NSIndexPath commentSelectedIndexPath;
        BeanComment commentSelected;
        List<KeyValuePair<string, bool>> lst_sectionState;
        public BeanAttachFile currentAttachment { get; set; }
        CmmLoading loading;
        UIActivityIndicatorView loading_sendComment;
        nfloat est_CommentHeight = 0;

        UIView view_note;
        UITextView txt_note;
        string hintDefault = CmmFunction.GetTitle("TEXT_HINT_REQUIRE_COMMENT", "Nhập ý kiến");
        //UILabel lbl_type, lbl_commentTitle, lbl_typeName;
        UIButton BT_sendComment, BT_addAttach;

        public ControlInputComments(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
        {
            parentView = _parentView;
            element = _element;
            indexPath = _indexPath;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            this.BackgroundColor = UIColor.White;

            view_note = new UIView();
            //view_note.BackgroundColor = UIColor.LightGray;

            txt_note = new UITextView();
            txt_note.Started += Txt_note_Started;
            txt_note.Ended += Txt_note_Ended;

            BT_addAttach = new UIButton();
            BT_addAttach.SetImage(UIImage.FromFile("Icons/icon_attachDark.png"), UIControlState.Normal);
            BT_addAttach.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_addAttach.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            BT_addAttach.AddTarget(HandleBtnAddattachment, UIControlEvent.TouchUpInside);

            BT_sendComment = new UIButton();
            BT_sendComment.SetImage(UIImage.FromFile("Icons/icon_comment.png"), UIControlState.Normal);
            BT_sendComment.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_sendComment.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            BT_sendComment.TouchUpInside += BT_sendComment_TouchUpInside;

            loading_sendComment = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Medium);
            loading_sendComment.TintColor = UIColor.Blue;
            loading_sendComment.HidesWhenStopped = true;

            var flowLayout = new UICollectionViewFlowLayout()
            {
                SectionInset = new UIEdgeInsets(2, 2, 2, 2),
                ScrollDirection = UICollectionViewScrollDirection.Horizontal,
                MinimumInteritemSpacing = 5, // minimum spacing between cells
                MinimumLineSpacing = 0 // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
            };

            CGRect frame = CGRect.Empty;
            collection_attach = new UICollectionView(frame: frame, layout: flowLayout);
            collection_attach.BackgroundColor = UIColor.White;

            tableView_attachment = new UITableView();
            tableView_attachment.ScrollEnabled = false;

            view_note.AddSubviews(txt_note, BT_addAttach, BT_sendComment, loading_sendComment, collection_attach, tableView_attachment);

            table_comment = new UITableView();
            table_comment.AllowsSelection = false;
            table_comment.ScrollEnabled = false;
            //table_comment.BackgroundColor = UIColor.Purple;

            this.ClipsToBounds = true;
            this.AddSubviews(view_note, table_comment);

            this.WillRemoveSubview(lbl_value);
            this.WillRemoveSubview(BT_action);
            this.WillRemoveSubview(lbl_line);

            lbl_title.Font = UIFont.FromName("ArialMT", 14f);
        }

        private void Txt_note_Ended(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_note.Text))
            {
                txt_note.Text = hintDefault;
                txt_note.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                txt_note.TextColor = UIColor.FromRGB(94, 94, 94);
            }
        }

        private void Txt_note_Started(object sender, EventArgs e)
        {
            if (parentView.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView toDoDetailView = parentView as ToDoDetailView;
                toDoDetailView.isShowKeyBoarFromComment = true;
                toDoDetailView.estCommmentViewRowHeight = this.Frame.Height;
                //toDoDetailView.ScrollToCommentViewRow(this.Frame.Height);
            }
            else if (parentView.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView workflowDetailView = parentView as WorkflowDetailView;
                workflowDetailView.isShowKeyBoarFromComment = true;
            }
            else if (parentView.GetType() == typeof(FormTaskDetails))
            {
                FormTaskDetails formTaskDetails = parentView as FormTaskDetails;
                formTaskDetails.estCommmentViewRowHeight = this.Frame.Height;
            }
            else if (parentView.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController followListViewController = parentView as FollowListViewController;
                followListViewController.estCommmentViewRowHeight = this.Frame.Height;
            }

            //table_comment.ScrollsToTop = true;
            if (txt_note.Text == hintDefault)
            {
                txt_note.Text = string.Empty;
                txt_note.Font = UIFont.FromName("ArialMT", 14f);
                txt_note.TextColor = UIColor.Black;
            }
        }

        public override void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;
            nfloat baseHeight = 100;
            const int paddingTop = 5;
            const int spaceView = 5;
            var width = Frame.Width;

            lbl_title.HeightAnchor.ConstraintEqualTo(20).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            view_note.Layer.BorderColor = UIColor.FromRGB(232, 232, 232).CGColor;
            view_note.Layer.BorderWidth = 0.5f;
            view_note.Layer.CornerRadius = 3;

            //txt_note.BackgroundColor = UIColor.Purple;
            txt_note.Frame = new CGRect(10, 10, Frame.Width - 150, 90);
            //txt_note.HeightAnchor.ConstraintEqualTo(70).Active = true;
            //NSLayoutConstraint.Create(txt_note, NSLayoutAttribute.Top, NSLayoutRelation.Equal, view_note, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            //NSLayoutConstraint.Create(txt_note, NSLayoutAttribute.Left, NSLayoutRelation.Equal, view_note, NSLayoutAttribute.Left, 1.0f, paddingTop).Active = true;
            //NSLayoutConstraint.Create(txt_note, NSLayoutAttribute.Right, NSLayoutRelation.Equal, view_note, NSLayoutAttribute.Right, 1.0f, 100).Active = true;

            BT_addAttach.Frame = new CGRect(Frame.Width - 100, txt_note.Frame.Y, 30, 30);
            //BT_addAttach.HeightAnchor.ConstraintEqualTo(30);
            //BT_addAttach.WidthAnchor.ConstraintEqualTo(30);
            //NSLayoutConstraint.Create(BT_addAttach, NSLayoutAttribute.Top, NSLayoutRelation.Equal, view_note, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            //NSLayoutConstraint.Create(BT_addAttach, NSLayoutAttribute.Right, NSLayoutRelation.Equal, BT_sendComment, NSLayoutAttribute.Left, 1.0f, 20).Active = true;

            BT_sendComment.Frame = new CGRect(BT_addAttach.Frame.Right + 20, BT_addAttach.Frame.Y, 30, 30);
            //BT_sendComment.HeightAnchor.ConstraintEqualTo(30);
            //BT_sendComment.WidthAnchor.ConstraintEqualTo(30);
            //NSLayoutConstraint.Create(BT_sendComment, NSLayoutAttribute.Top, NSLayoutRelation.Equal, view_note, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            //NSLayoutConstraint.Create(BT_sendComment, NSLayoutAttribute.Right, NSLayoutRelation.Equal, view_note, NSLayoutAttribute.Left, 1.0f, 20).Active = true;

            loading_sendComment.Frame = BT_sendComment.Frame;

            if (lst_attachment_img != null && lst_attachment_img.Count > 0)
                collection_attach.Frame = new CGRect(0, txt_note.Frame.Bottom + 5, Frame.Width, 150);
            else
                collection_attach.Frame = new CGRect(0, txt_note.Frame.Bottom + 5, Frame.Width, 0);

            if (lst_attachment_doc != null && lst_attachment_doc.Count > 0)
            {
                var tableAttachHeight = lst_attachment_doc.Count * 40;
                tableView_attachment.Frame = new CGRect(0, collection_attach.Frame.Bottom, Frame.Width, tableAttachHeight);
            }
            else
                tableView_attachment.Frame = new CGRect(0, collection_attach.Frame.Bottom, Frame.Width, 0);

            view_note.Frame = new CGRect(0, 30, Frame.Width, tableView_attachment.Frame.Bottom);

            //table_comment.BackgroundColor = UIColor.LightGray;
            table_comment.Frame = new CGRect(0, view_note.Frame.Bottom + 5, frame.Width, est_CommentHeight + 50);

        }

        public override void SetTitle()
        {
            base.SetTitle();

            if (!element.IsRequire)
                lbl_title.Text = element.Title;
            else
                lbl_title.Text = element.Title + " (*)";
        }

        public override void SetEnable()
        {
            base.SetEnable();
            /*if (!element.Enable)
            {
                BT_action.Hidden = true;
                //BT_action.UserInteractionEnabled = false;
                txt_note.Hidden = true;
                BT_sendComment.Hidden = true;
                BT_addAttach.Hidden = true;
            }
            else
            {
                BT_action.Hidden = false;
                //BT_action.UserInteractionEnabled = true;
            }*/

            txt_note.Hidden = !element.Enable;
            BT_sendComment.Hidden = !element.Enable;
            BT_addAttach.Hidden = !element.Enable;
        }

        public override void SetValue()
        {
            base.SetValue();

            if (element.Notes != null && element.Notes.Count > 0)
            {
                foreach (var note in element.Notes)
                {
                    if (note.Key == "image")
                    {
                        JArray json = JArray.Parse(note.Value);
                        lst_attachment_img.AddRange(json.ToObject<List<BeanAttachFile>>());
                    }
                    else if (note.Key == "doc")
                    {
                        JArray json = JArray.Parse(note.Value);
                        lst_attachment_doc.AddRange(json.ToObject<List<BeanAttachFile>>());
                    }
                }
            }

            if (lst_attachment_img != null && lst_attachment_img.Count > 0)
            {
                collectionAttachmentThumb_Source = new CollectionAttachmentThumb_Source(this, lst_attachment_img);
                collection_attach.RegisterClassForCell(typeof(AttachThumb_CollectionCell), AttachThumb_CollectionCell.CellID);
                collection_attach.Source = collectionAttachmentThumb_Source;
                collection_attach.Delegate = new CustomFlowLayoutDelegate(this, collectionAttachmentThumb_Source, collection_attach);
                collection_attach.ReloadData();
            }

            if (lst_attachment_doc != null && lst_attachment_doc.Count > 0)
            {
                tableView_attachment.Source = new Attachment_TableSource(lst_attachment_doc, this);
                tableView_attachment.ReloadData();
            }

            if (!string.IsNullOrEmpty(element.Value) && element.Value != "[]")
            {
                JArray json = JArray.Parse(element.Value.Trim());
                lstComment = json.ToObject<List<BeanComment>>();
            }
            //
            if (lst_attachment_doc.Count > 0)
                est_CommentHeight += 40 * lst_attachment_doc.Count;

            if (lst_attachment_img.Count > 0)
                est_CommentHeight += 120;

            //danh sach tat ca comment trong phieu
            if (lstComment != null && lstComment.Count > 0)
            {
                foreach (var comment in lstComment)
                {
                    // comment co dinh kem
                    if (!string.IsNullOrEmpty(comment.AttachFiles))
                    {
                        JArray json = JArray.Parse(comment.AttachFiles);
                        List<BeanAttachFile> newSortList = new List<BeanAttachFile>();
                        var lst_attachFiles = json.ToObject<List<BeanAttachFile>>();

                        foreach (var attach in lst_attachFiles)
                        {
                            string fileExt = string.Empty;
                            if (!string.IsNullOrEmpty(attach.Url))
                                fileExt = attach.Url.Split('.').Last();

                            bool isThumb = CmmFunction.CheckIsImageAttachmentType(fileExt);
                            if (isThumb)
                            {
                                est_CommentHeight = est_CommentHeight + 230;
                            }
                            else
                            {
                                newSortList.Insert(newSortList.Count, attach);
                                est_CommentHeight = est_CommentHeight + 40;
                            }
                        }
                        nfloat heightText = 0;
                        if (!string.IsNullOrEmpty(comment.Content))
                        {
                            CGRect rect = StringExtensions.StringRect(comment.Content, UIFont.FromName("ArialMT", 14f), (table_comment.Frame.Width / 5) * 4.4f);
                            if (rect.Height > 0 && rect.Height < 20)
                                rect.Height = 30;
                            heightText = rect.Height + 50;
                        }
                        else
                            heightText = 80;

                        est_CommentHeight = est_CommentHeight + heightText + 60;
                    }
                    // comment khong co dinh kem
                    else
                    {
                        est_CommentHeight = est_CommentHeight + 100;
                    }
                }
            }

            //comment typing
            //su dung truong formula de luu tam noi dung text commment chua gui
            if (!string.IsNullOrEmpty(element.Formula))
            {
                txt_note.Text = element.Formula;
                txt_note.Font = UIFont.FromName("ArialMT", 14f);
                txt_note.TextColor = UIColor.Black;
            }
            else
            {
                txt_note.Text = hintDefault;
                txt_note.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                txt_note.TextColor = UIColor.FromRGB(94, 94, 94);
            }

            table_comment.Source = new Comment_TableSource(lstComment, this);
            table_comment.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            table_comment.ReloadData();

        }

        public void HandleRemoveThumbItem(BeanAttachFile attachFile)
        {
            UIAlertController alert = UIAlertController.Create("Thông báo", CmmFunction.GetTitle("TEXT_DELETE_CONFIRM", "Bạn có chắc muốn xóa file này không?"), UIAlertControllerStyle.Alert);//"BPM"
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"), UIAlertActionStyle.Default, null));
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"), UIAlertActionStyle.Default, alertAction =>
            {
                List<BeanAttachFile> lst_attach_remove = new List<BeanAttachFile>();
                if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
                {
                    var index = lst_attachment_img.FindIndex(item => item.ID == attachFile.ID);
                    if (index != -1)
                    {
                        lst_attach_remove.Add(lst_attachment_img[index]);
                        lst_attachment_img.RemoveAt(index);
                    }

                    var json_attachRemove = JsonConvert.SerializeObject(lst_attach_remove);
                    var jsonStringFinal = JsonConvert.SerializeObject(lst_attachment_img);
                    //element.Value = jsonString;
                    ToDoDetailView controller = (ToDoDetailView)parentView;
                    controller.lst_addCommentAttachment.RemoveAt(index);
                    controller.HandleAttachmentThumbRemove(element, indexPath, this, lst_attachment_img.Count, jsonStringFinal);
                }
                else if (parentView.GetType() == typeof(WorkflowDetailView))
                {
                    var index = lst_attachment.FindIndex(item => item.ID == attachFile.ID);
                    if (index != -1)
                    {
                        lst_attach_remove.Add(lst_attachment[index]);
                        lst_attachment.RemoveAt(index);
                    }

                    var json_attachRemove = JsonConvert.SerializeObject(lst_attach_remove);
                    var jsonStringFinal = JsonConvert.SerializeObject(lst_attachment_img);

                    WorkflowDetailView controller = (WorkflowDetailView)parentView;
                    controller.lst_addCommentAttachment.RemoveAt(index);
                    controller.HandleAttachmentThumbRemove(element, indexPath, this, lst_attachment_img.Count, jsonStringFinal);
                }
                else if (parentView != null && parentView.GetType() == typeof(FormTaskDetails))
                {
                    FormTaskDetails controller = (FormTaskDetails)parentView;

                    var index = lst_attachment_img.FindIndex(item => item == attachFile);
                    if (index != -1)
                    {
                        lst_attachment_img.RemoveAt(index);
                        controller.lst_addCommentAttachment.RemoveAt(index);
                    }
                    var jsonStringFinal = JsonConvert.SerializeObject(lst_attachment_img);
                    //element.Value = jsonString;
                    controller.hintDefault = txt_note.Text;
                    controller.HandleAttachmentThumbRemove(element, indexPath, this, lst_attachment_img.Count, jsonStringFinal, "image");
                }
            }));
            parentView.PresentViewController(alert, true, null);

        }

        public void HandleRemoveDocItem(BeanAttachFile attachFile)
        {
            UIAlertController alert = UIAlertController.Create("Thông báo", CmmFunction.GetTitle("TEXT_DELETE_CONFIRM", "Bạn có chắc muốn xóa file này không?"), UIAlertControllerStyle.Alert);//"BPM"
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"), UIAlertActionStyle.Default, null));
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"), UIAlertActionStyle.Default, alertAction =>
            {
                List<BeanAttachFile> lst_attach_remove = new List<BeanAttachFile>();
                if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
                {
                    var index = lst_attachment_doc.FindIndex(item => item.ID == attachFile.ID);
                    if (index != -1)
                    {
                        lst_attach_remove.Add(lst_attachment_doc[index]);
                        lst_attachment_doc.RemoveAt(index);
                    }

                    var json_attachRemove = JsonConvert.SerializeObject(lst_attach_remove);
                    var jsonStringFinal = JsonConvert.SerializeObject(lst_attachment_doc);
                    //element.Value = jsonString;

                    ToDoDetailView controller = (ToDoDetailView)parentView;
                    controller.lst_addCommentAttachment.RemoveAt(index);
                    controller.HandleAttachmentThumbRemove(element, indexPath, this, lst_attachment_doc.Count, jsonStringFinal);
                }
                else if (parentView.GetType() == typeof(WorkflowDetailView))
                {
                    var index = lst_attachment.FindIndex(item => item.ID == attachFile.ID);
                    if (index != -1)
                    {
                        lst_attach_remove.Add(lst_attachment[index]);
                        lst_attachment.RemoveAt(index);
                    }

                    var json_attachRemove = JsonConvert.SerializeObject(lst_attach_remove);
                    var jsonStringFinal = JsonConvert.SerializeObject(lst_attachment_doc);

                    WorkflowDetailView controller = (WorkflowDetailView)parentView;
                    controller.lst_addCommentAttachment.RemoveAt(index);
                    controller.HandleAttachmentThumbRemove(element, indexPath, this, lst_attachment_doc.Count, jsonStringFinal);
                }
                else if (parentView.GetType() == typeof(FormTaskDetails))
                {
                    var index = lst_attachment.FindIndex(item => item.ID == attachFile.ID);
                    if (index != -1)
                    {
                        lst_attach_remove.Add(lst_attachment[index]);
                        lst_attachment.RemoveAt(index);
                    }

                    var json_attachRemove = JsonConvert.SerializeObject(lst_attach_remove);
                    var jsonStringFinal = JsonConvert.SerializeObject(lst_attachment_doc);

                    FormTaskDetails controller = (FormTaskDetails)parentView;
                    controller.lst_addCommentAttachment.RemoveAt(index);
                    controller.HandleAttachmentThumbRemove(element, indexPath, this, lst_attachment_img.Count, jsonStringFinal, "doc");
                }
                else if (parentView.GetType() == typeof(FollowListViewController))
                {
                    var index = lst_attachment.FindIndex(item => item.ID == attachFile.ID);
                    if (index != -1)
                    {
                        lst_attach_remove.Add(lst_attachment[index]);
                        lst_attachment.RemoveAt(index);
                    }

                    var json_attachRemove = JsonConvert.SerializeObject(lst_attach_remove);
                    var jsonStringFinal = JsonConvert.SerializeObject(lst_attachment_img);

                    FollowListViewController controller = (FollowListViewController)parentView;
                    controller.lst_addCommentAttachment.RemoveAt(index);
                    controller.HandleAttachmentThumbRemove(element, indexPath, this, lst_attachment_img.Count, jsonStringFinal);
                }
            }));
            parentView.PresentViewController(alert, true, null);

        }

        public void HandleEditItem(BeanAttachFile attachFile)
        {
            if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView controller = (ToDoDetailView)parentView;
                controller.HandleAttachmentEdit(element, indexPath, attachFile, this);
            }
            else if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView controller = (WorkflowDetailView)parentView;
                controller.HandleAttachmentEdit(element, indexPath, attachFile, this);
            }
            else if (parentView != null && parentView.GetType() == typeof(FormTaskDetails))
            {
                FormTaskDetails controller = (FormTaskDetails)parentView;
                controller.HandleAttachmentEdit(element, indexPath, attachFile);
            }
            else if (parentView != null && parentView.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController controller = (FollowListViewController)parentView;
                controller.HandleAttachmentEdit(element, indexPath, attachFile, this);
            }
        }

        public void HandleSeclectItem(BeanAttachFile _attachment)
        {
            if (parentView != null && parentView.GetType() == typeof(FormTaskDetails))
            {
                currentAttachment = _attachment;
                if (parentView.GetType() == typeof(FormTaskDetails))
                {
                    FormTaskDetails formTaskDetails = (FormTaskDetails)parentView;
                    formTaskDetails.NavigateToShowAttachView(_attachment);
                }
            }
            else if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
            {
                currentAttachment = _attachment;
                if (parentView.GetType() == typeof(ToDoDetailView))
                {
                    ToDoDetailView requestDetailsV2 = (ToDoDetailView)parentView;
                    requestDetailsV2.NavigateToShowAttachView(_attachment);
                }
            }
            else if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                currentAttachment = _attachment;
                if (parentView.GetType() == typeof(WorkflowDetailView))
                {
                    WorkflowDetailView workflowDetail = (WorkflowDetailView)parentView;
                    workflowDetail.NavigateToShowAttachView(_attachment);
                }
            }
            else if (parentView != null && parentView.GetType() == typeof(FormWorkFlowDetails))
            {
                currentAttachment = _attachment;
                if (parentView.GetType() == typeof(FormWorkFlowDetails))
                {
                    FormWorkFlowDetails workflowDetail = (FormWorkFlowDetails)parentView;
                    workflowDetail.NavigateToShowAttachView(_attachment);
                }
            }
            else if (parentView != null && parentView.GetType() == typeof(FollowListViewController))
            {
                currentAttachment = _attachment;
                if (parentView.GetType() == typeof(FollowListViewController))
                {
                    FollowListViewController followListViewController = (FollowListViewController)parentView;
                    followListViewController.NavigateToShowAttachView(_attachment);
                }
            }
        }

        private void HandleBtnAddattachment(object sender, EventArgs e)
        {
            if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView controller = (WorkflowDetailView)parentView;
                controller.HandleAddAttachment(element, indexPath, this);
            }
            else if (parentView.GetType() == typeof(ToDoDetailView))
            {
                // su dung truong formula de luu tam noi dung text commment chua gui
                if (txt_note.Text != hintDefault)
                    element.Formula = txt_note.Text;
                else if (string.IsNullOrEmpty(txt_note.Text))
                {
                    element.Formula = string.Empty;
                    txt_note.Text = hintDefault;
                }

                ToDoDetailView requestDetailsV2 = parentView as ToDoDetailView;
                requestDetailsV2.HandleAddAttachment(element, indexPath, this);
            }
            else if (parentView.GetType() == typeof(FormWorkFlowDetails))
            {
                FormWorkFlowDetails requestDetailsV2 = parentView as FormWorkFlowDetails;
                requestDetailsV2.HandleAddAttachment(element, indexPath, this);
            }
            else if (parentView != null && parentView.GetType() == typeof(FormTaskDetails))
            {
                FormTaskDetails controller = (FormTaskDetails)parentView;
                controller.hintDefault = txt_note.Text;
                controller.HandleAddAttachment(true);
            }
            else if (parentView.GetType() == typeof(FollowListViewController))
            {
                // su dung truong formula de luu tam noi dung text commment chua gui
                if (txt_note.Text != hintDefault)
                    element.Formula = txt_note.Text;
                else if (string.IsNullOrEmpty(txt_note.Text))
                {
                    element.Formula = string.Empty;
                    txt_note.Text = hintDefault;
                }

                FollowListViewController followListViewController = parentView as FollowListViewController;
                followListViewController.HandleAddAttachment(element, indexPath, this);
            }
        }

        private void HandleBtnLike(NSIndexPath _indexCommentSection, BeanComment commentItem)
        {
            commentSelectedIndexPath = _indexCommentSection;
            commentSelected = commentItem;

            if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                SubmitLikeAction(commentSelectedIndexPath, commentItem);
            }
            else if (parentView.GetType() == typeof(ToDoDetailView))
            {
                SubmitLikeAction(commentSelectedIndexPath, commentItem);
            }
            else if (parentView.GetType() == typeof(FormWorkFlowDetails))
            {
                SubmitLikeAction(commentSelectedIndexPath, commentItem);
            }
            else if (parentView.GetType() == typeof(FormTaskDetails))
            {
                SubmitLikeAction(commentSelectedIndexPath, commentItem);
            }
            //app
            if (parentView != null && parentView.GetType() == typeof(AppMyRequestView))
            {
                SubmitLikeAction(commentSelectedIndexPath, commentItem);
            }
            else if (parentView.GetType() == typeof(AppToDoView))
            {
                SubmitLikeAction(commentSelectedIndexPath, commentItem);
            }
        }

        public async void SubmitLikeAction(NSIndexPath _indexCommentSection, BeanComment commentItem)
        {
            ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
            _ = Task.Run(() =>
            {
                bool res;
                res = p_dynamic.SetLikeComment(commentItem.ID, !commentItem.IsLiked);
                if (res)
                {
                    var conn = new SQLiteConnection(CmmVariable.M_DataPath);

                    commentItem.IsLiked = !commentItem.IsLiked;
                    if (commentItem.IsLiked == true)
                        commentItem.LikeCount = commentItem.LikeCount + 1;
                    else
                        commentItem.LikeCount = commentItem.LikeCount - 1 < 0 ? 0 : commentItem.LikeCount - 1; // nếu <0 thì gán = 0

                    conn.Update(commentItem);

                    InvokeOnMainThread(() =>
                    {
                        if (parentView.GetType() == typeof(ToDoDetailView))
                        {
                            ToDoDetailView requestDetailsV2 = parentView as ToDoDetailView;
                            requestDetailsV2.SubmitLikeAction(_indexCommentSection, commentItem);
                        }
                        else if (parentView.GetType() == typeof(WorkflowDetailView))
                        {
                            WorkflowDetailView workflowDetailView = parentView as WorkflowDetailView;
                            workflowDetailView.SubmitLikeAction(_indexCommentSection, commentItem);
                        }
                        else if (parentView.GetType() == typeof(FormWorkFlowDetails))
                        {
                            FormWorkFlowDetails formWorkFlowDetails = parentView as FormWorkFlowDetails;
                            formWorkFlowDetails.SubmitLikeAction(_indexCommentSection, commentItem);
                        }
                        else if (parentView.GetType() == typeof(FormTaskDetails))
                        {
                            FormTaskDetails formTaskDetails = parentView as FormTaskDetails;
                            formTaskDetails.SubmitLikeAction(_indexCommentSection, commentItem);
                        }
                        else if (parentView.GetType() == typeof(FollowListViewController))
                        {
                            FollowListViewController followListViewController = parentView as FollowListViewController;
                            followListViewController.SubmitLikeAction(_indexCommentSection, commentItem);
                        }
                    });

                }
            });
        }

        private async void BT_sendComment_TouchUpInside(object sender, EventArgs e)
        {
            try
            {
                //loading = new CmmLoading(new CGRect((parentView.View.Bounds.Width - 200) / 2, (parentView.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
                loading_sendComment.StartAnimating();
                BT_sendComment.UserInteractionEnabled = false;
                BT_sendComment.Hidden = true;

                string commentvalue = "";

                if ((lst_attachment_doc != null && lst_attachment_doc.Count > 0) || (lst_attachment_img != null && lst_attachment_img.Count > 0))
                {
                    if (!string.IsNullOrEmpty(txt_note.Text) && txt_note.Text != hintDefault)
                        commentvalue = txt_note.Text;
                    else
                        commentvalue = "";
                }
                else
                {
                    if (!string.IsNullOrEmpty(txt_note.Text) && txt_note.Text != hintDefault)
                    {
                        commentvalue = txt_note.Text;
                    }
                    else
                    {
                        CmmIOSFunction.commonAlertMessage(parentView, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến."));
                        loading_sendComment.StopAnimating();
                        BT_sendComment.Hidden = false;
                        BT_sendComment.UserInteractionEnabled = true;
                        return;
                    }
                }

                string otherResourceId = string.Empty;

                if (parentView.GetType() == typeof(ToDoDetailView))
                {
                    ToDoDetailView toDoDetailView = parentView as ToDoDetailView;
                    otherResourceId = toDoDetailView._OtherResourceId;
                    lst_addAttachFile = toDoDetailView.lst_addCommentAttachment;
                }
                else if (parentView.GetType() == typeof(WorkflowDetailView))
                {
                    WorkflowDetailView workflowDetailView = parentView as WorkflowDetailView;
                    otherResourceId = workflowDetailView._OtherResourceId;
                    lst_addAttachFile = workflowDetailView.lst_addCommentAttachment;
                }
                else if (parentView.GetType() == typeof(FormWorkFlowDetails))
                {
                    FormWorkFlowDetails formWorkFlowDetails = parentView as FormWorkFlowDetails;
                    otherResourceId = formWorkFlowDetails._OtherResourceId;
                    lst_addAttachFile = formWorkFlowDetails.lst_addCommentAttachment;
                }
                else if (parentView.GetType() == typeof(FormTaskDetails))
                {
                    FormTaskDetails formTaskDetails = parentView as FormTaskDetails;
                    otherResourceId = formTaskDetails.OtherResourceId;
                    lst_addAttachFile = formTaskDetails.lst_addCommentAttachment;
                }
                else if (parentView.GetType() == typeof(FollowListViewController))
                {
                    FollowListViewController followListViewController = parentView as FollowListViewController;
                    otherResourceId = followListViewController._OtherResourceId;
                    lst_addAttachFile = followListViewController.lst_addCommentAttachment;
                }

                ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
                List<KeyValuePair<string, string>> _lstKeyVarAttachmentLocal = new List<KeyValuePair<string, string>>();
                if (lst_addAttachFile != null && lst_addAttachFile.Count > 0) // Lấy những file thêm mới từ App ra
                {
                    foreach (var item in lst_addAttachFile)
                    {
                        if (item.ID == "")
                        {
                            string key = item.Title;
                            KeyValuePair<string, string> _UploadFile = new KeyValuePair<string, string>(key, item.Path);
                            _lstKeyVarAttachmentLocal.Add(_UploadFile);
                        }
                    }
                }

                await Task.Run(() =>
                {
                    BeanOtherResource beanOtherResource = new BeanOtherResource();
                    beanOtherResource.Content = commentvalue;
                    beanOtherResource.ResourceId = otherResourceId;
                    beanOtherResource.ResourceCategoryId = (int)CmmFunction.CommentResourceCategoryID.WorkflowItem;
                    beanOtherResource.ResourceSubCategoryId = 0;
                    beanOtherResource.Image = "";
                    beanOtherResource.ParentCommentId = null; // cmt mới nên ko có parent

                    bool _result = p_dynamic.AddComment(beanOtherResource, _lstKeyVarAttachmentLocal);
                    if (_result)
                    {
                        ProviderBase p_base = new ProviderBase();
                        p_base.UpdateMasterData<BeanWorkflowItem>(null, true, 1, false);

                        InvokeOnMainThread(() =>
                        {
                            if (parentView.GetType() == typeof(ToDoDetailView))
                            {
                                ToDoDetailView toDoDetailView = parentView as ToDoDetailView;
                                txt_note.Text = hintDefault;
                                toDoDetailView.ReLoadDataFromServer(false, false);
                            }
                            else if (parentView.GetType() == typeof(WorkflowDetailView))
                            {
                                WorkflowDetailView workflowDetailView = parentView as WorkflowDetailView;
                                txt_note.Text = hintDefault;
                                workflowDetailView.ReLoadDataFromServer(false, false);
                            }
                            else if (parentView.GetType() == typeof(FormWorkFlowDetails))
                            {
                                FormWorkFlowDetails formWorkFlowDetails = parentView as FormWorkFlowDetails;
                                txt_note.Text = hintDefault;
                                formWorkFlowDetails.ReLoadDataFromServer();
                            }
                            else if (parentView.GetType() == typeof(FormTaskDetails))
                            {
                                FormTaskDetails formTaskDetails = parentView as FormTaskDetails;
                                formTaskDetails.lst_addCommentAttachment = null;
                                txt_note.Text = formTaskDetails.hintDefault = hintDefault;
                                formTaskDetails.ReLoadDataFromServer();
                            }
                            else if (parentView.GetType() == typeof(FollowListViewController))
                            {
                                FollowListViewController followListViewController = parentView as FollowListViewController;
                                txt_note.Text = hintDefault;
                                followListViewController.ReLoadDataFromServer(false, false);
                            }

                            loading_sendComment.StopAnimating();
                            BT_sendComment.UserInteractionEnabled = true;
                            BT_sendComment.Hidden = false;
                            //loading.Hide();
                        });
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {
                            CmmIOSFunction.commonAlertMessage(parentView, "BPM", CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"));
                            loading_sendComment.StopAnimating();
                            BT_sendComment.UserInteractionEnabled = true;
                            BT_sendComment.Hidden = false;
                            //loading.Hide();
                        });
                    }

                });
            }
            catch (Exception ex)
            {
                BT_sendComment.UserInteractionEnabled = true;
                Console.WriteLine("ControlInputComment - SubmitComment - Err: " + ex.ToString());
            }
        }

        private void HandleBtnReply(NSIndexPath _indexCommentSection, BeanComment commentItem)
        {
            commentSelectedIndexPath = _indexCommentSection;
            commentSelected = commentItem;

            if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView controller = (WorkflowDetailView)parentView;
                controller.NavigateToReplyComment(commentSelectedIndexPath, commentItem);
            }
            else if (parentView.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView toDoDetailView = parentView as ToDoDetailView;
                toDoDetailView.NavigateToReplyComment(commentSelectedIndexPath, commentItem);
            }
            else if (parentView.GetType() == typeof(FormWorkFlowDetails))
            {
                FormWorkFlowDetails formWorkFlowDetails = parentView as FormWorkFlowDetails;
                formWorkFlowDetails.NavigateToReplyComment(commentSelectedIndexPath, commentItem);
            }
            else if (parentView.GetType() == typeof(FormTaskDetails))
            {
                FormTaskDetails formTaskDetails = parentView as FormTaskDetails;
                formTaskDetails.NavigateToReplyComment(commentSelectedIndexPath, commentItem);
            }
            else if (parentView.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController followListViewController = parentView as FollowListViewController;
                followListViewController.NavigateToReplyComment(commentSelectedIndexPath, commentItem);
            }
        }

        public void UpdateTableSections(KeyValuePair<string, bool> _sectionState)
        {
            var index = lst_sectionState.FindIndex(x => x.Key == _sectionState.Key);
            lst_sectionState[index] = _sectionState;

            tableView_attachment.Source = new Attachment_TableSource(lst_attachment, this);
            tableView_attachment.ReloadSections(NSIndexSet.FromIndex(index), UITableViewRowAnimation.None);
        }

        #region custom views

        #region comment source table
        private class Comment_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellShareItemID");
            ControlInputComments parentView;
            List<BeanComment> lst_comments { get; set; }
            Dictionary<BeanComment, List<BeanComment>> dict_commentItem = new Dictionary<BeanComment, List<BeanComment>>();
            List<BeanComment> sectionKeys;
            private nfloat table_commentAttachHeaderHeight = 0;

            public Comment_TableSource(List<BeanComment> _lst_comment, ControlInputComments _parentview)
            {
                lst_comments = _lst_comment;
                parentView = _parentview;
                LoadData();
            }

            private void LoadData()
            {
                try
                {
                    if (lst_comments != null)
                    {
                        sectionKeys = lst_comments.Where(x => string.IsNullOrEmpty(x.ParentCommentId)).ToList();
                        sectionKeys = sectionKeys.OrderByDescending(x => x.Created).ToList();
                        List<BeanComment> lst_item = new List<BeanComment>();
                        foreach (var section in sectionKeys)
                        {
                            List<BeanComment> lst = lst_comments.Where(x => x.ParentCommentId == section.ID).OrderBy(o => o.Created).ToList();
                            dict_commentItem.Add(section, lst);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ControlInPutComponent - Comment_TableSource - LoadData - Err: " + ex.ToString());
                }
            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }
            /*
            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                nfloat height = 0;

                List<BeanAttachFile> lst_attachFiles = new List<BeanAttachFile>();
                List<BeanAttachFile> lst_AttachDoc = new List<BeanAttachFile>();
                List<BeanAttachFile> lst_attachImg = new List<BeanAttachFile>();

                var item = sectionKeys[Convert.ToInt32(section)];

                if (!string.IsNullOrEmpty(item.AttachFiles))
                {
                    JArray json = JArray.Parse(item.AttachFiles);
                    lst_attachFiles = json.ToObject<List<BeanAttachFile>>();

                    foreach (var attach in lst_attachFiles)
                    {
                        string fileExt = string.Empty;
                        if (!string.IsNullOrEmpty(attach.Url))
                            fileExt = attach.Url.Split('.').Last();

                        if (CmmFunction.CheckIsImageAttachmentType(fileExt))
                            lst_attachImg.Add(attach);
                        else
                            lst_AttachDoc.Add(attach);
                    }

                    if (lst_attachImg.Count > 0)
                        height = height + (lst_attachImg.Count * 200);

                    if (lst_AttachDoc.Count > 0)
                        height = height + (lst_AttachDoc.Count * 40);
                }

                nfloat heightText = 0;
                if (!string.IsNullOrEmpty(item.Content))
                {
                    CGRect rect = StringExtensions.StringRect(item.Content, UIFont.FromName("ArialMT", 14f), (parentView.table_comment.Frame.Width / 5) * 4.4f);
                    if (rect.Height > 0 && rect.Height < 20)
                        rect.Height = 30;
                    heightText = rect.Height + 50;
                }
                else
                    heightText = 80;


                table_commentAttachHeaderHeight = height + 20;
                return height + heightText + 40; // 50: buffer height
                //return height + heightText + 50; // 50: buffer height
            }
            */
            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                nfloat height = 0;

                List<BeanAttachFile> lst_attachFiles = new List<BeanAttachFile>();
                List<BeanAttachFile> lst_AttachDoc = new List<BeanAttachFile>();
                List<BeanAttachFile> lst_attachImg = new List<BeanAttachFile>();

                var item = sectionKeys[Convert.ToInt32(section)];

                if (!string.IsNullOrEmpty(item.AttachFiles))
                {
                    JArray json = JArray.Parse(item.AttachFiles);
                    lst_attachFiles = json.ToObject<List<BeanAttachFile>>();

                    foreach (var attach in lst_attachFiles)
                    {
                        string fileExt = string.Empty;
                        if (!string.IsNullOrEmpty(attach.Url))
                            fileExt = attach.Url.Split('.').Last();



                        if (CmmFunction.CheckIsImageAttachmentType(fileExt))
                            lst_attachImg.Add(attach);
                        else
                            lst_AttachDoc.Add(attach);
                    }

                    if (lst_attachImg.Count > 0)
                        height = height + (lst_attachImg.Count * 110);

                    if (lst_AttachDoc.Count > 0)
                        height = height + (lst_AttachDoc.Count * 35);
                }

                nfloat heightText = 0;
                nfloat width = UIScreen.MainScreen.Bounds.Width;
                if (!string.IsNullOrEmpty(item.Content))
                {
                    CGRect rect = StringExtensions.StringRect(item.Content, UIFont.FromName("ArialMT", 14f), width - 132); //padding view to content parrent
                    if (rect.Height > 0 && rect.Height < 20)
                        rect.Height = 30;
                    heightText = rect.Height + 50;
                }
                else
                    heightText = 80;

                table_commentAttachHeaderHeight = height;
                return height + heightText + 20; // 50: buffer height
                                                 //return height + heightText + 50; // 50: buffer height
            }
            /*
            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                try
                {
                    var sec = sectionKeys[(Int16)section];
                    var key = dict_commentItem.ElementAt(Convert.ToInt32(section)).Key;

                    if (dict_commentItem[key] != null)
                    {
                        #region height attachment
                        nfloat height = 0;

                        List<BeanAttachFile> lst_attachFiles1 = new List<BeanAttachFile>();
                        List<BeanAttachFile> lst_AttachDoc = new List<BeanAttachFile>();
                        List<BeanAttachFile> lst_attachImg = new List<BeanAttachFile>();

                        var item = sectionKeys[Convert.ToInt32(section)];

                        if (!string.IsNullOrEmpty(item.AttachFiles))
                        {
                            JArray json = JArray.Parse(item.AttachFiles);
                            lst_attachFiles1 = json.ToObject<List<BeanAttachFile>>();

                            foreach (var attach in lst_attachFiles1)
                            {
                                string fileExt = string.Empty;
                                if (!string.IsNullOrEmpty(attach.Url))
                                    fileExt = attach.Url.Split('.').Last();

                                if (CmmFunction.CheckIsImageAttachmentType(fileExt))
                                    lst_attachImg.Add(attach);
                                else
                                    lst_AttachDoc.Add(attach);
                            }

                            if (lst_attachImg.Count > 0)
                                height = height + (lst_attachImg.Count * 200);

                            if (lst_AttachDoc.Count > 0)
                                height = height + (lst_AttachDoc.Count * 40);
                        }
                        else
                            table_commentAttachHeaderHeight = 0;

                        nfloat heightText = 0;
                        if (!string.IsNullOrEmpty(item.Content))
                        {
                            CGRect rect = StringExtensions.StringRect(item.Content, UIFont.FromName("ArialMT", 14f), (parentView.table_comment.Frame.Width / 5) * 4.4f);
                            if (rect.Height > 0 && rect.Height < 20)
                                rect.Height = 30;
                            heightText = rect.Height + 50;
                        }
                        else
                            heightText = 80;

                        table_commentAttachHeaderHeight = height + 20;

                        #endregion
                        UIView baseView = new UIView();
                        baseView.Frame = new CGRect(0, 0, tableView.Frame.Width, 100);

                        UITableView table_commentAttachment = new UITableView();
                        table_commentAttachment.ScrollEnabled = false;
                        table_commentAttachment.SeparatorStyle = UITableViewCellSeparatorStyle.None;

                        // test layout
                        //baseView.BackgroundColor = UIColor.Green;
                        //table_commentAttachment.BackgroundColor = UIColor.Red;

                        UILabel lbl_createdDate = new UILabel()
                        {
                            Font = UIFont.FromName("ArialMT", 13f),
                            TextColor = UIColor.FromRGB(94, 94, 94),
                            TextAlignment = UITextAlignment.Right
                        };

                        UIImageView img_avatar = new UIImageView();
                        img_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;

                        UILabel lbl_title = new UILabel()
                        {
                            Font = UIFont.FromName("ArialMT", 14f),
                            TextColor = UIColor.Black
                        };

                        UILabel lbl_avatar = new UILabel()
                        {
                            Font = UIFont.FromName("Arial-BoldMT", 16f),
                            TextColor = UIColor.White
                        };

                        UILabel lbl_sub_title = new UILabel()
                        {
                            Font = UIFont.FromName("ArialMT", 13f),
                            TextColor = UIColor.FromRGB(94, 94, 94)
                        };

                        UILabel lbl_ykien = new UILabel
                        {
                            TextAlignment = UITextAlignment.Left,
                            Font = UIFont.FromName("ArialMT", 14f),
                            LineBreakMode = UILineBreakMode.WordWrap,
                            Lines = 0,
                            TextColor = UIColor.Black,
                            Hidden = false
                        };

                        UIButton BT_like = new UIButton();
                        BT_like.Font = UIFont.FromName("ArialMT", 11f);
                        //BT_like.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                        BT_like.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
                        if (key.IsLiked)
                            BT_like.SetTitle(CmmFunction.GetTitle("TEXT_UNLIKE", "Bỏ thích"), UIControlState.Normal);
                        else
                            BT_like.SetTitle(CmmFunction.GetTitle("TEXT_LIKE", "Thích"), UIControlState.Normal);

                        BT_like.TouchUpInside += delegate
                        {
                            NSIndexPath indexpath = NSIndexPath.FromItemSection(0, section);
                            parentView.HandleBtnLike(indexpath, key);
                        };

                        UIButton BT_reply = new UIButton();
                        BT_reply.Font = UIFont.FromName("ArialMT", 11f);
                        //BT_reply.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                        BT_reply.SetTitle(CmmFunction.GetTitle("TEXT_REPLY", "Trả lời"), UIControlState.Normal);
                        BT_reply.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
                        BT_reply.TouchUpInside += delegate
                        {
                            NSIndexPath indexpath = NSIndexPath.FromItemSection(0, section);
                            parentView.HandleBtnReply(indexpath, key);
                        };

                        BT_like.SetTitleColor(UIColor.FromRGB(0, 95, 212), UIControlState.Normal);
                        BT_reply.SetTitleColor(UIColor.FromRGB(0, 95, 212), UIControlState.Normal);

                        //============== Set Value =============

                        if (!string.IsNullOrEmpty(key.AttachFiles))
                        {
                            JArray json = JArray.Parse(key.AttachFiles);
                            List<BeanAttachFile> newSortList = new List<BeanAttachFile>();
                            var lst_attachFiles = json.ToObject<List<BeanAttachFile>>();

                            foreach (var attach in lst_attachFiles)
                            {
                                string fileExt = string.Empty;
                                if (!string.IsNullOrEmpty(attach.Url))
                                    fileExt = attach.Url.Split('.').Last();

                                bool isThumb = CmmFunction.CheckIsImageAttachmentType(fileExt);
                                if (isThumb)
                                {
                                    newSortList.Insert(0, attach);
                                    //table_commentAttachHeaderHeight = table_commentAttachHeaderHeight + 200;
                                }
                                else
                                {
                                    newSortList.Insert(newSortList.Count, attach);
                                    //table_commentAttachHeaderHeight = table_commentAttachHeaderHeight + 30;
                                }
                            }

                            table_commentAttachment.Source = new CommentAttachment_TableSource(newSortList, parentView);
                            table_commentAttachment.ReloadData();
                        }
                        else
                        {
                            table_commentAttachment.Hidden = true;
                        }

                        var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);

                        //set thong tin ngay tao
                        if (key.Created.HasValue)
                        {
                            if (CmmVariable.SysConfig.LangCode == "1033")
                                lbl_createdDate.Text = key.Created.Value.ToString(CmmVariable.M_WorkDateFormatDateTimeEN);
                            else if (CmmVariable.SysConfig.LangCode == "1066")
                                lbl_createdDate.Text = key.Created.Value.ToString(CmmVariable.M_WorkDateFormatDateTimeVN);
                        }

                        //lay thong tin user name
                        if (!string.IsNullOrEmpty(key.Author))
                        {

                            BeanUser user = CmmFunction.GetBeanUserByID(key.Author);
                            if (user != null)
                            {
                                lbl_title.Text = user.FullName;

                                //lay thong tin avatar cua User
                                string user_imagePath = "";
                                user_imagePath = user.ImagePath;
                                lbl_title.Text = user.FullName;
                                lbl_sub_title.Text = user.Position;

                                if (string.IsNullOrEmpty(user_imagePath))
                                {

                                    lbl_avatar.Hidden = false;
                                    img_avatar.Hidden = true;
                                    lbl_avatar.Text = CmmFunction.GetAvatarName(user.FullName);
                                    lbl_avatar.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_avatar.Text));

                                }
                                else
                                {
                                    lbl_avatar.Hidden = false;
                                    img_avatar.Hidden = true;
                                    lbl_avatar.Text = CmmFunction.GetAvatarName(user.FullName);
                                    lbl_avatar.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_avatar.Text));

                                    checkFileLocalIsExist(user, lbl_avatar, img_avatar);

                                    //kiem tra xong cap nhat lai avatar
                                    lbl_avatar.Hidden = true;
                                    img_avatar.Hidden = false;
                                }

                            }
                        }

                        UIImageView iv_attach = new UIImageView();
                        iv_attach.ContentMode = UIViewContentMode.ScaleAspectFill;
                        iv_attach.Image = UIImage.FromFile("Icons/icon_attachDark.png");
                        iv_attach.Hidden = true;

                        UILabel lbl_attach_count = new UILabel()
                        {
                            Font = UIFont.FromName("ArialMT", 11f),
                            TextColor = UIColor.FromRGB(94, 94, 94),
                            Hidden = true
                        };

                        if (!string.IsNullOrEmpty(key.AttachFiles))
                        {
                            List<BeanAttachFile> _lstAttach = JsonConvert.DeserializeObject<List<BeanAttachFile>>(key.AttachFiles);
                            if (_lstAttach != null && _lstAttach.Count > 0)
                            {
                                iv_attach.Hidden = false;
                                lbl_attach_count.Hidden = false;
                                lbl_attach_count.Text = _lstAttach.Count.ToString();
                            }
                        }

                        UIImageView iv_like = new UIImageView();
                        iv_like.ContentMode = UIViewContentMode.ScaleAspectFill;
                        iv_like.Image = UIImage.FromFile("Icons/icon_like.png");
                        iv_like.Hidden = true;


                        UILabel lbl_like_count = new UILabel()
                        {
                            Font = UIFont.FromName("ArialMT", 11f),
                            TextColor = UIColor.FromRGB(94, 94, 94),
                            Hidden = true
                        };

                        if (key.LikeCount > 0)
                        {
                            iv_like.Hidden = false;
                            lbl_like_count.Hidden = false;
                            lbl_like_count.Text = key.LikeCount.ToString();
                        }

                        // set noi dung comment
                        lbl_ykien.Text = key.Content;

                        img_avatar.Frame = new CGRect(10, 10, 40, 40);
                        img_avatar.Layer.CornerRadius = 20;
                        img_avatar.ClipsToBounds = true;
                        lbl_avatar.Frame = new CGRect(10, 10, 40, 40);
                        lbl_avatar.Layer.CornerRadius = 20;
                        lbl_avatar.ClipsToBounds = true;

                        lbl_title.Frame = new CGRect(img_avatar.Frame.Right + 10, 10, ((baseView.Frame.Width - img_avatar.Frame.Right) / 3) * 2.3f, 20);
                        lbl_createdDate.Frame = new CGRect(baseView.Frame.Width - 100, 10, 90, 20);
                        lbl_sub_title.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom, (baseView.Frame.Width - lbl_title.Frame.X) - 10, 20);
                        iv_attach.Frame = new CGRect(baseView.Frame.Width - 90, lbl_sub_title.Frame.Y + 2, 14, 14);
                        lbl_attach_count.Frame = new CGRect(iv_attach.Frame.Right + 5, lbl_sub_title.Frame.Y, 20, 20);
                        iv_like.Frame = new CGRect(lbl_attach_count.Frame.Right + 5, lbl_sub_title.Frame.Y + 2, 14, 14);
                        lbl_like_count.Frame = new CGRect(iv_like.Frame.Right + 5, lbl_sub_title.Frame.Y, 30, 20);

                        if (!string.IsNullOrEmpty(key.Content))
                        {
                            lbl_ykien.Hidden = false;
                            lbl_ykien.Frame = new CGRect(lbl_title.Frame.X, lbl_sub_title.Frame.Bottom, lbl_sub_title.Frame.Width, 20);
                            lbl_ykien.Text = key.Content;
                            lbl_ykien.SizeToFit();
                        }
                        else
                            lbl_ykien.Frame = new CGRect(lbl_title.Frame.X, lbl_sub_title.Frame.Bottom, lbl_sub_title.Frame.Width, 1);

                        table_commentAttachment.Frame = new CGRect(lbl_title.Frame.X, lbl_ykien.Frame.Bottom + 5, (baseView.Frame.Width - lbl_title.Frame.X), table_commentAttachHeaderHeight);
                        BT_like.Frame = new CGRect(lbl_title.Frame.X, table_commentAttachment.Frame.Bottom + 5, 60, 20);
                        BT_reply.Frame = new CGRect(BT_like.Frame.Right + 15, table_commentAttachment.Frame.Bottom + 5, 60, 20);

                        baseView.AddSubviews(new UIView[] { img_avatar, lbl_avatar, lbl_title, lbl_createdDate, lbl_sub_title, lbl_ykien, iv_like, lbl_like_count, iv_attach, lbl_attach_count, table_commentAttachment, BT_like, BT_reply });
                        return baseView;
                    }
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("FormCommentView - Comment_TableSource-GetViewForHeader - Err: " + ex.ToString());
                    return null;
                }
            }
            */
            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                try
                {
                    var sec = sectionKeys[(Int16)section];
                    var key = dict_commentItem.ElementAt(Convert.ToInt32(section)).Key;

                    if (dict_commentItem[key] != null)
                    {
                        UIView baseView = new UIView();
                        baseView.Frame = new CGRect(0, 0, tableView.Frame.Width, 100);

                        UITableView table_commentAttachment = new UITableView();
                        table_commentAttachment.ScrollEnabled = false;
                        table_commentAttachment.SeparatorStyle = UITableViewCellSeparatorStyle.None;

                        // test layout
                        //baseView.BackgroundColor = UIColor.Green;
                        //table_commentAttachment.BackgroundColor = UIColor.Red;

                        UILabel lbl_createdDate = new UILabel()
                        {
                            Font = UIFont.FromName("ArialMT", 12f),
                            TextColor = UIColor.FromRGB(94, 94, 94),
                            TextAlignment = UITextAlignment.Right
                        };

                        UIImageView img_avatar = new UIImageView();
                        img_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;

                        UILabel lbl_title = new UILabel()
                        {
                            Font = UIFont.FromName("ArialMT", 14f),
                            TextColor = UIColor.Black
                        };

                        UILabel lbl_avatar = new UILabel()
                        {
                            Font = UIFont.FromName("Arial-BoldMT", 14f),
                            TextColor = UIColor.White
                        };

                        UILabel lbl_sub_title = new UILabel()
                        {
                            Font = UIFont.FromName("ArialMT", 12f),
                            TextColor = UIColor.FromRGB(94, 94, 94)
                        };

                        UILabel lbl_ykien = new UILabel
                        {
                            TextAlignment = UITextAlignment.Left,
                            Font = UIFont.FromName("ArialMT", 14f),
                            LineBreakMode = UILineBreakMode.WordWrap,
                            Lines = 0,
                            TextColor = UIColor.Black,
                            Hidden = false
                        };
                        UIButton BT_like = new UIButton();
                        BT_like.Font = UIFont.FromName("ArialMT", 11f);
                        BT_like.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
                        /*if (key.IsLiked)
                            BT_like.SetTitleColor(UIColor.FromRGB(0, 95, 212), UIControlState.Normal);
                        else
                            BT_like.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);*/
                        if (key.IsLiked)
                            BT_like.SetTitle(CmmFunction.GetTitle("TEXT_UNLIKE", "Bỏ thích"), UIControlState.Normal);
                        else
                            BT_like.SetTitle(CmmFunction.GetTitle("TEXT_LIKE", "Thích"), UIControlState.Normal);

                        BT_like.TouchUpInside += delegate
                        {
                            NSIndexPath indexpath = NSIndexPath.FromItemSection(0, section);
                            parentView.HandleBtnLike(indexpath, key);
                        };

                        UIButton BT_reply = new UIButton();
                        BT_reply.Font = UIFont.FromName("ArialMT", 11f);
                        //BT_reply.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                        BT_reply.SetTitle(CmmFunction.GetTitle("TEXT_REPLY", "Trả lời"), UIControlState.Normal);
                        BT_reply.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
                        BT_reply.TouchUpInside += delegate
                        {
                            NSIndexPath indexpath = NSIndexPath.FromItemSection(0, section);
                            parentView.HandleBtnReply(indexpath, key);
                        };

                        BT_like.SetTitleColor(UIColor.FromRGB(0, 95, 212), UIControlState.Normal);
                        BT_reply.SetTitleColor(UIColor.FromRGB(0, 95, 212), UIControlState.Normal);
                        //============== Set Value =============

                        if (!string.IsNullOrEmpty(key.AttachFiles))
                        {
                            JArray json = JArray.Parse(key.AttachFiles);
                            List<BeanAttachFile> newSortList = new List<BeanAttachFile>();
                            var lst_attachFiles = json.ToObject<List<BeanAttachFile>>();

                            foreach (var attach in lst_attachFiles)
                            {
                                string fileExt = string.Empty;
                                if (!string.IsNullOrEmpty(attach.Url))
                                    fileExt = attach.Url.Split('.').Last();

                                bool isThumb = CmmFunction.CheckIsImageAttachmentType(fileExt);
                                if (isThumb)
                                    newSortList.Insert(0, attach);
                                else
                                    newSortList.Insert(newSortList.Count, attach);
                            }
                            table_commentAttachment.Source = new CommentAttachment_TableSource(newSortList, parentView);
                            table_commentAttachment.ReloadData();
                        }
                        else
                        {
                            table_commentAttachHeaderHeight = 0;
                            table_commentAttachment.Hidden = true;
                        }
                        var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);

                        //set thong tin ngay tao
                        if (key.Created.HasValue)
                        {
                            if (CmmVariable.SysConfig.LangCode == "1033")
                                lbl_createdDate.Text = key.Created.Value.ToString(CmmVariable.M_WorkDateFormatDateTimeEN);
                            else //if (CmmVariable.SysConfig.LangCode == "1066")
                                lbl_createdDate.Text = key.Created.Value.ToString(CmmVariable.M_WorkDateFormatDateTimeVN);
                        }

                        //lay thong tin user name
                        if (!string.IsNullOrEmpty(key.Author))
                        {
                            BeanUser user = CmmFunction.GetBeanUserByID(key.Author);
                            if (user != null)
                            {
                                lbl_title.Text = user.FullName;

                                //lay thong tin avatar cua User
                                string user_imagePath = "";
                                user_imagePath = user.ImagePath;
                                lbl_title.Text = user.FullName;
                                lbl_sub_title.Text = user.Position;
                                if (string.IsNullOrEmpty(user_imagePath))
                                {
                                    lbl_avatar.Hidden = false;
                                    img_avatar.Hidden = true;
                                    lbl_avatar.Text = CmmFunction.GetAvatarName(user.FullName);
                                    lbl_avatar.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_avatar.Text));
                                }
                                else
                                {
                                    lbl_avatar.Hidden = false;
                                    img_avatar.Hidden = true;
                                    lbl_avatar.Text = CmmFunction.GetAvatarName(user.FullName);
                                    lbl_avatar.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_avatar.Text));

                                    checkFileLocalIsExist(user, lbl_avatar, img_avatar);

                                    //kiem tra xong cap nhat lai avatar
                                    lbl_avatar.Hidden = true;
                                    img_avatar.Hidden = false;
                                }
                            }
                        }

                        UIImageView iv_attach = new UIImageView();
                        iv_attach.ContentMode = UIViewContentMode.ScaleAspectFill;
                        iv_attach.Image = UIImage.FromFile("Icons/icon_attachDark.png");
                        iv_attach.Hidden = true;

                        UILabel lbl_attach_count = new UILabel()
                        {
                            Font = UIFont.FromName("ArialMT", 11f),
                            TextColor = UIColor.FromRGB(94, 94, 94),
                            Hidden = true
                        };

                        if (!string.IsNullOrEmpty(key.AttachFiles))
                        {
                            List<BeanAttachFile> _lstAttach = JsonConvert.DeserializeObject<List<BeanAttachFile>>(key.AttachFiles);
                            if (_lstAttach != null && _lstAttach.Count > 0)
                            {
                                iv_attach.Hidden = false;
                                lbl_attach_count.Hidden = false;
                                lbl_attach_count.Text = _lstAttach.Count.ToString();
                            }
                        }

                        UIImageView iv_like = new UIImageView();
                        iv_like.ContentMode = UIViewContentMode.ScaleAspectFill;
                        iv_like.Image = UIImage.FromFile("Icons/icon_like.png");
                        iv_like.Hidden = true;

                        UILabel lbl_like_count = new UILabel()
                        {
                            Font = UIFont.FromName("ArialMT", 11f),
                            TextColor = UIColor.FromRGB(94, 94, 94),
                            Hidden = true
                        };

                        if (key.LikeCount > 0)
                        {
                            iv_like.Hidden = false;
                            lbl_like_count.Hidden = false;
                            lbl_like_count.Text = key.LikeCount.ToString();
                        }

                        // set noi dung comment
                        lbl_ykien.Text = key.Content;

                        img_avatar.Frame = new CGRect(0, 10, 36, 36);
                        img_avatar.Layer.CornerRadius = 18;
                        img_avatar.ClipsToBounds = true;
                        lbl_avatar.Frame = new CGRect(0, 10, 36, 36);
                        lbl_avatar.Layer.CornerRadius = 18;
                        lbl_avatar.ClipsToBounds = true;

                        lbl_title.Frame = new CGRect(img_avatar.Frame.Right + 10, 10, ((baseView.Frame.Width - img_avatar.Frame.Right) / 3) * 2, 16);
                        lbl_createdDate.Frame = new CGRect(baseView.Frame.Width - 100, 10, 90, 16);
                        lbl_sub_title.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom + 7, (baseView.Frame.Width - lbl_title.Frame.X) - 100, 14);
                        iv_attach.Frame = new CGRect(baseView.Frame.Width - 90, lbl_sub_title.Frame.Y + 2, 14, 14);
                        lbl_attach_count.Frame = new CGRect(iv_attach.Frame.Right + 5, lbl_sub_title.Frame.Y, 14, 14);
                        iv_like.Frame = new CGRect(lbl_attach_count.Frame.Right + 5, lbl_sub_title.Frame.Y + 2, 14, 14);
                        lbl_like_count.Frame = new CGRect(iv_like.Frame.Right + 5, lbl_sub_title.Frame.Y, 14, 14);

                        if (!string.IsNullOrEmpty(key.Content))
                        {
                            lbl_ykien.Hidden = false;
                            lbl_ykien.Frame = new CGRect(lbl_title.Frame.X, lbl_sub_title.Frame.Bottom + 7, baseView.Frame.Width - lbl_title.Frame.X - 25, 20);
                            lbl_ykien.Text = key.Content;
                            lbl_ykien.SizeToFit();
                        }
                        else
                            lbl_ykien.Frame = new CGRect(lbl_title.Frame.X, lbl_sub_title.Frame.Bottom + 7, baseView.Frame.Width - lbl_title.Frame.X - 25, 1);

                        //lbl_ykien.BackgroundColor = UIColor.Purple;
                        table_commentAttachment.Frame = new CGRect(lbl_title.Frame.X, lbl_ykien.Frame.Bottom + 5, (baseView.Frame.Width - lbl_title.Frame.X), table_commentAttachHeaderHeight);

                        BT_like.Frame = new CGRect(lbl_title.Frame.X, table_commentAttachment.Frame.Bottom + 5, 60, 15);
                        BT_reply.Frame = new CGRect(BT_like.Frame.Right, table_commentAttachment.Frame.Bottom + 5, 45, 15);

                        baseView.AddSubviews(new UIView[] { img_avatar, lbl_avatar, lbl_title, lbl_createdDate, lbl_sub_title, lbl_ykien, iv_like, lbl_like_count, iv_attach, lbl_attach_count, table_commentAttachment, BT_like, BT_reply });
                        return baseView;
                    }
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("FormCommentView - Comment_TableSource-GetViewForHeader - Err: " + ex.ToString());
                    return null;
                }
            }

            //Row - Comment
            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                nfloat height = 90;
                var comment = dict_commentItem[sectionKeys[indexPath.Section]][indexPath.Row];

                // comment co dinh kem
                if (!string.IsNullOrEmpty(comment.AttachFiles))
                {
                    JArray json = JArray.Parse(comment.AttachFiles);
                    List<BeanAttachFile> newSortList = new List<BeanAttachFile>();
                    var lst_attachFiles = json.ToObject<List<BeanAttachFile>>();

                    foreach (var attach in lst_attachFiles)
                    {
                        string fileExt = string.Empty;
                        if (!string.IsNullOrEmpty(attach.Url))
                            fileExt = attach.Url.Split('.').Last();

                        bool isThumb = CmmFunction.CheckIsImageAttachmentType(fileExt);
                        if (isThumb)
                        {
                            height = height + 200;
                        }
                        else
                        {
                            newSortList.Insert(newSortList.Count, attach);
                            height = height + 40;
                        }
                    }

                }
                // comment khong co dinh kem
                //else
                //{
                //    height = 190;
                //}


                //if (!string.IsNullOrEmpty(comment.AttachFiles))
                //{
                //    height = height + 190;
                //}
                //else
                //    height = height + 50;


                return height + 10;

                //if (!string.IsNullOrEmpty(commentitem.AttachFiles))
                //{
                //    if (commentitem.Content == "cmt web 5")
                //    {

                //    }
                //    return 300;
                //}
                //else
                //    return 100;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return sectionKeys.Count;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                //foreach (var i in dict_shareitem)
                //{
                return dict_commentItem[sectionKeys[(int)section]].Count;
                //}
                //return 0;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                //var item = lst_workRelated[indexPath.Row];
                //parentView.HandleSelectedItem(item);
                //tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var commentitem = dict_commentItem[sectionKeys[indexPath.Section]][indexPath.Row];

                Custom_CommentItemCell cell = new Custom_CommentItemCell(cellIdentifier);

                int section = indexPath.Section;
                cell.UpdateCell(commentitem, indexPath, parentView);
                //cell.BackgroundColor = UIColor.Red;
                return cell;
            }

            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            private async void checkFileLocalIsExist(BeanUser contact, UILabel label_cover, UIImageView image_view)
            {
                try
                {
                    string filename = contact.ImagePath.Split('/').Last();
                    string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + contact.ImagePath;
                    string localfilePath = Path.Combine(localDocumentFilepath, filename);

                    if (!File.Exists(localfilePath))
                    {
                        UIImage avatar = null;
                        await Task.Run(() =>
                        {
                            ProviderBase provider = new ProviderBase();
                            if (provider.DownloadFile(filepathURL, localfilePath, CmmVariable.M_AuthenticatedHttpClient))
                            {
                                NSData data = NSData.FromUrl(new NSUrl(localfilePath, false));

                                InvokeOnMainThread(() =>
                                {
                                    if (data != null)
                                    {
                                        UIImage image = UIImage.LoadFromData(data);
                                        if (image != null)
                                        {
                                            avatar = CmmIOSFunction.MaxResizeImage(image, 200, 200);
                                            image_view.Image = avatar;
                                        }
                                        else
                                            image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                                    }
                                    else
                                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");

                                    image_view.Hidden = false;

                                    //kiem tra xong cap nhat lai avatar
                                    label_cover.Hidden = true;
                                    image_view.Hidden = false;
                                });

                                if (data != null && avatar != null)
                                {
                                    NSError err = null;
                                    NSData imgData = avatar.AsPNG();
                                    if (imgData.Save(localfilePath, false, out err))
                                        Console.WriteLine("saved as " + localfilePath);
                                    return;
                                }
                            }
                            else
                            {
                                InvokeOnMainThread(() =>
                                {
                                    image_view.Hidden = true;
                                    label_cover.Hidden = false;
                                });
                            }
                        });
                    }
                    else
                    {
                        openFile(filename, image_view);
                        image_view.Hidden = false;
                        label_cover.Hidden = true;
                    }
                }
                catch (Exception ex)
                {
                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                    Console.WriteLine("ListUserView - checkFileLocalIsExist - Err: " + ex.ToString());
                    //CmmIOSFunction.IOSlog(null, "PopupContactDetailView - loadAvatar - " + ex.ToString());
                }
            }

            private async void openFile(string localfilename, UIImageView image_view)
            {
                try
                {
                    NSData data = null;
                    await Task.Run(() =>
                    {
                        string localfilePath = Path.Combine(localDocumentFilepath, localfilename);
                        data = NSData.FromUrl(new NSUrl(localfilePath, false));
                    });

                    if (data != null)
                    {
                        UIImage image = UIImage.LoadFromData(data);
                        if (image != null)
                        {
                            image_view.Image = image;
                        }
                        else
                        {
                            image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                        }
                    }
                    else
                    {
                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ContactView - openFile - Err: " + ex.ToString());
                }

            }
        }

        public class Custom_CommentItemCell : UITableViewCell
        {
            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            UILabel lbl_imgCover, lbl_note, lbl_title, lbl_createdDate, lbl_subTitle, lbl_attach_count, lbl_like_count;
            UITextView tv_comment;
            UITableView table_attachment;
            private bool isOdd;
            private UIImageView iv_avatar, iv_attach, iv_like;
            UIButton BT_like, BT_reply;
            BeanComment commentItem;
            List<BeanAttachFile> lst_attachFiles;
            string currentWorkFlowID;
            NSIndexPath indexpath;
            ControlInputComments parentView;

            public Custom_CommentItemCell(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
            {
                Accessory = UITableViewCellAccessory.None;
            }

            public void UpdateCell(BeanComment _comment, NSIndexPath _indexpath, ControlInputComments _parentview)
            {
                commentItem = _comment;
                indexpath = _indexpath;
                parentView = _parentview;
                ViewConfiguration();
                LoadData();
            }

            private void ViewConfiguration()
            {
                //if (isOdd)
                //    ContentView.BackgroundColor = UIColor.White;
                //else
                //    ContentView.BackgroundColor = UIColor.FromRGB(250, 250, 250);

                iv_avatar = new UIImageView();
                iv_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;
                iv_avatar.ClipsToBounds = true;
                iv_avatar.Layer.CornerRadius = 20;
                iv_avatar.Hidden = true;

                lbl_imgCover = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                    TextColor = UIColor.White
                };
                lbl_imgCover.Layer.CornerRadius = 20;
                lbl_imgCover.ClipsToBounds = true;

                lbl_title = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.Black,
                    TextAlignment = UITextAlignment.Left,
                };

                lbl_createdDate = new UILabel
                {
                    Font = UIFont.FromName("ArialMT", 12f),
                    TextColor = UIColor.FromRGB(94, 94, 94),
                    TextAlignment = UITextAlignment.Right,
                };

                lbl_subTitle = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 12f),
                    TextColor = UIColor.FromRGB(94, 94, 94)
                };

                iv_attach = new UIImageView();
                iv_attach.ContentMode = UIViewContentMode.ScaleAspectFill;
                iv_attach.Image = UIImage.FromFile("Icons/icon_attachDark.png");
                iv_attach.Hidden = true;

                lbl_attach_count = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 11f),
                    TextColor = UIColor.FromRGB(94, 94, 94),
                    Hidden = true
                };

                iv_like = new UIImageView();
                iv_like.ContentMode = UIViewContentMode.ScaleAspectFill;
                iv_like.Image = UIImage.FromFile("Icons/icon_like.png");
                iv_like.Hidden = true;

                lbl_like_count = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 11f),
                    TextColor = UIColor.FromRGB(94, 94, 94),
                    Hidden = true
                };

                tv_comment = new UITextView
                {
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.Black,
                    ScrollEnabled = false,
                    Editable = false
                };

                table_attachment = new UITableView(new CGRect(0, 0, 0, 0), UITableViewStyle.Plain);
                table_attachment.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                //table_attachment = new UITableView();
                table_attachment.ScrollEnabled = false;

                BT_like = new UIButton();
                BT_like.Font = UIFont.FromName("ArialMT", 11f);
                //BT_like.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                BT_like.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
                BT_like.TouchUpInside += delegate
                {
                    parentView.HandleBtnLike(indexpath, commentItem);
                };

                BT_reply = new UIButton();
                BT_reply.Font = UIFont.FromName("ArialMT", 11f);
                //BT_reply.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                BT_reply.SetTitle(CmmFunction.GetTitle("TEXT_REPLY", "Trả lời"), UIControlState.Normal);
                BT_reply.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
                BT_reply.TouchUpInside += delegate
                {
                    parentView.HandleBtnReply(indexpath, commentItem);
                };

                BT_like.SetTitleColor(UIColor.FromRGB(0, 95, 212), UIControlState.Normal);
                BT_reply.SetTitleColor(UIColor.FromRGB(0, 95, 212), UIControlState.Normal);

                ContentView.AddSubviews(new UIView[] { iv_avatar, lbl_imgCover, lbl_title, lbl_createdDate, lbl_subTitle, iv_attach, lbl_attach_count, iv_like, lbl_like_count, tv_comment, table_attachment, BT_like, BT_reply });
            }

            private void LoadData()
            {
                try
                {
                    var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);

                    LoadAttachments();

                    //lay thong tin user name - position
                    if (!string.IsNullOrEmpty(commentItem.Author))
                    {
                        BeanUser user = CmmFunction.GetBeanUserByID(commentItem.Author);
                        if (user != null)
                        {
                            lbl_title.Text = user.FullName;

                            //lay thong tin avatar cua User
                            string user_imagePath = "";
                            user_imagePath = user.ImagePath;
                            lbl_title.Text = user.FullName;
                            lbl_subTitle.Text = user.Position;
                            if (string.IsNullOrEmpty(user_imagePath))
                            {

                                lbl_imgCover.Hidden = false;
                                iv_avatar.Hidden = true;
                                lbl_imgCover.Text = CmmFunction.GetAvatarName(user.FullName);
                                lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                            }
                            else
                            {
                                lbl_imgCover.Hidden = false;
                                iv_avatar.Hidden = true;
                                lbl_imgCover.Text = CmmFunction.GetAvatarName(user.FullName);
                                lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));

                                checkFileLocalIsExist(user, lbl_imgCover, iv_avatar);

                                //kiem tra xong cap nhat lai avatar
                                lbl_imgCover.Hidden = true;
                                iv_avatar.Hidden = false;
                            }

                            //// lay thong tin user position
                            //BeanPosition position = CmmFunction.GetUserBeanPositionByUserID(user.PositionID.Value);
                            //if (position != null)
                            //{
                            //    lbl_subTitle.Text = position.Title;
                            //}
                        }
                    }

                    if (commentItem.Created.HasValue)
                    {
                        if (CmmVariable.SysConfig.LangCode == "1033")
                            lbl_createdDate.Text = commentItem.Created.Value.ToString(CmmVariable.M_WorkDateFormatDateTimeEN);
                        else
                            lbl_createdDate.Text = commentItem.Created.Value.ToString(CmmVariable.M_WorkDateFormatDateTimeVN);
                    }

                    if (!string.IsNullOrEmpty(commentItem.AttachFiles))
                    {
                        List<BeanAttachFile> _lstAttach = JsonConvert.DeserializeObject<List<BeanAttachFile>>(commentItem.AttachFiles);
                        if (_lstAttach != null && _lstAttach.Count > 0)
                        {
                            iv_attach.Hidden = false;
                            lbl_attach_count.Hidden = false;
                            lbl_attach_count.Text = _lstAttach.Count.ToString();
                        }
                    }

                    if (commentItem.LikeCount > 0)
                    {
                        iv_like.Hidden = false;
                        lbl_like_count.Hidden = false;
                        lbl_like_count.Text = commentItem.LikeCount.ToString();
                    }

                    tv_comment.Text = commentItem.Content;

                    if (commentItem.IsLiked)
                        BT_like.SetTitle(CmmFunction.GetTitle("TEXT_UNLIKE", "Bỏ thích"), UIControlState.Normal);
                    else
                        BT_like.SetTitle(CmmFunction.GetTitle("TEXT_LIKE", "Thích"), UIControlState.Normal);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("MainView - Todo_cell_custom - loaddata- ERR: " + ex.ToString());
                }
            }

            private void LoadAttachments()
            {
                if (!string.IsNullOrEmpty(commentItem.AttachFiles))
                {
                    List<BeanAttachFile> newSortList = new List<BeanAttachFile>();
                    JArray json = JArray.Parse(commentItem.AttachFiles);
                    lst_attachFiles = json.ToObject<List<BeanAttachFile>>();

                    foreach (var attach in lst_attachFiles)
                    {
                        string fileExt = string.Empty;
                        if (!string.IsNullOrEmpty(attach.Url))
                            fileExt = attach.Url.Split('.').Last();

                        bool isThumb = CmmFunction.CheckIsImageAttachmentType(fileExt);
                        if (isThumb)
                            newSortList.Insert(0, attach);
                        else
                            newSortList.Insert(newSortList.Count, attach);
                    }

                    table_attachment.Source = new CommentAttachment_TableSource(newSortList, parentView);
                    table_attachment.ReloadData();
                }
            }

            public async void SubmitLikeAction(NSIndexPath sectionIndex, BeanComment comment)
            {
                ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
                _ = Task.Run(() =>
                {
                    bool res;
                    res = p_dynamic.SetLikeComment(comment.ID, !comment.IsLiked);
                    if (res)
                    {
                        var conn = new SQLiteConnection(CmmVariable.M_DataPath);

                        comment.IsLiked = !comment.IsLiked;
                        if (comment.IsLiked == true)
                            comment.LikeCount = comment.LikeCount + 1;
                        else
                            comment.LikeCount = comment.LikeCount - 1 < 0 ? 0 : comment.LikeCount - 1; // nếu <0 thì gán = 0

                        conn.Update(comment);

                        InvokeOnMainThread(() =>
                        {
                            parentView.HandleBtnLike(sectionIndex, comment);
                        });


                    }
                });
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                iv_avatar.Frame = new CGRect(55, 10, 40, 40);
                lbl_imgCover.Frame = new CGRect(55, 10, 40, 40);
                lbl_title.Frame = new CGRect(lbl_imgCover.Frame.Right + 10, 10, ((ContentView.Frame.Width - iv_avatar.Frame.Right) / 3) * 2.3f, 20);
                lbl_createdDate.Frame = new CGRect(ContentView.Frame.Width - 100, 10, 90, 20);
                lbl_subTitle.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom, 270, 20);
                iv_attach.Frame = new CGRect(ContentView.Frame.Width - 90, lbl_subTitle.Frame.Y, 14, 14);
                lbl_attach_count.Frame = new CGRect(iv_attach.Frame.Right + 5, lbl_subTitle.Frame.Y, 20, 20);
                iv_like.Frame = new CGRect(lbl_attach_count.Frame.Right + 5, lbl_subTitle.Frame.Y, 14, 14);
                lbl_like_count.Frame = new CGRect(iv_like.Frame.Right + 5, lbl_subTitle.Frame.Y, 30, 20);
                tv_comment.Frame = new CGRect(lbl_title.Frame.X, lbl_subTitle.Frame.Bottom, ContentView.Frame.Width - (lbl_title.Frame.X * 2), 25);

                if (lst_attachFiles != null)
                {
                    float height = 0;
                    foreach (var item in lst_attachFiles)
                    {
                        string fileExt = string.Empty;
                        if (!string.IsNullOrEmpty(item.Url))
                            fileExt = item.Url.Split('.').Last();



                        bool isThumb = CmmFunction.CheckIsImageAttachmentType(fileExt);

                        if (isThumb)
                            height += 200;
                        else
                            height += 40;
                    }
                    table_attachment.Frame = new CGRect(lbl_title.Frame.X, tv_comment.Frame.Bottom, tv_comment.Frame.Width, height);
                }
                else
                    table_attachment.Frame = new CGRect(lbl_title.Frame.X, tv_comment.Frame.Bottom, tv_comment.Frame.Width, 0);

                BT_like.Frame = new CGRect(lbl_title.Frame.X, table_attachment.Frame.Bottom + 5, 60, 20);
                BT_reply.Frame = new CGRect(BT_like.Frame.Right + 15, table_attachment.Frame.Bottom + 5, 60, 20);
            }

            private async void checkFileLocalIsExist(BeanUser contact, UILabel label_cover, UIImageView image_view)
            {
                try
                {
                    string filename = contact.ImagePath.Split('/').Last();
                    string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + contact.ImagePath;
                    string localfilePath = Path.Combine(localDocumentFilepath, filename);

                    if (!File.Exists(localfilePath))
                    {
                        UIImage avatar = null;
                        await Task.Run(() =>
                        {
                            ProviderBase provider = new ProviderBase();
                            if (provider.DownloadFile(filepathURL, localfilePath, CmmVariable.M_AuthenticatedHttpClient))
                            {
                                NSData data = NSData.FromUrl(new NSUrl(localfilePath, false));

                                InvokeOnMainThread(() =>
                                {
                                    if (data != null)
                                    {
                                        UIImage image = UIImage.LoadFromData(data);
                                        if (image != null)
                                        {
                                            avatar = CmmIOSFunction.MaxResizeImage(image, 200, 200);
                                            image_view.Image = avatar;
                                        }
                                        else
                                            image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                                    }
                                    else
                                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");

                                    image_view.Hidden = false;

                                    //kiem tra xong cap nhat lai avatar
                                    label_cover.Hidden = true;
                                    image_view.Hidden = false;
                                });

                                if (data != null && avatar != null)
                                {
                                    NSError err = null;
                                    NSData imgData = avatar.AsPNG();
                                    if (imgData.Save(localfilePath, false, out err))
                                        Console.WriteLine("saved as " + localfilePath);
                                    return;
                                }
                            }
                            else
                            {
                                InvokeOnMainThread(() =>
                                {
                                    image_view.Hidden = true;
                                    label_cover.Hidden = false;
                                });
                            }
                        });
                    }
                    else
                    {
                        openFile(filename, image_view);
                        image_view.Hidden = false;
                        label_cover.Hidden = true;
                    }
                }
                catch (Exception ex)
                {
                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                    Console.WriteLine("ListUserView - checkFileLocalIsExist - Err: " + ex.ToString());
                    //CmmIOSFunction.IOSlog(null, "PopupContactDetailView - loadAvatar - " + ex.ToString());
                }
            }

            private async void openFile(string localfilename, UIImageView image_view)
            {
                try
                {
                    NSData data = null;
                    await Task.Run(() =>
                    {
                        string localfilePath = Path.Combine(localDocumentFilepath, localfilename);
                        data = NSData.FromUrl(new NSUrl(localfilePath, false));
                    });

                    if (data != null)
                    {
                        UIImage image = UIImage.LoadFromData(data);
                        if (image != null)
                        {
                            image_view.Image = image;
                        }
                        else
                        {
                            image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                        }
                    }
                    else
                    {
                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("ContactView - openFile - Err: " + ex.ToString());
                }

            }
        }
        #endregion

        #region collection thumb attachment
        public class CollectionAttachmentThumb_Source : UICollectionViewSource
        {
            ControlInputComments parentView { get; set; }
            public static Dictionary<string, List<BeanWorkflow>> dict_groupWorkFlow;
            public List<BeanAttachFile> lst_thumbAttach;

            public CollectionAttachmentThumb_Source(ControlInputComments _parentview, List<BeanAttachFile> _lst_thumbAttach)
            {
                parentView = _parentview;
                lst_thumbAttach = _lst_thumbAttach;


            }

            public void LoadData()
            {

            }

            public override nint NumberOfSections(UICollectionView collectionView)
            {
                return 1;
            }

            public override nint GetItemsCount(UICollectionView collectionView, nint section)
            {
                return lst_thumbAttach.Count;
            }

            public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
            {
                return true;
            }

            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {

            }

            public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
            {
                BeanAttachFile thumb = lst_thumbAttach[indexPath.Row];
                var cell = (AttachThumb_CollectionCell)collectionView.DequeueReusableCell(AttachThumb_CollectionCell.CellID, indexPath);
                cell.UpdateRow(thumb, parentView);
                return cell;
            }
        }

        private class CustomFlowLayoutDelegate : UICollectionViewDelegateFlowLayout
        {
            static ControlInputComments parentView;
            CollectionAttachmentThumb_Source collect_source;
            UICollectionView CollectionView;

            #region Constructors
            public CustomFlowLayoutDelegate(ControlInputComments _parent, CollectionAttachmentThumb_Source _collect_source, UICollectionView collectionView)
            {
                collect_source = _collect_source;
                CollectionView = collectionView;
                parentView = _parent;
            }
            #endregion

            #region Override Methods
            public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
            {
                return new CGSize(210, 150);
            }

            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                var item = collect_source.lst_thumbAttach[indexPath.Row];
                parentView.HandleSeclectItem(item);
            }
            #endregion
        }
        #endregion

        #region add attachment source table
        private class Attachment_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellAttachmentID");
            ControlInputComments parentView;
            List<BeanAttachFile> lst_attachment = new List<BeanAttachFile>();
            List<string> sectionKeys;

            public Attachment_TableSource(List<BeanAttachFile> _lst_attachment, ControlInputComments _parentview)
            {
                lst_attachment = _lst_attachment;
                parentView = _parentview;
                LoadData();
            }

            private void LoadData()
            {

            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 40;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_attachment.Count;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var itemSelected = lst_attachment[indexPath.Row];
                parentView.HandleSeclectItem(itemSelected);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var attachment = lst_attachment[indexPath.Row];

                Attachment_cell_custom cell = new Attachment_cell_custom(parentView, cellIdentifier, attachment, indexPath);
                return cell;
            }
        }
        private class Attachment_cell_custom : UITableViewCell
        {
            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            ControlInputComments parentView { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            BeanAttachFile attachment { get; set; }
            UILabel lbl_title;
            UIButton BT_delete;

            public Attachment_cell_custom(ControlInputComments _parentView, NSString cellID, BeanAttachFile _attachment, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
            {
                parentView = _parentView;
                attachment = _attachment;
                currentIndexPath = _currentIndexPath;
                Accessory = UITableViewCellAccessory.None;

                viewConfiguration();
                UpdateCell();
            }

            private void viewConfiguration()
            {
                lbl_title = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.FromRGB(59, 95, 179),
                    TextAlignment = UITextAlignment.Left,
                };

                BT_delete = new UIButton();
                BT_delete.SetImage(UIImage.FromFile("Icons/icon_close_circle_red.png"), UIControlState.Normal);
                BT_delete.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
                BT_delete.TouchUpInside += delegate
                {
                    parentView.HandleRemoveDocItem(attachment);
                };

                ContentView.AddSubviews(new UIView[] { lbl_title, BT_delete });
            }

            public void UpdateCell()
            {
                try
                {
                    //title
                    if (attachment.Title.Contains(";#"))
                        lbl_title.Text = attachment.Title.Split(";#")[0];
                    else
                        lbl_title.Text = attachment.Title;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("attachment_cell_custom - UpdateCell - ERR: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                lbl_title.Frame = new CGRect(20, 5, ContentView.Frame.Width - 40, 30);
                BT_delete.Frame = new CGRect(ContentView.Frame.Width - 30, 5, 30, 30);
            }
        }
        #endregion

        #region comment attachment
        public class CommentAttachment_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellAttachmentThumbID");
            ControlInputComments parentView;
            List<BeanAttachFile> lst_attachment = new List<BeanAttachFile>();
            List<string> sectionKeys;

            public CommentAttachment_TableSource(List<BeanAttachFile> _lst_attachment, ControlInputComments _parentview)
            {
                lst_attachment = _lst_attachment;
                parentView = _parentview;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_attachment.Count;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                string fileExt = string.Empty;
                if (!string.IsNullOrEmpty(lst_attachment[indexPath.Row].Url))
                    fileExt = lst_attachment[indexPath.Row].Url.Split('.').Last();

                bool isThumb = CmmFunction.CheckIsImageAttachmentType(fileExt);

                if (isThumb)
                    return 210;
                else
                    return 40;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var itemSelected = lst_attachment[indexPath.Row];
                parentView.HandleSeclectItem(itemSelected);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var attachment = lst_attachment[indexPath.Row];
                Custom_attachFileThumb cell = new Custom_attachFileThumb(parentView, cellIdentifier, attachment, indexPath);
                cell.UpdateCell();
                return cell;
            }
        }
        #endregion

        #endregion
    }
}