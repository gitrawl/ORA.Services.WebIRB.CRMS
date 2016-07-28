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
    public static class PostApprovalReportExtensions
    {
        /// <summary>
        /// Convert PostApprovalReportTransitionStateActivities from Domain model into ExternalStatusChanges Service Model 
        /// </summary>
        /// <param name="post_approval_report"></param>
        /// <param name="StudyUniqueId"></param>
        /// <returns></returns>
        public static IEnumerable<Models.ExternalStatusChange> ExternalStatusChanges( this PostApprovalReport post_approval_report, int StudyUniqueId )
        {
            if (post_approval_report.PostApprovalReportTransitionStateActivities != null && post_approval_report.PostApprovalReportTransitionStateActivities.Any())
            {
                return post_approval_report.PostApprovalReportTransitionStateActivities.Select(t => new Models.ExternalStatusChange()
                {
                    StudyStateKey = t.ActivityKey,
                    ExternalStatus = t.NewExternalStatus,
                    ExternalStatusDate = t.ExitDate,
                    PostApprovalReportKey = post_approval_report.ExternalKey,
                    IrbSystemUniqueId = StudyUniqueId,
                    Type = "Post Approval Report Activity"
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
        public static string CommitteeName( this PostApprovalReport post_approval_report )
        {
            if (post_approval_report.AssignedIrbCommittee == null)
            {
                return null;
            }
            else
            {
                return post_approval_report.AssignedIrbCommittee.CommitteeName;
            }
        }
    }
}