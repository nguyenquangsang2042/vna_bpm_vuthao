using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Presenter.Fragment;

namespace BPMOPMobile.Droid.Presenter.Adapter
{

    public class AdapterViewPagerHomePage : Android.Support.V4.App.FragmentStatePagerAdapter  /*FragmentStatePagerAdapter*/
    {
        private List<string> _lstservice;
        private List<List<KeyValuePair<string, string>>> _lstFilterCondition = new List<List<KeyValuePair<string, string>>>();
        private Android.Support.V4.App.Fragment _currentFragment;
        private bool IsChildApp = false;
        private bool IsFollowScreen = false;

        public AdapterViewPagerHomePage(Android.Support.V4.App.FragmentManager fm, CustomBaseFragment _currentFragment, List<string> _lstservice, List<List<KeyValuePair<string, string>>> _lstFilterCondition,bool IsChildApp = false) : base(fm)
        {
            this._lstservice = _lstservice;
            this._lstFilterCondition = _lstFilterCondition;
            this._currentFragment = _currentFragment;
            this.IsChildApp = IsChildApp;
        }
        public AdapterViewPagerHomePage(Android.Support.V4.App.FragmentManager fm, CustomBaseFragment _currentFragment, List<string> _lstservice, List<List<KeyValuePair<string, string>>> _lstFilterCondition, bool IsChildApp = false, bool _isFollowScreen=false) : base(fm)
        {
            this._lstservice = _lstservice;
            this._lstFilterCondition = _lstFilterCondition;
            this._currentFragment = _currentFragment;
            this.IsChildApp = IsChildApp;
            this.IsFollowScreen = _isFollowScreen;
        }
        public override int Count
        {
            get
            {
                return 2;
            }
        }
        public override int GetItemPosition(Java.Lang.Object @object)
        {
            return base.GetItemPosition(@object);
        }
        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            if (IsChildApp)
            {
                if (position == 0) // Việc đến tôi
                {
                    return PagerChildAppHomePageSingleList.NewInstance((CustomBaseFragment)_currentFragment, _lstservice[0], _lstFilterCondition[0]);
                }
                else // Việc tôi bắt đầu
                {
                    return PagerChildAppHomePageSingleList.NewInstance((CustomBaseFragment)_currentFragment, _lstservice[1], _lstFilterCondition[1]);
                }
            }
            else
            {
                if (position == 0) // Việc đến tôi
                {
                    if(IsFollowScreen)
                        return PagerFollowSingleList.NewInstance((CustomBaseFragment)_currentFragment, _lstservice[0], _lstFilterCondition[0],_isFollowScreen:IsFollowScreen);
                    else
                        return PagerHomePageSingleList.NewInstance((CustomBaseFragment)_currentFragment, _lstservice[0], _lstFilterCondition[0], _isFollowScreen: IsFollowScreen);
                }
                else // Việc tôi bắt đầu
                {
                    if (IsFollowScreen)
                        return PagerFollowSingleList.NewInstance((CustomBaseFragment)_currentFragment, _lstservice[1], _lstFilterCondition[1], _isFollowScreen: IsFollowScreen);
                    else
                        return PagerHomePageSingleList.NewInstance((CustomBaseFragment)_currentFragment, _lstservice[1], _lstFilterCondition[1], _isFollowScreen: IsFollowScreen);

                }
            }
        }
    }
}