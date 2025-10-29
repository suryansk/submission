using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Attributes;
using WebApplication2.Data;
using WebApplication2.Models.Entities;

namespace WebApplication2.Controllers
{
    /// <summary>
    /// Account management endpoints - demonstrates 403 Forbidden for ViewOnly users
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Get all accounts (ViewOnly users can access this)
        /// </summary>
        /// <returns>List of all accounts</returns>
        /// <response code="200">Returns list of accounts</response>
        /// <response code="401">Unauthorized - token required</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _context.Accounts
                .Include(a => a.User)
                .Include(a => a.Bank)
                .Include(a => a.Currency)
                .Select(a => new
                {
                    a.Id,
                    a.AccountNumber,
                    a.Balance,
                    a.IsActive,
                    a.OpenedDate,
                    Owner = new { a.User.FirstName, a.User.LastName, a.User.Email },
                    Bank = a.Bank.Name,
                    Currency = a.Currency.Code,
                    AccountType = "Account" // Simplified for now
                })
                .ToListAsync();
                
            return Ok(new { success = true, data = accounts, count = accounts.Count });
        }
        
        /// <summary>
        /// Get user's own accounts (ViewOnly users can access this)
        /// </summary>
        /// <returns>Current user's accounts</returns>
        /// <response code="200">Returns user's accounts</response>
        /// <response code="401">Unauthorized - token required</response>
        [HttpGet("my-accounts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyAccounts()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest(new { success = false, message = "Invalid user ID in token" });
            }
            
            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId)
                .Include(a => a.Bank)
                .Include(a => a.Currency)
                .Select(a => new
                {
                    a.Id,
                    a.AccountNumber,
                    a.Balance,
                    a.IsActive,
                    a.OpenedDate,
                    Bank = a.Bank.Name,
                    Currency = a.Currency.Code,
                    AccountType = "Account" // Simplified for now
                })
                .ToListAsync();
                
            return Ok(new { success = true, data = accounts, count = accounts.Count });
        }
        
        /// <summary>
        /// Create new account (FORBIDDEN for ViewOnly users - 403)
        /// </summary>
        /// <param name="request">Account creation details</param>
        /// <returns>Created account information</returns>
        /// <response code="201">Account created successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">Unauthorized - token required</response>
        /// <response code="403">Forbidden - ViewOnly users cannot perform this action</response>
        [HttpPost]
        [RequireWriteAccess]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid request data", errors = ModelState });
            }
            
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest(new { success = false, message = "Invalid user ID in token" });
            }
            
            // Generate account number
            var accountNumber = $"ACC{DateTime.Now:yyyyMMdd}{new Random().Next(1000, 9999)}";
            
            var account = new SavingAccount
            {
                AccountNumber = accountNumber,
                UserId = userId,
                BankId = request.BankId,
                BranchId = request.BranchId,
                CurrencyId = request.CurrencyId,
                Balance = request.InitialDeposit,
                MinimumBalance = 1000, // Default minimum balance for savings
                OpenedDate = DateTime.UtcNow,
                IsActive = true
            };
            
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetMyAccounts), new { id = account.Id }, 
                new { success = true, message = "Account created successfully", accountId = account.Id, accountNumber = accountNumber });
        }
        
        /// <summary>
        /// Update account balance (FORBIDDEN for ViewOnly users - 403)
        /// </summary>
        /// <param name="id">Account ID</param>
        /// <param name="request">Balance update details</param>
        /// <returns>Update confirmation</returns>
        /// <response code="200">Balance updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">Unauthorized - token required</response>
        /// <response code="403">Forbidden - ViewOnly users cannot perform this action</response>
        /// <response code="404">Account not found</response>
        [HttpPut("{id}/balance")]
        [RequireWriteAccess]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBalance(int id, [FromBody] UpdateBalanceRequest request)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound(new { success = false, message = "Account not found" });
            }
            
            var oldBalance = account.Balance;
            account.Balance = request.NewBalance;
            
            await _context.SaveChangesAsync();
            
            return Ok(new 
            { 
                success = true, 
                message = "Balance updated successfully", 
                accountId = id,
                oldBalance = oldBalance,
                newBalance = request.NewBalance
            });
        }
        
        /// <summary>
        /// Close account (ADMIN ONLY - 403 for ViewOnly users and regular users)
        /// </summary>
        /// <param name="id">Account ID to close</param>
        /// <returns>Account closure confirmation</returns>
        /// <response code="200">Account closed successfully</response>
        /// <response code="401">Unauthorized - token required</response>
        /// <response code="403">Forbidden - Admin privileges required</response>
        /// <response code="404">Account not found</response>
        [HttpDelete("{id}")]
        [RequireAdmin]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CloseAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound(new { success = false, message = "Account not found" });
            }
            
            account.IsActive = false;
            account.ClosedDate = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            return Ok(new { success = true, message = "Account closed successfully", accountId = id });
        }
    }
    
    public class CreateAccountRequest
    {
        public int BankId { get; set; }
        public int BranchId { get; set; }
        public int CurrencyId { get; set; }
        public decimal InitialDeposit { get; set; }
    }
    
    public class UpdateBalanceRequest
    {
        public decimal NewBalance { get; set; }
    }
}