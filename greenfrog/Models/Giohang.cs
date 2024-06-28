using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using greenfrog.Models;

namespace greenfrog.Models
{
    public class Giohang
    {
        doanwebEntities data = new doanwebEntities();
        public int masp { get; set; }
        [Display(Name = "Tên sản phẩm")]
        public string tensp { get; set; }

        [Display(Name = "Ảnh bìa")]
        public string hinh { get; set; }

        [Display(Name = "Giá gốc")]
        public Double giaban { get; set; }

        [Display(Name = "Số lượng")]
        public int iSoluong { get; set; }

        [Display(Name = "Giam gia")]
        public Double giamgia { get; set; }

        [Display(Name = "Giá khuyến mại")]
        public Double giakhuyenmai { get; set; }

        [Display(Name = "Thành tiền")]
        public Double dThanhtien {
            get { return iSoluong * giakhuyenmai; }
        }
        public Giohang(int id)
        {
            masp = id;
            SanPham sanpham = data.SanPhams.Single(n => n.masp == masp);
            tensp = sanpham.tensp;
            hinh = sanpham.hinh;
            giaban = double.Parse(sanpham.giaban.ToString());
            giamgia = Convert.ToInt32(sanpham.giamgia);
            giakhuyenmai = double.Parse(sanpham.giakhuyenmai.ToString());
            iSoluong = 1;
        }


    }
}