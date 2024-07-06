using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stage2.Data;
using Stage2.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;

namespace Stage2.Services
{
    public class UserService
    {

        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            // Validate user
            if (string.IsNullOrWhiteSpace(user.firstName) || string.IsNullOrWhiteSpace(user.lastName) ||
                string.IsNullOrWhiteSpace(user.email) || string.IsNullOrWhiteSpace(user.password))
            {
                throw new ValidationException("All required fields must be provided.");
            }

            // Check if email is unique
            if (await _context.Users.AnyAsync(u => u.email == user.email))
            {
                throw new ValidationException("Email already exists.");
            }

            // Hash password
            user.password = _passwordHasher.HashPassword(user, user.password);

            // Create default organisation
            var organisation = new Organisation
            {
                name = $"{user.firstName}'s Organisation",
                description = $"Default organisation for {user.firstName} {user.lastName}"
            };

            user.Organisations.Add(organisation);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> AuthenticateUserAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == email);

            if (user == null)
            {
                throw new AuthenticationException("Invalid email or password.");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.password, password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new AuthenticationException("Invalid email or password.");
            }

            return user;
        }

        public async Task<User> GetUserByIdAsync(string id, string currentUserId)
        {
            var user = await _context.Users
                .Include(u => u.Organisations)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return null;
            }

            // Check if the current user has access to this user's information
            if (id != currentUserId && !user.Organisations.Any(o => o.Users.Any(u => u.UserId == currentUserId)))
            {
                throw new UnauthorizedAccessException("You don't have permission to access this user's information.");
            }

            return user;
        }
    }
}
