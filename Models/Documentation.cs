using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QtechBackend.Models
{
    public class Documentation
    {
        [Key]
        public int DocId { get; set; }

        [Required]
        [StringLength(100)]
        public string FileName { get; set; } 

        [Required]
        public string Content { get; set; } 

        [Required]
        public byte[] FileContent { get; set; }

        [ForeignKey("Playlist")]
        public int PlaylistId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; } 
    }
}