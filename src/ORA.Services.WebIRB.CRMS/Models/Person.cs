using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORA.Services.WebIRBCRMS.Models
{
    /// <summary>
    /// This is the CRMS view of what makes a "Person" data object.
    /// </summary>
    public class Person // per KeyPersonnel Data Contract
    {
        [Key]
        public string UID { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string Institution { get; set; }
        public char HomeLocationCode { get; set; }
        public string HomeDepartmentCode { get; set; }
        public string HomeDepartmentTitle { get; set; }
        public string EmailAddress { get; set; }
        public IEnumerable<string> AccessRoles { get; set; }
        public string OtherRole { get; set; }
        public string WillObtainConsent { get; set; }
        public string ManageDeviceAccountability { get; set; }
        public string AccessToPersonallyIndentifiableInfo { get; set; }
        public string AccessToCodeKey { get; set; }
        [ForeignKey("Study")]
        public int IrbSystemUniqueId { get; set; }
        public virtual Study Study { get; set; }
        [ForeignKey("Award")]
        public string SponsorUclaCode { get; set; }
        public virtual Award Award { get; set; }
    }
}