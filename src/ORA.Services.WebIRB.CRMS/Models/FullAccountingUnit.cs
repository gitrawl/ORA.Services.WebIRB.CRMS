using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORA.Services.WebIRBCRMS.Models
{
    public class FullAccountingUnit
    {
        /// <summary>
        /// 1 digit
        /// </summary>
        [Key]
        public char LocationCode { get; set; }
        
        /// <summary>
        /// 6 digits
        /// </summary>
        [Key]
        public string Account { get; set; }

        /// <summary>
        /// 2 digits
        /// </summary>
        [Key]
        public string CostCenter { get; set; }
        
        /// <summary>
        /// 5 digits
        /// </summary>
        [Key]
        public string FundNumber { get; set; }

        public virtual SponsoredFund Fund { get; set; }
    }
}