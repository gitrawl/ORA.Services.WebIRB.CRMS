using ORA.Domain.Model.HumanSubjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ORA.Services.WebIRBCRMS.Extensions
{
    /// <summary>
    /// Extensions to convert from Domain model to Service model
    /// </summary>
    public static class AmendmentExtensions
    {
        /// <summary>
        /// convert AmendmentTransitionStateActivities from Domain model into ExternalStatusChanges Service model
        /// </summary>
        /// <param name="amendment"></param>
        /// <param name="StudyUniqueId"></param>
        /// <returns></returns>
        public static IEnumerable<Models.ExternalStatusChange> ExternalStatusChanges( this Amendment amendment, int StudyUniqueId )
        {
            if (amendment.AmendmentTransitionStateActivities != null && amendment.AmendmentTransitionStateActivities.Any())
            {
                return amendment.AmendmentTransitionStateActivities.Select(t => new Models.ExternalStatusChange()
                {
                    StudyStateKey = t.ActivityKey,
                    ExternalStatus = t.NewExternalStatus,
                    ExternalStatusDate = t.ExitDate,
                    AmendmentKey = amendment.ExternalKey,
                    IrbSystemUniqueId = StudyUniqueId,
                    Type = "Amendment Activity"
                });
            }
            else
            {
                return new List<Models.ExternalStatusChange>();
            }
        }

        /// <summary>
        /// convert AssignedIrbCommittee from Domain model into AssignedCommittee Service Model
        /// </summary>
        /// <param name="amendment"></param>
        /// <returns></returns>
        public static string CommitteeName(this Amendment amendment)
        {
            if (amendment.AssignedIrbCommittee == null)
            {
                return null;
            }
            else
            {
                return amendment.AssignedIrbCommittee.CommitteeName;
            }
        }
    }
}