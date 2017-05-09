using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using SinExWebApp20328800.Models;

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
        [Display(Name ="Invoice Description")]
        public virtual string PaymentDescription { get; set; }
        public virtual string CurrencyCode { get; set; }

        [Display(Name ="Sender Reference Number")]
        public virtual string SenderReferenceNumber { get; set; }
        [Display(Name ="Sender Full Name")]
        public virtual string SenderFullName { get; set; }
        [Display(Name = "Sender Mailing Address")]
        public virtual string SenderMailingAddress { get; set; }
        [Display(Name = "Recipient Full Name")]
        public virtual string RecipientFullName { get; set; }
        [Display(Name = "Recipient Mailing Address")]
        public virtual string RecipientDeliveryAddress { get; set; }
        [Display(Name = "Credit Card Type")]
        public virtual string CreditCardType { get; set; }
        [Display(Name = "Credit Card Number(only last four digits)")]
        public virtual string CreditCardNumber { get; set; }
        [Display(Name = "Authorization Code")]
        public virtual string AuthorizationCode { get; set; }

        public virtual ICollection<Package> Packages { get; set; }

    }
}