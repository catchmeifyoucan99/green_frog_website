using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace greenfrog.Models
{
    public class ViewModel
    {
        public KhachHang khachhang { get; set; }
        public ChiTietDonHang ctdh { get; set; }
        public DanhMuc danhmuc { get; set; }
        public DonHang donhang { get; set; }
        public SanPham sanpham { get; set; }
        public KhachHangRole khachhangrole { get; set; }
        public DanhGia danhgia { get; set; }
        public ThuVienAnh thuvienanh { get; set; }
    }
}