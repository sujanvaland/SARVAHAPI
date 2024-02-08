using SpiritualNetwork.Entities;

namespace SpiritualNetwork.API.Model
{
    public class EventModel: BaseEntity
    {
        public string? EventFormat { get; set; }

        public int? EventTypeId { get; set; }
        public string? EventTitle { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? StartTime { get; set; }

        public string? EndTime { get; set; }

        public string Description { get; set; }

        public string EventLink { get; set; }

        public string Eventaddress { get; set; }

        public string EventCoverImage { get; set; }
    }
}
