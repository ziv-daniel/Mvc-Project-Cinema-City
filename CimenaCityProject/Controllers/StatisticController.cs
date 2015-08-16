﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CimenaCityProject.Models;

namespace CimenaCityProject.Controllers
{
    public class StatisticController : Controller
    {
        private HomeCinemaContext db = new HomeCinemaContext();

        // GET: /Statistic/
        public  ActionResult Index()
        {

            return View( );
        }


        public PartialViewResult TotalIncome()
        {
            return PartialView();
        }

        public JsonResult TotalIncomeJson()
        {

            var data = db.CheckOut.Where(y => y.ISOrderComplete == true).ToArray().Distinct(new DisinctItemComparer())
                .OrderBy(y => y.Order.OrderDate).ToList();
 
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GraphTotalIncome()
        {
            var result = new List<dynamic>();
            for (int i = 0; i < 20; i++)
            {
                result.Add(new { date = i.ToString(), price = i * i + 10 });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        // GET: /Statistic/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckOut checkout = await db.CheckOut.FindAsync(id);
            if (checkout == null)
            {
                return HttpNotFound();
            }
            return View(checkout);
        }

        // GET: /Statistic/Create
        public ActionResult Create()
        {
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "CartId");
            ViewBag.PersonID = new SelectList(db.Persons, "PersonID", "FirstName");
            return View();
        }

        // POST: /Statistic/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="CheckOutID,CartId,OrderID,PersonID,TotalPrice,ISOrderComplete")] CheckOut checkout)
        {
            if (ModelState.IsValid)
            {
                db.CheckOut.Add(checkout);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "CartId", checkout.OrderID);
            ViewBag.PersonID = new SelectList(db.Persons, "PersonID", "FirstName", checkout.PersonID);
            return View(checkout);
        }

        // GET: /Statistic/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckOut checkout = await db.CheckOut.FindAsync(id);
            if (checkout == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "CartId", checkout.OrderID);
            ViewBag.PersonID = new SelectList(db.Persons, "PersonID", "FirstName", checkout.PersonID);
            return View(checkout);
        }

        // POST: /Statistic/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="CheckOutID,CartId,OrderID,PersonID,TotalPrice,ISOrderComplete")] CheckOut checkout)
        {
            if (ModelState.IsValid)
            {
                db.Entry(checkout).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "CartId", checkout.OrderID);
            ViewBag.PersonID = new SelectList(db.Persons, "PersonID", "FirstName", checkout.PersonID);
            return View(checkout);
        }

        // GET: /Statistic/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckOut checkout = await db.CheckOut.FindAsync(id);
            if (checkout == null)
            {
                return HttpNotFound();
            }
            return View(checkout);
        }

        // POST: /Statistic/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CheckOut checkout = await db.CheckOut.FindAsync(id);
            db.CheckOut.Remove(checkout);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


     


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    class DisinctItemComparer : IEqualityComparer<CheckOut>
    {

        public bool Equals(CheckOut x, CheckOut y)
        {
            return x.TotalPrice == y.TotalPrice &&
            x.Order.OrderDate == y.Order.OrderDate;
        }

        public int GetHashCode(CheckOut obj)
        {
            return obj.TotalPrice.GetHashCode();
        }
    }

}
