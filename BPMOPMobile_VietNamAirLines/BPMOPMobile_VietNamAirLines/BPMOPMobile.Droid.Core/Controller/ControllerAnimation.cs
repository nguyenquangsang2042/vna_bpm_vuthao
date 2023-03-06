using Android.Views;
using Android.Views.Animations;
using System.Runtime.Remoting.Contexts;

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

namespace BPMOPMobile.Droid.Core.Controller
{
    /// <summary>
    /// Author: KhoaHD - Quản lý toàn bộ Animation của App
    /// </summary>
    public static class ControllerAnimation
    {
        public const long _defaultDuration = 500;       // Duration (ms) của Animation
        public const float _defaultHeight = 500f;       // Height của Swipe Animation

        /// <summary>
        /// Animation chạy từ trên xuống đáy màn hình
        /// </summary>
        public static Animation GetAnimationSwipe_TopToBot(View view, float defaultHeight = _defaultHeight, long duration = _defaultDuration)
        {
            Animation anim = new TranslateAnimation(0f, 0f, -(view.Height > 0 ? view.Height : defaultHeight), 0f);
            anim.Duration = duration;
            return anim;
        }

        /// <summary>
        /// Animation chạy từ dưới lên đỉnh màn hình
        /// </summary>
        public static Animation GetAnimationSwipe_BotToTop(View view, float defaultHeight = _defaultHeight, long duration = _defaultDuration)
        {
            Animation anim = new TranslateAnimation(0f, 0f, 0f, -(view.Height > 0 ? view.Height : defaultHeight));
            anim.Duration = duration;
            return anim;
        }

        /// <summary>
        /// Animation chớp tắt -> xài khi click vào view
        /// </summary>
        public static Animation GetAnimationClick_FadeIn(Android.Content.Context _context, long duration = _defaultDuration)
        {
            Animation anim = AnimationUtils.LoadAnimation(_context, Resource.Animation.anim_clickview);
            anim.Duration = duration;
            return anim;
        }

        /// <summary>
        /// Animation thả view xuống đáy màn hình
        /// </summary>
        public static Animation GetAnimationFallDown(Android.Content.Context _context, float defaultHeight = _defaultHeight, long duration = _defaultDuration)
        {
            Animation anim = new TranslateAnimation(0f, 0f, -defaultHeight, 0f);
            anim.Duration = duration;
            return anim;
        }
    }
}