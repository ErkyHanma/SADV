using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Contexts.EntityConfigurations
{
    public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            #region Basic configuration
            builder.HasKey(p => p.ProductId);
            builder.ToTable("Products");
            #endregion

            #region Property configurations 
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Category).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Price).HasColumnType("decimal(10,2)");
            #endregion

            #region relationships
            builder.HasMany<SaleDetails>(p => p.SaleDetails)
                .WithOne(sd => sd.Product)
                .HasForeignKey(sd => sd.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

        }

    }
}

