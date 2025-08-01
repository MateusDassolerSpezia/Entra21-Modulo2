using ApiAulaEntra21.Data;
using ApiAulaEntra21.Models;
using ApiAulaEntra21.Models.Dto;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiAulaEntra21.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public LoginController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginDto login)
        {
            Usuario? usuario = _context.Usuario.FirstOrDefault(x => x.Email == login.Email);

            if (usuario is null || BCrypt.Net.BCrypt.Verify(login.Senha, usuario.SenhaHash))
            {
                return Unauthorized("Usuário e/ou senha inválidos.");
            }

            string token = GerarTokenJWT(usuario);

            return Ok(new { token });
        }

        [NonAction]
        public string GerarTokenJWT(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, usuario.Role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:chave"] ?? "chave"));
            var credencial = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: credencial
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
