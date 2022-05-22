using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication12.Models;

namespace WebApplication12.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Do");
        }

        #region Register

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SaveRegisterDetails(User registerDetails)
        {
            if (ModelState.IsValid)
            {
                using (var databaseContext = new ToDoDatabaseEntities())
                {

                    User reglog = new User();

                    reglog.FirstName = registerDetails.FirstName;
                    reglog.LastName = registerDetails.LastName;
                    reglog.Mail = registerDetails.Mail;
                    reglog.Password = registerDetails.Password;

                    databaseContext.User.Add(reglog);
                    databaseContext.SaveChanges();
                }

                ViewBag.Message = "Kayıt Olundu, Sağ Yukarıdan Giriş Yapabilirsin";
                return View("Register");
            }
            else
            {

                return View("Register", registerDetails);
            }
        }

        #endregion

        #region Login

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(LoginViewModel model, User user)
        {

            if (ModelState.IsValid)
            {
                using (var databaseContext = new ToDoDatabaseEntities())
                {
                    var isValidUser = IsValidUser(model);


                    if (isValidUser != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.Mail, false);
                        Session["LoginUser"] = user;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("Hata", "Yanlış Mail Veya Şifre !");
                        return View();
                    }
                }
            }
            else
            {
                return View(model);
            }
        }


        public User IsValidUser(LoginViewModel model)
        {
            using (var dataContext = new ToDoDatabaseEntities())
            {

                User user = dataContext.User.Where(query => query.Mail.Equals(model.Mail) && query.Password.Equals(model.Password)).SingleOrDefault();
                if (user == null)
                {
                    return null;
                }
                else
                {
                    return user;
                }
            }
        }

        #endregion

        #region Logout

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Login", "Account");
        }

        #endregion

    }
}