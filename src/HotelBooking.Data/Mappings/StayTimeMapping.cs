using HotelBooking.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Data.Mappings
{
    public class StayTimeMapping : IEntityTypeConfiguration<StayTime>
    {
        public void Configure(EntityTypeBuilder<StayTime> builder)
        {
            builder.Property(r => r.CheckIn)
               .IsRequired()
               .HasColumnType("datetime");

            builder.Property(r => r.CheckOut)
                    .IsRequired()
                    .HasColumnType("datetime");

            builder.Ignore(r => r.DaysToStay);

            builder.Ignore(x => x.ValidationResult);

            builder.ToTable("StayTime");
        }
    }
}
