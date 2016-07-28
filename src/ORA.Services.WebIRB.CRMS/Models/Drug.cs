using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORA.Services.WebIRBCRMS.Models
{
    public class Drug
    {
        [Key]
        public string ID { get; set; }
        public string HolderName { get; set; }
        public string Exempt { get; set; }
        public DateTimeOffset? SubmitToFDADate { get; set; }
        public DateTimeOffset? FDAApprovalDate { get; set; }
        [ForeignKey("Study")]
        public int IrbSystemUniqueId { get; set; }
        public virtual Study Study { get; set; }
    }
}