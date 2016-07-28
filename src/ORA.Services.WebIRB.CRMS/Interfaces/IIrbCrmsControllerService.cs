using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORA.Services.WebIRBCRMS.Models;
using ORA.Domain.Model;
using ORA.Domain.Model.HumanSubjects;
using ORA.Data.WebIrb.Staging;
using ORA.Data.Warehouse;
using ORA.Data;

namespace ORA.Services.WebIRBCRMS.Interfaces
{
    /// <summary>
    /// Describes the methods necessary to supply an IRB CRMS Controller, used for
    /// Inversion of Control.
    /// </summary>
    public interface IIrbCrmsControllerService
    {
        /// <summary>
        /// Manual dependency injection of the data manager, this is not integrated
        /// with the Unity framework yet.
        /// </summary>
        /// <param name="dataManager">The data manager to inject into the class</param>
        void setDataManager(IDataManagerWebIrbStaging dataManager);
        
        /// <summary>
        /// Manual dependency injection of the IRB CRMS converter, this is not
        /// integrated with the Unity framework yet.
        /// </summary>
        /// <param name="converter">The converter to inject</param>
        void setIrbCrmsConverter(IIrbCrmsConverter converter);
        
        /// <summary>
        /// Manual dependency injection of the data manager, this is not integrated
        /// with the Unity framework yet.
        /// </summary>
        /// <param name="dataManager"></param>
        void SetProjectDataManager( IProjectDao dataManager );
        
        /// <summary>
        /// Manual dependency injection of the data manager, this is not integrated
        /// with the Unity framework yet.
        /// </summary>
        /// <param name="dataManager"></param>
        void SetFundDataManager( IFundsDao dataManager );

        /// <summary>
        /// SC1 in the Requirements from IRB CRMS requires the ability to get the latest
        /// data for all studies for which data changed in a specified date window.
        /// </summary>
        /// <param name="earliestChange"></param>
        /// <param name="latestChange"></param>
        /// <returns>Enumerable format of studies that meet the CRMS data specification</returns>
        IEnumerable<WebIRBCRMS.Models.Study> GetChangedStudies( DateTime earliestChange, DateTime? latestChange ); //SC1

        /// <summary>
        /// SC2 in the Requirements from IRB CRMS requires the ability to get all status
        /// changes for studies in the specified date window. Compared to SC1 which only gets 
        /// the latest version of all changed data.
        /// </summary>
        /// <param name="earliestChange"></param>
        /// <param name="latestChange"></param>
        /// <returns>Study state history in the CRMS data specification, this may be multiple rows per study</returns>
        IEnumerable<WebIRBCRMS.Models.StudyStatusHistory> GetStudyChangedHistory( DateTime earliestChange, DateTime? latestChange ); //SC2

        /// <summary>
        /// SC3 in the Requirements from IRB CRMS requires the ability to get all Pre-Submission Protocol with
        /// the question for the field ClinicalPreReviewInvestigatorInitiatedFlag set to Yes
        /// </summary>
        /// <returns>Enumerable format of studies that meet the CRMS data specification</returns>
        IEnumerable<WebIRBCRMS.Models.Study> GetPreSubmissions(); //SC3

        /// <summary>
        /// Separate from SC1 but returning the same data type, CRMS expects to receive data for amendments-
        /// in-flight to facilitate parallel processing. Amendments in the IRB system are just like studies
        /// but they need the data before the amendment is "approved". Once the amendment is "approved" in WebIRB
        /// its data becomes the study data. In this context the expected behavior is that the amendment data would
        /// stop showing up in this contract but the SC1 GetChangedStudies() method would have one new row to return.
        /// </summary>
        /// <returns>Enumerable format of studies that meet the CRMS data specification, only including in-process amendments</returns>
        IEnumerable<WebIRBCRMS.Models.Study> GetChangedAmendments(); //SC4

        // Other useful methods
        /// <summary>
        /// The requirements don't specify the need to retrieve a specific study by unique id but for
        /// troubleshooting purposes they're likely to expect it or want it later.
        /// </summary>
        /// <param name="studyUniqueId">Persistent Unique ID of the Study</param>
        /// <returns>Study in CRMS Facade Format</returns>
        WebIRBCRMS.Models.Study GetStudyByUniqueId( int studyUniqueId );

    }
}

