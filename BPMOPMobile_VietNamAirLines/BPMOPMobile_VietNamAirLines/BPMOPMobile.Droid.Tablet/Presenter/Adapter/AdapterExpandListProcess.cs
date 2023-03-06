using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Controller;
using Refractored.Controls;

namespace BPMOPMobile.Droid.Tablet.Presenter.Adapter
{
    [Obsolete]
    class AdapterExpandListProcess : BaseExpandableListAdapter
    {
        MainActivity _mainAct;
        private Context _context;
        private List<SupTitleQtlc> _lst;
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        public AdapterExpandListProcess(Context context, List<SupTitleQtlc> list, MainActivity mainAct)
        {
            this._context = context;
            _lst = list;
            _mainAct = mainAct;
        }

        public override int GroupCount
        {
            get
            {
                return _lst.Count;
            }
        }
        public override bool HasStableIds
        {
            get
            {
                return false;
            }
        }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            List<BeanQuaTrinhLuanChuyen> lstObj = _lst[groupPosition].ListItemMenu;
            if (lstObj == null || lstObj.Count == 0)
                return 0;
            return lstObj.Count;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            LayoutInflater mInflater = LayoutInflater.From(_context);
            View rootView = mInflater.Inflate(Resource.Layout.ItemListProcessChild, null);
            TextView tvDate = rootView.FindViewById<TextView>(Resource.Id.tv_ItemListProcessChild_Date);
            TextView tvYkien = rootView.FindViewById<TextView>(Resource.Id.tv_ItemListProcessChild_YKien);
            TextView tvNguoixuly = rootView.FindViewById<TextView>(Resource.Id.tv_ItemListProcessChild_NguoiXuLy);
            TextView tvAction = rootView.FindViewById<TextView>(Resource.Id.tv_ItemListProcessChild_Action);
            View tvLine = rootView.FindViewById<View>(Resource.Id.tv_ItemListProcessChild_Line);
            TextView tvAvata = rootView.FindViewById<TextView>(Resource.Id.tv_ItemListProcessChild_Avata);
            CircleImageView imgAvata = rootView.FindViewById<CircleImageView>(Resource.Id.img_ItemListProcessChild_Avata);
            List<BeanQuaTrinhLuanChuyen> lstObj = _lst[groupPosition].ListItemMenu;
            if (lstObj != null && lstObj.Count > 0)
            {
                BeanQuaTrinhLuanChuyen beanQuaTrinhLuanChuyen = lstObj[childPosition];
                if (groupPosition == _lst.Count - 1)
                {
                    tvLine.Visibility = ViewStates.Invisible;
                }
                if (beanQuaTrinhLuanChuyen.CompletedDate.HasValue)
                {
                    tvDate.Text = beanQuaTrinhLuanChuyen.CompletedDate.Value.ToString("HH:mm, dd/MM/yyyy");
                }
                else
                {
                    tvDate.Text = "";
                }

                if (!string.IsNullOrEmpty(beanQuaTrinhLuanChuyen.Note))
                {
                    tvYkien.Text = beanQuaTrinhLuanChuyen.Note;
                }
                else
                {
                    tvYkien.Visibility = ViewStates.Gone;
                }

                if (!string.IsNullOrEmpty(beanQuaTrinhLuanChuyen.AssignedTo))
                {
                    tvNguoixuly.Text = beanQuaTrinhLuanChuyen.AssignedTo;
                }
                else
                {
                    tvNguoixuly.Visibility = ViewStates.Gone;
                }
                if (!string.IsNullOrEmpty(beanQuaTrinhLuanChuyen.SubmitAction))
                {
                    GradientDrawable _drawable = new GradientDrawable();
                    _drawable.SetStroke(2, new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));
                    _drawable.SetCornerRadius(5);
                    _drawable.SetShape(ShapeType.Rectangle);
                    _drawable.SetColor(CTRLHomePage.GetColorByAction(_mainAct, beanQuaTrinhLuanChuyen.SubmitAction, "VTBD"));
                    tvAction.Text = beanQuaTrinhLuanChuyen.SubmitAction;
                    tvAction.SetBackgroundDrawable(_drawable);
                }
                else
                {
                    tvAction.Visibility = ViewStates.Gone;
                }

                SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                if (!string.IsNullOrEmpty(beanQuaTrinhLuanChuyen.PersonEmail))
                {
                    string queryNotify = string.Format("SELECT * FROM BeanUser WHERE  Email = '{0}' ", beanQuaTrinhLuanChuyen.PersonEmail);
                    var lstUser = conn.Query<BeanUser>(queryNotify);
                    if (lstUser != null && lstUser.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(lstUser[0].ImagePath))
                        {
                            imgAvata.Visibility = ViewStates.Visible;
                            tvAvata.Visibility = ViewStates.Invisible;
                            SetAvata(lstUser[0].ImagePath, imgAvata);
                        }
                        else
                        {
                            imgAvata.Visibility = ViewStates.Invisible;
                            tvAvata.Visibility = ViewStates.Visible;
                            Random rnd = new Random();
                            var color = Color.Rgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                            tvAvata.BackgroundTintList = ColorStateList.ValueOf(color);
                            if (!string.IsNullOrEmpty(beanQuaTrinhLuanChuyen.PersonAccount))
                            {
                                var name = CmmFunction.GetNameFromLookupData(beanQuaTrinhLuanChuyen.PersonAccount);
                                if (name != null && name.Count > 0)
                                {
                                    tvAvata.Text = name[0].Substring(0, 1);
                                }
                            }
                        }
                    }
                    else
                    {
                        imgAvata.Visibility = ViewStates.Invisible;
                        tvAvata.Visibility = ViewStates.Visible;
                        Random rnd = new Random();
                        var color = Color.Rgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                        tvAvata.BackgroundTintList = ColorStateList.ValueOf(color);
                        if (!string.IsNullOrEmpty(beanQuaTrinhLuanChuyen.PersonAccount))
                        {
                            tvAvata.Text = beanQuaTrinhLuanChuyen.PersonAccount.Substring(0, 1);
                        }
                    }
                }
                else
                {
                    imgAvata.Visibility = ViewStates.Invisible;
                    tvAvata.Visibility = ViewStates.Visible;
                    Random rnd = new Random();
                    var color = Color.Rgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                    tvAvata.BackgroundTintList = ColorStateList.ValueOf(color);
                    if (!string.IsNullOrEmpty(beanQuaTrinhLuanChuyen.PersonAccount))
                    {
                        tvAvata.Text = beanQuaTrinhLuanChuyen.PersonAccount.Substring(0, 1);
                    }
                }
            }
            return rootView;
        }
        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return groupPosition;
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            View rootView = convertView;
            if (rootView == null)
            {
                LayoutInflater mInflater = LayoutInflater.From(_context);
                rootView = mInflater.Inflate(Resource.Layout.ItemListProcessGroup, null);
            }
            TextView txtTilteSupMenu = rootView.FindViewById<TextView>(Resource.Id.tv_ItemListProcessGroup_Title);
            TextView tvStatus = rootView.FindViewById<TextView>(Resource.Id.tv_ItemListProcessGroup_Status);
            ImageView img = rootView.FindViewById<ImageView>(Resource.Id.img_ItemListProcessGroup);
            BeanQuaTrinhLuanChuyen beanQuaTrinhLuanChuyen = _lst[groupPosition].ListItemMenu[0];
            if (!string.IsNullOrEmpty(beanQuaTrinhLuanChuyen.SubmitAction))
            {
                //img.SetImageResource(Resource.Mipmap.icon_complete30);
                //img.SetImageResource(Resource.Drawable.icon_check3);
                img.SetImageResource(Resource.Drawable.icon_circle);
                img.SetColorFilter(new Color(ContextCompat.GetColor(_context, Resource.Color.clGreen))); // tint green
            }
            else
            {
                //img.SetImageResource(Resource.Mipmap.icon_inprogress30);
                img.SetImageResource(Resource.Drawable.icon_circle);
                img.SetColorFilter(new Color(ContextCompat.GetColor(_context, Resource.Color.clOrange))); // tint orange
            }
            txtTilteSupMenu.Text = _lst[groupPosition].TitleName;
            tvStatus.Visibility = ViewStates.Gone;
            //if (!string.IsNullOrEmpty(beanQuaTrinhLuanChuyen.SubmitAction))
            //{
            //    tvStatus.Text = beanQuaTrinhLuanChuyen.SubmitAction;
            //    if (beanQuaTrinhLuanChuyen.SubmitAction.ToUpper().Equals("TỪ CHỐI"))
            //    {
            //        tvStatus.SetTextColor(Color.ParseColor("#e63b34"));
            //    }
            //    else if (beanQuaTrinhLuanChuyen.SubmitAction.TrimEnd().TrimStart().ToUpper().Equals("YÊU CẦU HIỆU CHỈNH"))
            //    {
            //        tvStatus.SetTextColor(Color.ParseColor("#e63b34"));
            //    }
            //    else
            //    {
            //        tvStatus.SetTextColor(Color.ParseColor("#09ab4e"));
            //    }
            //}
            //else
            //{
            //    tvStatus.Visibility = ViewStates.Gone;
            //}
            if (!string.IsNullOrEmpty(beanQuaTrinhLuanChuyen.Action))
            {
                txtTilteSupMenu.Text = beanQuaTrinhLuanChuyen.Action;
            }
            else
            {
                txtTilteSupMenu.Visibility = ViewStates.Gone;
            }
            //ViewGroup.LayoutParams parms = ln.LayoutParameters;
            //parms.Width = ViewGroup.LayoutParams.MatchParent;
            //ln.LayoutParameters = parms;


            return rootView;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
        private async void SetAvata(string path, ImageView img)
        {
            try
            {
                string pdfFilePath = System.IO.Path.Combine(CmmVariable.M_Folder_Avatar + "/", System.IO.Path.GetFileName(path) ?? throw new InvalidOperationException());
                string url = CmmVariable.M_Domain + path;
                ProviderBase pUser = new ProviderBase();
                bool result;
                if (!File.Exists(pdfFilePath))
                {
                    await Task.Run(() =>
                    {
                        result = pUser.DownloadFile(url, pdfFilePath, CmmVariable.M_AuthenticatedHttpClient);
                        if (result)
                        {
                            Bitmap myBitmap = BitmapHelper.LoadAndResizeBitmap(pdfFilePath, 200, 200);
                            if (myBitmap != null)
                            {
                                _mainAct.RunOnUiThread(() =>
                                {
                                    img.SetImageBitmap(myBitmap);
                                });
                            }
                            else
                            {
                                _mainAct.RunOnUiThread(() =>
                                {
                                    img.SetImageResource(Resource.Mipmap.icon_avatar64);
                                });
                            }
                        }
                    });
                }
                else
                {
                    Bitmap myBitmap = BitmapHelper.LoadAndResizeBitmap(pdfFilePath, 200, 200);
                    if (myBitmap != null)
                    {

                        img.SetImageBitmap(myBitmap);
                    }
                    else
                    {
                        img.SetImageResource(Resource.Mipmap.icon_avatar64);
                    }
                }

            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}