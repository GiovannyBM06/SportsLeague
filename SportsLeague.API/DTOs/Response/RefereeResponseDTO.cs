namespace SportsLeague.API.DTOs.Response
{
    public class RefereeResponseDTO
    {
        public int Id { get; set; }
        public String FirtsName { get; set; } = string.Empty;
        public String LastName { get; set; } = string.Empty;
        public String Nationality { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
