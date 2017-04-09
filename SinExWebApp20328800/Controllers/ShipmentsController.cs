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
    public class ShipmentsController : Controller
    {
        private SinExWebApp20328800DatabaseContext db = new SinExWebApp20328800DatabaseContext();

        // GET: Shipments
        [Authorize(Roles = "Customer,Employee")]
        public ActionResult Index()
        {
            var shipments = db.Shipments.Include(s => s.RecipientShippingAccount).Include(s => s.SenderShippingAccount).Include(s => s.ServiceType);
            return View(shipments.ToList());
        }

        // GET: Shipments/GenerateHistoryReport
        [Authorize(Roles = "Customer,Employee")]
        public ActionResult GenerateHistoryReport(
            int? ShippingAccountId,
            DateTime? StartingDate,
            DateTime? EndingDate,
            string sortOrder,
            int? currentShippingAccountId,
            DateTime? currentStartingDate,
            DateTime? currentEndingDate,
            int? page
            )
        {
            // Instantiate an instance of the ShipmentsReportViewModel and the ShipmentsSearchViewModel.
            var shipmentSearch = new ShipmentsReportViewModel();
            shipmentSearch.Shipment = new ShipmentsSearchViewModel();

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
            else
            {
                page = 1;
            }

            ViewBag.currentShippingAccountId = ShippingAccountId;
            ViewBag.CurrentStartingDate = StartingDate;
            ViewBag.CurrentEndingDate = EndingDate;



            // Populate the ShippingAccountId dropdown list.
            shipmentSearch.Shipment.ShippingAccounts = PopulateShippingAccountsDropdownList().ToList();
            shipmentSearch.Shipment.ShippingAccounts.Insert(0, new SelectListItem
            {
                Text = "All",
                Value = null,
                Selected = false
            });

            //Initialize the query to retrieve shipments using the ShipmentsListViewModel.
            var shipmentQuery = from s in db.Shipments
                                select new ShipmentsListViewModel
                                {
                                    WaybillId = s.WaybillId,
                                    ServiceType = s.ServiceType.Type,
                                    //TODO
                                    ShippedDate = new DateTime(2016, 12, 31),
                                    DeliveredDate = new DateTime(2017, 2, 11),
                                    RecipientName = s.RecipientFullName,
                                    NumberOfPackages = s.NumberOfPackages,
                                    Origin = s.Origin,
                                    Destination = s.Destination,
                                    ShippingAccountId = s.SenderShippingAccountID,
                                };


            if (User.IsInRole("Customer"))
            {
                ShippingAccount CShippingAccount = db.ShippingAccounts.SingleOrDefault(s => s.UserName == User.Identity.Name);
                shipmentQuery = shipmentQuery.Where(s => s.ShippingAccountId == CShippingAccount.ShippingAccountId);
            }

            // Add the condition to select a spefic shipping account if shipping account id is not null.
            if (ShippingAccountId != null)
            {
                // TODO: Construct the LINQ query to retrive only the shipments for the specified shipping account id.
                shipmentQuery = shipmentQuery.Where(s => s.ShippingAccountId == ShippingAccountId);

            }
            if ((StartingDate != null) && (EndingDate != null))
            {
                shipmentQuery = shipmentQuery.Where(s => (s.ShippedDate > StartingDate && s.DeliveredDate < EndingDate));
            }
            else
            {
                // Return an empty result if no shipping account id has been selected.
                shipmentSearch.Shipments = new ShipmentsListViewModel[0].ToPagedList(pageNumber, pageSize);

            }

            // Code for sorting on ServiceType, ShippedDate, DeliveredDate, RecipientName, Origin, Destination
            ViewBag.ServiceTypeSortParm = sortOrder == "serviceType" ? "serviceType_desc" : "serviceType";
            ViewBag.ShippedDateSortParm = sortOrder == "shippedDate" ? "shippedDate_desc" : "shippedDate";
            ViewBag.DeliveredDateSortParm = sortOrder == "deliveredDate" ? "deliveredDate_desc" : "deliveredDate";
            ViewBag.RecipientNameSortParm = sortOrder == "recipientName" ? "recipientName_desc" : "recipientName";
            ViewBag.OriginSortParm = sortOrder == "origin" ? "origin_desc" : "origin";
            ViewBag.DestinationSortParm = sortOrder == "destination" ? "destination_desc" : "destination";
            switch (sortOrder)
            {
                case "serviceType_desc":
                    shipmentQuery = shipmentQuery.OrderByDescending(s => s.ServiceType);
                    break;
                case "serviceType":
                    shipmentQuery = shipmentQuery.OrderBy(s => s.ServiceType);
                    break;
                case "shippedDate_desc":
                    shipmentQuery = shipmentQuery.OrderByDescending(s => s.ShippedDate);
                    break;
                case "shippedDate":
                    shipmentQuery = shipmentQuery.OrderBy(s => s.ShippedDate);
                    break;
                case "deliveredDate_desc":
                    shipmentQuery = shipmentQuery.OrderByDescending(s => s.DeliveredDate);
                    break;
                case "deliveredDate":
                    shipmentQuery = shipmentQuery.OrderBy(s => s.DeliveredDate);
                    break;
                case "recipientName_desc":
                    shipmentQuery = shipmentQuery.OrderByDescending(s => s.RecipientName);
                    break;
                case "recipientName":
                    shipmentQuery = shipmentQuery.OrderBy(s => s.RecipientName);
                    break;
                case "origin_desc":
                    shipmentQuery = shipmentQuery.OrderByDescending(s => s.Origin);
                    break;
                case "origin":
                    shipmentQuery = shipmentQuery.OrderBy(s => s.Origin);
                    break;
                case "destination_desc":
                    shipmentQuery = shipmentQuery.OrderByDescending(s => s.Destination);
                    break;
                case "destination":
                    shipmentQuery = shipmentQuery.OrderBy(s => s.Destination);
                    break;
                default:
                    shipmentQuery = shipmentQuery.OrderBy(s => s.WaybillId);
                    break;
            }
            shipmentSearch.Shipments = shipmentQuery.ToPagedList(pageNumber, pageSize);


            return View(shipmentSearch);
        }

        private SelectList PopulateShippingAccountsDropdownList()
        {
            // TODO: Construct the LINQ query to retrieve the unique list of shipping account ids.
            var shippingAccountQuery = db.Shipments.Select(s => s.SenderShippingAccountID).Distinct().OrderBy(s => s);
            return new SelectList(shippingAccountQuery);
        }


        // GET: Shipments/Details/5
        [Authorize(Roles = "Employee,Customer")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipment shipment = db.Shipments.Find(id);
            if (shipment == null)
            {
                return HttpNotFound();
            }
            return View(shipment);
        }

        // GET: Shipments/Create
        [Authorize(Roles = "Customer")]
        public ActionResult Create()
        {
            ViewBag.RecipientShippingAccountID = new SelectList(db.ShippingAccounts, "ShippingAccountId", "UserName");
            ViewBag.SenderShippingAccountID = new SelectList(db.ShippingAccounts, "ShippingAccountId", "UserName");
            ViewBag.ServiceTypeID = new SelectList(db.ServiceTypes, "ServiceTypeID", "Type");
            ViewBag.Origin = new SelectList(db.Destinations, "City", "City");
            ViewBag.Destination = new SelectList(db.Destinations, "City", "City");

            //Get the current username 
            string username = System.Web.HttpContext.Current.User.Identity.Name;
            if (username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingAccount account = db.ShippingAccounts.SingleOrDefault(s => s.UserName == username);
            if (account == null)
            {
                return HttpNotFound("There is no account with user name \"" + username + "\".");
            }
            IEnumerable<Recipient> hehe = db.Recipients.Select(s => s).Where(s => s.ShippingAccountId == account.ShippingAccountId);
            ViewBag.RecipientAddressNickname = new SelectList(hehe, "Nickname", "Nickname");
            IEnumerable<PickupLocation> lala = db.PickupLocations.Select(s => s).Where(s => s.ShippingAccountId == account.ShippingAccountId);
            ViewBag.PickupLocationNickname = new SelectList(lala, "Nickname", "Nickname");
            return View();
        }

        //xinruzhishui
        //inner peace

        public ActionResult GetRecipient(string RecipientAddressNickname)
        {
            if (string.IsNullOrEmpty(RecipientAddressNickname))
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }

            var query = db.Recipients.Single(hehe => hehe.Nickname == RecipientAddressNickname);
            Recipient data = query;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPickupLocation(string PickupLocationNickname) {
            if (string.IsNullOrEmpty(PickupLocationNickname))
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }

            var query = db.PickupLocations.Single(hehe => hehe.Nickname == PickupLocationNickname);
            PickupLocation data = query;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // POST: Shipments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult Create([Bind(Include = "WaybillId,ReferenceNumber,Origin,Destination,NumberOfPackages,ShipmentPayer,TaxPayer,Duty,Tax,ConfirmOrNot,PickupOrNot,PickupType,PickupDate,RecipientaddressNickname,RecipientFullName,RecipientCompanyName,RecipientDepartmentName,RecipientDeliveryAddress,RecipientPhoneNumber,RecipientEmail,ServiceTypeID,PickupLocationNickname,PickupLocation,SenderShippingAccountID,RecipientShippingAccountID,RecipientAddressNickname")] Shipment shipment)
        {
            shipment.SenderShippingAccountID = db.ShippingAccounts.SingleOrDefault(s => s.UserName == User.Identity.Name).ShippingAccountId;
            shipment.ConfirmOrNot = false;
            shipment.PickupOrNot = false;
            shipment.Duty = null;
            shipment.Tax = null;
            shipment.PickupType = PickupType.Immediate;
            shipment.PickupDate = DateTime.Now;
            shipment.NumberOfPackages = 0;
            if (ModelState.IsValid)
            {
                db.Shipments.Add(shipment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RecipientShippingAccountID = new SelectList(db.ShippingAccounts, "ShippingAccountId", "UserName", shipment.RecipientShippingAccountID);
            ViewBag.SenderShippingAccountID = new SelectList(db.ShippingAccounts, "ShippingAccountId", "UserName", shipment.SenderShippingAccountID);
            ViewBag.ServiceTypeID = new SelectList(db.ServiceTypes, "ServiceTypeID", "Type", shipment.ServiceTypeID);
            ViewBag.Origin = new SelectList(db.Destinations, "City", "City");
            ViewBag.Destination = new SelectList(db.Destinations, "City", "City");


            string username = System.Web.HttpContext.Current.User.Identity.Name;
            if (username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingAccount account = db.ShippingAccounts.SingleOrDefault(s => s.UserName == username);
            if (account == null)
            {
                return HttpNotFound("There is no account with user name \"" + username + "\".");
            }
            IEnumerable<Recipient> hehe = db.Recipients.Select(s => s).Where(s => s.ShippingAccountId == account.ShippingAccountId);
            ViewBag.RecipientAddressNickname = new SelectList(hehe, "Nickname", "Nickname");
            IEnumerable<PickupLocation> lala = db.PickupLocations.Select(s => s).Where(s => s.ShippingAccountId == account.ShippingAccountId);
            ViewBag.PickupLocationNickname = new SelectList(lala, "Nickname", "Nickname");
            return View(shipment);
        }

        // GET: Shipments/Edit/5
        [Authorize(Roles = "Customer,Employee")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipment shipment = db.Shipments.Find(id);
            if (shipment == null)
            {
                return HttpNotFound();
            }
            ViewBag.RecipientShippingAccountID = new SelectList(db.ShippingAccounts, "ShippingAccountId", "UserName", shipment.RecipientShippingAccountID);
            ViewBag.SenderShippingAccountID = new SelectList(db.ShippingAccounts, "ShippingAccountId", "UserName", shipment.SenderShippingAccountID);
            ViewBag.ServiceTypeID = new SelectList(db.ServiceTypes, "ServiceTypeID", "Type", shipment.ServiceTypeID);
            return View(shipment);
        }

        // POST: Shipments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer,Employee")]
        public ActionResult Edit([Bind(Include = "WaybillId,ReferenceNumber,Origin,Destination,NumberOfPackages,ShipmentPayer,TaxPayer,Duty,Tax,ConfirmOrNot,PickupOrNot,PickupType,PickupDate,RecipientaddressNickname,RecipientFullName,RecipientCompanyName,RecipientDepartmentName,RecipientDeliveryAddress,RecipientPhoneNumber,RecipientEmail,ServiceTypeID,PickupLocationNickname,PickupLocation,SenderShippingAccountID,RecipientShippingAccountID,RecipientAddressNickname")] Shipment shipment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shipment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RecipientShippingAccountID = new SelectList(db.ShippingAccounts, "ShippingAccountId", "UserName", shipment.RecipientShippingAccountID);
            ViewBag.SenderShippingAccountID = new SelectList(db.ShippingAccounts, "ShippingAccountId", "UserName", shipment.SenderShippingAccountID);
            ViewBag.ServiceTypeID = new SelectList(db.ServiceTypes, "ServiceTypeID", "Type", shipment.ServiceTypeID);
            return View(shipment);
        }

        // GET: Shipments/Delete/5
        [Authorize(Roles = "Customer,Employee")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipment shipment = db.Shipments.Find(id);
            if (shipment == null)
            {
                return HttpNotFound();
            }
            return View(shipment);
        }

        // POST: Shipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer,Employee")]
        public ActionResult DeleteConfirmed(int id)
        {
            Shipment shipment = db.Shipments.Find(id);
            db.Shipments.Remove(shipment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
