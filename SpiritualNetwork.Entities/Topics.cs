using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.Entities
{
    public class Topics : BaseEntity
    {
        [MaxLength(200)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public string Icon { get; set; }
    }
}
