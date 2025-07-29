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
        public const string ReadCertificateUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSCertificateRequestService/readUpdManagerCertificateList";
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

        //----------------------------------------------------------Loan Request------------------------------
        #region Loan Request
        public const string GetLoanIdUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSWorkerLoanTableService/getLoanId";
        public const string ReadLoanRequestTypeUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSWorkerLoanTableService/readLoanType";
        public const string CreateLoanRequestUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSWorkerLoanTableService/createLoanRequest";
        public const string ReadLoanTableRequestListUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSWorkerLoanTableService/readLoanTableRequestInfo";
		public const string ReadLoanRequestUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSWorkerLoanTableService/readUpdateLoanRequest";
		#endregion

		#region Circular
		public const string ReadUpdCircularListUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSCircularsService/readUpdCircularList";
		#endregion


		#region It Asset
		public const string GetAssetRequestIdUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSAssetRequestService/getAssetRequestId";
		public const string AssetsRequestCancelledUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSAssetRequestService/requestCancelled";
		public const string ReadLoanTypeUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSAssetRequestService/readLoanType";
		public const string CreateUpdateAssetRequestUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSAssetRequestService/createAssetRequest";
		public const string UpdateAssetRequestUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSAssetRequestService/readUpdateAssetRequest";
		public const string DeleteAssetRequestUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSAssetRequestService/deleteAssetRequest";
		public const string ReadAssetRequestUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSAssetRequestService/readUpdateAssetRequest";
		public const string ReadAssetRequestListUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSAssetRequestService/readAssetRequestInfo";
		public const string UpdateManagerAssetRequestListUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSAssetRequestService/readUpdateManagerAssetRequest";
		#endregion

		//------------------------------------------------------Permission Request-------------------
		public const string PermissionTypesUrl = "/FCC_ESSIntgerationServiceGroup/FCC_ESSPermissionService/readpermissionTypes";
	}
}