using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace BPMOPMobile.Droid.Core.Class
{
    public class BackgroundColorSpan : Java.Lang.Object, ILineBackgroundSpan
    {
        private float padding;
        private float radius;

        private RectF rect = new RectF();
        private Paint paint = new Paint();
        private Paint paintStroke = new Paint();
        private Path path = new Path();

        private float prevWidth = -1f;
        private float prevLeft = -1f;
        private float prevRight = -1f;
        private float prevBottom = -1f;
        private float prevTop = -1f;

        public BackgroundColorSpan(string backgroundColor,
                               float padding,
                               float radius)
        {
            this.padding = padding;
            this.radius = radius;



            paint.Color = Color.ParseColor(backgroundColor);
            paintStroke.Color = Color.ParseColor(backgroundColor);
        }



        public void DrawBackground(Canvas c, Paint p, int left, int right, int top, int baseline, int bottom, ICharSequence text, int start, int end, int lnum)
        {
            float width = p.MeasureText(text, start, end) + 2f * padding;
            float shift = (right - width) / 2f;



            rect.Set(shift, top, right - shift, bottom);



            if (lnum == 0)
            {
                c.DrawRoundRect(rect, radius, radius, paint);
            }
            else
            {
                path.Reset();
                float dr = width - prevWidth;
                float diff = -Java.Lang.Math.Signum(dr) * Java.Lang.Math.Min(2f * radius, Java.Lang.Math.Abs(dr / 2f)) / 2f;
                path.MoveTo(
                                prevLeft, prevBottom - radius
                );



                path.CubicTo(
                                prevLeft, prevBottom - radius,
                                prevLeft, rect.Top,
                                prevLeft + diff, rect.Top
                );
                path.LineTo(
                                rect.Left - diff, rect.Top
                );
                path.CubicTo(
                                rect.Left - diff, rect.Top,
                                rect.Left, rect.Top,
                                rect.Left, rect.Top + radius
                );
                path.LineTo(
                                rect.Left, rect.Bottom - radius
                );
                path.CubicTo(
                                rect.Left, rect.Bottom - radius,
                                rect.Left, rect.Bottom,
                                rect.Left + radius, rect.Bottom
                );
                path.LineTo(
                                rect.Right - radius, rect.Bottom
                );
                path.CubicTo(
                                rect.Right - radius, rect.Bottom,
                                rect.Right, rect.Bottom,
                                rect.Right, rect.Bottom - radius
                );
                path.LineTo(
                                rect.Right, rect.Top + radius
                );
                path.CubicTo(
                                rect.Right, rect.Top + radius,
                                rect.Right, rect.Top,
                                rect.Right + diff, rect.Top
                );
                path.LineTo(
                                prevRight - diff, rect.Top
                );
                path.CubicTo(
                                prevRight - diff, rect.Top,
                                prevRight, rect.Top,
                                prevRight, prevBottom - radius
                );
                path.CubicTo(
                                prevRight, prevBottom - radius,
                                prevRight, prevBottom,
                                prevRight - radius, prevBottom

                );
                path.LineTo(
                                prevLeft + radius, prevBottom
                );
                path.CubicTo(
                                prevLeft + radius, prevBottom,
                                prevLeft, prevBottom,
                                prevLeft, rect.Top - radius
                );
                c.DrawPath(path, paintStroke);
            }



        }
    }
}