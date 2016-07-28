using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORA.Services.WebIRBCRMS.Models
{
    public class SponsoredFund
    {
        [Key]
        public char LocationCode { get; set; }
        
        [Key]
        public string FundNumber { get; set; }

        [Key]
        public DateTimeOffset FundBeginDate { get; set; }

        public DateTimeOffset FundEndDate { get; set; }

        public string MinimumLedgerYearMonth { get; set; }

        public IEnumerable<FullAccountingUnit> FAUs { get; set; }

        [ForeignKey("Award")]
        public string SponsorUclaCode { get; set; }
        public virtual Award Award { get; set; }


    }
}
