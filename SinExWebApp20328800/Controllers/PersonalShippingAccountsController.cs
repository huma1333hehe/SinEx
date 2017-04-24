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
    public class PersonalShippingAccountsController : Controller
    {
        private SinExWebApp20328800DatabaseContext db = new SinExWebApp20328800DatabaseContext();





        // GET: PersonalShippingAccounts/Create
        /*public ActionResult Create()
        {
            return View();
        }*/

        // POST: PersonalShippingAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ShippingAccountId,PhoneNumber,EmailAddress,BuildingInformation,StreetInformation,City,ProvinceCode,PostalCode,Type,Number,SecurityNumber,CardholderName,ExpiryMonth,ExpiryYear,FirstName,LastName")] PersonalShippingAccount personalShippingAccount)
        {
            if (ModelState.IsValid)
            {
                db.ShippingAccounts.Add(personalShippingAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(personalShippingAccount);
        }*/

        // GET: PersonalShippingAccounts/Edit
        public ActionResult Edit()
        {
            PersonalShippingAccount personalShippingAccount = (PersonalShippingAccount)db.ShippingAccounts.SingleOrDefault(s => s.UserName == User.Identity.Name);
            ViewBag.Cities = new SelectList(db.Destinations, "City", "City", personalShippingAccount.City);
            if (personalShippingAccount == null)
            {
                return HttpNotFound();
            }
            return View(personalShippingAccount);
        }

        // POST: PersonalShippingAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ShippingAccountId,PhoneNumber,EmailAddress,BuildingInformation,StreetInformation,City,ProvinceCode,PostalCode,Type,Number,SecurityNumber,CardholderName,ExpiryMonth,ExpiryYear,FirstName,LastName,UserName")] PersonalShippingAccount personalShippingAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(personalShippingAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Manage");
            }
            return View(personalShippingAccount);
        }



        // POST: PersonalShippingAccounts/Delete/5
        /*[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PersonalShippingAccount personalShippingAccount = (PersonalShippingAccount)db.ShippingAccounts.Find(id);
            db.ShippingAccounts.Remove(personalShippingAccount);
            db.SaveChanges();
            return RedirectToAction("Index");
        }*/

        /*protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }*/
    }
}
