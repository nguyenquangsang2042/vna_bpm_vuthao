using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentQRCode : CustomBaseFragment, ISurfaceHolderCallback, Detector.IProcessor
    {
        private MainActivity _mainAct;
        private View _rootView;
        private ImageView _imgBack;
        private SurfaceView _surfaceCamera;

        private BarcodeDetector _barcodeDetector;
        private CameraSource _cameraSource;
        private bool CheckQRCode = true;

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
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewQRCode_Back);
                _surfaceCamera = _rootView.FindViewById<SurfaceView>(Resource.Id.surface_ViewQRCode_Camera);
                _imgBack.Click += Click_imgBack;

                SetData();
            }
            else
            {

            }
            return _rootView;
        }

        private void SetData()
        {
            try
            {
                _barcodeDetector = new BarcodeDetector.Builder(_mainAct).SetBarcodeFormats(BarcodeFormat.QrCode).Build();
                _cameraSource = new CameraSource.Builder(_mainAct, _barcodeDetector)
                    .SetAutoFocusEnabled(true)
                    .SetRequestedPreviewSize(_mainAct.Resources.DisplayMetrics.WidthPixels, _mainAct.Resources.DisplayMetrics.HeightPixels)
                    .SetRequestedFps(30.0f)
                    .Build();
                _surfaceCamera.Holder.AddCallback(this);
                _barcodeDetector.SetProcessor(this);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
        }

        public override void OnDestroyView()
        {

        }

        #region Event
        private void Click_imgBack(object sender, EventArgs e)
        {
            try
            {
                _mainAct.HideFragment();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgBack", ex);
#endif
            }
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
        }
        public void SurfaceCreated(ISurfaceHolder holder)
        {
            if (ActivityCompat.CheckSelfPermission(_rootView.Context, Manifest.Permission.Camera) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(_mainAct, new[]
                {
                    Manifest.Permission.Camera
                }, CmmDroidVariable.M_MyPermissionsRequestReadExternalStorage);
                return;
            }
            try
            {
                _cameraSource.Start(_surfaceCamera.Holder);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("CheckQRCodeFragment - SurfaceCreated - Error : " + ex);
#endif
            }
        }
        public void SurfaceDestroyed(ISurfaceHolder holder)
        { 
        }
        public void ReceiveDetections(Detector.Detections detections)
        {
           /* try
            {
                SparseArray qrcodes = detections.DetectedItems;
                if (qrcodes.Size() != 0 && CheckQRCode)
                {

                    CheckQRCode = false;
                    string json = ((Barcode)qrcodes.ValueAt(0)).RawValue;
                    if (string.IsNullOrEmpty(json)) return;
                    Uri myUri = new Uri(json);
                    //string ListId = HttpUtility.ParseQueryString(myUri.Query).Get("listid");
                    //string ItemId = HttpUtility.ParseQueryString(myUri.Query).Get("itemid");
                    //if (!string.IsNullOrEmpty(ListId) && !string.IsNullOrEmpty(ItemId))
                    //{
                    //    GetWorkflowItemByQRCode(ListId, ItemId);
                    //}
                    //else
                    //{

                    //    _imgBack.Post(() =>
                    //    {
                    //        _cameraSource.Stop();
                    //        Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(_mainAct);
                    //        alert.SetTitle("Thông báo");
                    //        alert.SetMessage("QRCode không hợp lệ. Vui lòng thử lại.");
                    //        alert.SetPositiveButton("Đóng",
                    //            (senderAlert, args) =>
                    //            {
                    //                CheckQRCode = true;
                    //                _cameraSource.Start(_surfaceCamera.Holder);
                    //                alert.Dispose();
                    //            });
                    //        Dialog dialog = alert.Create();
                    //        dialog.SetCanceledOnTouchOutside(false);
                    //        dialog.Show();
                    //    });

                    //}
                }
            }
            catch (Exception ex)
            {

                _imgBack.Post(() =>
                {
                    _cameraSource.Stop();
                    //Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(_mainAct);
                    //alert.SetTitle("Thông báo");
                    //alert.SetMessage("QRCode không hợp lệ. Vui lòng thử lại.");
                    //alert.SetPositiveButton("Đóng",
                    //(senderAlert, args) =>
                    //{
                    //    CheckQRCode = true;
                    //    _cameraSource.Start(_surfaceCamera.Holder);
                    //    alert.Dispose();
                    //});
                    //Dialog dialog = alert.Create();
                    //dialog.SetCanceledOnTouchOutside(false);
                    //dialog.Show();
                });
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ReceiveDetections", ex);
#endif
            }*/
        }
        public void Release()
        {
            
        }
        #endregion

        #region Data

        private void GetWorkflowItemByQRCode(string listId, string itemId)
        {
            try
            {
                //var conn = new SQLiteConnection(CmmVariable.M_DataPath);
                //string query = @"SELECT * FROM BeanWorkflowItem Where ItemID = ? AND ListId = ? ORDER BY Created DESC";
                //var lstWorkflow = conn.Query<BeanWorkflowItem>(query, itemId.ToLower(), listId.ToLower());
                //if (lstWorkflow != null && lstWorkflow.Count > 0)
                //{
                //    DetailWorkFlowFragment detailWorkFlow = new DetailWorkFlowFragment(lstWorkflow[0], null);
                //    _mainAct.ShowFragment(FragmentManager, detailWorkFlow, "DetailWorkFlow", 0);
                //}
                //else
                //{
                //    await Task.Run(() =>
                //    {
                //        _mainAct.RunOnUiThread(() =>
                //        {
                //            CmmDroidFunction.ShowProcessingDialog("Xin vui lòng đợi...", _mainAct, false);
                //        });
                //        ProviderBase pBase = new ProviderBase();
                //        pBase.UpdateMasterData<BeanWorkflowItem>(null, true);
                //        _mainAct.RunOnUiThread(() =>
                //        {
                //            CmmDroidFunction.HideProcessingDialog();
                //        });
                //    });

                //    lstWorkflow = conn.Query<BeanWorkflowItem>(query);
                //    if (lstWorkflow != null && lstWorkflow.Count > 0)
                //    {
                //        DetailWorkFlowFragment detailWorkFlow = new DetailWorkFlowFragment(lstWorkflow[0], null);
                //        _mainAct.ShowFragment(FragmentManager, detailWorkFlow, "DetailWorkFlow", 0);
                //    }
                //    else
                //    {
                //        _mainAct.RunOnUiThread(() =>
                //        {
                //            CheckQRCode = true;
                //            if (_flagDialog == false)
                //            {
                //                _flagDialog = true;
                //                //CmmDroidFunction.ShowAlertDialog(_mainAct, "Thông báo", "Thao tác không thực hiện được, vui lòng thử lại sau.", null, "Đóng");
                //                ShowAlertDialog_QR(_mainAct, "Thông báo", "Không tìm được thông tin phiếu.", null, "Đóng");
                //            }
                //        });
                //    }
                //}
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("CheckQRCodeFragment - GetWorkflowItemByQRCode - Error : " + ex);
#endif
            }
        }

        #endregion
    }
}