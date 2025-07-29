using NuGet.Packaging.Signing;
using System.Text.Json.Serialization;

namespace SMARTHR.WEB.Models
{

    public class CertificateRequestViewModel
    {
        [JsonPropertyName("$id")]
        public string? Id { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }

        [JsonPropertyName("messageFlag")]
        public string? MessageFlag { get; set; }

        [JsonIgnore]
        public bool Success => Message?.ToLower() == "yes";
    }

    public class LeaveNumIdRequestC
    {
        [JsonPropertyName("contract")]
        public ContractVMC? contract { get; set; }
    }
    public class ContractVMC
    {
        public string? companyCode { get; set; }
    }
 
    public class CreateCertificateRequest
    {
        public string? companyId { get; set; }
        public string? userID { get; set; } // Changed to string for parsing
        public string? CertificateRequestId { get; set; }   // Changed to string for parsing
        public string? transDate { get; set; }   // Changed to string for parsing
        public string? description { get; set; }    
        public string? wfComments { get; set; }
        public string? RaisedBy { get; set; }
        // added
        public string? type { get; set; }
        public List<AttachmentRef>? attachRefList { get; set; }

    }

    public class CreateCertificateContractRequest
    {
        [JsonPropertyName("certificateDetails")]
        public CreateCertificateRequest? certificateDetails { get; set; }
    }
    public class ReadCertificateInfoRespone
    {

        [JsonPropertyName("$id")]
        public string? Id { get; set; }


        [JsonPropertyName("certificateDetails")]
        public List<CertificateDetails>? CertificateDetails { get; set; }

    }

    public class ReadCertificateResponeById
    {

        [JsonPropertyName("$id")]
        public string? Id { get; set; }

        [JsonPropertyName("certificateDetails")]
        public List<CertificateDetails>? CertificateDetails { get; set; }

    }
    public class CertificateDetails
    {


        [JsonPropertyName("emplId")]
        public string? EmplId { get; set; }
        [JsonPropertyName("emplName")]
        public string? EmplName { get; set; }

        [JsonPropertyName("certificateId")]
        public string? CertificateId { get; set; }

        [JsonPropertyName("hideMsg")]
        public string? HideMsg { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("userID")]
        public string? UserID { get; set; }

        [JsonPropertyName("userName")]
        public string? UserName { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        public string? transDate { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("certificateRecId")]
        public long CertificateRecId { get; set; }

        [JsonPropertyName("requestrecId")]
        public long RequestrecId { get; set; }

        [JsonPropertyName("msg")]
        public string? Msg { get; set; }

        [JsonPropertyName("msgFlag")]
        public string? MsgFlag { get; set; }

        [JsonPropertyName("workflowTableId")]
        public long WorkflowTableId { get; set; }

        [JsonPropertyName("workflowPermissionId")]
        public long WorkflowPermissionId { get; set; }

        [JsonPropertyName("workflowUserid")]
        public string? WorkflowUserid { get; set; }
        [JsonPropertyName("requestTableId")]
        public long requestTableId { get; set; }

        //added
        [JsonPropertyName("wfComments")]
        public string? wfComments { get; set; }
        [JsonPropertyName("attachRefList")]
        public object? AttachRefList { get; set; }
    }

    public class certificateRequestList
    {
        public string? companyId { get; set; }
        public string? userID { get; set; }
        public string? CertificateRequestId { get; set; }
    }

    public class ReadCertificateRequestInfo
    {
        [JsonPropertyName("certificateRequestList")]
        public certificateRequestList? certificateRequestList { get; set; }
    }
	public class RootObjects
	{
		public List<AttachmentRef> ? attachRefList { get; set; }
	}
	public class UpdateCertificateRequest
    {
        [JsonPropertyName("$id")]
        public string? Id { get; set; }

        [JsonPropertyName("certificateRequestList")]
        public List<UpdateCertificate>? certificateRequestLists { get; set; }
    }

    public class UpdateCertificate
    {
        public string? companyId { get; set; }
        public string? requestType { get; set; } // Changed to string for parsing
        public string? CertificateRequestId { get; set; }   // Changed to string for parsing
        public string? description { get; set; }   // Changed to string for parsing
        public string? transDate { get; set; }   // Changed to string for parsing
        public string? type { get; set; }
        public string? userID { get; set; }
        public string? wfComments { get; set; }
    }
    public class ReadUpdateCertificateRequestInfo
    {
        [JsonPropertyName("certificateRequestList")]
        public UpdateCertificate? certificateRequestList { get; set; }
    }

    public class CertificateResponse
    {
        [JsonPropertyName("$id")]
        public string? Id { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }

        [JsonPropertyName("messageFlag")]
        public string? MessageFlag { get; set; }

        [JsonIgnore]
        public bool Success => Message?.ToLower() == "yes";
    }
    public class DeleteCretificateResponseRequest
    {
        [JsonPropertyName("certificateDetails")]
        public DeleteContractRequestDetails? certificateDetails { get; set; }
    }

    public class DeleteAttachedCretificateResponseRequest
    {
        [JsonPropertyName("deleteAttachFile")]
        public DeleteAttchedContractRequestDetails? deleteAttachFile { get; set; }
    }
    public class DeleteContractRequestDetails
    {
        public string? companyId { get; set; }
        public string? CertificateRequestId { get; set; }

    }

    public class DeleteAttchedContractRequestDetails
    {
        public string? companyId { get; set; }
        public string? attachRecID { get; set; }

    }
    public class certificateTypes
    {
        //[JsonPropertyName("certificateId")]
        //public string? certificateId { get; set; }
        //[JsonPropertyName("certificateName")]
        //public string? certificateName { get; set; }
        public string? Id { get; set; } // optional - $id
        public List<string>? CertificateTypes { get; set; }
    }

    public class CertificateData
    {
  
    }
    public class CertificateListResponse
    {
        [JsonPropertyName("CertificateList")]
        public certificateTypes Certificate { get; set; } = new certificateTypes();
    }
    public class AttachmentRef
    {
        public string? fileName { get; set; }
        public string? attachRef { get; set; }
    }

    public class LeaveAttachWrapper
    {
        public List<AttachmentRef> attachRefList { get; set; }
    }
}
