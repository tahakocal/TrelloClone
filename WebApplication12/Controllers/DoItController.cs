using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
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

        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind
            (Include = "Title,Description,StatusId")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                db.ToDo.Add(toDo);
                db.SaveChanges();
                
                return RedirectToAction("Index");
                
            }
            return null;
        }

    }
}