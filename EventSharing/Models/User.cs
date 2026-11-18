using Microsoft.AspNetCore.Identity;

namespace EventSharing.Models
{
    /// <summary>Utilisateur authentifié pouvant créer et rejoindre des événements.</summary>
    public class User : IdentityUser
    {
        public string? Name { get; set; }
        public ICollection<Event> JoinedEvents { get; set; } = new List<Event>();
        public ICollection<Event> CreatedEvents { get; set; } = new List<Event>();


    }
}
