using ApiAulaEntra21.Data;
using ApiAulaEntra21.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiAulaEntra21.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class LojaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LojaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_context.Loja.ToList());
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromRoute] int id, [FromBody] Loja updateLoja)
        {
            var loja = _context.Loja.FirstOrDefault(x => x.Id == id);

            if (loja is null)
            {
                return BadRequest("A loja não existe.");
            }

            loja.Nome = updateLoja.Nome;

            _context.Loja.Update(loja);
            _context.SaveChanges();

            return Ok(loja);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Loja newLoja)
        {
            if (string.IsNullOrEmpty(newLoja.Nome))
            {
                return BadRequest("O nome da loja é obrigatório.");
            }

            _context.Loja.Add(newLoja);
            _context.SaveChanges();

            return Created("/loja", newLoja);
        }
    }
}
