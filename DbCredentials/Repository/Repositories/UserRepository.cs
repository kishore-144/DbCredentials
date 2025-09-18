using DbCredentials.Data;
using DbCredentials.DTOs;
using DbCredentials.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DbCredentials.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserRepository(AppDbContext db, IPasswordHasher<User> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        public async Task<ApiResponse> CreateUserAsync(SignupDto dto)
        {
            try
            {
                var user = new User
                {
                    firstName = dto.firstName,
                    middleName = dto.middleName,
                    lastName = dto.lastName,
                    email = dto.email,
                    phoneNumber = dto.phoneNumber,
                    dob = dto.dob,
                    password = _passwordHasher.HashPassword(null, dto.password),
                    createdDate = DateTime.UtcNow,
                    createdBy="sample"
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                user.createdBy = user.id.ToString();
                _db.Users.Update(user);
                await _db.SaveChangesAsync();

                return new ApiResponse
                {
                    status = "Success",
                    message = "Created Successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    status = "Failure",
                    message = $"Error creating user: {ex.Message}"
                };
            }
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            try
            {
                return await _db.Users.SingleOrDefaultAsync(u => u.email == email);
            }
            catch
            {
                return null; // controller can handle null
            }
        }

        public async Task<User> GetByIdAsync(int id)
        {
            try
            {
                return await _db.Users.FindAsync(id);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            try
            {
                return await _db.Users.AnyAsync(u => u.email == email);
            }
            catch
            {
                return false; // default safe
            }
        }

        public async Task<bool> VerifyPasswordAsync(User user, string password)
        {
            try
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.password, password);
                return result != PasswordVerificationResult.Failed;
            }
            catch
            {
                return false;
            }
        }
    }
}
