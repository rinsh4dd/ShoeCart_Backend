using System.Threading.Tasks;

namespace ShoeCartBackend.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task BlockUnblockUserAsync(int id);
        Task SoftDeleteUserAsync(int id);
    }
}
