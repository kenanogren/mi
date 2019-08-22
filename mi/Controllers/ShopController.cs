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
using PagedList;
using mi.Models.ViewModel;

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
        public ActionResult Products(int?categoryId, int? page, int? PageSize)
        {
            var result = repoProduct.GetAll();
            int _page = page ?? 1;
            int _pageSize = PageSize ?? 6;
            if (categoryId != null)
            {
                result = result.Where(x=>x.categoryId==categoryId).ToList();
            }
            return View(result.ToPagedList(_page, _pageSize));
        }
        public PartialViewResult PartialCategory()
        {
            return PartialView(repoCategory.GetAll());
        }

        public ActionResult ProductDetail(int productId)
        {
            return View(repoProduct.Get(productId));
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

        [NonAction]
        private int isExistInCard(int id)
        {
            List<BasketItem> card = (List<BasketItem>)Session["card"];
            for (int i = 0; i < card.Count; i++)
                if (card[i].product.productId.Equals(id))
                    return i;
            return -1;
        }

        public ActionResult AddCard(int productId, int quantity)
        {
            tbl_product _product = repoProduct.Get(productId);
            if (Session["card"] == null)
            {
                List<BasketItem> Card = new List<BasketItem>();
                Card.Add(new BasketItem()
                {
                    Id = Guid.NewGuid(),
                    product = _product,
                    quantity = quantity,
                    DateCreated = DateTime.Now
                });
                Session["card"] = Card;
            }
            else
            {
                List<BasketItem> card = (List<BasketItem>)Session["card"];
                // sepette eklenen ürünün  sepetteki sıra numarasına bakılır. varsa sepetteki sıra no gönderilir, yoksa -1 değeri gönderilir.
                int index = isExistInCard(productId);
                // sepette eklenen ürün varsa
                if (index != -1)
                {
                    // sadece adedini gelen quantity kadar arttıracak.
                    card[index].quantity += quantity;
                }
                // sepette girilen ürün yoksa 
                else
                {
                    // sepete ekle
                    card.Add(new BasketItem
                    {
                        product = _product,
                        quantity = quantity,
                        DateCreated = DateTime.Now
                    });
                }
                Session["card"] = card;

            }
            return Redirect(Request.UrlReferrer.PathAndQuery);
            // return Json((List<BasketItem>)Session["card"],JsonRequestBehavior.AllowGet);
        }
        // sepetteki elemanı silme
        public void Remove(int productId)
        {
            List<BasketItem> card = (List<BasketItem>)Session["card"];
            if (card.Exists(x => x.product.productId == productId))
            {
                int index = isExistInCard(productId);
                card.RemoveAt(index);
                Session["card"] = card;
            }

        }

        public ActionResult Basket()
        {
            return View((List<BasketItem>)Session["card"]);
        }
    }
}