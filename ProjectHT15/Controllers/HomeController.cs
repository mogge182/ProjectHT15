using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;
using ProjectHT15.Helpers;
using ProjectHT15.Models;
using ProjectHT15.Models.Context;


namespace ProjectHT15.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home

        public ActionResult Index()
        {

            return View();
        }


        public ActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel user)
        {
            if (!ModelState.IsValid) return View();
                
            var hashPassword = HashAndSaltHelper.PasswordHash(user);
            try
            {
                using (var context = new HermodsProjektEntities())

                    foreach (var pw in context.Users)
                    {
                        if (pw.Username == user.Salt && pw.Password == hashPassword)
                        {
                            Session["LoggedIn"] = new LoggedInUserModel() { UserId = pw.Id, UserName = pw.Username, FirstName = pw.FirstName, LastName = pw.LastName, Email = pw.Email };
                            return RedirectToAction("Index", "Start");
                        }
                    }
            }
            catch (Exception)
            {
                return View("_Error");
                
            }
            return View();
        }


        [HttpPost]
        public ActionResult RegisterUser(RegisterModel user)
        {
            try
            {
                using (var context = new HermodsProjektEntities())
                {

                    var newUser = new User();

                    newUser.Id = Guid.NewGuid();
                    newUser.Username = user.UserName;
                    newUser.FirstName = user.FirstName;
                    newUser.LastName = user.LastName;
                    newUser.Password = HashAndSaltHelper.PasswordHash(new LoginModel() { Password = user.Password, Salt = user.UserName });
                    newUser.Salt = user.UserName;
                    newUser.Email = user.Email.ToLower();

                    context.Users.Add(newUser);
                    context.SaveChanges();

                    Session["LoggedIn"] = new LoggedInUserModel()
                    {
                        UserName = newUser.Username,
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        UserId = newUser.Id,
                        Email = newUser.Username
                        
                    };
                    return RedirectToAction("Index","Start");
                }
            }
            catch (Exception e)
            {
                return View("_Error");
            }
        }
        public ActionResult LogOut()
        {
            Session["LoggedIn"] = null;
            return RedirectToAction("Login");
        }

        [HttpPost]
        public JsonResult CheckUserName(string UserName)
        {

            try
            {
                using (var context = new HermodsProjektEntities())
                {
                    var user = context.Users.FirstOrDefault(x => x.Username == UserName);
                    return Json(user == null);
                }
            }
            catch (Exception e)
            {
                return null;
            }
          

        }
         public JsonResult CheckEmail(string email)
        {
             try
             {
                 using (var context = new HermodsProjektEntities())
                 {
                     var user = context.Users.FirstOrDefault(x => x.Email == email);
                     return Json(user == null);
                 }
                   
             }
             catch (Exception)
             {
                 
                 throw;
             }
           
        }
        public void Error(Exception e)
        {

        }
    }

 
}
