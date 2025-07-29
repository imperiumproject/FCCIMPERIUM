using System.Text.Json.Serialization;

namespace SMARTHR.WEB.Models
{
	public class TokenResponse
	{
		[JsonPropertyName("access_token")]
		public string? AccessToken { get; set; }
	}
}
