
using Fartak;
using Fartak.DbModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fartak.Services
{
    public class UploadFile
    {
        private readonly DataBase context;
        private readonly string userName;
        public UploadFile(IWebHostEnvironment environment, DataBase context, string userName)
        {
            this.context = context;
            this.userName = userName;
        }
        public string upload(IFormFile file)
        {
            if (context.PictureNumbers.Count() == 0)
            {
                PictureNumber picture = new PictureNumber()
                {
                   Number = (context.PictureNumbers.Count()+1).ToString()
                };
                context.PictureNumbers.Add(picture);
                context.SaveChanges();
            }
            var number = (context.PictureNumbers.Count() + 1).ToString();
            PictureNumber picture1 = new PictureNumber() 
            {
                Number=number
            };
            context.PictureNumbers.Add(picture1);
            context.SaveChanges();
            var filnam = file.FileName.Split(".")[0];
            var filnam2 = file.FileName.Split(".")[1];
            if (file == null) return "";
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/images/AllPicture", $"{userName}-{filnam}-{number}.{filnam2}");
            //var path = _webHostEnveronment.WebRootPath + "\\images//admin//";
            using var f = System.IO.File.Create(imagePath);
            file.CopyTo(f);
            return $"{userName}-{filnam}-{number}.{filnam2}";
        }

        public string Uploads(IFormFileCollection files)
        {
            // لیست برای نگهداری نام فایل‌ها
            List<string> fileNames = new List<string>();

            // بررسی اینکه آیا هیچ عکسی وجود ندارد
            if (context.PictureNumbers.Count() == 0)
            {
                PictureNumber picture = new PictureNumber()
                {
                    Number = (context.PictureNumbers.Count() + 1).ToString()
                };
                context.PictureNumbers.Add(picture);
                context.SaveChanges();
            }

            // شماره جدید برای ذخیره‌سازی عکس‌ها
            var number = (context.PictureNumbers.Count() + 1).ToString();

            // پردازش هر فایل در مجموعه فایل‌ها
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // ایجاد نام جدید برای فایل
                    var fileName = $"{number}-{file.FileName}";

                    // مسیر ذخیره‌سازی فایل
                    var filePath = Path.Combine("wwwroot/images/AllPicture", fileName);

                    // ذخیره فایل در سرور
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    // اضافه کردن نام فایل به لیست
                    fileNames.Add(fileName);
                }
            }

            // برگرداندن نام فایل‌ها به صورت یک رشته جداشده با کاما
            return string.Join(",", fileNames);
        }

        public string uUploadVideo(IFormFile file)
        {
            if (file == null) return "";
            
            string videoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/videos", file.FileName);
            //var path = _webHostEnveronment.WebRootPath + "\\videos//admin//";
            //videoPath = videoPath.Split("wwwroot")[1];
            using var f = System.IO.File.Create(videoPath);
            file.CopyTo(f);
            return file.FileName;
        }
        //public string uploadVideo(IFormFile file)
        //{
        //    if (file == null) return "";
        //    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/admin", file.FileName);
        //    //var path = _webHostEnveronment.WebRootPath + "\\images//admin//";
        //    using var f = System.IO.File.Create(imagePath);
        //    file.CopyTo(f);
        //    return file.FileName;
        //}


    }
}
