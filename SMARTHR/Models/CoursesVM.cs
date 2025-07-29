using System.Text.Json.Serialization;

namespace SMARTHR.WEB.Models
{
    public class CoursesVM
    {
    }


    public class Listcontract
    {
        public string? companyId { get; set; }

        public string? description {get;set;}
        public string? employeeId { get; set; }
    }

    public class ListcontractRequest
    {
        [JsonPropertyName("contract")]

        public Listcontract? Listcontract { get; set; }
    }
}
