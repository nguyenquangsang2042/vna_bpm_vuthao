using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Tablet.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using Refractored.Controls;
using BPMOPMobile.Droid.Presenter.Fragment;

namespace BPMOPMobile.Droid.Tablet.Presenter.Fragment
{
    [Obsolete]
    public class FragmentLeftMenu : Android.App.Fragment
    {
        private MainActivity _mainAct;
        private View _rootView;
        private TextView _tvSignout, _tvName, _tvEmail, _tvYcxl, _tvYcxlCount, _tvVDT, _tvVDTCount, _tvVTBD, _tvVTBDCount, _tvHome, _tvVApp, _tvLanguage, _tvApplabel;
        private View _viewHome, _viewVDT, _viewVTBD;
        private ImageView _imgSignOut, _imgYcxl, _imgVDT, _imgVTBD, _imgHome;
        private CircleImageView _imgAvata;
        private Switch _switchLanguage;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _mainAct = (MainActivity)this.Activity;
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewLeftMenu, null);
                _tvName = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_Name);
                _tvEmail = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_Email);
                _tvSignout = _rootView.FindViewById<TextView>(Resource.Id.tv_LeftMain_SignOut);
                _tvHome = _rootView.FindViewById<TextView>(Resource.Id.tv_LeftMain_HomePage);
                _tvVTBD = _rootView.FindViewById<TextView>(Resource.Id.tv_LeftMain_VTBD);
                _tvVTBDCount = _rootView.FindViewById<TextView>(Resource.Id.tv_LeftMain_VTBDCount);
                _tvVDT = _rootView.FindViewById<TextView>(Resource.Id.tv_LeftMain_VDT);
                _tvVDTCount = _rootView.FindViewById<TextView>(Resource.Id.tv_LeftMain_VDTCount);
                _tvYcxl = _rootView.FindViewById<TextView>(Resource.Id.tv_LeftMain_YCXL);
                _tvYcxlCount = _rootView.FindViewById<TextView>(Resource.Id.tv_LeftMain_YCXLCount);
                _imgSignOut = _rootView.FindViewById<ImageView>(Resource.Id.img_LeftMain_SignOut);
                _tvLanguage = _rootView.FindViewById<TextView>(Resource.Id.tv_LeftMain_Language);
                _imgVTBD = _rootView.FindViewById<ImageView>(Resource.Id.img_LeftMain_VTBD);
                _imgVDT = _rootView.FindViewById<ImageView>(Resource.Id.img_LeftMain_VDT);
                _imgYcxl = _rootView.FindViewById<ImageView>(Resource.Id.img_LeftMain_YCXL);
                _imgHome = _rootView.FindViewById<ImageView>(Resource.Id.img_LeftMain_HomePage);
                _imgAvata = _rootView.FindViewById<CircleImageView>(Resource.Id.img_ViewLeftMenu_Avata);
                _imgHome.SetColorFilter(Resources.GetColor(Resource.Color.clViolet));
                _switchLanguage = _rootView.FindViewById<Switch>(Resource.Id.sw_LeftMain_Language);
                _tvVApp = _rootView.FindViewById<TextView>(Resource.Id.tv_LeftMain_InforApp);
                _tvApplabel = _rootView.FindViewById<TextView>(Resource.Id.tv_LeftMain_InfoApp);
                _viewHome = _rootView.FindViewById<View>(Resource.Id.view_LeftMain_HomePage);
                _viewVDT = _rootView.FindViewById<View>(Resource.Id.view_LeftMain_VDT);
                _viewVTBD = _rootView.FindViewById<View>(Resource.Id.view_LeftMain_VTBD);
                if (CmmVariable.SysConfig.LangCode == "VN")
                {
                    _switchLanguage.Checked = false;
                }
                else
                {
                    _switchLanguage.Checked = true;
                }

                SetData();
                SetAvata();
                _imgSignOut.Click += Click_SignOut;
                _tvSignout.Click += Click_SignOut;
                _imgHome.Click += Click_Home;
                _tvHome.Click += Click_Home;
                _tvVTBD.Click += Click_VTBD;
                _tvVTBDCount.Click += Click_VTBD;
                _imgVTBD.Click += Click_VTBD;
                _tvVDT.Click += Click_VDT;
                _tvVDTCount.Click += Click_VDT;
                _imgVDT.Click += Click_VDT;
                _switchLanguage.CheckedChange += Change_Language;
                MinionAction.RefreshFragmentLeftMenu += Update_LeftMenu;
            }
            else
            {
                SetData();
                SetAvata();
            }
            SetViewByLanguage();
            return _rootView;
        }

        private void Update_LeftMenu(object sender, EventArgs e)
        {
            try
            {
                SetData();
                SetAvata();
                Update_Navigation();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - LeftMenuFragment - Update_LeftMenu - Error: " + ex.Message);
#endif
            }
        }


        #region Event
        private void SetViewByLanguage()
        {
            if (CmmVariable.SysConfig.LangCode == "VN")
            {
                _tvHome.Text = "Trang chủ";
                _tvVDT.Text = "Đến tôi";
                _tvVTBD.Text = "Tôi bắt đầu";
                _tvApplabel.Text = "Thông tin ứng dụng";
                _tvLanguage.Text = "Tiếng Việt";
                _tvSignout.Text = "Đăng xuất";
            }
            else
            {
                _tvHome.Text = "Home";
                _tvVDT.Text = "To me";
                _tvVTBD.Text = "From me";
                _tvApplabel.Text = "About";
                _tvLanguage.Text = "English";
                _tvSignout.Text = "Logout";
            }
        }
        private void Click_Home(object sender, EventArgs e)
        {
            try
            {
                _viewHome.SetBackgroundColor(Resources.GetColor(Resource.Color.clViolet));
                _viewVDT.SetBackgroundColor(Resources.GetColor(Resource.Color.clWhite));
                _viewVTBD.SetBackgroundColor(Resources.GetColor(Resource.Color.clWhite));
                SetView_Selected(_tvHome, _imgHome);
                SetView_NotSelected(_tvVDT, _imgVDT);
                SetView_NotSelected(_tvVTBD, _imgVTBD);
                _mainAct.CloseDawer();
                var tam = _mainAct.GetCurrentFragmentTag();
                if (tam == "FragmentHomePage")
                {

                }
                else
                {
                    _mainAct.HideFragment("FragmentHomePage");
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - LeftMenuFragment - Click_Home - Error: " + ex.Message);
#endif
            }
        }
        private void Click_YCXL(object sender, EventArgs e)
        {
            try
            {
                _viewHome.SetBackgroundColor(Resources.GetColor(Resource.Color.clWhite));
                _viewVDT.SetBackgroundColor(Resources.GetColor(Resource.Color.clWhite));
                _viewVTBD.SetBackgroundColor(Resources.GetColor(Resource.Color.clViolet));
                SetView_NotSelected(_tvHome, _imgHome);
                SetView_Selected(_tvYcxl, _imgYcxl);
                SetView_NotSelected(_tvVDT, _imgVDT);
                SetView_NotSelected(_tvVTBD, _imgVTBD);
                _mainAct.CloseDawer();
                var tam = _mainAct.GetCurrentFragmentTag();
                if (tam == "FragmentHomePage")
                {

                }
                else
                {
                    FragmentHomePage homePage = new FragmentHomePage(true, false);
                    _mainAct.ShowFragment(FragmentManager, homePage, "FragmentHomePage", 1);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - LeftMenuFragment - Click_YCXL - Error: " + ex.Message);
#endif
            }
        }
        private void Click_VDT(object sender, EventArgs e)
        {
            try
            {
                _viewHome.SetBackgroundColor(Resources.GetColor(Resource.Color.clWhite));
                _viewVDT.SetBackgroundColor(Resources.GetColor(Resource.Color.clViolet));
                _viewVTBD.SetBackgroundColor(Resources.GetColor(Resource.Color.clWhite));
                SetView_NotSelected(_tvHome, _imgHome);
                SetView_Selected(_tvVDT, _imgVDT);
                SetView_NotSelected(_tvVTBD, _imgVTBD);
                _mainAct.CloseDawer();
                var tam = _mainAct.GetCurrentFragmentTag();
                if (tam == "FragmentListWorkflow")
                {
                    MinionAction.ViewListWorkflow(null, new MinionAction.ChangeViewListWorkflow(1));
                }
                else
                {
                    FragmentListWorkflow listWorkflow = new FragmentListWorkflow("VDT",null);
                    _mainAct.ShowFragment(FragmentManager, listWorkflow, "FragmentListWorkflow", 1);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - LeftMenuFragment - Click_VDT - Error: " + ex.Message);
#endif
            }
        }
        private void Click_VTBD(object sender, EventArgs e)
        {
            try
            {
                _viewHome.SetBackgroundColor(Resources.GetColor(Resource.Color.clWhite));
                _viewVDT.SetBackgroundColor(Resources.GetColor(Resource.Color.clWhite));
                _viewVTBD.SetBackgroundColor(Resources.GetColor(Resource.Color.clViolet));
                SetView_NotSelected(_tvHome, _imgHome);
                SetView_NotSelected(_tvYcxl, _imgYcxl);
                SetView_NotSelected(_tvVDT, _imgVDT);
                SetView_Selected(_tvVTBD, _imgVTBD);
                _mainAct.CloseDawer();
                var tam = _mainAct.GetCurrentFragmentTag();
                if (tam == "FragmentListWorkflow")
                {
                    MinionAction.ViewListWorkflow(null, new MinionAction.ChangeViewListWorkflow(2));
                }
                else
                {
                    FragmentListWorkflow listWorkflow = new FragmentListWorkflow("VTBD",null);
                    _mainAct.ShowFragment(FragmentManager, listWorkflow, "FragmentListWorkflow", 1);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - LeftMenuFragment - Click_VTBD - Error: " + ex.Message);
#endif
            }
        }
        private void Click_SignOut(object sender, EventArgs e)
        {
            try
            {
                _imgSignOut.Enabled = false;
                _tvSignout.Enabled = false;
                if (CmmVariable.SysConfig.LangCode == "VN")
                {
                    Android.Support.V7.App.AlertDialog.Builder builderMess = new Android.Support.V7.App.AlertDialog.Builder(_rootView.Context);
                    builderMess.SetTitle("BPMOP");
                    builderMess.SetCancelable(false);
                    builderMess.SetMessage(CmmFunction.GetTitle("K_Mess_Logout", "Bạn có muốn đăng xuất khỏi tài khoản?"));
                    builderMess.SetNegativeButton(CmmFunction.GetTitle("K_Agree", "Đồng ý"), (senderAlert, args) =>
                    {
                        _imgSignOut.Enabled = true;
                        _tvSignout.Enabled = true;
                        _imgHome.SetColorFilter(Resources.GetColor(Resource.Color.clViolet));
                        _imgVTBD.SetColorFilter(null);
                        _imgVDT.SetColorFilter(null);
                        _imgYcxl.SetColorFilter(null);
                        _tvHome.SetTextColor(Resources.GetColor(Resource.Color.clViolet));
                        _tvVTBD.SetTextColor(Resources.GetColor(Resource.Color.clBlack));
                        _tvVDT.SetTextColor(Resources.GetColor(Resource.Color.clBlack));
                        _tvYcxl.SetTextColor(Resources.GetColor(Resource.Color.clBlack));
                        _mainAct.CloseDawer();
                        _mainAct.SignOut();
                        builderMess.Dispose();
                    });
                    builderMess.SetPositiveButton(CmmFunction.GetTitle("K_Reject", "Hủy"), (senderAlert, args) =>
                    {
                        _imgSignOut.Enabled = true;
                        _tvSignout.Enabled = true;
                        builderMess.Dispose();
                    });
                    builderMess.Show();
                }
                else
                {
                    Android.Support.V7.App.AlertDialog.Builder builderMess = new Android.Support.V7.App.AlertDialog.Builder(_rootView.Context);
                    builderMess.SetTitle("BPMOP");
                    builderMess.SetCancelable(false);
                    builderMess.SetMessage(CmmFunction.GetTitle("K_Mess_Logout", "Do you want to log out of your account?"));
                    builderMess.SetNegativeButton(CmmFunction.GetTitle("K_Agree", "Agree"), (senderAlert, args) =>
                    {
                        _imgSignOut.Enabled = true;
                        _tvSignout.Enabled = true;
                        _imgHome.SetColorFilter(Resources.GetColor(Resource.Color.clViolet));
                        _imgVTBD.SetColorFilter(null);
                        _imgVDT.SetColorFilter(null);
                        _imgYcxl.SetColorFilter(null);
                        _tvHome.SetTextColor(Resources.GetColor(Resource.Color.clViolet));
                        _tvVTBD.SetTextColor(Resources.GetColor(Resource.Color.clBlack));
                        _tvVTBDCount.SetTextColor(Resources.GetColor(Resource.Color.clWhite));
                        _tvVDT.SetTextColor(Resources.GetColor(Resource.Color.clBlack));
                        _tvVDTCount.SetTextColor(Resources.GetColor(Resource.Color.clWhite));
                        _tvYcxl.SetTextColor(Resources.GetColor(Resource.Color.clBlack));
                        _tvYcxlCount.SetTextColor(Resources.GetColor(Resource.Color.clWhite));
                        _mainAct.CloseDawer();
                        _mainAct.SignOut();
                        builderMess.Dispose();
                    });
                    builderMess.SetPositiveButton(CmmFunction.GetTitle("K_Reject", "Reject"), (senderAlert, args) =>
                    {
                        _imgSignOut.Enabled = true;
                        _tvSignout.Enabled = true;
                        builderMess.Dispose();
                    });
                    builderMess.Show();
                }
            }
            catch (Exception ex)
            {
                _imgSignOut.Enabled = true;
                _tvSignout.Enabled = true;
#if DEBUG
                Console.WriteLine("Author: khoahd - LeftMenuFragment - Click_SignOut - Error: " + ex.Message);
#endif
            }
        }
        private void SetView_Selected(TextView _tv, ImageView _img)
        {
            try
            {
                if (_tv != null && _img != null)
                {
                    _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clViolet)));
                    _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clViolet)));
                    _tv.SetTypeface(_tv.Typeface, TypefaceStyle.Bold);
                    _img.SetColorFilter(Resources.GetColor(Resource.Color.clViolet));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - LeftMenuFragment - SetView_Selected - Error: " + ex.Message);
#endif
            }
        }
        private void SetView_NotSelected(TextView _tv, ImageView _img)
        {
            try
            {
                if (_tv != null && _img != null)
                {
                    _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
                    _tv.SetTypeface(_tv.Typeface, TypefaceStyle.Normal);
                    _img.SetColorFilter(Resources.GetColor(Resource.Color.clBlack));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - LeftMenuFragment - SetView_NotSelected - Error: " + ex.Message);
#endif
            }
        }
        #endregion

        #region Data
        private async void SetAvata()
        {
            try
            {
                string url = CmmVariable.M_Domain + CmmVariable.SysConfig.AvatarPath;
                ProviderBase pUser = new ProviderBase();
                bool result;
                if (!File.Exists(CmmVariable.M_Avatar))
                {
                    await Task.Run(() =>
                    {
                        result = pUser.DownloadFile(url, CmmVariable.M_Avatar, CmmVariable.M_AuthenticatedHttpClient);
                        if (result)
                        {
                            Bitmap myBitmap = BitmapHelper.LoadAndResizeBitmap(CmmVariable.M_Avatar, 200, 200);
                            if (myBitmap != null)
                            {
                                _mainAct.RunOnUiThread(() =>
                                {
                                    _imgAvata.SetImageBitmap(myBitmap);
                                });
                            }
                        }
                    });
                }
                else
                {
                    Bitmap myBitmap = BitmapHelper.LoadAndResizeBitmap(CmmVariable.M_Avatar, 200, 200);
                    if (myBitmap != null)
                    {

                        _imgAvata.SetImageBitmap(myBitmap);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - LeftMenuFragment - SetAvata - Error: " + ex.Message);
#endif
            }
        }
        private void Change_Language(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                if (e.IsChecked)
                {
                    CmmVariable.SysConfig.LangCode = "EN";
                    load_Language();
                }
                else
                {
                    CmmVariable.SysConfig.LangCode = "VN";
                    load_Language();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - LeftMenuFragment - Change_Language - Error: " + ex.Message);
#endif
            }
        }
        private async void load_Language()
        {
            try
            {
                ProviderUser pApp = new ProviderUser();
                bool result;
                if (CmmVariable.SysConfig.LangCode == "VN")
                {
                    CmmDroidFunction.ShowProcessingDialog(CmmFunction.GetTitle("K_Mess_Wait", "Xin vui lòng đợi..."), _mainAct);
                }
                else
                {
                    CmmDroidFunction.ShowProcessingDialog(CmmFunction.GetTitle("K_Mess_Wait", "Please wait a moment"), _mainAct);
                }
                await Task.Run(() =>
                {

                    if (File.Exists(CmmVariable.M_DataLangPath))
                    {
                        result = pApp.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                    }
                    else
                    {
                        result = pApp.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                    }
                    if (result)
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.HideProcessingDialog();
                            CmmFunction.WriteSetting();
                            CmmEvent.UpdateLangComplete_Performence(null, null);
                            SetViewByLanguage();

                        });
                    }
                    else
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.HideProcessingDialog();
                            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(_mainAct);
                            alert.SetTitle("Eurowindow");
                            if (CmmVariable.SysConfig.LangCode == "VN")
                            {
                                alert.SetMessage(CmmFunction.GetTitle("K_Mess_ActionFalse", "Thao tác không thực hiện được. Xin vui lòng thử lại!"));
                                alert.SetNegativeButton(CmmFunction.GetTitle("K_Agree", "Đồng ý"), (senderAlert, args) =>
                                {
                                    alert.Dispose();
                                });
                            }
                            else
                            {
                                alert.SetMessage(CmmFunction.GetTitle("K_Mess_ActionFalse", "Your action failed. Please try again!"));
                                alert.SetNegativeButton(CmmFunction.GetTitle("K_Agree", "Agree"), (senderAlert, args) =>
                                {
                                    alert.Dispose();
                                });
                            }

                            Dialog dialog = alert.Create();
                            dialog.SetCanceledOnTouchOutside(false);
                            dialog.Show();
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                _mainAct.RunOnUiThread(() =>
                {
                    CmmDroidFunction.HideProcessingDialog();
                });
#if DEBUG
                Console.WriteLine("Author: khoahd - LeftMenuFragment - load_Language - Error: " + ex.Message);
#endif
            }
        }
        private void Update_Data(object sender, EventArgs e)
        {
            SetData();
            SetAvata();
        }
        /// <summary>
        /// Update lại View khi click vào Bottom Navigation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update_Navigation()
        {
            if (MinionAction._FlagNavigation == 1) // Home
            {
                _viewHome.SetBackgroundColor(Resources.GetColor(Resource.Color.clViolet));
                _viewVDT.SetBackgroundColor(Resources.GetColor(Resource.Color.clWhite));
                _viewVTBD.SetBackgroundColor(Resources.GetColor(Resource.Color.clWhite));
                SetView_Selected(_tvHome, _imgHome);
                SetView_NotSelected(_tvVDT, _imgVDT);
                SetView_NotSelected(_tvVTBD, _imgVTBD);
                // Perform Click - chuyển trang
                Click_Home(null, null);
            }
            else if (MinionAction._FlagNavigation == 2) // VDT
            {
                _viewHome.SetBackgroundColor(Resources.GetColor(Resource.Color.clWhite));
                _viewVDT.SetBackgroundColor(Resources.GetColor(Resource.Color.clViolet));
                _viewVTBD.SetBackgroundColor(Resources.GetColor(Resource.Color.clWhite));
                SetView_NotSelected(_tvHome, _imgHome);
                SetView_Selected(_tvVDT, _imgVDT);
                SetView_NotSelected(_tvVTBD, _imgVTBD);
                // Perform Click - chuyển trang
                Click_VDT(null, null);
            }
            else if (MinionAction._FlagNavigation == 3) // VTBD
            {
                _viewHome.SetBackgroundColor(Resources.GetColor(Resource.Color.clWhite));
                _viewVDT.SetBackgroundColor(Resources.GetColor(Resource.Color.clWhite));
                _viewVTBD.SetBackgroundColor(Resources.GetColor(Resource.Color.clViolet));
                SetView_NotSelected(_tvHome, _imgHome);
                SetView_NotSelected(_tvYcxl, _imgYcxl);
                SetView_NotSelected(_tvVDT, _imgVDT);
                SetView_Selected(_tvVTBD, _imgVTBD);
                // Perform Click - chuyển trang
                Click_VTBD(null, null);
            }

        }
        private void SetData()
        {
            try
            {
                GetCurentFragment();
                _tvName.Text = CmmVariable.SysConfig.Title;
                _tvEmail.Text = CmmVariable.SysConfig.Email;
                var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);

                //string queryVDT = string.Format(@"SELECT Count(*) as count FROM BeanWorkflowItem WHERE CreatedBy = '{0}' ", CmmVariable.SysConfig.LoginName);
                string queryVDT = string.Format(@"SELECT Count(*) as count FROM BeanNotify ORDER BY Created");

                var lstVDT = conn.Query<CountNum>(queryVDT);
                if (lstVDT != null)
                {
                    if (lstVDT[0].Count > 99)
                    {
                        _tvVDTCount.Text = "99+";
                    }
                    else
                    {
                        _tvVDTCount.Text = lstVDT[0].Count.ToString();
                    }
                }
                else
                {
                    _tvVDTCount.Text = "0";
                }
                //string queryVTBD = string.Format(@"SELECT Count(*) as count FROM BeanWorkflowItem WHERE CreatedBy <> '{0}' ", CmmVariable.SysConfig.LoginName);
                string queryVTBD = string.Format(@"SELECT Count(*) as count FROM BeanWorkflowItem WHERE CreatedBy = '{0}' ", CmmVariable.SysConfig.LoginName);
                var lstVTBD = conn.Query<CountNum>(queryVTBD);
                if (lstVTBD != null)
                {
                    if (lstVTBD[0].Count > 99)
                    {
                        _tvVTBDCount.Text = "99+";
                    }
                    else
                    {
                        _tvVTBDCount.Text = lstVTBD[0].Count.ToString();
                    }
                }
                else
                {
                    _tvVTBDCount.Text = "0";
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - LeftMenuFragment - SetData - Error: " + ex.Message);
#endif
            }
        }
        private void GetCurentFragment()
        {
            try
            {
                string tag = _mainAct.GetCurrentFragmentTag();
                if (!string.IsNullOrEmpty(tag) && tag == "FragmentHomePage")
                {
                    _imgHome.SetColorFilter(Resources.GetColor(Resource.Color.clViolet));
                    _tvHome.SetTextColor(Resources.GetColor(Resource.Color.clViolet));
                    _tvVTBD.SetTextColor(Resources.GetColor(Resource.Color.clBlack));
                    _tvVDT.SetTextColor(Resources.GetColor(Resource.Color.clBlack));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - LeftMenuFragment - GetCurentFragment - Error: " + ex.Message);
#endif
            }
        }

        #endregion
    }
}