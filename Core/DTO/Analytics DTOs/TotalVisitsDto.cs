using Core.Domain.Entities;

namespace Core.DTO.Analytics_DTOs
{
    public class TotalVisitsDto
    {
        public int Count { get; set; }
        public List<UrlVisit> Visits { get; set; }
    }
}
