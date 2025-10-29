using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Attributes;
using WebApplication2.Data;
using WebApplication2.Models.DTOs;
using WebApplication2.Models.Entities;

namespace WebApplication2.Controllers
{
    /// <summary>
    /// User management endpoints - demonstrates 403 Forbidden for ViewOnly users
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Get all users (ViewOnly users can access this)
        /// </summary>
        /// <returns>List of all users</returns>
        /// <response code="200">Returns list of users</response>
        /// <response code="401">Unauthorized - token required</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.PhoneNumber,
                    u.IsActive,
                    UserType = "User", // Simplified for now
                    u.CreatedAt
                })
                .ToListAsync();
                
            return Ok(new { success = true, data = users, count = users.Count });
        }
        
        /// <summary>
        /// Get current user info (ViewOnly users can access this)
        /// </summary>
        /// <returns>Current user information</returns>
        /// <response code="200">Returns current user info</response>
        /// <response code="401">Unauthorized - token required</response>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest(new { success = false, message = "Invalid user ID in token" });
            }
            
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { success = false, message = "User not found" });
            }
            
            return Ok(new 
            { 
                success = true, 
                data = new 
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber,
                    user.DateOfBirth,
                    user.Address,
                    user.IsActive,
                    UserType = "User" // Simplified for now
                }
            });
        }
        
        /// <summary>
        /// Update user information (FORBIDDEN for ViewOnly users - 403)
        /// </summary>
        /// <param name="id">User ID to update</param>
        /// <param name="request">Updated user information</param>
        /// <returns>Update confirmation</returns>
        /// <response code="200">User updated successfully</response>
        /// <response code="401">Unauthorized - token required</response>
        /// <response code="403">Forbidden - ViewOnly users cannot perform this action</response>
        /// <response code="404">User not found</response>
        [HttpPut("{id}")]
        [RequireWriteAccess]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { success = false, message = "User not found" });
            }
            
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.Address = request.Address;
            user.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            return Ok(new { success = true, message = "User updated successfully", userId = id });
        }
        
        /// <summary>
        /// Deactivate user (ADMIN ONLY - sets IsActive to false)
        /// </summary>
        /// <param name="id">User ID to deactivate</param>
        /// <returns>Deactivation confirmation</returns>
        /// <response code="200">User deactivated successfully</response>
        /// <response code="401">Unauthorized - token required</response>
        /// <response code="403">Forbidden - Admin privileges required</response>
        /// <response code="404">User not found</response>
        [HttpPut("{id}/deactivate")]
        [RequireAdmin]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { success = false, message = "User not found" });
            }
            
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            return Ok(new { success = true, message = "User deactivated successfully", userId = id });
        }
        
        /// <summary>
        /// Delete user (ADMIN ONLY - 403 for ViewOnly users and regular users, only when IsActive is false)
        /// </summary>
        /// <param name="id">User ID to delete</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">User deleted successfully</response>
        /// <response code="400">User must be inactive before deletion</response>
        /// <response code="401">Unauthorized - token required</response>
        /// <response code="403">Forbidden - Admin privileges required</response>
        /// <response code="404">User not found</response>
        [HttpDelete("{id}")]
        [RequireAdmin]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { success = false, message = "User not found" });
            }
            
            // Only allow deletion if user is already inactive
            if (user.IsActive)
            {
                return BadRequest(new { success = false, message = "User must be inactive before deletion. Please deactivate the user first." });
            }
            
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            
            return Ok(new { success = true, message = "User deleted successfully", userId = id });
        }
        
        /// <summary>
        /// Create new user (ADMIN ONLY - 403 for ViewOnly users)
        /// </summary>
        /// <param name="request">New user information</param>
        /// <returns>Created user information</returns>
        /// <response code="201">User created successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">Unauthorized - token required</response>
        /// <response code="403">Forbidden - Admin privileges required</response>
        [HttpPost]
        [RequireAdmin]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid request data", errors = ModelState });
            }
            
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { success = false, message = "Email already exists" });
            }
            
            var user = new NormalUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                Address = request.Address,
                IdentificationNumber = request.IdentificationNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetCurrentUser), new { id = user.Id }, 
                new { success = true, message = "User created successfully", userId = user.Id });
        }
    }
}