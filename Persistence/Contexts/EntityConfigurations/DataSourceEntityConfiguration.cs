using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Contexts.EntityConfigurations
{
    public class DataSourceEntityConfiguration : IEntityTypeConfiguration<DataSource>
    {
        public void Configure(EntityTypeBuilder<DataSource> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.DataSourceId);
            builder.ToTable("DataSources");
            #endregion

            #region Property configurations 
            builder.Property(c => c.SourceType).IsRequired().HasMaxLength(100);
            #endregion

            #region relationships
            #endregion
        }
    }
}
