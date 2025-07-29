using System.Text.Json;
using System.Text.Json.Serialization;

namespace SMARTHR.WEB.Models
{
    public class LeaveRequestViewModel
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Status { get; set; }
    }
	public class WorkflowTrackingList
	{
		[JsonPropertyName("wfTrackingList")]
		public List<WorkflowEntry> WFTrackingList { get; set; } = new List<WorkflowEntry>();
	}
	public class WorkflowEntry
	{
		[JsonPropertyName("status")]
		public string Status { get; set; } = string.Empty;

		[JsonPropertyName("userId")]
		public string? UserId { get; set; }

		[JsonPropertyName("wFTransID")]
		public string WFTransId { get; set; } = string.Empty;

		[JsonPropertyName("comments")]
		public string? Comments { get; set; }

		[JsonPropertyName("createdDateTime")]
		public string? CreatedDateTime { get; set; }

		[JsonPropertyName("transDate")]
		public string TransDate { get; set; } = string.Empty;

		[JsonPropertyName("employeeName")]
		public string? EmployeeName { get; set; }

		[JsonPropertyName("levelName")]
		public string? LevelName { get; set; }

		[JsonPropertyName("levelId")]
		public int LevelId { get; set; }

		[JsonPropertyName("raisedBy")]
		public string RaisedBy { get; set; } = string.Empty;

		[JsonPropertyName("refRecId")]
		public int RefRecId { get; set; }

		[JsonPropertyName("refTableId")]
		public int RefTableId { get; set; }

		[JsonPropertyName("remarks")]
		public string? Remarks { get; set; }
	}
	public class WorkFlowHistoryTracking
	{
		[JsonPropertyName("CompanyId")]
		public string? CompanyId { get; set; }
		[JsonPropertyName("TransType")]
		public string? TransType { get; set; }
		[JsonPropertyName("RecordId")]
		public long? RecordId { get; set; }
		[JsonPropertyName("TableId")]
		public int? TableId { get; set; }	
	}
	public class WorkFlowHistoryTrackingResponse
	{
		[JsonPropertyName("wfContract")] // Changed to match JSON
		public WorkFlowHistoryTracking? WfContract { get; set; }
	}

	public class WorkFlowSubmitRequest
	{
		[JsonPropertyName("workflowTemplateName")]
		public string? WorkflowTemplateName { get; set; }
		[JsonPropertyName("TableId")]
		public int? TableId { get; set; }
		[JsonPropertyName("RecordId")]
		public long? RecordId { get; set; }

		[JsonPropertyName("Comments")]
		public string? Comments { get; set; }
		[JsonPropertyName("raisedUser")]
		public string? RaisedUser { get; set; }
		[JsonPropertyName("submittingUser")]
		public string? SubmittingUser { get; set; }
		[JsonPropertyName("CompanyId")]
		public string? CompanyId { get; set; }

	}

	public class WorkFlowSubmitResponse
	{
		[JsonPropertyName("wfContract")] // Changed to match JSON
		public WorkFlowSubmitRequest? WfContract { get; set; }
	}

	public class UserLeaveTypeViewModel
	{
        public string? UserId { get; set; }
		public string? companyCode { get; set; }
		public string? Nationality { get; set; }
		public string? Religioncheck { get; set; }
    }
	
	public class EssUserLeaveTypeViewModel
	{
		[JsonPropertyName("userleaveTypes")] // Changed to match JSON
		public UserLeaveTypeViewModel? UserleaveTypes { get; set; }
	}


    public class CancelLeaveRequestViewModel
    {
		public string? CompanyCode { get; set; }
        public string? leaveNum {  get; set; }     
	}

	public class EssCancelLeaveRequestViewModel
	{
		[JsonPropertyName("leaveContract")] 
		public CancelLeaveRequestViewModel? LeaveContract { get; set; }
	}

	public class LeaveNumId
	{
		public string? companyId { get; set; }
	}

	public class LeaveResponse
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
	public class ContractVM
	{
		public string? companyId { get; set; }
	}
	public class LeaveNumIdRequest
	{
		[JsonPropertyName("contract")]
		public ContractVM? contract { get; set; }
	}
	public class leaveContract
	{
		public string? CompanyCode { get; set; }
		public string? LeaveStart { get; set; }
		public string? LeaveEnd { get; set; }
		public string? UserId { get; set; }
		public string? LeaveType { get; set; }
	}
	public class leaveContractRequest
	{
		[JsonPropertyName("leaveContract")]
		public leaveContract? leaveContract { get; set; }
	}
	public class leaveBalanceContract
	{
		public string? CompanyCode { get; set; }
		public string? UserId { get; set; }
		public string? LeaveType { get; set; }
	}
	public class leaveBalanceRequest
	{
		[JsonPropertyName("leaveBalanceContract")]
		public leaveBalanceContract? leaveBalanceContract { get; set; }
	}
	public class EmployeeList
	{
		[JsonPropertyName("employeeId")]
		public string? employeeId { get; set; }
		[JsonPropertyName("employeeName")]
		public string? employeeName { get; set; }
	}
	public class EmployeeListResponse
	{
		[JsonPropertyName("EmployeeList")]
		public List<EmployeeList> Employees { get; set; } = new List<EmployeeList>();
	}
	public class userInfoContract
	{
		public string? CompanyCode { get; set; }
		public string? UserId { get; set; }
	}
	public class userInfoContractRequest
	{
		[JsonPropertyName("userInfoContract")]
		public userInfoContract? userInfoContract { get; set; }
	}

	public class LoanInfoContractRequest
	{
		[JsonPropertyName("leaveCreationContract")]
		public userInfoContract? LeaveCreationContract { get; set; }
	}

	public class BackupEmpResponse
	{
		public string? id { get; set; }
		public List<string>? emplId { get; set; }
    }
    public class LeaveTypeModel
	{
		public string? EmpId {  get; set; }
		public string? LeaveType { get; set; }
	}

	public class DDLLeaveType
	{
		[JsonPropertyName("$id")]
		public string? Id { get; set; }

		[JsonPropertyName("leaveType")]
		public string? Type { get; set; }

		[JsonPropertyName("leaveBalance")]
		public JsonElement Balance { get; set; } // Use JsonElement to handle any type
	}

	public class LeaveData
	{
		[JsonPropertyName("$id")]
		public string? Id { get; set; }

		[JsonPropertyName("leaveType")]
		public List<DDLLeaveType>? LeaveTypes { get; set; }
	}

	public class LoanItemsResponse
	{
		[JsonPropertyName("$id")]
		public string? Id { get; set; }

		[JsonPropertyName("loanedItemsResult")]
		public List<LoanItems>? LoanedItemsResult { get; set; }
	}

	public class LoanItems
	{
		[JsonPropertyName("$id")]
		public string? Id { get; set; }
		[JsonPropertyName("loantype")]
		public string? loantype { get; set; }
		[JsonPropertyName("loanItemid")]
		public string? loanItemid {  get; set; }
		[JsonPropertyName("assignedOn")]
		public string? assignedOn { get; set; }
	}


	public class GetNoofDays
	{
		public string? leavestart { get; set; }
		public string? leaveend { get; set; }
		public string? leavetype { get; set; }
		public string? EmpId { get;  set; }
	}
	public class NoOfDaysVM 
	{
		public string? id {get;set;}
		public int? NoofDays { get; set; }
		public string? messageFlag {  get; set; }
		public string? message {  get; set; }
	}

	public class CreateLeaveRequest
	{

		
		public string? CompanyCode { get; set; }
		public string? LeaveStart { get; set; } // Changed to string for parsing
		public string? LeaveEnd { get; set; }   // Changed to string for parsing
		public string? Duedate { get; set; }   // Changed to string for parsing
		public string? UserId { get; set; }
		public string? LeaveType { get; set; }
		public int? NoOfDays { get; set; }      // Changed to int? for numeric value
		public bool? PayLeaveWithSalary { get; set; } // Changed to bool? for clarity
		public string? Comments { get; set; }
		public string? backupEmployee { get; set; }
		public string? RaisedBy { get; set; }
		public string? LeaveNum { get; set; }
		public string? LeaveID { get; set; }
        public List<attachRefList>? attachRefList { get; set; }

    }
	public class attachRefList
	{
		public string? fileName { get; set; }
        public string? attachRef { get; set; }
    }


    public class CreateleaveContractRequest
	{
		[JsonPropertyName("leaveContract")]
		public CreateLeaveRequest? LeaveContract { get; set; }
        //public List<IFormFile>? Files { get; set; } // Uploaded files
    }

	public class userLeaveContract
	{
		public string? companyCode { get; set; }
		public string? UserId { get; set; }
	}

	public class ReadLeaveRequestInfo
	{
		[JsonPropertyName("userLeaveContract")]
		public userLeaveContract? userLeaveContract { get; set; }
	}
	public class ReadleaveInfoRespone
	{

		[JsonPropertyName("$id")]
		public string? Id { get; set; }

		[JsonPropertyName("LeaveDetails")]
		public List<LeaveDetails>? LeaveDetails { get; set; }
	}
	public class ReadleaveResponeById
	{

		[JsonPropertyName("$id")]
		public string? Id { get; set; }

		[JsonPropertyName("leaveDetails")]
		public List<LeaveDetails>? LeaveDetails { get; set; }
	}
	public class LeaveDetails
	{

		[JsonPropertyName("$id")]
		public string? Id { get; set; }

		[JsonPropertyName("leaveID")]
		public string? LeaveID { get; set; }

		[JsonPropertyName("emplId")]
		public string? EmplId { get; set; }

		[JsonPropertyName("leaveStart")]
		public string? LeaveStart { get; set; }

		[JsonPropertyName("leaveEnd")]
		public string? LeaveEnd { get; set; }

		[JsonPropertyName("hideMsg")]
		public string? HideMsg { get; set; }

		[JsonPropertyName("returnDate")]
		public string? ReturnDate { get; set; }
		[JsonConverter(typeof(FlexibleIntConverter))]
		[JsonPropertyName("noOfDays")]
		public int? NoOfDays { get; set; }

		[JsonPropertyName("UserId")]
		public string? UserId { get; set; }

		[JsonPropertyName("userName")]
		public string? UserName { get; set; }

		[JsonPropertyName("leaveType")]
		public string? LeaveType { get; set; }
		[JsonConverter(typeof(FlexibleIntConverter))]

		[JsonPropertyName("leaveBalance")]
		public int LeaveBalance { get; set; }

		[JsonPropertyName("backDatedLeave")]
		public string? BackDatedLeave { get; set; }

		[JsonPropertyName("backupEmployee")]
		public string? BackupEmployee { get; set; }
		[JsonPropertyName("employeeName")]
		public string? employeeName { get; set; }

		[JsonPropertyName("payleavewithSalary")]
		public string? PayLeaveWithSalary { get; set; }

		[JsonPropertyName("status")]
		public string? Status { get; set; }

		[JsonPropertyName("leaveRecId")]
		public long LeaveRecId { get; set; }
		[JsonConverter(typeof(FlexibleIntConverter))]
		[JsonPropertyName("requestrecId")]
		public int RequestRecId { get; set; }

		[JsonPropertyName("workflowPermissionId")]
		public long WorkflowPermissionId { get; set; }
		[JsonConverter(typeof(FlexibleIntConverter))]
		[JsonPropertyName("workflowTableId")]
		public int WorkflowTableId { get; set; }

		[JsonPropertyName("workflowUserid")]
		public string? WorkflowUserid { get; set; }
		[JsonConverter(typeof(FlexibleIntConverter))]
		[JsonPropertyName("requestTableId")]
		public int requestTableId {  get; set; }

		[JsonPropertyName("Msg")]
		public string? Msg { get; set; }
		[JsonPropertyName("comments")]
		public string? comments { get; set; }
		[JsonPropertyName("assignedUserId")]
		public string? assignedUserId { get; set; }

		[JsonPropertyName("msgFlag")]
		public string? MsgFlag { get; set; }

        [JsonPropertyName("attachRefList")]
        public List<AttachRefItem>? AttachRefList { get; set; } // Use JsonElement or create a separate class if it's a known structure
    }
	
	public class FlexibleIntConverter : JsonConverter<int>
	{
		public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.Number)
			{
				if (reader.TryGetInt32(out int intValue))
				{
					return intValue;
				}
				if (reader.TryGetDouble(out double doubleValue))
				{
					return (int)doubleValue; // Truncate floating-point to int
				}
			}
			else if (reader.TokenType == JsonTokenType.String)
			{
				string stringValue = reader.GetString();
				if (string.IsNullOrEmpty(stringValue))
				{
					return 0; // Handle empty string
				}
				if (int.TryParse(stringValue, out int result))
				{
					return result;
				}
				throw new JsonException($"Cannot convert string '{stringValue}' to int at path");
			}
			else if (reader.TokenType == JsonTokenType.Null)
			{
				return 0; // Default to 0 for null
			}
			throw new JsonException($"Unexpected token type {reader.TokenType} for int at path");
		}

		public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
		{
			writer.WriteNumberValue(value);
		}
	}
    public class FlexibleLongConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out long intValue))
                {
                    return intValue;
                }
                if (reader.TryGetDouble(out double doubleValue))
                {
                    return (long)doubleValue; // Truncate floating-point to int
                }
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                string stringValue = reader.GetString();
                if (string.IsNullOrEmpty(stringValue))
                {
                    return 0; // Handle empty string
                }
                if (long.TryParse(stringValue, out long result))
                {
                    return result;
                }
                throw new JsonException($"Cannot convert string '{stringValue}' to long at path");
            }
            else if (reader.TokenType == JsonTokenType.Null)
            {
                return 0; // Default to 0 for null
            }
            throw new JsonException($"Unexpected token type {reader.TokenType} for long at path");
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }


    public class ReadLeaveRequestVm
	{
		public string? companyCode { get; set; }
		public string? UserId { get; set; }
		public string? LeaveId { get; set;}
	}
	public class ReadLeaveResumptionVm
	{
		public string? CompanyCode { get; set; }

		public string? leaveID { get; set; }

		public string? search { get; set; }
	}
	public class ESSReadLeaveRequest
	{
		[JsonPropertyName("leaveResponse")]
		public ReadLeaveRequestVm? ReadLeaveRequest { get; set; }
	}
	public class ESSReadLeaveResumptnMgnrRequest
	{
		[JsonPropertyName("readUpdateLeave")]
		public ReadLeaveResumptionVm? ReadLeaveRequestResVm { get; set; }
	}

	public class readUpdateLeave
	{

		public string? userID { get; set; }

		public string? companyCode { get; set; }

		public string? leaveID { get; set; }




	}
	public class userLeaveResumptionContractRequest
	{
		[JsonPropertyName("readUpdateLeave")]
		public readUpdateLeave? readUpdateLeave { get; set; }
	}
	public class userLeaveResumptionContractResponse
	{

		[JsonPropertyName("$id")]
		public string? Id { get; set; }


		[JsonPropertyName("LeaveDetails")]
		public List<LeaveDetails>? LeaveDetails { get; set; }

	}
	public class userLeaveResumptionContract
	{
		public string? userID { get; set; }

		public string? companyCode { get; set; }
	}
	public class userLeaveResumptionContractInfoRequest
	{
		[JsonPropertyName("userLeaveResumptionContract")]
		public userLeaveResumptionContract? userLeaveResumptionContract { get; set; }
	}
	public class userLeaveResumptionContractInfoResponse
	{

		[JsonPropertyName("$id")]
		public string? Id { get; set; }


		[JsonPropertyName("leaveResumptionDetails")]
		public List<leaveResumptionDetails>? leaveResumptionDetails { get; set; }

	}

	public class leaveResumptionDetails
	{
		[JsonPropertyName("$id")]
		public string? Id { get; set; }

		[JsonPropertyName("leaveID")]
		public string? LeaveID { get; set; }

		[JsonPropertyName("leaveStart")]
		public string? LeaveStart { get; set; }

		[JsonPropertyName("leaveEnd")]
		public string? LeaveEnd { get; set; }

		[JsonPropertyName("leaveType")]
		public string? LeaveType { get; set; }

		[JsonPropertyName("leaveRecId")]
		[JsonConverter(typeof(FlexibleLongConverter))]
		public long LeaveRecId { get; set; }

		[JsonPropertyName("requestTableId")]
		[JsonConverter(typeof(FlexibleIntConverter))]
		public int RequestTableId { get; set; }

		[JsonPropertyName("workflowPermissionId")]
		[JsonConverter(typeof(FlexibleIntConverter))]
		public int WorkflowPermissionId { get; set; }

		[JsonPropertyName("workflowTableId")]
		public int WorkflowTableId { get; set; }

		[JsonPropertyName("workflowUserid")]
		public string? WorkflowUserid { get; set; }
		[JsonPropertyName("Status")]
		public string? Status { get; set; }

		[JsonPropertyName("hideMsg")]
		public string? HideMsg { get; set; }
	}
    public class UpdateLeaveResponse
    {
        [JsonPropertyName("$id")]
        public string? Id { get; set; }

        [JsonPropertyName("LeaveDetail")]
        public List<LeaveDetail>? LeaveDetails { get; set; }
    }

    public class LeaveDetail
    {
        [JsonPropertyName("$id")]
        public string? Id { get; set; }

        [JsonPropertyName("leaveID")]
        public string? LeaveID { get; set; }

        [JsonPropertyName("emplId")]
        public string? EmplId { get; set; }

        [JsonPropertyName("leaveStart")]
        public string? LeaveStart { get; set; }

        [JsonPropertyName("leaveEnd")]
        public string? LeaveEnd { get; set; }

        [JsonPropertyName("hideMsg")]
        public string? HideMsg { get; set; }

        [JsonPropertyName("returnDate")]
        public string? ReturnDate { get; set; }

        [JsonConverter(typeof(FlexibleIntConverter))]
        [JsonPropertyName("noOfDays")]
        public int? NoOfDays { get; set; }

        [JsonPropertyName("UserId")]
        public string? UserId { get; set; }

        [JsonPropertyName("userName")]
        public string? UserName { get; set; }

        [JsonPropertyName("leaveType")]
        public string? LeaveType { get; set; }
        [JsonConverter(typeof(FlexibleIntConverter))]

        [JsonPropertyName("leaveBalance")]
        public int LeaveBalance { get; set; }

        [JsonPropertyName("backDatedLeave")]
        public string? BackDatedLeave { get; set; }

        [JsonPropertyName("backupEmployee")]
        public string? BackupEmployee { get; set; }

        [JsonPropertyName("payleavewithSalary")]
        public string? PayLeaveWithSalary { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("leaveRecId")]
        public long LeaveRecId { get; set; }

        [JsonPropertyName("requestrecId")]
        public long RequestRecId { get; set; }

        [JsonPropertyName("workflowPermissionId")]
        public long WorkflowPermissionId { get; set; }

        [JsonPropertyName("workflowTableId")]
        public int WorkflowTableId { get; set; }

        [JsonPropertyName("workflowUserid")]
        public string? WorkflowUserid { get; set; }

        [JsonPropertyName("Msg")]
        public string? Msg { get; set; }

        [JsonPropertyName("msgFlag")]
        public string? MsgFlag { get; set; }

        [JsonPropertyName("attachRefList")]
        public object? AttachRefList { get; set; }
    }

    public class leaveResponse
	{		
		public string? requestType { get; set; }
		public string? CompanyCode { get; set; }
        public string? UserId { get; set; }
        public string? leaveID { get; set; }
        public string? leaveType { get; set; }
        public string? leaveStart { get; set; }
        public string? leaveEnd { get; set; }
		public string? backDatedLeave { get; set; }
		public string? payleavewithSalary { get; set; }

		public string? NoOfDays { get; set; }
        public string? Comments { get; set; }
    }
    public class leaveResponseRequest
    {
        [JsonPropertyName("leaveResponse")]
        public leaveResponse? leaveResponse { get; set; }
    }

    public class leaveRequestDetails
    {
        public string? CompanyCode { get; set; }
        public string? UserId { get; set; }
        public string? leaveID { get; set; }
    }


    public class DeleteleaveResponseRequest
    {
        [JsonPropertyName("leaveRequestDetails")]
        public leaveRequestDetails? leaveRequestDetails { get; set; }
    }
	public class wfContract
	{
        public string? CompanyId { get; set; }       
        public string? Comments { get; set; }
        public string? workflowTemplateName { get; set; }
        public string? TableId { get; set; }
        public string? RecordId { get; set; }
        public string? raisedUser { get; set; }
        public string? submittingUser { get; set; }
    }

    public class wfContractRequest
    {
        [JsonPropertyName("wfContract")]
        public wfContract? wfContract { get; set; }
    }

	public class wfContractResponse
	{
        [JsonPropertyName("$id")]
        public string? Id { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }

        [JsonPropertyName("messageFlag")]
        public string? MessageFlag { get; set; }
    }
	public class UpdateLeaveResumptionResponse
	{
		[JsonPropertyName("$id")]
		public string? Id { get; set; }

		[JsonPropertyName("leaveDetails")]
		public List<LeaveDetail>? LeaveDetails { get; set; }
		[JsonPropertyName("leaveMsgDetails")]
		public List<LeaveResponseMsg>? LeaveResponseMsg { get; set; }
	}
	public class LeaveResponseMsg
	{
		[JsonPropertyName("$id")]
		public string? Id { get; set; }
		[JsonPropertyName("messageFlag")]
		public string? MessageFlag { get; set; }
		[JsonPropertyName("message")]
		public string? message { get; set; }
	}
	public class _contract
	{
		public string? RecordId { get; set; }
		public string? TableId { get; set; }
		public string? CompanyId { get; set; }
		public string? Comments { get; set; }
		public string? SpecialApproval { get; set; }
		public string? RaisedBy { get; set; }
		public string? EmpId { get; set; }
		public string? raisedUser { get; set; }
	}
	public class DelegatewfRequest
	{
		[JsonPropertyName("_contract")]
		public _contract? _contract { get; set; }
	}
	public class RejectwfRequest
	{
		[JsonPropertyName("_contract")]
		public _contract? _contract { get; set; }
	}

	public class ApprovewfRequest
	{
		[JsonPropertyName("_contract")]
		public _contract? _contract { get; set; }
	}
	public class LeaveResumptionDetailsResponse
	{
		[JsonPropertyName("$id")]
		public string? Id { get; set; }
		[JsonPropertyName("leaveDetails")]
		public List<LeaveDetails>? leaveResumptionDetails { get; set; }
	}
	public class ReadUpdateLeaveResumption
	{
		[JsonPropertyName("requestType")]
		public string? RequestType { set; get; }
		[JsonPropertyName("CompanyCode")]
		public string? CompanyCode { get; set; }
		[JsonPropertyName("leaveID")]
		public string? leaveID { get; set; }
		[JsonPropertyName("returnDate")]
		public string? ReturnDate { get; set; }
		[JsonPropertyName("wfComments")]
		public string? WfComments { get; set; }

		public List<AttachRef>? attachRefList { get; set; }
	}

	public class ReadUpdateLeaveResumptionResponse
	{
		[JsonPropertyName("readUpdateLeave")]
		public ReadUpdateLeaveResumption? readUpdateLeave { get; set; }
	}
    public class AttachRef
    {
        [JsonPropertyName("fileName")]
        public string? fileName { get; set; }

        [JsonPropertyName("attachRef")]
        public string? attachRef { get; set; }
    }

    public class UpdAttachFile
    {
        [JsonPropertyName("companyCode")]
        public string? companyCode { get; set; }

        [JsonPropertyName("uniqueRecID")]
        public long uniqueRecID { get; set; }

        [JsonPropertyName("requestTableID")]
        public int requestTableID { get; set; }

        [JsonPropertyName("attachRefList")]
        public List<AttachRef>? attachRefList { get; set; }
    }

    public class RootObject
    {
        [JsonPropertyName("updAttachFile")]
        public UpdAttachFile? updAttachFile { get; set; }
    }
    public class FileUploadViewModel
    {
        [JsonPropertyName("uniqueRecID")]
        public long uniqueRecID { get; set; }

        [JsonPropertyName("requestTableID")]
        public int requestTableID { get; set; }
        public List<IFormFile>? Files { get; set; }
    }


    public class FileResponce
    {
        [JsonPropertyName("attachRefList")]
        public List<AttachRefItem>? AttachRefList { get; set; }
    }

    public class AttachRefItem
    {
        [JsonPropertyName("fileName")]
        public string? FileName { get; set; }

        [JsonPropertyName("attachRef")]
        public string? AttachRef { get; set; }

        [JsonPropertyName("attachRecID")]
        public long AttachRecID { get; set; }

        [JsonPropertyName("attachedBy")]
        public string? AttachedBy { get; set; }

        [JsonPropertyName("attachedDate")]
        public string? DateOfUpload { get; set; }
    }
}
