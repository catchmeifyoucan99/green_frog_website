using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using greenfrog.Models;
using System.Security.Cryptography;
using System.Text;
using greenfrog.Common;
using System.Net.Mail;
using System.Net;

namespace greenfrog.Controllers
{
    public class NguoiDungController : Controller
    {
        // GET: NguoiDung
        doanwebEntities data = new doanwebEntities();

        [HttpGet]
        public ActionResult DangKy()
        {
            return View();

        }
        [HttpPost]
        public ActionResult DangKy(FormCollection collection, KhachHang kh)
        {
            var hoten = collection["hoten"];
            var tendangnhap = collection["tendangnhap"];

            var matkhau1 = collection["matkhau"];
            var matkhau = Encryptor.MD5Hash(matkhau1);

            doanwebEntities data = new doanwebEntities();
            var dblist = data.KhachHangs.ToList();


            if (matkhau1.Length < 6)
            {
                ViewData["Length>6"] = "Mật khẩu phải dài hơn 5 kí tự";
            }
            else if (hoten.Length < 4)
            {
                ViewData["hoten>3"] = "Họ tên phải dài hơn 3 kí tự";
            }
            else if (tendangnhap.Length < 4)
            {
                ViewData["tendangnhap>3"] = "Tên đăng nhập phải dài hơn 3 kí tự";
            }
            else
            {
                var MatKhauXacNhan1 = collection["MatKhauXacNhan"];
                var MatKhauXacNhan = Encryptor.MD5Hash(MatKhauXacNhan1);

                var email = collection["email"];
                var diachi = collection["diachi"];
                var dienthoai = collection["dienthoai"];
                var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["ngaysinh"]);
                if (String.IsNullOrEmpty(MatKhauXacNhan))
                {
                    ViewData["NhapMKXN"] = "Phải nhập mật khẩu xác nhận";
                }

                else
                {
                    foreach (var item in dblist)
                    {
                        if (item.tendangnhap == tendangnhap)
                        {
                            ViewData["existtk"] = "Tên tài khoản đã tồn tại";
                            continue;
                        }

                        //if (item.email == email)
                        //{
                        //    ViewData["existemail"] = "Email đã được sử dụng cho tài khoản khác";
                            
                        //}

                        //if (!matkhau.Equals(MatKhauXacNhan))
                        //{
                        //    ViewData["MatKhauGiongNhau"] = "Mật khẩu và mật khẩu xác nhận phải giống nhau";
                        //}


                        //if ((DateTime.Now.Year - DateTime.Parse(ngaysinh.ToString()).Year) < 18)
                        //{
                        //    ViewData["calcns"] = "Khách hàng phải đảm bảo đủ 18 tuổi";
                        //}

                        else
                        {
                            kh.hoten = hoten;
                            kh.tendangnhap = tendangnhap;
                            kh.matkhau = matkhau;
                            kh.email = email;
                            kh.diachi = diachi;
                            kh.dienthoai = dienthoai;
                            kh.ngaysinh = DateTime.Parse(ngaysinh);
                            kh.RoleID = 2;
                            kh.status = 1;

                            data.KhachHangs.Add(kh);
                            data.SaveChanges();

                            return RedirectToAction("DangNhap");
                        }
                    }

                }
            }

            return this.DangKy();
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();

        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var tendangnhap = collection["tendangnhap"];
            var matkhau1 = collection["matkhau"];

            var matkhau = Encryptor.EncryptMD5(matkhau1);

            KhachHang kh =
                data.KhachHangs.FirstOrDefault(n => n.tendangnhap == tendangnhap && n.matkhau == matkhau);

            if (kh != null)
            {
                Session["TaiKhoan"] = kh;
                Session["Username"] = tendangnhap;

                if (kh.RoleID == 1)
                {
                    Session["Admin"] = kh;
                    return RedirectToAction("Index", "Admin");
                }
                else if (kh.RoleID == 3)
                {
                    Session["Staff"] = kh;
                    return RedirectToAction("Index", "Admin");
                }
                else if (kh.status == 2)
                {
                    ViewData["blockAccount"] = "Tài khoản của bạn đã bị khóa do có hành vi bất thường";
                    Session["TaiKhoan"] = null;
                    Session["Username"] = null;
                    return View();
                }

                else
                {
                    return RedirectToAction("ListSanPham", "SanPham");
                }

            }
            else
            {
                ViewBag.msg = "tên đăng nhập hoặc mật khẩu không chính xác";
                return View();
            }
        }

        public ActionResult DangXuat()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            Session.Clear();
            return RedirectToAction("ListSanPham", "SanPham");
        }


        [HttpPost]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/NguoiDung/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("kjitvaf4@gmail.com", "greenfrog");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "123456789Abc";

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Tai khoan da duoc tao thanh cong";
                body = "chua lam";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset password";
                body = "Bạn vừa gửi link xác thực tài khoản, Hãy click vào link bên dưới để lấy lại mật khẩu<br>" +
                    "<a href=" + link + ">Reset password</a>";
            }

            /*var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);*/
            /*smtp - mail.outlook.com*/

            MailMessage mc = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["Email"].ToString(), emailID);
            mc.Subject = subject;
            mc.Body = body;
            mc.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
            smtp.Timeout = 1000000;
            //smtp.Timeout = 1000;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            NetworkCredential nc = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Email"].ToString(), System.Configuration.ConfigurationManager.AppSettings["Password"].ToString());
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            smtp.Send(mc);

        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

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

        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {

            if (IsValidEmail(EmailID))
            {
                string message = "";
                doanwebEntities data = new doanwebEntities();
                var account = data.KhachHangs.Where(a => a.email == EmailID).FirstOrDefault();

                if (account != null)
                {
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.email, resetCode, "ResetPassword");
                    account.resetpasswordcode = resetCode;

                    data.SaveChanges();

                    message = "Reset password link đã được gửi đến email của bạn";
                }


                ViewBag.Message = message;
                return View();
            }
            else
            {
                TempData["mgss"] = "Email không hợp lệ";
                return View();
            }

        }

        public ActionResult ResetPassword(string id)
        {
            doanwebEntities data = new doanwebEntities();
            var user = data.KhachHangs.Where(a => a.resetpasswordcode == id).FirstOrDefault();
            if (user != null)
            {
                ResetPasswordModel model = new ResetPasswordModel();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                doanwebEntities data = new doanwebEntities();
                var user = data.KhachHangs.Where(a => a.resetpasswordcode == model.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    user.matkhau = Encryptor.MD5Hash(model.NewPassword);
                    user.resetpasswordcode = "";

                    data.SaveChanges();
                    message = "Cập nhập mật khẩu thành công";
                }
            }
            else
            {
                message = "Cập nhập mật khẩu thất bại";
            }
            ViewBag.Message = message;
            return View(model);
        }
    }
}