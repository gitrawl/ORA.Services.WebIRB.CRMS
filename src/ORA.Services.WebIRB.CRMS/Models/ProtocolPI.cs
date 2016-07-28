using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ORA.Domain.Model;
using ORA.Domain.Model.HumanSubjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORA.Services.WebIRBCRMS.Models
{
    public class ProtocolPI
    {
        [Key]
        public string UID { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }

        [ForeignKey("Study")]
        public int IrbSystemUniqueId { get; set; }
        public virtual Study Study { get; set; }
    }
}