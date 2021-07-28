using System.Threading.Tasks;

namespace HotelBooking.Core.Data
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
    }
}
