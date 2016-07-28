using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORA.Services.WebIRBCRMS.Models;


namespace ORA.Services.WebIRBCRMS.Interfaces
{
    /// <summary>
    /// Describes a simple collection of helper methods which convert between ORA.Domain.Model.HumanSubjects
    /// data and ORA.Services.Protocols.Human data. 
    /// </summary>
    public interface IIrbCrmsConverter 
    {
        // not going to use IConvertible because I think it's overkill
        // and we're not going to bind this to the type with reflection

        /// <summary>
        /// Converts a ORA Domain Study object into a Service Study object. 
        /// Be careful about null pointers. ORA.Domain contains complex objects and does not instantiate
        /// them with default CTORs all the time.
        /// </summary>
        /// <param name="study">ORA.Domain.Models.HumanSubjects.Study</param>
        /// <param name="ProjectLookup"></param>
        /// <param name="FAUKeysLookup">FAUKeys Lookup</param>
        /// <param name="FundEndDatesLookup"></param>
        /// <returns>ORA.Services.WebIRBCRMS.Models.Study</returns>
        WebIRBCRMS.Models.Study GetStudyFacade( ORA.Domain.Model.HumanSubjects.Study study, ILookup<string, ORA.Domain.Model.Project> ProjectLookup, ILookup<Tuple<string, string, string>, ORA.Domain.Model.FullAccountingUnitKey> FAUKeysLookup, ILookup<Tuple<string, string>, DateTime> FundEndDatesLookup );

        /// <summary>
        /// Converts an enumeration of ORA.Domain studies into an enumeration of Service Study.
        /// This is likely to be a critical method in this service.
        /// 
        /// Be careful about null pointers. ORA.Domain contains complex objects and does not instantiate
        /// them with default CTORs all the time.
        /// </summary>
        /// <param name="studies"></param>
        /// <param name="ProjectLookup"></param>
        /// <param name="FAUKeysLookup"></param>
        /// <param name="FundEndDatesLookup"></param>
        /// <returns></returns>
        IEnumerable<WebIRBCRMS.Models.Study> GetStudyFacades( IEnumerable<ORA.Domain.Model.HumanSubjects.Study> studies, ILookup<string, ORA.Domain.Model.Project> ProjectLookup, ILookup<Tuple<string, string, string>, ORA.Domain.Model.FullAccountingUnitKey> FAUKeysLookup, ILookup<Tuple<string, string>, DateTime> FundEndDatesLookup );

        /// <summary>
        /// Converts an enumeration of ORA.Domain studies into an enumeration of Services Status History
        /// facades.
        /// 
        /// Be careful about null pointers. ORA.Domain contains complex objects and does not instantiate
        /// them with default CTORs all the time.
        /// </summary>
        /// <param name="studies">IEnumerable of ORA.Domain.Model.HumanSubjects.Study </param>
        /// <returns>IEnumerable of WebIRBCRMS.Models.StatusHistory</returns>
        IEnumerable<WebIRBCRMS.Models.StudyStatusHistory> GetStatusHistoryFacades( IEnumerable<ORA.Domain.Model.HumanSubjects.Study> studies );

    }
}
