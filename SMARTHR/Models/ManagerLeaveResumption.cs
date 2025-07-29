using System.Text.Json.Serialization;

namespace SMARTHR.WEB.Models
{
	public class UnifiedLeaveResponse
	{
		public List<BusinessTripDeatils>? BusinessTripDetails { get; set; }
		public List<LeaveDetails>? LeaveDetails { get; set; } // Assuming LeaveDetail is the type of leaveDetails items
		public List<LoanItems>? loanList {  get; set; }
		public string? Message { get; set; }
		public bool Success { get; set; }
	}
	public class ManagerLeaveResumption
	{
		public string? CompanyCode { get; set; }
		public string? UserId { get; set; }
	   public string? leaveID { get; set; }
		public string? search { get; set; }
	}	

	public class ManagerLeaveResumptionResponse
	{
		[JsonPropertyName("readUpdateLeave")]
		public ManagerLeaveResumption? readUpdateLeave { get; set; }
	}

	public class WorkflowProcessServiceContract
	{
		public string? CompanyId { get; set; }
		public string? Status { get; set; }
		public string? RaisedBy { get; set; }
		public string? TransType { get; set; }
		public string? EmpId { get; set; }
	}
	public class WorkflowProcessServiceContractResponse
	{
		[JsonPropertyName("WorkflowProcessServiceContract")]
		public WorkflowProcessServiceContract? WorkflowProcessServiceContract { get; set; }
	}


	public class WorkflowProcess
	{
		[JsonPropertyName("approveDues")]
		public string? ApproveDues { get; set; }
		[JsonPropertyName("approveNegativeBalance")]
		public string? ApproveNegativeBalance { get; set; }
		[JsonPropertyName("status")]
		public string? Status { get; set; }
		[JsonPropertyName("specialApproval")]
		public string? SpecialApproval { get; set; }
		[JsonPropertyName("eSSStatus")]
		public string? ESSStatus { get; set; }
		[JsonPropertyName("value")]
		public string? Value { get; set; }
		[JsonPropertyName("wFTransID")]
		public string? WFTransID { get; set; }
		[JsonPropertyName("delegatedUserId")]
		public string? DelegatedUserId { get; set; }
		[JsonPropertyName("assignedUserId")]
		public string? AssignedUserId { get; set; }
		[JsonPropertyName("personnelNumber")]
		public string? PersonnelNumber { get; set; }
		[JsonPropertyName("raisedBy")]
		public string? RaisedBy { get; set; }
		[JsonPropertyName("employeeName")]
		public string? EmployeeName { get; set; }
		[JsonPropertyName("transactionId")]
		public string? TransactionId { get; set; }
		[JsonPropertyName("transCompanyId")]
		public string? TransCompanyId { get; set; }
		[JsonPropertyName("extendedLeave")]
		public int ExtendedLeave { get; set; }
		[JsonPropertyName("levelId")]
		public int LevelId { get; set; }
		[JsonPropertyName("transLineTableId")]
		public int TransLineTableId { get; set; }
		[JsonPropertyName("transTableId")]
		public int TransTableId { get; set; }
		[JsonPropertyName("remarks")]
		public string? Remarks { get; set; }
		[JsonPropertyName("createdTransDate")]
		public string? CreatedTransDate { get; set; }
		[JsonPropertyName("transDate")]
		public string? TransDate { get; set; }
		[JsonPropertyName("transLineRecId")]
		public long TransLineRecId { get; set; }
		[JsonPropertyName("transRecId")]
		public long TransRecId { get; set; }
	}
	public class WorkflowProcessList
	{
		[JsonPropertyName("workflowProcessList")]
		public List<WorkflowProcess>? WorkflowProcesses { get; set; }
	}

	public class ApprovalWorkFlow
	{
		[JsonPropertyName("RecordId")]
		public long RecordId {  get; set; }
		[JsonPropertyName("TableId")]
		public string? TableId {  get; set; }
		[JsonPropertyName("CompanyId")]
		public string? CompanyId {  get; set; }
		[JsonPropertyName("Comments")]
		public string? Comments {  get; set; }
		[JsonPropertyName("SpecialApproval")]
		public string? SpecialApproval {  get; set; }
        public string? workFlowValue { get; set; }
    }
	public class ApprovalWorkFlowResponse
	{
		[JsonPropertyName("_contract")]
		public ApprovalWorkFlow? ContractWorkFlow { get; set; }
	}
    public class RejectWorkFlow
    {
        [JsonPropertyName("RecordId")]
        public long RecordId { get; set; }
        [JsonPropertyName("TableId")]
        public string? TableId { get; set; }
        [JsonPropertyName("Comments")]
        public string? Comments { get; set; }
        [JsonPropertyName("CompanyId")]
        public string? CompanyId { get; set; }
    }
    public class RejectWorkFlowResponse
    {
        [JsonPropertyName("_contract")]
        public RejectWorkFlow? Contract { get; set; }
    }
}
