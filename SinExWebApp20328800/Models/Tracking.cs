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
        public virtual int WaybillId { get; set; }
        public virtual DateTime Time { get; set; }
        public virtual string Description { get; set; }
        public virtual string Location { get; set; }
        public virtual string Remark { get; set; }
        //
        [ForeignKey("WaybillId")]
        public virtual Shipment Shipment { get; set; }
    }
}
