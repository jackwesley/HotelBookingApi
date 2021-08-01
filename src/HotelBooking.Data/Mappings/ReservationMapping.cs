﻿using HotelBooking.Domain.Models;
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

            builder.HasOne(x => x.StayTime)
                .WithMany(y => y.Reservations)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Room);

            builder.Ignore(x => x.ValidationResult);

            builder.ToTable("Reservations");
        }
    }
}
