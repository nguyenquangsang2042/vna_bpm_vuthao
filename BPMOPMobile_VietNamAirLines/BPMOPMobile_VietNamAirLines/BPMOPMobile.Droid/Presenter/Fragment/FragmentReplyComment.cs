using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Style;
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
using BPMOPMobile.Droid.Presenter.Adapter;
using BPMOPMobile.Droid.Presenter.SharedView;
using Java.Lang;
using static BPMOPMobile.Droid.Class.CustomResizeLinearLayout;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentReplyComment : CustomBaseFragment, ITextWatcher, OnKeyboardStateChange
    {
        private MainActivity _mainAct;
        private LayoutInflater _inflater;
        private View _rootView;
        private ImageView _imgBack, _imgAttach, _imgComment;
        private EditText _edtComment;
        private TextView _tvTitle;
        private CustomFlexBoxRecyclerView _recyAttach;
        private RecyclerView _recyAttach_Image;
        private LinearLayout _lnAll, _lnComponentComment, _lnKeyBoard;
        private CustomResizeLinearLayout _lnContentCustomResize;
        private ControllerComment CTRLComment = new ControllerComment();
        private ComponentComment.AdapterParentAttachFile _adapterCommentAttach;
        private ComponentComment.AdapterParentAttachFile_Image _adapterCommentAttach_Image;
        //private AdapterExpandComment _adapterComment;

        private List<BeanAttachFile> _lstAttachFile = new List<BeanAttachFile>();
        private List<BeanComment> _lstComment = new List<BeanComment>();
        private BeanComment _parentComment = new BeanComment();

        private bool _isDeleteFullName = false;
        private string _currentFullName = "";

        private string _OtherResourceId;
        private string _ResourceCategoryId;
        private string _previousFragmentName;
        private CustomBaseFragment _previousFragment;
        private bool _renewPreviousFragment = false; // Nếu có click API like -> bật cờ này lên để renew lại list
        private ComponentComment componentComment;
        public Java.IO.File _tempfileFromCamera;

        public FragmentReplyComment() { /* Prevent Darkmode */}
        public FragmentReplyComment(CustomBaseFragment _previousFragment, string _previousFragmentName, BeanComment _parentComment, List<BeanComment> _lstComment, string _OtherResourceId, string _ResourceCategoryId)
        {
            this._previousFragment = _previousFragment;
            this._previousFragmentName = _previousFragmentName;
            this._parentComment = _parentComment;
            this._lstComment = _lstComment;
            this._OtherResourceId = _OtherResourceId;
            this._ResourceCategoryId = _ResourceCategoryId;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override void OnDestroyView()
        {

            _mainAct.Window.SetBackgroundDrawableResource(Resource.Drawable.img_background_splashscreen_2);
            base.OnDestroyView();
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _inflater = inflater;
            _mainAct = (MainActivity)this.Activity;
            if (_rootView == null)
            {
                _mainAct.Window.SetSoftInputMode(SoftInput.AdjustResize);
                _mainAct.Window.SetBackgroundDrawableResource(Resource.Drawable.textwhitesolid);
                _rootView = inflater.Inflate(Resource.Layout.ViewReplyComment, null);
                _lnAll = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewReplyComment_All);
                _lnContentCustomResize = _rootView.FindViewById<CustomResizeLinearLayout>(Resource.Id.ln_ViewReplyComment_Content);
                _lnKeyBoard = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_KeyBoard);
                _lnComponentComment = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewReplyComment_ComponentComment);
                _tvTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewReplyComment_Title);
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewReplyComment_Back);
                _imgAttach = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewReplyComment_Attach);
                _imgComment = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewReplyComment_Comment);
                _edtComment = _rootView.FindViewById<EditText>(Resource.Id.edt_ViewReplyComment_Comment);
                _recyAttach = _rootView.FindViewById<CustomFlexBoxRecyclerView>(Resource.Id.recy_ViewReplyComment_AttachFile);
                _recyAttach_Image = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewReplyComment_AttachFile_Image);

                _recyAttach.SetMaxRowAndRowHeight((int)CmmDroidFunction.ConvertDpToPixel(40, _rootView.Context), 3);

                _lnAll.Click += delegate { };
                _imgBack.Click += Click_imgBack;
                _imgComment.Click += Click_imgComment;
                _imgAttach.Click += Click_imgAttach;

                _lnContentCustomResize.SetKeyboardStateListener(this);

                _edtComment.AddTextChangedListener(this);
            }
            SetViewByLanguage();

            Action action = new Action(() =>
            {
                CmmDroidFunction.ShowSoftKeyBoard(_edtComment, _mainAct);
                _edtComment.RequestFocus();
                Action action2 = new Action(() =>
                {
                    // Gán tên thằng dc reply lên First time, sau đó gọi SetData để cập nhật lại item phù hợp
                    BeanUser _user = CmmFunction.GetBeanUserByID(_parentComment.Author);
                    if (_user != null)
                    {
                        _edtComment.Text = " " + _user.FullName + "  ";
                        _currentFullName = " " + _user.FullName + " ";

                        CmmDroidFunction.HightLightTextSpecific(_edtComment, _edtComment.Text, _user.FullName, "#e5eeff");
                        _edtComment.SetSelection(_edtComment.Text.Length);
                    }
                    SetData();
                });
                new Handler().PostDelayed(action2, CmmDroidVariable.M_ActionDelayTime + 500);
            });
            new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
            return _rootView;
        }

        private void Textchanged_EdtComment(object sender, TextChangedEventArgs e)
        {
            try
            {


            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Textchanged_EdtComment", ex);
#endif
            }
        }
        private void SetViewByLanguage()
        {
            try
            {
                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    _tvTitle.Text = CmmFunction.GetTitle("TEXT_COMMENT", "Bình luận");
                }
                else
                {
                    _tvTitle.Text = CmmFunction.GetTitle("TEXT_COMMENT", "Comment");
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewByLanguage", ex);
#endif
            }
        }
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                if ((requestCode == CmmDroidVariable.M_ReplyComment_AttachFile || requestCode == CmmDroidVariable.M_ReplyComment_AttachFile_Camera) && resultCode == (int)Result.Ok) // đính kèm comment
                {
                    // Comment
                    BeanAttachFile _beanAttachFile = new BeanAttachFile();

                    if (requestCode == CmmDroidVariable.M_ReplyComment_AttachFile) // chọn file thường
                    {
                        _beanAttachFile = _mainAct.GetAttachFileFromURI(_mainAct, _rootView.Context, data.Data);
                    }
                    else // chụp từ camera
                    {
                        if (_tempfileFromCamera != null)
                        {
                            Android.Net.Uri contentUri = FileProvider.GetUriForFile(_mainAct, CmmDroidVariable.M_PackageProvider, _tempfileFromCamera);
                            _beanAttachFile = _mainAct.GetAttachFileFromURI_Camera(_mainAct, _rootView.Context, contentUri);
                            _beanAttachFile.Path = _tempfileFromCamera.Path;
                        }
                        else
                        {
                            _beanAttachFile = null;
                        }
                    }

                    if (_beanAttachFile == null || (_beanAttachFile != null && string.IsNullOrEmpty(_beanAttachFile.Path)))
                    {
                        CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"),
                                          CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                        return;
                    }

                    if (CTRLComment.CheckFileExistInList(_lstAttachFile, _beanAttachFile) == true)
                    {
                        CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_EXISTINGFILE", "File đã tồn tại trong danh sách"),
                                           CmmFunction.GetTitle("TEXT_EXISTINGFILE", "File has been existed in list"));
                        return;
                    }

                    _lstAttachFile.Add(_beanAttachFile);
                    _lstAttachFile = CmmDroidFunction.ClassifyListAttachFile(_lstAttachFile).ToList(); // phân loại nếu ảnh thì IsImage = true

                    if (_adapterCommentAttach != null)
                    {
                        _adapterCommentAttach.UpdateListAttach(_lstAttachFile);
                        _adapterCommentAttach.NotifyDataSetChanged();
                    }

                    if (_adapterCommentAttach_Image != null)
                    {
                        _adapterCommentAttach_Image.UpdateListAttach(_lstAttachFile);
                        _adapterCommentAttach_Image.NotifyDataSetChanged();
                    }
                }
            }
            catch (System.Exception ex)
            {
                CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"),
                  CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));

#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnActivityResult", ex);
#endif
            }
        }

        #region Event
        private void Click_imgBack(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    //OnKeyboardHide();                
                    CmmDroidFunction.HideSoftKeyBoard(_edtComment, _mainAct);

                    Action action = new Action(() =>
                    {
                        if (_renewPreviousFragment == true) // Renew lại list trước đó
                        {
                            if (_previousFragmentName.Equals("FragmentDetailWorkflow"))
                            {
                                FragmentDetailWorkflow _temp = (FragmentDetailWorkflow)_previousFragment;
                                _temp._adapterDetailExpandControl.NotifyDataSetChanged();
                            }
                            else if (_previousFragmentName.Equals("FragmentDetailCreateTask"))
                            {
                                FragmentDetailCreateTask _temp = (FragmentDetailCreateTask)_previousFragment;
                                _temp.SetDataComment();
                            }
                        }
                        _mainAct.HideFragment();

                        Action action2 = new Action(() =>
                        {
                            _mainAct.Window.SetSoftInputMode(SoftInput.AdjustPan);
                        });
                        new Handler().PostDelayed(action2, CmmDroidVariable.M_ActionDelayTime);
                    });
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime + 500);
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgBack", ex);
#endif
            }
        }

        private void Click_imgBackWithReFresh(object sender, EventArgs e)
        {
            try
            {
                CmmDroidFunction.HideSoftKeyBoard(_edtComment, _mainAct);
                if (_previousFragmentName.Equals("FragmentDetailWorkflow"))
                {
                    FragmentDetailWorkflow _temp = (FragmentDetailWorkflow)_previousFragment;
                    _temp.GetAndSetDataFromServer();
                }
                else if (_previousFragmentName.Equals("FragmentDetailCreateTask"))
                {
                    FragmentDetailCreateTask _temp = (FragmentDetailCreateTask)_previousFragment;
                    _temp.SetData();
                }
                _mainAct.HideFragment();
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgBack", ex);
#endif
            }
        }

        private async void Click_imgComment(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    CmmDroidFunction.HideSoftKeyBoard(_edtComment, _mainAct);
                    if (!string.IsNullOrEmpty(_edtComment.Text) || (_lstAttachFile != null && _lstAttachFile.Count > 0))
                    {
                        CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);

                        await Task.Run(() =>
                        {
                            ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();

                            BeanOtherResource beanOtherResource = new BeanOtherResource();
                            beanOtherResource.Content = _edtComment.Text.Trim().Replace("  ", " ");
                            beanOtherResource.ResourceId = _OtherResourceId;
                            beanOtherResource.ResourceCategoryId = int.Parse(_ResourceCategoryId);
                            beanOtherResource.ResourceSubCategoryId = 0;
                            beanOtherResource.Image = "";
                            beanOtherResource.ParentCommentId = _parentComment.ID;

                            #region Get Import Attach File
                            List<KeyValuePair<string, string>> _lstKeyVarAttachmentLocal = new List<KeyValuePair<string, string>>();

                            if (_lstAttachFile != null && _lstAttachFile.Count > 0)
                            {
                                foreach (var item in _lstAttachFile)
                                {
                                    if (item.ID == "")
                                    {
                                        string key = item.Title;
                                        KeyValuePair<string, string> _UploadFile = new KeyValuePair<string, string>(key, item.Path);
                                        _lstKeyVarAttachmentLocal.Add(_UploadFile);
                                    }
                                }
                            }
                            #endregion


                            bool _result = _pControlDynamic.AddComment(beanOtherResource, _lstKeyVarAttachmentLocal);

                            _mainAct.RunOnUiThread(() =>
                            {
                                CmmDroidFunction.HideProcessingDialog();
                                if (_result == true)
                                {
                                    Click_imgBackWithReFresh(null, null);
                                }
                                else
                                {
                                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"),
                                        CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                                }
                            });
                        });
                    }
                    else
                    {
                        CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_HINT_REQUIRE_CONTENT", "Vui lòng nhập nội dung..."),
                            CmmFunction.GetTitle("TEXT_HINT_REQUIRE_CONTENT", "Please enter content..."));
                    }
                }
            }
            catch (System.Exception ex)
            {
                CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgComment", ex);
#endif
            }
        }

        private void Click_imgAttach(object sender, EventArgs e)
        {
            try
            {
                CmmDroidFunction.HideSoftKeyBoard(_edtComment, _mainAct);
                SharedView_PopupChooseFile SharedPopUpChooseFile = new SharedView_PopupChooseFile(_inflater, _mainAct, this, "FragmentReplyComment", _rootView,
                    CmmDroidVariable.M_ReplyComment_AttachFile,
                    CmmDroidVariable.M_ReplyComment_AttachFile_Camera,
                    (int)SharedView_PopupChooseFile.FlagView.ReplyComment);
                SharedPopUpChooseFile.InitializeView();
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgAttach", ex);
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
                            CmmDroidFunction.HideProcessingDialog();
                            if (_result == true)
                            {
                                _renewPreviousFragment = true;
                                #region Update View Value
                                e.IsLiked = !e.IsLiked;
                                if (e.IsLiked == true)
                                    e.LikeCount = e.LikeCount + 1;
                                else
                                    e.LikeCount = e.LikeCount - 1 < 0 ? 0 : e.LikeCount - 1; // nếu < 0 thì gán = 0
                                #endregion

                                CTRLComment.UpdateIsLikedComment(_OtherResourceId, e.IsLiked, e.LikeCount); // Update Sqlite
                                //_adapterComment.NotifyDataSetChanged();
                                SetData();
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
            catch (System.Exception ex)
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
                    CmmDroidFunction.ShowSoftKeyBoard(_edtComment, _mainAct);
                    BeanUser _user = CmmFunction.GetBeanUserByID(e.Author);
                    if (_user != null)
                    {
                        _edtComment.Text = " " + _user.FullName + "  ";
                        _currentFullName = " " + _user.FullName + " ";

                        CmmDroidFunction.HightLightTextSpecific(_edtComment, _edtComment.Text, _user.FullName, "#e5eeff");
                    }
                    _edtComment.RequestFocus();
                    _edtComment.SetSelection(_edtComment.Text.Length);

                    // Attach File
                    _lstAttachFile = new List<BeanAttachFile>();
                    if (_adapterCommentAttach != null)
                    {
                        _adapterCommentAttach.UpdateListAttach(_lstAttachFile);
                        _adapterCommentAttach.NotifyDataSetChanged();
                    }
                    if (_adapterCommentAttach_Image != null)
                    {
                        _adapterCommentAttach_Image.UpdateListAttach(_lstAttachFile);
                        _adapterCommentAttach_Image.NotifyDataSetChanged();
                    }
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_itemExpandComment_Reply", ex);
#endif
            }
        }

        private void click_ItemRecyAttach_Delete(object sender, BeanAttachFile e)
        {
            try
            {
                if (e != null)
                {
                    _lstAttachFile = _lstAttachFile.Where(x => !x.Path.Equals(e.Path)).ToList();

                    if (_adapterCommentAttach != null)
                    {
                        _adapterCommentAttach.UpdateListAttach(_lstAttachFile);
                        _adapterCommentAttach.NotifyDataSetChanged();
                    }
                    if (_adapterCommentAttach_Image != null)
                    {
                        _adapterCommentAttach_Image.UpdateListAttach(_lstAttachFile);
                        _adapterCommentAttach_Image.NotifyDataSetChanged();
                    }
                }
            }
            catch (System.Exception ex)
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
                if (!e.ID.Equals("")) // mở file từ server
                {
                    CmmDroidFunction.DownloadAndOpenFile(_mainAct, _rootView.Context, CmmVariable.M_Domain + e.Path);
                }
                else // mở file từ local
                {
                    if (System.IO.File.Exists(e.Path))
                    {
                        CmmDroidFunction.OpenFile(_mainAct, _rootView.Context, e.Path);
                    }
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "click_ItemRecyAttach_Detail", ex);
#endif
            }
        }

        private void Click_ItemRecyAttach_Attach_Detail(object sender, BeanAttachFile e)
        {
            try
            {
                CmmDroidFunction.DownloadAndOpenFile(_mainAct, _rootView.Context, CmmVariable.M_Domain + e.Url); // trường hợp đặc biệt xài URL
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "click_ItemRecyAttach_Detail", ex);
#endif
            }
        }

        public void OnKeyboardShow()
        {
            try
            {
                _lnKeyBoard.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                //Action action = new Action(() =>
                //{
                //    _mainAct.Window.SetSoftInputMode(SoftInput.AdjustResize);
                //});
                //new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime + 500);
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnKeyboardShow", ex);
#endif
            }
        }

        public void OnKeyboardHide()
        {
            try
            {
                _lnKeyBoard.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                //Action action = new Action(() =>
                //{
                //    _mainAct.Window.SetSoftInputMode(SoftInput.AdjustPan);
                //});
                //new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime+500);
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnKeyboardHide", ex);
#endif
            }
        }
        #endregion

        private void SetData()
        {
            try
            {
                if (string.IsNullOrEmpty(_parentComment.ParentCommentId)) // Reply cho comment parent -> search theo chính nó
                {
                    _lstComment = _lstComment.Where(x => x.ID == _parentComment.ID || x.ParentCommentId == _parentComment.ID).ToList();
                }
                else // Reply cho comment child -> search theo parent
                {
                    _lstComment = _lstComment.Where(x => x.ID == _parentComment.ParentCommentId || x.ParentCommentId == _parentComment.ParentCommentId).ToList();
                    // Gán root lại cho Parent vì chỉ có thể reply cho Root (root thì parent = null)
                    _parentComment = _lstComment.Where(x => string.IsNullOrEmpty(x.ParentCommentId)).FirstOrDefault();
                }

                _lnComponentComment.RemoveAllViews();
                componentComment = new ComponentComment(_mainAct, _lnComponentComment, _lstComment, true);
                componentComment.InitializeFrameView(_lnComponentComment);
                componentComment.SetTitle();
                componentComment.SetValue();
                componentComment.SetEnable();
                componentComment.SetProprety();

                //Event Component
                componentComment.CustomItemClick_ItemListComment_tvLike += Click_ItemListComment_Like;
                componentComment.CustomItemClick_ItemListComment_tvReply += Click_ItemListComment_Reply;
                componentComment.CustomItemClick_ItemListComment_AttachDetail += Click_ItemRecyAttach_Attach_Detail;


                //BeanUser _user = CmmFunction.GetBeanUserByID(_parentComment.Author);
                //if (_user != null)
                //{
                //    _edtComment.Text = " " + _user.FullName + "  ";
                //    _currentFullName = " " + _user.FullName + " ";
                //    CmmDroidFunction.HightLightTextSpecific(_edtComment, _edtComment.Text, _user.FullName, "#e5eeff");
                //    _edtComment.SetSelection(_edtComment.Text.Length);
                //}

                // Attach File

                if (_lstAttachFile != null && _lstAttachFile.Count > 0)
                    _lstAttachFile = CmmDroidFunction.ClassifyListAttachFile(_lstAttachFile).ToList(); // phân loại nếu ảnh thì IsImage = true

                _adapterCommentAttach = new ComponentComment.AdapterParentAttachFile(_mainAct, _rootView.Context, _lstAttachFile, true);
                _adapterCommentAttach.CustomItemClick_Detail += click_ItemRecyAttach_Detail;
                _adapterCommentAttach.CustomItemClick_Delete += click_ItemRecyAttach_Delete;
                _recyAttach.SetAdapter(_adapterCommentAttach);
                _recyAttach.SetLayoutManager(new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical));

                _adapterCommentAttach_Image = new ComponentComment.AdapterParentAttachFile_Image(_mainAct, _rootView.Context, _lstAttachFile, true);
                _adapterCommentAttach_Image.CustomItemClick_Detail += click_ItemRecyAttach_Detail;
                _adapterCommentAttach_Image.CustomItemClick_Delete += click_ItemRecyAttach_Delete;
                _recyAttach_Image.SetAdapter(_adapterCommentAttach_Image);
                _recyAttach_Image.SetLayoutManager(new LinearLayoutManager(_rootView.Context, LinearLayoutManager.Horizontal, false));


            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
        }

        public void AfterTextChanged(IEditable s)
        {
            try
            {
                if (!_edtComment.Text.Contains(_currentFullName))
                {
                    //CmmDroidFunction.HightLightTextSpecific(_edtComment, _edtComment.Text, _edtComment.Text, "#ffffff");
                }

                if (_isDeleteFullName == true)
                {
                    _edtComment.Text = "";
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "AfterTextChanged", ex);
#endif
            }
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
            try
            {

            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "BeforeTextChanged", ex);
#endif
            }
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            try
            {
                if (s.ToString() == _currentFullName)
                    _isDeleteFullName = true;
                else
                    _isDeleteFullName = false;
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnTextChanged", ex);
#endif
            }
        }
    }
}
