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
    public class PostApprovalReport
    {
        [Key]
        public int PostApprovalReportKey { get; set; }
        public string PostApprovalReportType { get; set; }
        public string InvestigatorBrochureUpdate { get; set; }
        public string AssignedReviewType { get; set; }
        public string AssignedCommittee { get; set; }
        [ForeignKey("Study")]
        public int IrbSystemUniqueId { get; set; }
        public virtual Study Study { get; set; }
    }
}