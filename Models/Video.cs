using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QtechBackend.Models
{
    public class Video
    {

        [Key]
        public int? VideoId { get; set; }

        [ForeignKey("Playlist")]
        public int PlaylistId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [Url]
        public string Url { get; set; }
     
        public string? ImageUrl { get; set; } 

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

      


    }
}
