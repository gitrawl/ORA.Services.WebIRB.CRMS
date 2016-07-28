using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ORA.Services.WebIRBCRMS.Models;
using ORA.Services.WebIRBCRMS.Interfaces;
using ORA.Domain.Model;
using ORA.Domain.Model.HumanSubjects;
using ORA.Data.WebIrb.Staging;
using ORA.Data.Warehouse;
using ORA.Data;

namespace ORA.Services.WebIRBCRMS
{
    // clumsy names, should change

    // set up Dependency Injection

    // Example
    public class IrbCrmsService : IIrbCrmsControllerService
    {
        private IDataManagerWebIrbStaging _WebIrbdataManager;
        private IProjectDao _ProjectDataManager;
        private IFundsDao _FundDataManager;
        private IIrbCrmsConverter _converter;

        public void setDataManager( IDataManagerWebIrbStaging DataManager ) // Setter injection
        {
            _WebIrbdataManager = DataManager;
        }
        
        public void SetProjectDataManager( IProjectDao DataManager ) // Setter injection
        {
            _ProjectDataManager = DataManager;
        }

        public void SetFundDataManager( IFundsDao DataManager ) // Setter injection
        {
            _FundDataManager = DataManager;
        }

        public void setIrbCrmsConverter(IIrbCrmsConverter converter) // Setter injection
        {
            _converter = converter;
        }
        public IEnumerable<WebIRBCRMS.Models.Study> GetChangedStudies( DateTime earliestChange, DateTime? latestChange )
        {
            ILookup<string, ORA.Domain.Model.Project> ProjectLookup = null;
            ILookup<Tuple<string, string, string>, FullAccountingUnitKey> FAUKeysLookup = null;
            ILookup<Tuple<string, string>, DateTime> FundEndDatesLookup = null;

            var studies = _WebIrbdataManager.GetStudyChangesByDateRange(earliestChange, latestChange ?? DateTime.Today).ToList();
            if (_ProjectDataManager != null)
            {
                ProjectLookup = _ProjectDataManager.GetProjectByIrbNumbers(studies);
            }
            if (_FundDataManager != null && ProjectLookup != null)
            {
                List<Project> Projects = ProjectLookup.SelectMany(x => x).ToList();
                FAUKeysLookup = _FundDataManager.GetExpenditureFausByProjects(Projects);
                FundEndDatesLookup = _FundDataManager.GetFundEndDateByProjects(Projects);
            }
            return _converter.GetStudyFacades(studies, ProjectLookup, FAUKeysLookup, FundEndDatesLookup);
        }

        public IEnumerable<WebIRBCRMS.Models.StudyStatusHistory> GetStudyChangedHistory( DateTime earliestChange, DateTime? latestChange )
        {
            var studies =
                _WebIrbdataManager.GetStudyStatusHistoryByDateRange(earliestChange, latestChange ?? DateTime.Today).ToList();
            
            return _converter.GetStatusHistoryFacades(studies);
        }

        public IEnumerable<WebIRBCRMS.Models.Study> GetPreSubmissions()
        {
            ILookup<string, ORA.Domain.Model.Project> ProjectLookup = null;
            ILookup<Tuple<string, string, string>, FullAccountingUnitKey> FAUKeysLookup = null;
            ILookup<Tuple<string, string>, DateTime> FundEndDatesLookup = null;

            var studies = _WebIrbdataManager.GetPreReviewedStudies().ToList();

            if (_ProjectDataManager != null)
            {
                ProjectLookup = _ProjectDataManager.GetProjectByIrbNumbers(studies);
            }
            if (_FundDataManager != null && ProjectLookup != null)
            {
                List<Project> Projects = ProjectLookup.SelectMany(x => x).ToList();
                FAUKeysLookup = _FundDataManager.GetExpenditureFausByProjects(Projects);
                FundEndDatesLookup = _FundDataManager.GetFundEndDateByProjects(Projects);
            }
            return _converter.GetStudyFacades(studies, ProjectLookup, FAUKeysLookup, FundEndDatesLookup);
        }

        public IEnumerable<WebIRBCRMS.Models.Study> GetChangedAmendments()
        {
            ILookup<string, ORA.Domain.Model.Project> ProjectLookup = null;
            ILookup<Tuple<string, string, string>, FullAccountingUnitKey> FAUKeysLookup = null;
            ILookup<Tuple<string, string>, DateTime> FundEndDatesLookup = null;

            var studies = _WebIrbdataManager.GetAmendmentsInProcessByDateRange().ToList();

            if (_ProjectDataManager != null)
            {
                ProjectLookup = _ProjectDataManager.GetProjectByIrbNumbers(studies);
            }
            if (_FundDataManager != null && ProjectLookup != null)
            {
                List<Project> Projects = ProjectLookup.SelectMany(x => x).ToList();
                FAUKeysLookup = _FundDataManager.GetExpenditureFausByProjects(Projects);
                FundEndDatesLookup = _FundDataManager.GetFundEndDateByProjects(Projects);
            }
            return _converter.GetStudyFacades(studies,ProjectLookup,FAUKeysLookup,FundEndDatesLookup);
        }

        public WebIRBCRMS.Models.Study GetStudyByUniqueId( int studyUniqueId )
        {
            ILookup<string, ORA.Domain.Model.Project> ProjectLookup = null;
            ILookup<Tuple<string, string, string>, FullAccountingUnitKey> FAUKeysLookup = null;
            ILookup<Tuple<string, string>, DateTime> FundEndDatesLookup = null;

            var study = _WebIrbdataManager.GetStudyByUniqueId(studyUniqueId);
            List<ORA.Domain.Model.HumanSubjects.Study> studies = new List<Domain.Model.HumanSubjects.Study>();
            studies.Add(study);
            if (_ProjectDataManager != null)
            {
                ProjectLookup = _ProjectDataManager.GetProjectByIrbNumbers(studies);
                _ProjectDataManager.GetStudiesSponsoredFunding(studies);
            }
            if (_FundDataManager != null)
            {
                List<Project> Projects = study.Approvals.SelectMany(a => a.Projects).ToList();
                FAUKeysLookup = _FundDataManager.GetExpenditureFausByProjects(Projects);
                FundEndDatesLookup = _FundDataManager.GetFundEndDateByProjects(Projects);
            }
            return _converter.GetStudyFacade(study, ProjectLookup, FAUKeysLookup,FundEndDatesLookup);
        }
    }
}