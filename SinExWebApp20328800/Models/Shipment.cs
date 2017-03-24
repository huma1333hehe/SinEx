using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinExWebApp20328800.Models
{
    [Table("Shipment")]
    public class Shipment
    {
        [Key]
        public virtual int WaybillId { get; set; }
        [Display(Name = "Reference Number")]
        public virtual string ReferenceNumber { get; set; }
        [Display(Name = "Service Type")]
        public virtual string ServiceType { get; set; }
        [Display(Name = "Shipped Date")]
        public virtual DateTime ShippedDate { get; set; }
        [Display(Name = "Delivered Date")]
        public virtual DateTime DeliveredDate { get; set; }
        [Display(Name = "Recipient Name")]
        public virtual string RecipientName { get; set; }
        [Display(Name = "Number Of Packages")]
        public virtual int NumberOfPackages { get; set; }
        public virtual string Origin { get; set; }
        public virtual string Destination { get; set; }
        public virtual string Status { get; set; }
        public virtual int ShippingAccountId { get; set; }

        public virtual ShippingAccount ShippingAccount { get; set; }
    }
}