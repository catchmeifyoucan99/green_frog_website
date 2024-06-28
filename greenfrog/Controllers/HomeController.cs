using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using greenfrog.Models;

namespace greenfrog.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home

        bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }


         bool IsPhoneNbr(string number)
        {
             string motif = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
            if (number != null) return Regex.IsMatch(number, motif);
            else return false;
        }

        public ActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection collection, DichVu dv)
        {
            doanwebEntities data = new doanwebEntities();
            var ten = collection["hoten"];
            var email = collection["email"];
            var sdt = collection["sdt"];
            var diachi = collection["diachi"];
            var loaidv = collection["loaidv"];
            var ngaydat = DateTime.Parse(collection["ngaydat"]);

            var ngaydat2 = String.Format("{0:MM/dd/yyyy}", collection["ngaydat"]);

            //DateTime dDate;
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            else
            {
                if (ten == "" || diachi == "" || sdt == "" || email == "")
                {
                    TempData["mgss"] = "Không được để trống dữ liệu";
                }
                else if (!IsValidEmail(email))
                {
                    TempData["mgss"] = "Email không hợp lệ";
                    return View();
                }
                else if (!IsPhoneNbr(sdt))
                {
                    TempData["mgss"] = "SĐT không hợp lệ";
                    return View();
                }
               
                //else if (DateTime.TryParse(ngaydat2, out dDate))
                //{
                //    TempData["mgss"] = "Không đúng định dạng ngày tháng năm";
                //}
                else
                {

                    var tendn = Session["Username"];
                    var E_sach2 = data.KhachHangs.FirstOrDefault(m => m.tendangnhap == tendn);

                    if (E_sach2 != null)
                    {
                        dv.makh = E_sach2.makh;
                    }

                    dv.hoten = ten;
                    dv.email = email;
                    dv.sdt = sdt;
                    dv.diachi = diachi;
                    dv.trangthai = "đang chờ";
                    dv.tendichvu = loaidv;
                    dv.ngaydat = ngaydat;
                    data.DichVus.Add(dv);
                    data.SaveChanges();


                    return RedirectToAction("Index");
                }
            }


            return this.Index();
        }


    }
}