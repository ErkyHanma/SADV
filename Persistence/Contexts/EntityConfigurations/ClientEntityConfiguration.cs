using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Contexts.EntityConfigurations
{
    public class ClientEntityConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("Clients");
            #endregion

            #region Property configurations 
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Email).IsRequired().HasMaxLength(100);
            builder.Property(c => c.PhoneNumber).HasMaxLength(50);
            builder.Property(c => c.City).HasMaxLength(70);
            builder.Property(c => c.Country).HasMaxLength(70);
            #endregion

            #region indexes
            builder.HasIndex(c => c.Email).IsUnique();
            builder.HasIndex(c => c.PhoneNumber).IsUnique();
            #endregion

            #region relationships
            builder.HasMany<Sale>(u => u.Sales)
                    .WithOne(s => s.Client)
                    .HasForeignKey(s => s.ClientId)
                    .OnDelete(DeleteBehavior.Cascade);
            #endregion

        }



    }
}

