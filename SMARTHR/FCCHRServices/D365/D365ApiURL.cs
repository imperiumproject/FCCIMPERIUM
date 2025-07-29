using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using System.Text;

namespace SMARTHR.WEB.FCCHRServices.D365
{
	public static class D365ApiURL
	{	
		//--------------------------------------------AuthenticationURL----------------------------------------------------
		public const string LoginUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLoginService/login";
		public const string ForgotPasswordverifyUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLoginService/forgotPasswordVerify";
		public const string Updatepassword = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLoginService/updatePassword";
		public const string Token = "https://login.microsoftonline.com/b445bfd7-8ff7-4f9b-b1f3-f597234bc8fd/oauth2/token";
		//------------------------------------------BusinessTrip-------------------------------------------------------------
		public const string RecallWorkflowUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSWorkflowProcessService/recallWorkflow";
		public const string ReadBusniessTripRequestInfoUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSBusinessTripService/readBusniessTripRequestInfo";
		public const string GetBusinessTripIdUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSBusinessTripService/getbusinessTripId";
		public const string CreateBusinessTripRequestUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSBusinessTripService/createBusinessTripRequest";
		public const string UpdateBusinessTripRequestUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSBusinessTripService/updateBusinessTripRequest";
		public const string DeleteBusinessTripRequestUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSBusinessTripService/deleteBusinessTripRequest";
		public const string ReadBusinessTripRequestUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSBusinessTripService/readBusinessTripRequest";
		public const string ExpenseTypesListUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSBusinessTripService/expenseTypesList";
		public const string CountriesListUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSBusinessTripService/countriesList";
		//-------------------------------------------Leave Request-----------------------------------------------------------------
		public const string ReadLoanedItemsDetailsUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/readLoanedItemsDetails";
		public const string SubmitWorkflowUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSWorkflowProcessService/submitWorkflow";
		public const string ReadWFTrackingDetailsUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSWorkflowProcessService/readWFTrackingDetails";
		public const string ReadLeaveInfoUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/readLeaveInfo";
		public const string GetLeaveTypesUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/getLeaveTypes";
		public const string RequestCancelledUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/requestCancelled";
		public const string GetLeaveNumIdUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/getLeaveNumId";
		public const string GetNoOfDaysUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/getNoOfDays";
		public const string GetLeaveBalanceUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/getLeaveBalance";
		public const string GetAllEmployeesUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLoginService/getAllEmployees";
		public const string GetBackupEmplNamesUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/getBackupEmplNames";
		public const string CreateLeaveRequestUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/createLeaveRequest";
		public const string ReadLeaveRequestUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/readLeaveRequest";
		public const string ReadManagerLeaveResumption = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/readUpdateManagerLeaveResumption";
		public const string UpdateLeaveRequestUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/updateLeaveRequest";
		public const string DeleteLeaveRequestUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/deleteLeaveRequest";
		public const string ApproveWorkflowUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSWorkflowProcessService/approveWorkflow";
		public const string DelegateWorkflowUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSWorkflowProcessService/delegateWorkflow";
		public const string RejectWorkflowUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSWorkflowProcessService/rejectWorkflow";
		public const string ReadLeaveResumptionUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/readLeaveResumption";
		public const string ReadLeaveResumptionInfoUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/readLeaveResumptionInfo";
		public const string UpdateLeaveResumptionUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/updateLeaveResumption";
		public const string CreateAttachFileUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/createAttachFile";
		//----------------------------------------------Profile------------------------------------------------------------------

		public const string GetEmpProfileUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSEmployeeProfileService/getEmpProfile";
		//-----------------------------------------------Manager--------------------------------------------------------------------
		public const string WorkListByEmpGlobalFilterUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSWorkflowProcessService/workListByEmpGlobalFilter";


		//----------------------------------------------Circular--------------------------------------------------------------------
		#region #Circular
		public const string ReadUpdCircularListUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSCircularsService/readUpdCircularList";



		public const string ReadCircularAttachmentList = "/FCC_ESSIntgerationServiceGroup/FCC_ESSCircularsService/readCircularAttachmentList";
        public const string UpdateCircularCount = "/FCC_ESSIntgerationServiceGroup/FCC_ESSCircularsService/UpdCircularCount";
        #endregion

        #region #Jobs
        public const string ReadJobVacancyList = "/FCC_ESSIntgerationServiceGroup/FCC_ESSJobVacancyService/readJobVacancyList";
		public const string ReadJobVacancyInfo = "/FCC_ESSIntgerationServiceGroup/FCC_ESSJobVacancyService/readJobVacancyInfo";
		public const string ApplyForJob = "/FCC_ESSIntgerationServiceGroup/FCC_ESSJobVacancyService/applyForJob";
        #endregion

        #region #DocumentDownload
        public const string ReadDocumentList = "/FCC_ESSIntgerationServiceGroup/FCC_ESSDocumentDownloadService/readUpdCircularList";
        #endregion

        #region Courses
        public const string ReadCoursesList = "/FCC_ESSIntgerationServiceGroup/FCC_ESSCourseTableService/readCoursesList";
        public const string ReadCoursesInfo = "/FCC_ESSIntgerationServiceGroup/FCC_ESSCourseTableService/readCoursesInfo";
        public const string CourseRegister = "/FCC_ESSIntgerationServiceGroup/FCC_ESSCourseTableService/readCourseRegister";
        #endregion

    }
}