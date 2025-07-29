using System.Text.Json.Serialization;

namespace SMARTHR.WEB.Models
{
    public class LoanRequestVM
    {
		[JsonPropertyName("$id")]
		public string? id {get;set;}
		[JsonPropertyName("loanId")]
		public string? LoanId { get; set; }
		[JsonPropertyName("templateName")]
		public string? TemplateName { get; set; }
		[JsonPropertyName("workflowUserid")]
		public string? WorkflowUserid { get; set; }
		[JsonPropertyName("loanType")]
		public string? LoanType { get; set; }
		[JsonPropertyName("loanDate")]
		public string? LoanDate { get; set; } // If needed as DateTime, convert accordingly
		[JsonPropertyName("methodOfPayment")]
		public string? MethodOfPayment { get; set; }
		[JsonPropertyName("remarks")]
		public string? Remarks { get; set; }
		[JsonPropertyName("companyId")]
		public string? CompanyId { get; set; }
		[JsonPropertyName("employeeName")]
		public string? EmployeeName { get; set; }
		[JsonPropertyName("employeeId")]
		public string? EmployeeId { get; set; }
		[JsonPropertyName("amount")]
		public decimal Amount { get; set; }
		[JsonPropertyName("loanDeduction")]
		public DateTime LoanDeduction { get; set; }
		[JsonPropertyName("raisedBy")]
		public string? RaisedBy { get; set; }
		[JsonPropertyName("tableId")]
		public int TableId { get; set; }
		[JsonPropertyName("recordId")]
		public long RecordId { get; set; }
		[JsonPropertyName("workflowTableId")]
		public int WorkflowTableId { get; set; }
		[JsonPropertyName("workflowPermissionId")]
		public long WorkflowPermissionId { get; set; }
		[JsonPropertyName("hideMsg")]
		public string? HideMsg { get; set; }
		[JsonPropertyName("status")]
		public string? Status { get; set; }
		[JsonPropertyName("attachRefList")]
		public List<AttachRef>? attachRefList { get; set; }
	}

	public class LoanRequestResult
	{
		[JsonPropertyName("$id")]
		public string? id { get; set; }
		[JsonPropertyName("loanRequestResult")]
		public List<LoanRequestVM>? loanRequestResult { get; set; }
	}


	public class ReadLoanRequest
	{
		public string? companyId { get; set; }
		public string? loanId { get; set; }
	}
	public class ReadLoanResponse
	{
		[JsonPropertyName("loanTableContract")]
		public ReadLoanRequest? loanTableContract { get; set; }
	}

	public class LoanItemsId
    {
        public string? companyId { get; set; }
    }
    public class LoanItemIdRequest
    {
        [JsonPropertyName("loanTableContract")]
        public LoanItemsId? LoanTableContract { get; set; }
    }
    public class CreateLoanRequest
    {
        public string? companyId { get; set; }
        public string? employeeId { get; set; }
        public decimal? amount { get; set; }
        public string? loanDate { get; set; }
        public string? loanDeduction { get; set; }
        public string? loanType { get; set; }
        public string? raisedBy { get; set; }
        public string? loanId { get; set; }
        public string? comments { get; set; }
        public List<AttachRef>? attachRefList { get; set; }
    }
    public class CreateLoanRequestReponse
    {
        [JsonPropertyName("loanTableContract")]
        public CreateLoanRequest? LoanTableContract { get; set; }
    }
    public class ReadLoanInfoRequest
    {
        [JsonPropertyName("loanTableContract")]
        public ReadAssetsInfo? loanTableContract { get; set; }
    }
}
