using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobileV1.iOS.Components;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using Foundation;
using MobileCoreServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace BPMOPMobileV1.iOS
{
    public partial class FormCommentView : UIViewController
    {
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        BeanWorkflowItem workflowItem;
        BeanComment beanComment;
        List<BeanComment> lst_Comment;
        bool isLoadData = false;
        UIViewController parent { get; set; }
        CmmLoading loading;
        string str_json_FormDefineInfo;
        string WorkflowActionID;
        public nfloat estRowHeight;
        nfloat constraintHeightView_noteDefault;
        //Attachments
        int numRowAttachmentFile = 0;
        List<BeanAttachFile> lst_addAttachFile = new List<BeanAttachFile>();
        List<BeanAttachFile> lst_removeAttachFile = new List<BeanAttachFile>();

        List<BeanAttachFile> lst_addAttachImg = new List<BeanAttachFile>();
        List<BeanAttachFile> lst_removeAttachhImg = new List<BeanAttachFile>();

        UIImagePickerController imagePicker;
        UIDocumentPickerViewController docPicker;
        NSIndexPath indexSelected;
        string json_attachRemove;
        string otherResourceId;
        nfloat heightTxtNote;
        nfloat heightTxtNotOneRow;
        nfloat heightViewNote;
        NSLayoutConstraint constraintHeightCollection_attach, constraintHeightTable_attachment;
        string hintDefault = CmmFunction.GetTitle("TEXT_HINT_REQUIRE_COMMENT", "Vui lòng nhập bình luận tại đây …");
        Custom_AttachFileView attachFileView;
        private UITapGestureRecognizer gestureRecognizer;

        public FormCommentView(IntPtr handle) : base(handle)
        {
        }

        #region override
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            gestureRecognizer = new UITapGestureRecognizer(Self, new ObjCRuntime.Selector("hideKeyboard"));
            gestureRecognizer.ShouldReceiveTouch += delegate (UIGestureRecognizer recognizer, UITouch touch)
            {
                var name = touch.View.Class.Name;
                var touchName = touch.View.Superview.Superview.Class.Name;

                if (name == "UITableViewCellContentView")
                    return false;
                else
                    return true;
            };
            this.view_top.AddGestureRecognizer(gestureRecognizer);

            Constraint();
            ViewConfiguration();
            SetLangTitle();
            LoadContent();

            txt_note.BecomeFirstResponder();
            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_addAttach.TouchUpInside += BT_addAttach_TouchUpInside;
            BT_sendComment.TouchUpInside += BT_sendComment_TouchUpInside;
            txt_note.Changed += Txt_note_Changed;
            txt_note.Started += delegate
            {
                if (txt_note.Text != hintDefault)
                {
                    txt_note.Font = UIFont.FromName("ArialMT", 15f);
                    txt_note.TextColor = UIColor.Black;
                }
                else
                {
                    txt_note.Font = UIFont.FromName("Arial-ItalicMT", 15f);
                    txt_note.TextColor = UIColor.FromRGB(153, 153, 153);
                    txt_note.Text = hintDefault;
                }
            };

            txt_note.Ended += delegate
            {
                if (string.IsNullOrEmpty(txt_note.Text))
                {
                    txt_note.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                    txt_note.TextColor = UIColor.LightGray;
                    txt_note.Text = hintDefault;
                }
            };

            #endregion
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _willResignActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyBoardUpNotification);
            _didBecomeActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            //CmmIOSFunction.AddShadowForTopORBotBar(view_note, false);

        }
        #endregion

        #region constraint
        private void Constraint()
        {

            collection_attach.TranslatesAutoresizingMaskIntoConstraints = false;
            collection_attach.LeadingAnchor.ConstraintEqualTo(view_note.LeadingAnchor, 10).Active = true;
            collection_attach.TopAnchor.ConstraintEqualTo(txt_note.BottomAnchor, 5).Active = true;
            collection_attach.TrailingAnchor.ConstraintEqualTo(view_note.TrailingAnchor, -10).Active = true;
            constraintHeightCollection_attach = collection_attach.HeightAnchor.ConstraintEqualTo(0);
            constraintHeightCollection_attach.Active = true;

            table_attachment.TranslatesAutoresizingMaskIntoConstraints = false;
            table_attachment.LeadingAnchor.ConstraintEqualTo(view_note.LeadingAnchor, 10).Active = true;
            table_attachment.TopAnchor.ConstraintEqualTo(collection_attach.BottomAnchor, 5).Active = true;
            table_attachment.TrailingAnchor.ConstraintEqualTo(view_note.TrailingAnchor, -10).Active = true;
            constraintHeightTable_attachment = table_attachment.HeightAnchor.ConstraintEqualTo(0);
            constraintHeightTable_attachment.Active = true;

        }

        #endregion
        public void HandleSeclectItem(BeanAttachFile _attachment)
        {
            if (parent != null && parent.GetType() == typeof(RequestDetailsV2))
            {
                //currentAttachment = _attachment;
                if (parent.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 requestDetailsV2 = (RequestDetailsV2)parent;
                    requestDetailsV2.NavigateToAttachView(_attachment);
                }
            }
            if (parent != null && parent.GetType() == typeof(FormTaskDetails))
            {
                //currentAttachment = _attachment;
                if (parent.GetType() == typeof(FormTaskDetails))
                {
                    FormTaskDetails formTaskDetails = (FormTaskDetails)parent;
                    formTaskDetails.NavigateToShowAttachView(_attachment);
                }
            }
        }

        #region private - public method
        public void SetContent_BK(UIViewController _parent, BeanWorkflowItem _workflowItem, string _str_json_FormDefineInfo, List<BeanComment> _lstComment, string _otherResourceId)
        {
            workflowItem = _workflowItem;
            parent = _parent;
            str_json_FormDefineInfo = _str_json_FormDefineInfo;
            lst_Comment = _lstComment;
            otherResourceId = _otherResourceId;
        }

        public void SetContent(UIViewController _parent, BeanWorkflowItem _workflowItem, BeanComment _beanComment, string _otherResourceId, NSIndexPath nSIndexPath)
        {
            beanComment = _beanComment;
            workflowItem = _workflowItem;
            parent = _parent;
            otherResourceId = _otherResourceId;
            indexSelected = nSIndexPath;
        }

        private void ViewConfiguration()
        {
            heightTxtNotOneRow = "a".StringRect(UIFont.FromName("ArialMT", 14f), txt_note.Frame.Width).Height;
            heightTxtNote = txt_note.Frame.Height;
            heightViewNote = view_note.Frame.Height;

            BT_close.ContentEdgeInsets = new UIEdgeInsets(5, 7, 5, 7);
            BT_sendComment.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_addAttach.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            view_note.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_note.Layer.BorderWidth = 1f;
            view_note.Layer.CornerRadius = 4;
            table_attachment.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            table_attachment.ShowsHorizontalScrollIndicator = false;
            table_attachment.ShowsVerticalScrollIndicator = false;
            table_attachment.ScrollEnabled = false;

            collection_attach.RegisterClassForCell(typeof(AttachThumb_CollectionCell), AttachThumb_CollectionCell.CellID);
            collection_attach.BackgroundColor = UIColor.White;

            txt_note.Text = hintDefault;
            txt_note.TextColor = UIColor.LightGray;
            table_comments.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            //table_comments.ScrollEnabled = false;
        }

        private void SetLangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }
        private void LoadContent()
        {
            if (parent != null && parent.GetType() == typeof(RequestDetailsV2))
                LoadContentOffline();
            else if (parent != null && parent.GetType() == typeof(FormTaskDetails))
                LoadContentOnline();
        }
        [Export("hideKeyboard")]
        private void hideKeyboard()
        {
            this.View.EndEditing(true);
        }

        private void LoadContentOffline()
        {

            try
            {
                //sectionKeys = lst_comments.Where(x => string.IsNullOrEmpty(x.ParentCommentId)).ToList();
                //sectionKeys = sectionKeys.OrderByDescending(x => x.Created).ToList();

                SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                if (!string.IsNullOrEmpty(beanComment.ParentCommentId))
                {
                    string _PqueryComment = string.Format(@"SELECT * FROM BeanComment WHERE ID = ?");
                    var lst_Parentcomment = conn.Query<BeanComment>(_PqueryComment, beanComment.ParentCommentId); // List Local

                    string _queryComment = string.Format(@"SELECT * FROM BeanComment WHERE ParentCommentId = ?");
                    lst_Comment = conn.Query<BeanComment>(_queryComment, lst_Parentcomment[0].ID); // List Local

                    lst_Comment.Add(lst_Parentcomment[0]);
                    if (lst_Comment != null && lst_Comment.Count > 0)
                    {
                        table_comments.Source = new Comment_TableSource(lst_Comment, this);
                        table_comments.ReloadData();
                    }

                    BeanUser user = CmmFunction.GetBeanUserByID(beanComment.Author);

                    //txt_note.Text = user.FullName + '-';
                    var str_transalte = user.FullName + "  ";
                    var indexA = str_transalte.IndexOf("  ");
                    NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);

                    att.AddAttribute(UIStringAttributeKey.BackgroundColor, UIColor.FromRGB(205, 227, 255), new NSRange(0, indexA));
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.Black, new NSRange(indexA, str_transalte.Length - indexA));

                    txt_note.AttributedText = att;

                }
                else
                {
                    string _queryComment = string.Format(@"SELECT * FROM BeanComment WHERE ParentCommentId = ?");
                    lst_Comment = conn.Query<BeanComment>(_queryComment, beanComment.ID); // List Local

                    lst_Comment.Add(beanComment);
                    if (lst_Comment != null && lst_Comment.Count > 0)
                    {
                        table_comments.Source = new Comment_TableSource(lst_Comment, this);
                        table_comments.ReloadData();
                    }

                    BeanUser user = CmmFunction.GetBeanUserByID(beanComment.Author);

                    //txt_note.Text = user.FullName + '-';
                    var str_transalte = user.FullName + "  ";
                    var indexA = str_transalte.IndexOf("  ");
                    NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);

                    att.AddAttribute(UIStringAttributeKey.BackgroundColor, UIColor.FromRGB(205, 227, 255), new NSRange(0, indexA));
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.Black, new NSRange(indexA, str_transalte.Length - indexA));

                    txt_note.AttributedText = att;
                }

                //table_comments.SelectRow(indexSelected, true, UITableViewScrollPosition.Top);
                table_comments.ScrollToRow(indexSelected, UITableViewScrollPosition.Bottom, true);

            }
            catch (Exception ex)
            {
                Console.WriteLine("FormCommentView - LoadContentOffline - Err: " + ex.ToString());
            }
        }

        private void LoadContentOnline()
        {

            try
            {
                //sectionKeys = lst_comments.Where(x => string.IsNullOrEmpty(x.ParentCommentId)).ToList();
                //sectionKeys = sectionKeys.OrderByDescending(x => x.Created).ToList();
                if (parent.GetType() == typeof(FormTaskDetails))
                {
                    FormTaskDetails formTaskDetails = parent as FormTaskDetails;
                    SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                    if (!string.IsNullOrEmpty(beanComment.ParentCommentId))
                    {
                        var lst_Parentcomment = formTaskDetails.lst_comment.Find(s => s.ID == beanComment.ParentCommentId);

                        lst_Comment = formTaskDetails.lst_comment.FindAll(s => s.ParentCommentId == lst_Parentcomment.ID);

                        lst_Comment.Add(lst_Parentcomment);
                        if (lst_Comment != null && lst_Comment.Count > 0)
                        {
                            table_comments.Source = new Comment_TableSource(lst_Comment, this);
                            table_comments.ReloadData();
                        }

                        BeanUser user = CmmFunction.GetBeanUserByID(beanComment.Author);

                        //txt_note.Text = user.FullName + '-';
                        var str_transalte = user.FullName + "  ";
                        var indexA = str_transalte.IndexOf("  ");
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);

                        att.AddAttribute(UIStringAttributeKey.BackgroundColor, UIColor.FromRGB(205, 227, 255), new NSRange(0, indexA));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.Black, new NSRange(indexA, str_transalte.Length - indexA));
                        txt_note.AttributedText = att;

                    }
                    else
                    {
                        lst_Comment = formTaskDetails.lst_comment.FindAll(s => s.ID == beanComment.ParentCommentId);

                        lst_Comment.Add(beanComment);
                        if (lst_Comment != null && lst_Comment.Count > 0)
                        {
                            table_comments.Source = new Comment_TableSource(lst_Comment, this);
                            table_comments.ReloadData();
                        }

                        BeanUser user = CmmFunction.GetBeanUserByID(beanComment.Author);

                        //txt_note.Text = user.FullName + '-';
                        var str_transalte = user.FullName + "  ";
                        var indexA = str_transalte.IndexOf("  ");
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);

                        att.AddAttribute(UIStringAttributeKey.BackgroundColor, UIColor.FromRGB(205, 227, 255), new NSRange(0, indexA));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.Black, new NSRange(indexA, str_transalte.Length - indexA));

                        txt_note.AttributedText = att;
                    }

                    //table_comments.SelectRow(indexSelected, true, UITableViewScrollPosition.Top);
                    table_comments.ScrollToRow(indexSelected, UITableViewScrollPosition.Bottom, true);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("FormCommentView - LoadContentOnline - Err: " + ex.ToString());
            }
        }

        private void ReplyComment()
        {

            BeanUser user = CmmFunction.GetBeanUserByID(beanComment.Author);

            //txt_note.Text = user.FullName + '-';
            var str_transalte = user.FullName + "  ";
            var indexA = str_transalte.IndexOf("  ");
            NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);

            att.AddAttribute(UIStringAttributeKey.BackgroundColor, UIColor.FromRGB(205, 227, 255), new NSRange(0, indexA));
            att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.Black, new NSRange(indexA, str_transalte.Length - indexA));

            txt_note.AttributedText = att;
        }

        public void UpdateTableSections(int sectionIndex, BeanComment comment)
        {
            var item = lst_Comment.Where(i => i.ID == comment.ID).FirstOrDefault();
            item = comment;
            table_comments.ReloadSections(NSIndexSet.FromIndex(sectionIndex), UITableViewRowAnimation.Automatic);
            //table_comments.Source = new Comment_TableSource(lst_Comment, this);
            table_comments.ReloadData();
        }
        private void LoadAttachments(bool isRemove)
        {
            try
            {
                var tableHeight = constraintHeightTable_attachment.Constant;
                if (lst_addAttachFile.Count > 3)
                    table_attachment.ScrollEnabled = true;
                else
                {
                    table_attachment.ScrollEnabled = false;
                    if (isRemove)
                    {
                        heightViewNote -= 35;
                        constraintHeightView_note.Constant -= 35;
                        tableHeight = lst_addAttachFile.Count * 35;
                    }
                    else
                    {
                        heightViewNote += 35;
                        constraintHeightView_note.Constant += 35;
                        tableHeight = lst_addAttachFile.Count * 35;
                    }
                }

                constraintHeightTable_attachment.Constant = tableHeight;
                table_attachment.Source = new Attachment_TableSource(lst_addAttachFile, this);
                table_attachment.ReloadData();
                HandleAttachFileClose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FormCommentView - LoadAttachments - Err: " + ex.ToString());
            }
        }
        private void LoadAttachmentsIMG(bool isRemove)
        {
            try
            {

                if (isRemove)
                {
                    if (lst_addAttachImg == null || lst_addAttachImg.Count == 0)
                    {
                        heightViewNote -= 55;
                        constraintHeightView_note.Constant -= 55;
                        constraintHeightCollection_attach.Constant = 0;
                    }
                }
                else
                {
                    if (lst_addAttachImg != null && lst_addAttachImg.Count == 1)
                    {
                        heightViewNote += 55;
                        constraintHeightView_note.Constant += 55;
                        constraintHeightCollection_attach.Constant = 55;
                    }
                }


                var flowLayout = new UICollectionViewFlowLayout()
                {
                    SectionInset = new UIEdgeInsets(2, 2, 2, 2),
                    ScrollDirection = UICollectionViewScrollDirection.Horizontal,
                    MinimumInteritemSpacing = 5, // minimum spacing between cells
                    MinimumLineSpacing = 10 // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
                };
                collection_attach.CollectionViewLayout = flowLayout;
                view_note.Frame = new CGRect(view_note.Frame.X, view_note.Frame.Y, view_note.Frame.Width, collection_attach.Frame.Bottom + 50);
                CollectionAttachmentThumb_Source collectionAttachmentThumb_Source = new CollectionAttachmentThumb_Source(this, lst_addAttachImg);
                collection_attach.Source = collectionAttachmentThumb_Source;
                collection_attach.Delegate = new CustomFlowLayoutDelegate(this, collectionAttachmentThumb_Source, collection_attach);
                collection_attach.ReloadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FormCommentView - LoadAttachmentsIMG - Err: " + ex.ToString());
            }
        }
        public async void SubmitLikeAction(int sectionIndex, BeanComment comment)
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
                        UpdateTableSections(sectionIndex, comment);
                    });
                }
            });
        }
        public async void SubmitComment()
        {
            try
            {
                UIActivityIndicatorView acti = new UIActivityIndicatorView();
                string commentvalue = "";
                BeanUser user = CmmFunction.GetBeanUserByID(beanComment.Author);
                if ((!string.IsNullOrEmpty(txt_note.Text) && txt_note.Text != hintDefault && txt_note.Text != user.FullName.Trim())
                        || (lst_addAttachImg != null && lst_addAttachImg.Count > 0)
                        || (lst_addAttachFile != null && lst_addAttachFile.Count > 0))
                {
                    commentvalue = txt_note.Text.Remove(0, user.FullName.Length).TrimStart();
                    acti.StartAnimating();
                    acti.Frame = BT_sendComment.Frame;
                    acti.TintColor = UIColor.LightGray;
                    view_top.AddSubview(acti);
                    BT_sendComment.Hidden = true;
                    BT_sendComment.UserInteractionEnabled = false;
                }
                else
                {
                    CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến."));
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
                if (lst_addAttachImg != null && lst_addAttachImg.Count > 0) // Lấy những file img thêm mới từ App ra
                {
                    foreach (var item in lst_addAttachImg)
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
                    if (parent.GetType() == typeof(RequestDetailsV2))
                        beanOtherResource.ResourceCategoryId = (int)CmmFunction.CommentResourceCategoryID.WorkflowItem;
                    else if (parent.GetType() == typeof(FormTaskDetails))
                        beanOtherResource.ResourceCategoryId = (int)CmmFunction.CommentResourceCategoryID.Task;
                    beanOtherResource.ResourceSubCategoryId = 0;
                    beanOtherResource.Image = "";
                    beanOtherResource.ParentCommentId = beanComment.ParentCommentId != null ? beanComment.ParentCommentId : beanComment.ID;

                    bool _result = p_dynamic.AddComment(beanOtherResource, _lstKeyVarAttachmentLocal);

                    if (_result)
                    {
                        ProviderBase p_base = new ProviderBase();
                        p_base.UpdateMasterData<BeanWorkflowItem>(null, true, 1, false);
                    }

                    InvokeOnMainThread(() =>
                    {
                        if (_result)
                        {
                            if (parent.GetType() == typeof(RequestDetailsV2))
                            {
                                RequestDetailsV2 requestDetailsV2 = parent as RequestDetailsV2;
                                requestDetailsV2.ReLoadDataFromServer();
                                this.NavigationController.PopViewController(true);
                            }
                            if (parent.GetType() == typeof(FormTaskDetails))
                            {
                                FormTaskDetails formTaskDetails = parent as FormTaskDetails;
                                formTaskDetails.ReLoadDataFromServer();
                                this.NavigationController.PopViewController(true);
                            }
                        }
                        else
                        {
                            CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"));
                        }
                        acti.StopAnimating();
                        acti.Hidden = true;
                        BT_sendComment.Hidden = false;
                        BT_sendComment.UserInteractionEnabled = true;
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("FormCommentView - SubmitComment - Err: " + ex.ToString());
            }

        }

        #region attachment
        public void HandleAttachFileClose()
        {
            Custom_AttachFileView custom_AttachFileView = Custom_AttachFileView.Instance;
            if (custom_AttachFileView.Superview != null)
                custom_AttachFileView.RemoveFromSuperview();
        }
        public void HandleAddAttachFileResult(BeanAttachFileLocal _attachFile)
        {
            //lst_addAttachment = new List<BeanAttachFile>();

            //numRowAttachmentFile++;


            string custID = numRowAttachmentFile + "";
            BeanAttachFile attachFile = new BeanAttachFile()
            {
                ID = "",
                Title = _attachFile.Name + ";#" + DateTime.Now.ToShortTimeString(),
                Path = _attachFile.Path,
                Size = _attachFile.Size,
                IsImage = _attachFile.IsImage,
                Category = "",
                IsAuthor = true,
                CreatedBy = CmmVariable.SysConfig.UserId,
                CreatedName = CmmVariable.SysConfig.DisplayName,
                CreatedPositon = CmmVariable.SysConfig.PositionTitle,
                AttachTypeId = null,
                AttachTypeName = "",
                //WorkflowId = currentItemSelected.WorkflowID,
                //WorkflowItemId = int.Parse(currentItemSelected.ID)
            };
            if (_attachFile.IsImage)
            {
                lst_addAttachImg.Add(attachFile);
                LoadAttachmentsIMG(false);
            }
            else
            {
                lst_addAttachFile.Add(attachFile);
                LoadAttachments(false);
            }


            //lst_addAttachFile.Add(attachFile);

        }
        public void NavigationToDocumentPicker()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                var allowedUTIs = new string[] {
                            //UTType.UTF8PlainText,
                            //UTType.PlainText,
                            UTType.RTF,
                            UTType.PNG,
                            UTType.Text,
                            UTType.PDF,
                            UTType.Image,
                            //UTType.Data,
                            UTType.Content,
                            new NSString("com.microsoft.word.doc")
                        };

                docPicker = new UIDocumentPickerViewController(allowedUTIs, UIDocumentPickerMode.Import);
                docPicker.WasCancelled += (s, wasCancelledArgs) =>
                {
                };

                docPicker.DidPickDocumentAtUrls += (object s, UIDocumentPickedAtUrlsEventArgs ev) =>
                {
                    try
                    {
                        string filePath = ev.Urls[0].Path;
                        string fileName = ev.Urls[0].LastPathComponent;

                        var index = fileName.LastIndexOf('.');
                        var type = fileName.Substring((index + 1), fileName.Length - (index + 1));

                        string[] arrType = new string[] { "doc", "docx", "xls", "xlsx", "pdf", "png", "jpeg", "jpg" };

                        if (arrType.Contains(type.ToLower()))
                        {
                            var FileManager = new NSFileManager();
                            var size = (Int64)FileManager.Contents(filePath).Length;

                            BeanAttachFileLocal itemiCloudAndDevice = new BeanAttachFileLocal() { ID = -1, Name = fileName, Path = filePath, Size = size, Type = type };
                            //HandleAddAttachFileResult(itemiCloudAndDevice, addAttachmentsView);
                            HandleAddAttachFileResult(itemiCloudAndDevice);
                        }
                        else
                        {
                            CmmIOSFunction.AlertUnsupportFile(this);
                            Console.WriteLine("Selected file type: " + type);
                        }
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Console.WriteLine("DidPickDocumentAtUrls - err - :" + ex.ToString());
#endif
                    }
                };

                docPicker.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                PresentViewController(docPicker, true, null);
            }
            else
                CmmIOSFunction.commonAlertMessage(this, "Thông báo", "Chỉ hỗ trợ thêm tập tin office đính kèm từ hệ điều hành 11 trở lên.");
        }
        public void NavigationToImagePicker()
        {
            if (imagePicker != null)
            {
                imagePicker.Dispose();
                imagePicker = null;
            }

            imagePicker = new UIImagePickerController();

            imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            //imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
            imagePicker.MediaTypes = new string[] { UTType.Image };
            imagePicker.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            imagePicker.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;

            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
            imagePicker.Canceled += Handle_Canceled;

            this.PresentModalViewController(imagePicker, true);

        }

        public void NavigationToCameraPicker()
        {
            if (imagePicker != null)
            {
                imagePicker.Dispose();
                imagePicker = null;
            }

            imagePicker = new UIImagePickerController();

            imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
            //imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.Camera);
            imagePicker.MediaTypes = new string[] { UTType.Image };
            imagePicker.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            imagePicker.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;

            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
            imagePicker.Canceled += Handle_Canceled;

            this.PresentViewController(imagePicker, true, null);
        }

        protected void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            bool isImage = false;
            var mediaType = e.Info[UIImagePickerController.MediaType].ToString();
            switch (mediaType)
            {
                case "public.image":
                    Console.WriteLine("Image selected");
                    isImage = true;
                    break;
                case "public.video":
                case "public.movie":
                    Console.WriteLine("Video or Movie selected");
                    break;
                default:
                    Console.WriteLine("Selected media type: " + mediaType);
                    break;
            }

            // get common info (shared between images and video)
            NSUrl filePath = e.Info[new NSString("UIImagePickerControllerImageURL")] as NSUrl;

            // if it was an image, get the other image info
            if (isImage)
            {
                // get the original image
                UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
                if (originalImage != null)
                {
                    if (filePath != null)
                    {
                        string[] fileName = filePath.ToString().Split("/");
                        var FileManager = new NSFileManager();
                        var size = (Int64)FileManager.Contents(filePath.Path).Length;

                        BeanAttachFileLocal itemiCloudAndDevice = new BeanAttachFileLocal() { ID = -1, Name = fileName[fileName.Length - 1], Path = filePath.Path, Size = size, IsImage = true };
                        HandleAddAttachFileResult(itemiCloudAndDevice);
                    }
                    else
                    {
                        string fileName = "IMG_" + DateTime.Now.ToString("MMss") + ".JPG";
                        //CollectFileInfo(fileName, originalImage);

                        NSError err = null;

                        var imageFormat = CmmIOSFunction.RotateCameraImageToProperOrientation(originalImage, 1024);

                        var documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        string img_Path = Path.Combine(documentFolder, fileName);
                        NSData imgData = imageFormat.AsJPEG();
                        if (imgData.Save(img_Path, false, out err))
                            Console.WriteLine("saved as " + img_Path);

                        var fileNameCust = fileName.Substring(fileName.LastIndexOf('/') + 1);
                        var FileManager = new NSFileManager();
                        var size = (Int64)FileManager.Contents(img_Path).Length;

                        BeanAttachFileLocal itemiCloudAndDevice = new BeanAttachFileLocal() { ID = -1, Name = fileName, Path = img_Path, Size = size, IsImage = true };
                        HandleAddAttachFileResult(itemiCloudAndDevice);
                    }
                }
                else
                {
                    CmmIOSFunction.AlertUnsupportFile(this);
                }
            }

            // dismiss the picker
            imagePicker.DismissModalViewController(false);
            var vc = this.PresentedViewController;
            vc.DismissViewController(true, null);

            HandleAttachFileClose();

        }

        private void Handle_Canceled(object sender, EventArgs e)
        {
            imagePicker.DismissModalViewController(true);
        }
        public void HandleAttachmentRemove(BeanAttachFile beanAttachRemove)
        {
            UIAlertController alert = UIAlertController.Create("Thông báo", CmmFunction.GetTitle("TEXT_DELETE_CONFIRM", "Bạn có chắc muốn xóa file này không?"), UIAlertControllerStyle.Alert);//"BPM"
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"), UIAlertActionStyle.Default, null));
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"), UIAlertActionStyle.Default, alertAction =>
            {
                lst_removeAttachFile.Add(beanAttachRemove);
                foreach (var item in lst_removeAttachFile)
                {
                    lst_addAttachFile.Remove(item);
                }
                LoadAttachments(true);
            }));
            this.PresentViewController(alert, true, null);

        }
        public void HandleRemoveThumbItem(BeanAttachFile beanAttachRemove)
        {

            UIAlertController alert = UIAlertController.Create("Thông báo", CmmFunction.GetTitle("TEXT_DELETE_CONFIRM", "Bạn có chắc muốn xóa file này không?"), UIAlertControllerStyle.Alert);//"BPM"
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"), UIAlertActionStyle.Default, null));
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"), UIAlertActionStyle.Default, alertAction =>
            {
                lst_removeAttachhImg.Add(beanAttachRemove);
                foreach (var item in lst_removeAttachhImg)
                {
                    lst_addAttachImg.Remove(item);
                }

                LoadAttachmentsIMG(true);
            }));
            this.PresentViewController(alert, true, null);
        }
        public void HandleAttachmentEdit(ViewElement element, NSIndexPath indexPath, BeanAttachFile _attach)
        {
            FormEditAttachFileView formEditAttach = (FormEditAttachFileView)Storyboard.InstantiateViewController("FormEditAttachFileView");
            formEditAttach.SetContent(this, _attach);
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            formEditAttach.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            formEditAttach.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            formEditAttach.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(formEditAttach, true);
        }
        public void ReloadAttachmentElement(ViewElement _element, BeanAttachFile attachFile)
        {
            //List<BeanAttachFile> lst_attachFile = new List<BeanAttachFile>();
            //if (!string.IsNullOrEmpty(_element.Value))
            //{
            //    JArray json = JArray.Parse(_element.Value);
            //    lst_attachFile = json.ToObject<List<BeanAttachFile>>();
            //}

            //var index = lst_attachFile.FindIndex(item => item.ID == attachFile.ID);
            //if (index != -1)
            //    lst_attachFile[index] = attachFile;

            //var jsonString = JsonConvert.SerializeObject(lst_attachFile);
            //_element.Value = jsonString;

            //table_content.ReloadData();
        }
        public void HandleAddAttachFileClose()
        {
            try
            {
                attachFileView.Frame = new CGRect(0, 0, view_content.Frame.Width, view_content.Frame.Height);

                UIView.BeginAnimations("hide_animationShowTable");
                UIView.SetAnimationDuration(0.3f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                attachFileView.Frame = new CGRect(view_content.Frame.Right, 0, view_content.Frame.Width, view_content.Frame.Height);
                UIView.CommitAnimations();

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("CreateNewTaskView - HandleAddAttachment - Err: " + ex.ToString());
#endif
            }

        }
        #endregion
        #endregion

        #region events
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopViewController(true);
        }
        private void BT_addAttach_TouchUpInside(object sender, EventArgs e)
        {
            try
            {
                this.View.EndEditing(true);
                attachFileView = Custom_AttachFileView.Instance;
                attachFileView.parentview = this;
                attachFileView.InitFrameView(new CGRect(0, 0, view_content.Frame.Width, view_content.Frame.Height));
                attachFileView.TableLoadData();

                view_content.AddSubview(attachFileView);

                attachFileView.Frame = new CGRect(view_content.Frame.Right, 0, view_content.Frame.Width, view_content.Frame.Height);
                UIView.BeginAnimations("show_animationShowTable");
                UIView.SetAnimationDuration(0.3f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                attachFileView.Frame = new CGRect(0, 0, view_content.Frame.Width, view_content.Frame.Height);
                UIView.CommitAnimations();

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("CreateNewTaskView - HandleAddAttachment - Err: " + ex.ToString());
#endif
            }

        }
        private void BT_submit_TouchUpInside(object sender, EventArgs e)
        {
            //SubmitComment();
        }
        private void BT_sendComment_TouchUpInside(object sender, EventArgs e)
        {
            SubmitComment();
        }
        private void Txt_note_Changed(object sender, EventArgs e)
        {
            CGRect rect = txt_note.Text.StringRect(UIFont.FromName("ArialMT", 15f), txt_note.Frame.Width);
            if ((rect.Height / heightTxtNotOneRow) >= 3)// 3 dong tro len
            {
                constraintHeight_txtNote.Constant = heightTxtNotOneRow * 2 + heightTxtNote;
                constraintHeightView_note.Constant = heightTxtNotOneRow * 2 + heightViewNote;
                txt_note.ScrollEnabled = true;
            }
            else if ((rect.Height / heightTxtNotOneRow) >= 2) // 2 dong
            {
                constraintHeight_txtNote.Constant = heightTxtNotOneRow + heightTxtNote;
                constraintHeightView_note.Constant = heightTxtNotOneRow + heightViewNote;
                txt_note.ScrollEnabled = false;

            }
            else  //1 dong thi default
            {
                constraintHeight_txtNote.Constant = heightTxtNote;
                constraintHeightView_note.Constant = heightViewNote;
                txt_note.ScrollEnabled = false;
            }

        }

        private void KeyBoardUpNotification(NSNotification notification)
        {
            try
            {
                if (View.Frame.Y == 0)
                {
                    CGRect keyboardSize = UIKeyboard.BoundsFromNotification(notification);
                    constraintBottomViewNote.Constant = keyboardSize.Height + 3;
                }
            }
            catch (Exception ex)
            { Console.WriteLine("StartView - Err: " + ex.ToString()); }
        }
        private void KeyBoardDownNotification(NSNotification notification)
        {
            try
            {
                constraintBottomViewNote.Constant = 15;
            }
            catch (Exception ex)
            {
                Console.WriteLine("StartView - Err: " + ex.ToString());
            }
        }
        #endregion

        #region custom class

        #region comment source table
        private class Comment_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellShareItemID");
            FormCommentView parentView;
            List<BeanComment> lst_comments { get; set; }
            Dictionary<BeanComment, List<BeanComment>> dict_commentItem = new Dictionary<BeanComment, List<BeanComment>>();
            List<BeanComment> sectionKeys;
            private nfloat table_commentAttachHeaderHeight = 0;

            public Comment_TableSource(List<BeanComment> _lst_comment, FormCommentView _parentview)
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
                if (!string.IsNullOrEmpty(item.Content))
                {
                    CGRect rect = StringExtensions.StringRect(item.Content, UIFont.FromName("ArialMT", 14f), tableView.Frame.Width - 85);
                    if (rect.Height > 0 && rect.Height < 20)
                        rect.Height = 30;
                    heightText = rect.Height + 50;
                }
                else
                    heightText = 60;

                table_commentAttachHeaderHeight = height;
                return height + heightText + 20; // 50: buffer height
                //return height + heightText + 50; // 50: buffer height
            }

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
                        BT_like.Font = UIFont.FromName("ArialMT", 12f);
                        //BT_like.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                        BT_like.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
                        BT_like.SetTitle(CmmFunction.GetTitle("TEXT_LIKE", "Thích"), UIControlState.Normal);
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
                            //NSIndexPath indexpath = NSIndexPath.FromItemSection(0, section);
                            //parentView.SubmitLikeAction((int)indexpath, key);
                            parentView.SubmitLikeAction((int)section, key);
                        };

                        UIButton BT_reply = new UIButton();
                        BT_reply.Font = UIFont.FromName("ArialMT", 11f);
                        //BT_reply.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                        BT_reply.SetTitle(CmmFunction.GetTitle("TEXT_REPLY", "Trả lời"), UIControlState.Normal);
                        BT_reply.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
                        BT_reply.TouchUpInside += delegate
                        {
                            //NSIndexPath indexpath = NSIndexPath.FromItemSection(0, section);
                            //parentView.HandleBtnReply(indexpath, key);
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
                nfloat height = 110;
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
                            height = height + 110;
                        }
                        else
                        {
                            newSortList.Insert(newSortList.Count, attach);
                            height = height + 35;
                        }
                    }

                }
                return height;
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
                tableView.DeselectRow(indexPath, true);
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
            FormCommentView parentView;

            public Custom_CommentItemCell(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
            {
                Accessory = UITableViewCellAccessory.None;
            }

            public void UpdateCell(BeanComment _comment, NSIndexPath _indexpath, FormCommentView _parentview)
            {
                commentItem = _comment;
                indexpath = _indexpath;
                parentView = _parentview;
                ViewConfiguration();
                LoadData();
            }

            private void ViewConfiguration()
            {

                iv_avatar = new UIImageView();
                iv_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;
                iv_avatar.ClipsToBounds = true;
                iv_avatar.Layer.CornerRadius = 18;
                iv_avatar.Hidden = true;

                lbl_imgCover = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = UIFont.SystemFontOfSize(14, UIFontWeight.Bold),
                    TextColor = UIColor.White
                };
                lbl_imgCover.Layer.CornerRadius = 18;
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
                    Editable = false,
                    TextAlignment = UITextAlignment.Left
                };

                table_attachment = new UITableView(new CGRect(0, 0, 0, 0), UITableViewStyle.Plain);
                table_attachment.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                //table_attachment = new UITableView();
                table_attachment.ScrollEnabled = false;

                BT_like = new UIButton();
                BT_like.Font = UIFont.FromName("ArialMT", 12f);
                //BT_like.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                BT_like.SetTitleColor(UIColor.FromRGB(0, 95, 212), UIControlState.Normal);
                BT_like.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;

                BT_like.TouchUpInside += delegate
                {
                    FormCommentView formCommentView = parentView as FormCommentView;
                    formCommentView.SubmitLikeAction(indexpath.Section, commentItem);
                };

                BT_reply = new UIButton();
                BT_reply.Font = UIFont.FromName("ArialMT", 11f);
                //BT_reply.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                BT_reply.SetTitleColor(UIColor.FromRGB(0, 95, 212), UIControlState.Normal);
                BT_reply.SetTitle(CmmFunction.GetTitle("TEXT_REPLY", "Trả lời"), UIControlState.Normal);
                BT_reply.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
                BT_reply.TouchUpInside += delegate
                {
                    //parentView.HandleBtnReply(indexpath, commentItem);
                    FormCommentView formCommentView = parentView as FormCommentView;
                    formCommentView.beanComment = commentItem;
                    formCommentView.indexSelected = indexpath;
                    formCommentView.LoadContent();
                    formCommentView.txt_note.BecomeFirstResponder();
                };

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
                    BT_like.SetTitle(CmmFunction.GetTitle("TEXT_LIKE", "Thích"), UIControlState.Normal);
                    /*if (commentItem.IsLiked)
                        BT_like.SetTitleColor(UIColor.FromRGB(0, 95, 212), UIControlState.Normal);
                    else
                        BT_like.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    */
                    //if (commentItem.IsLiked)
                    //    BT_like.SetTitle(CmmFunction.GetTitle("TEXT_UNLIKE", "Bỏ thích"), UIControlState.Normal);
                    //else
                    //    BT_like.SetTitle(CmmFunction.GetTitle("TEXT_LIKE", "Thích"), UIControlState.Normal);

                    BT_like.SetTitleColor(UIColor.FromRGB(0, 95, 212), UIControlState.Normal);
                    BT_reply.SetTitleColor(UIColor.FromRGB(0, 95, 212), UIControlState.Normal);
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

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                iv_avatar.Frame = new CGRect(55, 10, 36, 36);
                lbl_imgCover.Frame = new CGRect(55, 5, 36, 36);
                lbl_title.Frame = new CGRect(lbl_imgCover.Frame.Right + 10, 10, ContentView.Frame.Width - iv_avatar.Frame.Right - 100, 16);
                lbl_createdDate.Frame = new CGRect(ContentView.Frame.Width - 100, 10, 90, 16);
                lbl_subTitle.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom + 7, ContentView.Frame.Width - iv_avatar.Frame.Right - 100, 14);
                iv_attach.Frame = new CGRect(ContentView.Frame.Width - 90, lbl_subTitle.Frame.Y + 2, 14, 14);
                lbl_attach_count.Frame = new CGRect(iv_attach.Frame.Right + 5, lbl_subTitle.Frame.Y + 2, 14, 14);
                iv_like.Frame = new CGRect(lbl_attach_count.Frame.Right + 5, lbl_subTitle.Frame.Y, 14, 14);
                lbl_like_count.Frame = new CGRect(iv_like.Frame.Right + 5, lbl_subTitle.Frame.Y, 14, 14);
                tv_comment.Frame = new CGRect(lbl_title.Frame.X, lbl_subTitle.Frame.Bottom + 7, ContentView.Frame.Width - lbl_title.Frame.X - 25, 25);

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
                            height += 110;
                        else
                            height += 35;
                    }
                    table_attachment.Frame = new CGRect(lbl_title.Frame.X, tv_comment.Frame.Bottom, tv_comment.Frame.Width, height);
                }
                else
                    table_attachment.Frame = new CGRect(lbl_title.Frame.X, tv_comment.Frame.Bottom, tv_comment.Frame.Width, 0);

                BT_like.Frame = new CGRect(lbl_title.Frame.X, table_attachment.Frame.Bottom + 5, 60, 15);
                BT_reply.Frame = new CGRect(BT_like.Frame.Right, table_attachment.Frame.Bottom + 5, 45, 15);
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

        #region
        #region collection thumb attachment
        public class CollectionAttachmentThumb_Source : UICollectionViewSource
        {
            FormCommentView parentView { get; set; }
            public static Dictionary<string, List<BeanWorkflow>> dict_groupWorkFlow;
            public List<BeanAttachFile> lst_thumbAttach;

            public CollectionAttachmentThumb_Source(FormCommentView _parentview, List<BeanAttachFile> _lst_thumbAttach)
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
                cell.UpdateRowReply(thumb, parentView);
                return cell;
            }
        }

        private class CustomFlowLayoutDelegate : UICollectionViewDelegateFlowLayout
        {
            static FormCommentView parentView;
            CollectionAttachmentThumb_Source collect_source;
            UICollectionView CollectionView;

            #region Constructors
            public CustomFlowLayoutDelegate(FormCommentView _parent, CollectionAttachmentThumb_Source _collect_source, UICollectionView collectionView)
            {
                collect_source = _collect_source;
                CollectionView = collectionView;
                parentView = _parent;
            }
            #endregion

            #region Override Methods
            public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
            {
                return new CGSize(115, 55);
            }

            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                var item = collect_source.lst_thumbAttach[indexPath.Row];
                parentView.HandleSeclectItem(item);
            }
            #endregion
        }
        #endregion
        #endregion

        #region add attachment source table
        private class Attachment_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellAttachmentID");
            FormCommentView parentView;
            List<BeanAttachFile> lst_attachment = new List<BeanAttachFile>();
            List<string> sectionKeys;

            public Attachment_TableSource(List<BeanAttachFile> _lst_attachment, FormCommentView _parentview)
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
                return 35;
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
            FormCommentView parentView { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            BeanAttachFile attachment { get; set; }
            UILabel lbl_title;
            UIButton BT_delete;
            UIImageView iv_type;

            public Attachment_cell_custom(FormCommentView _parentView, NSString cellID, BeanAttachFile _attachment, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
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
                    parentView.HandleAttachmentRemove(attachment);
                    //parentView.HandleRemoveThumbItem(attachment);
                };

                iv_type = new UIImageView();
                iv_type.ContentMode = UIViewContentMode.ScaleAspectFill;

                ContentView.AddSubviews(new UIView[] { lbl_title, BT_delete, iv_type });
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

                    SetImageAttachment(attachment.Path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("attachment_cell_custom - UpdateCell - ERR: " + ex.ToString());
                }
            }
            /// <summary>
            /// Trả về Resource Drawable ID của file đính kèm ứng với Path
            /// </summary>
            public void SetImageAttachment(string _path)
            {
                iv_type.Image = UIImage.FromFile("Icons/icon_attachFile_other.png");
                try
                {
                    if (!string.IsNullOrEmpty(_path))
                    {
                        string extend = System.IO.Path.GetExtension(_path).ToLowerInvariant();

                        switch (extend.ToLower())
                        {
                            case ".doc":
                            case ".docx":
                                iv_type.Image = UIImage.FromFile("Icons/icon_word.png");
                                break;
                            case ".txt":
                                iv_type.Image = UIImage.FromFile("Icons/icon_attachFile_txt.png");
                                break;
                            case ".xls":
                            case ".xlsx":
                                iv_type.Image = UIImage.FromFile("Icons/icon_attachFile_excel.png");
                                break;
                            case ".pdf":
                                iv_type.Image = UIImage.FromFile("Icons/icon_attachFile_pdf.png");
                                break;
                            case ".png":
                            case ".jpeg":
                            case ".jpg":
                                iv_type.Image = UIImage.FromFile("Icons/icon_attachFile_photo.png");
                                break;
                            default:
                                iv_type.Image = UIImage.FromFile("Icons/icon_attachFile_other.png");
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine("Author: khoahd - ControllerStartView - CheckAppHasConnection - Error: " + ex.Message);
#endif
                }
            }
            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                iv_type.Frame = new CGRect(10, 5, 20, 20);
                lbl_title.Frame = new CGRect(iv_type.Frame.Right + 15, 5, ContentView.Frame.Width - 80, 16);
                BT_delete.Frame = new CGRect(ContentView.Frame.Width - 30, 0, 25, 25);
            }
        }
        #endregion

        #region comment attachment
        public class CommentAttachment_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellAttachmentThumbID");
            FormCommentView parentView;
            List<BeanAttachFile> lst_attachment = new List<BeanAttachFile>();
            List<string> sectionKeys;

            public CommentAttachment_TableSource(List<BeanAttachFile> _lst_attachment, FormCommentView _parentview)
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
                    return 110;
                else
                    return 35;
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