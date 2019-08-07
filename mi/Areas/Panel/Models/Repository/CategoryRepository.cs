using mi.Areas.Panel.Models.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace mi.Areas.Panel.Models.Repository
{
    public class CategoryRepository : IRepository<tbl_category>
    {
        private MoriconEntities _context;

        public CategoryRepository(MoriconEntities Context)
        {
            this._context = Context;
        }

        public void Delete(tbl_category model)
        {
            _context.tbl_category.Remove(model);
            _context.SaveChanges();
        }

        public tbl_category Get(int id)
        {
            return _context.tbl_category.Find(id);
        }

        public List<tbl_category> GetAll()
        {
            return _context.tbl_category.ToList();
        }

        public void Save(tbl_category model)
        {
            _context.tbl_category.Add(model);
            _context.SaveChanges();
        }

        public void Update(tbl_category model)
        {
            tbl_category old = Get(model.categoryId);
            _context.Entry(old).State = EntityState.Detached;
            _context.Entry(model).State = EntityState.Modified;
            _context.SaveChanges();
        }
        
    }
}