using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ORA.Services.WebIRBCRMS.Models
{
    /// <summary>
    /// This describes the CRMS perspective on Awards data from the Contracts and Grants
    /// side of thing. Ultimately we're going to get this data from PATS.
    /// </summary>
    public class Award
    {
        // Sponsor data from the requirements document
        /// <summary>
        /// Sponsor UCLA code from the Sponsor object
        /// </summary>
        [Key]
        public string SponsorUclaCode { get; set; } // for Edb-Qdb
        /// <summary>
        /// Sponsor Name from the Sponsor object
        /// </summary>
        public string SponsorName { get; set; }
        /// <summary>
        /// Protocol number 
        /// </summary>
        public string SponsorProtocolNumber { get; set; }

        /// <summary>
        /// Sponsor Name from the Prime Sponsor Object
        /// </summary>
        public string PrimeSponsorName { get; set; }

        /// <summary>
        /// Sponsor Code from the Prime Sponsor Object
        /// </summary>
        public string PrimeSponsorUclaCode { get; set; }

        /// <summary>
        /// Fund associated with this sponsor for this protocol
        /// </summary>
        public SponsoredFund Fund { get; set;}
        public string SponsoredProjectTitle { get; set; } // project_title / granttitle
        public Person PrincipalInvestigator { get; set; } // awarded PI

        [ForeignKey("Study")]
        public int IrbSystemUniqueId { get; set; }
        public virtual Study Study { get; set; }
    }
}