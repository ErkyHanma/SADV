using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Contexts.EntityConfigurations
{
    public class SaleEntityConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {

            #region Basic configuration
            builder.HasKey(s => s.SaleId);
            builder.ToTable("Sales");
            #endregion

            #region Property configurations 
            #endregion

            #region relationships
            builder.HasOne(s => s.SaleDetails)
                    .WithOne(sd => sd.Sale)
                    .HasForeignKey<SaleDetails>(sd => sd.SaleId)
                    .OnDelete(DeleteBehavior.Cascade);


            #endregion
        }
    }
}
