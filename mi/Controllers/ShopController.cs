using mi.Areas.Panel.Models;
using mi.Areas.Panel.Models.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mi.Controllers
{
    public class ShopController : Controller
    {
        ProductRepository repoProduct = new ProductRepository(new MoriconEntities());
        CategoryRepository repoCategory = new CategoryRepository(new MoriconEntities());
        // GET: Sho
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Products(int?categoryId)
        {
            var result = repoProduct.GetAll();
            if (categoryId != null)
            {
                result = result.Where(x=>x.categoryId==categoryId).ToList();
            }
            return View(result);
        }
        public PartialViewResult PartialCategory()
        {
            return PartialView(repoCategory.GetAll());
        }
        public ActionResult Thumbnail(int width, int height, int Id, int _imageId)
        {
            byte[] photo = repoProduct.Get(Id).tbl_productImage.FirstOrDefault(x => x.imageId == _imageId).image;
            var base64 = Convert.ToBase64String(photo);
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);

            using (var newImage = new Bitmap(width, height))
            using (var graphics = Graphics.FromImage(newImage))
            using (var stream = new MemoryStream())
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(image, new Rectangle(0, 0, width, height));
                newImage.Save(stream, ImageFormat.Png);
                return File(stream.ToArray(), "image/png");
            }

        }

    }
}