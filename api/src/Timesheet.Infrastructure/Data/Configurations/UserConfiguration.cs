using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Timesheet.Domain.Entities;

namespace Timesheet.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity configuration for User entity using Fluent API.
    /// 
    /// WHY USE IEntityTypeConfiguration?
    /// - Separates configuration from DbContext (Single Responsibility Principle)
    /// - Each entity has its own configuration class
    /// - Easier to maintain and test
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Table name (optional - by default uses DbSet name)
            builder.ToTable("Users");

            // Primary Key
            builder.HasKey(u => u.Id);

            // Properties
            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(u => u.Role)
                .IsRequired();

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);

            // Indexes
            builder.HasIndex(u => u.Email)
                .IsUnique(); // Email must be unique

            // Navigation properties are configured in the related entity configurations
        }
    }
}
