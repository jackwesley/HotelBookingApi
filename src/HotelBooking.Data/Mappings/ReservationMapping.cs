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

            builder.Property(r => r.CheckIn)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(r => r.CheckOut)
                .IsRequired()
                .HasColumnType("datetime");


            builder.HasOne(x => x.Guest)
                .WithOne(y => y.Reservation);

            builder.HasOne(x => x.Room);

            builder.Ignore(x => x.ValidationResult);


            builder.ToTable("Reservations");
        }
    }
}
