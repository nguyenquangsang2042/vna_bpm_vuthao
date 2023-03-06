using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;

namespace BPMOPMobile.Droid.Core.Class
{
    public class UnderLayoutButton
    {
        private int imageResId, textSize, pos, drawableSize;
        private string text, color;
        private RectF clickRegion;
        private UnderLayoutButtonListener listener;

        private Context context;
        private Resources resources;

        public UnderLayoutButton(Context context, string text, int textSize, int imageResId, string color, UnderLayoutButtonListener listener, int drawableSize = 40)
        {
            this.context = context;
            this.text = text;
            this.textSize = textSize;
            this.imageResId = imageResId;
            this.color = color;
            this.listener = listener;
            this.drawableSize = drawableSize;
            resources = context.Resources;
        }

        public bool OnClick(float x, float y)
        {
            if (clickRegion != null && clickRegion.Contains(x, y))
            {
                listener.OnClick(pos);
                return true;
            }
            return false;
        }

        public void OnDraw(Canvas c, RectF rectF, int pos)
        {
            Paint p = new Paint();
            p.Color = Color.ParseColor(color);
            c.DrawRect(rectF, p);

            // Text

            p.Color = Color.White;
            p.TextSize = textSize;

            Rect r = new Rect();
            float cHeight = rectF.Height();
            float cWidth = rectF.Width();
            p.TextAlign = Paint.Align.Left;
            p.GetTextBounds(text, 0, text.Length, r);
            float x = 0, y = 0;

            if (imageResId == 0) // không có Image
            {
                x = cWidth / 2f - r.Width() / 2f - r.Left;
                y = cHeight / 2f + r.Height() / 2f - r.Bottom;
                c.DrawText(text, rectF.Left + x, rectF.Top + y, p);
            }
            else
            {
                ////Drawable d = ContextCompat.GetDrawable(context, imageResId);
                ////Bitmap bitmap = DrawableToBitmap(d);
                //////bitmap.Height = 50;
                //////bitmap.Width = 50;
                ////p.SetColorFilter(new PorterDuffColorFilter(new Color(ContextCompat.GetColor(context, Resource.Color.clWhite)), PorterDuff.Mode.SrcIn)); // Tint White
                //////c.DrawBitmap(bitmap, (rectF.Left + rectF.Right) / 2, (rectF.Top + rectF.Bottom) / 2, p);

                ////c.DrawBitmap(bitmap, ((rectF.Left + rectF.Right) / 2) - 30, ((rectF.Top + rectF.Bottom) / 2) - 20, p);

                Drawable d = ContextCompat.GetDrawable(context, imageResId);
                Bitmap bitmap = DrawableToBitmap(d);
                p.SetColorFilter(new PorterDuffColorFilter(new Color(ContextCompat.GetColor(context, Resource.Color.clWhite)), PorterDuff.Mode.SrcIn)); // Tint White
                c.DrawBitmap(bitmap, ((rectF.Left + rectF.Right) / 2) - (bitmap.Width / 2), ((rectF.Top + rectF.Bottom) / 2) - (bitmap.Height / 2), p);
            }
            clickRegion = rectF;
            this.pos = pos;

        }

        private Bitmap DrawableToBitmap(Drawable d)
        {
            //if (d is BitmapDrawable)
            //    return ((BitmapDrawable)d).Bitmap;

            ////Bitmap bitmap = Bitmap.CreateBitmap((int)(d.IntrinsicWidth / 1.2), (int)(d.IntrinsicHeight / 1.2), Bitmap.Config.Argb8888);
            //Bitmap bitmap = Bitmap.CreateBitmap(100, 100, Bitmap.Config.Argb8888);
            //Canvas canvas = new Canvas(bitmap);
            //d.SetBounds(0, 0, canvas.Width, canvas.Height);
            //d.Draw(canvas);
            //return bitmap;

            Bitmap bitmap = Bitmap.CreateScaledBitmap(((BitmapDrawable)d).Bitmap, drawableSize, drawableSize, true);
            Canvas canvas = new Canvas(bitmap);
            d.SetBounds(0, 0, canvas.Width, canvas.Height);
            d.Draw(canvas);
            return bitmap;
        }
    }
}