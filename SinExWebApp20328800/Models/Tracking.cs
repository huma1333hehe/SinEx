using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinExWebApp20328800.Models
{
    [Table("Tracking")]
    public class Tracking
    {
        public virtual int TrackingID { get; set; }

        [Required]
        [Display(Name = "Waybill ID")]

        public virtual int WaybillId { get; set; }

        [Required]
        [Display(Name = "Time")]

        public virtual DateTime Time { get; set; }

        [Required]
        [Display(Name = "Description")]

        public virtual string Description { get; set; }

        [Required]
        [Display(Name = "Location")]

        public virtual string Location { get; set; }

        
        [Display(Name = "Remark")]

        public virtual string Remark { get; set; }

        [ForeignKey("WaybillId")]
        public virtual Shipment Shipment { get; set; }

        public virtual string DeliveredTo { get; set; }
        public virtual string DeliveredAt { get; set; }
        public virtual string StatusInformation { get; set; }
    }
}
