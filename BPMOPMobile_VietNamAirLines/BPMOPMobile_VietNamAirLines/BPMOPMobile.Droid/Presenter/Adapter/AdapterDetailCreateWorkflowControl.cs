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
using BPMOPMobile.Droid.Core.Component;
using BPMOPMobile.Droid.Core.Controller;

namespace BPMOPMobile.Droid.Presenter.Adapter
{
    class AdapterDetailCreateWorkflowControl : BaseAdapter<ViewRow>
    {
        private MainActivity _mainAct;
        private Context context;
        private List<string> _lstCategoryFlag = new List<string>(); // flag này để lưu lại trạng thái item[n-1] để xem có hiện category item[n] không
        private List<ViewRow> _lstRow = new List<ViewRow>();
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        public event EventHandler<BeanNotify> CustomItemClick;

        public AdapterDetailCreateWorkflowControl(Context context, List<ViewRow> _lstRow, MainActivity mainAct)
        {
            this.context = context;
            this._lstRow = _lstRow;
            this._mainAct = mainAct;
        }
        public override ViewRow this[int position] => _lstRow[position];

        public override int Count => _lstRow.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            //DetailCreateWorkflowControlViewHolder holder;

            //convertView = LayoutInflater.From(context).Inflate(Resource.Layout.ItemExpandDetailWorkflow, null);
            //holder = new DetailCreateWorkflowControlViewHolder
            //{
            //    _lnContent = convertView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailWorkflow_Content),
            //};
            //ViewRow _viewRow = _lstRow[position];
            //ComponentBase _components;
            //switch (_viewRow.RowType)
            //{
            //    case 1:
            //        _components = new ComponentRow1(_mainAct, holder._lnContent, _viewRow.Elements[0], -1, true);
            //        break;
            //    case 2:
            //        _components = new ComponentRow2(_mainAct, holder._lnContent, _viewRow, _mainAct.Resources, -1, false);
            //        break;
            //    case 3:
            //        _components = new ComponentRow3(_mainAct, holder._lnContent, _viewRow, _mainAct.Resources, -1, false);
            //        break;
            //    default:
            //        _components = new ComponentRow1(_mainAct, holder._lnContent, _viewRow.Elements[0], -1, true);
            //        break;
            //}
            //_components.InitializeFrameView(holder._lnContent);
            //_components.SetTitle();
            //_components.SetValue();
            //_components.SetEnable();
            //_components.SetProprety();
            return convertView;
        }
        class DetailCreateWorkflowControlViewHolder : Java.Lang.Object
        {
            public LinearLayout _lnContent { get; set; }
        }

    }
}