using greenfrog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace greenfrog.Controllers
{
    public class XemLichHenController : Controller
    {
        // GET: XemLichHen
        doanwebEntities data = new doanwebEntities();

       
        public ActionResult Index()
        {
            var userName = (string)Session["Username"];

            var all_danhmuc = from dv in data.DichVus
                              join kh in data.KhachHangs on dv.makh equals kh.makh
                              where kh.tendangnhap == userName
                              select dv;

            return View(all_danhmuc.ToList());
        }


        public ActionResult HuyDichVu(int id)
        {
            var D_sach = data.DichVus.First(m => m.iddichvu == id);
            return View(D_sach);
        }
        [HttpPost]
        public ActionResult HuyDichVu(int id, FormCollection collection)
        {
            try
            {
                var D_danhmuc2 = data.DichVus.Where(m => m.iddichvu == id).First();

                if (D_danhmuc2.trangthai == "đang chờ")
                {

                    data.DichVus.Remove(D_danhmuc2);
                    data.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["thongbao1"] = "<script>alert('Đã tiếp nhận thông tin khách hàng không được xóa');</script>";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e) { return View("Error" + e); }


        }
    }
}