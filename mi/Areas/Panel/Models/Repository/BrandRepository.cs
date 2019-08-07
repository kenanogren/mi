using mi.Areas.Panel.Models.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;



namespace mi.Areas.Panel.Models.Repository
{
    public class BrandRepository : IRepository<tbl_brand>
    {

        private MoriconEntities _context;
      // private goldstoreEntities1 goldstoreEntities1;

        public BrandRepository(MoriconEntities Context)
        {
            this._context = Context;
        }

       

        public void Delete(tbl_brand model)
        {
            _context.tbl_brand.Remove(model);
            _context.SaveChanges();
        }

        public tbl_brand Get(int id)
        {
            return _context.tbl_brand.Find(id);
        }

        public List<tbl_brand> GetAll()
        {
            return _context.tbl_brand.ToList();
        }

        public void Save(tbl_brand model)
        {
            _context.tbl_brand.Add(model);
            _context.SaveChanges();
        }

        public void Update(tbl_brand model)
        {
            tbl_brand old = Get(model.brandId);
            _context.Entry(old).State = EntityState.Detached;
            _context.Entry(model).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}