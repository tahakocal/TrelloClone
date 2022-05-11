using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebApplication12.Models;

namespace WebApplication12.Controllers
{
    public class DoItController : Controller
    {
        private ToDoDatabaseEntities db = new ToDoDatabaseEntities();

        // GET: ToDo
        public ActionResult Index()
        {
            List<ToDo> todoList = db.ToDo.ToList();
            return View(todoList);
        }

        //[HttpPost]
        //public ActionResult StatusChange(int taskId)
        //{
        //    var data = db.ToDo.SingleOrDefault(x =>x.Id==taskId);
        //    if (data!=null)
        //    {
        //        switch (data.StatusId)
        //        {
        //            case 1:
        //                data.StatusId = 2;
        //                break;
        //            case 2:
        //                data.StatusId = 3;
        //                break;
        //            default:
        //                data.StatusId = 1;
        //                break;
        //        }

        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return null;
        //}
        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Description")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                toDo.UserId = 1;
                toDo.StatusId = 1;
                db.ToDo.Add(toDo);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update([Bind(Include = "Id")] ToDo toDo)
        {
            db.ToDo.Add(toDo);
            db.SaveChanges();
            int id = toDo.Id;

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            ToDo toDo = db.ToDo.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }

            return View(toDo);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ToDo todo)
        {
            var Id = todo.Id;
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        public ActionResult Edit(int id)
        {
            return View(db.ToDo.Where(x => x.StatusId == id).FirstOrDefault());


        }
        [HttpPost]
        public ActionResult Edit(int id,ToDo toDo)
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


    }
}