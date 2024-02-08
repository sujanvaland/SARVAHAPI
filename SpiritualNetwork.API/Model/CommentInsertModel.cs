using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.API.Model
{
    public class CommentInsertModel
    {
        public int UserId { get; set; }
        [MaxLength(2000)]
        public string PostMessage { get; set; }
        [MaxLength(10)]
        public string Type { get; set; }
        public int? ParentId { get; set; }
        public string? Category { get; set; }
        public int? PostId { get; set; }
    }
}
