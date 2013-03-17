using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FineUploader
{
    public class UploadController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public FineUploaderResult UploadFile(FineUpload upload)
        {
            var dir = HttpContext.Server.MapPath("~/App_Data");
            var fileName = Guid.NewGuid().ToString("N");
            var filePath = Path.Combine(dir, fileName);

            try
            {
                var image = Image.FromStream(upload.InputStream);
                image.Save(filePath, ImageFormat.Png);
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, error: ex.Message);
            }

            return new FineUploaderResult(true, new { fileName = fileName });
        }

        public ActionResult GetImage(String fileName)
        {
            var dir = HttpContext.Server.MapPath("~/App_Data/");
            var path = Path.Combine(dir, fileName);
            return base.File(path, "image/png");
        }
    }
}
