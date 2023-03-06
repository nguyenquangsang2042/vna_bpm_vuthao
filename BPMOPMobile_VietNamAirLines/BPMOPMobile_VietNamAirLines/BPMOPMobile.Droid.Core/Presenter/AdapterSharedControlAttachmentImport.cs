using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Core.Class;

namespace BPMOPMobile.Droid.Core.Presenter
{
    /// <summary>
    /// Adapter này để Load List file trong ứng dụng trường hợp có Control Import File
    /// </summary>
    public class AdapterSharedControlAttachmentImport : RecyclerView.Adapter
    {
        public Activity _mainAct;
        public Context _context;
        public List<BeanAttachFileControl> _lstAttachment = new List<BeanAttachFileControl>();

        public event EventHandler<BeanAttachFileControl> CustomItemClick;

        public AdapterSharedControlAttachmentImport(Activity _mainAct, Context _context, List<BeanAttachFileControl> _lstAttachment)
        {
            this._mainAct = _mainAct;
            this._lstAttachment = _lstAttachment;
            this._context = _context;
        }
        private void OnItemClick(BeanAttachFileControl obj)
        {
            CustomItemClick(this, obj);
        }
        public override int ItemCount => _lstAttachment.Count;
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemControlAttachmentImport, parent, false);
            AdapterControlAttachmentImportViewHolder holder = new AdapterControlAttachmentImportViewHolder(itemView);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AdapterControlAttachmentImportViewHolder vh = holder as AdapterControlAttachmentImportViewHolder;

            vh._imgDelete.Visibility = ViewStates.Invisible;

            if (!String.IsNullOrEmpty(_lstAttachment[position].Title))
            {
                vh._tvName.Text = _lstAttachment[position].Title;
            }
            else
            {
                vh._tvName.Text = "";
            }

            if (!String.IsNullOrEmpty(_lstAttachment[position].Type))
            {
                string exten = _lstAttachment[position].Type;
                if (exten != null && (exten.ToLower().Equals(".doc") || exten.ToLower().Equals(".docx")))
                {
                    vh._imgExtension.SetImageResource(Resource.Drawable.icon_word);
                }
                else if (exten != null && (exten.ToLower().Equals(".png") || exten.ToLower().Equals(".jpeg") || exten.ToLower().Equals(".jpg")))
                {
                    vh._imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_photo);
                }
                else if (exten != null && (exten.ToLower().Equals(".xls") || exten.ToLower().Equals(".xlsx")))
                {
                    vh._imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_excel);
                }
                else if (exten != null && exten.ToLower().Equals(".pdf"))
                {
                    vh._imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_pdf);
                }
                else if (exten != null && exten.ToLower().Equals(".ppt"))
                {
                    vh._imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_ppt);
                }
                else
                {
                    vh._imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_other);
                }
            }
            else
            {
                vh._imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_other);
            }

            //if (!String.IsNullOrEmpty(_lstAttachment[position].Size))
            //{
            //    vh._tvSize.Text = _lstAttachment[position].Size;
            //}
            //else
            //{
            //    vh._tvSize.Text = "";
            //}

            if (!String.IsNullOrEmpty(_lstAttachment[position].Category))
            {
                vh._tvCategory.Text = _lstAttachment[position].Category;
            }
            else
            {
                vh._tvCategory.Text = "";
            }

            vh._lnAll.Click += (sender, e) => OnItemClick(_lstAttachment[position]);
        }
        public class AdapterControlAttachmentImportViewHolder : RecyclerView.ViewHolder
        {
            public LinearLayout _lnAll { get; set; }
            public TextView _tvName { get; set; }
            public TextView _tvSize { get; set; }
            public ImageView _imgExtension { get; set; }
            public TextView _tvCategory { get; set; }
            public LinearLayout _lnName { get; set; }
            public LinearLayout _lnCategory { get; set; }
            public LinearLayout _lnDelete { get; set; }
            public ImageView _imgDelete { get; set; }
            public AdapterControlAttachmentImportViewHolder(View itemview) : base(itemview)
            {
                _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlAttachmentImport); 
                   _tvName = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlAttachmentImport_Name);
                _tvSize = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlAttachmentImport_Size);
                _imgExtension = itemview.FindViewById<ImageView>(Resource.Id.img_ItemControlAttachmentImport_Extension);
                _tvCategory = itemview.FindViewById<TextView>(Resource.Id.tv_ItemControlAttachmentImport_Category);
                _lnName = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlAttachmentImport_Name);
                _lnCategory = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlAttachmentImport_Category);
                _lnDelete = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemControlAttachmentImport_Delete);
                _imgDelete = itemview.FindViewById<ImageView>(Resource.Id.img_ItemControlAttachmentImport_Delete);
            }
        }
    }
}