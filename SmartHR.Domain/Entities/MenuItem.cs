using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartHR.Domain.Entities
{
    public class MenuItem
    {
        [JsonPropertyName("name")] 
        public string? Name { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("children")]
        public List<MenuItem> Children { get; set; } = new List<MenuItem>();
    }

    public class D365ApiResponse
    {
        [JsonPropertyName("value")]
        public List<MenuItem> Value { get; set; }= new List<MenuItem>();
    }

}