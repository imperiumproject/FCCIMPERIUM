using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHR.Application.Interfaces
{
    public interface IAuthentications
    {
        Task<string> GetAccessTokenAsync();
        bool TryParseDate(string dateStr, out string result);
        bool TryParseDateToString(string? dateStr, out DateTime result);
    }
}
