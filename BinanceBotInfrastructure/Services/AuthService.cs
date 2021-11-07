using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceBotInfrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBinanceBotDbContext db;

        public const string issuer = "a";
        public const string audience = "a";
        public static readonly SymmetricSecurityKey securityKey = 
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes("супер секретный ключ для шифрования"));
        public const string algorithms = SecurityAlgorithms.HmacSha256;

        private static readonly TimeSpan expiresTimespan = TimeSpan.FromDays(365.25);
        private static readonly Encoding encoding = Encoding.UTF8;
        private const int PasswordSaltLength = 5;
        private const string claimIdUser = "id";
        private const string claimNameidCompany = "idCompany";
        private readonly HashAlgorithm hashAlgoritm;
        private readonly Random rnd;

        public AuthService(IBinanceBotDbContext db)
        {
            this.db = db;
            hashAlgoritm = SHA384.Create();
            rnd = new Random((int)(DateTime.Now.Ticks % 2147480161));
        }

        public async Task<UserTokenDto> LoginAsync(string login, string password,
            CancellationToken token = default)
        {
            var (identity, user) = await GetClaimsUserAsync(login, password, token)
                .ConfigureAwait(false);

            if (identity == default)
                return null;

            return new UserTokenDto
            {
                Id = user.Id,
                Name = user.Name,
                Login = user.Login,
                Patronymic = user.Patronymic,
                RoleName = user.Role.Caption,
                Surname = user.Surname,
                Token = MakeToken(identity.Claims),
            };
        }

        public string Refresh(ClaimsPrincipal user) =>
            MakeToken(user.Claims);

        public int Register(UserDto userDto)
        {
            if (userDto.Login is null || userDto.Login.Length is < 3 or > 50)
                return -1;

            if (userDto.Password is null || userDto.Password.Length is < 3 or > 50)
                return -2;
            
            if (userDto.Email?.Length > 255)
                return -3;

            var user = db.Users.FirstOrDefault(u => u.Login == userDto.Login);
            
            if(user is not null)
                return -6;

            var salt = GenerateSalt();

            var newUser = new User
            {
                IdRole = userDto.IdRole ?? 2, // simple user
                Name = userDto.Name,
                Surname = userDto.Surname,
                Patronymic = userDto.Patronymic,
                Email = userDto.Email,
                Login = userDto.Login,
                PasswordHash = salt + ComputeHash(salt, userDto.Password),
            };

            db.Users.Add(newUser);
            try
            {
                db.SaveChanges();
            }
            catch //(Exception ex)
            {
                return -7;
            }

            return 0;
        }

        public int ChangePassword(string userLogin, string newPassword)
        {
            var user = db.Users.AsNoTracking()
                .FirstOrDefault(u => u.Login == userLogin);
            
            if (user == null)
                return -1;

            var salt = GenerateSalt();
            user.PasswordHash = salt + ComputeHash(salt, newPassword);
            db.SaveChanges();
            return 0;
        }

        public int ChangePassword(int idUser, string newPassword)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == idUser);
            if (user == null)
                return -1;

            var salt = GenerateSalt();
            user.PasswordHash = salt + ComputeHash(salt, newPassword);
            db.SaveChanges();
            return 0;
        }

        private static string MakeToken(IEnumerable<Claim> claims)
        {
            var now = DateTime.Now;

            var jwt = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    notBefore: now,
                    claims: claims,
                    expires: now.Add(expiresTimespan),
                    signingCredentials: new SigningCredentials(securityKey, algorithms));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private async Task<(ClaimsIdentity Identity, User User)> GetClaimsUserAsync(string login,
            string password, CancellationToken token = default)
        {
            var user = await db
                .GetUsersByLogin(login)
                .AsNoTracking()
                .FirstOrDefaultAsync(token)
                .ConfigureAwait(false);

            if (user is null)
                return default;

            if (!CheckPassword(user.PasswordHash, password))
                return default;

            var claims = new List<Claim>
                {
                    new Claim(claimIdUser, $"{user.Id}"),
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Caption??"Guest")
                };
            var claimsIdentity = new ClaimsIdentity(claims, "Token", 
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return (claimsIdentity, user);
        }

        private bool CheckPassword(string passwordHash, string password)
        {
            if (passwordHash.Length == 0 && password.Length == 0)
                return true;

            if (passwordHash.Length < PasswordSaltLength)
                return false;

            var salt = passwordHash[0..PasswordSaltLength];
            var hashDb = passwordHash[PasswordSaltLength..];

            return hashDb == ComputeHash(salt, password);
        }

        private string ComputeHash(string salt, string password)
        {
            var hashBytes = hashAlgoritm.ComputeHash(encoding.GetBytes(salt + password));
            var hashString = BitConverter.ToString(hashBytes)
                    .Replace("-", "")
                    .ToLower();
            return hashString;
        }

        public string GenerateSalt()
        {
            const string saltChars = "sHwiaX7kZT1QRp0cPILGUuK2Sz=9q8lmejDNfoYCE3B_WtgyVv6M5OxAJ4Frbhnd";
            string salt = "";
            for (int i = 0; i < PasswordSaltLength - 1; i++)
                salt += saltChars[rnd.Next(0, saltChars.Length)];
            salt += "|";
            return salt;
        }
    }
}