using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using Foundation;
using UIKit;
using static BPMOPMobile.Class.CmmFunction;

namespace BPMOPMobileV1.iOS.CustomControlClass
{
    class Custom_AttachFileView : UIView
    {
        UIButton BT_back;
        UILabel lbl_title;
        UIView view_topLine;
        UITableView table_attachFile;
        public CGRect topview_frame;
        public UIViewController supperview { get; set; }
        public UIViewController parentview { get; set; }
        public bool isComment { get; set; }
        public ViewElement element { get; set; }

        private Custom_AttachFileView()
        {
            this.BackgroundColor = UIColor.White;

            lbl_title = new UILabel()
            {
                Font = UIFont.BoldSystemFontOfSize(15),
                TextColor = UIColor.FromRGB(65, 80, 134),
                TextAlignment = UITextAlignment.Center
            };
            //lbl_title.Text = CmmFunction.GetTitle("TEXT_ATTACHMENT", "Đính kèm");
            lbl_title.Text = CmmFunction.GetTitle("TEXT_ATTACHMENT", "Tài liệu đính kèm");

            BT_back = new UIButton();
            BT_back.SetImage(UIImage.FromFile("Icons/icon_back20.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
            BT_back.TintColor = UIColor.LightGray;
            BT_back.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);

            view_topLine = new UIView();
            view_topLine.BackgroundColor = UIColor.FromRGB(216, 216, 216);

            table_attachFile = new UITableView();
            table_attachFile.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            table_attachFile.ContentInset = new UIEdgeInsets(0, 0, 0, 0);

            this.AddSubviews(new UIView[] { BT_back, lbl_title, view_topLine, table_attachFile });

            if (BT_back != null)
                BT_back.AddTarget(HandleBtnAction, UIControlEvent.TouchUpInside);
        }

        private void HandleBtnAction(object sender, EventArgs e)
        {
            if (supperview != null && supperview.GetType() == typeof(AddAttachmentsView))
            {
                AddAttachmentsView controller = (AddAttachmentsView)supperview;
                controller.HandleAddAttachFileClose();
            }
            else if (parentview != null && parentview.GetType() == typeof(FormCommentView))
            {
                FormCommentView controller = (FormCommentView)parentview;
                controller.HandleAddAttachFileClose();
            }
            else if (parentview != null && parentview.GetType() == typeof(FormCreateTaskView))
            {
                FormCreateTaskView controller = (FormCreateTaskView)parentview;
                controller.HandleAttachFileClose();
            }
            else if (parentview != null && parentview.GetType() == typeof(FormTaskDetails))
            {
                FormTaskDetails controller = (FormTaskDetails)parentview;
                controller.HandleAttachFileClose();
            }
        }

        private static Custom_AttachFileView instance = null;
        public static Custom_AttachFileView Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Custom_AttachFileView();
                }
                return instance;
            }
        }

        public void InitFrameView(CGRect frame)
        {
            if (this.Frame == CGRect.Empty)
            {
                this.Frame = frame;
                BT_back.Frame = new CGRect(15, 0, 30, 30);//40
                lbl_title.Frame = new CGRect((this.Frame.Width - 200) / 2, 0, 200, 30);
                view_topLine.Frame = new CGRect(18, lbl_title.Frame.Bottom + 10, Frame.Width - 36, 1);
                table_attachFile.Frame = new CGRect(0, view_topLine.Frame.Bottom + 10, Frame.Width, Frame.Height - 80);
            }
        }

        public void TableLoadData()
        {
            lbl_title.Text = CmmFunction.GetTitle("TEXT_ATTACHMENT", "Tài liệu đính kèm");
            //Tam khoa File in app
            List<BeanAttachFileLocal> lst_fileInApp = new List<BeanAttachFileLocal>();
            NSError err = null;
            string localDocumentFilepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), CmmVariable.M_DataFolder);
            var FileManager = new NSFileManager();
            var lst_file = FileManager.GetDirectoryContent(localDocumentFilepath, out err);
            if (lst_file != null)
            {
                for (int i = 0; i < lst_file.Count(); i++)
                {
                    var path = localDocumentFilepath + "/" + lst_file[i];
                    var size = (Int64)FileManager.Contents(path).Length;

                    int index;
                    string type;
                    if (lst_file[i].Contains('.'))
                    {
                        index = lst_file[i].LastIndexOf('.');
                        type = lst_file[i].Substring((index + 1), lst_file[i].Length - (index + 1));

                        BeanAttachFileLocal attachLocal = new BeanAttachFileLocal() { ID = i, Name = lst_file[i], Type = type, Path = path, Size = size };

                        if (attachLocal != null)
                            lst_fileInApp.Add(attachLocal);
                    }
                }
            }

            Dictionary<string, List<BeanAttachFileLocal>> dict_attachFile = new Dictionary<string, List<BeanAttachFileLocal>>();
            dict_attachFile.Add(CmmVariable.SysConfig.LangCode == "1033" ? "Files in app" : "Tập tin trong ứng dụng", lst_fileInApp);

            List<BeanAttachFileLocal> lst_otherLocation = new List<BeanAttachFileLocal>();
            BeanAttachFileLocal itemiCloudAndDevice = new BeanAttachFileLocal()
            {
                ID = 1,
                Name = CmmVariable.SysConfig.LangCode == "1033" ? "iCloud Drive & Device" : "iCloud và bộ nhớ thiết bị",
                Icon = "Icons/icon_cellphone_iphone.png"
            };
            BeanAttachFileLocal itemPhoto = new BeanAttachFileLocal()
            {
                ID = 2,
                Name = CmmVariable.SysConfig.LangCode == "1033" ? "Choose Photo From Library" : "Ứng dụng Thư Viện",
                Icon = "Icons/icon_photo_library.png"
            };
            BeanAttachFileLocal itemCamera = new BeanAttachFileLocal()
            {
                ID = 3,
                Name = CmmVariable.SysConfig.LangCode == "1033" ? "Take from Camera" : "Ứng dụng máy ảnh",
                Icon = "Icons/icon_camera1.png"
            };
            lst_otherLocation.AddRange(new[] { itemiCloudAndDevice, itemPhoto, itemCamera });

            dict_attachFile.Add(CmmVariable.SysConfig.LangCode == "1033" ? "Other Locations" : "Nguồn khác", lst_otherLocation);

            table_attachFile.Source = new AttachFile_TableSource(dict_attachFile, this);
        }

        public UITableView tableWorkFollow
        {
            get
            {
                return table_attachFile;
            }
        }

        public void HandleItemSelected(BeanAttachFileLocal _attachFile)
        {
            if (parentview != null && parentview.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 pa = (RequestDetailsV2)parentview;
                AddAttachmentsView sp = supperview as AddAttachmentsView;
                if (!string.IsNullOrEmpty(_attachFile.Type))
                {
                    pa.HandleAddAttachFileResult(_attachFile, null, element.DataType);
                    sp.HandleAddAttachFileClose();
                }
                else
                {
                    //sp.HandleAddAttachFileClose();
                    //pa.NavigationToDocumentPicker(sp);

                    //if (_attachFile.ID == 1)
                    //    pa.NavigationToDocumentPicker(sp, element.DataType);
                    //else
                    //    pa.NavigationToImagePicker(sp);

                    if (_attachFile.ID == 1)
                        pa.NavigationToDocumentPicker(sp, element.DataType);
                    else if (_attachFile.ID == 2)
                        pa.NavigationToImagePicker(sp);
                    else if (_attachFile.ID == 3)
                        pa.NavigationToCameraPicker(sp);
                }
            }
            else if (parentview != null && parentview.GetType() == typeof(FormCommentView))
            {
                FormCommentView formCommentView = (FormCommentView)parentview;
                if (!string.IsNullOrEmpty(_attachFile.Type))
                {
                    formCommentView.HandleAddAttachFileResult(_attachFile);
                    formCommentView.HandleAttachFileClose();
                }
                else
                {
                    if (_attachFile.ID == 1)
                        formCommentView.NavigationToDocumentPicker();
                    else if (_attachFile.ID == 2)
                        formCommentView.NavigationToImagePicker();
                    else if (_attachFile.ID == 3)
                        formCommentView.NavigationToCameraPicker();
                }
            }
            else if (parentview != null && parentview.GetType() == typeof(FormTaskDetails))
            {
                FormTaskDetails controller = (FormTaskDetails)parentview;
                if (!string.IsNullOrEmpty(_attachFile.Type))
                {
                    controller.HandleAddAttachFileResult(_attachFile);
                    controller.HandleAttachFileClose();
                }
                else
                {
                    if (_attachFile.ID == 1)
                        controller.NavigationToDocumentPicker();
                    else if (_attachFile.ID == 2)
                        controller.NavigationToImagePicker();
                    else if (_attachFile.ID == 3)
                        controller.NavigationToCameraPicker();
                }
            }
            else if (parentview != null && parentview.GetType() == typeof(FormCreateTaskView))
            {
                FormCreateTaskView controller = (FormCreateTaskView)parentview;
                if (!string.IsNullOrEmpty(_attachFile.Type))
                {
                    controller.HandleAddAttachFileResult(_attachFile);
                    controller.HandleAttachFileClose();
                }
                else
                {
                    if (_attachFile.ID == 1)
                        controller.NavigationToDocumentPicker();
                    else if (_attachFile.ID == 2)
                        controller.NavigationToImagePicker();
                    else if (_attachFile.ID == 3)
                        controller.NavigationToCameraPicker();
                }
            }
        }

        #region cust view
        #region data source attach file
        private class AttachFile_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellAttachFile");
            Dictionary<string, List<BeanAttachFileLocal>> dict_attachFile { get; set; }
            List<string> lst_section { get; set; }
            Custom_AttachFileView parentView;

            public AttachFile_TableSource(Dictionary<string, List<BeanAttachFileLocal>> _dict_workflow, Custom_AttachFileView _parentview)
            {
                dict_attachFile = _dict_workflow;
                parentView = _parentview;
                lst_section = dict_attachFile.Keys.ToList();
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return dict_attachFile.Count;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                var key = lst_section[Convert.ToInt32(section)];
                var numRow = dict_attachFile[key].Count;
                return numRow;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                //var key = lst_section[indexPath.Section];
                //var attachment = dict_attachFile[key][indexPath.Row];

                //if (!string.IsNullOrEmpty(attachment.Type))
                //    return 50;
                //else
                //    return 40;

                return 50;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return 40;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var key = lst_section[indexPath.Section];
                var attachment = dict_attachFile[key][indexPath.Row];

                parentView.HandleItemSelected(attachment);

                tableView.DeselectRow(indexPath, true);
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                UIView rooView = new UIView();
                rooView.Frame = new CGRect(0, 0, tableView.Frame.Width, 45);
                rooView.BackgroundColor = UIColor.FromRGB(249, 249, 249);

                UILabel lbl_title = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(14),
                    TextColor = UIColor.Black
                };

                rooView.AddSubviews(new UIView[] { lbl_title });

                lbl_title.Frame = new CGRect(18, 0, rooView.Frame.Width - 36, rooView.Frame.Height);
                lbl_title.Text = lst_section[Convert.ToInt32(section)];

                return rooView;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var key = lst_section[indexPath.Section];
                var attachment = dict_attachFile[key][indexPath.Row];

                Attachment_cell_custom cell = new Attachment_cell_custom(parentView, cellIdentifier, attachment, indexPath);

                return cell;
            }
        }

        private class Attachment_cell_custom : UITableViewCell
        {
            Custom_AttachFileView parentView { get; set; }
            NSIndexPath currentIndexPath { get; set; }

            BeanAttachFileLocal attachment { get; set; }
            UILabel lbl_name, lbl_size, lbl_line;
            UIImageView iv_type;

            public Attachment_cell_custom(Custom_AttachFileView _parentView, NSString cellID, BeanAttachFileLocal _attachment, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
            {
                parentView = _parentView;
                attachment = _attachment;
                currentIndexPath = _currentIndexPath;
                Accessory = UITableViewCellAccessory.None;
                ViewConfiguration();
            }

            private void ViewConfiguration()
            {
                ContentView.BackgroundColor = UIColor.White;

                iv_type = new UIImageView();
                iv_type.ContentMode = UIViewContentMode.ScaleAspectFit;
                iv_type.Image = UIImage.FromFile("Icons/icon_word.png");

                lbl_name = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                    TextColor = UIColor.Black,
                    TextAlignment = UITextAlignment.Left,
                };

                lbl_size = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(12, UIFontWeight.Regular),
                    TextColor = UIColor.DarkGray,
                    TextAlignment = UITextAlignment.Left,
                };

                lbl_line = new UILabel()
                {
                    BackgroundColor = UIColor.FromRGB(144, 164, 174),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };

                ContentView.AddSubviews(new UIView[] { iv_type, lbl_name, lbl_size, lbl_line });
                LoadData();
            }

            public void LoadData()
            {
                try
                {
                    lbl_name.Text = attachment.Name;

                    lbl_size.Text = FileSizeFormatter.FormatSize(attachment.Size);

                    if (!string.IsNullOrEmpty(attachment.Icon))
                        iv_type.Image = UIImage.FromFile(attachment.Icon);
                    else
                    {
                        switch (attachment.Type.ToLower())
                        {
                            case "doc":
                            case "docx":
                                iv_type.Image = UIImage.FromFile("Icons/icon_word.png");
                                break;
                            case "xls":
                            case "xlsx":
                                iv_type.Image = UIImage.FromFile("Icons/icon_attachFile_excel.png");
                                break;
                            case "pdf":
                                iv_type.Image = UIImage.FromFile("Icons/icon_attachFile_pdf.png");
                                break;
                            case "png":
                            case "jpeg":
                            case "jpg":
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
                    Console.WriteLine("Custom_AttachFileView - Attachment_cell_custom - loaddata- ERR: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                if (!string.IsNullOrEmpty(attachment.Type))
                {
                    iv_type.Frame = new CGRect(20, 10, 30, 30);
                    lbl_name.Frame = new CGRect(iv_type.Frame.Right + 10, 5, ContentView.Frame.Width - (iv_type.Frame.Right + 10), 20);
                    lbl_size.Frame = new CGRect(iv_type.Frame.Right + 10, lbl_name.Frame.Bottom, ContentView.Frame.Width - (iv_type.Frame.Right + 10), 20);
                }
                else
                {
                    iv_type.Frame = new CGRect(20, 10, 30, 30);
                    lbl_name.Frame = new CGRect(iv_type.Frame.Right + 10, 10, ContentView.Frame.Width - (iv_type.Frame.Right + 10), 30);
                    lbl_size.Frame = CGRect.Empty;
                }

                lbl_line.Frame = new CGRect(iv_type.Frame.Right + 10, ContentView.Frame.Bottom - 1, ContentView.Frame.Width - (iv_type.Frame.Right + 10), 1);
            }
        }
        #endregion
        #endregion
    }

    public class BeanAttachFileLocal
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public bool IsImage { get; set; }
        public long Size { get; set; }
    }
    public static class FileSizeFormatter
    {
        // Load all suffixes in an array  
        static readonly string[] suffixes =
        { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        public static string FormatSize(Int64 bytes)
        {
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1} {1}", number, suffixes[counter]);
        }
    }
}