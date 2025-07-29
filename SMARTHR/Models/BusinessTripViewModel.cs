using System.Text.Json.Serialization;

namespace SMARTHR.WEB.Models
{
	public class BusinessTripViewModel
	{
	}
	public class Contract
	{
		public string? CompanyId { get; set; }
	}
	public class contractRequest
	{
		[JsonPropertyName("contract")]
		public Contract? contract { get; set; }
	}
	public class BusinessTypeExpence
	{
		public string? companyId { get; set; }
	}
	public class BusinessExpencetypeResponse
	{
		[JsonPropertyName("businessTripContract")]
		public BusinessTypeExpence? businessTripContract { get; set; }
	}

	public class BusinessCountryResponse
	{
		[JsonPropertyName("essbusinessTripContract")]
		public BusinessTypeExpence? essbusinessTripContract { get; set; }
	}


	public class businessTripContract
	{
		public string? companyId { get; set; }
		public string? userId { get; set; }
		public string? BusinessTripReqId { get; set; }
		public string? BusinessTripsExpenseType { get; set; }
		public string? department { get; set; }
		public string? SourceCountry { get; set; }
		public string? DestinationCountry { get; set; }
		public string? FromDate { get; set; }
		public string? ToDate { get; set; }
		public string? Remarks { get; set; }
		public string? CurrencyCode { get; set; }
		public string? RaisedBy {  get; set; }

		public List<attachRefList>? attachRefList { get; set; }
    }
    
    public class CreateBusinessTripRequest
	{
		[JsonPropertyName("businessTripContract")]
		public businessTripContract? businessTripContract { get; set; }
	}
	public class BusinessTripResponse
	{
		[JsonPropertyName("$id")]
		public string? Id { get; set; }

		[JsonPropertyName("message")]
		public string? Message { get; set; }

		[JsonPropertyName("messageFlag")]
		public string? MessageFlag { get; set; }


	}
	public class ReadBusinessTripRespone
	{

		[JsonPropertyName("$id")]
		public string? Id { get; set; }

		[JsonPropertyName("LeaveDetails")]
		public List<LeaveDetails>? LeaveDetails { get; set; }
	}
	public class _fccESSBusinessTripContract
	{
		public string? companyId { get; set; }
		public string? BusinessTripReqId { get; set; }

	}
	public class ReadBusinessTripRequest
	{
		[JsonPropertyName("readBusinessTripAPIContract")]
		public readBusinessTripAPIContract? readBusinessTripAPIContract { get; set; }

	}
	public class readBusinessTripAPIContract
	{
		public string? BusinessTripReqId { get; set; }
		public string ? UserId { get; set; }

        public string? companyId { get; set; }
	}

	public class DeleteBusinessTripRequest
	{
		[JsonPropertyName("_fccESSBusinessTripContract")]
		public _fccESSBusinessTripContract? _fccESSBusinessTripContract { get; set; }

	}

	public class UpdateBusinessTripRequest
	{
		[JsonPropertyName("businessTripContract")]
		public businessTripContract? businessTripContract { get; set; }
	}
	public class ReadBusinessTripResponeForDelete
	{

		[JsonPropertyName("$id")]
		public string? Id { get; set; }

		[JsonPropertyName("DeleteBusinessTripResponse")]
		public List<DeleteBusinessTripResponse>? DeleteBusinessTripResponse { get; set; }
	}
	public class DeleteBusinessTripResponse
	{
		[JsonPropertyName("companyId")]
		public string? CompanyId { get; set; }

		[JsonPropertyName("WorkerRecId")]
		public long WorkerRecId { get; set; }

		[JsonPropertyName("TableId")]
		public int TableId { get; set; }

		[JsonPropertyName("BusinessTripRecId")]
		public long BusinessTripRecId { get; set; }

		[JsonPropertyName("Remarks")]
		public string? Remarks { get; set; }

		[JsonPropertyName("BusinessTripsExpenseType")]
		public string? BusinessTripsExpenseType { get; set; }

		[JsonPropertyName("WorkflowStatus")]
		public int WorkflowStatus { get; set; }

		[JsonPropertyName("DestinationCountry")]
		public string? DestinationCountry { get; set; }

		[JsonPropertyName("SourceCountry")]
		public string? SourceCountry { get; set; }

		[JsonPropertyName("CountryName")]
		public string? CountryName { get; set; }

		[JsonPropertyName("CurrencyCode")]
		public string? CurrencyCode { get; set; }

		[JsonPropertyName("WorkerName")]
		public string? WorkerName { get; set; }

		[JsonPropertyName("UserId")]
		public string? UserId { get; set; }

		[JsonPropertyName("SubmitedUserId")]
		public string? SubmitedUserId { get; set; }

		[JsonPropertyName("BusinessTripId")]
		public string? BusinessTripId { get; set; }

		[JsonPropertyName("TransDate")]
		public DateTime TransDate { get; set; }

		[JsonPropertyName("FromDate")]
		public DateTime FromDate { get; set; }

		[JsonPropertyName("ToDate")]
		public DateTime ToDate { get; set; }

		[JsonPropertyName("Error")]
		public string? Error { get; set; }

		[JsonPropertyName("reportingToCompnayHead")]
		public int ReportingToCompnayHead { get; set; }

		[JsonPropertyName("budgeted")]
		public int Budgeted { get; set; }

		[JsonPropertyName("department")]
		public string? Department { get; set; }
	}
	public class ReadBusinessTripResponse
	{

		[JsonPropertyName("$id")]
		public string? Id { get; set; }

		[JsonPropertyName("WorkerRecId")]
		public long WorkerRecId { get; set; }

		[JsonPropertyName("TableId")]
		public int TableId { get; set; }

		[JsonPropertyName("BusinessTripRecId")]
		public long BusinessTripRecId { get; set; }

		[JsonPropertyName("Remarks")]
		public string? Remarks { get; set; }

		[JsonPropertyName("BusinessTripsExpenseType")]
		public string? BusinessTripsExpenseType { get; set; }

		[JsonPropertyName("WorkflowStatus")]
		public int WorkflowStatus { get; set; }

		[JsonPropertyName("DestinationCountry")]
		public string? DestinationCountry { get; set; }

		[JsonPropertyName("SourceCountry")]
		public string? SourceCountry { get; set; }

		[JsonPropertyName("CountryName")]
		public string? CountryName { get; set; }

		[JsonPropertyName("CurrencyCode")]
		public string? CurrencyCode { get; set; }

		[JsonPropertyName("WorkerName")]
		public string? WorkerName { get; set; }

		[JsonPropertyName("SubmitedUserId")]
		public string? SubmitedUserId { get; set; }

		[JsonPropertyName("BusinessTripReqId")]
		public string? BusinessTripReqId { get; set; }

		[JsonPropertyName("TransDate")]
		public DateTime TransDate { get; set; }

		[JsonPropertyName("FromDate")]
		public DateTime FromDate { get; set; }

		[JsonPropertyName("ToDate")]
		public DateTime ToDate { get; set; }

		[JsonPropertyName("Error")]
		public string? Error { get; set; }

		[JsonPropertyName("budgeted")]
		public int Budgeted { get; set; }

		[JsonPropertyName("department")]
		public string? Department { get; set; }

		[JsonPropertyName("emplId")]
		public string? EmplId { get; set; }

		[JsonPropertyName("employeeName")]
		public string? EmployeeName { get; set; }

		[JsonPropertyName("workflowPermissionId")]
		public long WorkflowPermissionId { get; set; }

		[JsonPropertyName("workflowTableId")]
		public int WorkflowTableId { get; set; }

		[JsonPropertyName("workflowUserid")]
		public string? WorkflowUserId { get; set; }

		[JsonPropertyName("workflowlevelId")]
		public string? WorkflowLevelId { get; set; }

        [JsonPropertyName("attachRefList")]
        public List<AttachRefItem>? AttachRefList { get; set; }

    }

	public class BusinessTripContractList
	{
		public string? companyId { get; set; }
		public string? UserId { get; set; }
	}

	public class BusinessTripContractListResponse
	{
		[JsonPropertyName("businessTripContract")]
		public BusinessTripContractList? businessTripContract { get; set; }
	}



	public class BusinessTripDeatils
	{
		[JsonPropertyName("$id")]
		public string? Id { get; set; }
		[JsonPropertyName("BusinessTripReqId")]
		public string? BusinessTripReqId { get; set; }
		[JsonPropertyName("BusinessTripsExpenseType")]
		public string? BusinessTripsExpenseType { get; set; }
		[JsonPropertyName("WorkerName")]
		public string? WorkerName { get; set; }
		[JsonPropertyName("DestinationCountry")]
		public string? DestinationCountry { get; set; }
		[JsonPropertyName("SourceCountry")]
		public string? SourceCountry { get; set; }
		[JsonPropertyName("TransDate")]
		public string? TransDate { get; set; }
		[JsonPropertyName("FromDate")]
		public string? FromDate { get; set; }
		[JsonPropertyName("ToDate")]
		public string? ToDate { get; set; }
		[JsonConverter(typeof(FlexibleIntConverter))]
		[JsonPropertyName("NoOfDays")]
		public int NoOfDays { get; set; }
		[JsonPropertyName("returnDate")]
		public string? returnDate { get; set; }
		[JsonPropertyName("raisedBy")]
		public string? raisedBy { get; set; }
		[JsonPropertyName("WorkflowStatus")]
		public string? WorkflowStatus { get; set; }
		[JsonConverter(typeof(FlexibleIntConverter))]
		[JsonPropertyName("TableId")]
		public int TableId { get; set; }
		[JsonPropertyName("BusinessTripRecId")]
		public long? BusinessTripRecId { get; set; }
		[JsonPropertyName("Remarks")]
		public string? Remarks { get; set; }
		[JsonPropertyName("CountryName")]
		public string? CountryName { get; set; }
		[JsonPropertyName("CurrencyCode")]
		public string? CurrencyCode { get; set; }
		[JsonPropertyName("SubmitedUserId")]
		public string? SubmitedUserId { get; set; }
		[JsonPropertyName("Error")]
		public string? Error { get; set; }
		[JsonPropertyName("budgeted")]
		public int? Budgeted { get; set; }
		[JsonPropertyName("department")]
		public string? Department { get; set; }
		[JsonPropertyName("emplId")]
		public string? EmplId { get; set; }
		[JsonPropertyName("employeeName")]
		public string? EmployeeName { get; set; }
		[JsonPropertyName("assignedUserId")]
		public string? assignedUserId { get; set; }
		[JsonConverter(typeof(FlexibleIntConverter))]
		[JsonPropertyName("workflowPermissionId")]
		public int WorkflowPermissionId { get; set; }
        [JsonConverter(typeof(FlexibleLongConverter))]
        [JsonPropertyName("workflowRecIdId")]
        public long workflowRecIdId { get; set; }
        [JsonConverter(typeof(FlexibleIntConverter))]
		[JsonPropertyName("workflowTableId")]
		public int WorkflowTableId { get; set; }
		[JsonPropertyName("workflowUserid")]
		public string? WorkflowUserid { get; set; }
		[JsonPropertyName("workflowlevelId")]
		public string? WorkflowlevelId { get; set; }
		[JsonPropertyName("attachRefList")]
		public List<AttachRefItem>? AttachRefList { get; set; } // Use JsonElement or create a separate class if it's a known structure
	}

	public class BusinessTripResponseResponse
	{
		[JsonPropertyName("$id")]
		public string? Id { get; set; }
		[JsonPropertyName("businessDetails")]
		public List<BusinessTripDeatils>? BusinessDetails { get; set; }
	}
	public class ExpenseTypeModel
	{
		[JsonPropertyName("$id")]
		public string? Id { get; set; }

		[JsonPropertyName("expenseType")]
		public string? ExpenseType { get; set; }
	}

	public class CountryListModel
	{
		[JsonPropertyName("$id")]
		public string? Id { get; set; }

		[JsonPropertyName("countryCode")]
		public string? countryCode { get; set; }
		[JsonPropertyName("countryName")]
		public string? countryName { get; set; }
	}

    public class RecallWorkFlow
    {
        [JsonPropertyName("RaisedBy")]
        public string? RaisedBy { get; set; }
        [JsonPropertyName("RecordId")]
        public long RecordId { get; set; }
        [JsonPropertyName("TableId")]
        public int? TableId { get; set; }
        [JsonPropertyName("ReturnToLevel")]
        public string? ReturnToLevel { get; set; }
		[JsonPropertyName("CompanyId")]
		public string? CompanyId { get; set; }
    }

    public class RecallWorkResponse
    {
        [JsonPropertyName("_contract")] // Changed to match JSON
        public RecallWorkFlow? Contract { get; set; }
    }
}
