using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PAS.Domain.Funds;

namespace PAS.Infrastructure.Persistence.Configurations;

public sealed class FundConfiguration : IEntityTypeConfiguration<Fund> {
    public void Configure(EntityTypeBuilder<Fund> builder) {
        builder.ToTable("Funds");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasConversion(
                id => id.Value,
                value => new FundId(value))
            .ValueGeneratedNever();

        builder.Property(f => f.Name)
            .HasMaxLength(200)
            .IsRequired();


        builder.Property(f => f.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();


        builder.OwnsOne(
            f => f.Isin,
            isin => {
                isin.Property(x => x.Value)
                    .HasColumnName("Isin")
                    .HasMaxLength(12)
                    .IsRequired();
            });


        builder.OwnsOne(
            f => f.Currency,
            currency => {
                currency.Property(x => x.Code)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });


        builder.OwnsMany(
            f => f.Navs,
            nav => {
                nav.ToTable("FundNavs");

                nav.WithOwner()
                    .HasForeignKey("FundId");

                nav.Property<Guid>("Id");

                nav.HasKey("Id");

                nav.Property(n => n.Date)
                    .IsRequired();

                nav.Property(n => n.Value)
                    .HasPrecision(18, 6)
                    .IsRequired();
            });
    }
}