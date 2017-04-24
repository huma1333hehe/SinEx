using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SinExWebApp20328800.Models;

namespace SinExWebApp20328800.Controllers
{
    [Authorize(Roles = "Customer,Employee")]
    public class BusinessShippingAccountsController : Controller
    {
        private SinExWebApp20328800DatabaseContext db = new SinExWebApp20328800DatabaseContext();




        // GET: BusinessShippingAccounts/Create
        /*public ActionResult Create()
        {
            return View();
        }*/

        // POST: BusinessShippingAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ShippingAccountId,PhoneNumber,EmailAddress,BuildingInformation,StreetInformation,City,ProvinceCode,PostalCode,Type,Number,SecurityNumber,CardholderName,ExpiryMonth,ExpiryYear,ContactPersonName,CompanyName,DepartmentName")] BusinessShippingAccount businessShippingAccount)
        {
            if (ModelState.IsValid)
            {
                db.ShippingAccounts.Add(businessShippingAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(businessShippingAccount);
        }*/

        // GET: BusinessShippingAccounts/Edit
        public ActionResult Edit()
        {
            BusinessShippingAccount businessShippingAccount = (BusinessShippingAccount)db.ShippingAccounts.SingleOrDefault(s => s.UserName == User.Identity.Name);
            ViewBag.Cities = new SelectList(db.Destinations, "City", "City", businessShippingAccount.City);
            if (businessShippingAccount == null)
            {
                return HttpNotFound();
            }
            return View(businessShippingAccount);
        }

        // POST: BusinessShippingAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ShippingAccountId,PhoneNumber,EmailAddress,BuildingInformation,StreetInformation,City,ProvinceCode,PostalCode,Type,Number,SecurityNumber,CardholderName,ExpiryMonth,ExpiryYear,ContactPersonName,CompanyName,DepartmentName,UserName")] BusinessShippingAccount businessShippingAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(businessShippingAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Manage");
            }
            return View(businessShippingAccount);
        }



        // POST: BusinessShippingAccounts/Delete/5
        /*[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BusinessShippingAccount businessShippingAccount = (BusinessShippingAccount)db.ShippingAccounts.Find(id);
            db.ShippingAccounts.Remove(businessShippingAccount);
            db.SaveChanges();
            return RedirectToAction("Index");
        }*/

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
