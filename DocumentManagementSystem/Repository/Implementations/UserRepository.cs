using DocumentManagementSystem.Models;
using DocumentManagementSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DocumentManagementSystem.Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        

        User IUserRepository.Get(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        void IUserRepository.Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return;
        }
        void IUserRepository.Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
            return;
        }

       

        User IUserRepository.GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }
        User IUserRepository.GetWithDocuments(int id)
        {
            // this will give us the use with the id - 'id' and include the documents related to that and then give the catogory of that.
            return _context.Users
                .Include(u => u.Documents)
                    .ThenInclude(d => d.Category)
                .FirstOrDefault(u => u.Id == id);
        }
    }
}
