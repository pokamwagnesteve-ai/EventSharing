using System.ComponentModel.DataAnnotations;

namespace EventSharing.Models
{
    /// <summary>Catégorie permettant de classer les événements.</summary>
    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(80)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
