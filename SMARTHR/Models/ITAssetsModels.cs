using System.Text.Json.Serialization;

namespace SMARTHR.WEB.Models
{
	public class ITAssetsModels
	{
        [JsonPropertyName("$id")]
        public string? id{get;set;}
        public string? requestId { get; set; }
        public string? employeeName { get; set; }
        public string? employeeId { get; set; }
        public string? loanType { get; set; }
        public string? purpose { get; set; }
        public string? additionalDetails { get; set; }
        public string? requiredTill { get; set; }
        public string? requiredBy { get; set; }
		public string? transDate {  get; set; }
		public string? companyId { get; set; }
        public string? status { get; set; }
        public string? comments { get; set; }
        [JsonConverter(typeof(FlexibleIntConverter))]
        public int? tableId { get; set; }
        [JsonConverter(typeof(FlexibleLongConverter))]
        public long? recordId { get; set; }
        public string? raisedBy { get; set; }
        public string? msg { get; set; }
        public string? msgFlag { get; set; }
        public string? templateName { get; set; }
        [JsonConverter(typeof(FlexibleIntConverter))]
        public int? workflowTableId { get; set; }
        [JsonConverter(typeof(FlexibleLongConverter))]
        public long? workflowPermissionId { get; set; }
        public string? workflowUserid { get; set; }
        public string? hideMsg { get; set; }
        public List<AttachRef>? attachRefList { get; set; }
    }

	public class ITAssetsReadInfoResponse
	{
        [JsonPropertyName("$id")]
        public string? id { get; set; }
        [JsonPropertyName("assetRequestList")]
        public List<ITAssetsModels>? assetRequestList { get; set; }		
	}


	public class ReadAssetsDetailsResponse
	{
		[JsonPropertyName("$id")]
		public string? id { get; set; }	
		[JsonPropertyName("assetRequestList")]
		public ITAssetsModels? assetRequestList { get; set; }
	}




	public class ReadAssetsInfoRequest
    {
        [JsonPropertyName("assetRequestContract")]
        public ReadAssetsInfo? assetRequestContract { get; set; }
    }
    public class ReadAssetsInfo
	{
        public string? companyId { get; set; }
		public string? employeeId { get; set; }
    }

	public class ReadAssetsDetailsRequest
	{
		[JsonPropertyName("assetRequestContract")]
		public ReadAssetsDetails? assetRequestContract { get; set; }
	}
	public class ReadAssetsDetails
	{
		public string? companyId { get; set; }
		public string? requestId { get; set; }
	}
	public class ItAssetsId
	{
		public string? companyId { get; set; }
	}
	public class ItAssetsNumIdRequest
	{
		[JsonPropertyName("assetRequestContract")]
		public ItAssetsId? AssetRequestContract { get; set; }
	}
	public class LoanTypeResponse
	{
		[JsonPropertyName("$id")]
		public string? id { get; set; }
		[JsonPropertyName("loanTypeResultList")]
		public List<LoanTypes>? loanTypeResultList {  get; set; }
	}
	public class LoanTypes
	{
		[JsonPropertyName("$id")]
		public string? id { get; set; }
		[JsonPropertyName("loanType")]
		public string? loanType { get; set; }
	}
	public class AssetRequestContract
	{
		public string? companyId { get; set; }
		public string? employeeId { get; set; }
		public string? raisedBy { get; set; }
		public string? requestId { get; set; }
		public string? requiredBy { get; set; }
		public string? requiredTill { get; set; }
		public string? loanType { get; set; }
		public string? purpose { get; set; }
		public string? additionalDetails { get; set; }
		public string? comments { get; set; }
		public List<AttachRef>? attachRefList { get; set; }
	}
	public class AssetRequestContractResponse
	{
		[JsonPropertyName("assetRequestContract")]
		public AssetRequestContract? assetRequestContract { get; set; }
	}
}
