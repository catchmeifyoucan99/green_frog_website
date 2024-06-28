using Newtonsoft.Json.Linq;
using greenfrog.Others;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using greenfrog.Models;
using System.Configuration;

namespace greenfrog.Controllers
{
    public class GioHangController : Controller
    {
        doanwebEntities data = new doanwebEntities();

        public List<Giohang> Laygiohang()
        {
            List<Giohang> listGiohang = Session["Giohang"] as List<Giohang>;
            if (listGiohang == null)
            {
                listGiohang = new List<Giohang>();
                Session["Giohang"] = listGiohang;
            }
            return listGiohang;
        }

        public ActionResult ThemGioHang(int id, string strURL)
        {
            List<Giohang> listGiohang = Laygiohang();
            Giohang sanpham = listGiohang.Find(n => n.masp == id);
            SanPham sanpham1 = data.SanPhams.Single(n => n.masp == id);
            if (sanpham == null)
            {
                sanpham = new Giohang(id);
                listGiohang.Add(sanpham);
                TempData["themthanhcong"] = "<script>alert('thêm sản phẩm vào giỏ hàng thành công');</script>";
                return Redirect(strURL);
            }
            else if (sanpham != null && sanpham.iSoluong >= sanpham1.soluongton)
            {
                //sanpham.iSoluong++;
                TempData["msg"] = "<script>alert('Sản phẩm k được vượt quá số lượng tồn');</script>";
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                TempData["themthanhcong"] = "<script>alert('thêm sản phẩm vào giỏ hàng thành công');</script>";
                return Redirect(strURL);
            }
        }

        public ActionResult MuaNgay(int id, FormCollection collection)
        {
            List<Giohang> listGiohang = Laygiohang();
            Giohang sanpham = listGiohang.Find(n => n.masp == id);
            SanPham sanpham1 = data.SanPhams.Single(n => n.masp == id);
            if (sanpham == null)
            {
                sanpham = new Giohang(id);

                listGiohang.Add(sanpham);
                return RedirectToAction("GioHang", "GioHang");
            }
            else if (sanpham != null && sanpham.iSoluong >= sanpham1.soluongton)
            {
                //sanpham.iSoluong++;
                TempData["kmua"] = "<script>alert('Sản phẩm k được vượt quá số lượng tồn');</script>";
                return RedirectToAction("Details/" + id, "SanPham");
            }
            else
            {
                sanpham.iSoluong++;
                return RedirectToAction("GioHang", "GioHang");
            }
        }

        private int TongSoLuong()
        {
            int tsl = 0;
            List<Giohang> listGiohang = Session["Giohang"] as List<Giohang>;
            if (listGiohang != null)
            {
                tsl = listGiohang.Sum(n => n.iSoluong);
            }
            return tsl;
        }

        private int TongSoLuongSanPham()
        {
            int tsl = 0;
            List<Giohang> listGiohang = Session["Giohang"] as List<Giohang>;
            if (listGiohang != null)
            {
                tsl = listGiohang.Count;
            }
            return tsl;
        }

        private double TongTien()
        {
            double tt = 0;
            List<Giohang> listGiohang = Session["Giohang"] as List<Giohang>;
            if (listGiohang != null)
            {
                tt = listGiohang.Sum(n => n.iSoluong * n.giakhuyenmai);
            }
            return tt;
        }


        public ActionResult GioHang()
        {
            List<Giohang> listGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return View(listGiohang);
        }

        public ActionResult GioHangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return PartialView();
        }

        public ActionResult XoaGioHang(int id)
        {
            List<Giohang> listGiohang = Laygiohang();
            Giohang sanpham = listGiohang.SingleOrDefault(n => n.masp == id);
            if (sanpham != null)
            {
                listGiohang.RemoveAll(n => n.masp == id);
                return RedirectToAction("GioHang");
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult CapnhapGiohang(int id, FormCollection collection)
        {
            List<Giohang> listGiohang = Laygiohang();
            Giohang sanpham = listGiohang.SingleOrDefault(n => n.masp == id);
            SanPham sanpham1 = data.SanPhams.Single(n => n.masp == id);

            int inputSL = 0;
            if (!string.IsNullOrEmpty(collection["txtSoLg"].ToString()))
            {
                inputSL = int.Parse(collection["txtSoLg"].ToString());

                if (sanpham != null)
                {

                    if (inputSL > sanpham1.soluongton)
                    {
                        TempData["msg"] = "Sản phẩm k được vượt quá số lượng tồn";
                    }
                    else if (inputSL < 0)
                    {
                        TempData["msg"] = "Sản phẩm k nhỏ hơn 0";
                    }
                    else
                    {
                        sanpham.iSoluong = inputSL;
                    }

                }
                else
                {
                    TempData["msgnull"] = "Sản phẩm k được de trong";
                    sanpham.iSoluong = inputSL;
                }

            }
            else
            {
                TempData["msg"] = "Bạn chưa nhập giá trị";
                inputSL = 1;
            }

            //int inputSL = Convert.ToInt32(collection["txtSoLg"]);
            //sanpham.iSoluong = inputSL;


            return RedirectToAction("GioHang");
        }

        public ActionResult XoaTatCaGioHang()
        {
            List<Giohang> listGiohang = Laygiohang();
            listGiohang.Clear();
            return RedirectToAction("GioHang");
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("SanPham", "ListSanPham");
            }
            List<Giohang> listGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.TongSoLuongSanPham = TongSoLuongSanPham();
            return View(listGiohang);
        }

        //COD
        public ActionResult DatHang(FormCollection collection)
        {
            DonHang dh = new DonHang();
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            SanPham s = new SanPham();
            List<Giohang> gh = Laygiohang();


            dh.makh = kh.makh;
            dh.ngaydat = DateTime.Now;

            string str = "chờ xử lý";
            dh.giaohang = str;
            dh.thanhtoan = "COD";

            data.DonHangs.Add(dh);
            data.SaveChanges();
            foreach (var item in gh)
            {
                ChiTietDonHang ctdh = new ChiTietDonHang();
                ctdh.madon = dh.madon;
                ctdh.masp = item.masp;
                ctdh.soluong = item.iSoluong;

                ctdh.gia = (decimal?)item.giakhuyenmai;

                ctdh.tongsoluong = TongSoLuong();
                ctdh.tonggia = (decimal?)TongTien();
                ctdh.status = 0;
                s = data.SanPhams.Single(n => n.masp == item.masp);
                s.soluongton = s.soluongton - ctdh.soluong;
                data.SaveChanges();

                data.ChiTietDonHangs.Add(ctdh);
            }
            data.SaveChanges();
            Session["Giohang"] = null;

            return RedirectToAction("XacnhanDonhang", "GioHang");
        }

        public ActionResult XacnhanDonhang()
        {
            return View();
        }
        public ActionResult ThatBai()
        {
            return View();
        }


        //MOMO
        public ActionResult Payment()
        {
            //ChiTietDonHang ctdh = new ChiTietDonHang();
            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOJJXC20220503";
            string accessKey = "uEaIt98RK6WmkI98";
            string serectkey = "VfkVpoTG175xhH6ba4hRyVgvzFjSc6id";

            string orderInfo = "";
            string amount = "";

            orderInfo = "DH"; //thong tin san pham
            amount = TongTien().ToString(); //gia tien
            //amount = "1000";
            string returnUrl = "https://localhost:44314/GioHang/XacnhanDonhang";
            string notifyurl = "http://ba1adf48beba.ngrok.io/GioHang/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test


            string orderid = DateTime.Now.Ticks.ToString(); //ma don hang
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            ChiTietDonHang ctdh = new ChiTietDonHang();
            DonHang dh = new DonHang();
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            SanPham s = new SanPham();
            List<Giohang> gh = Laygiohang();


            dh.makh = kh.makh;
            dh.ngaydat = DateTime.Now;

            string str = "chờ xử lý";
            dh.giaohang = str;
            dh.thanhtoan = "MOMO";

            data.DonHangs.Add(dh);
            data.SaveChanges();
            foreach (var item in gh)
            {
                //ChiTietDonHang ctdh = new ChiTietDonHang();
                ctdh.madon = dh.madon;
                ctdh.masp = item.masp;
                ctdh.soluong = item.iSoluong;

                ctdh.gia = (decimal?)item.giakhuyenmai;

                ctdh.tongsoluong = TongSoLuong();
                ctdh.tonggia = (decimal?)TongTien();
                ctdh.status = 0;
                s = data.SanPhams.Single(n => n.masp == item.masp);
                s.soluongton = s.soluongton - ctdh.soluong;
                data.SaveChanges();

            }
            data.ChiTietDonHangs.Add(ctdh);
            data.SaveChanges();
            Session["Giohang"] = null;

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        public ActionResult ReturnUrl()
        {
            string param = Request.QueryString.ToString().Substring(0, Request.QueryString.ToString().IndexOf("signature") - 1);
            param = Server.UrlDecode(param);
            MoMoSecurity cryto = new MoMoSecurity();
            string serectkey = ConfigurationManager.AppSettings["serectkey"].ToString();
            string signature = cryto.signSHA256(param, serectkey);
            if (signature != Request["signature"].ToString())
            {
                ViewBag.message = "Thong tin Request khong hop le";
                return View();
            }
            if (!Request.QueryString["errorCode"].Equals("0"))
            {
                ViewBag.message = "Thanh toán thất bại";
                //return RedirectToAction("ThatBai", "GioHang");
                return View();
            }
            if (Request.QueryString["errorCode"].Equals("0"))
            {

                ViewBag.message = "Thanh toán thành công";
                //Session["Giohang"] = new List<Giohang>();
            }

            return View();
        }

        //Khi thanh toán xong ở cổng thanh toán Momo, Momo sẽ trả về một số thông tin, trong đó có errorCode để check thông tin thanh toán
        //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
        //Tham khảo bảng mã lỗi tại: https://developers.momo.vn/#/docs/aio/?id=b%e1%ba%a3ng-m%c3%a3-l%e1%bb%97i

        //VNPAY
        public ActionResult PaymentVnPay()
        {
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];
            string amount = (TongTien() * 100).ToString();
            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.0.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.0.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", amount); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);
        }

        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);

                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                        DonHang dh = new DonHang();
                        KhachHang kh = (KhachHang)Session["TaiKhoan"];
                        SanPham s = new SanPham();
                        List<Giohang> gh = Laygiohang();


                        dh.makh = kh.makh;
                        dh.ngaydat = DateTime.Now;

                        string str = "chờ xử lý";
                        dh.giaohang = str;
                        dh.thanhtoan = "VNPAY";

                        data.DonHangs.Add(dh);
                        data.SaveChanges();
                        foreach (var item in gh)
                        {
                            ChiTietDonHang ctdh = new ChiTietDonHang();
                            ctdh.madon = dh.madon;
                            ctdh.masp = item.masp;
                            ctdh.soluong = item.iSoluong;

                            ctdh.gia = (decimal?)item.giakhuyenmai;

                            ctdh.tongsoluong = TongSoLuong();
                            ctdh.tonggia = (decimal?)TongTien();
                            ctdh.status = 0;
                            s = data.SanPhams.Single(n => n.masp == item.masp);
                            s.soluongton = s.soluongton - ctdh.soluong;
                            data.SaveChanges();

                            data.ChiTietDonHangs.Add(ctdh);
                        }
                        data.SaveChanges();
                        Session["Giohang"] = null;
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }

        //PayPal
        public ActionResult PaymentPayPal(FormCollection collection)
        {
            bool useSandbox = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSandbox"]);
            var paypal = new PayPalModel(useSandbox);

            //ten san pham
            paypal.item_name = "1434567678345";

            //chuyen doi gia tien vnd sang usd
            double vndToUSD = Convert.ToDouble(TongTien().ToString()) * 0.000043;
            paypal.amount = vndToUSD.ToString();

            //so luong san pham
            paypal.item_quantity = TongSoLuong().ToString();

            //luu du lieu vao database
            #region
            DonHang dh = new DonHang();
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            SanPham s = new SanPham();
            List<Giohang> gh = Laygiohang();


            dh.makh = kh.makh;
            dh.ngaydat = DateTime.Now;

            string str = "chờ xử lý";
            dh.giaohang = str;
            dh.thanhtoan = "PayPal";

            data.DonHangs.Add(dh);
            data.SaveChanges();
            foreach (var item in gh)
            {
                ChiTietDonHang ctdh = new ChiTietDonHang();
                ctdh.madon = dh.madon;
                ctdh.masp = item.masp;
                ctdh.soluong = item.iSoluong;

                ctdh.gia = (decimal?)item.giakhuyenmai;

                ctdh.tongsoluong = TongSoLuong();
                ctdh.tonggia = (decimal?)TongTien();
                ctdh.status = 0;
                s = data.SanPhams.Single(n => n.masp == item.masp);
                s.soluongton = s.soluongton - ctdh.soluong;
                data.SaveChanges();

                data.ChiTietDonHangs.Add(ctdh);
            }
            data.SaveChanges();
            Session["Giohang"] = null;
            #endregion

            return View(paypal);
        }

        //thanh toan thanh cong
        public ActionResult RedirectFromPaypal()
        {
            return View();
        }

        //huy don hang
        public ActionResult CancelFromPaypal()
        {
            return View();
        }

        public ActionResult NotifyFromPaypal()
        {
            return View();
        }
    }
}