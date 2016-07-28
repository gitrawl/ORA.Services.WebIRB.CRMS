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
    public class ContinuingReview
    {
        [Key]
        public int ContinueReviewKey { get; set; }
        public string ContinueReviewReportType { get; set; }
        public string CurrentStatusOfTheStudyEnrollment { get; set; }
        public string ReasonForClosingStudyAtUCLA { get; set; }
        public string AssignedReviewType { get; set; }
        public string AssignedCommittee { get; set; }
        [ForeignKey("Study")]
        public int IrbSystemUniqueId { get; set; }
        public virtual Study Study { get; set; }
    }
}