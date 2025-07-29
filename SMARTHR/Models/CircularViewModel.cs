using System.Text.Json.Serialization;

namespace SMARTHR.WEB.Models
{
    public class CircularViewModel
    {
    }
    public class circularList
    {
        public string? CompanyCode { get; set; }
        public string? search { get; set; }
        public long? hrCircularRecID { get; set; }
      public string ? empId {  get; set; }

    }

    public class circularListRequest
    {
        [JsonPropertyName("circularList")]
        //public List<circularList>? circularList { get; set; }
        public circularList? circularList { get; set; }
    }
    public class CircularResponse
    {
        [JsonPropertyName("hrCircularDetails")]
        public List<HrCircularDetail>? HrCircularDetails { get; set; }
    }
    public class HrCircularDetail
    {
        [JsonPropertyName("circularName")]
        public string? CircularName { get; set; }

        [JsonPropertyName("circularDescription")]
        public string? CircularDescription { get; set; }

        [JsonPropertyName("circularDate")]
        public string? CircularDate { get; set; }

        [JsonPropertyName("hideFlag")]
        public bool HideFlag { get; set; }

        [JsonPropertyName("hrCircularRecId")]
        public long HrCircularRecId { get; set; }

        [JsonPropertyName("requestRecId")]
        public long RequestRecId { get; set; }

        [JsonPropertyName("attachRefList")]
        public List<AttachRefList>? AttachRefList { get; set; }
    }

    public class AttachRefList
    {
        [JsonPropertyName("fileName")]
        public string? FileName { get; set; }

        [JsonPropertyName("attachRef")]
        public string? AttachRefBase64 { get; set; }

        [JsonPropertyName("attachRecID")]
        public long AttachRecID { get; set; }

        [JsonPropertyName("attachedBy")]
        public string? AttachedBy { get; set; }

        [JsonPropertyName("attachedDate")]
        public DateTime AttachedDate { get; set; }
    }

    public class circularAttachmentList
    { 
       public string? companyCode { get; set; }
        public string? uniqueID { get; set; }

        public string? requestRecID { get; set; }

        public string? attachRecID { get; set; }

    }
    public class circularAttachmentListRequest

    {

        [JsonPropertyName("circularAttachmentList")]
        //public List<circularAttachmentList>? circularAttachmentList { get; set; }
        public circularAttachmentList? circularAttachmentList { get; set; }

    }


    public class documentContract
    {
     public string? CompanyCode { get; set; }
     public string? emplID{ get; set; }
    }


  public class documentContractRequest
    {
        [JsonPropertyName("documentContract")]
       
        public documentContract? documentContract { get; set; }
    }

    public class DocumentDownloadResponse
    {
        [JsonPropertyName("documentDownloadDetials")]
        public List<DocumentDownloadDetail>? DocumentDownloadDetials { get; set; }
    }

    public class DocumentDownloadDetail
    {
        [JsonPropertyName("emplID")]
        public string? EmplID { get; set; }

        [JsonPropertyName("employeeName")]
        public string? EmployeeName { get; set; }

        [JsonPropertyName("identificationNum")]
        public string? IdentificationNum { get; set; }

        [JsonPropertyName("identificationType")]
        public string? IdentificationType { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("expirationDate")]
        public string? ExpirationDate { get; set; }

        [JsonPropertyName("attachRefList")]
        public List<AttachRefList>? AttachRefList { get; set; }
    }

    
}
