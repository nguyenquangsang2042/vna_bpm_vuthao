using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Presenter.Fragment.FragmentChildApp;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    /// <summary>
    /// Bottom Navigation of Child App
    /// </summary>
    public class SharedView_BottomNavigationChildApp : SharedViewBase
    {
        public int flagView { get; set; }

        public LinearLayout _lnContainer { get; set; }

        private RecyclerView _recyNavigation { get; set; }

        private List<BeanBottomNavigation> _lstBeanBottomNavigation { get; set; }

        public SharedView_BottomNavigationChildApp(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
        {
            if (_fragment != null) // Init flagView
            {
                string _type = _fragment.GetType().Name;
                if (_type.Equals(typeof(FragmentChildAppHomePage).Name)) // Child App Home
                {
                    flagView = (int)EnumBottomNavigationView.ChildAppHomePage;
                }
                else if (_type.Equals(typeof(FragmentChildAppList).Name)) // Child App List
                {
                    flagView = (int)EnumBottomNavigationView.ChildAppList;
                }
                else if (_type.Equals(typeof(FragmentChildAppReport).Name)) // Child App Report
                {
                    flagView = (int)EnumBottomNavigationView.ChildAppReport;
                }
                else if (_type.Equals(typeof(FragmentChildAppSingleListVDT).Name)) // Child App VDT
                {
                    flagView = (int)EnumBottomNavigationView.ChildAppSingleListVDT;
                }
                else if (_type.Equals(typeof(FragmentChildAppSingleListVTBD).Name)) // Child App VTBD
                {
                    flagView = (int)EnumBottomNavigationView.ChildAppSingleListVTBD;
                }
            }
        }

        public void InitializeValue(LinearLayout _lnContainer)
        {
            this._lnContainer = _lnContainer;

            _lstBeanBottomNavigation = new List<BeanBottomNavigation>()
            {
                // Stable
                new BeanBottomNavigation { FlagNavigation = (int)EnumBottomNavigationView.ChildAppHomePage, DrawableID = Resource.Drawable.icon_ver2_home, TintColorID = Resource.Color.clBottomDisable,
                                           StableID = 0, Title= "" },
                new BeanBottomNavigation { FlagNavigation = (int)EnumBottomNavigationView.ChildAppSingleListVDT, DrawableID = Resource.Drawable.icon_ver4_tome, TintColorID = Resource.Color.clBottomDisable,
                                           StableID = 1, Title= "" },
                new BeanBottomNavigation { FlagNavigation = (int)EnumBottomNavigationView.ChildAppMore, DrawableID = Resource.Drawable.icon_more, TintColorID = Resource.Color.clBottomDisable,
                                           StableID = 3, Title= "" },
                // Optional
                new BeanBottomNavigation { FlagNavigation = (int)EnumBottomNavigationView.ChildAppSingleListVTBD, DrawableID = Resource.Drawable.icon_ver4_fromme, TintColorID = Resource.Color.clBottomDisable,
                                           StableID = null, Title = CmmFunction.GetTitle("TEXT_FROMME", "Tôi bắt đầu") },
                //new BeanBottomNavigation { FlagNavigation = (int)EnumBottomNavigationView.ChildAppList, DrawableID = Resource.Drawable.icon_ver2_list, TintColorID = Resource.Color.clBottomEnable,
                //                           StableID = null, Title = "List" },
                new BeanBottomNavigation { FlagNavigation = (int)EnumBottomNavigationView.ChildAppKanban, DrawableID = Resource.Drawable.icon_ver2_board2, TintColorID = Resource.Color.clActionGreen,
                                           StableID = null, Title = "Board" },
                //new BeanBottomNavigation { FlagNavigation = (int)EnumBottomNavigationView.ChildAppReport, DrawableID = Resource.Drawable.icon_ver2_report, TintColorID = Resource.Color.clActionRed,
                //                           StableID = null, Title = "Report" },
            };

            foreach (var item in _lstBeanBottomNavigation) // Set Optional
            {
                if (item.FlagNavigation == MainActivity.FlagNavigation_ChildOptional)
                {
                    item.StableID = 2;
                    item.TintColorID = Resource.Color.clBottomDisable;
                    break;
                }
            }
        }

        public override void InitializeView()
        {
            try
            {
                #region Get View - Init Data
                View _view = _inflater.Inflate(Resource.Layout.ViewBottomNavigationChildApp, null);
                _recyNavigation = _view.FindViewById<RecyclerView>(Resource.Id.recy_ViewBottomNavigationChildApp);

                switch (flagView)
                {
                    case (int)EnumBottomNavigationView.ChildAppHomePage:
                        {
                            break;
                        }
                    case (int)EnumBottomNavigationView.ChildAppSingleListVDT:
                        {
                            break;
                        }
                    case (int)EnumBottomNavigationView.ChildAppSingleListVTBD:
                        {
                            break;
                        }
                    case (int)EnumBottomNavigationView.Board:
                        {
                            break;
                        }
                }

                LinearLayoutManager _layoutManager = new LinearLayoutManager(_rootView.Context, LinearLayoutManager.Horizontal, false);
                AdapterRecyBottomNavigation _adapterBottomNavigation = new AdapterRecyBottomNavigation(_mainAct, _rootView.Context, _lstBeanBottomNavigation);
                _adapterBottomNavigation.CustomItemClick += Click_ItemBottomNavigation;
                _recyNavigation.SetAdapter(_adapterBottomNavigation);
                _recyNavigation.SetLayoutManager(_layoutManager);
                #endregion

                #region Show View
                if (_lnContainer != null)
                {
                    _lnContainer.RemoveAllViews();
                    _lnContainer.AddView(_view);
                }
                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "InitializeView", ex);
#endif
            }
        }



        #region Event

        private void Click_ItemBottomNavigation(object sender, BeanBottomNavigation e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick(200) == false) return;

                if (flagView == e.FlagNavigation) return; // click trùng view 

                switch (e.FlagNavigation)
                {
                    case (int)EnumBottomNavigationView.ChildAppHomePage:
                        {
                            MainActivity.FlagNavigation = (int)EnumBottomNavigationView.ChildAppHomePage;
                            MinionAction.OnRedirectFragmentLeftMenu(null, null);
                            break;
                        }
                    case (int)EnumBottomNavigationView.ChildAppSingleListVDT:
                        {
                            MainActivity.FlagNavigation = (int)EnumBottomNavigationView.ChildAppSingleListVDT;
                            MinionAction.OnRedirectFragmentLeftMenu(null, null);
                            break;
                        }
                    case (int)EnumBottomNavigationView.ChildAppSingleListVTBD:
                        {
                            MainActivity.FlagNavigation = (int)EnumBottomNavigationView.ChildAppSingleListVTBD;
                            MinionAction.OnRedirectFragmentLeftMenu(null, null);
                            break;
                        }
                    case (int)EnumBottomNavigationView.ChildAppKanban:
                        {
                            MainActivity.FlagNavigation = (int)EnumBottomNavigationView.ChildAppKanban;
                            MinionAction.OnRedirectFragmentLeftMenu(null, null);
                            break;
                        }
                    case (int)EnumBottomNavigationView.ChildAppList:
                        {
                            MainActivity.FlagNavigation = (int)EnumBottomNavigationView.ChildAppList;
                            MinionAction.OnRedirectFragmentLeftMenu(null, null);
                            break;
                        }
                    case (int)EnumBottomNavigationView.ChildAppReport:
                        {
                            MainActivity.FlagNavigation = (int)EnumBottomNavigationView.ChildAppReport;
                            MinionAction.OnRedirectFragmentLeftMenu(null, null);
                            break;
                        }
                    case (int)EnumBottomNavigationView.ChildAppMore:
                        {
                            Click_imgMore(null, null);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemBottomNavigation", ex);
#endif
            }
        }

        private void Click_imgMore(object sender, EventArgs e)
        {
            try
            {
                #region Get View - Init Data
                View _viewPopup = _inflater.Inflate(Resource.Layout.ViewBottomNavigationMore, null);

                RecyclerView _recy = _viewPopup.FindViewById<RecyclerView>(Resource.Id.recy_ViewBottomNavigationMore);
                TextView _tvClose = _viewPopup.FindViewById<TextView>(Resource.Id.tv_ViewBottomNavigationMore_Close);

                _tvClose.Text = CmmFunction.GetTitle("TEXT_CLOSE", "Thoát");

                List<BeanBottomNavigation> _lstOptional = _lstBeanBottomNavigation.Where(x => x.StableID == null).ToList();

                #endregion

                #region Show View
                Dialog _dialogActionMore = new Dialog(_rootView.Context);
                _dialogActionMore.RequestWindowFeature(1);
                _dialogActionMore.SetCanceledOnTouchOutside(true);
                _dialogActionMore.SetCancelable(true);
                _dialogActionMore.SetContentView(_viewPopup);
                _dialogActionMore.Show();

                Window window = _dialogActionMore.Window;
                WindowManagerLayoutParams s = window.Attributes;
                var dm = _mainAct.Resources.DisplayMetrics;

                window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                window.SetGravity(GravityFlags.Bottom);
                s.Width = dm.WidthPixels;
                s.Height = WindowManagerLayoutParams.WrapContent;
                window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                window.Attributes = s;
                #endregion

                if (_lstOptional != null && _lstOptional.Count > 0)
                {
                    AdapterRecyBottomNavigation_More _adapter = new AdapterRecyBottomNavigation_More(_mainAct, _rootView.Context, _lstOptional);
                    _adapter.CustomItemClick += (sender, e) =>
                    {
                        _dialogActionMore.Dismiss();
                        Click_ItemBottomNavigation(sender, e);
                    };
                    _recy.SetAdapter(_adapter);
                    _recy.SetLayoutManager(new LinearLayoutManager(_rootView.Context, LinearLayoutManager.Vertical, false));
                }
                _tvClose.Click += (sender, e) =>
                {
                    _dialogActionMore.Dismiss();
                };
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgMore", ex);
#endif
            }
        }

        #endregion

        private void SetViewIsSelected(LinearLayout _ln, ImageView _img, TextView _tv)
        {
            try
            {
                _ln.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueNavigation)));
                _img.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain)));
                _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain)));
                _tv.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewIsSelected", ex);
#endif
            }
        }

        #region Adapter for BottomNavigation
        public class AdapterRecyBottomNavigation : RecyclerView.Adapter
        {
            private AppCompatActivity _mainAct;
            private Context _context;
            private List<BeanBottomNavigation> _lstData = new List<BeanBottomNavigation>();
            public event EventHandler<BeanBottomNavigation> CustomItemClick;
            public int _visibleItemCount = 4;
            public AdapterRecyBottomNavigation(AppCompatActivity _mainAct, Context _context, List<BeanBottomNavigation> _lstData, int _visibleItemCount = 4)
            {
                this._mainAct = _mainAct;
                this._context = _context;
                this._lstData = _lstData;
                this._visibleItemCount = _visibleItemCount;
            }

            private void OnItemClick(int position)
            {
                if (CustomItemClick != null)
                {
                    List<BeanBottomNavigation> _lstcurrentItem = _lstData.Where(x => x.StableID == position).ToList();
                    if (_lstcurrentItem != null && _lstcurrentItem.Count > 0)
                        CustomItemClick(this, _lstcurrentItem[0]);
                }
            }

            public override int ItemCount => _lstData.Count;

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemBottomNavigation, parent, false);
                AdapterRecyBottomNavigationHolder holder = new AdapterRecyBottomNavigationHolder(itemView, OnItemClick);
                return holder;
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                AdapterRecyBottomNavigationHolder _holder = holder as AdapterRecyBottomNavigationHolder;

                if (position < _visibleItemCount)
                {
                    _holder._lnContent.LayoutParameters.Width = (int)(_mainAct.Resources.DisplayMetrics.WidthPixels / _visibleItemCount);
                    List<BeanBottomNavigation> _lstcurrentItem = _lstData.Where(x => x.StableID == position).ToList();

                    if (_lstcurrentItem != null && _lstcurrentItem.Count > 0)
                    {
                        _holder._imgContent.SetImageResource(_lstcurrentItem[0].DrawableID);
                        if (MainActivity.FlagNavigation == _lstcurrentItem[0].FlagNavigation)
                            _holder._imgContent.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain)));
                        else
                            _holder._imgContent.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, _lstcurrentItem[0].TintColorID)));
                    }
                }
                else
                {
                    _holder._lnContent.LayoutParameters.Width = 0;
                }
            }

            public class AdapterRecyBottomNavigationHolder : RecyclerView.ViewHolder
            {
                public LinearLayout _lnContent { get; set; }
                public ImageView _imgContent { get; set; }
                public AdapterRecyBottomNavigationHolder(View itemview, Action<int> listener) : base(itemview)
                {
                    _lnContent = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemBottomNavigation);
                    _imgContent = itemview.FindViewById<ImageView>(Resource.Id.img_ItemBottomNavigation);
                    _lnContent.Click += (sender, e) => listener(base.LayoutPosition);
                }
            }
        }

        public class AdapterRecyBottomNavigation_More : RecyclerView.Adapter
        {
            private AppCompatActivity _mainAct;
            private Context _context;
            private List<BeanBottomNavigation> _lstData = new List<BeanBottomNavigation>();
            public event EventHandler<BeanBottomNavigation> CustomItemClick;
            public AdapterRecyBottomNavigation_More(AppCompatActivity _mainAct, Context _context, List<BeanBottomNavigation> _lstData)
            {
                this._mainAct = _mainAct ?? throw new ArgumentNullException(nameof(_mainAct));
                this._context = _context;
                this._lstData = _lstData;
            }

            private void OnItemClick(int position)
            {
                if (CustomItemClick != null)
                    CustomItemClick(this, _lstData[position]);
            }

            public override int ItemCount => _lstData.Count;

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemPopupAction, parent, false);
                AdapterRecyBottomNavigation_MoreHolder holder = new AdapterRecyBottomNavigation_MoreHolder(itemView, OnItemClick);
                return holder;
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                AdapterRecyBottomNavigation_MoreHolder _holder = holder as AdapterRecyBottomNavigation_MoreHolder;

                BeanBottomNavigation _currentItem = _lstData[position];

                _holder._imgContent.SetColorFilter(new Color(ContextCompat.GetColor(_context, _currentItem.TintColorID)));
                _holder._imgContent.SetImageResource(_currentItem.DrawableID);
                _holder._tvContent.Text = _currentItem.Title;
            }

            public class AdapterRecyBottomNavigation_MoreHolder : RecyclerView.ViewHolder
            {
                public LinearLayout _lnContent { get; set; }
                public TextView _tvContent { get; set; }
                public ImageView _imgContent { get; set; }
                public View _viewLine { get; set; }

                public AdapterRecyBottomNavigation_MoreHolder(View itemview, Action<int> listener) : base(itemview)
                {
                    _lnContent = itemview.FindViewById<LinearLayout>(Resource.Id.ln_ItemPopupAction);
                    _tvContent = itemview.FindViewById<TextView>(Resource.Id.tv_ItemPopupAction);
                    _imgContent = itemview.FindViewById<ImageView>(Resource.Id.img_ItemPopupAction);
                    _viewLine = itemview.FindViewById<View>(Resource.Id.view_ItemPopupAction);
                    _lnContent.Click += (sender, e) => listener(base.LayoutPosition);
                }
            }
        }

        #endregion
    }
}