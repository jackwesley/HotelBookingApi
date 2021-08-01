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

            builder.Property(r => r.CheckIn)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(r => r.CheckOut)
                .IsRequired()
                .HasColumnType("datetime");


            builder.HasOne(x => x.Guest)
                .WithMany(y => y.Reservations);

            builder.HasOne(x => x.Room);

            builder.Ignore(x => x.ValidationResult);
            builder.Ignore(x => x.DaysToStay);


            builder.ToTable("Reservations");
        }
    }
}
