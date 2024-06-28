using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using greenfrog.Models;
using PagedList;
using System.IO;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace greenfrog.Controllers
{
    public class AdminController : BaseController
    {
        doanwebEntities data = new doanwebEntities();


        public ActionResult Index()
        {
            ViewBag.SoNguoiTruyCap = HttpContext.Application["SoNguoiTruyCap"].ToString();
            ViewBag.Online = HttpContext.Application["Online"].ToString();

            ViewBag.TongDoanhThu = ThongKeDoanhThu();
            ViewBag.TongDDH = ThongKeDonHang();
            ViewBag.TongKhachHang = ThongKeKhachHang();

            List<int> rep = new List<int>();
            List<ChiTietDonHang> ctdh = data.ChiTietDonHangs.ToList();
            List<SanPham> sanpham = data.SanPhams.ToList();

            //---------------- BIEU DO COT-------------------
            var listsp = sanpham.Select(x => x.masp).Distinct().Take(10); //lay ra danh sach ma sp
            var listsp2 = sanpham.Select(x => x.tensp).Distinct().Take(10);
            foreach (var item in listsp)
            {
                rep.Add((int)ctdh
                    .Where(x => x.masp == item && x.status == 1)
                    .Sum(x => x.soluong * x.gia)); //duyet ds ma don roi in ra tong tien tung ma sp
                                                   //.Count(x =>x.soluong == item));
            }

            ViewBag.AGES = listsp2;
            ViewBag.REP = rep.ToList().Take(10);
            //----------------

            return View();
        }

        //-----------------------------------DOANH THU --------------------------------------------
        #region  
        public ActionResult DoanhThu()
        {
            ViewBag.TongDoanhThu = ThongKeDoanhThu();

            var getlist = data.SanPhams.ToList();
            SelectList list = new SelectList(getlist, "masp", "tensp");

            return View();
        }

        public ActionResult DoanhThuNgay(FormCollection collection)
        {
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);


            DateTime dDate;

            if (DateTime.TryParse(ngaygiao, out dDate))
            {
                var getDay = DateTime.Parse(ngaygiao.ToString()).Day;
                var getMonth = DateTime.Parse(ngaygiao.ToString()).Month;
                var getYear = DateTime.Parse(ngaygiao.ToString()).Year;

                ViewBag.Ngay = getDay;
                ViewBag.Thang = getMonth;
                ViewBag.Nam = getYear;
                ViewBag.DoanhThuNgay = ThongKeDoanhThuNgay(getDay, getMonth, getYear);
                ViewBag.DoanhThuNgayCount = ThongKeDoanhThuNgayCount(getDay, getMonth, getYear);

                //ViewBag.CountDTN = ThongKeCountDoanhThuNgay(getDay, getMonth, getYear);


                List<DonHang> donhang = data.DonHangs.ToList();
                List<ChiTietDonHang> ctdh = data.ChiTietDonHangs.ToList();
                var ViewSP = (from sp in donhang
                              join ct in ctdh
                              on sp.madon equals ct.madon
                              where sp.ngaydat.Value.Day == getDay && sp.ngaydat.Value.Month == getMonth && sp.ngaydat.Value.Year == getYear && ct.status == 1
                              select new ViewModel
                              {
                                  donhang = sp,
                                  ctdh = ct
                              }).GroupBy(test => test.ctdh.madon)
                       .Select(grp => grp.First());


                ViewBag.ListCountDTN = ViewSP;

                return View();
            }
            else
            {
                TempData["msgDate"] = "<script>alert('Không đúng định dạng');</script>";
                return RedirectToAction("DoanhThu");
            }


        }

        public ActionResult DoanhThuThang(FormCollection collection)
        {
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);
            var getMonth = DateTime.Parse(ngaygiao.ToString()).Month;
            var getYear = DateTime.Parse(ngaygiao.ToString()).Year;

            ViewBag.Thang = getMonth;
            ViewBag.Nam = getYear;
            ViewBag.DoanhThuThang = ThongKeDoanhThuThang(getMonth, getYear);
            ViewBag.DoanhThuThangCount = ThongKeDoanhThuThangCount(getMonth, getYear);

            List<DonHang> donhang = data.DonHangs.ToList();
            List<ChiTietDonHang> ctdh = data.ChiTietDonHangs.ToList();
            var ViewSP = (from sp in donhang
                          join ct in ctdh
                          on sp.madon equals ct.madon
                          where sp.ngaydat.Value.Month == getMonth && sp.ngaydat.Value.Year == getYear && ct.status == 1
                          select new ViewModel
                          {
                              donhang = sp,
                              ctdh = ct
                          }).GroupBy(test => test.ctdh.madon)
                   .Select(grp => grp.First());


            ViewBag.ListCountDTN = ViewSP;

            return View();
        }

        public ActionResult DoanhThuNam(FormCollection collection)
        {
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);
            var getYear = DateTime.Parse(ngaygiao.ToString()).Year;

            ViewBag.Nam = getYear;
            ViewBag.DoanhThuNam = ThongKeDoanhThuNam(getYear);
            ViewBag.DoanhThuNamCount = ThongKeDoanhThuNamCount(getYear);

            List<DonHang> donhang = data.DonHangs.ToList();
            List<ChiTietDonHang> ctdh = data.ChiTietDonHangs.ToList();
            var ViewSP = (from sp in donhang
                          join ct in ctdh
                          on sp.madon equals ct.madon
                          where sp.ngaydat.Value.Year == getYear && ct.status == 1
                          select new ViewModel
                          {
                              donhang = sp,
                              ctdh = ct
                          }).GroupBy(test => test.ctdh.madon)
                   .Select(grp => grp.First());


            ViewBag.ListCountDTN = ViewSP;

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


        public decimal ThongKeDoanhTungSanPhamChoGia(int id)
        {
            decimal TongDoanhThu = data.ChiTietDonHangs
                .Where(m => m.masp == id && m.status == 1)
                .Sum(
                n => n.soluong * n.gia
            ).Value;
            return TongDoanhThu;
        }

        public decimal ThongKeDoanhTungSanPhamChoSL(int id)
        {
            decimal TongDoanhThu = data.ChiTietDonHangs
                .Where(m => m.masp == id && m.status == 1)
                .Sum(
                n => (n.soluong)
            ).Value;
            return TongDoanhThu;
        }

        public decimal ThongKeDoanhThu()
        {
            decimal TongDoanhThu = data.ChiTietDonHangs
                .Where(m => m.status == 1)
                .Sum(
                n => n.soluong * n.gia
            ).Value;
            return TongDoanhThu;
        }

        public decimal ThongKeDoanhThuNgay(int Ngay, int Thang, int Nam)
        {
            var listDDH = data.DonHangs.Where(n => n.ngaydat.Value.Day == Ngay && n.ngaydat.Value.Month == Thang &&
            n.ngaydat.Value.Year == Nam);
            decimal tongtien = 0;
            foreach (var item in listDDH)
            {
                tongtien += decimal.Parse(item.ChiTietDonHangs
                    .Where(m => m.status == 1)
                    .Sum(
                    n => n.soluong * n.gia).Value.ToString()
                );
            }
            return tongtien;
        }

        public decimal ThongKeDoanhThuNgayCount(int Ngay, int Thang, int Nam)
        {
            var listDDH = data.DonHangs.Where(n => n.ngaydat.Value.Day == Ngay && n.ngaydat.Value.Month == Thang &&
            n.ngaydat.Value.Year == Nam);
            decimal tongtien = 0;
            foreach (var item in listDDH)
            {
                tongtien += decimal.Parse(item.ChiTietDonHangs
                    .Where(m => m.status == 1)
                    .Sum(
                    n => n.soluong).Value.ToString()
                );
            }
            return tongtien;
        }

        public decimal ThongKeDoanhThuThang(int Thang, int Nam)
        {
            var listDDH = data.DonHangs.Where(n => n.ngaydat.Value.Month == Thang &&
            n.ngaydat.Value.Year == Nam);
            decimal tongtien = 0;
            foreach (var item in listDDH)
            {
                tongtien += decimal.Parse(item.ChiTietDonHangs
                    .Where(m => m.status == 1)
                    .Sum(
                    n => n.soluong * n.gia).Value.ToString()
                );
            }
            return tongtien;
        }

        public decimal ThongKeDoanhThuThangCount(int Thang, int Nam)
        {
            var listDDH = data.DonHangs.Where(n => n.ngaydat.Value.Month == Thang &&
            n.ngaydat.Value.Year == Nam);
            decimal tongtien = 0;
            foreach (var item in listDDH)
            {
                tongtien += decimal.Parse(item.ChiTietDonHangs
                    .Where(m => m.status == 1)
                    .Sum(
                    n => n.soluong).Value.ToString()
                );
            }
            return tongtien;
        }

        public decimal ThongKeDoanhThuNam(int Nam)
        {
            var listDDH = data.DonHangs.Where(n =>
            n.ngaydat.Value.Year == Nam);
            decimal tongtien = 0;
            foreach (var item in listDDH)
            {
                tongtien += decimal.Parse(item.ChiTietDonHangs
                    .Where(m => m.status == 1)
                    .Sum(
                    n => n.soluong * n.gia).Value.ToString()
                );
            }
            return tongtien;
        }

        public decimal ThongKeDoanhThuNamCount(int Nam)
        {
            var listDDH = data.DonHangs.Where(n =>
            n.ngaydat.Value.Year == Nam);
            decimal tongtien = 0;
            foreach (var item in listDDH)
            {
                tongtien += decimal.Parse(item.ChiTietDonHangs
                    .Where(m => m.status == 1)
                    .Sum(
                    n => n.soluong).Value.ToString()
                );
            }
            return tongtien;
        }

        public double ThongKeDonHang()
        {
            double slddh = data.DonHangs.Count();
            return slddh;
        }
        public double ThongKeKhachHang()
        {
            double slkh = data.KhachHangs.Count();
            return slkh;
        }

        #endregion 
        //---------------------------------------


        public ActionResult QLSanPham(int? page, string search)
        {
            if (page == null) page = 1;
            int pageSize = 5;
            int pageNum = page ?? 1;
            if (search != null)
            {
                var lstSP = data.SanPhams.Where(n => n.tensp.Contains(search));
                ViewBag.Search = search;
                return View(lstSP.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var all_sach = (from s in data.SanPhams select s).OrderBy(m => m.masp);
                return View(all_sach.ToPagedList(pageNum, pageSize));

            }
        }


        public ActionResult Details(int id)
        {
            var D_SanPham = data.SanPhams.Where(m => m.masp == id).First();

            List<SanPham> sanpham = data.SanPhams.ToList();
            var listSP = (from sp in sanpham
                          where sp.masp == id
                          select new ViewModel
                          {
                              sanpham = sp
                          }).Distinct();
            ViewBag.listSP = listSP;

            //tinh danh gia
            ViewBag.CountRate = Countrate(id);
            List<DanhGia> danhgia = data.DanhGias.ToList();

            var data1 = data.DanhGias.Where(d => d.Rating > 0 && d.id_sp == id);
            var rateSum = data1.Sum(d => d.Rating);
            var countRate = data1.Count();

            var tt = rateSum / countRate;
            ViewBag.SLuotRate = tt;

            ViewBag.TKTungSP = ThongKeDoanhTungSanPhamChoGia(id);
            ViewBag.TKTungSPSL = ThongKeDoanhTungSanPhamChoSL(id);

            return View(D_SanPham);
        }

        public static void Compressimage(string targetPath, String filename, Byte[] byteArrayIn)
        {
            try
            {
                System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();
                using (MemoryStream memstr = new MemoryStream(byteArrayIn))
                {
                    using (var image = Image.FromStream(memstr))
                    {
                        float maxHeight = 1920.0f;
                        float maxWidth = 1080.0f;
                        int newWidth;
                        int newHeight;
                        string extension;
                        Bitmap originalBMP = new Bitmap(memstr);
                        int orginalWidth = originalBMP.Width;
                        int originalHeight = originalBMP.Height;

                        if (orginalWidth > maxWidth || originalHeight > maxHeight)
                        {
                            //to preserve the aspect ratio
                            float ratioX = (float)maxWidth / (float)orginalWidth;
                            float ratioY = (float)maxHeight / (float)originalHeight;
                            float ratio = Math.Min(ratioX, ratioY);
                            newWidth = (int)(orginalWidth * ratio);
                            newHeight = (int)(originalHeight * ratio);
                        }
                        else
                        {
                            newWidth = (int)orginalWidth;
                            newHeight = (int)originalHeight;
                        }
                        Bitmap bitMap1 = new Bitmap(originalBMP, newWidth, newHeight);
                        Graphics imgGraph = Graphics.FromImage(bitMap1);
                        extension = Path.GetExtension(targetPath);
                        if (extension.ToLower() == ".png" || extension.ToLower() == ".gif" || extension.ToLower() == ".jpeg")
                        {
                            imgGraph.SmoothingMode = SmoothingMode.AntiAlias;
                            imgGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            imgGraph.DrawImage(originalBMP, 0, 0, newWidth, newHeight);
                            bitMap1.Save(targetPath, image.RawFormat);
                            bitMap1.Dispose();
                            imgGraph.Dispose();
                            originalBMP.Dispose();
                        }
                        else if (extension.ToLower() == ".jpg")
                        {
                            imgGraph.SmoothingMode = SmoothingMode.AntiAlias;
                            imgGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            imgGraph.DrawImage(originalBMP, 0, 0, newWidth, newHeight);
                            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                            EncoderParameters myEncoderParameters = new EncoderParameters(1);
                            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                            myEncoderParameters.Param[0] = myEncoderParameter;
                            bitMap1.Save(targetPath, jpgEncoder, myEncoderParameters);

                            bitMap1.Dispose();
                            imgGraph.Dispose();
                            originalBMP.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string exc = ex.Message;
                throw;
            }
        }

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }


        public ActionResult Create()
        {
            var getlist = data.DanhMucs.ToList();
            SelectList list = new SelectList(getlist, "idDanhmuc", "tendanhmuc");
            ViewBag.fulllist = list;

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FormCollection collection, SanPham s, ThuVienAnh nv, HttpPostedFileBase fileUpload, HttpPostedFileBase uploadhinh, HttpPostedFileBase uploadhinh1, HttpPostedFileBase uploadhinh2)
        {

            //nen anh giam kich thuoc anh dai dien
            var nameFile = Path.GetFileNameWithoutExtension(fileUpload.FileName) + DateTime.Now.ToString("ddMMyyyyhhmmss") + Path.GetExtension(fileUpload.FileName);
            using (var ms = new MemoryStream())
            {

                var targetImagepath = Server.MapPath("/Content/uploads/" + nameFile);
                byte[] image = new byte[fileUpload.ContentLength];
                fileUpload.InputStream.Read(image, 0, image.Length);
                Compressimage(targetImagepath, "", image);
            }



            var E_tensp = collection["tensp"];
            var E_hinh = "/Content/uploads/" + nameFile;
            var E_giaban = Convert.ToDecimal(collection["giaban"]);
            var E_giamgia = Convert.ToInt32(collection["giamgia"]);
            var E_ngaycapnhat = DateTime.Now;
            var E_iddanhmuc = collection["idDanhmuc"];
            var E_soluongton = Convert.ToInt32(collection["soluongton"]);
            var E_mota = collection["mota"];

            doanwebEntities db = new doanwebEntities();

            db.ThuVienAnhs.Add(nv);

            db.SaveChanges();

            int id = int.Parse(db.ThuVienAnhs.ToList().Last().idthuvien.ToString());

            if (uploadhinh != null && uploadhinh.ContentLength > 0)
            {
                //cavans luu cho thu vien 1
                //string nameFile = "MyUniqueImageFileName" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                var nameFile1 = Path.GetFileNameWithoutExtension(uploadhinh.FileName) + DateTime.Now.ToString("ddMMyyyyhhmmss") + "1" + Path.GetExtension(uploadhinh.FileName);
                string fileNameWitPath = Path.Combine(Server.MapPath("~/Content/uploads/"), nameFile1);

                using (FileStream fs = new FileStream(fileNameWitPath, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        byte[] data = Convert.FromBase64String(collection["imageData"]);
                        bw.Write(data);
                        bw.Close();

                    }
                    fs.Close();
                }

                ThuVienAnh unv = db.ThuVienAnhs.FirstOrDefault(x => x.idthuvien == id);
                //unv.img1 = _FileName;
                unv.img1 = "/Content/uploads/" + nameFile1;
                db.SaveChanges();
            }

            if (uploadhinh1 != null && uploadhinh1.ContentLength > 0)
            {

                string _FileName = "";
                string _FileName2 = "";
                int index = uploadhinh1.FileName.IndexOf('.');
                _FileName = "/Content/uploads/" + "imgg" + id.ToString() + "." + uploadhinh1.FileName.Substring(index + 1);
                _FileName2 = "imgg" + id.ToString() + "." + uploadhinh1.FileName.Substring(index + 1);
                string _path = Path.Combine(Server.MapPath("~/Content/uploads"), _FileName2);
                uploadhinh1.SaveAs(_path);

                ThuVienAnh unv = db.ThuVienAnhs.FirstOrDefault(x => x.idthuvien == id);
                unv.img2 = _FileName;
                db.SaveChanges();
            }

            if (uploadhinh2 != null && uploadhinh2.ContentLength > 0)
            {

                string _FileName = "";
                string _FileName2 = "";
                int index = uploadhinh2.FileName.IndexOf('.');
                _FileName = "/Content/uploads/" + "imggg" + id.ToString() + "." + uploadhinh2.FileName.Substring(index + 1);
                _FileName2 = "imggg" + id.ToString() + "." + uploadhinh2.FileName.Substring(index + 1);
                string _path = Path.Combine(Server.MapPath("~/Content/uploads"), _FileName2);
                uploadhinh2.SaveAs(_path);

                ThuVienAnh unv = db.ThuVienAnhs.FirstOrDefault(x => x.idthuvien == id);
                unv.img3 = _FileName;
                db.SaveChanges();
            }

            if (string.IsNullOrEmpty(E_tensp))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                s.tensp = E_tensp.ToString();
                s.hinh = E_hinh;

                if(E_giaban < 0)
                {
                    s.giaban = E_giaban;
                    TempData["errorkhong"] = "Không đúng định dạng";
                    return View();
                }
                else{
                    s.giamgia = E_giamgia;

                    s.idDanhmuc = Convert.ToInt32(E_iddanhmuc);
                    s.ngaycapnhat = E_ngaycapnhat;
                    s.soluongton = E_soluongton;
                    s.mota = E_mota;
                    s.idthuvien = id;

                    var x = s.giaban;
                    var y = s.giamgia;

                    var z = (x * y) / 100;

                    var price = x - z;

                    s.giakhuyenmai = price;

                    data.SanPhams.Add(s);
                    data.SaveChanges();
                    return RedirectToAction("QLSanPham");
                }

            }
            return this.Create();
        }

        public ActionResult Edit(int id)
        {
            var getlist = data.DanhMucs.ToList();
            SelectList list = new SelectList(getlist, "idDanhmuc", "tendanhmuc");
            ViewBag.fulllist = list;

            var E_sach = data.SanPhams.First(m => m.masp == id);

            List<SanPham> sanpham = data.SanPhams.ToList();
            List<ThuVienAnh> thuvien = data.ThuVienAnhs.ToList();
            var ViewSP = (from sp in sanpham
                          join tv in thuvien
               on sp.idthuvien equals tv.idthuvien
                          where sp.idthuvien == tv.idthuvien && sp.masp == id
                          select new ViewModel
                          {
                              sanpham = sp,
                              thuvienanh = tv
                          });
            ViewBag.listsp = ViewSP;

            return View(E_sach);

        }
        
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var E_sach = data.SanPhams.First(m => m.masp == id);
            var E_tensach = collection["tensp"];
            var E_hinh = collection["hinh"];
            var E_giaban = Convert.ToDecimal(collection["giaban"]);
            var E_giamgia = collection["giamgia"];
            var E_ngaycapnhat = DateTime.Now;
            var E_soluongton = Convert.ToInt32(collection["soluongton"]);
            var E_mota = collection["mota"];
            E_sach.masp = id;


            if (string.IsNullOrEmpty(E_tensach))
            {
                TempData["Error"] = "Don't empty!";
            }

            else if (E_giaban > 0 || E_giaban < 10000000)
            {
                E_sach.giaban = E_giaban;
                E_sach.tensp = E_tensach;
                E_sach.hinh = E_hinh;

                E_sach.giamgia = Convert.ToInt32(E_giamgia);
                E_sach.ngaycapnhat = E_ngaycapnhat;
                E_sach.soluongton = E_soluongton;
                E_sach.mota = E_mota;
                E_sach.idthuvien = id;

                var x = E_sach.giaban;
                var y = E_sach.giamgia;

                var z = (x * y) / 100;

                var price = x - z;

                E_sach.giakhuyenmai = price;

                //thu vien anh

                var E_sach2 = data.ThuVienAnhs.FirstOrDefault(m => m.idthuvien == E_sach.idthuvien);
                var uploadhinh = collection["hinh1"];
                var uploadhinh1 = collection["hinh2"];
                var uploadhinh2 = collection["hinh3"];

                E_sach2.img1 = uploadhinh;
                E_sach2.img2 = uploadhinh1;
                E_sach2.img3 = uploadhinh2;
                UpdateModel(E_sach2);



                UpdateModel(E_sach);
                data.SaveChanges();
                return RedirectToAction("QLSanPham");
            }
            else if(E_giaban < 0)
            {
                TempData["Error"] = "Không đúng định dạng!";
                return View();
            }
            return this.Edit(id);
        }

        //--Lay duong dan hinh anh khi sua
        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/Content/uploads/" + file.FileName));
            return "/Content/uploads/" + file.FileName;
        }


        public ActionResult Delete(int id)
        {
            var D_sach = data.SanPhams.First(m => m.masp == id);
            return View(D_sach);
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            //try
            //{
                var D_sach = data.SanPhams.Where(m => m.masp == id).First();
                data.SanPhams.Remove(D_sach);
                data.SaveChanges();
                return RedirectToAction("QLSanPham");
            //}
            //catch (Exception e) { return View("Error" + e); }

        }


        //Xem lich hen
        public ActionResult LichHen()
        {
            var all_danhmuc = from tt in data.DichVus select tt;
            return View(all_danhmuc);
        }


        public ActionResult SuaLichHen(int id)
        {
            var E_sach = data.DichVus.First(m => m.iddichvu == id);

            return View(E_sach);
        }

        [HttpPost]
        public ActionResult SuaLichHen(int id, FormCollection collection)
        {
            var E_sach = data.DichVus.First(m => m.iddichvu == id);
            var trangthai = collection["suatrangthai"];

            E_sach.trangthai = trangthai;

                UpdateModel(E_sach);
                data.SaveChanges();
                return RedirectToAction("LichHen", "Admin");

            return this.Edit(id);
        }


        public ActionResult XoaLichHen(int id)
        {
            var D_sach = data.DichVus.First(m => m.iddichvu == id);
            return View(D_sach);
        }
        [HttpPost]
        public ActionResult XoaLichHen(int id, FormCollection collection)
        {
            try
            {
                var D_sach = data.DichVus.Where(m => m.iddichvu == id).First();
                data.DichVus.Remove(D_sach);
                data.SaveChanges();
                return RedirectToAction("Index", "LichHen");
            }
            catch (Exception e) { return View("Error" + e); }

        }

        //Tao phong LiveStream

        public ActionResult LiveStream()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LiveStream(FormCollection collection, LiveStream ls, HttpPostedFileBase uploadhinh1)
        {

            var noidunglive = collection["noidunglive"];
            var link = collection["link"];


            if (string.IsNullOrEmpty(noidunglive))
            {
                ViewData["Error"] = "Du lieu khong duoc de trong!";
            }
            else
            {
                ls.noidunglive = noidunglive;
                ls.link = link;
                ls.hinh = collection["hinh"].ToString();

                data.LiveStreams.Add(ls);
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            return this.Create();
        }

        //Quan ly phan quyen
        public ActionResult ChiaQuyen()
        {
            List<KhachHang> khachhang = data.KhachHangs.ToList();
            List<KhachHangRole> khachhangrole = data.KhachHangRoles.ToList();

            var main = from kh in khachhang
                       join r in khachhangrole
                       on kh.RoleID equals r.RoleID
                       where (kh.RoleID == r.RoleID)
                       select new ViewModel
                       {
                           khachhang = kh,
                           khachhangrole = r
                       };
            ViewBag.Main = main;
            return View();
        }


        public ActionResult SuaQuyen(int id)
        {
            var E_category = data.KhachHangs.First(m => m.makh == id);
            return View(E_category);
        }
        [HttpPost]
        public ActionResult SuaQuyen(int id, FormCollection collection)
        {
            var danhmuc = data.KhachHangs.First(m => m.makh == id);
            var E_tendanhmuc = collection["RoleID"];
            var status = collection["status"];
            if (string.IsNullOrEmpty(E_tendanhmuc))
            {
                ViewData["Error"] = "Du lieu khong duoc de trong!";
            }
            else
            {
                danhmuc.RoleID = Convert.ToInt32(E_tendanhmuc);
                danhmuc.status = Convert.ToInt32(status);

                if (danhmuc.status == 1 || danhmuc.status == 2)
                {
                    UpdateModel(danhmuc);
                    data.SaveChanges();
                    return RedirectToAction("ChiaQuyen");
                }
                else
                {
                    return RedirectToAction("ChiaQuyen");
                }


                UpdateModel(danhmuc);
                data.SaveChanges();
                return RedirectToAction("ChiaQuyen");

            }
            return this.Edit(id);
        }


        public ActionResult XoaQuyen(int id)
        {
            var D_sach = data.KhachHangs.First(m => m.makh == id);
            return View(D_sach);
        }
        
        [HttpPost]
        public ActionResult XoaQuyen(int id, FormCollection collection)
        {
            try
            {
                var D_sach = data.KhachHangs.Where(m => m.makh == id).First();
                data.KhachHangs.Remove(D_sach);
                data.SaveChanges();
                return RedirectToAction("ChiaQuyen");
            }
            catch (Exception e) { return View("Error" + e); }

        }
    }
}