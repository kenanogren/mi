using mi.Areas.Panel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mi.Models.ViewModel
{
    public class BasketItem
    {
        public Guid Id { get; set; }
        public tbl_product product { get; set; }
        public int quantity { get; set; }
        public DateTime DateCreated { get; set; }

    }
}