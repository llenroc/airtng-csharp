﻿using System.Data.Entity;
using System.Threading.Tasks;

namespace AirTNG.Web.Models.Repository
{
    public interface IUsersRepository
    {
        Task<ApplicationUser> FindAsync(string id);
        Task<ApplicationUser> FindByPhoneNumberAsync(string phoneNumber);
    }

    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext _context;

        public UsersRepository()
        {
           _context = new ApplicationDbContext();
        }

        public async Task<ApplicationUser> FindAsync(string id)
        {
            return await _context.Users.FirstAsync(u => u.Id == id);
        }

        public async Task<ApplicationUser> FindByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Users.FirstAsync(u => u.PhoneNumber == phoneNumber);
        }
    }
}