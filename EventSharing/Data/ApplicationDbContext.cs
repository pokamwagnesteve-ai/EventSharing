using EventSharing.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventSharing.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options) { }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(user => user.Name)
                .HasMaxLength(120);

            modelBuilder.Entity<Event>()
                .HasOne(eventItem => eventItem.Category)
                .WithMany(category => category.Events)
                .HasForeignKey(eventItem => eventItem.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .HasOne(eventItem => eventItem.Creator)
                .WithMany(user => user.CreatedEvents)
                .HasForeignKey(eventItem => eventItem.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .HasMany(eventItem => eventItem.Participants)
                .WithMany(user => user.JoinedEvents)
                .UsingEntity<Dictionary<string, object>>(
                    "EventParticipants",
                    join => join
                        .HasOne<User>()
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join => join
                        .HasOne<Event>()
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.HasKey("EventId", "UserId");
                        join.ToTable("EventParticipants");
                    });
        }
    }
}
