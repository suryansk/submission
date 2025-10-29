using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication2.Data;
using WebApplication2.Models.DTOs;
using WebApplication2.Models.Entities;

namespace WebApplication2.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        
        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Bank)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Account)
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);
            
            if (user == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }
            
            var credential = await _context.UserCredentials.FirstOrDefaultAsync(c => c.UserId == user.Id);
            
            if (credential == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }
            
            if (credential.IsLocked && credential.LockedUntil > DateTime.UtcNow)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = $"Account is locked until {credential.LockedUntil.Value.ToLocalTime()}"
                };
            }
            
            if (!VerifyPassword(request.Password, credential.PasswordHash, credential.PasswordSalt))
            {
                credential.FailedLoginAttempts++;
                
                if (credential.FailedLoginAttempts >= 5)
                {
                    credential.IsLocked = true;
                    credential.LockedUntil = DateTime.UtcNow.AddMinutes(30);
                }
                
                await _context.SaveChangesAsync();
                
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }
            
            credential.FailedLoginAttempts = 0;
            credential.IsLocked = false;
            credential.LockedUntil = null;
            await _context.SaveChangesAsync();
            
            var roles = await GetUserRolesAsync(user);
            var token = GenerateJwtToken(user, roles);
            
            return new LoginResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                TokenExpiry = DateTime.UtcNow.AddHours(24),
                User = new UserInfo
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserType = user switch
                    {
                        NormalUser => "NormalUser",
                        ViewOnlyUser => "ViewOnlyUser",
                        Admin => "Admin",
                        SYSAdmin => "SYSAdmin",
                        _ => "Unknown"
                    },
                    IsMinor = user is NormalUser normalUser && normalUser.IsMinor
                },
                Roles = roles
            };
        }
        
        public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Email already exists"
                };
            }
            
            // Validate PrimaryBankId for ManagerUser if provided
            if (request.UserType?.ToLower() == "admin" && request.Department?.Length > 100)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Department name cannot exceed 100 characters."
                };
            }
            
            var (hash, salt) = HashPassword(request.Password);
            
            // Create user based on UserType
            User user = request.UserType?.ToLower() switch
            {
                "viewonlyuser" => new ViewOnlyUser
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
                },
                "admin" => new Admin
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
                    IsActive = true,
                    // Admin-specific fields
                    Department = request.Department,
                    Position = request.Position,
                    YearsOfExperience = request.YearsOfExperience,
                    LastAdminActionAt = DateTime.UtcNow
                },
                "sysadmin" => new SYSAdmin
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
                    IsActive = true,
                    // SYSAdmin-specific fields
                    Department = request.Department,
                    Position = request.Position,
                    YearsOfExperience = request.YearsOfExperience,
                    LastSystemActionAt = DateTime.UtcNow,
                    AdminLevel = request.AdminLevel ?? "Junior"
                },
                _ => new NormalUser
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
                }
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            var credential = new UserCredential
            {
                UserId = user.Id,
                PasswordHash = hash,
                PasswordSalt = salt,
                CreatedAt = DateTime.UtcNow,
                LastPasswordChange = DateTime.UtcNow,
                FailedLoginAttempts = 0,
                IsLocked = false
            };
            
            _context.UserCredentials.Add(credential);
            
            var accountHolderRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "ACCOUNT_HOLDER");
            if (accountHolderRole != null)
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = accountHolderRole.Id,
                    AssignedDate = DateTime.UtcNow,
                    IsActive = true
                };
                _context.UserRoles.Add(userRole);
            }
            
            await _context.SaveChangesAsync();
            
            return new LoginResponse
            {
                Success = true,
                Message = "Registration successful",
                User = new UserInfo
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserType = user switch
                    {
                        NormalUser => "NormalUser",
                        ViewOnlyUser => "ViewOnlyUser",
                        Admin => "Admin",
                        SYSAdmin => "SYSAdmin",
                        _ => "Unknown"
                    },
                    IsMinor = user is NormalUser normalUser && normalUser.IsMinor
                }
            };
        }
        
        private string GenerateJwtToken(User user, List<RoleInfo> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            
            var claims = new List<Claim>
            {
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("userType", user switch
                {
                    NormalUser => "NormalUser",
                    ViewOnlyUser => "ViewOnlyUser",
                    Admin => "Admin",
                    SYSAdmin => "SYSAdmin",
                    _ => "Unknown"
                })
            };
            
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
                
                if (role.BankId.HasValue)
                {
                    claims.Add(new Claim($"bank_{role.BankId}", role.RoleName));
                }
                
                foreach (var permission in role.Permissions)
                {
                    claims.Add(new Claim("permission", permission));
                }
            }
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        private async Task<List<RoleInfo>> GetUserRolesAsync(User user)
        {
            var roles = new List<RoleInfo>();
            
            foreach (var userRole in user.UserRoles.Where(ur => ur.IsActive))
            {
                var permissions = userRole.Role.RolePermissions
                    .Where(rp => rp.IsActive && rp.Permission.IsActive)
                    .Select(rp => rp.Permission.Name)
                    .ToList();
                
                roles.Add(new RoleInfo
                {
                    RoleName = userRole.Role.Name,
                    BankName = userRole.Bank?.Name,
                    BankId = userRole.BankId,
                    AccountNumber = userRole.Account?.AccountNumber,
                    Permissions = permissions
                });
            }
            
            return roles;
        }
        
        private (string hash, string salt) HashPassword(string password)
        {
            using var hmac = new HMACSHA512();
            var salt = Convert.ToBase64String(hmac.Key);
            var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return (hash, salt);
        }
        
        private bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            using var hmac = new HMACSHA512(saltBytes);
            var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return computedHash == storedHash;
        }
    }
}
