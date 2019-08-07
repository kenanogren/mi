using mi.Areas.Panel.Models;
using mi.Areas.Panel.Models.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mi.Areas.Panel.Controllers
{
    
    public class BrandController : Controller
    {
        // GET: Panel/Brand
        BrandRepository repository = new BrandRepository(new MoriconEntities());

        public ActionResult Index()
        {
            return View(repository.GetAll());
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tbl_brand brand)
        {
            
            repository.Save(brand);
            return RedirectToAction("/");
        }

        public ActionResult Edit(int id)
        {
            return View(repository.Get(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tbl_brand brand)
        {
            if (brand != null)
            {
                
                repository.Update(brand);
            }
            return RedirectToAction("/");
        }
        public ActionResult Delete(int id)
        {
            return View(repository.Get(id));
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategory(int id)
        {
            repository.Delete(repository.Get(id));
            return RedirectToAction("/");
        }
    }
}