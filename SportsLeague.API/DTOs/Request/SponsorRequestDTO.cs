using SportsLeague.Domain.Enums;

namespace SportsLeague.API.DTOs.Request
{
    public class SponsorRequestDTO
    {
        public String Name { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? WebsiteUrl { get; set; } = string.Empty;
        public SponsorCategory Category { get; set; }
    }
}
