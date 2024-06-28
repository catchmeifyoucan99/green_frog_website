using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace greenfrog.Models
{
    public class HomeModel
    {
        public List<SanPham> ListProduct { get; set; }
        public List<DanhMuc> ListCategory { get; set; }
    }
}