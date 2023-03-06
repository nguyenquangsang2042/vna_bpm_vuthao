using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using Xamarin.Essentials;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentConfirmOTP : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private View _rootView;
        private TextView _tvTitle, _tvInput, _tvCharacter1, _tvCharacter2, _tvCharacter3, _tvCharacter4, _tvCharacter5, _tvCharacter6, _tvOtherAccount, _tvClearText,
            _tvInputNumber1, _tvInputNumber2, _tvInputNumber3, _tvInputNumber4, _tvInputNumber5, _tvInputNumber6, _tvInputNumber7, _tvInputNumber8, _tvInputNumber9, _tvInputNumber0;
        private string _loginName, _loginPass;
        private bool _isFirstLogin = false;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _rootView = inflater.Inflate(Resource.Layout.ViewConfirmOTP, null);
            _mainAct = (MainActivity)this.Activity;
            if (_mainAct._drawerLayout != null)
            {
                _mainAct._drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            }

            if (_rootView != null)
            {
                _tvTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_Title);
                _tvInput = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_Input);
                _tvCharacter1 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_Character1);
                _tvCharacter2 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_Character2);
                _tvCharacter3 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_Character3);
                _tvCharacter4 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_Character4);
                _tvCharacter5 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_Character5);
                _tvCharacter6 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_Character6);
                _tvOtherAccount = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_OtherAccount);
                _tvClearText = _rootView.FindViewById<TextView>(Resource.Id.ln_ViewConfirmOTP_ClearText);
                _tvInputNumber1 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_InputNumber_1);
                _tvInputNumber2 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_InputNumber_2);
                _tvInputNumber3 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_InputNumber_3);
                _tvInputNumber4 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_InputNumber_4);
                _tvInputNumber5 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_InputNumber_5);
                _tvInputNumber6 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_InputNumber_6);
                _tvInputNumber7 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_InputNumber_7);
                _tvInputNumber8 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_InputNumber_8);
                _tvInputNumber9 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_InputNumber_9);
                _tvInputNumber0 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewConfirmOTP_InputNumber_0);

                _tvClearText.Click += Click_tvClearText;
                _tvOtherAccount.Click += Click_tvOtherAccount;
                _tvInputNumber1.Click += Click_tvInputNumber1;
                _tvInputNumber2.Click += Click_tvInputNumber2;
                _tvInputNumber3.Click += Click_tvInputNumber3;
                _tvInputNumber4.Click += Click_tvInputNumber4;
                _tvInputNumber5.Click += Click_tvInputNumber5;
                _tvInputNumber6.Click += Click_tvInputNumber6;
                _tvInputNumber7.Click += Click_tvInputNumber7;
                _tvInputNumber8.Click += Click_tvInputNumber8;
                _tvInputNumber9.Click += Click_tvInputNumber9;
                _tvInputNumber0.Click += Click_tvInputNumber0;
            }
            SetViewByLanguague();
            return _rootView;
        }

        public FragmentConfirmOTP(string _loginName, string _loginPass, bool _isFirstLogin)
        {
            this._loginName = _loginName;
            this._loginPass = _loginPass;
            this._isFirstLogin = _isFirstLogin;
        }

        #region Event
        private void SetViewByLanguague()
        {
            try
            {
                Click_tvClearText(null, null);
                _tvTitle.Text = CmmFunction.GetTitle("TEXT_TWOFACTOR", "Two-factor authentication"); // only English
                _tvInput.Text = CmmFunction.GetTitle("TEXT_TWOFACTOR_SIXNUM", "Enter 6 number from Authenticator Application"); // only English
                _tvOtherAccount.Text = CmmFunction.GetTitle("TEXT_TWOFACTOR_CHANGEACCOUNT", "Sign in with different user?"); // only English
                _tvClearText.Text = CmmFunction.GetTitle("TEXT_TWOFACTOR_RESET", "Reset"); // only English
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetView", ex);
#endif
            }
        }

        private void Click_tvClearText(object sender, EventArgs e)
        {
            try
            {
                _tvCharacter1.Text = "";
                _tvCharacter2.Text = "";
                _tvCharacter3.Text = "";
                _tvCharacter4.Text = "";
                _tvCharacter5.Text = "";
                _tvCharacter6.Text = "";
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvClearText", ex);
#endif
            }
        }

        private void Click_tvOtherAccount(object sender, EventArgs e)
        {
            try
            {
                _mainAct.SignOut();
                //_mainAct.HideFragment();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvClearText", ex);
#endif
            }
        }

        private void Click_tvInputNumber1(object sender, EventArgs e)
        {
            try
            {
                InsertCharacterOTP("1");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvInputNumber1", ex);
#endif
            }
        }

        private void Click_tvInputNumber2(object sender, EventArgs e)
        {
            try
            {
                InsertCharacterOTP("2");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvInputNumber2", ex);
#endif
            }
        }

        private void Click_tvInputNumber3(object sender, EventArgs e)
        {
            try
            {
                InsertCharacterOTP("3");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvInputNumber3", ex);
#endif
            }
        }

        private void Click_tvInputNumber4(object sender, EventArgs e)
        {
            try
            {
                InsertCharacterOTP("4");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvInputNumber4", ex);
#endif
            }
        }

        private void Click_tvInputNumber5(object sender, EventArgs e)
        {
            try
            {
                InsertCharacterOTP("5");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvInputNumber5", ex);
#endif
            }
        }

        private void Click_tvInputNumber6(object sender, EventArgs e)
        {
            try
            {
                InsertCharacterOTP("6");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvInputNumber6", ex);
#endif
            }
        }

        private void Click_tvInputNumber7(object sender, EventArgs e)
        {
            try
            {
                InsertCharacterOTP("7");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvInputNumber7", ex);
#endif
            }
        }

        private void Click_tvInputNumber8(object sender, EventArgs e)
        {
            try
            {
                InsertCharacterOTP("8");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvInputNumber8", ex);
#endif
            }
        }

        private void Click_tvInputNumber9(object sender, EventArgs e)
        {
            try
            {
                InsertCharacterOTP("9");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvInputNumber9", ex);
#endif
            }
        }

        private void Click_tvInputNumber0(object sender, EventArgs e)
        {
            try
            {
                InsertCharacterOTP("0");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvInputNumber0", ex);
#endif
            }
        }
        #endregion

        #region Data
        private void InsertCharacterOTP(string _character)
        {
            try
            {
                if (String.IsNullOrEmpty(_character))
                {
                    return;
                }

                CmmDroidFunction.ShowVibrateEvent(0.2);

                if (String.IsNullOrEmpty(_tvCharacter1.Text))
                {
                    _tvCharacter1.Text = _character;
                }
                else if (String.IsNullOrEmpty(_tvCharacter2.Text))
                {
                    _tvCharacter2.Text = _character;
                }
                else if (String.IsNullOrEmpty(_tvCharacter3.Text))
                {
                    _tvCharacter3.Text = _character;
                }
                else if (String.IsNullOrEmpty(_tvCharacter4.Text))
                {
                    _tvCharacter4.Text = _character;
                }
                else if (String.IsNullOrEmpty(_tvCharacter5.Text))
                {
                    _tvCharacter5.Text = _character;
                }
                else if (String.IsNullOrEmpty(_tvCharacter6.Text))
                {
                    _tvCharacter6.Text = _character;
                    _tvCharacter6.Post(() =>
                    {
                        // Sau khi set view xong thì gọi hàm này
                        HandleConfirmOTP();
                    });
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "InsertCharacterOTP", ex);
#endif
            }
        }

        private async void HandleConfirmOTP()
        {
            try
            {
                // Only English
                CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_LOADING", "Please wait a minute..."), CmmFunction.GetTitle("TEXT_LOADING", "Please wait a minute..."), false);

                string _resultOTP = "";
                if (!String.IsNullOrEmpty(_tvCharacter1.Text) && !String.IsNullOrEmpty(_tvCharacter2.Text) && !String.IsNullOrEmpty(_tvCharacter3.Text) &&
                    !String.IsNullOrEmpty(_tvCharacter4.Text) && !String.IsNullOrEmpty(_tvCharacter5.Text) && !String.IsNullOrEmpty(_tvCharacter6.Text))
                {
                    _resultOTP = _tvCharacter1.Text + _tvCharacter2.Text + _tvCharacter3.Text + _tvCharacter4.Text + _tvCharacter5.Text + _tvCharacter6.Text;
                }
                else
                {
                    // Only English
                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_OTP_NOTVALID", "The OTP Code is not valid!"), CmmFunction.GetTitle("TEXT_OTP_NOTVALID", "The OTP Code is not valid!"),
                        "Alert", "Alert", "Close", "Close");

                    return;
                }

                if (!String.IsNullOrEmpty(_resultOTP) && _resultOTP.Length == 6) // Mã OTP hợp lệ -> xử lý
                {
                    await Task.Run(() =>
                    {
                        CmmEvent.ReloginRequest += CmmEvent_ReloginRequest;
                        string getCurrentUserUrl = CmmVariable.M_Domain.TrimEnd('/') + (new BeanUser().GetCurrentUserUrl());
                        if (CmmFunction.Login(getCurrentUserUrl, _loginName, _loginPass, true, 1, _resultOTP) != null)
                        {
                            _mainAct.RunOnUiThread(() =>
                            {
                                // Đợi Tick event
                            });
                        }
                        else
                        {

                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _mainAct.RunOnUiThread(() =>
                {
                    Click_tvClearText(null, null);
                    CmmDroidFunction.HideProcessingDialog();
                });
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "HandleConfirmOTP", ex);
#endif
            }
        }

        private void CmmEvent_ReloginRequest(object sender, CmmEvent.LoginEventArgs e)
        {
            try
            {
                if (e != null && e.IsSuccess) // Login bình thường
                {
                    _mainAct.RunOnUiThread(() =>
                    {
                        Click_tvClearText(null, null);
                    });
                    if (e.UserInfo != null)
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmVariable.SysConfig.VerifyOTP = e.VerifyOTP;
                            CmmVariable.SysConfig.AvatarPath = e.UserInfo.ImagePath;
                            CmmVariable.SysConfig.UserId = e.UserInfo.ID;
                            CmmVariable.SysConfig.Title = e.UserInfo.FullName;
                            CmmVariable.SysConfig.DisplayName = e.UserInfo.FullName;
                            CmmVariable.SysConfig.Email = e.UserInfo.Email;
                            CmmVariable.SysConfig.Department = e.UserInfo.Department;
                            CmmVariable.SysConfig.Address = e.UserInfo.Address;
                            CmmVariable.SysConfig.PositionID = e.UserInfo.PositionID;
                            CmmVariable.SysConfig.PositionTitle = e.UserInfo.PositionTitle;
                            CmmVariable.SysConfig.Mobile = e.UserInfo.Mobile;
                            CmmVariable.SysConfig.LoginName = _loginName;
                            CmmVariable.SysConfig.LoginPassword = _loginPass;
                            CmmVariable.SysConfig.SiteName = e.UserInfo.SiteName;
                            CmmVariable.SysConfig.AppConfigVersion = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;
                            CmmVariable.SysConfig.LangCode = e.UserInfo.Language.ToString();

                            if (String.IsNullOrEmpty(CmmVariable.SysConfig.LangCode)) // Ko có ngôn ngữ -> Lấy Default
                            {
                                if (File.Exists(CmmVariable.M_DataLangPath))
                                {
                                    ProviderUser proUser = new ProviderUser();
                                    string langCode = proUser.GetCurrentlangCode();
                                    if (!string.IsNullOrEmpty(langCode))
                                        CmmVariable.SysConfig.LangCode = langCode;
                                }
                            }
                            _mainAct.UpdateDBLanguage(CmmVariable.SysConfig.LangCode);

                            CmmFunction.WriteSetting(); // Lưu Setting
                            if (_isFirstLogin) // Đăng nhập lần đầu -> Login
                            {
                                GetDataFirstTimeLogin();
                            }
                            else // Đã đăng nhập trước đó -> Relogin
                            {
                                GetDataAutoLogin();
                            }
                        });
                    }
                }
                else if (e != null && e.IsSuccess == false) // Confirm OTP lỗi -> hiện lỗi ra
                {
                    _mainAct.RunOnUiThread(() =>
                    {
                        Click_tvClearText(null, null);
                        string _ErrMessage = e.ErrCode;
                        CmmDroidFunction.HideProcessingDialog();

                        CmmDroidFunction.ShowAlertDialog(_mainAct, _ErrMessage, _ErrMessage, "Alert", "Alert", "Close", "Close");
                    });
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "CmmEvent_ReloginRequest", ex);
#endif
            }

            CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
        }

        private async void GetDataFirstTimeLogin()
        {
            try
            {
                await Task.Run(() =>
                {
                    if (!File.Exists(CmmVariable.M_DataPath))
                    {
                        CmmFunction.InstanceDB(CmmVariable.M_DataPath);
                    }
                    else
                    {
                        File.Delete(CmmVariable.M_DataPath);
                        CmmFunction.InstanceDB(CmmVariable.M_DataPath);
                    }
                    ProviderBase pBase = new ProviderBase();
                    ProviderUser pApp = new ProviderUser();
                    if (File.Exists(CmmVariable.M_DataLangPath))
                    {
                        pApp.UpdateLangData(CmmVariable.SysConfig.LangCode, true, false);
                    }
                    else
                    {
                        pApp.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                    }
                    pBase.UpdateAllMasterData(false, CmmVariable.SysConfig.DataLimitDay, true);
                    pBase.UpdateAllDynamicData(false, CmmVariable.SysConfig.DataLimitDay, true);

                    _mainAct.RunOnUiThread(() =>
                    {
                        //_checkAutoLogin = false;
                        //FragmentHomePage homePage = new FragmentHomePage(true, _checkAutoLogin);

                        CmmDroidFunction.HideProcessingDialog();
                        FragmentHomePage homePage = new FragmentHomePage();
                        _mainAct.ShowFragment(FragmentManager, homePage, "FragmentHomePage", 1);
                    });

                });
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetDataFirstTimeLogin", ex);
#endif
            }
        }

        private async void GetDataAutoLogin()
        {
            try
            {
                await Task.Run(() =>
                {
                    ProviderUser pApp = new ProviderUser();
                    if (File.Exists(CmmVariable.M_DataLangPath))
                    {
                        pApp.UpdateLangData(CmmVariable.SysConfig.LangCode, true, false);
                    }
                    else
                    {
                        pApp.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                    }
                    ProviderBase pBase = new ProviderBase();
                    pBase.UpdateAllMasterData(true);
                    pBase.UpdateAllDynamicData(true);

                    _mainAct.RunOnUiThread(() =>
                    {
                        //GetNotifyKillApp();
                        //_checkAutoLogin = true;
                        //FragmentHomePage homePage = new FragmentHomePage(true, _checkAutoLogin);

                        CmmDroidFunction.HideProcessingDialog();
                        FragmentHomePage homePage = new FragmentHomePage();
                        _mainAct.ShowFragment(FragmentManager, homePage, "FragmentHomePage", 1);
                    });
                });
            }
            catch (Exception ex)
            {
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetDataAutoLogin", ex);
            }
        }
        #endregion
    }
}