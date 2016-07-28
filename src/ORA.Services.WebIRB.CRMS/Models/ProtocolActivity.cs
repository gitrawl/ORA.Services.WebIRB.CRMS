using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORA.Services.WebIRBCRMS.Models
{
    public class ProtocolActivity
    {
        [Key]
        public int ID { get; set; }
        public string ActionStatus { get; set; }
        public DateTimeOffset? ActionStatusDate { get; set; }
        public string Note { get; set; }
        public DateTimeOffset? NoteCreatedDate { get; set; }
        [ForeignKey("Study")]
        public int IrbSystemUniqueId { get; set; }
        public virtual Study Study { get; set; }
    }
}