using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingapp.api.Helpers;
using datingapp.api.Models;
using Microsoft.EntityFrameworkCore;

namespace datingapp.api.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext context;
        public DatingRepository(DataContext context)
        {
            this.context = context;
        }
        public void Add<T>(T item) where T : class // niazi nis async bashe chon fqt daram add mikonm va amaln kari ba db nadaram
        {
            context.Add(item);
        }

        public void Delete<T>(T item) where T : class
        {
            context.Remove(item);
        }

        public Photo FindMainPhoto(User user)
        {
            return user.Photos.FirstOrDefault(e => e.IsMain);
        }

        public async Task<Like> GetLike(int likerId, int likeeId)
        {
            var like = await this.context.Likes.FirstOrDefaultAsync(e => e.LikeeId == likeeId && e.LikerId == likerId);
            return like;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await context.Photos.FirstOrDefaultAsync(e => e.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            return await context.Users
                .Include(e => e.Photos)//foeign key
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<PageList<User>> GetUsers(PaginationParams p)
        {
            var users = context.Users.Include(e => e.Photos).AsQueryable();
            // ignore the current user
            users = users.Where(e => e.Id != p.UserId);
            // select the gender
            if(!(p.showlikees || p.showlikers)){

            users = users.Where(e => e.Gender == p.Gender);
            // get datetime for minage and maxage
            var minDate = DateTime.Now.AddYears(-p.MaxAge);
            var maxDate = DateTime.Now.AddYears(-p.MinAge);
            // add date criteria
            users = users.Where(e => e.DateOfBirth < maxDate && e.DateOfBirth > minDate);
            // ordering users whether by lastactive or createddate
            if (p.Order == "create")
            {
                users = users.OrderByDescending(e => e.Created);
            }
            else
            {
                users = users.OrderByDescending(e => e.LastActive);
            }
            }
            // show likers or likees if necessary
            if (p.showlikers)
            {
                // persons who liked me
                // users = users.Include(e=> e.Likers);
                var findCurrentUser = await this.context.Users.Include(e=>e.Likers).FirstOrDefaultAsync(e => e.Id == p.UserId);
                var ids = giveids(true, findCurrentUser);
                users = users.Where(e => ids.Contains(e.Id));
            }
            if (p.showlikees)
            {
                // persons who liked me
                // users = users.Include(e=> e.Likees);
                var findCurrentUser = await this.context.Users.Include(e=>e.Likees).FirstOrDefaultAsync(e => e.Id == p.UserId);
                var ids = giveids(false, findCurrentUser);
                users = users.Where(e => ids.Contains(e.Id));
            }

            var pagelist = await PageList<User>.CreateAsync(users, p.PageNumber, p.pageSize);
            return pagelist;
        }

        private IEnumerable<int> giveids(bool showlikers, User findCurrentUser)
        {
            if (showlikers)
            {
                var ids = findCurrentUser.Likers.Select(e => e.LikerId);
                return ids;
            }
            else
            {
                var ids = findCurrentUser.Likees.Select(e => e.LikeeId);
                return ids;
            }
        }

        public async Task<bool> SaveAll()
        {
            return await context.SaveChangesAsync() > 0;// mikham bbinm taqiri dade shode ya na
        }

        public async Task<Message> GetMessage(int id)
        {
            return await this.context.Messages.FirstOrDefaultAsync(e=>e.Id == id);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int reciverId)
        {
            var messages = await this.context.Messages
            .Include(e=>e.Sender).ThenInclude(e=>e.Photos)
            .Include(e=>e.Reciver).ThenInclude(e=>e.Photos)
            .Where(e=>e.SenderId == reciverId && e.ReciverId == userId && !e.ReciverDeleted
            || e.SenderId == userId && e.ReciverId == reciverId && !e.SenderDeleted)
            .OrderByDescending(e=>e.DateSent)
            .ToListAsync();

            return messages;
        }

        public async Task<PageList<Message>> GetMessageForUser(MessageParams msgParams)
        {
            var msges = this.context.Messages
            .Include(e=>e.Sender).ThenInclude(e=>e.Photos)
            .Include(e=>e.Reciver).ThenInclude(e=>e.Photos).AsQueryable();

            switch(msgParams.MessageContainer)
            {
                case "Inbox":
                msges = msges.Where(e=>e.ReciverId == msgParams.UserId && !e.ReciverDeleted);
                break;
                case "Outbox":
                msges = msges.Where(e=>e.SenderId == msgParams.UserId && !e.SenderDeleted);
                break;
                default:
                msges = msges.Where(e=>e.ReciverId == msgParams.UserId && !e.IsRead && !e.ReciverDeleted);
                break;
            }

            msges = msges.OrderByDescending(e=>e.DateSent);

            return await PageList<Message>.CreateAsync(msges, msgParams.PageNumber, msgParams.pageSize);
        }
    }
}