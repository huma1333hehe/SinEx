using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SinExWebApp20328800.ViewModels
{
    public enum DateType
    {
        ShippedDate,
        DeliveredDate,
    }
    public class ShipmentsSearchViewModel
    {
        public virtual int ShippingAccountId { get; set; }
        public virtual List<SelectListItem> ShippingAccounts { get; set; }

        public virtual DateTime StartingDate { get; set; }
        public virtual DateTime EndingDate { get; set; }

        public virtual DateType DateType { get; set; }
    }
}