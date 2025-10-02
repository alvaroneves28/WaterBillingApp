using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingWebAPI.Model;
using WaterBillingWebAPI.Model.DTO;

namespace WaterBillingWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AuthController(IConfiguration configuration, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _configuration = configuration;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null || !user.IsActive)
                return Unauthorized("Utilizador not found");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
                return Unauthorized("Invalid Credentials");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("FullName", user.FullName),
            };

            if (user.Customer != null)
            {
                claims.Add(new Claim("CustomerId", user.Customer.Id.ToString()));
            }

            var jwtKey = _configuration["JwtSettings:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new Exception("JWT Key not found. Check your configuration.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["JwtSettings:TokenExpiryMinutes"])
                ),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return Ok(new { message = "If the email is registered, a reset link has been sent." });
            }

            // Gerar token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Converter token para Base64 URL-safe
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            var tokenBase64 = Convert.ToBase64String(tokenBytes)
                .Replace('+', '-')   // substituir + por -
                .Replace('/', '_')   // substituir / por _
                .TrimEnd('=');       // remover padding

            // Criar deep link com token URL-safe
            var resetLink = $"waterbilling://reset-password?token={tokenBase64}&email={Uri.EscapeDataString(model.Email)}";

            // Email com link clicável
            var emailBody = $@"
<html>
<body style='font-family: Arial, sans-serif;'>
    <h2>Password Recovery</h2>
    <p>You requested to reset your password for your Water Billing account.</p>
    <p>Click the button below to reset your password in the app:</p>
    <p style='margin: 30px 0;'>
        <a href='{resetLink}' 
           style='background-color: #007bff; 
                  color: white; 
                  padding: 12px 30px; 
                  text-decoration: none; 
                  border-radius: 5px; 
                  display: inline-block;
                  font-weight: bold;'>
            Reset Password
        </a>
    </p>
    <p style='color: #666; font-size: 12px;'>
        If the button doesn't work, copy and paste this link:<br/>
        <span style='color: #007bff;'>{resetLink}</span>
    </p>
    <p style='color: #666; font-size: 12px;'>
        This link expires in 1 hour.
    </p>
    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'/>
    <p style='color: #999; font-size: 11px;'>
        If you didn't request this, please ignore this email.
    </p>
</body>
</html>
";

            await _emailSender.SendEmailAsync(
                model.Email,
                "Password Reset - Water Billing App",
                emailBody
            );

            return Ok(new { message = "If the email is registered, a reset link has been sent." });
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new { message = "Invalid request." });

            
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Password reset successful." });
        }





    }
}
