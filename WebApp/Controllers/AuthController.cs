using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Common.DataBase;
using Common.DataBase.Entities;
using Common.Environment;
using Common.Helpers;
using WebApp.Models.User;
using WebApp.Helpers;
using WebApp.Exceptions;

namespace WebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IDbContext _dbContext;
        private readonly JwtSettings _jwtSettings;

        public AuthController(IDbContext dbContext, JwtSettings jwtSettings)
        {
            _dbContext = dbContext;
            _jwtSettings = jwtSettings;
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> Profile()
        {
            var user = await _dbContext.Users.Find(u => u.Id == HttpContext.User.Id()).Project(Builders<User>.Projection.Expression(u => new ProfileVm { 
                Id = u.Id,
                DisplayName = u.DisplayName,
                Email = u.Email
            }))
            .FirstOrDefaultAsync();

            return View(user);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterForm form)
        {
            new RegisterFormValidator().ValidateOrThrow(form);

            #region Validate register credentials and insert user to database 
            bool userExists = await _dbContext.Users.Find(u => u.Email == form.Email).AnyAsync();

            if (userExists)
                throw new ValidationException("Email is already in use by another user", form);

            var user = new User()
            {
                Email = form.Email,
                DisplayName = form.DisplayName,
                PasswordHash = PasswordHasher.HashPassword(form.Password),
                CreationDate = DateTime.UtcNow,
                IsEmailConfirmed = true
            };
            await _dbContext.Users.InsertOneAsync(user);
            #endregion 

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginForm form)
        {
            #region Get user from database and validate login credentials
            var projection = Builders<User>.Projection.Expression(u => new { u.Id, u.Email, u.PasswordHash, u.DisplayName, u.LockoutEndTime, u.IsEmailConfirmed, u.Role });
            var user = await _dbContext.Users.Find(u => u.Email == form.Email).Project(projection).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ValidationException("Username or password is incorrect", form);
            }

            if (!PasswordHasher.VerifyHashedPassword(user.PasswordHash, form.Password))
            {
                await _dbContext.Users.UpdateOneAsync(u => u.Id == user.Id, Builders<User>.Update.Inc(u => u.FailedLoginCount, 1));
                throw new ValidationException("Username or password is incorrect", form);
            }

            if (user.LockoutEndTime > DateTime.UtcNow)
            {
                throw new ValidationException("Your account has been locked until: " + user.LockoutEndTime.Date);
            }

            if (!user.IsEmailConfirmed)
            {
                throw new ValidationException("You must confirm your email to be able to login");
            }

            await _dbContext.Users.UpdateOneAsync(u => u.Id == user.Id, Builders<User>.Update.Set(u => u.FailedLoginCount, 0));
            #endregion

            #region Generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(Claims.Id, user.Id),
                new Claim(Claims.Email, user.Email),
                new Claim(Claims.DisplayName, user.DisplayName),
                new Claim(Claims.Role, user.Role.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.Lifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token); 
            #endregion

            HttpContext.Response.Cookies.Append(_jwtSettings.CookieName, tokenString);       

            return RedirectToAction("Explore", "Challenges");
        }

        [HttpGet]
        public IActionResult Logout()
        {            
            HttpContext.Response.Cookies.Delete(_jwtSettings.CookieName);
            return RedirectToAction("Index", "Home");
        }
    }
}
