using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ORA.Domain.Model;
using ORA.Domain.Model.HumanSubjects;
using System.ComponentModel.DataAnnotations;

namespace ORA.Services.WebIRBCRMS.Models
{
    /// <summary>
    /// This is the CRMS requested stateful study history, one persistent
    /// ID and a list of the study states in time.
    /// </summary>
    public class StudyStatusHistory
    {
        [Key]
        public int IrbSystemUniqueId { get; set; }
        public string IrbNumber { get; set; }
        public string ReferenceIrbNumber { get; set; }
        public string AssignedReviewType { get; set; }
        public string TypeOfSubmission { get; set; }
        public string PrmcReviewRequired { get; set; }
        public string ProtocolStatus { get; set; }
        //public string ProtocolUCLAHomeDepartment { get; set; }
        //public string RequiresHealthSystemResources { get; set; }
        //public string NonInterventional { get; set; }
        //public string BillingDesignation { get; set; }
        //public string ClinicalPreReviewInvestigatorInitiated { get; set; }
        public IEnumerable<string> ContactWithSubjectsTypeList { get; set; }
        public IEnumerable<ExternalStatusChange> ExternalStatusChanges { get; set; }
    }
}