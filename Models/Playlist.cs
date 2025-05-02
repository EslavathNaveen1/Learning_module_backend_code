using System;
using System.ComponentModel.DataAnnotations;

namespace QtechBackend.Models
{
    public class Playlist
    {
        [Key]
        public int PlaylistId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }


        public string ImageUrl { get; set; } 

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}