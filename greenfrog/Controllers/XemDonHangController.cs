using greenfrog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using PagedList;

namespace greenfrog.Controllers
{
    public class XemDonHangController : Controller
    {
        // GET: XemDonHang
        doanwebEntities data = new doanwebEntities();


        public ViewResult Index(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 3;

            var username = (string)Session["UserName"];

            var all__danhmuc = from dh in data.DonHangs
                               join kh in data.KhachHangs on dh.makh equals kh.makh
                               where kh.tendangnhap == username
                               select dh;

            return View(all__danhmuc.ToList());
        }



        //---------Detail-------------

        public ActionResult DetailsCTDH2(int id)
        {
            var results = (from t1 in data.ChiTietDonHangs
                           join t2 in data.DonHangs
                           on new { t1.madon } equals
                               new { t2.madon }
                           where t2.madon == id
                           select t1).ToList();

            List<KhachHang> khachhang = data.KhachHangs.ToList();
            List<DonHang> donhang = data.DonHangs.ToList();
            List<ChiTietDonHang> ctdh = data.ChiTietDonHangs.ToList();
            List<SanPham> sanpham = data.SanPhams.ToList();

            var ViewKH2 = from kh in khachhang
                          join dh in donhang
    on kh.makh equals dh.makh
                          where dh.madon == id && kh.makh == dh.makh
                          select new ViewModel
                          {
                              khachhang = kh,
                              donhang = dh
                          };

            var ViewSP = from ct in ctdh
                         join sp in sanpham on ct.masp equals sp.masp
                         join dh in donhang on ct.madon equals dh.madon
                         where ct.madon == id && sp.masp == ct.masp && ct.madon == dh.madon

                         select new ViewModel
                         {
                             sanpham = sp,
                             ctdh = ct,
                             donhang = dh
                         };

            ViewBag.ViewChiTietDH2 = ViewKH2;
            ViewBag.ViewSP = ViewSP;
            return View(results);
        }


        [HttpGet]
        public ActionResult Details(int id)
        {
            doanwebEntities data = new doanwebEntities();
            List<SanPham> sanpham = data.SanPhams.ToList();
            var main = from sp in sanpham
                       where (sp.masp == id)
                       select new ViewModel
                       {
                           sanpham = sp
                       };


            /*List<DanhMuc> danhmuc = data.DanhMucs.ToList();
            var sub = (from sp in sanpham join d in danhmuc
                      on sp.idDanhmuc equals d.idDanhmuc
                       where sp.idDanhmuc == dm
                       select new ViewModel
                       {
                           sanpham = sp,
                           danhmuc = d
                       }).Take(5);
            */
            Random rnd = new Random();
            var _randomizedList = (from sp in sanpham
                                   orderby rnd.Next()
                                   select new ViewModel
                                   {
                                       sanpham = sp
                                   }).Take(5);

            List<KhachHang> khachhang = data.KhachHangs.ToList();
            List<DanhGia> danhgia = data.DanhGias.ToList();
            var ViewDanhGia = from dg in danhgia
                              join kh in khachhang
                              on dg.id_kh equals kh.makh
                              where (dg.id_sp == id && dg.id_kh == kh.makh)
                              select new ViewModel
                              {
                                  danhgia = dg,
                                  khachhang = kh
                              };

            //sp vs thu vien anh
            List<ThuVienAnh> thuvienanh = data.ThuVienAnhs.ToList();
            var thu = from sp in sanpham
                      join tv in thuvienanh
                              on sp.idthuvien equals tv.idthuvien
                      where (sp.masp == id && sp.idthuvien == tv.idthuvien)
                      select new ViewModel
                      {
                          sanpham = sp,
                          thuvienanh = tv
                      };
            ViewBag.thu = thu;

            ViewBag.Main = main;
            //ViewBag.Sub = sub;
            ViewBag.SelectedPostt = _randomizedList;

            ViewBag.ViewDanhGia = ViewDanhGia;

            SanPham product = data.SanPhams.FirstOrDefault(n => n.masp == id);
            Session["IdSp"] = product.masp;

            //tinh danh gia
            ViewBag.CountRate = Countrate(id);

            var data1 = data.DanhGias.Where(d => d.Rating > 0 && d.id_sp == id);
            var rateSum = data1.Sum(d => d.Rating);
            var countRate = data1.Count();

            var tt = rateSum / countRate;
            ViewBag.SLuotRate = tt;

            return View();
        }

        public int Countrate(int id)
        {
            List<SanPham> sanpham = data.SanPhams.ToList();
            List<DanhGia> danhgia = data.DanhGias.ToList();
            int CountRate = (from dg in danhgia
                             join sp in sanpham
                             on dg.id_sp equals sp.masp
                             where (dg.id_sp == id && dg.id_sp == sp.masp && dg.Rating == 5)
                             select new ViewModel
                             {
                                 danhgia = dg,
                                 sanpham = sp
                             }).Count();
            return CountRate;
        }

        [HttpPost]
        public ActionResult SendDanhGia(DanhGia review, double rating, string content)
        {
            string username = Session["Username"].ToString();
            review.Ngaycapnhap = DateTime.Now;
            review.Content = content;
            review.id_kh = data.KhachHangs.Single(
                 a => a.tendangnhap.Equals(username)).makh;
            review.Rating = rating;
            review.id_sp = (int)Session["IdSp"];
            
            review.trangthai = 1;
            data.DanhGias.Add(review);
            data.SaveChanges();
            return RedirectToAction("Details", "XemDonHang", new { id = review.id_sp });
        }



        public ActionResult HuyDon(int id)
        {
            var D_sach = data.DonHangs.First(m => m.madon == id);
            return View(D_sach);
        }
        [HttpPost]
        public ActionResult HuyDon(int id, FormCollection collection)
        {
            try
            {
                var D_danhmuc2 = data.ChiTietDonHangs.Where(m => m.madon == id).ToList();
                var D_danhmuc = data.DonHangs.Where(m => m.madon == id).First();

                if (D_danhmuc.giaohang == "chờ xử lý")
                {
                    foreach (var item in D_danhmuc2)
                    {
                        data.ChiTietDonHangs.Remove(item);
                        data.SaveChanges();
                    }
                    data.DonHangs.Remove(D_danhmuc);
                    data.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["thongbao"] = "<script>alert('Đơn hàng đang được đang xử lý không thể hủy');</script>";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e) { return View("Error" + e); }

        }


    }
}