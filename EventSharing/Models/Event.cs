using System.ComponentModel.DataAnnotations;

namespace EventSharing.Models
{
    /// <summary>Événement publié dans l'application.</summary>
    public class Event
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(4000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        [Required]
        public string CreatorId { get; set; } = string.Empty;
        public User Creator { get; set; } = null!;

        public ICollection<User> Participants { get; set; } = new List<User>();

    }
}
