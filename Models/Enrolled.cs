using QtechBackend.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QtechBackendv2.Models
{
    public class Enrolled
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [ForeignKey("Playlist")]
        public int PlaylistId { get; set; }

        [Required]
        public bool EnrollStatus { get; set; }



    }
}
