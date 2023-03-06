using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Core.Component;

namespace BPMOPMobile.Droid.Tablet.Presenter.Adapter
{
    [Obsolete]
    class AdapterDetailExpandControl : BaseExpandableListAdapter
    {
        private Context _context;
        private MainActivity _mainAct;
        private List<ViewSection> _lstSection;
        private Resources _resources;
        public AdapterDetailExpandControl(MainActivity _mainAct, Context context, List<ViewSection> _lstSection, Resources _resources)
        {
            this._mainAct = _mainAct;
            this._context = context;
            this._lstSection = _lstSection;
            this._resources = _resources;
        }
        public override int GroupCount
        {
            get
            {
                return _lstSection.Count;
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
            return _lstSection[groupPosition].ViewRows.Count;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            LayoutInflater mInflater = LayoutInflater.From(_context);
            View rootView = mInflater.Inflate(Resource.Layout.ItemExpandDetailWorkflow, null);
            LinearLayout _lnContent = rootView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailWorkflow_Content);

            ViewRow _viewRow = _lstSection[groupPosition].ViewRows[childPosition];
            ComponentBase _components;

            switch (_viewRow.RowType)
            {
                case 1:
                    _components = new ComponentRow1(_mainAct, _lnContent, _viewRow.Elements[0]);
                    break;
                case 2:
                    _components = new ComponentRow2(_mainAct, _lnContent, _viewRow, _resources);
                    break;
                case 3:
                    _components = new ComponentRow3(_mainAct, _lnContent, _viewRow, _resources);
                    break;
                default:
                    _components = new ComponentRow1(_mainAct, _lnContent, _viewRow.Elements[0]);
                    break;
            }
            _components.InitializeFrameView(_lnContent);
            _components.SetTitle();
            _components.SetValue();
            _components.SetEnable();
            _components.SetProprety();
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
            LayoutInflater mInflater = LayoutInflater.From(_context);
            View rootView = mInflater.Inflate(Resource.Layout.ItemExpandDetailWorkflow, null);

            LinearLayout _lnContent = rootView.FindViewById<LinearLayout>(Resource.Id.ln_ItemExpandDetailWorkflow_Content);
            ComponentSection _section = new ComponentSection(_mainAct, _lnContent, _lstSection[groupPosition], groupPosition, _resources);
            _section.InitializeFrameView(_lnContent);
            _section.UpdateContentSection();
            return rootView;
        }
        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
    }
}