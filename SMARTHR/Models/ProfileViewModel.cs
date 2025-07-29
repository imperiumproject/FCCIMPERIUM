using System.Text.Json.Serialization;

namespace SMARTHR.WEB.Models
{
    public class ProfileViewModel
    {
        public string? UserId { get; set; }
        public string? CompanyId { get; set; }
    }

    public class EssViewProfileRequest
    {
        [JsonPropertyName("emplProfileContract")] // Changed to match JSON
        public ProfileViewModel? EssProfileContract { get; set; }
    }
    public class MyProfileViewModel
    {
		[JsonPropertyName("id")]
		public string? Id { get; set; } // Keep as is, matches "$id"
		[JsonPropertyName("EmployeeId")]
		public string? EmployeeId { get; set; }
		[JsonPropertyName("EmployeeName")]
		public string? EmployeeName { get; set; }
		[JsonPropertyName("Title")]
		public string? Title { get; set; }
		[JsonPropertyName("JoiningDate")]
		public string? JoiningDate { get; set; }
		[JsonPropertyName("CivilId")]
		public string? CivilId { get; set; }
		[JsonPropertyName("Phone")]
		public string? Phone { get; set; }
		[JsonPropertyName("Email")]
		public string? Email { get; set; }
		[JsonPropertyName("CompanyId")]
		public string? CompanyId { get; set; }
		[JsonPropertyName("EmployeeImage")]
		public string? EmployeeImage { get; set; }
		[JsonPropertyName("Gender")]
		public string? Gender {  get; set; }
		[JsonPropertyName("BirthDate")]
		public string? BirthDate { get; set; }
		[JsonPropertyName("NationalityCountryRegion")]		
		public string? NationalityCountryRegion { get; set; }
		[JsonPropertyName("MaritalStatus")]		
		public string? MaritalStatus { get; set; }
		[JsonPropertyName("PersonalContactRelationship")]
		public string? PersonalContactRelationship { get; set; }
		[JsonPropertyName("PrimarySecondary")]
		public string? PrimarySecondary { get; set; }
		[JsonPropertyName("AccountId")]
		public string? AccountId { get; set; }
		[JsonPropertyName("AccountNum")]	
		public string? AccountNum { get; set; }
		[JsonPropertyName("bankAccountType")]
		public string? bankAccountType { get; set; }
		[JsonPropertyName("AccountHolder")]
		public string? AccountHolder { get; set; }
		[JsonPropertyName("BranchName")]
		public string? BranchName { get; set; }
		[JsonPropertyName("BankLocationCode")]
		public string? BankLocationCode { get; set; }
		[JsonPropertyName("bankIBAN")]
		public string? bankIBAN { get; set; }
		[JsonPropertyName("Education")]
		public string? Education { get; set; }
		[JsonPropertyName("NumberOfDependents")]
		public string? NumberOfDependents { get; set; }
		[JsonPropertyName("passportNumber")]
		public string? PassportNo {  get; set; }
		[JsonPropertyName("[JsonPropertyName(\"passportNumber\")]")]
		public string? passportExpiryDate { get; set; }
		[JsonPropertyName("bankName")]
		public string? bankName { get; set; }

	}
}
