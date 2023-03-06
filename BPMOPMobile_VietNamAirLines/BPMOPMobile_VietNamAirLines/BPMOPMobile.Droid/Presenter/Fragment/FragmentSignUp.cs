using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentSignUp : CustomBaseFragment, IDialogInterfaceOnClickListener
    {
        private MainActivity _mainAct;
        private View _rootView;
        private int positionGotoLayout;
        private ImageButton btnBack;
        private CardView btnAccept;
        private EditText edtEmailCompany;
        private Button txtLogin;
        private string stringRegisterEror = "Email của bạn không hợp lệ hoặc không được liên kết trên hệ thống, bạn chỉ có thể đăng ký kích hoạt với thông tin doanh nghiệp đã liên kết với hệ thống.";
        private string stringRegisterErorEN = "Your email is invalid or not linked on the system, you can only register for activation with business information associated with the system.";
        private bool isRegister = false;
        public FragmentSignUp(int positionGotoLayout)
        {
            //1 -> startviewLayout
            //0-> FirstLoginLayout
            this.positionGotoLayout = positionGotoLayout;
        }
        public void OnClick(IDialogInterface dialog, int which)
        {
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _rootView = inflater.Inflate(Resource.Layout.Layout_SignUp, null);
            _mainAct = (MainActivity)this.Activity;
            if (_mainAct._drawerLayout != null)
            {
                _mainAct._drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            }
            btnBack = _rootView.FindViewById<ImageButton>(Resource.Id.btnBack);
            btnAccept = _rootView.FindViewById<CardView>(Resource.Id.btnAccept);
            edtEmailCompany = _rootView.FindViewById<EditText>(Resource.Id.edt_Email_Company_Signup);
            txtLogin = _rootView.FindViewById<Button>(Resource.Id.txtLogin);
            btnBack.Click += BtnBack_Click;
            btnAccept.Click += BtnAccept_Click;
            txtLogin.Click += TxtLogin_Click;
            return _rootView;
        }
        private void ChangeView(int positionChangview, bool isregisterCliclk = false)
        {
           
            if (positionChangview == 1)
                if (isregisterCliclk)
                {
                    _mainAct.HideFragment();
                    _mainAct.HideFragment();
                    FragmentStartView firstLogin = new FragmentStartView(true, true);
                    _mainAct.ShowFragment(FragmentManager, firstLogin, "FragmentStartView", 1);
                }
                else
                {
                    if (FragmentManager.BackStackEntryCount == 2)
                    {
                        _mainAct.HideFragment();
                    }
                    else if (FragmentManager.BackStackEntryCount == 3)
                    {
                        _mainAct.HideFragment();
                        _mainAct.HideFragment();
                        FragmentStartView firstLogin = new FragmentStartView(true, false);
                        _mainAct.ShowFragment(FragmentManager, firstLogin, "FragmentStartView", 1);
                    }
                }
            else if (isregisterCliclk)
            {
                _mainAct.HideFragment();
                FragmentStartView firstLogin = new FragmentStartView(true, true);
                _mainAct.ShowFragment(FragmentManager, firstLogin, "FragmentStartView", 1);

            }
            else
            {
                _mainAct.HideFragment();
                FragmentStartView firstLogin = new FragmentStartView(true, false);
                _mainAct.ShowFragment(FragmentManager, firstLogin, "FragmentStartView", 1);
            }
            _mainAct.RunOnUiThread(() =>
            {
                CmmDroidFunction.HideProcessingDialog();
            });
        }
        private void TxtLogin_Click(object sender, EventArgs e)
        {
            ChangeView(positionGotoLayout);
        }

        private async void BtnAccept_Click(object sender, EventArgs e)
        {
            _mainAct.RunOnUiThread(() =>
            {
                CmmDroidFunction.ShowProcessingDialog(_mainAct, "Vui lòng đợi...", "Please wait...", false);
            });
            await Task.Run(() =>
            {
                if (edtEmailCompany.Text != null && edtEmailCompany.Text != string.Empty)
                {
                    if (edtEmailCompany.Text.Contains("@"))
                    {
                        bool isRegister = CmmFunction.Register(edtEmailCompany.Text);
                        //call API -> đợi api trả về thì hiện thông báo
                        if (isRegister)
                        {
                            ChangeView(positionGotoLayout, true);
                        }
                        else
                        {
                            _mainAct.RunOnUiThread(() => { CmmDroidFunction.ShowAlertDialog(_mainAct, stringRegisterEror, stringRegisterErorEN); });
                        }
                    }
                    else
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.HideProcessingDialog();
                        });
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.ShowAlertDialog(_mainAct, "Vui lòng nhập email doanh nghiệp đã đăng ký", "Please enter your registered business email");
                        });
                    }    
                }
                else
                {
                    _mainAct.RunOnUiThread(() =>
                    {
                        CmmDroidFunction.HideProcessingDialog();
                    });
                    _mainAct.RunOnUiThread(() =>
                    {
                        CmmDroidFunction.ShowAlertDialog(_mainAct, "Vui lòng nhập email doanh nghiệp đã đăng ký", "Please enter your registered business email");
                    });
                }
            });
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            ChangeView(1);
        }
    }
}