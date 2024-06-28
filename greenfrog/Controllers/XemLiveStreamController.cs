using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using greenfrog.Models;
namespace greenfrog.Controllers
{
    public class XemLiveStreamController : Controller
    {
        // GET: XemLiveStream

        public ActionResult Index()
        {
            doanwebEntities data = new doanwebEntities();
            var D_sach = data.LiveStreams.OrderByDescending(a => a.idLiveStream).ToList();
            return View(D_sach);
        }
    }
}