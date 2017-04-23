using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinExWebApp20328800.Models
{
    [Table("Payment")]
    public class Payment
    {
        [Key]
        [Required]
        public virtual int PaymentID { get; set; }
        [Required]
        public virtual string AuthorizationCode { get; set; }
        [Required]
        public virtual int WaybillID { get; set; }
        public virtual decimal PaymentAmount { get; set; }
        public virtual string CurrencyCode { get; set; }
        public virtual string UserName { get; set; }
        public virtual string PayerCharacter { get; set; }
        public virtual string PaymentDescription { get; set; }

        [ForeignKey("WaybillID")]
        public virtual Shipment Shipment { get; set; }

    }
}