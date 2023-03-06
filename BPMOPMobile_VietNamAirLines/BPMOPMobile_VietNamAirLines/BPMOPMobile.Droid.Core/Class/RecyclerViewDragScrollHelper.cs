using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace BPMOPMobile.Droid.Core.Class
{
    public class RecyclerViewDragScrollHelper
    {
        DragEvent dragEvent = null; //For passing the drag event to newly bound views
        Handler handler = new Handler(); //to pass the run things in the ui thread
        ScrollDirection currentScrollDirection = ScrollDirection.None; //the current direction being scrolled

        /// <summary>
        /// If this is not set then it will try to find a recycler view
        /// in the hovered over view's view tree
        /// </summary>
        public RecyclerView RecyclerViewToScroll { get; set; }

        public RecyclerViewDragScrollHelper()
        {

        }

        public RecyclerViewDragScrollHelper(RecyclerView recyclerViewToScroll)
        {
            RecyclerViewToScroll = recyclerViewToScroll;
        }


        public enum UnitType
        {
            Pixels,
            Percent
        }


        enum ScrollDirection
        {
            None,
            Foward,
            Back
        }

        /// <summary>
        /// Threshold value can be a percentage or pixels
        /// </summary>
        public UnitType ThresholdUnits { get; set; } = UnitType.Percent;

        /// <summary>
        /// Threshold of how many pixels or percentage from the edge it has to be before it 
        /// starts scrolling
        /// </summary>
        public float Threshold { get; set; } = 0.20f;

        /// <summary>
        /// Speed of the scroll
        /// </summary>
        public int ScrollInterval { get; set; } = 5;


        /// <summary>
        /// Place this method in the method for event handler for View.Drag
        /// Or if useing View.SetOnDragListener(), in the implimentation of
        /// View.IOnDragListener
        /// </summary>
        /// <param name="view"></param>
        /// <param name="dragEvent"></param>
        /// <returns></returns>
        public void HandleDrag(View view, DragEvent dragEvent)
        {
            var action = dragEvent.Action;
            switch (action)
            {
                case DragAction.Started:
                    DragStarted(dragEvent);
                    break;
                case DragAction.Ended:
                    DragEnded();
                    break;
                case DragAction.Location:
                    var dragpoint = new Point(Convert.ToInt32(dragEvent.GetX()), Convert.ToInt32(dragEvent.GetY()));
                    HandleLocation(view, dragpoint);
                    break;
                case DragAction.Exited:
                    currentScrollDirection = ScrollDirection.None;
                    break;
            }
        }



        /// <summary>
        /// Run on DragAction.Location.
        /// If it's within the threshold bounds it should scroll the recycler view.
        /// Should work for both vertical and horizontal linear layouts.
        /// Wont currently work for other layouts
        /// </summary>
        /// <param name="viewHoveredOver">This is the view that is being dragged over.
        /// It can be a view in a recyclerview, or the recycler view it's self</param>
        /// <param name="position"></param>
        /// <returns></returns>
        private void HandleLocation(View viewHoveredOver, Point position)
        {
            var recyclerView = RecyclerViewToScroll;
            var viewHoveredOverIsChildOfRecyclerView = false;
            //if RecyclerViewToScroll has not been set then
            //see if there is a recycler view in the viewHoveredOver view tree
            if (recyclerView == null)
            {
                recyclerView = FindRecyclerViewInTree(viewHoveredOver);
                viewHoveredOverIsChildOfRecyclerView = true;
            }
            else
            {
                viewHoveredOverIsChildOfRecyclerView = IsViewChildOfView(viewHoveredOver, recyclerView);
            }

            if (recyclerView != null &&
                    viewHoveredOverIsChildOfRecyclerView
                    && recyclerView.GetLayoutManager() is LinearLayoutManager layoutManager)
            {
                //get the drag position in the view, and work out where it is
                //relative to the recycler view. (Both viewHoveredOver and recyclerView
                //might be the same view, this doesn't matter)
                var pointInRecyclerView = GetPointRelativeTo(viewHoveredOver, recyclerView, position);

                //work out the properties for the scrolling based on if it's 
                //verticle or horizontal
                float offSet = 0; //the x or y coord for the drag depending on the orientaion
                float upperThreshold = 0; //threshold postion at the end of the recyclerview
                int scrollX = 0; //the absolute amount to scroll horizontally
                int scrollY = 0; //the absolute amount to scroll vertically
                var layoutDirection = layoutManager.Orientation;

                //If the threshold units are pixels then just take the threshold value.
                //If it's percentage times the threshold by the height to get the pixels
                var thresholdPixels = ThresholdUnits == UnitType.Pixels ? Threshold : Threshold * recyclerView.Height;

                //If the layout direction is vertical scroll Y coord and check Y offset
                //If the layout direction is horizontal scroll X coord and check X offset
                if (layoutDirection == LinearLayoutManager.Vertical)
                {
                    offSet = pointInRecyclerView.Y;
                    upperThreshold = recyclerView.Height - thresholdPixels;
                    scrollY = ScrollInterval;
                }
                else if (layoutDirection == LinearLayoutManager.Horizontal)
                {
                    offSet = pointInRecyclerView.X;
                    upperThreshold = recyclerView.Width - thresholdPixels;
                    scrollX = ScrollInterval;
                }

                //if the drag position is less than the lower threshold then scroll backwards
                //if it's greater than the uper threshold then scroll forward
                //if it's in neither, the set the scroll direction to none
                var newScrollDirection = offSet < thresholdPixels
                    ? ScrollDirection.Back
                    : offSet > upperThreshold
                    ? ScrollDirection.Foward
                    : ScrollDirection.None;

                InScrollPostion(newScrollDirection, recyclerView, scrollX, scrollY);

            }
        }


        Java.Util.Timer timer;
        /// <summary>
        /// Scroll continiously in the set direction while the position is in the bounds
        /// </summary>
        /// <param name="newDirection"></param>
        /// <param name="recyclerView"></param>
        /// <param name="scrollX"></param>
        /// <param name="scrollY"></param>
        /// <returns></returns>
        private void InScrollPostion(
                                ScrollDirection newDirection,
                                RecyclerView recyclerView,
                                int scrollX,
                                int scrollY)
        {
            //if it's not already scrolling 
            if (currentScrollDirection != newDirection && newDirection != ScrollDirection.None)
            {
                currentScrollDirection = newDirection;
                //1 for forward -1 for backwards
                var scrollDirectionInt = newDirection == ScrollDirection.Foward ? 1 : -1;
                var canScrollDown = recyclerView.CanScrollVertically(scrollDirectionInt);
                var canScrollRight = recyclerView.CanScrollHorizontally(scrollDirectionInt);
                scrollX *= scrollDirectionInt;
                scrollY *= scrollDirectionInt;

                timer?.Cancel(); //cancel the previous timer so they don't stack
                timer?.Dispose();
                timer = new Java.Util.Timer(); //make a new timer
                                               //Action to be performed every timer tick
                Action action = () =>
                {
                    if (currentScrollDirection == newDirection
                            && (recyclerView.CanScrollVertically(scrollDirectionInt)
                            || recyclerView.CanScrollHorizontally(scrollDirectionInt)))
                    {
                        handler.Post(() => recyclerView.ScrollBy(scrollX, scrollY));
                    }
                    else
                    {
                        currentScrollDirection = ScrollDirection.None;
                        timer?.Cancel();
                        timer?.Dispose();
                        timer = null;
                    }
                };
                //start the timer to do the action every 25ms
                timer.ScheduleAtFixedRate(new TimerTask(action), 0, 25);



                //the C# async way of doing it 
                //while (currentScrollDirection == newDirection
                //        && (recyclerView.CanScrollVertically(scrollDirectionInt) 
                //        || recyclerView.CanScrollHorizontally(scrollDirectionInt))) {
                //    //Not sure how this would be done in a native android way
                //    recyclerView.ScrollBy(scrollX, scrollY);
                //    await Task.Delay(25); //Go back to the ui thread for 25ms before scrolling another increment
                //}
                //currentScrollDirection = ScrollDirection.None;

            }
            else if (newDirection == ScrollDirection.None)
            {
                currentScrollDirection = ScrollDirection.None;
                timer?.Cancel();
                timer?.Dispose();
                timer = null;
            }
        }


        private class TimerTask : Java.Util.TimerTask
        {
            Action action;
            public TimerTask(Action action)
            {
                this.action = action;
            }
            public override void Run()
            {
                action?.Invoke(); ;
            }
        }

        /// <summary>
        /// Run this on DragAction.Ended 
        /// </summary>
        private void DragEnded()
        {
            currentScrollDirection = ScrollDirection.None;
            dragEvent = null;
        }

        /// <summary>
        /// Run on DragAction.Started
        /// </summary>
        /// <param name="dragEvent"></param>
        private void DragStarted(DragEvent dragEvent)
        {
            this.dragEvent = dragEvent;
        }


        /// <summary>
        /// This allows a newly bound cell that was previously off screen when
        /// the drag started to trigger drag events.
        /// Add this to OnBindViewHolder in override of RecyclerView.Adapter
        /// </summary>
        /// <param name="view"></param>
        public void PrepareCellView(View view)
        {
            handler.Post(() =>
            {
                if (dragEvent != null && view.Parent is ViewGroup parent)
                {
                    parent.DispatchDragEvent(dragEvent);
                }
            });
        }


        /// <summary>
        /// This allows a newly bound cell that was previously off screen when
        /// the drag started to trigger drag events.
        /// Add this to OnBindViewHolder in override of RecyclerView.Adapter
        /// </summary>
        /// <param name="view"></param>
        public void PrepareCellView(RecyclerView.ViewHolder holder)
        {
            handler.Post(() =>
            {
                if (dragEvent != null && holder.ItemView.Parent is ViewGroup parent)
                {
                    parent.DispatchDragEvent(dragEvent);
                }
            });
        }


        /// <summary>
        /// Find a recycler view in the views view tree
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        private static RecyclerView FindRecyclerViewInTree(View view)
        {
            if (view == null)
            {
                return null;
            }
            else if (view is RecyclerView recyclerView)
            {
                return recyclerView;
            }
            else
            {
                return FindRecyclerViewInTree(view.Parent as View);
            }

        }

        private static bool IsViewChildOfView(View child, View parent)
        {
            if (child == null || parent == null)
            {
                return false;
            }
            else if (child.Parent is View childsParent && childsParent == parent)
            {
                return true;
            }
            else
            {
                return IsViewChildOfView(child.Parent as View, parent);
            }
        }

        /// <summary>
        /// Get's a point in a view relative to another view
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="toView"></param>
        /// <param name="fromPoint"></param>
        /// <returns></returns>
        public static Point GetPointRelativeTo(View fromView, View toView, Point fromPoint)
        {
            var toViewRect = GetRectOnScreen(toView);
            var fromViewRect = GetRectOnScreen(fromView);
            var pointInToView = new Point(fromViewRect.Left - toViewRect.Left + fromPoint.X,
                fromViewRect.Top - toViewRect.Top + fromPoint.Y);
            return pointInToView;
        }

        /// <summary>
        /// Gets the views Rect relative to the screen
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static Rect GetRectOnScreen(View view)
        {
            int[] l = new int[2];
            view.GetLocationOnScreen(l);
            int x = l[0];
            int y = l[1];
            Rect rect = new Rect(x, y, view.Width + x, view.Height + y);
            return rect;
        }

    }

}