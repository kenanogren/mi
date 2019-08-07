using mi.Areas.Panel.Models.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace mi.Areas.Panel.Models.Repository
{
    public class ProductRepository : IRepository<tbl_product>
    {
        private MoriconEntities _context;
        public ProductRepository(MoriconEntities Context)
        {
            _context = Context;
        }
        public void Delete(int id)
        {
            // delete içerisine bir id geldiğinde resim silecek
            tbl_productImage img = _context.tbl_productImage.Find(id);
            if (img != null)
            {
                _context.tbl_productImage.Remove(img);
                _context.SaveChanges();
            }

        }
        public void Delete(tbl_product model)
        {
            // delete içerisine bir product geldiğinde product silecek
        
        _context.tbl_product.Remove(model);
            _context.SaveChanges();
        }

        public tbl_product Get(int id)
        {
            return _context.tbl_product.Find(id);
        }

        public List<tbl_product> GetAll()
        {
            return _context.tbl_product.ToList();
        }
        public void Save(tbl_product model)
        {
            _context.tbl_product.Add(model);
            _context.SaveChanges();
        }
        public void Save(tbl_productImage model)
        {
            _context.tbl_productImage.Add(model);
            _context.SaveChanges();
        }

        public void Update(tbl_product model)
        {
            tbl_product old = Get(model.productId);
            _context.Entry(old).State = EntityState.Detached;
            _context.Entry(model).State = EntityState.Modified;
            _context.SaveChanges();
        }
        //Kategori Listesi
        public List<tbl_category> GetCategories()
        {
            return _context.tbl_category.ToList() ;
        }
        //Marka Listesi
        public List<tbl_brand> GetBrands()
        {
            return _context.tbl_brand.ToList();
        }
    }
}