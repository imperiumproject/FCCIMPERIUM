using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Collections;
namespace SmartHR.Application.DTOs
{
    public class MenuItemDTOs
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public List<MenuItemDTOs> Children { get; set; } = new List<MenuItemDTOs>();
    }
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
    }       
}

