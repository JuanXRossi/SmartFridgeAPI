using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFridgeAPI.Dtos.Account;
using SmartFridgeAPI.Extensions;
using SmartFridgeAPI.Interfaces;
using SmartFridgeAPI.Models;

namespace SmartFridgeAPI.Controllers
{
    public class AccountController : ApiBaseController<AccountController>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        public AccountController(
            UserManager<User> userManager, 
            ITokenService tokenService, 
            SignInManager<User> signInManager, 
            ILogger<AccountController> logger)
        : base(logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null)
            {
                return Unauthorized("Nombre de usuario inválido");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized("Nombre de usuario no encontrado y/o contraseña incorrecta");
            }

            user.RefreshToken = _tokenService.CreateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(
                new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user, roles),
                    RefreshToken = user.RefreshToken
                }
            );
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshTokenDto.RefreshToken);

            if (user == null)
            {
                return Unauthorized("Usuario inválido.");
            }

            if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return Unauthorized("Inicio de sesión necesario.");
            }

            user.RefreshToken = _tokenService.CreateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(
                new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user, roles),
                    RefreshToken = user.RefreshToken
                }
            );
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var user = new User
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                };

                var createdUser = await _userManager.CreateAsync(user, registerDto.Password!);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "Member");

                    if (roleResult.Succeeded)
                    {
                        user.RefreshToken = _tokenService.CreateRefreshToken();
                        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                        await _userManager.UpdateAsync(user);

                        var roles = await _userManager.GetRolesAsync(user);

                        return Ok(
                            new NewUserDto
                            {
                                UserName = user.UserName,
                                Email = user.Email,
                                Token = _tokenService.CreateToken(user, roles),
                                RefreshToken = user.RefreshToken
                            }
                        );
                    } 
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            } catch(Exception e)
            {
                return StatusCode(500, e.InnerException?.Message ?? e.Message);
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.FindByNameAsync(User.GetUsername());

            if (user == null)
            {
                return Unauthorized("Usuario inválido");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);

            await _signInManager.SignOutAsync();
            return Ok("Sesión cerrada.");
        }
    }
}