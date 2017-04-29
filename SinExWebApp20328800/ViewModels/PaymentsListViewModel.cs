using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SinExWebApp20328800.ViewModels
{
    public class PaymentsListViewModel
    {
        public virtual int WaybillId { get; set; }
        public virtual int ShippingAccountId { get; set; }
        public virtual DateTime ShipDate { get; set; }
        public virtual string RecipientName { get; set; }
        public virtual string OriginCity { get; set; }
        public virtual string DestinationCity { get; set; }
        public virtual string ServiceType { get; set; }
        public virtual decimal TotalPaymentAmount { get; set; }
        public virtual string PaymentDescription { get; set; }
    }
}