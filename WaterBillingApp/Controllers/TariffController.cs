using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Models;

namespace WaterBillingApp.Controllers
{
    public class TariffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TariffController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Exibir todas as tarifas
        public async Task<IActionResult> Index()
        {
            var tariffs = await _context.TariffBrackets.ToListAsync();
            return View(tariffs);
        }

        // Criar nova tarifa (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Criar nova tarifa (POST)
        [HttpPost]
        public async Task<IActionResult> Create(TariffBracket model)
        {
            if (ModelState.IsValid)
            {
                _context.TariffBrackets.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // Editar tarifa (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var tariff = await _context.TariffBrackets.FindAsync(id);
            if (tariff == null) return NotFound();
            return View(tariff);
        }

        // Editar tarifa (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(TariffBracket model)
        {
            if (ModelState.IsValid)
            {
                _context.TariffBrackets.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // Excluir tarifa
        public async Task<IActionResult> Delete(int id)
        {
            var tariff = await _context.TariffBrackets.FindAsync(id);
            if (tariff == null) return NotFound();

            _context.TariffBrackets.Remove(tariff);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
