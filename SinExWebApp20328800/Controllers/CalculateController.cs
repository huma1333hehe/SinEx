﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SinExWebApp20328800.Models;
using SinExWebApp20328800.ViewModels;

namespace SinExWebApp20328800.Controllers
{
    public class CalculateController : Controller
    {

        private SinExWebApp20328800DatabaseContext db = new SinExWebApp20328800DatabaseContext();
        // GET: Calculate
        public ActionResult Index(
            string origin,
            string destination,
            string packageType,
            string serviceType,
            string size,
            decimal? weight,
            string currencyCode
        )
        {
            FeeCalculateViewModel Calculator = new FeeCalculateViewModel();
            Calculator.param = new FeeCalculateSearchViewModel();
            //populate dropdownlists
            Calculator.param.origins = PopulateCitiesDropdownlist().ToList();
            Calculator.param.destinations = PopulateCitiesDropdownlist().ToList();
            Calculator.param.packageTypes = PopulatePackageTypesDropdownlist().ToList();
            Calculator.param.serviceTypes = PopulateServiceTypesDropdownlist().ToList();
            Calculator.param.currencies  =   PopulateCurrenciesDropdownlist().ToList();
            //populate size dropdownlist
            if (!String.IsNullOrEmpty(packageType))
            {
                Calculator.param.sizes = PopulatePackageTypeSizesDropdownlist(packageType).ToList();
            }
            else
            {
                Calculator.param.sizes = new List<SelectListItem>();
            }
            //set viewbag
            ViewBag.currentOrigin = origin;
            ViewBag.currentDestination = destination;
            ViewBag.currentPackageType = packageType;
            ViewBag.currentServiceType = serviceType;
            ViewBag.currentSize = size;
            ViewBag.currentWeight = weight;
            ViewBag.currentCurrencyCode = currencyCode;
            //calculate result
            if (packageType != null && serviceType != null && weight != null && currencyCode != null)
            {
                Calculator.result = db.ServicePackageFees.SingleOrDefault(a => a.PackageType.Type == packageType && a.ServiceType.Type== serviceType);
                decimal price = 0 ;
                ViewBag.penalty = false;
                switch (Calculator.result.PackageTypeID)
                {
                    //Envelope
                    case 1:
                        price = Calculator.result.Fee;
                        break;
                    //Pak or Box
                    case 2:    
                    case 4:
                        int limit=0;
                        string limitString = db.PackageTypeSizes.Where(a => a.TypeSize == size).Select(a => a).First().WeightLimit;
                        bool convertResult = Int32.TryParse(limitString.Substring(0, limitString.Length-2), out limit);
                        ViewBag.limit = limit;
                        price = weight * Calculator.result.Fee > Calculator.result.MinimumFee ? (decimal)weight * Calculator.result.Fee : Calculator.result.MinimumFee;
                        if (limit!=0 && convertResult ==true && weight > (decimal)limit)
                        {
                            price += 500;
                            ViewBag.penalty = true;
                        }
                        //TODO: weight limit for box
                        break;
                    //Tube
                    case 3:
                        price = weight * Calculator.result.Fee > Calculator.result.MinimumFee ? (decimal)weight * Calculator.result.Fee : Calculator.result.MinimumFee;
                        break;
                    //Customer
                    case 5:
                        price = weight * Calculator.result.Fee > Calculator.result.MinimumFee ? (decimal)weight * Calculator.result.Fee : Calculator.result.MinimumFee;
                        break;
                }
                decimal rate = db.Currencies.Where(a => a.CurrencyCode == currencyCode).Select(a=>a.ExchangeRate).First();
                ViewBag.price = (price *rate).ToString("0.00");
                ViewBag.currency = currencyCode;
                ViewBag.weight = weight;
            }
            return View(Calculator);
        }

        private SelectList PopulateCitiesDropdownlist()
        {
            var Query = db.Destinations.Select(a => a.City).Distinct().OrderBy(a => a);
            return new SelectList(Query);
        }

        private SelectList PopulatePackageTypesDropdownlist()
        {
            var Query = db.PackageTypes.Select(a => a.Type).Distinct().OrderBy(a => a);
            return new SelectList(Query);
        }

        private SelectList PopulateServiceTypesDropdownlist()
        {
            var Query = db.ServiceTypes.Select(a => a.Type).Distinct().OrderBy(a => a);
            return new SelectList(Query);
        }

        private SelectList PopulatePackageTypeSizesDropdownlist(string packageType)
        {
            var Query = db.PackageTypeSizes.Where(a => a.PackageType.Type == packageType).Select(a=>a.TypeSize);
            return new SelectList(Query);
        }
        
        private SelectList PopulateCurrenciesDropdownlist()
        {
            var Query = db.Currencies.Select(a => a.CurrencyCode).Distinct().OrderBy(a => a);
            return new SelectList(Query);
        }

        public ActionResult GetSizes(string packageType)
        {
            if (String.IsNullOrEmpty(packageType))
            {
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }

            var query = db.PackageTypeSizes.Where(a => a.PackageType.Type == packageType).Select(a => a.TypeSize);
            List<SelectListItem> data = new SelectList(query).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSizesByID(int? packageTypeID)
        {
            if (packageTypeID == null)
            {
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }

            var query = db.PackageTypeSizes.Where(a => a.PackageType.PackageTypeID == packageTypeID);
            List<SelectListItem> data = new SelectList(query, "PackageTypeSizeID", "TypeSize").ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}