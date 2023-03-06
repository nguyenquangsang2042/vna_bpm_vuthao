using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using Refractored.Controls;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    public class AdapterExpandListAttachFile : BaseExpandableListAdapter
    {
        private Context _context;
        private MainActivity _mainAct;
        private List<BeanGroupAttachFile> _lstGroupAttachFile = new List<BeanGroupAttachFile>();

        public AdapterExpandListAttachFile(MainActivity _mainAct, Context context, List<BeanGroupAttachFile> _lstGroupAttachFile)
        {
            this._context = context;
            this._mainAct = _mainAct;
            this._lstGroupAttachFile = _lstGroupAttachFile;
        }
        public override int GroupCount => _lstGroupAttachFile.Count;

        public override bool HasStableIds => false;

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
            return _lstGroupAttachFile[groupPosition].AttachFiles.Count;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            BeanAttachFile _itemAttachFile = _lstGroupAttachFile[groupPosition].AttachFiles[childPosition];

            View rootView = convertView;
            if (rootView == null)
            {
                LayoutInflater mInflater = LayoutInflater.From(_context);
                rootView = mInflater.Inflate(Resource.Layout.ItemExpandDetailAttachFileChild, null);
            }
            LinearLayout _lnAll = rootView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailAttachFileGroup);

            TextView _tvName = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailAttachFileGroup_Name);
            TextView _tvSize = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailAttachFileGroup_Size);
            ImageView _imgExtension = rootView.FindViewById<ImageView>(Resource.Id.img_ItemExpandDetailAttachFileGroup_Extension);
            TextView _tvCategory = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailAttachFileGroup_Category);
            LinearLayout _lnName = rootView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailAttachFileGroup_Name);
            LinearLayout _lnCategory = rootView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailAttachFileGroup_Category);

            if (childPosition % 2 == 0) // tô màu so le
            {
                _lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clGraySearchUser)));
            }
            else
            {
                _lnAll.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.clWhite)));
            }

            if (!String.IsNullOrEmpty(_itemAttachFile.Path))
            {
                string exten = _itemAttachFile.Path;
                if (exten != null && (exten.ToLower().Contains(".doc") || exten.ToLower().Contains(".docx")))
                {
                    _imgExtension.SetImageResource(Resource.Drawable.icon_word);
                }
                else if (exten != null && (exten.ToLower().Contains(".png") || exten.ToLower().Contains(".jpeg") || exten.ToLower().Contains(".jpg")))
                {
                    _imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_photo);
                }
                else if (exten != null && (exten.ToLower().Contains(".xls") || exten.ToLower().Contains(".xlsx")))
                {
                    _imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_excel);
                }
                else if (exten != null && exten.ToLower().Contains(".pdf"))
                {
                    _imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_pdf);
                }
                else if (exten != null && exten.ToLower().Contains(".ppt"))
                {
                    _imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_ppt);
                }
                else
                {
                    _imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_other);
                }
            }
            else
            {
                _imgExtension.SetImageResource(Resource.Drawable.icon_attachFile_other);
            }


            if (!String.IsNullOrEmpty(_itemAttachFile.Title))
            {
                if (_itemAttachFile.Title.Contains(";#"))
                {
                    _tvName.Text = _itemAttachFile.Title.Split(new string[] { ";#" }, StringSplitOptions.None)[0];
                }
                else
                {
                    _tvName.Text = _itemAttachFile.Title;
                }
            }
            else
            {
                _tvName.Text = "";
            }

            try
            {
                _tvSize.Text = CmmDroidFunction.GetFormatFileSize(_itemAttachFile.Size);
            }
            catch (Exception)
            {
                _tvSize.Text = "";
            }

            //if (!String.IsNullOrEmpty(_itemAttachFile.Category))
            //{
            //    _tvCategory.Text = _itemAttachFile.Category.Split(new string[] { ";#" }, StringSplitOptions.None)[1];
            //}
            //else
            //{
            //    _tvCategory.Text = "";
            //}

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
                rootView = mInflater.Inflate(Resource.Layout.ItemExpandDetailAttachFileGroup, null);
            }
            ImageView _imgExpand = rootView.FindViewById<ImageView>(Resource.Id.img_ItemExpandDetailAttachFileGroup_Expand);
            TextView _tvtitle = rootView.FindViewById<TextView>(Resource.Id.tv_ItemExpandDetailAttachFileGroup_Title);

            if (isExpanded == true)
            {
                _imgExpand.Rotation = -90;
            }
            else
            {
                _imgExpand.Rotation = 180;
            }

            if (!String.IsNullOrEmpty(_lstGroupAttachFile[groupPosition].Category))
            {
                if (_lstGroupAttachFile[groupPosition].Category.Contains(";#"))
                {
                    _tvtitle.Text = _lstGroupAttachFile[groupPosition].Category.Split(";#")[1];
                }
                else
                {
                    _tvtitle.Text = _lstGroupAttachFile[groupPosition].Category;
                }
                if (_lstGroupAttachFile[groupPosition].AttachFiles != null && _lstGroupAttachFile.Count > 0) // Gắn thêm count vào
                {
                    _tvtitle.Text += String.Format(" ({0})", _lstGroupAttachFile[groupPosition].AttachFiles.Count);
                }
            }
            else
            {
                _tvtitle.Text = "";
            }

            return rootView;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
    }
}