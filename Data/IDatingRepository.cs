using System.Collections.Generic;
using System.Threading.Tasks;
using datingapp.api.Helpers;
using datingapp.api.Models;

namespace datingapp.api.Data
{
    public interface IDatingRepository
    {
        void Add<T>(T item) where T : class;
        void Delete<T>(T item) where T: class;
        Task<bool> SaveAll();
        Task<PageList<User>> GetUsers(PaginationParams p);
        Task<User> GetUser(int id);
        Task<Photo> GetPhoto(int id);
        Photo FindMainPhoto(User user);
        Task<Like> GetLike(int likerId, int likeeId);
        Task<Message> GetMessage(int id);
        Task<IEnumerable<Message>> GetMessageThread(int userId, int reciverId);
        Task<PageList<Message>> GetMessageForUser(MessageParams msgParams);
    }
}