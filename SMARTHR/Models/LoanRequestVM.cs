using System.Text.Json.Serialization;

namespace SMARTHR.WEB.Models
{
    public class LoanRequestVM
    {
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
