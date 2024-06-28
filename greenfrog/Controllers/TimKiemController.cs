using System;
using greenfrog.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Text.RegularExpressions;

namespace greenfrog.Controllers
{
    public class TimKiemController : Controller
    {
        // GET: Student
        doanwebEntities data = new doanwebEntities();

        [HttpGet]
        public ActionResult TimKiem(int? page, string search)
        {
            if (search == null || search == "" || search == " ")
            {
                TempData["thongbaodetrrong"] = "Không được để trống khi tìm kiếm";
            }

            if (search.Length > 30)
            {
                TempData["thongbaododai"] = "Độ dài vượt quá 30 kí tự";
            }

            if (page == null) page = 1;
            var lstSP = data.SanPhams.Where(n => n.tensp.Contains(search)).OrderBy(n => n.tensp);
            int pageSize = 5;
            int pageNum = page ?? 1;

            ViewBag.Search = search;

            return View(lstSP.ToPagedList(pageNum, pageSize));
        }


        public ActionResult KQ(int? page, string search, string searchby)
        {
            if (page == null) page = 1;

            int pageSize = 5;
            int pageNum = page ?? 1;

            if (searchby == "giakm")
            {
                var tblProduct = data.SanPhams.Where(n => n.giakhuyenmai.ToString().Contains(search));
                ViewBag.Search = search;
                ViewBag.Searchby = searchby;
                return View(tblProduct.ToPagedList(pageNum, pageSize));
            }
            else if (searchby == "danhmuc")
            {
                var tblProduct = data.SanPhams.Where(n => n.idDanhmuc.ToString().Contains(search));
                ViewBag.Search = search;
                ViewBag.Searchby = searchby;
                return View(tblProduct.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var tblProduct = data.SanPhams.Where(n => n.tensp.Contains(search));
                ViewBag.Search = search;
                ViewBag.Searchby = searchby;
                return View(tblProduct.ToPagedList(pageNum, pageSize));

            }

        }

    }
}