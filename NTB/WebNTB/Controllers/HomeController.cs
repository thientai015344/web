using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebNTB.Context;

namespace WebNTB.Controllers
{
    public class HomeController : Controller
    {





        NTBEntities1 objModel = new NTBEntities1();
        public ActionResult Index()
        {
            return View();
        }


        //GET: Register

        public ActionResult Register()
        {
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(  string username , string password , User user)
        {
            if (ModelState.IsValid)
            {
                var check = objModel.Users.FirstOrDefault(s => s.UserName == username);
                if (check == null)
                {
                    password = GetMD5(password);
                    objModel.Configuration.ValidateOnSaveEnabled = false;
                    user.UserName = username;
                    user.Passsword = password;


                    objModel.Users.Add(user);
                    objModel.SaveChanges();

                    Response.Write("<script>alert('Bạn đã đăng ký thành công đăng nhập thôi nào !');</script>");
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return View();
                }


            }
            return View();


        }

        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(string IP, string Name)
        {

         



            if (ModelState.IsValid) {


                var f_password = GetMD5(Name);
                var data = objModel.Users.Where(s => s.UserName.Equals(IP) && s.Passsword.Equals(f_password)).ToList();
                if (data.Count() > 0) {

                    var i = data.ToArray();
                    

                    Session["userName"] = data.FirstOrDefault().UserName;
                    Session["UserId"] = data.FirstOrDefault().UserId;
                    if (data.FirstOrDefault().UserInfo != null) {

                     Session["fullname"] = data.FirstOrDefault().UserInfo.FullName;

                    }


                    if (Session["fullname"] == null){

                        return RedirectToAction("Create");

                    }
                    else {

                        return RedirectToAction("Details");


                    }





                }

                else {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login");
                }


            }

            return View();

        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        //POST: create card
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserInfo userInfo)
        {

            
            var check = objModel.UserInfoes.FirstOrDefault(s => s.UserId == userInfo.UserId);
            
           


                var userId = Session["UserId"].ToString();

                var id = Convert.ToInt32(userId);

                userInfo.UserId = id;

           if(check == null) {

                try {
                    if (userInfo.ImageUpload != null) {
                        string fileName = Path.GetFileNameWithoutExtension(userInfo.ImageUpload.FileName);
                        string extension = Path.GetExtension(userInfo.ImageUpload.FileName);
                        fileName = fileName + extension;
                        userInfo.avatar = fileName;

                        userInfo.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/fonts/images/Avatas/"), fileName));

                    }
                    objModel.UserInfoes.Add(userInfo);
                    objModel.SaveChanges();

                    

                }
                catch {

                return RedirectToAction("index");

                }

            }
                return View("About");

          
        }

        [HttpGet]
        public ActionResult Details()
        {
            var userId = Session["UserId"].ToString();

            var id = Convert.ToInt32(userId);

            

            var objUserInfo = objModel.UserInfoes.Where(n => n.UserId == id).First();
            return View(objUserInfo);



        }


    }


}