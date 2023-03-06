using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.IOSClass
{
    public class PresentationController : UIPresentationController
    {
        private UIView dimmingView;
        private CGSize showSize;
        private float YPosition;

        #region override
        public override CGRect FrameOfPresentedViewInContainerView
        {
            get
            {
                var containerBounds = this.ContainerView.Bounds;

                var presentedViewFrame = CGRect.Empty;
                presentedViewFrame.Size = showSize;
                presentedViewFrame.X = (containerBounds.Width - presentedViewFrame.Width) / 2;
                if (YPosition == 0)
                {
                    presentedViewFrame.X = (containerBounds.Width / 2) - (presentedViewFrame.Width / 2);
                    presentedViewFrame.Y = (containerBounds.Height / 2) - (presentedViewFrame.Height / 2);
                    this.PresentedView.Layer.CornerRadius = presentedViewFrame.Size.Width / 2;
                }
                else
                {
                    presentedViewFrame.Y = containerBounds.Height - YPosition;
                }

                this.PresentedView.Layer.CornerRadius = 2;
                this.PresentedView.ClipsToBounds = true;

                return presentedViewFrame;
            }
        }

        public override void PresentationTransitionWillBegin()
        {
            this.dimmingView.Frame = this.ContainerView.Bounds;
            this.dimmingView.Alpha = 0;

            this.ContainerView.InsertSubview(this.dimmingView, 0);
            var coordinator = this.PresentedViewController.GetTransitionCoordinator();
            if (coordinator != null)
            {
                coordinator.AnimateAlongsideTransition((context) =>
                {
                    this.dimmingView.Alpha = 1;
                },
                (context) =>
                { });
            }
            else
            {
                this.dimmingView.Alpha = 1;
            }
        }

        public override void DismissalTransitionWillBegin()
        {
            var coordinator = this.PresentedViewController.GetTransitionCoordinator();
            if (coordinator != null)
            {
                coordinator.AnimateAlongsideTransition((context) =>
                {
                    this.dimmingView.Alpha = 0;
                },
                (context) => { });
            }
            else
            {
                this.dimmingView.Alpha = 0;
            }
        }


        #endregion

        public PresentationController(UIViewController presentingViewController, UIViewController presentedViewController, CGSize presentedViewControllerSize, float _yPosition)
            : base(presentingViewController, presentedViewController)
        {
            showSize = presentedViewControllerSize;
            YPosition = _yPosition;
            SetUpDimmingView();
        }

        void SetUpDimmingView()
        {
            this.dimmingView = new UIView();
            this.dimmingView.BackgroundColor = UIColor.Black.ColorWithAlpha(0.4f);
            this.dimmingView.Alpha = 0;

            var dimmingViewTapGestureRecogniser = new UITapGestureRecognizer(this.DimmingViewTapped);

            this.dimmingView.AddGestureRecognizer(dimmingViewTapGestureRecogniser);
        }

        private void DimmingViewTapped(UIGestureRecognizer gesture)
        {
            //if (gesture.State == UIGestureRecognizerState.Recognized)
            //{
            //    this.PresentingViewController.DismissViewControllerAsync(true);
            //}
        }
    }
}