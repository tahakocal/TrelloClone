﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebApplication12.Models;

namespace WebApplication12.Controllers
{
    public class DoController : Controller
    {
        private ToDoDatabaseEntities db = new ToDoDatabaseEntities();

        #region Index

        // GET: ToDo
        public ActionResult Index()
        {
            var userLogin = Session["LoginUser"] as User;
            if (userLogin!=null)
            {
                List<ToDo> todoList = db.ToDo.Where(x => x.User.UserId == userLogin.UserId).ToList();
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
                toDo.UserId = userLogin.UserId;
                toDo.IsActive = 0;
                toDo.StatusId = 1;
                db.ToDo.Add(toDo);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return null;
        }

        #endregion

        #region statusChange

        public ActionResult StatusChange(int id)
        {
            var data = db.ToDo.SingleOrDefault(x => x.Id == id);
            if (data != null)
            {
                switch (data.StatusId)
                {
                    case 1:
                        data.StatusId = 2;
                        break;
                    case 2:
                        data.StatusId = 3;
                        break;
                    default:
                        data.StatusId = 1;
                        break;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return Json("hata", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Delete
        public ActionResult Delete(int id)
        {

            var data = db.ToDo.SingleOrDefault(x => x.Id == id);
            if (data != null)
            {
                switch (data.StatusId)
                {
                    case 3:
                        data.IsActive = 1;
                        break;
                    default:
                        data.IsActive = 0;
                        break;
                }

                db.SaveChanges();
                return RedirectToAction("Index");

            }
            return Json("hata", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Trash

        public ActionResult Trash()
        {
            var userLogin = Session["LoginUser"] as User;
            List<ToDo> todoList = db.ToDo.Where(x => x.User.UserId == userLogin.UserId).ToList();
            return View(todoList);
        }

        #endregion

        #region Edit
        public ActionResult Edit(int id)
        {
            var data = db.ToDo.SingleOrDefault(x => x.Id == id);
            if (data == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Name", data.StatusId);
            ViewBag.UserId = new SelectList(db.User, "UserId", "NameSurname", data.UserId);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(int id, ToDo toDo)
        {
            try
            {
                db.Entry(toDo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #endregion
    }
}