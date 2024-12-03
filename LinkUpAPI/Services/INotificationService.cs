using System.Threading.Tasks;

namespace LinkUpAPI.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(int userId, string message);
    }
}