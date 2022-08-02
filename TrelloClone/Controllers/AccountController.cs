using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication12.Models;

namespace WebApplication12.Controllers
{
    public class AccountController : Controller
    {
        private readonly ToDoDatabaseEntities db = new ToDoDatabaseEntities();
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Do");
        }

        #region MyAccount

        public ActionResult MyAccount()
        {
            string username = User.Identity.Name;

            // Fetch the userprofile
            ToDo user = db.ToDo.FirstOrDefault(u => u.Userid.Equals(username));

            // Construct the viewmodel
            ToDo model = new ToDo();
            model.User.FirstName = user.User.FirstName;
            model.User.LastName = user.User.LastName;
            model.User.Mail = user.User.Mail;

            return View(model);
        }

        [HttpPost]
        public ActionResult MyAccount(ToDo toDo)
        {

            if (ModelState.IsValid)
            {
                string username = toDo.User.Identity.Name;
                // Get the userprofile
                ToDo user = db.ToDo.FirstOrDefault(u => u.UserName.Equals(username));

                // Update fields
                user.FirstName = userprofile.FirstName;
                user.LastName = userprofile.LastName;
                user.Email = userprofile.Email;

                db.Entry(user).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index", "Home"); // or whatever
            }

            return View(userprofile);

        }

        #endregion

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

                    User reglog = new User
                    {
                        FirstName = registerDetails.FirstName,
                        LastName = registerDetails.LastName,
                        Mail = registerDetails.Mail,
                        Password = registerDetails.Password
                    };

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
        public ActionResult Login(User user)
        {

            if (ModelState.IsValid)
            {
                var checkUser = db.User.SingleOrDefault(x => x.Mail == user.Mail && x.Password == user.Password);
                if (checkUser != null)
                {
                    Session["LoginUser"] = checkUser;
                    return RedirectToAction("Index");
                }

                return View(user);
            }
            else
            {
                ModelState.AddModelError("Hata", "Yanlış Mail Veya Şifre !");
                return View();
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