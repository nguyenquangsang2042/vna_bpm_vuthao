using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Allyants.BoardViewLib;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Presenter.Adapter;

namespace BPMOPMobile.Droid.Class
{
    public class MyCustomBoardView : BoardView
    {
        //AdapterBoardDetailGroupLibrary _adapter;
        public MyCustomBoardView(Context context) : base(context)
        {
        }

        public MyCustomBoardView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public MyCustomBoardView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        protected MyCustomBoardView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
        public void CustomNotifyDataSetChanged(/*AdapterBoardDetailGroupLibrary _adapter*/)
        {
            ////try
            ////{
            ////    List<BeanBoardStepDefine> _newListData = _adapter.GetListData();
            ////    //base.NotifyDataSetChanged();
            ////    if (_adapter != null)
            ////    {
            ////        for (int column_pos = 0; column_pos < BoardAdapter.ColumnCount; column_pos++)
            ////        {
            ////            BoardAdapter.Column _tempColumns = (BoardAdapter.Column)BoardAdapter.Columns[column_pos]; // Object của column

            ////            for (int item_pos = _tempColumns.Objects.Count - 1; item_pos > -1; item_pos--)
            ////            {
            ////                // remove hết View cũ ra
            ////                ViewGroup parent_item;
            ////                try
            ////                {
            ////                    parent_item = removeParent((View)_tempColumns.Views[item_pos]);
            ////                    _tempColumns.Views.RemoveAt(item_pos);
            ////                }
            ////                catch (Exception ex)
            ////                {
            ////                    parent_item = null;
            ////                }

            ////                if (item_pos < _newListData[column_pos].lstWorkflowItem.Count && parent_item != null)
            ////                {
            ////                    View _newItemView = BoardAdapter.CreateItemView(Context, null, null, column_pos, item_pos);
            ////                    _tempColumns.Views.Insert(item_pos, _newItemView);
            ////                    parent_item.AddView(_newItemView);
            ////                }
            ////            }
            ////            ////for (int item_pos = 0; item_pos < _tempColumns.Objects.Count; item_pos++)
            ////            ////{

            ////            ////    ViewGroup parent_item = removeParent((View)_tempColumns.Views[item_pos]);

            ////            ////    _tempColumns.Views.RemoveAt(item_pos);

            ////            ////    if (item_pos < _newListData[column_pos].lstWorkflowItem.Count)
            ////            ////    {
            ////            ////        View _newItemView = BoardAdapter.CreateItemView(Context, null, null, column_pos, item_pos);
            ////            ////        _tempColumns.Views.Insert(item_pos, _newItemView);
            ////            ////        parent_item.AddView(_newItemView);
            ////            ////    }
            ////            ////}
            ////        }
            ////    }
            ////            }
            ////            catch (Exception ex)
            ////            {
            ////#if DEBUG
            ////                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "CustomNotifyDataSetChanged", ex);
            ////#endif
            ////            }
        }
        private ViewGroup removeParent(View view)
        {
            if (view == null)
            {
                return null;
            }
            ViewGroup viewGroup = ((ViewGroup)view.Parent);
            if (viewGroup != null)
            {
                viewGroup.RemoveView(view);
            }
            return viewGroup;
        }
        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
            //ViewGroup parent_header = RemoveParent(BoardAdapter.columns.get(column_pos).header);
            //BoardAdapter.columns.get(column_pos).header = BoardAdapter.createHeaderView(getContext(), BoardAdapter.columns.get(column_pos).header_object, column_pos);
            //parent_header.addView(BoardAdapter.columns.get(column_pos).header);
        }

    }
}