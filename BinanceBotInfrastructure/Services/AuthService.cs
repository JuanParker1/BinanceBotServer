using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotDb.Models;

namespace BinanceBotInfrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBinanceBotDbContext _db;

        public const string Issuer = "a";
        public const string Audience = "a";
        public static readonly SymmetricSecurityKey SecurityKey = 
            new (Encoding.ASCII.GetBytes("super secret encryption key"));
        
        private static string _algorithms = SecurityAlgorithms.HmacSha256;
        private static readonly TimeSpan _expiresTimespan = TimeSpan.FromDays(365.25);
        private readonly Encoding _encoding = Encoding.UTF8;
        private const int _passwordSaltLength = 5;
        private const string _claimIdUser = "id";
        private readonly HashAlgorithm _hashAlgoritm;
        private readonly Random _rnd;

        public AuthService(IBinanceBotDbContext db)
        {
            _db = db;
            _hashAlgoritm = SHA384.Create();
            _rnd = new Random((int)(DateTime.Now.Ticks % 2147480161));
        }

        public async Task<AuthUserInfoDto> LoginAsync(string login, string password,
            CancellationToken token = default)
        {
            var (identity, user) = await GetClaimsUserAsync(login, password, token);

            if (identity == default)
                return null;

            return new AuthUserInfoDto
            {
                Id = user.Id,
                Name = user.Name,
                Login = user.Login,
                RoleName = user.Role.Caption,
                Surname = user.Surname,
                Token = MakeToken(identity.Claims),
            };
        }

        public string Refresh(ClaimsPrincipal user) =>
            MakeToken(user.Claims);

        public async Task<bool> RegisterAsync(RegisterDto registerDto, CancellationToken token)
        {
            var user = _db.Users.FirstOrDefault(u => u.Login == registerDto.Login);
            
            if(user is not null)
                return false;

            var salt = GenerateSalt();

            var newUser = new User
            {
                IdRole = registerDto.IdRole ?? 2, // simple user
                DateCreated = DateTime.Now,
                Name = registerDto.Name,
                Surname = registerDto.Surname,
                Email = registerDto.Email,
                Login = registerDto.Login,
                Password = salt + ComputeHash(salt, registerDto.Password),
            };

            _db.Users.Add(newUser);

            await _db.SaveChangesAsync(token);

            return true;
        }

        public async Task<int> ChangePasswordAsync(string userLogin, 
            string newPassword, CancellationToken token)
        {
            var user = await _db.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Login == userLogin, token);;
            
            if (user == null)
                return -1;

            var salt = GenerateSalt();
            user.Password = salt + ComputeHash(salt, newPassword);
            
            return await _db.SaveChangesAsync(token);
        }

        public async Task<int> ChangePasswordAsync(int idUser, string newPassword,
            CancellationToken token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == idUser,
                token);
            
            if (user == null)
                return -1;

            var salt = GenerateSalt();
            user.Password = salt + ComputeHash(salt, newPassword);
            return await _db.SaveChangesAsync(token);
        }

        private static string MakeToken(IEnumerable<Claim> claims)
        {
            var now = DateTime.Now;

            var jwt = new JwtSecurityToken(
                    issuer: Issuer,
                    audience: Audience,
                    notBefore: now,
                    claims: claims,
                    expires: now.Add(_expiresTimespan),
                    signingCredentials: new SigningCredentials(SecurityKey, _algorithms));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private async Task<(ClaimsIdentity Identity, User User)> GetClaimsUserAsync(string login,
            string password, CancellationToken token = default)
        {
            var user = await _db
                .GetUserByLogin(login)
                .AsNoTracking()
                .FirstOrDefaultAsync(token);

            if (user is null)
                return default;

            if (!CheckPassword(user.Password, password))
                return default;

            var claims = new List<Claim>
                {
                    new Claim(_claimIdUser, $"{user.Id}"),
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Caption??"Guest")
                };
            var claimsIdentity = new ClaimsIdentity(claims, "Token", 
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            
            return (claimsIdentity, user);
        }

        private bool CheckPassword(string passwordHash, string password)
        {
            switch (passwordHash.Length)
            {
                case 0 when password.Length == 0:
                    return true;
                case < _passwordSaltLength:
                    return false;
            }

            var salt = passwordHash[0.._passwordSaltLength];
            var hashDb = passwordHash[_passwordSaltLength..];

            return hashDb == ComputeHash(salt, password);
        }

        private string ComputeHash(string salt, string password)
        {
            var hashBytes = _hashAlgoritm.ComputeHash(_encoding.GetBytes(salt + password));
            var hashString = BitConverter.ToString(hashBytes)
                    .Replace("-", "")
                    .ToLower();
            
            return hashString;
        }

        private string GenerateSalt()
        {
            const string saltChars = "sHwiaX7kZT1QRp0cPILGUuK2Sz=9q8lmejDNfoYCE3B_WtgyVv6M5OxAJ4Frbhnd";
            string salt = "";
            for (int i = 0; i < _passwordSaltLength; i++) //TODO: Вернуть соль, замешаную в сам пароль
                salt += saltChars[_rnd.Next(0, saltChars.Length)];

            return salt;
        }
    }
}