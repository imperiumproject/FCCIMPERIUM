using System.Text.Json.Serialization;

namespace SMARTHR.WEB.Models
{
	public class PermissionRequestVM
	{
	}
	public class PermissionContract
	{
		public string? companyCode { get; set; }
	}
	public class PermissionContractRequest
	{
		[JsonPropertyName("permissionContract")]
		public PermissionContract? permissionContract { get; set; }
	}


	public class PermissionTypeResponse
	{
		[JsonPropertyName("$id")]
		public string? id { get; set; }
		[JsonPropertyName("earlyLeaveRequestTypeList")]
		public List<PermissionTypes>? EarlyLeaveRequestTypeList { get; set; }
	}
	public class PermissionTypes
	{
		[JsonPropertyName("$id")]
		public string? id { get; set; }
		[JsonPropertyName("leaveType")]
		public string? leaveType { get; set; }
	}
}
