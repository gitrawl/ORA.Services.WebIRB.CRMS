using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ORA.Domain.Model.HumanSubjects;

namespace ORA.Services.WebIRBCRMS.Models
{
    /// <summary>
    /// Captures a point-in-time snapshot of a study's external status.
    /// This is primarily to serve the contract for study external state history.
    /// </summary>
    public class ExternalStatusChange
    {
        [Key]
        public int StudyStateKey { get; set; }
        public string Type { get; set; }
        public string ExternalStatus { get; set; }
        public DateTimeOffset? ExternalStatusDate { get; set; }
        public int AmendmentKey { get; set; }
        public int ContinueReviewKey { get; set; }
        public int PostApprovalReportKey { get; set; }
        [ForeignKey("StudyStatusHistory")]
        public int IrbSystemUniqueId { get; set; }
        public virtual StudyStatusHistory StudyStatusHistory { get; set; }
    }
}