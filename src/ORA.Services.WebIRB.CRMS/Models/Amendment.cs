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
    public class Amendment
    {
        [Key]
        public int AmendmentKey { get; set; }
        public string AmendmentNumber { get; set; }
        public string AmendmentShortTitle { get; set; }
        public IEnumerable<string> ChangeInStaffList { get; set; }
        public string PIChangeReason { get; set; }
        public IEnumerable<string> AmendmentDetailMinorList { get; set; }
        public IEnumerable<string> AmendmentDetailMajorList { get; set; }
        public string AmendmentDetailOther { get; set; }
        public string Modifications { get; set; }
        public string ParticipantsEnrolled { get; set; }
        public string Reconsent { get; set; }
        public string CompletedNotification { get; set; }
        public string AssignedReviewType { get; set; }
        public string RadiationProcedures { get; set; }
        public IEnumerable<string> UpdatedDocuments { get; set; }
        public string AssignedCommittee { get; set; }
        public string ProtocolRequiresCalendarRevisions { get; set; }
        [ForeignKey("Study")]
        public int IrbSystemUniqueId { get; set; }
        public virtual Study Study { get; set; }

    }
}