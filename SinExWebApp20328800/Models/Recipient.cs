using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinExWebApp20328800.Models
{
    [Table("Recipient")]
    public class Recipient
    {
        public virtual int RecipientID { get; set; }
        public virtual int ShippingAccountId { get; set; }
        public virtual string FullName { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string DepartmentName { get; set; }
        public virtual string DeliveryAddress { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string Email { get; set; }
        //
        [ForeignKey("ShippingAccountId")]
        public virtual ShippingAccount ShippingAccount { get; set; }
    }
}