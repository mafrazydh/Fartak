using Fartak.DbModels;
using Fartak.Models.User;
using Fartak.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Fartak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly DataBase db;



        public AuthController(DataBase db, IOptions<JwtSettings> jwtSettings)
        {
            this.db = db;
            _jwtSettings = jwtSettings.Value;
        }




        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // ۱. اعتبارسنجی اطلاعات کاربر (مثلاً از دیتابیس)
            var user = ValidateUser(model.Email, model.Password);
            if (user == null)
            {
                return Unauthorized("نام کاربری یا رمز عبور اشتباه است.");
            }

            // ۲. تولید توکن JWT
            var token = GenerateJwtToken(user);
            // ۳. اضافه کردن توکن به کوکی
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true, // کوکی فقط از طریق HTTP قابل دسترسی است و از JavaScript قابل دسترسی نیست
                Secure = true,   // فقط در ارتباطات HTTPS ارسال می‌شود (در محیط تولید)
                SameSite = SameSiteMode.Strict, // جلوگیری از حملات CSRF
                Expires = DateTimeOffset.UtcNow.AddHours(1) // زمان انقضای کوکی (به دلخواه تنظیم کنید)
            });
            return Ok(new { token });
        }

        private User ValidateUser(string email, string password)
        {

            var user = db.Users
                              .FirstOrDefault(u => u.Email == email);
            if (user != null) {
                if (user.Email == email && user.Password == password)
                {
                    return user;
                }
                else 
                {
                    return null;           
                }
            }
            return null;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    // می‌توانید Claim های دیگری مانند نقش (Role) را نیز اضافه کنید.
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}
