using HotelBooking.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Data.Mappings
{
    public class ReservationMapping : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasOne(x => x.Guest)
                .WithMany(y => y.Reservations);

            builder.OwnsOne(
                x => x.StayTime,
                stayTime =>
            {
                stayTime.Property(s => s.CheckIn)
                    .IsRequired()
                    .HasColumnName("CheckIn")
                    .HasColumnType("datetime");

                stayTime.Property(s => s.CheckOut)
                    .IsRequired()
                    .HasColumnName("CheckOut")
                    .HasColumnType("datetime");

                stayTime.Ignore(s => s.ValidationResult);
                stayTime.Ignore(s => s.DaysToStay);
            });

            builder.HasOne(x => x.Room);

            builder.Ignore(x => x.ValidationResult);

            builder.ToTable("Reservations");
        }
    }
}
