using System.Text.Json;

namespace SMARTHR.WEB.FCCHRServices.FCCHRBusinessInterface
{
	public interface IAuthentications
	{
		Task<string> GetAccessTokenAsync();
		bool TryParseDate(string dateStr, out string result);
		bool TryParseDateToString(string? dateStr, out DateTime result);
		JsonSerializerOptions GetJsonOptions();
	}
}
