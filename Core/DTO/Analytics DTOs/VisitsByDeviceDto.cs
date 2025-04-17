using Core.Domain.Entities;

namespace Core.DTO.Analytics_DTOs
{
    public class VisitsByDeviceDto
    {
        public string Device { get; set; }
        public int Count { get; set; }
        public List<UrlVisit> Visits { get; set; }

    }
}
