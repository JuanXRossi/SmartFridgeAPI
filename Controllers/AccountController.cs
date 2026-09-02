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
        private readonly IAccountRepository _accountRepository;
        public AccountController(
            UserManager<User> userManager, 
            ITokenService tokenService, 
            SignInManager<User> signInManager, 
            ILogger<AccountController> logger,
            IAccountRepository accountRepository)
        : base(logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _accountRepository = accountRepository;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var user = await _userManager.FindByNameAsync(User.GetUsername());

            if (user == null)
                return StatusCode(404, new { success = false, message = "Usuario no encontrado" });

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserPublicInfoDto
            {
                UserName = user.UserName!,
                Name = user.Name!,
                Email = user.Email!,
                Roles = roles.ToList()
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null)
            {
                return StatusCode(401, new { success = false, message = "Nombre de usuario inválido" });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return StatusCode(401, new { success = false, message = "Nombre de usuario no encontrado y/o contraseña incorrecta" });
            }

            if (!user.EmailConfirmed)
                return StatusCode(403, new { success = false, message = "Debés confirmar tu correo antes de iniciar sesión.", code = "EMAIL_NOT_CONFIRMED" });

            user.RefreshToken = _tokenService.CreateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(
                new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList(),
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
                return StatusCode(401, new { success = false, message = "Usuario inválido" });
            }

            if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return StatusCode(401, new { success = false, message = "Inicio de sesión necesario" });
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
            var existingUserName = await _userManager.FindByNameAsync(registerDto.Username!);
            
            if (existingUserName != null)
                return StatusCode(400, new { success = false, message = "El nombre de usuario ya está en uso" });

            var existingEmail = await _userManager.FindByEmailAsync(registerDto.Email!);
            
            if (existingEmail != null)
                return StatusCode(400, new { success = false, message = "El email ya está en uso" });
            
            var user = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                Name = registerDto.Name,
            };

            var result = await _accountRepository.RegisterUserAsync(user, registerDto);

            if (!result.Succeeded)
                return result.IsServerError 
                    ? StatusCode(500, new { success = false, message = result.ErrorMessage })
                    : StatusCode(400, new { success = false, message = result.ErrorMessage });

            var emailResult = await _accountRepository.SendConfirmationEmailAsync(user);

            return Ok(new
            {
                success = true,
                message = emailResult.Succeeded
                    ? "Cuenta creada. Revisá tu correo para confirmar tu cuenta."
                    : "Cuenta creada, pero no pudimos enviar el correo de confirmación. Podés solicitar un reenvío."
            });
        }

        [Authorize]
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UpdateDto updateDto)
        {
            var user = await _userManager.FindByNameAsync(User.GetUsername());

            if (user == null)
                return StatusCode(404, new { success = false, message = "Usuario no encontrado" });

            if (!string.Equals(user.UserName, updateDto.Username, StringComparison.OrdinalIgnoreCase))
            {
                var existingUser = await _userManager.FindByNameAsync(updateDto.Username!);
                if (existingUser != null)
                    return StatusCode(400, new { success = false, message = "El nombre de usuario ya está en uso" });
            }

            if (!string.Equals(user.Email, updateDto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingUser = await _userManager.FindByEmailAsync(updateDto.Email!);
                if (existingUser != null)
                    return StatusCode(400, new { success = false, message = "El email ya está en uso" });
            }

            var result = await _accountRepository.UpdateUserAsync(user, updateDto);

            if (!result.Succeeded)
                return result.IsServerError 
                    ? StatusCode(500, new { success = false, message = result.ErrorMessage })
                    : StatusCode(400, new { success = false, message = result.ErrorMessage });

            user.RefreshToken = _tokenService.CreateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new NewUserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList(),
                Token = _tokenService.CreateToken(user, roles),
                RefreshToken = user.RefreshToken
            });
        }

        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if(user == null)
                return StatusCode(404, new { success = false, message = "No existe un usuario asociado al mail provisto" });

            if(user.EmailConfirmed)
                return StatusCode(400, new { success = false, message = "El mail provisto ya fue confirmado" });
            
            await _accountRepository.SendConfirmationEmailAsync(user);

            return Ok(new { success = true, message = "Se te ha enviado un nuevo enlace para confirmar el mail" });
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
        {
            var result = await _accountRepository.ConfirmEmailAsync(dto.UserId, dto.Token);

            if (!result.Succeeded)
                return StatusCode(400, new { success = false, message = result.ErrorMessage });

            return Ok(new { success = true, message = "Correo confirmado. Ya podés iniciar sesión." });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return StatusCode(404, new { success = false, message = "No existe un usuario asociado al mail provisto" });

            if (user.EmailConfirmed)
                await _accountRepository.SendPasswordResetEmailAsync(user);

            return Ok(new { success = true, message = "Se te enviaron instrucciones para restablecer tu contraseña" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _accountRepository.ResetPasswordAsync(dto.UserId, dto.Token, dto.NewPassword);

            if (!result.Succeeded)
                return StatusCode(400, new { success = false, message = result.ErrorMessage });

            return Ok(new { success = true, message = "Contraseña actualizada. Ya podés iniciar sesión." });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.FindByNameAsync(User.GetUsername());

            if (user == null)
            {
                return StatusCode(401, new { success = false, message = "Usuario inválido" });
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);

            await _signInManager.SignOutAsync();
            return Ok("Sesión cerrada.");
        }
    }
}