using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace QtechBackend.Models
{
    public class Users
    {


        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Unicode]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Role { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        public DateTime DateJoined { get; set; }
    }
}
