using System.Text.Json.Serialization;

namespace SMARTHR.WEB.Models
{
    public class ForgetPassword
    {
        public string? UserName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? CompanyCode { get; set; }
    }
    public class ForgetPasswordResponse
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

    public class serviceContract
    {
        [JsonPropertyName("userID")]
        public string? UserID { get; set; }

        [JsonPropertyName("Password")]
        public string? Password { get; set; }

        [JsonPropertyName("CompanyCode")]
        public string? CompanyCode { get; set; }
    }
    public class serviceContractRequest
    {
        [JsonPropertyName("serviceContract")]
        public serviceContract? serviceContract { get; set; }
    }
    public class ChangePasswordVM
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? NewPassword { get; set; }
        public string? CompanyCode { get; set; }
    }

    public class ChangePasswordResponse
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

    public class ChangePasswordContract
    {
        [JsonPropertyName("userID")]
        public string? UserID { get; set; }

        [JsonPropertyName("Password")]
        public string? Password { get; set; }
        [JsonPropertyName("ConfirmPassword")]
        public string? ConfirmPassword { get; set; }

        [JsonPropertyName("CompanyCode")]
        public string? CompanyCode { get; set; }
    }
    public class ChangePasswordRequest
    {
        [JsonPropertyName("ChangePasswordContract")]
        public ChangePasswordContract? ChangePasswordContract { get; set; }
    }

}
