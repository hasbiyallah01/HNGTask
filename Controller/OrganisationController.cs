using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stage2.Data.DTOs;
using Stage2.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Stage2.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        private readonly OrganisationService _organisationService;

        public OrganisationController(OrganisationService organisationService)
        {
            _organisationService = organisationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrganisations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    status = "Unauthorized",
                    message = "User not authorized"
                });
            }

            var organisations = await _organisationService.GetOrganisationsForUserAsync(userId);

            return Ok(new
            {
                status = "success",
                message = "Organisations retrieved successfully",
                data = new
                {
                    organisations = organisations.Select(o => new
                    {
                        o.OrgId,
                        o.name,
                        o.description
                    })
                }
            });
        }

        [HttpGet("{orgId}")]
        public async Task<IActionResult> GetOrganisation(string orgId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    status = "Unauthorized",
                    message = "User not authorized"
                });
            }

            var organisation = await _organisationService.GetOrganisationByIdAsync(orgId, userId);

            if (organisation == null)
            {
                return NotFound(new
                {
                    status = "Not Found",
                    message = "Organisation not found"
                });
            }

            return Ok(new
            {
                status = "success",
                message = "Organisation retrieved successfully",
                data = new
                {
                    organisation.OrgId,
                    organisation.name,
                    organisation.description
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrganisation(OrganisationCreationDTO organisationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    status = "Unauthorized",
                    message = "User not authorized"
                });
            }

            var organisation = await _organisationService.CreateOrganisationAsync(organisationDto, userId);
            if (organisation == null)
            {
                return StatusCode(400, new
                {
                    status = "Bad Request",
                    message = "Client error",
                    statusCode = 400
                });
            }

            return StatusCode(201, new
            {
                status = "success",
                message = "Organisation created successfully",
                data = new
                {
                    organisation.OrgId,
                    organisation.name,
                    organisation.description
                }
            });
        }

        [HttpPost("{orgId}/users")]
        public async Task<IActionResult> AddUserToOrganisation(string orgId, AddUserToOrganisationDTO addUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new
                {
                    status = "Unauthorized",
                    message = "User not authorized"
                });
            }

            try
            {
                await _organisationService.AddUserToOrganisationAsync(orgId, addUserDto.UserId, currentUserId);
                return Ok(new
                {
                    status = "success",
                    message = "User added to organisation successfully"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    status = "Bad Request",
                    message = "Could not add user to organisation"
                });
            }
        }
    }
}
