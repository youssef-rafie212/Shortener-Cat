using Core.Domain.Entities;

namespace Core.DTO.Analytics_DTOs
{
    public class VisitsByCountryDto
    {
        public string Country { get; set; }
        public int Count { get; set; }
        public List<UrlVisit> Visits { get; set; }

    }
}
