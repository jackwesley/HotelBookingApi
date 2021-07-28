using HotelBooking.Core.DomainObjects;
using System;

namespace HotelBooking.Core.Data
{
    public interface IRepository<T> : IDisposable where T : Entity
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
