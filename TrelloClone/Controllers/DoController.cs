using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebApplication12.Models;

namespace WebApplication12.Controllers
{
    public class DoController : Controller
    {
        private ToDoDatabaseEntities _db = new ToDoDatabaseEntities();

        #region Index

        // GET: ToDo
        public ActionResult Index()
        {
            if (Session["LoginUser"] is User userLogin)
            {
                List<ToDo> todoList = _db.ToDo.Where(x => x.User.Userid == userLogin.Userid).ToList();
                return View(todoList);

            }

            return RedirectToAction("Login", "Account");
        }

        #endregion

        #region Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Description")] ToDo toDo)
        {
            var userLogin = Session["LoginUser"] as User;

            if (ModelState.IsValid)
            {
                toDo.Userid = userLogin.Userid;
                toDo.IsActive = 0;
                toDo.Statusid = 1;
                if (Request.Files.Count > 0)
                {
                    string fileName = Path.GetFileName(Request.Files[0].FileName);
                    string ext = Path.GetExtension(Request.Files[0].FileName);
                    string path = "~/images/" + fileName + ext;
                    Request.Files[0].SaveAs(Server.MapPath(path));
                    toDo.Image = "/images/" + fileName + ext;
                }


                _db.ToDo.Add(toDo);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Login", "Account");
        }

        #endregion

        #region statusChange

        public ActionResult StatusChange(int id)
        {
            var data = _db.ToDo.SingleOrDefault(x => x.Id == id);
            if (data != null)
            {
                switch (data.Statusid)
                {
                    case 1:
                        data.Statusid = 2;
                        break;
                    case 2:
                        data.Statusid = 3;
                        break;
                    default:
                        data.Statusid = 1;
                        break;
                }

                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return Json("hata", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Delete
        public ActionResult Delete(int id)
        {

            var data = _db.ToDo.SingleOrDefault(x => x.Id == id);
            if (data != null)
            {
                switch (data.Statusid)
                {
                    case 3:
                        data.IsActive = 1;
                        break;
                    default:
                        data.IsActive = 0;
                        break;
                }

                _db.SaveChanges();
                return RedirectToAction("Index");

            }
            return Json("hata", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Trash

        public ActionResult Trash()
        {
            var userLogin = Session["LoginUser"] as User;
            List<ToDo> todoList = _db.ToDo.Where(x => x.User.Userid == userLogin.Userid).ToList();
            return View(todoList);
        }

        #endregion

        #region Edit
        public ActionResult Edit(int id)
        {
            var data = _db.ToDo.SingleOrDefault(x => x.Id == id);
            if (data == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatusId = new SelectList(_db.Status, "Statusid", "Name", data.Statusid);
            ViewBag.Userid = new SelectList(_db.User, "Userid", "NameSurname", data.Userid);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(ToDo toDo)
        {
            ViewBag.StatusId = new SelectList(_db.Status, "Statusid", "Name", toDo.Statusid);
            ViewBag.Userid = new SelectList(_db.User, "Userid", "NameSurname", toDo.Userid);
            var loginuser = Session["LoginUser"] as User;
            toDo.Userid = loginuser.Userid;
            _db.Entry(toDo).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion
    }
}