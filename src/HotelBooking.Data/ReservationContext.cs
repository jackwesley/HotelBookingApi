using HotelBooking.Core.Data;
using HotelBooking.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBooking.Data
{
    public class ReservationContext : DbContext, IUnitOfWork
    {
        public ReservationContext(DbContextOptions<ReservationContext> options)
           : base(options)
        {

        }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<StayTime> StayTime { get; set; }
        public DbSet<Guest> Guests { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
               e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationContext).Assembly);
        }


        public async Task<bool> CommitAsync()
        {
            return await base.SaveChangesAsync() > 0;
        }
    }
}
