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
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;
using static Android.Views.View;

namespace BPMOPMobile.Droid.Core.Class
{
    /// <summary>
    /// Author: ThachNLS, đừng tìm KhoaHD
    /// </summary>
    public abstract class CustomAnimatedExpandableAdapter : BaseExpandableListAdapter
    {
        private SparseArray<GroupInfo> groupInfo = new SparseArray<GroupInfo>();
        private AnimatedExpandableListView parent;

        private static int STATE_IDLE = 0;
        private static int STATE_EXPANDING = 1;
        private static int STATE_COLLAPSING = 2;

        public void setParent(AnimatedExpandableListView parent)
        {
            this.parent = parent;
        }

        public static int getRealChildType(int groupPosition, int childPosition)
        {
            return 0;
        }

        public int getRealChildTypeCount()
        {
            return 1;
        }

        public abstract View getRealChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent);
        public abstract int getRealChildrenCount(int groupPosition);

        private GroupInfo getGroupInfo(int groupPosition)
        {
            GroupInfo info = groupInfo.Get(groupPosition);
            if (info == null)
            {
                info = new GroupInfo();
                groupInfo.Put(groupPosition, info);
            }
            return info;
        }

        public void notifyGroupExpanded(int groupPosition)
        {
            GroupInfo info = getGroupInfo(groupPosition);
            info.dummyHeight = -1;
        }

        public void startExpandAnimation(int groupPosition, int firstChildPosition)
        {
            GroupInfo info = getGroupInfo(groupPosition);
            info.animating = true;
            info.firstChildPosition = firstChildPosition;
            info.expanding = true;
        }

        public void startCollapseAnimation(int groupPosition, int firstChildPosition)
        {
            GroupInfo info = getGroupInfo(groupPosition);
            info.animating = true;
            info.firstChildPosition = firstChildPosition;
            info.expanding = false;
        }

        private void stopAnimation(int groupPosition)
        {
            GroupInfo info = getGroupInfo(groupPosition);
            info.animating = false;
        }

        public override int GetChildType(int groupPosition, int childPosition)
        {
            GroupInfo info = getGroupInfo(groupPosition);
            if (info.animating)
            {
                return 0;
            }
            else return getRealChildType(groupPosition, childPosition) + 1;
        }

        public override int ChildTypeCount => getRealChildTypeCount() + 1;

        protected ViewGroup.LayoutParams generateDefaultLayoutParams()
        {
            return new AbsListView.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                                                ViewGroup.LayoutParams.WrapContent, 0);
        }

        public override int GroupCount => throw new NotImplementedException();

        public override bool HasStableIds => throw new NotImplementedException();

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            throw new NotImplementedException();
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            throw new NotImplementedException();
        }

        public override int GetChildrenCount(int groupPosition)
        {
            GroupInfo info = getGroupInfo(groupPosition);
            if (info.animating)
            {
                return info.firstChildPosition + 1;
            }
            else
            {
                return getRealChildrenCount(groupPosition);
            }
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            GroupInfo info = getGroupInfo(groupPosition);
            if (info.animating)
            {
                if (convertView?.GetType() != typeof(DummyView))
                {
                    convertView = new DummyView(parent.Context);
                    convertView.LayoutParameters = new AbsListView.LayoutParams(ViewGroup.LayoutParams.MatchParent, 0);
                }
                if (childPosition < info.firstChildPosition)
                {
                    convertView.LayoutParameters.Height = 0;
                    return convertView;
                }
                ExpandableListView listView = (ExpandableListView)parent;
                DummyView dummyView = (DummyView)convertView;
                dummyView.clearViews();
                dummyView.setDivider(listView.Divider, parent.MeasuredWidth, listView.DividerHeight);

                int measureSpecW = MeasureSpec.MakeMeasureSpec(parent.Width, MeasureSpecMode.Exactly);
                int measureSpecH = MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);

                int totalHeight = 0;
                int clipHeight = parent.Height;

                int len = getRealChildrenCount(groupPosition);

                for (int i = info.firstChildPosition; i < len; i++)
                {
                    View childView = getRealChildView(groupPosition, i, (i == len - 1), null, parent);

                    ViewGroup.LayoutParams p = (ViewGroup.LayoutParams)childView.LayoutParameters;

                    if (p == null)
                    {
                        p = (AbsListView.LayoutParams)generateDefaultLayoutParams();
                        childView.LayoutParameters = p;
                    }

                    int lpHeight = p.Height;
                    int childHeightSpec;
                    if (lpHeight > 0)
                    {
                        childHeightSpec = MeasureSpec.MakeMeasureSpec(lpHeight, MeasureSpecMode.Exactly);
                    }
                    else
                    {
                        childHeightSpec = measureSpecH;
                    }

                    childView.Measure(measureSpecW, childHeightSpec);
                    totalHeight += childView.MeasuredHeight;
                    if (totalHeight < clipHeight)
                    {
                        // we only need to draw enough views to fool the user...
                        dummyView.addFakeView(childView);
                    }
                    else
                    {
                        dummyView.addFakeView(childView);

                        // if this group has too many views, we don't want to
                        // calculate the height of everything... just do a light
                        // approximation and break
                        int averageHeight = totalHeight / (i + 1);
                        totalHeight += (len - i - 1) * averageHeight;
                        break;
                    }
                }

                Java.Lang.Object o;
                int state = (o = dummyView.Tag) == null ? STATE_IDLE : ((Number)o).IntValue();
                if (info.expanding && state != STATE_EXPANDING)
                {
                    ExpandAnimation ani = new ExpandAnimation(dummyView, 0, totalHeight, info);
                    ani.Duration = this.parent.getAnimationDuration();
                    ani.AnimationEnd += (ss, ee) =>
                    {
                        stopAnimation(groupPosition);
                        NotifyDataSetChanged();
                        dummyView.Tag = STATE_IDLE;
                    };
                    dummyView.StartAnimation(ani);
                    dummyView.Tag = STATE_EXPANDING;
                }
                else if (!info.expanding && state != STATE_COLLAPSING)
                {
                    if (info.dummyHeight == -1)
                        info.dummyHeight = totalHeight;

                    ExpandAnimation ani = new ExpandAnimation(dummyView, info.dummyHeight, 0, info);
                    ani.Duration = this.parent.getAnimationDuration();
                    ani.AnimationEnd += (ss, ee) =>
                    {
                        stopAnimation(groupPosition);
                        listView.CollapseGroup(groupPosition);
                        NotifyDataSetChanged();
                        info.dummyHeight = -1;
                        dummyView.Tag = STATE_IDLE;
                    };
                    dummyView.StartAnimation(ani);
                    dummyView.Tag = STATE_COLLAPSING;
                }
                return convertView;
            }
            else
                return getRealChildView(groupPosition, childPosition, isLastChild, convertView, parent);
        }


        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            throw new NotImplementedException();
        }

        public override long GetGroupId(int groupPosition)
        {
            throw new NotImplementedException();
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            throw new NotImplementedException();
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            throw new NotImplementedException();
        }
    }

    public class AnimatedExpandableListView : ExpandableListView
    {
        private static int ANIMATION_DURATION = 300;
        private CustomAnimatedExpandableAdapter adapter;
        public AnimatedExpandableListView(Context context) : base(context)
        {
        }
        public AnimatedExpandableListView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public AnimatedExpandableListView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {

        }

        public override void SetAdapter(IExpandableListAdapter adapter)
        {
            base.SetAdapter(adapter);
            // Make sure that the adapter extends AnimatedExpandableListAdapter
            if (adapter.GetType().BaseType == typeof(CustomAnimatedExpandableAdapter))
            {
                this.adapter = (CustomAnimatedExpandableAdapter)adapter;
                this.adapter.setParent(this);
            }
        }

        public bool expandGroupWithAnimation(int groupPos)
        {
            bool lastGroup = groupPos == this.adapter.GroupCount - 1;
            /*if (lastGroup && Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.IceCreamSandwich)
            {
                return ExpandGroup(groupPos, true);
            }*/

            int groupFlatPos = GetFlatListPosition(GetPackedPositionForGroup(groupPos));
            if (groupFlatPos != -1)
            {
                int childIndex = groupFlatPos - FirstVisiblePosition;
                if (childIndex < ChildCount)
                {
                    // Get the view for the group is it is on screen...
                    View v = GetChildAt(childIndex);
                    if (v.Bottom >= Bottom)
                    {
                        // If the user is not going to be able to see the animation
                        // we just expand the group without an animation.
                        // This resolves the case where getChildView will not be
                        // called if the children of the group is not on screen

                        // We need to notify the adapter that the group was expanded
                        // without it's knowledge
                        adapter.notifyGroupExpanded(groupPos);
                        return ExpandGroup(groupPos);
                    }
                }
            }
            adapter.startExpandAnimation(groupPos, 0);
            return ExpandGroup(groupPos);
        }

        public bool collapseGroupWithAnimation(int groupPos)
        {
            int groupFlatPos = GetFlatListPosition(GetPackedPositionForGroup(groupPos));
            if (groupFlatPos != -1)
            {
                int childIndex = groupFlatPos - FirstVisiblePosition;
                if (childIndex >= 0 && childIndex < ChildCount)
                {
                    // Get the view for the group is it is on screen...
                    View v = GetChildAt(childIndex);
                    if (v.Bottom >= Bottom)
                    {
                        // If the user is not going to be able to see the animation
                        // we just collapse the group without an animation.
                        // This resolves the case where getChildView will not be
                        // called if the children of the group is not on screen
                        return CollapseGroup(groupPos);
                    }
                }
                else
                {
                    // If the group is offscreen, we can just collapse it without an
                    // animation...
                    return CollapseGroup(groupPos);
                }
            }

            long packedPos = GetExpandableListPosition(FirstVisiblePosition);
            int firstChildPos = GetPackedPositionChild(packedPos);
            int firstGroupPos = GetPackedPositionGroup(packedPos);

            firstChildPos = firstChildPos == -1 || firstGroupPos != groupPos ? 0 : firstChildPos;

            // Let the adapter know that we are going to start animating the
            // collapse animation.
            adapter.startCollapseAnimation(groupPos, firstChildPos);

            // Force the listview to refresh it's views
            adapter.NotifyDataSetChanged();
            return IsGroupExpanded(groupPos);
        }

        public int getAnimationDuration()
        {
            return ANIMATION_DURATION;
        }
    }

    public class DummyView : View
    {
        private List<View> views = new List<View>();
        private Android.Graphics.Drawables.Drawable divider;
        private int dividerWidth;
        private int dividerHeight;

        public DummyView(Context context) : base(context)
        {
        }

        public void setDivider(Android.Graphics.Drawables.Drawable divider, int dividerWidth, int dividerHeight)
        {
            if (divider != null)
            {
                this.divider = divider;
                this.dividerWidth = dividerWidth;
                this.dividerHeight = dividerHeight;

                divider.SetBounds(0, 0, dividerWidth, dividerHeight);
            }
        }

        /**
         * Add a view for the DummyView to draw.
         * @param childView View to draw
         */
        public void addFakeView(View childView)
        {
            childView.Layout(0, 0, Width, childView.MeasuredHeight);
            views.Add(childView);
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);
            int len = views.Count;
            for (int i = 0; i < len; i++)
            {
                View v = views[i];
                v.Layout(left, top, left + v.MeasuredWidth, top + v.MeasuredHeight);
            }
        }

        public void clearViews()
        {
            views.Clear();
        }

        protected override void DispatchDraw(Canvas canvas)
        {
            canvas.Save();
            if (divider != null)
            {
                divider.SetBounds(0, 0, dividerWidth, dividerHeight);
            }

            int len = views.Count;
            for (int i = 0; i < len; i++)
            {
                View v = views[i];

                canvas.Save();
                canvas.ClipRect(0, 0, Width, v.MeasuredHeight);
                v.Draw(canvas);
                canvas.Restore();

                if (divider != null)
                {
                    divider.Draw(canvas);
                    canvas.Translate(0, dividerHeight);
                }

                canvas.Translate(0, v.MeasuredHeight);
            }

            canvas.Restore();
        }
    }

    public class GroupInfo
    {
        public bool animating = false;
        public bool expanding = false;
        public int firstChildPosition;
        public int dummyHeight = -1;
    }

    public class ExpandAnimation : Animation
    {
        private int baseHeight;
        private int delta;
        private View view;
        private GroupInfo groupInfo;

        public ExpandAnimation(View v, int startHeight, int endHeight, GroupInfo info)
        {
            baseHeight = startHeight;
            delta = endHeight - startHeight;
            view = v;
            groupInfo = info;

            view.LayoutParameters.Height = startHeight;
            view.RequestLayout();
        }

        protected override void ApplyTransformation(float interpolatedTime, Transformation t)
        {
            base.ApplyTransformation(interpolatedTime, t);

            if (interpolatedTime < 1.0f)
            {
                int val = baseHeight + (int)(delta * interpolatedTime);
                view.LayoutParameters.Height = val;
                groupInfo.dummyHeight = val;
                view.RequestLayout();
            }
            else
            {
                int val = baseHeight + delta;
                view.LayoutParameters.Height = val;
                groupInfo.dummyHeight = val;
                view.RequestLayout();
            }
        }
    }
}