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
    public static class ContinuingReviewExtensions
    {
        /// <summary>
        /// Convert ContinuingReviewTransitionStateActivities from Domain model into ExternalStatusChanges Service model
        /// </summary>
        /// <param name="continuing_review"></param>
        /// <param name="StudyUniqueId"></param>
        /// <returns></returns>
        public static IEnumerable<Models.ExternalStatusChange> ExternalStatusChanges( this ContinuingReview continuing_review, int StudyUniqueId )
        {
            if (continuing_review.ContinuingReviewTransitionStateActivities != null && continuing_review.ContinuingReviewTransitionStateActivities.Any())
            {
                return continuing_review.ContinuingReviewTransitionStateActivities.Select(t => new Models.ExternalStatusChange()
                {
                    StudyStateKey = t.ActivityKey,
                    ExternalStatus = t.NewExternalStatus,
                    ExternalStatusDate = t.ExitDate,
                    ContinueReviewKey = continuing_review.ExternalKey,
                    IrbSystemUniqueId = StudyUniqueId,
                    Type = "Continuing Review Activity"
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
        /// <param name="continuing_review"></param>
        /// <returns></returns>
        public static string CommitteeName( this ContinuingReview continuing_review )
        {
            if (continuing_review.AssignedIrbCommittee == null)
            {
                return null;
            }
            else
            {
                return continuing_review.AssignedIrbCommittee.CommitteeName;
            }
        }
    }
}