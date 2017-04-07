using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinExWebApp20328800.Models
{
    [Table("PickupLocation")]
    public class PickupLocation
    {
        public virtual int PickupLocationID { get; set; }
        public virtual int ShippingAccountId { get; set; }
        public virtual string Nickname { get; set; }
        public virtual string Location { get; set; }
        //
        [ForeignKey("ShippingAccountId")]
        public virtual ShippingAccount ShippingAccount { get; set; }
    }
}
