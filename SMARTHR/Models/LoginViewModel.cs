using System.Text.Json.Serialization;

namespace SMARTHR.WEB.Models
{
    public class LoginViewModel
    {
		public string? Username { get; set; }
		public string? Password { get; set; }
		public string? CompanyCode { get; set; }
		public bool RememberMe { get; set; }

	}
	public class LoginResponse
	{
		[JsonPropertyName("$id")]
		public string? Id { get; set; }

		[JsonPropertyName("message")]
		public string? Message { get; set; }

		[JsonPropertyName("errorMessage")]
		public string? ErrorMessage { get; set; }

		[JsonPropertyName("messageFlag")]
		public string? MessageFlag { get; set; }

        [JsonPropertyName("userName")]
        public string? username { get; set; }
        [JsonPropertyName("UserId")]
        public string? UserId { get; set; }
        [JsonPropertyName("Password")]
        public string? Password { get; set; }

        [JsonIgnore]
		public bool Success => Message?.ToLower() == "yes";
	}

	public class EssLoginContract
	{
		[JsonPropertyName("userID")]
		public string? UserID { get; set; }

		[JsonPropertyName("Password")]
		public string? Password { get; set; }

		[JsonPropertyName("CompanyCode")]
		public string? CompanyCode { get; set; }
	}
	public class EssLoginRequest
	{
		[JsonPropertyName("essLoginContract")]
		public EssLoginContract? EssLoginContract { get; set; }
	}
}
