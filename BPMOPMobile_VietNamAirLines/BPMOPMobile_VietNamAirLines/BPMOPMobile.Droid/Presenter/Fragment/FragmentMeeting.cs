using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using Com.Github.Barteksc.Pdfviewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentMeeting : CustomBaseFragment
    {
        private MainActivity mainAct;
        private View rootView;
        private ImageButton imgBack;
        private PDFView pdfView;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mainAct = (MainActivity)this.Activity;
            mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            if (rootView == null)
            {
                rootView = inflater.Inflate(Resource.Layout.layout_meeting_activity, null);
                imgBack = rootView.FindViewById<ImageButton>(Resource.Id.btnBackMeeting);
                pdfView = rootView.FindViewById<PDFView>(Resource.Id.pdfVIewMeeting);
                imgBack.Click += ImgBack_Click;
                CmmDroidFunction.ShowProcessingDialog(mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);
                Action action = new Action(() =>
                {
                    getPDf();
                });
                new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime + 500);

            }
            return rootView;
        }

        private void ImgBack_Click(object sender, EventArgs e)
        {
            mainAct.HideFragment();
        }
        private void getPDf()
        {
            try
            {
                string path = CmmDroidFunction.DownloadAndGetPathFile(mainAct, rootView.Context, CmmVariable.M_Domain + "/workflow/Shared%20Documents/Lichtuan.PDF");
                if (File.Exists(path))
                {
                    pdfView.FromUri(Android.Net.Uri.FromFile(new Java.IO.File(path))).Load();
                }
                mainAct.RunOnUiThread(() =>
                {
                    CmmDroidFunction.HideProcessingDialog();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                mainAct.RunOnUiThread(() =>
                {
                    CmmDroidFunction.HideProcessingDialog();
                });
            }
        }

    }
}