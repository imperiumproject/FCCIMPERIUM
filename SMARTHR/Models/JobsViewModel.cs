using System.Text.Json.Serialization;

namespace SMARTHR.WEB.Models
{
    public class JobsViewModel
    {
    }


    public class jobVacancyContract
    {
        public string? companyId { get; set; }
        public string? employeeId { get; set; }
        public string? jobDescription { get; set; }
    }

    public class jobVacancyContractRequest
    {
        [JsonPropertyName("jobVacancyContract")]
        public jobVacancyContract? jobVacancyContract { get; set; }
    }

    public class JobVacancyResponse
    {
        [JsonPropertyName("$id")]
        public string? Id { get; set; }

        [JsonPropertyName("jobVacancyList")]
        public List<JobVacancy>? JobVacancyList { get; set; }
    }

    public class JobVacancy
    {
        [JsonPropertyName("$id")]
        public string? Id { get; set; }

        [JsonPropertyName("openJobs")]
        public string? OpenJobs { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("department")]
        public string? Department { get; set; }

        [JsonPropertyName("hiringManager")]
        public string? HiringManager { get; set; }

        [JsonPropertyName("finalDate")]
        public string? FinalDate { get; set; }

        [JsonPropertyName("dataAreaId")]
        public string? DataAreaId { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("certificateList")]
        public List<Certificate>? CertificateList { get; set; }

        [JsonPropertyName("educationList")]
        public List<Education>? EducationList { get; set; }

        [JsonPropertyName("skillList")]
        public List<Skill>? SkillList { get; set; }
    }

    public class Certificate
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("importance")]
        public string? Importance { get; set; }
    }

    public class Education
    {
        [JsonPropertyName("education")]
        public string? EducationName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("importance")]
        public string? Importance { get; set; }
    }

    public class Skill
    {
        [JsonPropertyName("skill")]
        public string? SkillName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("ratingLevel")]
        public string? RatingLevel { get; set; }

        [JsonPropertyName("importance")]
        public string? Importance { get; set; }
    }

    public class JobApplicationResponse
    {
        [JsonPropertyName("$id")]
        public string? Id { get; set; }

        [JsonPropertyName("jobVacancyList")]
        public JobVacancy? JobVacancyList { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("success")]
        public string? Success { get; set; }
    }



}
