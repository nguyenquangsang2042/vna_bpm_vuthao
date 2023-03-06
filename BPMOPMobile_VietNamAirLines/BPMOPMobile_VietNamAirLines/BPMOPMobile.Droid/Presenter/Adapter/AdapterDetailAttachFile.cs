using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    class AdapterDetailAttachFile : BaseAdapter<BeanAttachFile>
    {
        private Context context;
        private List<BeanAttachFile> _lstFile;

        public AdapterDetailAttachFile(List<BeanAttachFile> lstFile, Context context)
        {
            _lstFile = lstFile;
            this.context = context;
        }
        public override BeanAttachFile this[int position]
        {
            get
            {
                return _lstFile[position];
            }
        }

        public override int Count
        {
            get
            {
                return _lstFile.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater mInflater = LayoutInflater.From(context);
            View rootView = mInflater.Inflate(Resource.Layout.ItemAttachFile, null);
            TextView tvTitle = rootView.FindViewById<TextView>(Resource.Id.tv_ItemAttachFile_Name);
            ImageView imgDinhkem = rootView.FindViewById<ImageView>(Resource.Id.img_ItemAttachFile);
           
            if (!String.IsNullOrEmpty(_lstFile[position].Title))
            {
                if (_lstFile[position].Title.Contains(";#"))
                {
                    tvTitle.Text = _lstFile[position].Title.Split(new string[] { ";#" }, StringSplitOptions.None)[0];
                }
                else
                {
                    tvTitle.Text = _lstFile[position].Title;
                }
            }
            else
            {
                tvTitle.Text = _lstFile[position].Title;
            }

            string exten = System.IO.Path.GetExtension(_lstFile[position].Path);
            if (exten != null && (exten.ToLower().Equals(".doc") || exten.ToLower().Equals(".docx")))
            {
                //imgDinhkem.SetImageResource(Resource.Drawable.icon_attach_word);
                imgDinhkem.SetImageResource(Resource.Drawable.icon_word);
            }
            else if (exten != null && (exten.ToLower().Equals(".png") || exten.ToLower().Equals(".jpeg") || exten.ToLower().Equals(".jpg")))
            {
                //imgDinhkem.SetImageResource(Resource.Drawable.icon_attach_image); 
                imgDinhkem.SetImageResource(Resource.Drawable.icon_attachFile_photo);
            }
            else if (exten != null && (exten.ToLower().Equals(".xls") || exten.ToLower().Equals(".xlsx")))
            {
                //imgDinhkem.SetImageResource(Resource.Drawable.icon_attach_excel);
                imgDinhkem.SetImageResource(Resource.Drawable.icon_attachFile_excel);
            }
            else if (exten != null && exten.ToLower().Equals(".pdf"))
            {
                //imgDinhkem.SetImageResource(Resource.Drawable.icon_attach_pdf);     
                imgDinhkem.SetImageResource(Resource.Drawable.icon_attachFile_pdf);
            }
            else if (exten != null && exten.ToLower().Contains(".ppt"))
            {
                imgDinhkem.SetImageResource(Resource.Drawable.icon_attachFile_ppt);
            }
            else
            {
                imgDinhkem.SetImageResource(Resource.Drawable.icon_attach_other);
            }
            return rootView;
        }

    }
}