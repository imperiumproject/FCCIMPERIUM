using System.Text.Json;

namespace SMARTHR.WEB.FCCHRServices.D365
{
	public static class TranTypes
	{
		public static string fingerPrintTransType 
		{ 
			get 
			{ 
				return "fingerprintAmendment";
			} 
		}
		public const string Submitte = "Submitted";
		public const string BusinessTrip = "Business Trip";
		public const string Approve = "Approve";
		public const string Rejected = "Reject";
		public const string Delegation = "Delegation";
        public const string Certificate = "Certificate request";
        public const string ITAssets = "IT Assets";
		public const string LoanRequest = "Loan request";

    }
}
