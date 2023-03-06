using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.IOSClass
{
    public class PresentationDelegate : UIViewControllerTransitioningDelegate
    {
        private CGRect startFrame;
        private CGSize showSize;
        private float YPosition;

        private PresentationController awesomePresentationController;
        private PresentationTransitioning animationTransitioning;
        public PresentationTransitioning AnimationTransitioning
        {
            get
            {
                if (animationTransitioning == null)
                {
                    animationTransitioning = new PresentationTransitioning(this.startFrame);
                }
                return animationTransitioning;
            }
        }

        public PresentationDelegate(CGRect startFrame, CGSize showSize, float _yPosition)
        {
            this.startFrame = startFrame;
            this.showSize = showSize;
            this.YPosition = _yPosition;
        }

        public override UIPresentationController GetPresentationControllerForPresentedViewController(UIViewController presentedViewController, UIViewController presentingViewController, UIViewController sourceViewController)
        {
            if (this.awesomePresentationController == null)
            {
                this.awesomePresentationController = new PresentationController(presentedViewController, presentedViewController, showSize, YPosition);
            }
            return this.awesomePresentationController;
        }

        public override IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController(UIViewController dismissed)
        {
            var transitioning = this.AnimationTransitioning;
            transitioning.IsPresentation = false;
            return transitioning;
        }

        public override IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController(UIViewController presented, UIViewController presenting, UIViewController source)
        {
            var transitioning = this.AnimationTransitioning;
            transitioning.IsPresentation = true;
            return transitioning;
        }
    }
}