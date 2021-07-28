using HotelBooking.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Data.Mappings
{
    public class GuestMapping : IEntityTypeConfiguration<Guest>
    {
        public void Configure(EntityTypeBuilder<Guest> builder)
        {
            builder.HasKey(g => g.Id);

            builder.Property(g => g.Name)
                .IsRequired()
                .HasColumnType("varchar(250)");

            builder.Property(c => c.Email)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.Property(c => c.Document)
                .IsRequired()
                .HasColumnType("varchar(15)");

            builder.Ignore(x => x.ValidationResult);

            builder.Property(c => c.Phone)
                .IsRequired()
                .HasColumnType("varchar(15)");
        }
    }
}
