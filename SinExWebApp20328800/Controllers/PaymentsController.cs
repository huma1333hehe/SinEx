using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SinExWebApp20328800.Models;
using SinExWebApp20328800.ViewModels;
using X.PagedList;

namespace SinExWebApp20328800.Controllers
{
    public class PaymentsController : Controller
    {
        private SinExWebApp20328800DatabaseContext db = new SinExWebApp20328800DatabaseContext();

        private ShippingAccount GetCurrentAccount()
        {
            string username = System.Web.HttpContext.Current.User.Identity.Name;
            if (username == null)
            {
                return null;
            }
            ShippingAccount current_account = db.ShippingAccounts.SingleOrDefault(s => s.UserName == username);
            return current_account;
        }

        private SelectList PopulateShippingAccountsDropdownList()
        {
            // TODO: Construct the LINQ query to retrieve the unique list of shipping account ids.
            var shippingAccountQuery = db.Payments.Select(s => s.UserName).Distinct().OrderBy(s => s);
            var hehe = db.ShippingAccounts.Where(m => shippingAccountQuery.Contains(m.UserName)).Select(m => m.ShippingAccountId).Distinct().OrderBy(m => m);

            return new SelectList(hehe);
        }

        public ActionResult GeneratePaymentHistoryReport(
            int? ShippingAccountId,
            int? WaybillId,
            DateTime? StartingDate,
            DateTime? EndingDate,
            string sortOrder,
            int? currentShippingAccountId,
            int? currentWaybillId,
            DateTime? currentStartingDate,
            DateTime? currentEndingDate,
            int? page)
        {
            // Instantiate an instance of the PaymentsReportViewModel and the PaymentsSearchViewModel.
            var PaymentReport = new PaymentsReportViewModel();
            PaymentReport.PaymentSearch = new PaymentsSearchViewModel();

            // Code for paging.
            ViewBag.CurrentSort = sortOrder;
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            // Retain search conditions for sorting.
            if (StartingDate == null)
            {
                StartingDate = currentStartingDate;
            }
            if (EndingDate == null)
            {
                EndingDate = currentEndingDate;
            }
            if (ShippingAccountId == null)
            {
                ShippingAccountId = currentShippingAccountId;
            }
            if (WaybillId == null)
            {
                WaybillId = currentWaybillId;
            }
            else
            {
                page = 1;
            }
            ViewBag.CurrentShippingAccountId = ShippingAccountId;
            ViewBag.CurrentWaybillId = WaybillId;
            ViewBag.CurrentStartingDate = StartingDate;
            ViewBag.CurrentEndingDate = EndingDate;

            PaymentReport.PaymentSearch.ShippingAccountId = ShippingAccountId == null ? 0 : (int)ShippingAccountId;
            PaymentReport.PaymentSearch.WaybillId = WaybillId == null ? 0 : (int)WaybillId;
            // Populate the ShippingAccountId dropdown list.

            if (User.IsInRole("Employee"))
            {
                var n = PopulateShippingAccountsDropdownList().ToList();
                foreach (SelectListItem k in n)
                {
                    k.Value = k.Text;
                }
                n.Insert(0, new SelectListItem
                {
                    Text = "All",
                    Value = "0"
                });
                int haha = 0;
                for (int i = 0; i < n.Count(); i++)
                {
                    if (n.ElementAt(i).Value == ShippingAccountId.ToString())
                    {
                        haha = i;
                        break;
                    }
                }
                foreach (var item in n)
                {
                    if (item.Value == ShippingAccountId.ToString())
                    {
                        item.Selected = true;
                    }
                }
                PaymentReport.PaymentSearch.ShippingAccounts = n;
                if(!(ShippingAccountId == 0 || ShippingAccountId == null))
                {
                    string accountusername = db.ShippingAccounts.Single(i => i.ShippingAccountId == ShippingAccountId).UserName;
                    PaymentReport.PaymentSearch.WaybillIds = new SelectList(db.Payments.Where(o => o.UserName == accountusername).Select(o => o.WaybillID).Distinct()).ToList();
                }
                else
                {
                    IQueryable<int> foo = Enumerable.Empty<int>().AsQueryable();
                    foo = foo.Concat(new int[] { 0 });
                    PaymentReport.PaymentSearch.WaybillIds = new SelectList(foo).ToList();
                }
            }
            else
            {
                PaymentReport.PaymentSearch.ShippingAccounts = null;
                string accountusername = GetCurrentAccount().UserName;
                PaymentReport.PaymentSearch.WaybillIds = new SelectList(db.Payments.Where(o => o.UserName == accountusername).Select(o => o.WaybillID).Distinct()).ToList();
            }

            //Initialize the query to retrieve payments using the PaymentsListViewModel.
            var list = new List<PaymentsListViewModel>();
            foreach (Payment v in db.Payments)
            {
                PaymentsListViewModel m = new PaymentsListViewModel();
                Shipment ship = db.Shipments.SingleOrDefault(a => a.WaybillId == v.WaybillID);
                m.WaybillId = v.WaybillID;
                m.ShippingAccountId = v.PayerCharacter == "Recipient" ? ship.RecipientShippingAccountID : ship.SenderShippingAccountID;
                m.ShipDate = (DateTime)ship.PickupDate;
                m.RecipientName = ship.RecipientFullName;
                m.OriginCity = ship.Origin;
                m.DestinationCity = ship.Destination;
                m.ServiceType = db.ServiceTypes.Single(a => a.ServiceTypeID == ship.ServiceTypeID).Type;
                m.TotalPaymentAmount = v.PaymentAmount;
                m.PaymentDescription = v.PaymentDescription;
                m.CurrencyCode = v.CurrencyCode;

                ShippingAccount lala_account = db.ShippingAccounts.Single(a => a.UserName == v.UserName);
                Shipment lala_shipment = db.Shipments.Single(a => a.WaybillId == v.WaybillID);
                m.SenderReferenceNumber = lala_shipment.ReferenceNumber;
                m.SenderFullName = "";
                if (lala_shipment.SenderShippingAccount is PersonalShippingAccount)
                {
                    PersonalShippingAccount temp = (PersonalShippingAccount)db.ShippingAccounts.Single(a => a.ShippingAccountId == lala_shipment.SenderShippingAccountID);
                    m.SenderFullName = temp.FirstName + temp.LastName;
                }
                else
                {
                    BusinessShippingAccount temp = (BusinessShippingAccount)db.ShippingAccounts.Single(a => a.ShippingAccountId == lala_shipment.SenderShippingAccountID);
                    m.SenderFullName = temp.ContactPersonName;
                }
                m.SenderMailingAddress = lala_shipment.SenderShippingAccount.ProvinceCode + ", " + lala_shipment.SenderShippingAccount.City + ", " + lala_shipment.SenderShippingAccount.StreetInformation + ", " + lala_shipment.SenderShippingAccount.BuildingInformation;
                m.RecipientFullName = lala_shipment.RecipientFullName;
                m.RecipientDeliveryAddress = lala_shipment.RecipientDeliveryProvince + ", " + lala_shipment.RecipientDeliveryCity + ", " + lala_shipment.RecipientDeliveryStreet + ", " + lala_shipment.RecipientDeliveryBuilding;
                m.CreditCardType = lala_account.Type;
                m.CreditCardNumber = lala_account.Number.Substring(lala_account.Number.Length - 4);
                m.AuthorizationCode = v.AuthorizationCode;

                m.Packages = lala_shipment.Packages;
                list.Add(m);
            }
            var paymentQuery = list.AsQueryable();
            // Add the condition to select a spefic shipping account if shipping account id is not null.

            if (User.IsInRole("Employee"))
            {
                if (ShippingAccountId != 0 && ShippingAccountId != null)
                {
                    paymentQuery = paymentQuery.Where(s => s.ShippingAccountId == ShippingAccountId);
                    if (WaybillId != 0 && WaybillId != null)
                    {
                        paymentQuery = paymentQuery.Where(s => s.WaybillId == WaybillId);
                    }
                }
            }
            else
            {
                int accountid = GetCurrentAccount().ShippingAccountId;
                ViewBag.lala = accountid;
                paymentQuery = paymentQuery.Where(p => p.ShippingAccountId == accountid);
                if (WaybillId != 0 && WaybillId != null)
                {
                    paymentQuery = paymentQuery.Where(s => s.WaybillId == WaybillId);
                }
            }


            if ((StartingDate != null) && (EndingDate != null))
            {
                paymentQuery = paymentQuery.Where(s => (s.ShipDate > StartingDate && s.ShipDate < EndingDate));
            }
            else
            {
                // Return an empty result if no shipping account id has been selected.
                PaymentReport.PaymentList = new PaymentsListViewModel[0].ToPagedList(pageNumber, pageSize);
            }

            // Code for sorting on ServiceType, ShippedDate, DeliveredDate, RecipientName, Origin, Destination
            ViewBag.ServiceTypeSortParm = sortOrder == "serviceType" ? "serviceType_desc" : "serviceType";
            ViewBag.ShippedDateSortParm = sortOrder == "shippedDate" ? "shippedDate_desc" : "shippedDate";
            ViewBag.RecipientNameSortParm = sortOrder == "recipientName" ? "recipientName_desc" : "recipientName";
            ViewBag.OriginSortParm = sortOrder == "origin" ? "origin_desc" : "origin";
            ViewBag.DestinationSortParm = sortOrder == "destination" ? "destination_desc" : "destination";
            ViewBag.InvoiceAmountSortParm = sortOrder == "invoiceAmount" ? "invoiceAmount_desc" : "invoiceAmount";
            switch (sortOrder)
            {
                case "serviceType_desc":
                    paymentQuery = paymentQuery.OrderByDescending(s => s.ServiceType);
                    break;
                case "serviceType":
                    paymentQuery = paymentQuery.OrderBy(s => s.ServiceType);
                    break;
                case "shippedDate_desc":
                    paymentQuery = paymentQuery.OrderByDescending(s => s.ShipDate);
                    break;
                case "shippedDate":
                    paymentQuery = paymentQuery.OrderBy(s => s.ShipDate);
                    break;
                case "recipientName_desc":
                    paymentQuery = paymentQuery.OrderByDescending(s => s.RecipientName);
                    break;
                case "recipientName":
                    paymentQuery = paymentQuery.OrderBy(s => s.RecipientName);
                    break;
                case "origin_desc":
                    paymentQuery = paymentQuery.OrderByDescending(s => s.OriginCity);
                    break;
                case "origin":
                    paymentQuery = paymentQuery.OrderBy(s => s.OriginCity);
                    break;
                case "destination_desc":
                    paymentQuery = paymentQuery.OrderByDescending(s => s.DestinationCity);
                    break;
                case "destination":
                    paymentQuery = paymentQuery.OrderBy(s => s.DestinationCity);
                    break;
                case "invoiceAmount_desc":
                    paymentQuery = paymentQuery.OrderByDescending(s => s.TotalPaymentAmount);
                    break;
                case "invoiceAmount":
                    paymentQuery = paymentQuery.OrderBy(s => s.TotalPaymentAmount);
                    break;
                default:
                    paymentQuery = paymentQuery.OrderBy(s => s.WaybillId);
                    break;
            }
            PaymentReport.PaymentList = paymentQuery.ToPagedList(pageNumber, pageSize);
            if (ShippingAccountId != null && ShippingAccountId != 0 && WaybillId != 0 && WaybillId != null)
            {
                ViewBag.ShowShipmentPackages = true;
            }else
            {
                ViewBag.ShowShipmentPackages = null;
            }
            return View(PaymentReport);
        }

        // GET: Payments
        public ActionResult Index()
        {
            var payments = db.Payments.Include(p => p.Shipment);
            return View(payments.ToList());
        }

        // GET: Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // GET: Payments/Create
        public ActionResult Create()
        {
            ViewBag.WaybillID = new SelectList(db.Shipments, "WaybillId", "ReferenceNumber");
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PaymentID,AuthorizationCode,WaybillID,PaymentAmount,CurrencyCode,UserName,PayerCharacter,PaymentDescription")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Payments.Add(payment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.WaybillID = new SelectList(db.Shipments, "WaybillId", "ReferenceNumber", payment.WaybillID);
            return View(payment);
        }

        // GET: Payments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            ViewBag.WaybillID = new SelectList(db.Shipments, "WaybillId", "ReferenceNumber", payment.WaybillID);
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PaymentID,AuthorizationCode,WaybillID,PaymentAmount,CurrencyCode,UserName,PayerCharacter,PaymentDescription")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.WaybillID = new SelectList(db.Shipments, "WaybillId", "ReferenceNumber", payment.WaybillID);
            return View(payment);
        }

        // GET: Payments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Payment payment = db.Payments.Find(id);
            db.Payments.Remove(payment);
            db.SaveChanges();
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

        public ActionResult GetWaybillIds(int shippingaccountid)
        {
            if (shippingaccountid == 0)
            {
                var ppp = db.Payments.Select(r => r.WaybillID).Distinct();
                List<SelectListItem> ooo = new SelectList(ppp).ToList();
                return Json(ooo, JsonRequestBehavior.AllowGet);
            }
            string accountusername = db.ShippingAccounts.Single(p => p.ShippingAccountId == shippingaccountid).UserName;
            var query = db.Payments.Where(a => a.UserName == accountusername).Select(a => a.WaybillID).Distinct();
            List<SelectListItem> data = new SelectList(query).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
