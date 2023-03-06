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
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    public class AdapterCommentAttachFile : RecyclerView.Adapter
    {
        private MainActivity _mainAct;
        private Context _context;
        private List<BeanAttachFile> _lstData = new List<BeanAttachFile>();

        public event EventHandler<BeanAttachFile> CustomItemClick_Detail;
        public event EventHandler<BeanAttachFile> CustomItemClick_Delete;

        bool _showImgDelete = true;

        public AdapterCommentAttachFile(MainActivity _mainAct, Context _context, List<BeanAttachFile> _lstData, bool _showImgDelete)
        {
            this._mainAct = _mainAct;
            this._context = _context;
            this._lstData = _lstData;
            this._showImgDelete = _showImgDelete;
        }

        private void OnItemClickDetail(int position)
        {
            if (CustomItemClick_Detail != null)
                CustomItemClick_Detail(this, _lstData[position]);

            NotifyDataSetChanged();
        }
        private void OnItemClickDelete(int position)
        {
            if (CustomItemClick_Delete != null)
                CustomItemClick_Delete(this, _lstData[position]);

            //RemoveItemListIsClicked(_lstUser[position]);
            NotifyDataSetChanged();
        }
        public void UpdateListAttach(List<BeanAttachFile> _lstData)
        {
            try
            {
                this._lstData = _lstData;
                NotifyDataSetChanged();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "UpdateListAttach", ex);
#endif
            }
        }

        public override int ItemCount => _lstData.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemCommentAttachFile, parent, false);
            AdapterCommentAttachFileHolder holder = new AdapterCommentAttachFileHolder(itemView, OnItemClickDetail, OnItemClickDelete);
            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                AdapterCommentAttachFileHolder _holder = holder as AdapterCommentAttachFileHolder;

                if (!String.IsNullOrEmpty(_lstData[position].Title))
                {
                    _holder._tvTitle.Text = CmmDroidFunction.GetFormatTitleFile(_lstData[position].Title);
                }                   
                else
                    _holder._tvTitle.Text = "";

                if (_showImgDelete == true)
                    _holder._imgDelete.Visibility = ViewStates.Visible;
                else
                    _holder._imgDelete.Visibility = ViewStates.Gone;

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnBindViewHolder", ex);
#endif
            }
        }

        public class AdapterCommentAttachFileHolder : RecyclerView.ViewHolder
        {
            public LinearLayout _lnAll { get; set; }
            public TextView _tvTitle { get; set; }
            public ImageView _imgDelete { get; set; }
            public AdapterCommentAttachFileHolder(View itemview, Action<int> listenerDetail, Action<int> listenerDelete) : base(itemview)
            {
                _lnAll = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemCommentAttachFile_All);
                _tvTitle = itemview.FindViewById<TextView>(Resource.Id.tv_ItemCommentAttachFile_Title);
                _imgDelete = itemview.FindViewById<ImageView>(Resource.Id.img_ItemCommentAttachFile_Delete);
                _tvTitle.Click += (sender, e) => listenerDetail(base.LayoutPosition);
                _imgDelete.Click += (sender, e) => listenerDelete(base.LayoutPosition);
            }
        }
    }
}