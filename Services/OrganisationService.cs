using Stage2.Data.DTOs;
using Stage2.Data;
using Stage2.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Stage2.Services
{
    public class OrganisationService
    {
        private readonly ApplicationDbContext _context;

        public OrganisationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Organisation>> GetOrganisationsForUserAsync(string userId)
        {
            return await _context.Organisations
                .Where(o => o.Users.Any(u => u.UserId == userId))
                .ToListAsync();
        }

        public async Task<Organisation> GetOrganisationByIdAsync(string orgId, string userId)
        {
            var organisation = await _context.Organisations
                .Include(o => o.Users)
                .FirstOrDefaultAsync(o => o.OrgId == orgId);

            if (organisation == null || !organisation.Users.Any(u => u.UserId == userId))
            {
                return null;
            }
            return organisation;
        }

        public async Task<Organisation> CreateOrganisationAsync(OrganisationCreationDTO organisationDTO, string userId)
        {
            if (string.IsNullOrWhiteSpace(organisationDTO.name))
            {
                throw new ValidationException("Organisation name is required.");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var organisation = new Organisation
            {
                name = organisationDTO.name,
                description = organisationDTO.description
            };

            organisation.Users.Add(user);

            await _context.Organisations.AddAsync(organisation);
            await _context.SaveChangesAsync();

            return organisation;
        }

        public async Task AddUserToOrganisationAsync(string orgId, string userIdToAdd, string currentUserId)
        {
            var organisation = await _context.Organisations
                .Include(o => o.Users)
                .FirstOrDefaultAsync(o => o.OrgId == orgId);

            if (organisation == null)
            {
                throw new Exception("Organisation not found.");
            }

            if (!organisation.Users.Any(u => u.UserId == currentUserId))
            {
                throw new UnauthorizedAccessException("You don't have permission to add users to this organisation.");
            }

            var userToAdd = await _context.Users.FindAsync(userIdToAdd);
            if (userToAdd == null)
            {
                throw new Exception("User to add not found.");
            }

            if (organisation.Users.Any(u => u.UserId == userIdToAdd))
            {
                throw new Exception("User is already a member of this organisation.");
            }

            organisation.Users.Add(userToAdd);
            await _context.SaveChangesAsync();
        }
    }
}
