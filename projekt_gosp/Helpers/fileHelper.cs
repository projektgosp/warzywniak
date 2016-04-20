using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace projekt_gosp.Helpers
{
    public static class fileHelper
    {
        public static List<string> addfile(HttpPostedFileBase file)
        {
            var imageName = String.Format("{0:yyyyMMdd-HHmmssfff}", DateTime.Now);
            var extension = System.IO.Path.GetExtension(file.FileName).ToLower();

            string fullSizeName = imageName + extension;
            string thumbName = imageName + "_thumb" + extension;
            //create full path to folder
            var path = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Media/uploads/"), fullSizeName);
            if (!Directory.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/Media/uploads/")))
            {
                Directory.CreateDirectory(System.Web.Hosting.HostingEnvironment.MapPath("~/Media/uploads/"));
            }

            var thumbPath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Media/uploads/"), thumbName);

            using (var img = System.Drawing.Image.FromStream(file.InputStream))
            {
                // Save thumbnail size image, 400 x 400
                saveThumb(img, thumbPath);
            }
            //save file in given path
            file.SaveAs(path);

            string mediaFullSizePath = "/Media/uploads/" + fullSizeName;
            string mediaThumbPath = "/Media/uploads/" + thumbName;

            List<string> output = new List<string>();
            output.Add(mediaFullSizePath);
            output.Add(mediaThumbPath);
            return output;
        }

        public static bool deletefile(string mediaPath)
        {
            string filepath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + mediaPath);

            //remove file
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }

            string directoryPath = Path.GetDirectoryName(filepath);

            if (Directory.Exists(directoryPath))
            {
                if (IsDirectoryEmpty(directoryPath))
                {
                    // second parameter = recursive deleting
                    Directory.Delete(directoryPath, false);
                }
            }

            return true;
        }

        public static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        private static void saveThumb(Image img, string path)
        {
            Size imgSize = resize(img.Size);
            using (System.Drawing.Image newImg = new Bitmap(img, imgSize.Width, imgSize.Height))
            {
                newImg.Save(path, img.RawFormat);
            }
        }

        public static Size resize(Size imgSize)
        {
            Size thumbSize = new Size(400, 400);

            Size finalSize;
            double tempval;

            if (imgSize.Height > thumbSize.Height || imgSize.Width > thumbSize.Width)
            {
                if (imgSize.Height > imgSize.Width)
                    tempval = thumbSize.Height / (imgSize.Height * 1.0);
                else
                    tempval = thumbSize.Width / (imgSize.Width * 1.0);

                finalSize = new Size((int)(tempval * imgSize.Width), (int)(tempval * imgSize.Height));
            }
            else
            {
                finalSize = imgSize;
            }
            return finalSize;
        }
    }
}