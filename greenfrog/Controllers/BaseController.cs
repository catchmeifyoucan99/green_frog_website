using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace greenfrog.Controllers
{
    public class BaseController : Controller
    {

        // GET: Base
        public BaseController()
        {
            if (System.Web.HttpContext.Current.Session["Admin"] == null || System.Web.HttpContext.Current.Session["Admin"].Equals(""))
            {
                if (System.Web.HttpContext.Current.Session["Staff"] == null || System.Web.HttpContext.Current.Session["Staff"].Equals(""))
                {
                    System.Web.HttpContext.Current.Response.Redirect("~/NguoiDung/DangNhap");
                }
            }
        }

    }
}