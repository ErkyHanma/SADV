using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Contexts.EntityConfigurations
{
    public class SaleDetailsEntityConfiguration : IEntityTypeConfiguration<SaleDetails>
    {
        public void Configure(EntityTypeBuilder<SaleDetails> builder)
        {
            #region Basic configuration
            builder.HasKey(sd => sd.SaleDetailId);
            builder.ToTable("SaleDetails");
            #endregion

            #region Property configurations 
            builder.Property(p => p.Total).HasColumnType("decimal(10,2)");
            #endregion

            #region relationships
            #endregion
        }
    }
}
