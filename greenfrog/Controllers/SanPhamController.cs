using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using greenfrog.Models;

namespace greenfrog.Controllers
{
    public class SanPhamController : Controller
    {

        // GET: SanPham
        doanwebEntities data = new doanwebEntities();

        public ActionResult ListSanPham()
        {
            HomeModel objHomeModel = new HomeModel();

            objHomeModel.ListCategory = data.DanhMucs.ToList();
            objHomeModel.ListProduct = data.SanPhams.ToList();

            return View(objHomeModel);
        }

        public ActionResult TatCaSanPham(int? page)
        {
            if (page == null) page = 1;
            var all_sach = (from s in data.SanPhams select s).OrderBy(m => m.masp);
            int pageSize = 8;
            int pageNum = page ?? 1;
            return View(all_sach.ToPagedList(pageNum, pageSize));
        }

        public ActionResult SanPham(int? page)
        {
            if (page == null) page = 1;
            var all_sach = (from s in data.SanPhams select s).OrderBy(m => m.masp);
            int pageSize = 8;
            int pageNum = page ?? 1;
            return View(all_sach.ToPagedList(pageNum, pageSize));
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

            HttpCookie nameCookie = Request.Cookies["searchname"];

            Random rnd = new Random();

            //If Cookie exists fetch its value.
            var sub = (object)null;

            if(nameCookie == null)
            {
                sub = (from sp in sanpham
                        orderby rnd.Next()
                        select new ViewModel
                        {
                            sanpham = sp
                        }).Take(5);
            }
            else
            {
                string name = Server.UrlDecode(nameCookie.Value);

                string[] arrListStr = name.Split(',');

                //cat dau [ va \
                string[] words, words2, words3;
                if (arrListStr[0].Contains("[") || arrListStr[0].Contains("\""))
                {
                    words = arrListStr[0].Split(new[] { "[", "\"" }, StringSplitOptions.RemoveEmptyEntries);

                }
                else
                {
                    words = null;
                }

                string sp1 = String.Join(" ", words);

                //cat dau srting 2
                if (arrListStr[1].Contains("[") || arrListStr[0].Contains("\""))
                {
                    words2 = arrListStr[1].Split(new[] { "[", "\"" }, StringSplitOptions.RemoveEmptyEntries);

                }
                else
                {
                    words2 = null;
                }

                string sp2 = String.Join(" ", words2);

                //cat dau srting 3
                if (arrListStr[2].Contains("]") || arrListStr[0].Contains("\""))
                {
                    words3 = arrListStr[2].Split(new[] { "]", "\"" }, StringSplitOptions.RemoveEmptyEntries);

                }
                else
                {
                    words3 = null;
                }

                string sp3 = String.Join(" ", words3);


                sub = (from sp in sanpham
                       orderby rnd.Next()
                       where (sp.tensp.Contains(sp1) || sp.tensp.Contains(sp2) || sp.tensp.Contains(sp3))
                       select new ViewModel
                       {
                           sanpham = sp
                       }).Take(5);
            }


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
            ViewBag.Sub = sub;
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
            data.DanhGias.Add(review);
            data.SaveChanges();
            return RedirectToAction("Details", "SanPham", new { id = review.id_sp });
        }


        public ActionResult productDanhmuc(int id, int? page)
        {
            if (page == null) page = 1;
            var listpr = data.SanPhams.Where(n => n.idDanhmuc == id).ToList();
            int pageSize = 5;
            int pageNum = page ?? 1;
            return View(listpr.ToPagedList(pageNum, pageSize));

        }

        [ChildActionOnly]
        public PartialViewResult DanhMuc()
        {
            HomeModel objHomeModel = new HomeModel();

            objHomeModel.ListCategory = data.DanhMucs.ToList();
            objHomeModel.ListProduct = data.SanPhams.ToList();
            return PartialView(objHomeModel);
        }

    }
}
