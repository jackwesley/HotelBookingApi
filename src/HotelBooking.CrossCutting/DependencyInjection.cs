using HotelBooking.Application.Services;
using HotelBooking.Application.Services.Interfaces;
using HotelBooking.Data.Repositories;
using HotelBooking.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.CrossCutting
{
    public static class DependencyInjection
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IGuestRepository, GuestRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IGuestService, GuestService>();
            services.AddScoped<IReservationService, ReservationService>();
        }
    }
}
