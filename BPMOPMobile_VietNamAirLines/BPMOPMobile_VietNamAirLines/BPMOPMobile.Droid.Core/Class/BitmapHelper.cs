using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BPMOPMobile.Droid.Core.Class
{
    public static class BitmapHelper
    {
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options
            {
                ////InPurgeable = true,
                InJustDecodeBounds = true
            };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            options.InBitmap = BitmapFactory.DecodeFile(fileName, options);
            options.InBitmap = modifyOrientation(options.InBitmap, fileName);
            //Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);
            //return resizedBitmap;

            return options.InBitmap;
        }

        /// <summary>
        /// Download ảnh về và resize trước khi lưu lại, nếu down fail trả ra null
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="qualityPercent">% chất lượng ảnh</param>
        /// <param name="localpath"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public static Bitmap DownloadAndResizeBitmap(string url, int widthPixel, string localpath, HttpClient httpClient)
        {
            Bitmap bitmap = null;
            try
            {
                if (httpClient != null)
                {
                    Uri filepath = new Uri(url);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, filepath);
                    request.Headers.Add("ACCEPT", "*/*");
                    HttpResponseMessage response = httpClient.SendAsync(request).Result;
                    if (response.StatusCode != HttpStatusCode.OK) return null;

                    var byteArray = response.Content.ReadAsByteArrayAsync().Result;
                    bitmap = BitmapFactory.DecodeByteArray(byteArray, 0, byteArray.Length);

                    int _qualityPercent = (int)(((float)widthPixel / (float)bitmap.Width) * 100);
                    using (var fs = new FileStream(localpath, FileMode.OpenOrCreate))
                    {
                        bitmap.Compress(Bitmap.CompressFormat.Png, _qualityPercent, fs); // Xài PNG ko bị lỗi transparent background -> black
                        fs.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return bitmap;
        }

        public static Bitmap modifyOrientation(Bitmap bitmap, String image_absolute_path)
        {
            ExifInterface ei = new ExifInterface(image_absolute_path);
            int orientation = ei.GetAttributeInt(ExifInterface.TagOrientation, (int)Android.Media.Orientation.Normal);

            switch (orientation)
            {
                case (int)Android.Media.Orientation.Rotate90:
                    return rotate(bitmap, 90);



                case (int)Android.Media.Orientation.Rotate180:
                    return rotate(bitmap, 180);



                case (int)Android.Media.Orientation.Rotate270:
                    return rotate(bitmap, 270);



                case (int)Android.Media.Orientation.FlipHorizontal:
                    return flip(bitmap, true, false);



                case (int)Android.Media.Orientation.FlipVertical:
                    return flip(bitmap, false, true);



                default:
                    return bitmap;
            }
        }

        public static Bitmap rotate(Bitmap bitmap, float degrees)
        {
            Matrix matrix = new Matrix();
            matrix.PostRotate(degrees);
            return Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
        }

        public static Bitmap flip(Bitmap bitmap, bool horizontal, bool vertical)
        {
            Matrix matrix = new Matrix();
            matrix.PreScale(horizontal ? -1 : 1, vertical ? -1 : 1);
            return Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
        }
    }
}