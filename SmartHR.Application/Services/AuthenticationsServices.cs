using SmartHR.Application.Interfaces;
using SmartHR.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHR.Application.Services
{
    public class AuthenticationsServices : IAuthentications
    {
        private readonly ID365AccountClientApi _d365ApiClient;

        public AuthenticationsServices(ID365AccountClientApi d365ApiClient)
        {
            _d365ApiClient = d365ApiClient;
        }
        public async Task<string> GetAccessTokenAsync()
        {
            var token = await _d365ApiClient.GetAccessTokenAsync();
            return token;
        }
        public bool TryParseDate(string dateStr, out string result)
        {
            var result1 = _d365ApiClient.TryParseDate(dateStr, out result);
            return result1;
        }

        public bool TryParseDateToString(string? dateStr, out DateTime result)
        {
            var result1 = _d365ApiClient.TryParseDateToString(dateStr, out result);
            return result1;
        }
    }
}
