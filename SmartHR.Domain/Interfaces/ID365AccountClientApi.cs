using SmartHR.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHR.Domain.Interfaces
{
	public interface ID365AccountClientApi
	{
        Task<string> GetAccessTokenAsync();
        bool TryParseDate(string dateStr, out string result);
        bool TryParseDateToString(string? dateStr, out DateTime result);
    }
}
